using System;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Data.Infrastructure
{
    public class EntityProperty
    {
        public EntityProperty(string name, Type type, int keyIndex, bool concurrency)
        {
            Throw.IfArgumentNullOrEmpty(name, "name");
            Throw.IfArgumentNull(type, "type");
            Name = name;
            Type = type;
            KeyIndex = keyIndex;
            Concurrency = concurrency;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public int KeyIndex { get; private set; }
        public bool Concurrency { get; private set; }
    }
}
