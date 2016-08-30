using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edreamer.Framework.Data.Infrastructure
{
    /// <summary>
    /// Defines an exception to be thrown when a problem occurs while trying to remove an entity by its keys.
    /// </summary>
    public class RemoveByKeyException : Exception
    {
        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="RemoveByKeyException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RemoveByKeyException(string message)
            : base(message)
        {
        }

        #endregion
    }
}
