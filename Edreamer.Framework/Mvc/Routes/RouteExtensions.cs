using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Routes
{
    public static class RouteExtensions
    {
        public static string GetAreaName(this RouteBase route)
        {
            Throw.IfArgumentNull(route, "route");

            var routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
            {
                return routeWithArea.Area;
            }

            var castRoute = route as Route;
            if (castRoute != null && castRoute.DataTokens != null)
            {
                return castRoute.DataTokens["area"] as string;
            }

            return null;
        }
    }
}
