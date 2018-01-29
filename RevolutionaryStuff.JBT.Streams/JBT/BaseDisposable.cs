using System;
using System.Diagnostics;

namespace RevolutionaryStuff.JBT
{
    /// <summary>
    /// This class provides an abstract implementation of IDisposable
    /// That makes it easier for subclasses to handle dispose
    /// </summary>
    public abstract class BaseDisposable : IDisposable
    {
        #region Constructors

        protected BaseDisposable()
        {
        }

        ~BaseDisposable()
        {
            MyDispose(false);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            MyDispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Returns true if dispose has been called
        /// </summary>
        protected bool IsDisposed
        {
            [DebuggerStepThrough]
            get { return IsDisposed_p > 0; }
        }
        private int IsDisposed_p;

        private void MyDispose(bool disposing)
        {
            if (1 != System.Threading.Interlocked.Increment(ref IsDisposed_p)) return;
            OnDispose(disposing);
        }

        /// <summary>
        /// Override this function to handle calls to dispose.
        /// This will only get called once
        /// </summary>
        /// <param name="disposing">True when the object is being disposed, 
        /// false if it is being destructed</param>
        protected virtual void OnDispose(bool disposing)
        { }
    }
}
