using System;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class SourceDiggingValueInjectorBase: LoopValueInjectorBase
    {
        protected abstract MatchDiggingStatus MatchOrContinue(ObjectInfo sourceProperty, ObjectInfo targetProperty);

        protected virtual bool SetValue(ObjectInfo sourceProperty, ObjectInfo targetProperty, out object value)
        {
            value = sourceProperty.Value;
            return true;
        }

        sealed protected override void Inject(ObjectInfo sourceProperty, ObjectInfo targetProperty, Action<object> propertyInjector)
        {
            object value;
            var matchProperty = SourceDiggingMatchProperty(sourceProperty, targetProperty);
            if (matchProperty != null && SetValue(matchProperty, targetProperty, out value))
            {
                propertyInjector(value);
            }
        }

        private ObjectInfo SourceDiggingMatchProperty(ObjectInfo sourceProperty, ObjectInfo targetProperty)
        {
            var status = MatchOrContinue(sourceProperty, targetProperty);
            if (status == MatchDiggingStatus.Match) return sourceProperty;
            if (status == MatchDiggingStatus.Continue)
            {
                var sourcePropertyPropertiesDescriptors = PropertiesStorage.GetProperties(sourceProperty.Type);
                foreach (var propertyDescriptor in sourcePropertyPropertiesDescriptors)
                {
                    var sourcePropertyProperty = new ObjectInfo
                                                     {
                                                         Container = sourceProperty,
                                                         Descriptor = propertyDescriptor,
                                                         Name = propertyDescriptor.Name,
                                                         Type = propertyDescriptor.PropertyType,
                                                         Value = propertyDescriptor.GetValue(sourceProperty.Value)
                                                     };
                    var matchProperty = SourceDiggingMatchProperty(sourcePropertyProperty, targetProperty);
                    if (matchProperty != null) return matchProperty;
                }
            }
            return null;
        }
    }
}
