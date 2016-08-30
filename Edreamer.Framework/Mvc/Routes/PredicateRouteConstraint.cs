using System.Web;
using System.Web.Routing;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Routes
{
    public delegate bool RouteConstraintMatch(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection);

    public class PredicateRouteConstraint: IRouteConstraint
    {
        private readonly RouteConstraintMatch _predicate;

        public PredicateRouteConstraint(RouteConstraintMatch predicate)
        {
            Throw.IfArgumentNull(predicate, "predicate");
            _predicate = predicate;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return _predicate(httpContext, route, parameterName, values, routeDirection);
        }
    }
}
