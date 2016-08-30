using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class LabelExtensions
    {
        public static string LabelTextFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return LabelTextHelper(ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData), ExpressionHelper.GetExpressionText(expression));
        }

        public static string LabelText(this HtmlHelper htmlHelper, string expression)
        {
            return LabelTextHelper(ModelMetadata.FromStringExpression(expression, htmlHelper.ViewData), expression);
        }

        private static string LabelTextHelper(ModelMetadata metadata, string htmlFieldName) 
        {
            return metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        }
    }
}