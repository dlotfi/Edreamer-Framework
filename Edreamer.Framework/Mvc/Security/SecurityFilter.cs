// Based on Orchard CMS

using System.Web.Mvc;
using Edreamer.Framework.Logging;
using Edreamer.Framework.Mvc.Filters;
using Edreamer.Framework.Security;

namespace Edreamer.Framework.Mvc.Security
{
    public class SecurityFilter : FilterProviderBase, IExceptionFilter
    {
        public ILogger Logger { get; set; }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !(filterContext.Exception is SecurityException)) return;

            try
            {
                Logger.Information(filterContext.Exception, "Security exception converted to access denied result");
            }
            catch
            {
                //a logger exception can't be allowed to interrupt this process
            }

            filterContext.Result = new HttpUnauthorizedResult();
            filterContext.ExceptionHandled = true;
        }
    }
}
