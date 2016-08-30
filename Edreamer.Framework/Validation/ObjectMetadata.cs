using System;
using System.Collections.Generic;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Validation
{
    public class ObjectMetadata
    {
        private readonly Func<ObjectMetadata, IEnumerable<ObjectMetadata>> _propertiesMetadataFinder;
        private IEnumerable<ObjectMetadata> _properties;

        public ObjectMetadata(Type type, Type containerType, string name, string qualifiedName, Func<ObjectMetadata, IEnumerable<ObjectMetadata>> propertiesMetadataFinder)
        {
            Throw.IfArgumentNull(type, "type");
            Throw.IfArgumentNull(propertiesMetadataFinder, "propertiesMetadataFinder");
            Type = type;
            ContainerType = containerType;
            Name = name ?? "";
            QualifiedName = qualifiedName ?? "";
            _propertiesMetadataFinder = propertiesMetadataFinder;
        }

        public Type Type { get; private set; }
        public Type ContainerType { get; private set; }
        public string Name { get; private set; }
        public string QualifiedName { get; private set; }
        public IEnumerable<ObjectMetadata> Properties
        {
            get { return _properties ?? (_properties = _propertiesMetadataFinder(this)); }
        }
    }
}
