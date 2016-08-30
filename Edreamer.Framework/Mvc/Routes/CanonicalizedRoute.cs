using System;
using System.Web;
using System.Web.Routing;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Routes
{
    public class CanonicalizedRoute : Route
    {
        private readonly IRouteCanonicalizer _routeCanonicalizer;
        public const string NoCanonicalizeFlag = "DoNotCanonicalize";

        public CanonicalizedRoute(string url, IRouteHandler routeHandler, IRouteCanonicalizer routeCanonicalizer) 
            : this(url, null, null, null, routeHandler, routeCanonicalizer)
        {
        }

        public CanonicalizedRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler, IRouteCanonicalizer routeCanonicalizer)
            : this(url, defaults, null, null, routeHandler,  routeCanonicalizer)
        {
        }

        public CanonicalizedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler, IRouteCanonicalizer routeCanonicalizer)
            : this(url, defaults, constraints, null, routeHandler,  routeCanonicalizer)
        {
        }

        public CanonicalizedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler, IRouteCanonicalizer routeCanonicalizer) 
            : base(url, defaults, constraints, dataTokens, null)
        {
            RouteHandler = new CanonicalizedRouteHandler(routeCanonicalizer, routeHandler, base.GetVirtualPath);
            Throw.IfArgumentNull(routeCanonicalizer, "routeCanonicalizer");
            _routeCanonicalizer = routeCanonicalizer;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var noCanonicalizing = values.ContainsKey(NoCanonicalizeFlag) && ((bool?)values[NoCanonicalizeFlag] == true);
            if (noCanonicalizing)
                values.Remove(NoCanonicalizeFlag); // It's just a signal and should not be considered in routing

            var virtualPathData = base.GetVirtualPath(requestContext, values);
            if (virtualPathData == null || noCanonicalizing) return virtualPathData;

            var canonicalRouteValues = _routeCanonicalizer.GetCanonicalRouteValues(requestContext, values, RouteDirection.UrlGeneration);
            return canonicalRouteValues != null 
                ? base.GetVirtualPath(requestContext, canonicalRouteValues)
                : virtualPathData;
        }

        private class CanonicalizedRouteHandler : IRouteHandler
        {
            private readonly IRouteCanonicalizer _routeCanonicalizer;
            private readonly IRouteHandler _defalutHandler;
            private readonly Func<RequestContext, RouteValueDictionary, VirtualPathData> _getVirtualPath;

            public CanonicalizedRouteHandler(IRouteCanonicalizer routeCanonicalizer, IRouteHandler defalutHandler, Func<RequestContext, RouteValueDictionary, VirtualPathData> getVirtualPath)
            {
                if (getVirtualPath == null) throw new ArgumentNullException(nameof(getVirtualPath));
                _routeCanonicalizer = routeCanonicalizer;
                _defalutHandler = defalutHandler;
                _getVirtualPath = getVirtualPath;
            }

            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                if (requestContext.HttpContext.Request.HttpMethod == "GET")
                {
                    var canonicalRouteValues = _routeCanonicalizer.GetCanonicalRouteValues(requestContext, requestContext.RouteData.Values, RouteDirection.IncomingRequest);
                    if (canonicalRouteValues != null)
                    {
                        var query = requestContext?.HttpContext?.Request?.Url?.Query;
                        var pathData = _getVirtualPath(requestContext, canonicalRouteValues);
                        var canonicalUrl = "/" + pathData.VirtualPath + query;
                        return new PermanentRedirectHttpHandler(canonicalUrl);
                    }
                }
                return _defalutHandler.GetHttpHandler(requestContext);
            }

            private class PermanentRedirectHttpHandler : IHttpHandler
            {
                private readonly string _targetUrl;

                public PermanentRedirectHttpHandler(string targetUrl)
                {
                    _targetUrl = targetUrl;
                }

                public bool IsReusable => true;

                public void ProcessRequest(HttpContext context)
                {
                    context.Response.RedirectPermanent(_targetUrl);
                }
            }
        }
    }
}