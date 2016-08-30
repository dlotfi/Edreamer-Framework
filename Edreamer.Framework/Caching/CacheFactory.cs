using System;
using System.Collections.Concurrent;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Caching
{
    public class CacheFactory: ICacheFactory
    {
        private readonly ConcurrentDictionary<Type, ICache> _caches = new ConcurrentDictionary<Type, ICache>();
        private readonly ICacheContextAccessor _cacheContextAccessor;

        public CacheFactory(ICacheContextAccessor cacheContextAccessor)
        {
            Throw.IfArgumentNull(cacheContextAccessor, "cacheContextAccessor");
            _cacheContextAccessor = cacheContextAccessor;
        }

        public ICache CreateCache(Type type)
        {
            Throw.IfArgumentNull(type, "type");
            return _caches.GetOrAdd(type, new Cache(_cacheContextAccessor));
        }
    }
}
