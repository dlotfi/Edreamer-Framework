using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class InputExtensions
    {
        #region File

        public static MvcHtmlString File(this HtmlHelper htmlHelper, string name)
        {
            return File(htmlHelper, name, null /* value */, (object)null /* htmlAttributes */);
        }

        public static MvcHtmlString File(this HtmlHelper htmlHelper, string name, object value)
        {
            return File(htmlHelper, name, value, (object)null /* htmlAttributes */);
        }

        public static MvcHtmlString File(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return File(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString File(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            Throw.IfArgumentNull(htmlHelper, "htmlHelper");
            htmlAttributes = CollectionHelpers.EmptyIfNull(htmlAttributes);
            htmlAttributes.Add("type", "file");
            return htmlHelper.TextBox(name, value, htmlAttributes);
        }

        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return FileFor(htmlHelper, expression, (object)null /* htmlAttributes */);
        }

        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return FileFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            Throw.IfArgumentNull(htmlHelper, "htmlHelper");
            htmlAttributes = CollectionHelpers.EmptyIfNull(htmlAttributes);
            htmlAttributes.Add("type", "file");
            return htmlHelper.TextBoxFor(expression, htmlAttributes);
        }
        
        #endregion

        #region SimpleCheckBox

        public static MvcHtmlString SimpleCheckBox(this HtmlHelper htmlHelper, string name)
        {
            return SimpleCheckBox(htmlHelper, name, (object)null /* htmlAttributes */);
        }

        public static MvcHtmlString SimpleCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked)
        {
            return SimpleCheckBox(htmlHelper, name, isChecked, (object)null /* htmlAttributes */);
        }

        public static MvcHtmlString SimpleCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, object htmlAttributes)
        {
            return SimpleCheckBox(htmlHelper, name, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString SimpleCheckBox(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return SimpleCheckBox(htmlHelper, name, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString SimpleCheckBox(this HtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
        {
            return RemoveHiddenInput(htmlHelper.CheckBox(name, htmlAttributes));
        }

        public static MvcHtmlString SimpleCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            return RemoveHiddenInput(htmlHelper.CheckBox(name, isChecked, htmlAttributes));
        }

        public static MvcHtmlString SimpleCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            return SimpleCheckBoxFor(htmlHelper, expression, null /* htmlAttributes */);
        }

        public static MvcHtmlString SimpleCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            return SimpleCheckBoxFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString SimpleCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
        {
            return RemoveHiddenInput(htmlHelper.CheckBoxFor(expression, htmlAttributes));
        }

        private static MvcHtmlString RemoveHiddenInput(MvcHtmlString originalHtml)
        {
            const string pattern = @"<input name=""[^""]+"" type=""hidden"" value=""false"" />";
            var single = Regex.Replace(originalHtml.ToString(), pattern, "");
            return MvcHtmlString.Create(single);
        } 

        #endregion
    }
}
