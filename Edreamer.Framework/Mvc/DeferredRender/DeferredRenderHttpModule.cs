using System;
using System.Linq;
using System.Web;
using Edreamer.Framework.Context;
using Edreamer.Framework.Mvc.Extensions;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof(Edreamer.Framework.Mvc.DeferredRender.DeferredRenderHttpModule), "Register")]
namespace Edreamer.Framework.Mvc.DeferredRender
{
    public class DeferredRenderHttpModule : IHttpModule
    {
        private static bool _isRegistered;

        public static void Register()
        {
            // All Register calls are made on the same thread, so no lock needed here.
            if (_isRegistered) return;

            DynamicModuleUtility.RegisterModule(typeof(DeferredRenderHttpModule));
            _isRegistered = true;
        }

        public void Init(HttpApplication context)
        {
            context.PostReleaseRequestState += OnPostReleaseRequestState;
        }

        // Called just before performing response filtering
        private void OnPostReleaseRequestState(object sender, EventArgs e)
        {
            var container = HttpContext.Current?.Request.RequestContext.GetContainer();
            if (container != null) // (RequestContext.Route != null)
            {
                var workContextAccessor = container.GetExportedValue<IWorkContextAccessor>();
                var deferredRenders = workContextAccessor.Context.CurrentDeferredRenders();
                if (deferredRenders.Any())
                {
                    HttpContext.Current.Response.Filter = new DeferredRendersReplacementStream(HttpContext.Current.Response.Filter, deferredRenders);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}