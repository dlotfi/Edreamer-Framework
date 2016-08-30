using System;
using System.Reflection;
using System.Web.Mvc;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AjaxAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            Throw.IfArgumentNull(controllerContext, "controllerContext");
            return (controllerContext.HttpContext.Request.IsAjaxRequest());
        }
    }

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    //public sealed class AjaxOnlyAttribute : FilterAttribute, IAuthorizationFilter
    //{
    //    public void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        Throw.IfArgumentNull(filterContext, "filterContext");
    //        Throw.IfNot(filterContext.HttpContext.Request.IsAjaxRequest())
    //            .A<InvalidOperationException>("Only ajax requests are accepted.");
    //    }
    //}
}
