using System;

namespace Edreamer.Framework.UI.Resources
{
    /// <summary>
    /// Defines an exception to be thrown when a duplicate resource is registered.
    /// </summary>
    public class ResourceDuplicateException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="ResourceDuplicateException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ResourceDuplicateException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
