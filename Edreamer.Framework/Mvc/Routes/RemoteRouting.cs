using System;
using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Mvc.Routes
{
    public class RemoteRoutingController : Controller
    {
        [Ajax, HttpGet]
        public JsonResult GetRouteUrl(string routeName, object routeData)
        {
            if (String.IsNullOrEmpty(routeName))
                return Json(new { success = false, message = "Route name is not specified.", url = "" }, JsonRequestBehavior.AllowGet);
            var url = Url.RouteUrl(routeName, routeData);
            if (url == null)
                return Json(new { success = false, message = "Route not found.", url = "" }, JsonRequestBehavior.AllowGet);
            return Json(new { success = true, message = "", url = url }, JsonRequestBehavior.AllowGet);
        }
    }

    public class RemoteRouteRegistrar : IRouteRegistrar
    {
        private readonly string _getRouteUrl;

        public RemoteRouteRegistrar(ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            _getRouteUrl = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Routing", Name = "GetRouteUrl" });
        }

        public void RegisterRoutes(RouteRegistrarContext context)
        {
            context.MapRoute(_getRouteUrl, new { Controller = "RemoteRouting", Action = "GetRouteUrl" },
                             new { httpMethod = new HttpMethodConstraint("GET") });
        }
    }
}
