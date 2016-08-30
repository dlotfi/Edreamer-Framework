using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;

namespace Edreamer.Framework.Mvc.Routes
{
    public class RoutesPublisher : IRoutesPublisher
    {
        private readonly IModuleManager _moduleManager;
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;
        
        public RoutesPublisher(IModuleManager moduleManager, ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(moduleManager, "moduleManager");
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _moduleManager = moduleManager;
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        public void Publish(RouteCollection routeCollection, IEnumerable<IRouteRegistrar> routeRegistrars)
        {
            Throw.IfArgumentNull(routeCollection, "routeCollection");
            Throw.IfArgumentNull(routeRegistrars, "routeRegistrars");

            var routeDescriptors = new List<RegistrarRouteDescriptor>();
            int rgistrarIndex = 0;
            foreach (var routeRegistrar in routeRegistrars)
            {
                var moduleName = _moduleManager.GetModule(routeRegistrar.GetType()).Name;

                var context = new RouteRegistrarContext(moduleName);
                routeRegistrar.RegisterRoutes(context);
                routeDescriptors.AddRange(context.RouteDescriptors.Select(r => new RegistrarRouteDescriptor(rgistrarIndex, moduleName, r)));
                rgistrarIndex++;
            }

            var routeDescriptorsList = routeDescriptors
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.RegistrarIndex)
                .ToList();

            foreach (var routeDescriptor in routeDescriptorsList)
            {
                routeCollection.Add(routeDescriptor.Name, new RouteWrapper(routeDescriptor.ModuleName, routeDescriptor.Route, _compositionContainerAccessor));
            }

            // Add routes for action webparts in all modules
            foreach (var moduleName in _moduleManager.Modules.Select(m => m.Name))
            {
                var route = new Route("webparts/" + moduleName + "/{controller}/{action}", new MvcRouteHandler()) { DataTokens = new RouteValueDictionary() };
                route.DataTokens["area"] = moduleName;
                routeCollection.Add(null, new RouteWrapper(moduleName, route, _compositionContainerAccessor));
            }
        }

        private class RegistrarRouteDescriptor : RouteDescriptor
        {
            public RegistrarRouteDescriptor(int registrarIndex, string moduleName, RouteDescriptor routeDescriptor)
            {
                Priority = routeDescriptor.Priority;
                Name = routeDescriptor.Name;
                Route = routeDescriptor.Route;
                RegistrarIndex = registrarIndex;
                ModuleName = moduleName;
            }

            public int RegistrarIndex { get; set; }
            public string ModuleName { get; set; }
        }
    }

    // Based on Orchard CMS
    internal class RouteWrapper : RouteBase, IRouteWithArea
    {
        private readonly RouteBase _route;
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        public RouteWrapper(string defaultArea, RouteBase route, ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(route, "route");
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            Area = route.GetAreaName() ?? defaultArea;
            Throw.IfNull(Area).AnArgumentException("Area should be specified by either route or defaultArea.");
            _route = route;
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = _route.GetRouteData(httpContext);
            if (routeData == null)
                return null;

            routeData.DataTokens[MvcConstants.ContainerAccessorKey] = _compositionContainerAccessor;
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return _route.GetVirtualPath(requestContext, values);
        }

        public string Area { get; private set; }
    }
}
