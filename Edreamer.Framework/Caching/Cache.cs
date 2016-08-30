// Based on Orchard CMS

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Caching
{
    public class Cache : ICache
    {
        private readonly ICacheContextAccessor _cacheContextAccessor;
        private readonly ConcurrentDictionary<object, CacheEntry> _entries;

        public Cache(ICacheContextAccessor cacheContextAccessor)
        {
            Throw.IfArgumentNull(cacheContextAccessor, "cacheContextAccessor");
            _cacheContextAccessor = cacheContextAccessor;
            _entries = new ConcurrentDictionary<object, CacheEntry>();
        }

        public TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire)
        {
            Throw.IfArgumentNull(key, "key");
            Throw.IfArgumentNull(acquire, "acquire");

            var entry = _entries.AddOrUpdate(key,
                // "Add" lambda
                k => AddEntry((TKey)k, acquire),
                // "Update" lamdba
                (k, currentEntry) => UpdateEntry(currentEntry, (TKey)k, acquire));

            return (TResult)entry.Result;
        }

        private CacheEntry AddEntry<TKey, TResult>(TKey k, Func<AcquireContext<TKey>, TResult> acquire)
        {
            var entry = CreateEntry(k, acquire);
            PropagateTokens(entry);
            return entry;
        }

        private CacheEntry UpdateEntry<TKey, TResult>(CacheEntry currentEntry, TKey k, Func<AcquireContext<TKey>, TResult> acquire)
        {
            var entry = (currentEntry.Tokens.Any(t => !t.IsCurrent)) ? CreateEntry(k, acquire) : currentEntry;
            PropagateTokens(entry);
            return entry;
        }

        private void PropagateTokens(CacheEntry entry)
        {
            // Bubble up volatile tokens to parent context
            if (_cacheContextAccessor.Current != null)
            {
                foreach (var token in entry.Tokens)
                    _cacheContextAccessor.Current.Monitor(token);
            }
        }

        private CacheEntry CreateEntry<TKey, TResult>(TKey k, Func<AcquireContext<TKey>, TResult> acquire)
        {
            var entry = new CacheEntry();
            var context = new AcquireContext<TKey>(k, entry.AddToken);

            IAcquireContext parentContext = null;
            try
            {
                // Push context
                parentContext = _cacheContextAccessor.Current;
                _cacheContextAccessor.Current = context;

                entry.Result = acquire(context);
            }
            finally
            {
                // Pop context
                _cacheContextAccessor.Current = parentContext;
            }
            entry.CompactTokens();
            return entry;
        }

        private class CacheEntry
        {
            private IList<IVolatileToken> _tokens;
            public object Result { get; set; }

            public IEnumerable<IVolatileToken> Tokens
            {
                get
                {
                    return _tokens ?? Enumerable.Empty<IVolatileToken>();
                }
            }

            public void AddToken(IVolatileToken volatileToken)
            {
                if (_tokens == null)
                {
                    _tokens = new List<IVolatileToken>();
                }

                _tokens.Add(volatileToken);
            }

            public void CompactTokens()
            {
                if (_tokens != null)
                    _tokens = _tokens.Distinct().ToArray();
            }
        }
    }
}
