using System;
using System.Diagnostics;
using System.IO;

namespace RevolutionaryStuff.JBT.Streams
{
    /// <summary>
    /// Allows a stream to be sub-divided into independent logical streams with varying access rights
    /// </summary>
    public class StreamMuxer : BaseDisposable
    {
        /// <summary>
        /// The underlying stream
        /// </summary>
        /// <remarks>
        /// While tempting to make this public, doing so is dangerous
        /// as the outside world could easily access the member's without 
        /// using the appropriate locks, which would kill us in multithreaded
        /// situations
        /// </remarks>
        protected Stream Inner
        {
            get
            {
                if (this.IsDisposed) throw new ObjectDisposedException("StreamMuxer");
                return this.Inner_p;
            }
        }
        private readonly Stream Inner_p;

        /// <summary>
        /// When true, we should leave the inner stream open when the muxer
        /// is either closed or disposed.
        /// </summary>
        private readonly bool LeaveOpen;

        #region Constructors

        public StreamMuxer(Stream inner)
            : this(inner, false)
        { }

        public StreamMuxer(Stream inner, bool leaveOpen)
        {
            Validate.StreamArg(inner, "inner", false, false, true);
            this.Inner_p = inner;
            this.LeaveOpen = leaveOpen;
        }

        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);
            this.Inner_p.Flush();
            if (!this.LeaveOpen)
            {
                this.Inner_p.Close();
                this.Inner_p.Dispose();
            }
        }

        #endregion

        #region Quickies...

        /// <summary>
        /// The length of the underlying stream
        /// </summary>
        public long Length
        {
            get
            {
                lock (Inner)
                {
                    return Inner.Length;
                }
            }
        }

        /// <summary>
        /// Flush the contents of the underlying stream
        /// </summary>
        public void Flush()
        {
            lock (Inner)
            {
                this.Inner.Flush();
            }
        }

        /// <summary>
        /// Write to a section of the underlying stream
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <param name="offsetInFile">The place in the current stream to where we should begin to write data</param>
        public void Write(byte[] buffer, int offset, int count, long offsetInFile)
        {
            Validate.ArrayArg(buffer, offset, count);
            if (!Inner.CanWrite) throw new NotSupportedException();
            lock (Inner)
            {
                Inner.Position = offsetInFile;
                Inner.Write(buffer, offset, count);
            }
        }

        #endregion

        #region Sub-Stream Creation

        /// <summary>
        /// Create a new stream the full size of the underlying stream
        /// With read/write access
        /// </summary>
        /// <returns>A stream</returns>
        public Stream Create()
        {
            return Create(true, true);
        }

        /// <summary>
        /// Create a new stream the full size of the underlying stream with specified access rights
        /// </summary>
        /// <param name="canRead">When true, the new stream has read access</param>
        /// <param name="canWrite">When true, the new stream has write access</param>
        /// <returns>A Stream</returns>
        public Stream Create(bool canRead, bool canWrite)
        {
            return Create(canRead, canWrite, 0, -1);
        }

        /// <summary>
        /// Create a new stream that is a subset of the underlying stream with the specified access rights
        /// </summary>
        /// <param name="canRead">When true, the new stream has read access</param>
        /// <param name="canWrite">When true, the new stream has write access</param>
        /// <param name="offset">The offset into the original stream to use as the base for this new stream</param>
        /// <param name="size">The size of this new stream, -1 if it should be adjusted according to the size of the underlying stream</param>
        /// <returns>A Stream</returns>
        public Stream Create(bool canRead, bool canWrite, long offset, long size)
        {
            return new MyStream(this, canRead, canWrite, offset, size);
        }

        #endregion

        /// <summary>
        /// Our sub-stream
        /// </summary>
        private class MyStream : Stream
        {
            /// <summary>
            /// The parent muxer
            /// </summary>
            private readonly StreamMuxer Muxer;
            /// <summary>
            /// The muxer's stream
            /// </summary>
            private readonly Stream Inner;
            /// <summary>
            /// The offset into the parent "Inner" stream which serves as our base
            /// </summary>
            private readonly long Offset;
            /// <summary>
            /// The size of this stream.  When -1, this is the determined by the underlying stream
            /// </summary>
            private readonly long Size;
            /// <summary>
            /// Have we been closed
            /// </summary>
            private bool IsClosed;

            #region Constructors

            /// <summary>
            /// Construct a new stream
            /// </summary>
            /// <param name="muxer">The muxer</param>
            /// <param name="canRead">When true, the caller can read from this new stream</param>
            /// <param name="canWrite">When true, the caller can write to this new stream</param>
            /// <param name="offset">The offset into the parent stream to use as a base</param>
            /// <param name="size">The size of this new stream, -1 if determined by the parent</param>
            public MyStream(StreamMuxer muxer, bool canRead, bool canWrite, long offset, long size)
            {
                Validate.NonNullArg(muxer, "muxer");

                this.Muxer = muxer;
                this.Inner = muxer.Inner;
                Validate.Between(offset, "offset", 0, Inner.Length);
                if (size != -1)
                {
                    Validate.Between(size, "size", 0, Inner.Length - offset + 1);
                }
                this.Offset = offset;
                this.Size = size;
                this.CanRead_p = canRead;
                this.CanWrite_p = canWrite;
            }

            #endregion

            #region Stream Overrides

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                this.IsClosed = true;
            }

            public override bool CanRead
            {
                [DebuggerStepThrough]
                get { return !this.IsClosed && CanRead_p && Muxer.Inner.CanRead; }
            }
            private readonly bool CanRead_p;

            public override bool CanWrite
            {
                [DebuggerStepThrough]
                get { return !this.IsClosed && CanWrite_p && Muxer.Inner.CanWrite; }
            }
            private readonly bool CanWrite_p;

            public override bool CanSeek
            {
                [DebuggerStepThrough]
                get { return !this.IsClosed; }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return StreamStuff.SeekViaPos(this, offset, origin);
            }

            public override void SetLength(long value)
            {
                if (!CanWrite) throw new InvalidOperationException();
                if (this.Size != -1 || this.Offset != 0) throw new InvalidOperationException();
                Inner.SetLength(value);
            }

            public override long Length
            {
                get
                {
                    if (this.IsClosed) throw new NotNowException();
                    if (this.Size == -1)
                    {
                        return Inner.Length - this.Offset;
                    }
                    return this.Size;
                }
            }

            public override long Position
            {
                get
                {
                    if (this.IsClosed) throw new NotNowException();
                    return Position_p;
                }
                set
                {
                    if (this.IsClosed) throw new NotNowException();
                    try
                    {
                        Validate.Between(value, "value", 0, this.Length);
                    }
                    catch (Exception ex)
                    {
                        //we rethrow so we can support the accepted Stream exception conventions
                        throw new NotSupportedException("New Position is past the acceptable bounds", ex);
                    }
                    Position_p = value;
                }
            }
            private long Position_p;

            public override void Close()
            {
                base.Close();
                this.IsClosed = true;
            }

            public override void Flush()
            {
                if (this.IsClosed) throw new NotNowException();
                this.Muxer.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (!this.CanRead) throw new NotNowException();
                Validate.ArrayArg(buffer, offset, count);
                count = (int)Math.Min(count, Length - Position);
                lock (Inner)
                {
                    Inner.Position = this.Offset + Position;
                    int read = Inner.Read(buffer, offset, count);
                    this.Position += read;
                    return read;
                }
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (!this.CanWrite) throw new ReadOnlyException();
                this.Muxer.Write(buffer, offset, count, this.Position);
                this.Position += count;
            }

            #endregion
        }
    }
}
