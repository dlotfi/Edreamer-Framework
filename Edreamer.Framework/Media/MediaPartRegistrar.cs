using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Media.Image;
using Edreamer.Framework.Media.Storage;

namespace Edreamer.Framework.Media
{
    public class MediaPartRegistrar: IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IMediaPathProvider>()
                .Export<IMediaPathProvider>();

            rb.ForTypesDerivedFrom<IMediaService>()
                .Export<IMediaService>();

            rb.ForTypesDerivedFrom<IMimeDetector>()
                .Export<IMimeDetector>();

            rb.ForTypesDerivedFrom<IStorageProvider>()
                .Export<IStorageProvider>();

            rb.ForTypesDerivedFrom<IImageService>()
                .Export<IImageService>();
        }
    }
}
