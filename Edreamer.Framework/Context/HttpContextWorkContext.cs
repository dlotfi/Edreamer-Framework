using System;
using System.Web;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Context
{
    public class HttpContextWorkContext : IWorkContextStateProvider
    {
        public Func<IWorkContext, object> Get(string name)
        {
            if (name.EqualsIgnoreCase("HttpContext"))
            {
                var result = HttpContext.Current == null
                                 ? null
                                 : new HttpContextWrapper(HttpContext.Current);
                return ctx => result;
            }
            return null;
        }
    }

    public static class HttpContextWorkContextExtensions
    {
        public static HttpContextBase CurrentHttpContext(this IWorkContext workContext)
        {
            Throw.IfArgumentNull(workContext, "workContext");
            return workContext.GetState<HttpContextBase>("HttpContext");
        } 
    }
}
