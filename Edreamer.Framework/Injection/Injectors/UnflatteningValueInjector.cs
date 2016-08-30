using System;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection.Injectors
{
    public class UnflatteningValueInjector: TargetDiggingValueInjectorBase
    {
        sealed protected override MatchDiggingStatus MatchOrContinue(ObjectInfo sourceProperty, ObjectInfo targetProperty)
        {
            var flattenedPropertyName = targetProperty.GetFalttenedPropertyName();
            if (sourceProperty.Name.EqualsIgnoreCase(flattenedPropertyName) &&
                TypesMatch(sourceProperty.Value == null ? sourceProperty.Type : sourceProperty.Value.GetType(), targetProperty.Type))
                return MatchDiggingStatus.Match;
            if (sourceProperty.Name.StartsWith(flattenedPropertyName, StringComparison.OrdinalIgnoreCase))
                return MatchDiggingStatus.Continue;
            return MatchDiggingStatus.Stop;
        }

        protected virtual bool TypesMatch(Type sourcePropertyType, Type targetPropertyType)
        {
            return targetPropertyType.IsAssignableFrom(sourcePropertyType);
        }
    }

    public abstract class UnflatteningValueInjector<TSourceProperty, TTargetProperty> : UnflatteningValueInjector
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
