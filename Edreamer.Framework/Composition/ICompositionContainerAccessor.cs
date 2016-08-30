namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// ICompositionContainerAccessor
    /// </summary>
    public interface ICompositionContainerAccessor
    {
        /// <summary> 
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        ICompositionContainer Container { get; }
    }
}