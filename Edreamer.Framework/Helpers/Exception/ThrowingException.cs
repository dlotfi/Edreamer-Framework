// Based on Matthew Abbott's article "MVC3 and MEF" - http://www.fidelitydesign.net/?p=259

using System;

namespace Edreamer.Framework.Helpers
{
    /// <summary>
    /// Defines an exception to be thrown when an intended exception could not be created.
    /// </summary>
    public sealed class ThrowingException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="ThrowingException"/>.
        /// </summary>
        /// <param name="exception">The exception that was intended to be thrown.</param>
        /// <param name="exceptionMessage">The original message for the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        internal ThrowingException(Type exception, string exceptionMessage, string message)
            : base(message)
        {
            ExceptionType = exception;
            ExceptionMessage = exceptionMessage;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the intended exception type to be thrown.
        /// </summary>
        public Type ExceptionType { get; private set; }
        /// <summary>
        /// Gets the original message for the exception.
        /// </summary>
        public string ExceptionMessage { get; private set; }
        #endregion
    }
}
