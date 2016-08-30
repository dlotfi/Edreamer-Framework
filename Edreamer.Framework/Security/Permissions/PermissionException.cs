using System;

namespace Edreamer.Framework.Security.Permissions
{
    /// <summary>
    /// Defines an exception to be thrown when a problem occurs in permission operations
    /// </summary>
    public class PermissionException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="PermissionException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PermissionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="PermissionException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public PermissionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
