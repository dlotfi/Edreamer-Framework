using System;
using System.Linq;
using System.Reflection;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Caching
{
    public class CacheCompositionInterceptor : ICompositionInterceptor
    {
        private readonly Lazy<ICacheFactory> _cacheFactory;

        // Lazily resolving dependencies to prevent cyclic dependencies
        public CacheCompositionInterceptor(Lazy<ICacheFactory> cacheFactory)
        {
            Throw.IfArgumentNull(cacheFactory, "cacheFactory");
            _cacheFactory = cacheFactory;
        }

        public object Intercept(object value)
        {
            var componentType = value.GetType();

            // Look for settable properties of type "ICache" 
            var cacheProperties = componentType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(ICache) && p.CanWrite);

            foreach (var property in cacheProperties)
            {
                var cache = _cacheFactory.Value.CreateCache(componentType);
                property.SetValue(value, cache, null);
            }

            return value;
        }
    }
}
