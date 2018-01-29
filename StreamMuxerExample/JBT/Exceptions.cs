using System;
using System.Runtime.Serialization;

namespace RevolutionaryStuff.JBT
{
    /// <summary>
    /// Summary description for JbtException.
    /// </summary>
    [Serializable]
    public class JbtException : ApplicationException
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
        protected JbtException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    [Serializable]
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
        protected TestCodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    [Serializable]
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
        protected SingleCallException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    [Serializable]
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
        protected NotNowException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    [Serializable]
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
        protected ReadOnlyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    [Serializable]
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
        protected InvalidTransitionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    [Serializable]
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
        protected CallOrderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    [Serializable]
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
        protected MustOverrideException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }
}
