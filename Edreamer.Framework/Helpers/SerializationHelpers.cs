using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Edreamer.Framework.Helpers
{
    public static class SerializationHelpers
    {
        public static string Serialize(object value)
        {
            if (value == null) return null;
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, value);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static object Deserialize(string serializedValue)
        {
            if (string.IsNullOrEmpty(serializedValue)) return null;
            var byteArray = Convert.FromBase64String(serializedValue);
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream(byteArray))
            {
                return formatter.Deserialize(ms);
            }
        } 
    }
}