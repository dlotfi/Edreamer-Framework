using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection
{
    public class InjectionBuilder<TTarget> : IInjectionBuilder<TTarget>
    {
        private TTarget _target;
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;
        private readonly ICollection<string> _injectedProperties;

        public InjectionBuilder(TTarget target, ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(target, "target");
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _target = target;
            _compositionContainerAccessor = compositionContainerAccessor;
            _injectedProperties = new List<string>();
        }

        public IInjectionBuilder<TTarget> For(Expression<Func<TTarget, object>> includedProperties, Func<IPropertyInjector, InjectionResult> propertiesInjection)
        {
            return For(includedProperties, (i, t) => propertiesInjection(i));
        }

        public IInjectionBuilder<TTarget> For(Expression<Func<TTarget, object>> includedProperties, Func<IPropertyInjector, TTarget, InjectionResult> propertiesInjection)
        {
            Throw.IfArgumentNull(includedProperties, "includedProperties");
            Throw.IfArgumentNull(propertiesInjection, "propertiesInjection");
            var properties = includedProperties.FindProperties().Select(mi => mi.Name).ToList();
            var alreadyInjectedProperties = _injectedProperties.Intersect(properties, StringComparer.OrdinalIgnoreCase);
            Throw.If(alreadyInjectedProperties.Any())
                .AnArgumentException("These properties are already injected: {0}".FormatWith(String.Join(", ", alreadyInjectedProperties)), "includedProperties");

            var injectionResult = propertiesInjection(new PropertyInjector(_target, properties, _compositionContainerAccessor), _target);
            _injectedProperties.AddRange(injectionResult.InjectedProperties);
            _target = (TTarget)injectionResult.Target;

            return this;
        }

        public IInjectionBuilder<TTarget> ForAny(Func<IPropertyInjector, InjectionResult> propertiesInjection)
        {
            return ForAny((i, t) => propertiesInjection(i));
        }

        public IInjectionBuilder<TTarget> ForAny(Func<IPropertyInjector, TTarget, InjectionResult> propertiesInjection)
        {
            Throw.IfArgumentNull(propertiesInjection, "propertiesInjection");

            var injectionResult = propertiesInjection(new PropertyInjector(_target, GetAllPropertiesExcept(_injectedProperties), _compositionContainerAccessor), _target);
            _injectedProperties.AddRange(injectionResult.InjectedProperties);
            _target = (TTarget)injectionResult.Target;

            return this;
        }

        public IInjectionBuilder<TTarget> ForAnyExcept(Expression<Func<TTarget, object>> excludedProperties, Func<IPropertyInjector, InjectionResult> propertiesInjection)
        {
            return ForAnyExcept(excludedProperties, (i, t) => propertiesInjection(i));
        }

        public IInjectionBuilder<TTarget> ForAnyExcept(Expression<Func<TTarget, object>> excludedProperties, Func<IPropertyInjector, TTarget, InjectionResult> propertiesInjection)
        {
            Throw.IfArgumentNull(excludedProperties, "excludedProperties");
            Throw.IfArgumentNull(propertiesInjection, "propertiesInjection");
            var properties = excludedProperties.FindProperties().Select(mi => mi.Name).ToList();
            var injectionResult = propertiesInjection(new PropertyInjector(_target, GetAllPropertiesExcept(properties.Concat(_injectedProperties)), _compositionContainerAccessor), _target);
            _injectedProperties.AddRange(injectionResult.InjectedProperties);
            _target = (TTarget)injectionResult.Target;

            return this;
        }

        public TTarget GetValue()
        {
            return _target;
        }

        public TTarget GetValue(out IEnumerable<string> injectedProperties)
        {
            injectedProperties = _injectedProperties;
            return _target;
        }

        #region Private Methods

        private IEnumerable<string> GetAllPropertiesExcept(IEnumerable<string> properties)
        {
            properties = CollectionHelpers.EmptyIfNull(properties);
            var allProperties = PropertiesStorage.GetProperties(_target.GetType(), true);
            return allProperties.Select(p => p.Name).Except(properties, StringComparer.OrdinalIgnoreCase);
        }

        #endregion
    }
}
