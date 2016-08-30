using System;
using System.Web.Mvc;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Mvc.Routes;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class UrlExtensions
    {
        public static T Route<T>(this UrlHelper urlHelper)
            where T : IAnyNamedRoute
        {
            var route = urlHelper.GetContainer().GetExportedValue<T>();
            return route;
        }

        public static string ConvertToAbsoluteUrl(this UrlHelper urlHelper, string relativeUrl)
        {
            Throw.IfArgumentNull(urlHelper, "urlHelper");
            Throw.IfArgumentNull(relativeUrl, "relativeUrl");
            var request = urlHelper.RequestContext.HttpContext.Request;
            Throw.IfNull(request).A<InvalidOperationException>("Cannot create absolute url when current request is empty.");
            return "{0}://{1}{2}".FormatWith(request.Url.Scheme, request.Headers["Host"], relativeUrl);
        }
    }
}
