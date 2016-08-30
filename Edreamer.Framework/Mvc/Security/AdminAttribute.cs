using System;
using System.Web.Routing;
using Edreamer.Framework.Context;
using Edreamer.Framework.Mvc.Extensions;
using Edreamer.Framework.Security.Permissions;

namespace Edreamer.Framework.Mvc.Security
{
    /// <summary>
    /// Specifies that an action requires <see cref="StandardPermissions.AccessAdminPanel"/> permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AdminAttribute : AuthorizeAttribute
    {
        public AdminAttribute()
            : base(StandardPermissions.AccessAdminPanel, "Can't access the admin")
        {
        }

        public static bool IsApplied(RequestContext context)
        {
            return IsApplied(context.GetContainer().GetExportedValue<IWorkContextAccessor>().Context, StandardPermissions.AccessAdminPanel);
        }
    }
}
