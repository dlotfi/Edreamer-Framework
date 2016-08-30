using System.Collections.Generic;
using Edreamer.Framework.Bootstrapping;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Security.Permissions;

namespace Edreamer.Framework.Security
{
    [DependendantOn("Injection")]
    public class SecurityBootstrapper: IBootstrapperTask
    {
        private readonly IPermissionPublisher _permissionPublisher;
        private readonly IEnumerable<IPermissionRegistrar> _permissionRegistrars;

        public SecurityBootstrapper(IPermissionPublisher permissionPublisher, IEnumerable<IPermissionRegistrar> permissionRegistrars)
        {
            Throw.IfArgumentNull(permissionPublisher, "permissionPublisher");
            _permissionPublisher = permissionPublisher;
            _permissionRegistrars = CollectionHelpers.EmptyIfNull(permissionRegistrars);
        }

        public void Run()
        {
            _permissionPublisher.Publish(_permissionRegistrars);
        }
    }
}
