namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// Defines the required contract for implementing module metadata.
    /// </summary>
    public interface IModuleMetadata
    {
        #region Properties
        /// <summary>
        /// Gets the name of the module in which the exported part is defined
        /// </summary>
        string ModuleName { get; }
        #endregion
    }
}