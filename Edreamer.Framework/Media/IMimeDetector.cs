namespace Edreamer.Framework.Media
{
    public interface IMimeDetector
    {
        string GetMimeType(byte[] data, string name);
        string GetExtension(string mimeType);
    }
}