using System;
using System.IO;

namespace Edreamer.Framework.Helpers
{
    public static class StreamExtensions
    {
        public static byte[] ConvertToByteArray(this Stream stream)
        {
            var streamLength = Convert.ToInt32(stream.Length);
            var data = new byte[streamLength];

            // convert to a byte array
            stream.Read(data, 0, streamLength);
            stream.Close();

            return data;
        }
    }
}
