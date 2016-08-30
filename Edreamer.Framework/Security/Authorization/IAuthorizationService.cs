// Based on Orchard CMS

namespace Edreamer.Framework.Security.Authorization
{
    public interface IAuthorizationService
    {
        void CheckAccess(Permission permission, User user, object content);
        bool TryCheckAccess(Permission permission, User user, object content);
    }
}