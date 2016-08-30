using System;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection.Injectors
{
    public class PlaneValueInjector : PlaneValueInjectorBase
    {
        sealed protected override bool Match(ObjectInfo sourceProperty, ObjectInfo targetProperty)
        {
            return NamesMatch(sourceProperty.Name, targetProperty.Name) &&
                   TypesMatch(sourceProperty.Value == null ? sourceProperty.Type : sourceProperty.Value.GetType(),
                              targetProperty.Type);
        }

        protected virtual bool TypesMatch(Type sourcePropertyType, Type targetPropertyType)
        {
            return targetPropertyType.IsAssignableFrom(sourcePropertyType);
        }

        protected virtual bool NamesMatch(string sourcePropertyName, string targetPropertyName)
        {
            return sourcePropertyName.EqualsIgnoreCase(targetPropertyName);
        }
    }

    public abstract class PlaneValueInjector<TSourceProperty, TTargetProperty> : PlaneValueInjector
    {
        sealed protected override bool TypesMatch(Type sourcePropertyType, Type targetPropertyType)
        {
            return typeof(TSourceProperty).IsAssignableFrom(sourcePropertyType) &&
                   targetPropertyType.IsAssignableFrom(typeof(TTargetProperty));
        }

        sealed protected override bool SetValue(ObjectInfo sourceProperty, ObjectInfo targetProperty, out object value)
        {
            TTargetProperty targetPropertyValue;
            var targetPropertyCurrentValue = targetProperty.Value is TTargetProperty ? (TTargetProperty)targetProperty.Value : default(TTargetProperty);
            if (SetValue((TSourceProperty)sourceProperty.Value, targetPropertyCurrentValue, out targetPropertyValue))
            {
                value = targetPropertyValue;
                return true;
            }
            value = default(TTargetProperty);
            return false;
        }

        protected abstract bool SetValue(TSourceProperty sourcePropertyValue, TTargetProperty targetPropertyValue, out TTargetProperty value);
    }
}
