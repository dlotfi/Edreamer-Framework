// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Caching
{
    public interface ICache
    {
        TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire);
    }
}
