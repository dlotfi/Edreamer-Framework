// Based on Orchard CMS

using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Security.Roles;
using Edreamer.Framework.Security.Users;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Security.Authorization
{
    public class RolesBasedAuthorizationService : IAuthorizationService
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly ISettingsService _settingsService;
        private readonly IEnumerable<IAuthorizationServiceEventHandler> _authorizationServiceEventHandlers;
        private static readonly string[] AnonymousRole = new[] { "Anonymous" };
        private static readonly string[] AuthenticatedRole = new[] { "Authenticated" };

        public RolesBasedAuthorizationService(IRoleService roleService, IUserService userService, ISettingsService settingsService, IEnumerable<IAuthorizationServiceEventHandler> authorizationServiceEventHandlers)
        {
            _userService = userService;
            _roleService = roleService;
            _settingsService = settingsService;
            _authorizationServiceEventHandlers = CollectionHelpers.EmptyIfNull(authorizationServiceEventHandlers);
        }

        public void CheckAccess(Permission permission, User user, object content)
        {
            Throw.IfNot(TryCheckAccess(permission, user, content))
                .A<SecurityException>("A security exception occurred.", new SecurityExceptionInfo
                                                                               {
                                                                                   PermissionName = permission.Name,
                                                                                   User = user,
                                                                                   Content = content,
                                                                               });
        }

        public bool TryCheckAccess(Permission permission, User user, object content)
        {
            var context = new CheckAccessContext { Permission = permission, User = user, Content = content };
            foreach (var authorizationServiceEventHandler in _authorizationServiceEventHandlers)
            {
                authorizationServiceEventHandler.Checking(context);
            }

            var superUser = _settingsService.GetSuperUser();
            for (var adjustmentLimiter = 0; adjustmentLimiter != 3; ++adjustmentLimiter)
            {
                if (!context.Granted && context.User != null && context.User.Username.EqualsIgnoreCase(superUser))
                {
                    context.Granted = true;
                }


                if (!context.Granted)
                {
                    // determine which set of permissions would satisfy the access check
                    var grantingNames = context.Permission.ImpliedBy.Select(p => p.Name).ToList();

                    // determine what set of roles should be examined by the access check
                    IEnumerable<string> rolesToExamine;
                    if (context.User == null)
                    {
                        rolesToExamine = AnonymousRole;
                    }
                    else
                    {
                        // the current user is not null, so get his roles and add "Authenticated" to it
                        rolesToExamine = GetEffectiveRolesOfUser(context.User.Id).Select(r => r.Name);

                        // when it is a simulated anonymous user in the admin
                        if (!rolesToExamine.Contains(AnonymousRole[0]))
                        {
                            rolesToExamine = rolesToExamine.Concat(AuthenticatedRole);
                        }
                    }

                    foreach (var role in rolesToExamine)
                    {
                        foreach (var permissionName in GetEffectivePermissionsOfRole(role).Select(p => p.Name))
                        {
                            if (grantingNames.Any(permissionName.EqualsIgnoreCase))
                            {
                                context.Granted = true;
                                break;
                            }
                        }

                        if (context.Granted)
                            break;
                    }
                }

                context.Adjusted = false;
                foreach (var authorizationServiceEventHandler in _authorizationServiceEventHandlers)
                {
                    authorizationServiceEventHandler.Adjust(context);
                }
                if (!context.Adjusted)
                    break;
            }

            foreach (var authorizationServiceEventHandler in _authorizationServiceEventHandlers)
            {
                authorizationServiceEventHandler.Complete(context);
            }

            return context.Granted;
        }

        protected virtual IEnumerable<Role> GetEffectiveRolesOfUser(int userId)
        {
            return _userService.GetRolesOfUser(userId);
        }

        protected virtual IEnumerable<Permission> GetEffectivePermissionsOfRole(string roleName)
        {
            return _roleService.GetPermissionsOfRole(roleName);
        }
    }
}
