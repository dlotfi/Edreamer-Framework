// Based on the original work of Maarten Balliauw, published as part of MefContrib

using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// CompositionControllerActivator
    /// </summary>
    public class CompositionControllerActivator : IControllerActivator
    {
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionControllerActivator"/> class.
        /// </summary>
        /// <param name="compositionContainerAccessor">The composition container accessor.</param>
        public CompositionControllerActivator(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        /// <summary>
        /// Creates a controller.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerType">The controller type.</param>
        /// <returns>The created controller.</returns>
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            var controller = _compositionContainerAccessor.Container
                .GetExports(controllerType, null, null)
                .Select(c => c.Value)
                .FirstOrDefault() as IController;
                
            if (controller is Controller)
            {
                (controller as Controller).ActionInvoker = _compositionContainerAccessor.Container.GetExportedValue<IActionInvoker>();
            }
            return controller;
        } 
    }
}
