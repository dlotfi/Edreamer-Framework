using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Module
{
    public class ModulePartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IModuleManager>() 
                .Export<IModuleManager>();
            
            rb.ForTypesDerivedFrom<Module>()
                .Export<Module>();

            rb.ForTypesDerivedFrom<IModuleEventHandler>()
                .Export<IModuleEventHandler>();
        }
    }
}
