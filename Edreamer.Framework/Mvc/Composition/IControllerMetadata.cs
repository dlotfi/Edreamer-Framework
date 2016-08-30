using System;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// Defines the required contract for implementing controller metadata.
    /// </summary>
    public interface IControllerMetadata : IModuleMetadata
    {
        #region Properties
        /// <summary>
        /// Gets the type of the controller.
        /// </summary>
        Type ControllerType { get; }
        #endregion
    }
}