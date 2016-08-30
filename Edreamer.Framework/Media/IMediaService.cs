using System.IO;

namespace Edreamer.Framework.Media
{
    public interface IMediaService
    {
        Media CreateMedia(Stream source, string name = null, int id = 0, bool disposeSource = true);

        Media AddMedia(Media media);
        void UpdateMedia(Media media);
        void DeleteMedia(int id);

        Media GetMedia(int id);

        bool IsMediaAcceptable(Media media);
    }
}
