using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.UI.MetaData
{
    public class MetaDataPartRegistrar: IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IMetaDataManager>()
                .Export<IMetaDataManager>();

            rb.ForTypesDerivedFrom<IMetaTagBuilder>()
                .Export<IMetaTagBuilder>();
        }
    }
}
