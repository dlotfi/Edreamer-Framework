using System.IO;

namespace Edreamer.Framework.Media.Image
{
    public interface IImageService
    {
        Stream ManipulateImage(Stream data, bool disposeSource, params ImageManipulation[] manipulations);
        Media ManipulateImage(Media media, params ImageManipulation[] manipulations);
    }
}
