using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Mvc.Filters;
using Edreamer.Framework.Security.Authorization;
using Edreamer.Framework.Security.Permissions;

namespace Edreamer.Framework.Mvc.Security
{
    /// <summary>
    /// Checks access to the front end actions (all actions except admin actions and those used for authentication)
    /// </summary>
    public class AccessFrontEndFilter : FilterProviderBase, IAuthorizationFilter
    {
        private readonly IAuthorizer _authorizer;

        public AccessFrontEndFilter(IAuthorizer authorizer) {
            _authorizer = authorizer;
        }

        public Localizer T { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!AdminAttribute.IsApplied(filterContext.RequestContext) && // Exclude admin actions
                !HasAuthActionAttribute(filterContext.ActionDescriptor) && // Exclude actions used for authenticating users
                !_authorizer.Authorize(StandardPermissions.AccessFrontEnd, T("Can't access this website")))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        private static bool HasAuthActionAttribute(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(AuthActionAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthActionAttribute), true))
                .Any();
        }
    }
}
