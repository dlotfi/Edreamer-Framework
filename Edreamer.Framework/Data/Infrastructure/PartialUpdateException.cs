using System;

namespace Edreamer.Framework.Data.Infrastructure
{
    public class PartialUpdateException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialUpdateException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PartialUpdateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="PartialUpdateException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="entity">The entity which has been marked for partial update.</param>
        public PartialUpdateException(string message, object entity)
            : base(message)
        {
            Entity = entity;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="PartialUpdateException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <param name="entity">The entity which has been marked for partial update.</param>
        public PartialUpdateException(string message, Exception innerException, object entity)
            : base(message, innerException)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entity which has been marked for partial update.
        /// </summary>
        public object Entity { get; private set; }
        #endregion
    }
}
