// Based on Orchard CMS

using System;
using System.Collections.Generic;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Injection;
using Edreamer.Framework.Localization;
using Edreamer.Framework.UI.Notification;
using Edreamer.Framework.Security.Users;

namespace Edreamer.Framework.Security.Authorization
{
    public class Authorizer : IAuthorizer
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IEnumerable<IUserEventHandler> _userEventHandlers;

        public Authorizer(IAuthorizationService authorizationService, INotifier notifier, IWorkContextAccessor workContextAccessor, IEnumerable<IUserEventHandler> userEventHandlers)
        {
            _authorizationService = authorizationService;
            _notifier = notifier;
            _workContextAccessor = workContextAccessor;
            _userEventHandlers = CollectionHelpers.EmptyIfNull(userEventHandlers);
        }

        public Localizer T { get; set; }

        public bool Authorize(Permission permission)
        {
            return Authorize(permission, null, null);
        }

        public bool Authorize(Permission permission, string message)
        {
            return Authorize(permission, null, message);
        }

        public bool Authorize(Permission permission, object content, string message)
        {
            var currentUser = _workContextAccessor.Context.CurrentUser();
            if (_authorizationService.TryCheckAccess(permission, currentUser, content))
                return true;

            if (!String.IsNullOrEmpty(message))
            {
                _notifier.Error((currentUser == null)
                    ? T("{0}. Anonymous users do not have '{1}' permission.", message, permission.Description)
                    : T("{0}. Current user, {2}, does not have '{1}' permission.", message, permission.Description, currentUser.Username));
            }

            if (currentUser != null)
            {
                foreach (var userEventHandler in _userEventHandlers)
                {
                    userEventHandler.AccessDenied(currentUser);
                }
            }

            return false;
        }

        public void ForceAuthorize(Permission permission)
        {
            ForceAuthorize(permission, null, null);
        }

        public void ForceAuthorize(Permission permission, string message)
        {
            ForceAuthorize(permission, null, message);
        }

        public void ForceAuthorize(Permission permission, object content, string message)
        {
            var currentUser = _workContextAccessor.Context.CurrentUser();
            if (_authorizationService.TryCheckAccess(permission, currentUser, content))
                return;

            if (!String.IsNullOrEmpty(message))
            {
                message = (currentUser == null)
                    ? T("{0}. Anonymous users do not have '{1}' permission.", message, permission.Description)
                    : T("{0}. Current user, {2}, does not have '{1}' permission.", message, permission.Description, currentUser.Username);
            }

            if (currentUser != null)
            {
                var currentUserCopy = Injector.PlaneInject(new User(), currentUser);
                foreach (var userEventHandler in _userEventHandlers)
                {
                    userEventHandler.AccessDenied(currentUserCopy);
                }
            }

            Throw.Now.A<SecurityException>("A security exception occurred.", message, new SecurityExceptionInfo
                                                                                           {
                                                                                               PermissionName = permission.Name,
                                                                                               User = currentUser,
                                                                                               Content = content
                                                                                           });
        }
    }
}
