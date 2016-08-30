// Based on Orchard CMS

using System.Collections.Generic;

namespace Edreamer.Framework.Security.Permissions
{
    /// <summary>
    /// Implemented by modules to register their required permissions
    /// </summary>
    public interface IPermissionRegistrar
    {
        void RegisterPermissions(ICollection<Permission> permissions);
        void RegisterDefaultStereotypes(ICollection<PermissionStereotype> stereotypes);
    }
}