using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Data
{
    public class DataPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IDataContext>()
                .ExportInterfaces(t => typeof(IDataContext).IsAssignableFrom(t) && t != typeof(IDataContext))
                .SetCreationPolicy(CreationPolicy.Shared); // Shared in a request
        }
    }
}
