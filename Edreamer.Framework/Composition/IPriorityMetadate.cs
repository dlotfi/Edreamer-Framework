namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// Defines the required contract for implementing priority metadata.
    /// </summary>
    public interface IPriorityMetadata
    {
        #region Properties
        /// <summary>
        /// Gets the priority of the exported part
        /// </summary>
        int Priority { get; }
        #endregion
    }
}
