// Based on Orchard CMS

using System.Collections.Generic;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;


namespace Edreamer.Framework.Security.Permissions
{
    public class StandardPermissions : IPermissionRegistrar
    {
        public StandardPermissions()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public static readonly Permission AccessAdminPanel = new Permission { Name = "AccessAdminPanel", Description = "Access admin panel" };
        public static readonly Permission AccessFrontEnd = new Permission { Name = "AccessFrontEnd", Description = "Access site front-end" };
        public static readonly Permission SiteOwner = new Permission { Name = "SiteOwner", Description = "Site Owners Permission" };

        public void RegisterPermissions(ICollection<Permission> permissions)
        {
            Throw.IfArgumentNull(permissions, "permissions");
            AccessAdminPanel.Description = T(AccessAdminPanel.Description);
            AccessFrontEnd.Description = T(AccessFrontEnd.Description);
            SiteOwner.Description = T(SiteOwner.Description);
            permissions.AddRange(new[] { AccessAdminPanel, AccessFrontEnd, SiteOwner });
        }

        public void RegisterDefaultStereotypes(ICollection<PermissionStereotype> stereotypes)
        {
            stereotypes.AddRange(new[] {
                new PermissionStereotype {
                    RoleName = "Administrator",
                    Permissions = new[] { SiteOwner, AccessAdminPanel }
                },
                new PermissionStereotype {
                    RoleName = "Anonymous",
                    Permissions = new[] { AccessFrontEnd }
                },
                new PermissionStereotype {
                    RoleName = "Authenticated",
                    Permissions = new[] { AccessFrontEnd }
                }
            });
        }
    }
}