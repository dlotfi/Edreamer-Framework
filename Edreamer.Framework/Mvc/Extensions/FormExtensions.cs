using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class FormExtensions
    {
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string formAction)
        {
            return BeginForm(htmlHelper, formAction, FormMethod.Post, null);
        } 

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string formAction, object formHtmlAttributes)
        {
            return BeginForm(htmlHelper, formAction, FormMethod.Post, formHtmlAttributes);
        }    

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string formAction, FormMethod method, object formHtmlAttributes)
        {
            return GetSimpleForm(htmlHelper, formAction, method, HtmlHelper.AnonymousObjectToHtmlAttributes(formHtmlAttributes));
        }    

        public static MvcForm BeginTemplateForm(this HtmlHelper htmlHelper, string formAction, object formHtmlAttributes, object additionalViewData)
        {
            return BeginTemplateForm(htmlHelper, formAction, FormMethod.Post, formHtmlAttributes, additionalViewData);
        }

        public static MvcForm BeginTemplateForm(this HtmlHelper htmlHelper, string formAction, FormMethod method, object formHtmlAttributes, object additionalViewData)
        {
            return GetTemplateForm(htmlHelper, formAction, method, HtmlHelper.AnonymousObjectToHtmlAttributes(formHtmlAttributes), new RouteValueDictionary(additionalViewData));
        }



        private static MvcForm GetTemplateForm(HtmlHelper html, string formAction, FormMethod method, IDictionary<string, object> formHtmlAttributes, IDictionary<string, object> additionalViewData)
        {
            var viewData = new ViewDataDictionary(html.ViewDataContainer.ViewData);
            foreach (var kvp in CollectionHelpers.EmptyIfNull(additionalViewData))
            {
                viewData[kvp.Key] = kvp.Value;
            }

            var beginFormView = ViewEngines.Engines.FindPartialView(html.ViewContext, "FormTemplates/BeginForm").View;
            if (beginFormView != null)
            {
                var viewContext = new ViewContext(html.ViewContext, beginFormView, viewData, html.ViewContext.TempData, html.ViewContext.Writer);
                beginFormView.Render(viewContext, viewContext.Writer);
            }

            var form = GetSimpleForm(html, formAction, method, formHtmlAttributes);

            // Determine end form  status
            var endFormView = ViewEngines.Engines.FindPartialView(html.ViewContext, "FormTemplates/EndForm").View;
            if (endFormView != null)
            {
                return new TemplatedMvcForm(html.ViewContext, endFormView, viewData);
            }
            return form;
        }

        private static MvcForm GetSimpleForm(HtmlHelper html, string formAction, FormMethod method, IDictionary<string, object> formHtmlAttributes)
        {
            var tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes(formHtmlAttributes);
            tagBuilder.MergeAttribute("action", formAction);
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            html.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            return new MvcForm(html.ViewContext);
        }

        private class TemplatedMvcForm : MvcForm
        {
            private readonly ViewContext _viewContext;
            private readonly IView _endFormView;
            private readonly ViewDataDictionary _viewData;
            private bool _disposed;

            public TemplatedMvcForm(ViewContext viewContext, IView endFormView, ViewDataDictionary viewData)
                :base(viewContext)
            {
                Throw.IfArgumentNull(endFormView, "endFormView");
                _viewContext = viewContext;
                _endFormView = endFormView;
                _viewData = viewData;
            }

            protected override void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    base.Dispose(disposing);
                    var viewContext = new ViewContext(_viewContext, _endFormView, _viewData, _viewContext.TempData, _viewContext.Writer);
                    _endFormView.Render(viewContext, viewContext.Writer);
                }
            }
        }
    }
}
