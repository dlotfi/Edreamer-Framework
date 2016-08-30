using System;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// IModelBinderMetaData
    /// </summary>
    public interface IBinderMetadata
    {
        /// <summary>
        /// Gets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        Type[] ModelTypes { get; }
    }
}
