using System;
using System.ComponentModel;

namespace Edreamer.Framework.Injection.Injectors
{
    public class ObjectInfo
    {
        public ObjectInfo Container { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
        public PropertyDescriptor Descriptor { get; set; }

        public string GetFalttenedPropertyName()
        {
            var obj = this;
            var name = Name;
            while (obj.Container != null)
            {
                name = obj.Container.Name + name;
                obj = obj.Container;
            }
            return name;
        }
    }
}
