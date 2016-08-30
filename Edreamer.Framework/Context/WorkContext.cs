// Based on Orchard CMS

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Context
{
    public class WorkContext : IWorkContext
    {
        private readonly ConcurrentDictionary<string, Func<object>> _stateResolvers = new ConcurrentDictionary<string, Func<object>>();
        private readonly IEnumerable<Lazy<IWorkContextStateProvider, IPriorityMetadata>> _workContextStateProviders;

        public WorkContext(IEnumerable<Lazy<IWorkContextStateProvider, IPriorityMetadata>> workContextStateProviders)
        {
            _workContextStateProviders = CollectionHelpers.EmptyIfNull(workContextStateProviders);
        }

        public T GetState<T>(string name)
        {
            Throw.IfArgumentNull(name, "name");
            T value;
            Throw.IfNot(TryGetState(name, out value))
                .AnArgumentException("No state named '{0}' found.".FormatWith(name), "name");
            return value;
        }

        public virtual bool TryGetState<T>(string name, out T value)
        {
            var resolver = _stateResolvers.GetOrAdd(name, FindResolverForState);
            if (resolver == null)
            {
                // FindResolverForState returns null even if there exists no suitable state provider.
                // This code cleans up unused resolvers from dictionary.
                _stateResolvers.TryRemove(name, out resolver); 
                value = default(T);
                return false;
            }

            value = (T) resolver();
            return true;
        }

        Func<object> FindResolverForState(string name)
        {
            Throw.IfArgumentNull(name, "name");
            var resolvers = _workContextStateProviders
                .Select(wcsp => new { Value = wcsp.Value.Get(name), wcsp.Metadata.Priority })
                .Where(x => x.Value != null).ToList();
            if (!resolvers.Any())
            {
                return null;
            }
            var resolver = resolvers.GetTopPriorityItem(r => r.Priority).Value;
            return () => resolver(this);
        }

        public virtual void SetState<T>(string name, T value)
        {
            _stateResolvers[name] = () => value;
        }

        public virtual bool StateExists(string name)
        {
            var resolver = _stateResolvers.GetOrAdd(name, FindResolverForState);
            if (resolver == null)
            {
                _stateResolvers.TryRemove(name, out resolver);
                return false;
            }
            return true;
        }
    }
}
