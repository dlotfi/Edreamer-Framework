using System;

namespace Edreamer.Framework.Mvc
{
    /// <summary>
    /// Defines an exception to be thrown when a key or path duplication occurs in a selector.
    /// </summary>
    public class SelectorDuplicateKeyException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="SelectorDuplicateKeyException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SelectorDuplicateKeyException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
