using System;

namespace Edreamer.Framework.Caching
{
    public interface ICacheFactory
    {
        ICache CreateCache(Type type);
    }
}
