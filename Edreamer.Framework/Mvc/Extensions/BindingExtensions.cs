// Based on Phil Haack's article "Model Binding To A List" - http://haacked.com/archive/2008/10/23/model-binding-to-a-list.aspx

using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class BindingExtensions
    {
        private readonly static Regex StripIndexerRegex = new Regex(@"\[(?<index>\d+)\]", RegexOptions.Compiled);

        private static string GetIndexerFieldName(string fieldName)
        {
            fieldName = StripIndexerRegex.Replace(fieldName, string.Empty);
            return fieldName.StartsWith(".") ? fieldName.Substring(1) : fieldName;
        }

        private static string GetIndex(string fieldName)
        {
            var match = StripIndexerRegex.Match(fieldName);
            return match.Success ? match.Groups["index"].Value : "0";
        }

        public static MvcHtmlString HiddenIndexerForModel<TModel>(this HtmlHelper<TModel> html)
        {
            var name = GetIndexerFieldName(html.ViewData.TemplateInfo.GetFullHtmlFieldName(""));
            var index = GetIndex(name);
            return HiddenIndexer(html, name, index);
        }

        public static MvcHtmlString HiddenIndexerFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var index = GetIndex(name);
            return HiddenIndexer(html, name, index);
        }

        public static MvcHtmlString HiddenIndexer(this HtmlHelper html, string htmlFieldName, string index)
        {
            var name = htmlFieldName + ".Index";
            var markup = @"<input type=""hidden"" name=""{0}"" value=""{1}"" />".FormatWith(name, index);
            return MvcHtmlString.Create(markup);
        }
    }
}
