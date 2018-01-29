using System;

namespace RevolutionaryStuff.JBT
{
    /// <summary>
    /// Summary description for JbtException.
    /// </summary>
    public class JbtException : Exception
    {
        #region Constructors
        public JbtException()
        { }
        public JbtException(string message)
            : base(message)
        { }
        public JbtException(string message, Exception inner)
            : base(message, inner)
        { }
        #endregion
    }

    public class TestCodeException : JbtException
    {
        #region Constructors
        public TestCodeException()
        { }
        public TestCodeException(string message)
            : base(message)
        { }
        public TestCodeException(string message, Exception inner)
            : base(message, inner)
        { }

        #endregion
    }

    public class SingleCallException : JbtException
    {
        #region Constructors
        public SingleCallException()
        { }
        public SingleCallException(string message)
            : base(message)
        { }
        public SingleCallException(string message, Exception inner)
            : base(message, inner)
        { }

        #endregion
    }

    public class NotNowException : JbtException
    {
        #region Constructors
        public NotNowException()
        { }
        public NotNowException(string message)
            : base(message)
        { }
        public NotNowException(string message, Exception inner)
            : base(message, inner)
        { }

        #endregion
    }

    public class ReadOnlyException : NotNowException
    {
        #region Constructors
        public ReadOnlyException()
        { }
        public ReadOnlyException(string message)
            : base(message)
        { }
        public ReadOnlyException(string message, Exception inner)
            : base(message, inner)
        { }
        #endregion
    }

    public class InvalidTransitionException : NotNowException
    {
        #region Constructors
        public InvalidTransitionException()
        { }
        public InvalidTransitionException(string from, string to)
            : this(String.Format("Cannot transition from {0} to {1}", from, to))
        { }
        public InvalidTransitionException(string message)
            : base(message)
        { }
        public InvalidTransitionException(string message, Exception inner)
            : base(message, inner)
        { }

        #endregion
    }

    public class CallOrderException : NotNowException
    {
        #region Constructors
        public CallOrderException()
        { }
        public CallOrderException(string message)
            : base(message)
        { }
        public CallOrderException(string message, Exception inner)
            : base(message, inner)
        { }

        #endregion
    }

    public class MustOverrideException : JbtException
    {
        #region Constructors
        public MustOverrideException()
        { }
        public MustOverrideException(string message)
            : base(message)
        { }
        public MustOverrideException(string message, Exception inner)
            : base(message, inner)
        { }

        #endregion
    }
}
