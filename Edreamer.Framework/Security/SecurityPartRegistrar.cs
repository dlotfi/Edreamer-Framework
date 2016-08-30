// Based on Orchard CMS

using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Security.Authentication;
using Edreamer.Framework.Security.Authorization;
using Edreamer.Framework.Security.Encryption;
using Edreamer.Framework.Security.Permissions;
using Edreamer.Framework.Security.Roles;
using Edreamer.Framework.Security.Users;

namespace Edreamer.Framework.Security
{
    public class SecurityPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IPermissionRegistrar>()
                .Export<IPermissionRegistrar>();

            rb.ForTypesDerivedFrom<IPermissionPublisher>()
                .Export<IPermissionPublisher>();

            rb.ForTypesDerivedFrom<IPermissionService>()
                .Export<IPermissionService>();

            rb.ForTypesDerivedFrom<IRoleService>()
                .Export<IRoleService>();

            rb.ForTypesDerivedFrom<IUserService>()
                .Export<IUserService>();

            rb.ForTypesDerivedFrom<IUserEventHandler>()
                .Export<IUserEventHandler>();

            rb.ForTypesDerivedFrom<IEncryptionService>()
                .Export<IEncryptionService>();

            rb.ForTypesDerivedFrom<IAuthenticationService>()
                .Export<IAuthenticationService>();

            rb.ForTypesDerivedFrom<IAuthorizationService>()
                .Export<IAuthorizationService>();

            rb.ForTypesDerivedFrom<IAuthorizationServiceEventHandler>()
                .Export<IAuthorizationServiceEventHandler>();

            rb.ForTypesDerivedFrom<IAuthorizer>()
                .Export<IAuthorizer>();
        }
    }
}
