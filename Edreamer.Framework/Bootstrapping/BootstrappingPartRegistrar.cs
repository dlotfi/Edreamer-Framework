using System;
using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Bootstrapping
{
    public class BootstrappingPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IBootstrapperTask>()
                .Export<IBootstrapperTask>( c =>
                    c.AddMetadata("IsPartOfFramework", t => IsInNamespace(t, "Edreamer.Framework"))
                     .AddMetadata("Type", t => t));
        }

        private static bool IsInNamespace(Type type, string namespaceFragment)
        {
            return type.Namespace != null &&
                  (type.Namespace == namespaceFragment ||
                  type.Namespace.StartsWith(namespaceFragment + "."));
        }
    }
}