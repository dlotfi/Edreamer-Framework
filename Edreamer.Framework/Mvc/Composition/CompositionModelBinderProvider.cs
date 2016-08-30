// Based on the original work of Maarten Balliauw, published as part of MefContrib

using System;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// CompositionModelBinderProvider
    /// </summary>
    public class CompositionModelBinderProvider : IModelBinderProvider
    {
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionModelBinderProvider"/> class.
        /// </summary>
        /// <param name="compositionContainerAccessor">The composition container accessor.</param>
        public CompositionModelBinderProvider(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        public IModelBinder GetBinder(Type modelType)
        {
            Throw.IfArgumentNull(modelType, "modelType");
            var modelBinder = _compositionContainerAccessor.Container
                .GetExports<IModelBinder, IBinderMetadata>()
                .FirstOrDefault(b => b.Metadata.ModelTypes.Contains(modelType));
            if (modelBinder != null)
            {
                return modelBinder.Value;
            }
            return null;
        }
    }
}
