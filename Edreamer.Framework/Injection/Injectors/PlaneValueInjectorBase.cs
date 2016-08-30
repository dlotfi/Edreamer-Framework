using System;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class PlaneValueInjectorBase : LoopValueInjectorBase
    {
        protected abstract bool Match(ObjectInfo sourceProperty, ObjectInfo targetProperty);

        protected virtual bool SetValue(ObjectInfo sourceProperty, ObjectInfo targetProperty, out object value)
        {
            value = sourceProperty.Value;
            return true;
        }

        sealed protected override void Inject(ObjectInfo sourceProperty, ObjectInfo targetProperty, Action<object> propertyInjector)
        {
            object value;
            if (Match(sourceProperty, targetProperty) && SetValue(sourceProperty, targetProperty, out value))
            {
                propertyInjector(value);
            }
        }
    }
}
