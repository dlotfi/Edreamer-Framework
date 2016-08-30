using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Injection.Injectors;

namespace Edreamer.Framework.Injection
{
    public class InjectionsPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IValueInjector>()
                .ExportInterfaces(t => typeof(IValueInjector).IsAssignableFrom(t) && t != typeof(IValueInjector))
                .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForTypesDerivedFrom<INoSourceValueInjector>()
                .ExportInterfaces(t => typeof(INoSourceValueInjector).IsAssignableFrom(t) && t != typeof(INoSourceValueInjector))
                .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForType<FlatteningValueInjector>()
                .Export()
                .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForType<UnflatteningValueInjector>()
                .Export()
                .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForType<PlaneValueInjector>()
                .Export()
                .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForType<NestedCloneInjector>()
                .Export()
                .SetCreationPolicy(CreationPolicy.NonShared);
        }
    }
}
