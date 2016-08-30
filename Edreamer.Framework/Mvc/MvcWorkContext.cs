using System;
using System.Collections.Generic;
using System.Web.WebPages;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc
{
    public class MvcWorkContext : IWorkContextStateProvider
    {
        public Func<IWorkContext, object> Get(string name)
        {
            if (name.EqualsIgnoreCase("TemplateContexts"))
            {
                var result = new Stack<string>();
                return ctx => result;
            }

            if (name.EqualsIgnoreCase("DeferredRenders"))
            {
                var result = new Dictionary<string, Func<HelperResult>>();
                return ctx => result;
            }

            return null;
        }
    }

    public static class MvcWorkContextExtensions
    {
        public static Stack<string> CurrentTemplateContexts(this IWorkContext workContext)
        {
            Throw.IfArgumentNull(workContext, "workContext");
            return workContext.GetState<Stack<string>>("TemplateContexts");
        }

        public static Dictionary<string, Func<HelperResult>> CurrentDeferredRenders(this IWorkContext workContext)
        {
            Throw.IfArgumentNull(workContext, "workContext");
            return workContext.GetState<Dictionary<string, Func<HelperResult>>>("DeferredRenders");
        }
    }
}
