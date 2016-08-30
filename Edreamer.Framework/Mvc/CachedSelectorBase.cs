using System;
using System.Collections.Generic;
using Edreamer.Framework.Caching;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc
{
    public abstract class CachedSelectorBase<TRegistrar, TItem>
        where TItem : class
    {
        private readonly IEnumerable<TRegistrar> _registrars;

        protected IDictionary<string, object> Items 
        {
            get { return Cache.Get("Items", ctx => InitializeCache(_registrars)); }
        }

        public ICache Cache { get; set; }

        protected CachedSelectorBase(IEnumerable<TRegistrar> registrars)
        {
            _registrars = CollectionHelpers.EmptyIfNull(registrars);
        }

        protected abstract IEnumerable<TItem> Register(TRegistrar registrar);
        protected abstract string GetKey(TRegistrar registrar, TItem item);
        protected abstract object GetValue(TRegistrar registrar, TItem item);

        protected virtual void InitializeCaching()
        {}

        protected virtual void FinalizeCaching()
        {}

        protected virtual IDictionary<string, object> InitializeCache(IEnumerable<TRegistrar> registrars)
        {
            InitializeCaching();
            var items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var registrar in registrars)
            {
                foreach (var item in CollectionHelpers.EmptyIfNull(Register(registrar)))
                {
                    var key = GetKey(registrar, item);
                    Throw.If(keys.Contains(key))
                        .A<SelectorDuplicateKeyException>("Another item with the key '{0}' has already been registerd in selector.".FormatWith(key));
                    keys.Add(key);

                    var value = GetValue(registrar, item);
                    if (value != null)
                    {
                        items.Add(key, value);
                    }
                }
            }
            FinalizeCaching();
            return items;
        }
    }
}
