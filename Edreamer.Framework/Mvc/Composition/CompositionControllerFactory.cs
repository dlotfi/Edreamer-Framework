// Based on the original work of Maarten Balliauw, published as part of MefContrib

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Composition
{
    public class CompositionControllerFactory : DefaultControllerFactory
    {
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        public CompositionControllerFactory(IControllerActivator activator, ICompositionContainerAccessor compositionContainerAccessor)
            : base(activator)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");

            _compositionContainerAccessor = compositionContainerAccessor;
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Throw.IfArgumentNull(requestContext, "requestContext");
            Throw.IfArgumentNull(controllerName, "controllerName");
            Throw.IfNull(requestContext.RouteData.DataTokens["area"] as string)
                .A<InvalidOperationException>("The request contains no area information." +
                                              "The Controller factory can't find the proper controller within modules without module/area name");
            Type controllerType;
            var controllerTypes = _compositionContainerAccessor.Container.GetExports<IController, IControllerMetadata>()
                    .Where(e =>
                           e.Metadata.ControllerType.Name.EqualsIgnoreCase(controllerName + "controller") &&
                           e.Metadata.ModuleName.EqualsIgnoreCase(requestContext.RouteData.DataTokens["area"] as string))
                    .Select(e => e.Metadata.ControllerType).ToList();

            if (controllerTypes.Count == 1)
            {
                controllerType = controllerTypes.First();
            }
            else
            {
                throw CreateAmbiguousControllerException(requestContext.RouteData.Route, controllerName, controllerTypes);
            }

            return controllerType;
        }

        /// <summary>
        /// Creates the ambiguous controller exception.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="matchingTypes">The matching types.</param>
        /// <returns>The exception to be thrown.</returns>
        private static InvalidOperationException CreateAmbiguousControllerException(RouteBase route, string controllerName, IEnumerable<Type> matchingTypes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var current in matchingTypes)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(current.FullName);
            }
            var route2 = route as Route;
            string message;
            if (route2 != null)
            {
                message = "The request for '{0}' has found the following matching controllers:{1}\r\n\r\nMultiple types were found that match the controller named '{2}'. This can happen if the route that services this request does not specify namespaces to search for a controller that matches the request. If this is the case, register this route by calling an overload of the 'MapRoute' method that takes a 'namespaces' parameter."
                    .FormatWith(route2.Url, stringBuilder, controllerName);
            }
            else
            {
                message = "The request for '{0}' has found the following matching controllers:{1}\r\n\r\nMultiple types were found that match the controller named '{0}'."
                    .FormatWith(controllerName, stringBuilder);
            }
            return new InvalidOperationException(message);
        }

    }
}