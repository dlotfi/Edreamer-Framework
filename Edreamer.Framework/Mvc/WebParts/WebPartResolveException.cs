using System;

namespace Edreamer.Framework.Mvc.WebParts
{
    /// <summary>
    /// Defines an exception to be thrown when a problem occurs in webpart resolution.
    /// </summary>
    public class WebPartResolveException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="WebPartResolveException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WebPartResolveException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WebPartResolveException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public WebPartResolveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
