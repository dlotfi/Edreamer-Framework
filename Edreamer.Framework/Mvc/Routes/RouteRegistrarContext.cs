// Based on ASP.Net Mvc3 source code (AreaRegistrationContext.cs & RouteCollectionExtensions.cs)

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Routes
{
    public class RouteRegistrarContext
    {
        public const int DefaultRoutePriority = 0;

        public RouteRegistrarContext(string areaName)
        {
            Throw.IfArgumentNullOrEmpty(areaName, "areaName");

            RouteDescriptors = new List<RouteDescriptor>();
            AreaName = areaName;
        }

        public string AreaName { get; private set; }

        public ICollection<RouteDescriptor> RouteDescriptors { get; private set; }

        /// <summary>
        /// Ignores the specified URL route for the given list of available routes.
        /// </summary>
        /// <param name="url">The URL pattern for the route to ignore.</param>
        /// <param name="priority">The priority of this routing rule. The default value is <see cref="DefaultRoutePriority"/>.</param>
        public void IgnoreRoute(string url, int priority = DefaultRoutePriority)
        {
            IgnoreRoute(url, null, priority);
        }

        /// <summary>
        /// Ignores the specified URL route for the given list of the available routes
        /// and a list of constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route to ignore.</param>
        /// <param name="constraints">A set of expressions that specify values for the url parameter.</param> 
        /// <param name="priority">The priority of this routing rule. The default value is <see cref="DefaultRoutePriority"/>.</param>
        public void IgnoreRoute(string url, object constraints, int priority = DefaultRoutePriority)
        {
            Throw.IfArgumentNull(url, "url");

            var route = new IgnoreRouteInternal(url)
            {
                Constraints = new RouteValueDictionary(constraints)
            };

            RouteDescriptors.Add(new RouteDescriptor
                                      {
                                          Name = "",
                                          Priority = priority,
                                          Route = route
                                      });
        }


        #region Unnamed Routes
        /// <summary>
        /// Maps the specified URL route.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="priority">The priority of this routing rule. The default value is <see cref="DefaultRoutePriority"/>.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute(string url, int priority = DefaultRoutePriority)
        {
            return MapRouteInternal(null, url, null, null, priority);
        }

        /// <summary>
        /// Maps the specified URL route.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="priority">The priority of this routing rule. The default value is <see cref="DefaultRoutePriority"/>.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute(string url, object defaults, int priority = DefaultRoutePriority)
        {
            return MapRouteInternal(null, url, defaults, null, priority);
        }

        /// <summary>
        /// Maps the specified URL route and sets default route values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
        /// <param name="priority">The priority of this routing rule. The default value is <see cref="DefaultRoutePriority"/>.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute(string url, object defaults, object constraints, int priority = DefaultRoutePriority)
        {
            return MapRouteInternal(null, url, defaults, constraints, priority);
        }

        /// <summary>
        /// Maps the specified URL route and sets default route values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
        /// <param name="routeCanonicalizer">An instance of <see cref="IRouteCanonicalizer"/> that canonicalize route url.</param>
        /// <param name="priority">The priority of this routing rule. The default value is <see cref="DefaultRoutePriority"/>.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute(string url, object defaults, object constraints, IRouteCanonicalizer routeCanonicalizer, int priority = DefaultRoutePriority)
        {
            return MapRouteInternal(null, url, defaults, constraints, priority, routeCanonicalizer);
        }

        /// <summary>
        /// Adds a route to the end of the <see cref="RouteRegistrarContext"/> object
        /// and assigns the specified name and priority to the route.
        /// </summary>
        /// <param name="item">
        /// The route to add to the end of the collection. If area is not specified the default area of the
        /// current <see cref="RouteRegistrarContext"/> is used.
        /// </param>
        /// <param name="priority">The priority of this routing rule. The default value is <see cref="DefaultRoutePriority"/>.</param>
        public void Add(RouteBase item, int priority = DefaultRoutePriority)
        {
            AddInternal(null, item, priority);
        }
        #endregion

        #region Named Routes
        /// <summary>
        /// Maps the specified URL route.
        /// </summary>
        /// <typeparam name="T">Type of named route object used to identify a route</typeparam>
        /// <param name="url">The URL pattern for the route.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute<T>(string url)
            where T: class, IAnyNamedRoute
        {
            return MapRouteInternal<T>(url, null, null);
        }

        /// <summary>
        /// Maps the specified URL route.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute<T>(string url, object defaults)
            where T : class, IAnyNamedRoute
        {
            return MapRouteInternal<T>(url, defaults, null);
        }

        /// <summary>
        /// Maps the specified URL route and sets default route values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute<T>(string url, object defaults, object constraints)
            where T : class, IAnyNamedRoute
        {
            return MapRouteInternal<T>(url, defaults, constraints);
        }

        /// <summary>
        /// Maps the specified URL route and sets default route values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
        /// <param name="routeCanonicalizer">An instance of <see cref="IRouteCanonicalizer"/> that canonicalize route url.</param>
        /// <returns>A reference to the mapped route.</returns>
        public Route MapRoute<T>(string url, object defaults, object constraints, IRouteCanonicalizer routeCanonicalizer)
            where T : class, IAnyNamedRoute
        {
            return MapRouteInternal<T>(url, defaults, constraints, routeCanonicalizer);
        }

        /// <summary>
        /// Adds a route to the end of the <see cref="RouteRegistrarContext"/> object
        /// and assigns the specified name and priority to the route.
        /// </summary>
        /// <param name="item">
        /// The route to add to the end of the collection. If area is not specified the default area of the
        /// current <see cref="RouteRegistrarContext"/> is used.
        /// </param>
        public void Add<T>(RouteBase item)
            where T : class, IAnyNamedRoute
        {
            AddInternal<T>(item);
        }
        #endregion

        protected Route MapRouteInternal<T>(string url, object defaults, object constraints, IRouteCanonicalizer routeCanonicalizer = null)
            where T : class, IAnyNamedRoute
        {
            Throw.IfArgumentNull(url, "url");
            var route = routeCanonicalizer == null
                ? new Route(url, new MvcRouteHandler())
                : new CanonicalizedRoute(url, new MvcRouteHandler(), routeCanonicalizer);
            route.Defaults = new RouteValueDictionary(defaults);
            route.Constraints = new RouteValueDictionary(constraints);
            route.DataTokens = new RouteValueDictionary();
            route.DataTokens["area"] = AreaName;
            AddInternal<T>(route);
            return route;
        }

        protected void AddInternal<T>(RouteBase item)
            where T : class, IAnyNamedRoute
        {
            var name = typeof(T).FullName;
            var priority = typeof(T).GetCustomAttributes(typeof(PartPriorityAttribute), false)
                               .OfType<PartPriorityAttribute>().Select(a => (int?)a.Priority).SingleOrDefault()
                           ?? DefaultRoutePriority;
            RouteDescriptors.Add(new RouteDescriptor
            {
                Name = name,
                Priority = priority,
                Route = item
            });
        }

        protected Route MapRouteInternal(string name, string url, object defaults, object constraints, int priority, IRouteCanonicalizer routeCanonicalizer = null)
        {
            Throw.IfArgumentNull(url, "url");
            var route = routeCanonicalizer == null
                ? new Route(url, new MvcRouteHandler())
                : new CanonicalizedRoute(url, new MvcRouteHandler(), routeCanonicalizer);
            route.Defaults = new RouteValueDictionary(defaults);
            route.Constraints = new RouteValueDictionary(constraints);
            route.DataTokens = new RouteValueDictionary();
            route.DataTokens["area"] = AreaName;
            AddInternal(name, route, priority);
            return route;
        }

        protected void AddInternal(string name, RouteBase item, int priority)
        {
            RouteDescriptors.Add(new RouteDescriptor
            {
                Name = name,
                Priority = priority,
                Route = item
            });
        }

        // ToDo-OnDemand [12071818]: Add other overloads of MapRoute that take namespaces as a parameter.

        private sealed class IgnoreRouteInternal : Route
        {
            public IgnoreRouteInternal(string url)
                : base(url, new StopRoutingHandler())
            {
            }

            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues)
            {
                // Never match during route generation. This avoids the scenario where an IgnoreRoute with
                // fairly relaxed constraints ends up eagerly matching all generated URLs.
                return null;
            }
        }
    }
}
