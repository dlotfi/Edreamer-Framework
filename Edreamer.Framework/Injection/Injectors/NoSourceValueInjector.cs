using System;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class NoSourceValueInjector<TTargetProperty> : LoopNoSourceValueInjectorBase
    {
        protected abstract bool SetValue(TTargetProperty targetPropertyValue, out TTargetProperty value);

        protected override void Inject(ObjectInfo targetProperty, Action<object> propertyInjector)
        {
            Throw.IfNot(targetProperty.Type.IsAssignableFrom(typeof(TTargetProperty)))
                .A<InjectionException>("{0} cannot inject into a property of type {1}."
                .FormatWith(GetType().Name, targetProperty.Type.FullName));

            TTargetProperty targetPropertyValue;
            var targetPropertyCurrentValue = targetProperty.Value is TTargetProperty ? (TTargetProperty)targetProperty.Value : default(TTargetProperty);
            if (SetValue(targetPropertyCurrentValue, out targetPropertyValue))
            {
                propertyInjector(targetPropertyValue);
            }
        }
    }
}
