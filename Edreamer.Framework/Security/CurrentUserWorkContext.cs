using System;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Security.Authentication;

namespace Edreamer.Framework.Security
{
    public class CurrentUserWorkContext : IWorkContextStateProvider
    {
        private readonly IAuthenticationService _authenticationService;

        public CurrentUserWorkContext(IAuthenticationService authenticationService)
        {
            Throw.IfArgumentNull(authenticationService, "authenticationService");
            _authenticationService = authenticationService;
        }

        public Func<IWorkContext, object> Get(string name)
        {
            if (name.EqualsIgnoreCase("CurrentUser"))
            {
                return ctx => _authenticationService.GetAuthenticatedUser();
            }
            return null;
        }
    }

    public static class CurrentUserWorkContextExtensions
    {
        public static User CurrentUser(this IWorkContext workContext)
        {
            Throw.IfArgumentNull(workContext, "workContext");
            return workContext.GetState<User>("CurrentUser");
        }
    }
}
