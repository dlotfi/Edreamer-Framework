using System;

namespace Edreamer.Framework.Mvc.Templates
{
    /// <summary>
    /// Defines an exception to be thrown when a problem occurs in template registration.
    /// </summary>
    public class TemplateRegistrationException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="TemplateRegistrationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TemplateRegistrationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="TemplateRegistrationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public TemplateRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
