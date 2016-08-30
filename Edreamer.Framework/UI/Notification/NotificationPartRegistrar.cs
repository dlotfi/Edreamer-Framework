using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.UI.Notification
{
    public class NotificationPartRegistrar: IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<INotifier>()
                .Export<INotifier>()
                .SetCreationPolicy(CreationPolicy.Shared); // Shared in a request
        }
    }
}
