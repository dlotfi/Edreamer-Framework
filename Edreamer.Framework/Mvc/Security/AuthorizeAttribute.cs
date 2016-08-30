using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Mvc.Extensions;
using Edreamer.Framework.Security;
using Edreamer.Framework.Security.Authorization;

namespace Edreamer.Framework.Mvc.Security
{
    /// <summary>
    /// Represents the base class for attributes used to authorize user's access to an action method. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class AuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        [Import]
        public IAuthorizer Authorizer { get; set; }

        [Import]
        public ILocalizerProvider LocalizerProvider { get; set; }

        public string Message { get; set; }
        public Permission Permission { get; set; }

        protected AuthorizeAttribute(Permission permission, string message)
        {
            Throw.IfArgumentNull(permission, "permission");
            Permission = permission;
            Message = message;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            bool authorized;
            if (String.IsNullOrEmpty(Message))
            {
                authorized = Authorizer.Authorize(Permission);
            }
            else
            {
                var controllerLocalizer = LocalizerProvider.GetLocalizer(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.FullName);
                var globalLocalizer = LocalizerProvider.GetLocalizer(GetType().FullName);
                authorized = Authorizer.Authorize(Permission, controllerLocalizer.IfCantThen(globalLocalizer)(Message));
            }

            if (!authorized)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

            Apply(filterContext.GetContainer().GetExportedValue<IWorkContextAccessor>().Context, Permission);
        }


        private const string Key = "_AuthorizeUserAppliedPermissions";

        private static void Apply(IWorkContext workContext, Permission permission)
        {
            Throw.IfArgumentNull(workContext, "workContext");
            Throw.IfArgumentNull(permission, "permission");

            lock (workContext)
            {
                if (!workContext.StateExists(Key))
                    workContext.SetState(Key, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

                workContext.GetState<ISet<string>>(Key).Add(permission.Name);
            }
        }

        public static bool IsApplied(IWorkContext workContext, Permission permission)
        {
            Throw.IfArgumentNull(workContext, "workContext");
            Throw.IfArgumentNull(permission, "permission");

            ISet<string> appliedPermissions;
            return workContext.TryGetState(Key, out appliedPermissions) && 
                appliedPermissions.Contains(permission.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
