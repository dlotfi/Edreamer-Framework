using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Context
{
    public class ContextPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IWorkContextAccessor>()
                .Export<IWorkContextAccessor>()
                .SetAsSingleton();

            rb.ForTypesDerivedFrom<IWorkContext>()
                .Export<IWorkContext>()
                .SetCreationPolicy(CreationPolicy.Shared);

            rb.ForTypesDerivedFrom<IWorkContextStateProvider>()
                .Export<IWorkContextStateProvider>();
        }
    }
}
