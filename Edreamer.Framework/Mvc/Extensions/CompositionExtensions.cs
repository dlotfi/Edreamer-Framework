using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class CompositionExtensions
    {
        public static ICompositionContainer GetContainer(this Controller controller)
        {
            if (controller == null) return null;
            return controller.ControllerContext.GetContainer();
        }

        public static ICompositionContainer GetContainer(this ControllerContext controllerContext)
        {
            if (controllerContext == null) return null;
            return controllerContext.RequestContext.GetContainer();
        }

        public static ICompositionContainer GetContainer(this HtmlHelper htmlHelper)
        {
            if (htmlHelper == null) return null;
            return htmlHelper.ViewContext.GetContainer();
        }

        public static ICompositionContainer GetContainer(this UrlHelper urlHelper)
        {
            if (urlHelper == null) return null;
            return urlHelper.RequestContext.GetContainer();
        }

        public static ICompositionContainer GetContainer(this RequestContext requestContext)
        {
            if (requestContext == null)
                return null;

            var routeData = requestContext.RouteData;
            if (routeData == null || routeData.DataTokens == null)
                return null;

            object compositionContainerAccessorValue;
            if (!routeData.DataTokens.TryGetValue(MvcConstants.ContainerAccessorKey, out compositionContainerAccessorValue))
            {
                compositionContainerAccessorValue = FindWorkContextInParent(routeData);
            }

            if (!(compositionContainerAccessorValue is ICompositionContainerAccessor))
                return null;

            var compositionContainerAccessor = (ICompositionContainerAccessor)compositionContainerAccessorValue;
            return compositionContainerAccessor.Container;
        }

        private static object FindWorkContextInParent(RouteData routeData)
        {
            object parentViewContextValue;
            // RiskPoint: This is the key MVC used to store parent action view context. It would probably be changed in future.
            if (!routeData.DataTokens.TryGetValue("ParentActionViewContext", out parentViewContextValue)
                || !(parentViewContextValue is ViewContext))
            {
                return null;
            }

            var parentRouteData = ((ViewContext)parentViewContextValue).RouteData;
            if (parentRouteData == null || parentRouteData.DataTokens == null)
                return null;

            object compositionContainerAccessor;
            if (!parentRouteData.DataTokens.TryGetValue(MvcConstants.ContainerAccessorKey, out compositionContainerAccessor))
            {
                compositionContainerAccessor = FindWorkContextInParent(parentRouteData);
            }

            return compositionContainerAccessor;
        }
    }
}
