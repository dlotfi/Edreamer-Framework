using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class FullTemplateExtensions
    {
        #region Editor
        //public static IHtmlString FullEditor(this HtmlHelper html, string expression)
        //{
        //}

        //public static IHtmlString FullEditor(this HtmlHelper html, string expression, object additionalViewData)
        //{
        //}

        //public static IHtmlString FullEditor(this HtmlHelper html, string expression, string templateName, string htmlFieldName, string validationMessage, object additionalViewData)
        //{
        //}

        public static IHtmlString FullEditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            return FullEditorFor(htmlHelper, expression, null, null, null);
        }

        public static IHtmlString FullEditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object additionalViewData)
        {
            return FullEditorFor(htmlHelper, expression, null, null, additionalViewData);
        }

        public static IHtmlString FullEditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string templateName, string htmlFieldName, object additionalViewData)
        {
            // Creates a new HtmlHelper with a copy of original ViewDataContainer so that view data of one editor does not mix with another
            var html = new HtmlHelper<TModel>(htmlHelper.ViewContext, new ViewDataContainer(htmlHelper.ViewData), htmlHelper.RouteCollection);

            // Merges the additional view data to current view data so that they can be seen in editor and validator
            foreach (var kvp in new RouteValueDictionary(additionalViewData))
            {
                html.ViewDataContainer.ViewData[kvp.Key] = kvp.Value;
            }

            lock (html.ViewContext)
            {
                html.ViewContext.ViewData.Add(MvcConstants.FullEditorFlag, null);
                var result = html.EditorFor(expression, templateName, htmlFieldName, additionalViewData);
                html.ViewContext.ViewData.Remove(MvcConstants.FullEditorFlag);
                return result;
            }
        }
        #endregion

        #region Display
        //public static IHtmlString FullDisplay(this HtmlHelper html, string expression)
        //{
        //}

        //public static IHtmlString FullDisplay(this HtmlHelper html, string expression, object additionalViewData)
        //{
        //}

        //public static IHtmlString FullDisplay(this HtmlHelper html, string expression, string templateName, string htmlFieldName, object additionalViewData)
        //{
        //}

        public static IHtmlString FullDisplayFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            return FullDisplayFor(htmlHelper, expression, null, null, null);
        }

        public static IHtmlString FullDisplayFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object additionalViewData)
        {
            return FullDisplayFor(htmlHelper, expression, null, null, additionalViewData);
        }

        public static IHtmlString FullDisplayFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string templateName, string htmlFieldName, object additionalViewData)
        {
            // Creates a new HtmlHelper with a copy of original ViewDataContainer so that view data of one displayer does not mix with another
            var html = new HtmlHelper<TModel>(htmlHelper.ViewContext, new ViewDataContainer(htmlHelper.ViewData), htmlHelper.RouteCollection);

            // Merges the additional view data to current view data so that they can be seen in displayer
            foreach (var kvp in new RouteValueDictionary(additionalViewData)) 
            {
                html.ViewDataContainer.ViewData[kvp.Key] = kvp.Value;
            }

            lock (html.ViewContext)
            {
                html.ViewContext.ViewData.Add(MvcConstants.FullDisplayFlag, null);
                var result = html.DisplayFor(expression, templateName, htmlFieldName, additionalViewData);
                html.ViewContext.ViewData.Remove(MvcConstants.FullDisplayFlag);
                return result;
            }
        }
        #endregion

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = new ViewDataDictionary(viewData);
            }

            public ViewDataDictionary ViewData { get; set; }
        }
    }
}
