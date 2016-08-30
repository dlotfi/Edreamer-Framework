using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.UI.Resources
{
    public class ResourcesPartRegistrar: IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IResourceManager>()
                .Export<IResourceManager>();

            rb.ForTypesDerivedFrom<IResourceTagBuilder>()
                .Export<IResourceTagBuilder>();

            rb.ForTypesDerivedFrom<IResourceRegistrar>()
                .Export<IResourceRegistrar>();

            rb.ForTypesDerivedFrom<IContentLocator>()
                .Export<IContentLocator>();
        }
    }
}
