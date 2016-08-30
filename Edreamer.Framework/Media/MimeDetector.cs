namespace Edreamer.Framework.Media
{
    public class MimeDetector: IMimeDetector
    {
        public virtual string GetMimeType(byte[] data, string name)
        {
            return MimeUtility.GetMimeType(data, name);
        }

        public virtual string GetExtension(string mimeType)
        {
            return MimeUtility.GetExtension(mimeType);
        }
    }
}