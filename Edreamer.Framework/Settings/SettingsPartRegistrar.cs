using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Settings
{
    public class SettingsPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
           rb.ForTypesDerivedFrom<ISettingsService>()
                .Export<ISettingsService>();

            rb.ForTypesDerivedFrom<ISettingsProvider>()
                .Export<ISettingsProvider>();

            rb.ForTypesDerivedFrom<IWritableSettingsProvider>()
                .Export<IWritableSettingsProvider>();

            rb.ForTypesDerivedFrom<ISettingRegistrar>()
                .Export<ISettingRegistrar>();
        }
    }
}
