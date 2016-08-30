using System.Web.Routing;

namespace Edreamer.Framework.Mvc.Routes
{
    public interface IRouteCanonicalizer
    {
        RouteValueDictionary GetCanonicalRouteValues(RequestContext requestContext, RouteValueDictionary values, RouteDirection routeDirection);
    }
}