using System;

namespace Edreamer.Framework.Injection
{
    /// <summary>
    /// Defines an exception to be thrown when an error occured durring injection
    /// </summary>
    public class InjectionException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="InjectionException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InjectionException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
