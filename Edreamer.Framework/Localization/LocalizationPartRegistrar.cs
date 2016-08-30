using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Localization
{
    public class LocalizationPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<ILocalizationManager>()
                .Export<ILocalizationManager>();

            rb.ForTypesDerivedFrom<ILocalizerProvider>()
                .Export<ILocalizerProvider>();
        }
    }
}
