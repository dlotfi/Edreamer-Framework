// Based on Orchard CMS


namespace Edreamer.Framework.Security.Authorization
{
    public interface IAuthorizer
    {
        bool Authorize(Permission permission);
        bool Authorize(Permission permission, string message);
        bool Authorize(Permission permission, object content, string message);

        void ForceAuthorize(Permission permission);
        void ForceAuthorize(Permission permission, string message);
        void ForceAuthorize(Permission permission, object content, string message);
    }
}
