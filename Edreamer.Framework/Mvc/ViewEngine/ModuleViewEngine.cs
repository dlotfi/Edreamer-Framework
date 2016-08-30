using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Mvc.Layouts;
using Edreamer.Framework.Mvc.Templates;

namespace Edreamer.Framework.Mvc.ViewEngine
{
    /// <summary>
    /// Provides view engine support for imported modules.
    /// </summary>
    public class ModuleViewEngine : RazorViewEngine
    {
        private readonly ITemplateSelector _templateSelector;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILayoutSelector _layoutSelector;
        private readonly string _templateCacheItemId;

        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="ModuleViewEngine" />.
        /// </summary>
        /// <param name="viewLocationProvider">View location provider that provides location formats for views.</param>
        /// <param name="layoutSelector">Layout selector used to find the proper master/layout for the view.</param>
        /// <param name="templateSelector">Template selector used to find the proper partial views and model templates.</param>
        /// <param name="workContextAccessor">The work context accessor.</param>
        public ModuleViewEngine(IViewLocationProvider viewLocationProvider, ILayoutSelector layoutSelector,
            ITemplateSelector templateSelector, IWorkContextAccessor workContextAccessor)
        {
            Throw.IfArgumentNull(viewLocationProvider, "viewLocationProvider");
            Throw.IfArgumentNull(layoutSelector, "layoutSelector");
            Throw.IfArgumentNull(templateSelector, "templateSelector");
            Throw.IfArgumentNull(workContextAccessor, "workContextAccessor");

            var locationFormats = new List<string>();
            locationFormats.AddRange(AreaViewLocationFormats);

            AreaViewLocationFormats = AreaViewLocationFormats.Concat(viewLocationProvider.AreaViewLocationFormats).ToArray();
            AreaMasterLocationFormats = AreaMasterLocationFormats.Concat(viewLocationProvider.AreaMasterLocationFormats).ToArray();
            AreaPartialViewLocationFormats = AreaPartialViewLocationFormats.Concat(viewLocationProvider.AreaPartialViewLocationFormats).ToArray();

            _templateSelector = templateSelector;
            _workContextAccessor = workContextAccessor;
            _layoutSelector = layoutSelector;

            // RiskPoint: I use reflection to get template cache id of Mvc in the reponsible class (TemplateHelpers) which is internal.
            var templateHelperType = typeof(Controller).Assembly.GetType("System.Web.Mvc.Html.TemplateHelpers");
            _templateCacheItemId = (string) templateHelperType.GetFieldInfo("CacheItemId").GetValue(null);
        }
        #endregion

        #region Methods
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string originalPartialViewName, bool useCache)
        {
            var partialViewName = originalPartialViewName;

            // Remove templates cached by Mvc. This is necessary because Mvc caches templates based on their names but I want to
            // choose templates according to the active template context. But this can degrade performance and I should find a
            // better solution for it. See System.Web.Mvc.Html.TemplateHelpers class for more information.
            // ToDo-Low [02251952]: Find a better solution for template selection which doesn't need clearing the Mvc templates cache.
            controllerContext.HttpContext.Items.Remove(_templateCacheItemId);
            
            // Change editor/display templates to fulleditor/fulldisplay templates if appropriate flag in ViewData has been set.
            var viewData = controllerContext is ViewContext
                ? ((ViewContext) controllerContext).ViewData
                : new ViewDataDictionary();
            var fullTemplate = viewData.ContainsKey(MvcConstants.FullEditorFlag) && originalPartialViewName.StartsWith("EditorTemplates") ||
                               viewData.ContainsKey(MvcConstants.FullDisplayFlag) && originalPartialViewName.StartsWith("DisplayTemplates");

            if (fullTemplate)
                partialViewName = "Full" + originalPartialViewName;
            
            var result = base.FindPartialView(controllerContext, partialViewName, useCache);
            if (result.View != null || useCache)
                return result;

            // It is not directly indicated in MSDN docs that Stack<T> iterates top to bottom but in practice it does.
            foreach (var templateContext in _workContextAccessor.Context.CurrentTemplateContexts())
            {
                if (!String.IsNullOrEmpty(templateContext))
                {
                    partialViewName = _templateSelector.GetTemplatePath(partialViewName, templateContext);
                    if (!String.IsNullOrEmpty(partialViewName))
                        result = base.FindPartialView(controllerContext, partialViewName, useCache);
                    break;
                }
            }

            return result.View != null && fullTemplate
                ? new ViewEngineResult(new FullTemplateView(result.View, viewData, originalPartialViewName), result.ViewEngine)
                : result;
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            //var result = base.FindView(controllerContext, viewName, masterName, useCache);
            //if (result.View != null || useCache || String.IsNullOrEmpty(masterName))
            //    return result;

            //masterName = _layoutSelector.GetLayoutPath(masterName);
            //if (!String.IsNullOrEmpty(masterName))
            //{
            //    result = base.FindView(controllerContext, viewName, masterName, useCache);
            //}
            //return result;

            if (masterName != null && masterName.EnclosedBy("<", ">"))
            {
                masterName = _layoutSelector.GetLayoutPath(masterName.Trim('<', '>'));
            }
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }


        #endregion

        private class FullTemplateView : IView
        {
            private readonly IView _view;
            private readonly ViewDataDictionary _viewData;
            private readonly string _partialViewName;

            public FullTemplateView(IView view, ViewDataDictionary viewData, string partialViewName)
            {
                _view = view;
                _viewData = viewData;
                _partialViewName = partialViewName;
            }

            public void Render(ViewContext viewContext, TextWriter writer)
            {
                // Remove all traces of full template
                // RiskPoint: I use reflection to remove model metadata from TemplateInfo.VisitedObject
                // which checks for recursion and returns empty if a template has been visited before
                var visitedObjects = typeof(TemplateInfo).GetProperty("VisitedObjects", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(viewContext.ViewData.TemplateInfo) as ISet<object>;
                visitedObjects.Remove(viewContext.ViewData.ModelMetadata.Model ?? viewContext.ViewData.ModelMetadata.ModelType);
                _viewData.Remove(_partialViewName.StartsWith("EditorTemplates") ? MvcConstants.FullEditorFlag : MvcConstants.FullDisplayFlag);
                
                _view.Render(viewContext, writer);
            }
        }
    }
}
