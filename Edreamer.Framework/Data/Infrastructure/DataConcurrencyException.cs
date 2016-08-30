using System;

namespace Edreamer.Framework.Data.Infrastructure
{
    /// <summary>
    /// Defines an exception to be thrown when a concurrency problem occurs in data operations.
    /// </summary>
    public class DataConcurrencyException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="DataConcurrencyException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="DataConcurrencyException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DataConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
