using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Validation
{
    public class ValidationPartRegistrar: IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IValidationService>()
                .Export<IValidationService>();

            rb.ForTypesDerivedFrom<IValidatorProvider>()
                .Export<IValidatorProvider>();

            rb.ForTypesDerivedFrom<IMetadataProvider>()
                .Export<IMetadataProvider>();
        }
    }
}
