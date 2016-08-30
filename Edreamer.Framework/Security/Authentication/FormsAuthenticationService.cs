// Based on Orchard CMS

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Logging;
using Edreamer.Framework.Security.Users;

namespace Edreamer.Framework.Security.Authentication
{
    public class FormsAuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IEnumerable<IUserEventHandler> _userEventHandlers;
        private User _signedInUser;

        public FormsAuthenticationService(IUserService userService, IWorkContextAccessor workContextAccessor, IEnumerable<IUserEventHandler> userEventHandlers)
        {
            Throw.IfArgumentNull(userService, "userService");
            Throw.IfArgumentNull(workContextAccessor, "workContextAccessor");
            _workContextAccessor = workContextAccessor;
            _userEventHandlers = CollectionHelpers.EmptyIfNull(userEventHandlers);
            _userService = userService;
        }

        public ILogger Logger { get; set; }

        public void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            var userData = Convert.ToString(user.Id);

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                "",
                now,
                now.Add(FormsAuthentication.Timeout),
                createPersistentCookie,
                userData,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                             {
                                 HttpOnly = true,
                                 Secure = FormsAuthentication.RequireSSL,
                                 Path = FormsAuthentication.FormsCookiePath
                             };
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            var httpContext = _workContextAccessor.Context.CurrentHttpContext();
            httpContext.Response.Cookies.Add(cookie);
            _signedInUser = user;

            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.LoggedIn(user);
            }
        }

        public void SignOut()
        {
            var user = GetAuthenticatedUser();
            _signedInUser = null;
            FormsAuthentication.SignOut();
            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.LoggedOut(user);
            }
        }

        public void SetAuthenticatedUserForRequest(User user)
        {
            _signedInUser = user;
        }

        public User GetAuthenticatedUser()
        {
            if (_signedInUser != null)
                return _signedInUser;

            var httpContext = _workContextAccessor.Context.CurrentHttpContext();
            if (httpContext == null || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            var userData = formsIdentity.Ticket.UserData;
            int userId;
            if (!int.TryParse(userData, out userId))
            {
                Logger.Fatal("User id not a parsable integer");
                return null;
            }
            return _userService.GetUser(userId);
        }

        public bool IsAuthenticated()
        {
            var httpContext = _workContextAccessor.Context.CurrentHttpContext();
            return (_signedInUser != null) ||
                   (httpContext != null && httpContext.Request.IsAuthenticated && httpContext.User.Identity is FormsIdentity);
        }
    }
}
