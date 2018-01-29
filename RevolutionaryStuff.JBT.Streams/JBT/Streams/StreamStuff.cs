using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace RevolutionaryStuff.JBT.Streams
{
    public static class StreamStuff
    {
        /// <summary>
        /// Seek via the Position member
        /// </summary>
        /// <remarks>
        /// Used for implemeters of a stream
        /// </remarks>
        /// <param name="st">The stream</param>
        /// <param name="offset">The offset</param>
        /// <param name="origin">The origin</param>
        /// <returns>The new position</returns>
        public static long SeekViaPos(Stream st, long offset, SeekOrigin origin)
        {
            long p;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    p = offset;
                    break;
                case SeekOrigin.Current:
                    p = st.Position + offset;
                    break;
                case SeekOrigin.End:
                    p = st.Length + offset;
                    break;
                default:
                    throw new ArgumentException("origin is invalid", "origin");
            }
            st.Position = p;
            return p;
        }
    }
}

