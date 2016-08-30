using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Returns true if an error exists for the specified field in the ModelStateDictionary object.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the object that contains the properties to check for error.</param>
        /// <returns></returns>
        public static bool CheckErrorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return CheckErrorHelper(htmlHelper,
                ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
                ExpressionHelper.GetExpressionText(expression));
        }

        /// <summary>
        /// Returns true if an error exists for each data field that is represented by the specified expression.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="modelName">The name of the property or model object that is being checked for error.</param>
        /// <returns></returns>
        public static bool CheckError(this HtmlHelper htmlHelper, string modelName)
        {
            return CheckErrorHelper(htmlHelper, 
                ModelMetadata.FromStringExpression(modelName, htmlHelper.ViewContext.ViewData), 
                modelName);
        }

        private static bool CheckErrorHelper(this HtmlHelper htmlHelper, ModelMetadata modelMetadata, string expression)
        {
            var modelName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expression);
            var formContext = htmlHelper.ViewContext.FormContext;
            if (formContext == null)
                return false;

            if (!htmlHelper.ViewData.ModelState.ContainsKey(modelName))
                return false;

            var modelState = htmlHelper.ViewData.ModelState[modelName];
            if (modelState == null)
                return false;

            var modelErrors = modelState.Errors;
            if (modelErrors == null)
                return false;

            return (modelErrors.Count > 0);
        }
    }
}