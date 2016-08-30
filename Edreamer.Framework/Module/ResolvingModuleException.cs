using System;

namespace Edreamer.Framework.Module
{
    /// <summary>
    /// Defines an exception to be thrown when a problem occurs while resolving a module.
    /// </summary>
    public class ResolvingModuleException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="ResolvingModuleException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ResolvingModuleException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
