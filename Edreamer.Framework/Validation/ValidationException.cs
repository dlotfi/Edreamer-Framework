using System;
using System.Collections.Generic;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Validation
{
    public class ValidationException : Exception
    {
        public IEnumerable<ValidationResult> ValidationResults { get; private set; }

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="ValidationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ValidationException(string message)
            : this(message, null, null)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ValidationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ValidationException(string message, Exception innerException)
            : this(message, innerException, null)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ValidationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <param name="validationResults">The list of validation results.</param>
        public ValidationException(string message, Exception innerException,
            IEnumerable<ValidationResult> validationResults)
            : base(message, innerException)
        {
            ValidationResults = CollectionHelpers.EmptyIfNull(validationResults);
        }
        #endregion
    }
}
