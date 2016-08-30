using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Edreamer.Framework.Injection
{
    public static class PropertiesStorage
    {
        // ToDo-Low [08261014]: Alter use of unmanaged static cache to universal cache mechanism
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyDescriptor>> Storage =
            new ConcurrentDictionary<Type, IEnumerable<PropertyDescriptor>>();

        public static IEnumerable<PropertyDescriptor> GetProperties(Type type, bool onlyWritables = false)
        {
            var result = Storage.GetOrAdd(type, TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>());
            return onlyWritables ? result.Where(p => !p.IsReadOnly).ToList() : result.ToList();
        }
    }
}