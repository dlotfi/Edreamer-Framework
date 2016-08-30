using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Edreamer.Framework.Injection
{
    public interface IInjectionBuilder<TTarget>
    {
        IInjectionBuilder<TTarget> For(Expression<Func<TTarget, object>> includedProperties, Func<IPropertyInjector, InjectionResult> propertiesInjection);
        //IInjectionBuilder<TTarget> For(Expression<Func<TTarget, object>> includedProperties, Func<IPropertyInjector, TTarget, InjectionResult> propertiesInjection);
        IInjectionBuilder<TTarget> ForAny(Func<IPropertyInjector, InjectionResult> propertiesInjection);
        //IInjectionBuilder<TTarget> ForAny(Func<IPropertyInjector, TTarget, InjectionResult> propertiesInjection);
        IInjectionBuilder<TTarget> ForAnyExcept(Expression<Func<TTarget, object>> excludedProperties, Func<IPropertyInjector, InjectionResult> propertiesInjection);
        //IInjectionBuilder<TTarget> ForAnyExcept(Expression<Func<TTarget, object>> excludedProperties, Func<IPropertyInjector, TTarget, InjectionResult> propertiesInjection);
        
        /// <summary>
        /// Returns the target value. This method or similar overloads should be called for value-typed targets
        /// after injections are done.
        /// </summary>
        /// <returns>The target value</returns>
        TTarget GetValue();

        /// <summary>
        /// Returns the target value and names of injected properties. This method or similar overloads should be
        /// called for value-typed targets after injections are done.
        /// </summary>
        /// <returns>The target value</returns>
        TTarget GetValue(out IEnumerable<string> injectedProperties);
    }
}
