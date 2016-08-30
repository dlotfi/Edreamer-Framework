using System;
namespace Edreamer.Framework.Bootstrapping
{
    /// <summary>
    /// Defines the required contract for implementing bootstrapper task metadata.
    /// </summary>
    public interface IBootstrapperTaskMetadata
    {
        #region Properties
        /// <summary>
        /// Gets the type of bootstrapper used to resolve dependencies
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Indicates if this task is part of framework bootstarpping process.
        /// </summary>
        bool IsPartOfFramework { get; }
        #endregion
    }
}