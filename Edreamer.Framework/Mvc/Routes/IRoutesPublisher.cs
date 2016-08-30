using System.Collections.Generic;
using System.Web.Routing;

namespace Edreamer.Framework.Mvc.Routes
{
    public interface IRoutesPublisher
    {
        
        void Publish(RouteCollection routeCollection, IEnumerable<IRouteRegistrar> routeRegistrars);
    }
}
