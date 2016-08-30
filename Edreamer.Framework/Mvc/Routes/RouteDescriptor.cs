// Based on Orchard CMS

using System.Web.Routing;

namespace Edreamer.Framework.Mvc.Routes
{
    public class RouteDescriptor
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public RouteBase Route { get; set; }
    }
}