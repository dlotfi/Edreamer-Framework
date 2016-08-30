// Based on Orchard CMS

namespace Edreamer.Framework.Security.Authentication
{
    public interface IAuthenticationService
    {
        void SignIn(User user, bool createPersistentCookie);
        void SignOut();
        void SetAuthenticatedUserForRequest(User user);
        User GetAuthenticatedUser();
        bool IsAuthenticated();
    }
}
