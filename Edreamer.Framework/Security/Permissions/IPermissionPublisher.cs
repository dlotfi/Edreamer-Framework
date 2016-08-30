using System.Collections.Generic;

namespace Edreamer.Framework.Security.Permissions
{
    public interface IPermissionPublisher
    {
        void Publish(IEnumerable<IPermissionRegistrar> permissionRegistrars);
    }
}
