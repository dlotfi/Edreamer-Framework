using System;
using System.Linq;

namespace Edreamer.Framework.Media
{
    public class Media
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public byte[] Data { get; set; }

        public string TypeGroup
        {
            get { return String.IsNullOrEmpty(Type) ? null : Type.Split('/').FirstOrDefault(); }
        }
    }
}
