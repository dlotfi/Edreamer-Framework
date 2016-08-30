using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;
using Edreamer.Framework.Mvc.DeferredRender;
using Edreamer.Framework.Mvc.Routes;
using Edreamer.Framework.Settings;
using JetBrains.Annotations;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class ControllerExtensions
    {
        public static string ResolveLocalUrl(this Controller controller, [PathReference]string localUrl)
        {
            Throw.IfArgumentNull(controller, "controller");
            Throw.IfArgumentNullOrEmpty(localUrl, "localUrl");
            var settingsService = controller.GetContainer().GetExportedValue<ISettingsService>();
            var modulesPath = settingsService.GetModulesPath();
            var moduleName = controller.RouteData.Route.GetAreaName();
            if (localUrl.StartsWith("~"))
            {
                localUrl = localUrl.Replace("~", modulesPath + moduleName);
                return localUrl;
            }
            return localUrl;
        }

        /// <summary>
        /// Checks the url to prevent open redirection attacks
        /// </summary>
        public static RedirectResult SafeRedirect(this Controller controller, string url, string defaultUrl)
        {
            Throw.IfArgumentNull(controller, "controller");
            Throw.IfArgumentNullOrEmpty(defaultUrl, "defaultUrl");
            return (!String.IsNullOrEmpty(url) && controller.Url.IsLocalUrl(url))
                ? new RedirectResult(url)
                : new RedirectResult(defaultUrl);
        }

        public static HtmlHelper HtmlHelper(this Controller controller)
        {
            var viewContext = new ViewContext(controller.ControllerContext, new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
            return new HtmlHelper(viewContext, new ViewPage());
        }

        public static string RenderView(this Controller controller, string viewPath, object model = null, bool partial = false)
        {
            // first find the view
            var viewEngineResult = partial 
                ? ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewPath) 
                : ViewEngines.Engines.FindView(controller.ControllerContext, viewPath, null);

            if (viewEngineResult.View == null)
            {
                var locationsText = new StringBuilder();
                foreach (var location in viewEngineResult.SearchedLocations)
                {
                    locationsText.AppendLine();
                    locationsText.Append(location);
                }

                throw new InvalidOperationException("The " + (partial ? "partial " : "") + "view '{0}' was not found or no view engine supports the searched locations. The following locations were searched:{1}"
                    .FormatWith(viewPath, locationsText));
            }

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(controller.ControllerContext, view, controller.ViewData, controller.TempData, sw);
                view.Render(ctx, sw);
                var workContextAccessor = controller.GetContainer().GetExportedValue<IWorkContextAccessor>();
                var deferredRenders = workContextAccessor.Context.CurrentDeferredRenders();
                var renderedView = DeferredRenderHelper.ProcessDefferedRenders(sw.ToString(), deferredRenders);
                return renderedView;
            }
        }

        class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }
    }
}