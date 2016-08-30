// Based on the original work of Maarten Balliauw, published as part of MefContrib

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// CompositionFilterAttributeFilterProvider
    /// </summary>
    public class CompositionFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        /// <summary>
        /// The dependency builder.
        /// </summary>
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionFilterAttributeFilterProvider"/> class.
        /// </summary>
        /// <param name="compositionContainerAccessor">The composition container accessor.</param>
        public CompositionFilterAttributeFilterProvider(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        /// <summary>
        /// Gets the controller attributes.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The filters defined by attributes</returns>
        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor).ToList();
            foreach (var attribute in attributes)
            {
                _compositionContainerAccessor.Container.SatisfyImportsOnce(attribute);
            }

            return attributes;
        }

        /// <summary>
        /// Gets the action attributes.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The filters defined by attributes.</returns>
        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor).ToList();
            foreach (var attribute in attributes)
            {
                _compositionContainerAccessor.Container.SatisfyImportsOnce(attribute);
            }

            return attributes;
        }
    }
}
