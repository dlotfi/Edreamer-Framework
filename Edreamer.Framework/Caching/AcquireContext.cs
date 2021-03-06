﻿// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Caching
{
    public interface IAcquireContext
    {
        Action<IVolatileToken> Monitor { get; }
    }

    public class AcquireContext<TKey> : IAcquireContext 
    {
        public AcquireContext(TKey key, Action<IVolatileToken> monitor)
        {
            Key = key;
            Monitor = monitor;
        }

        public TKey Key { get; private set; }
        public Action<IVolatileToken> Monitor { get; private set; }
    }
}
