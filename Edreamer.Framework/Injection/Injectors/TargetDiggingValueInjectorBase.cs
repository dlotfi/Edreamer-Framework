using System;
using System.ComponentModel;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class TargetDiggingValueInjectorBase: LoopValueInjectorBase
    {
        protected abstract MatchDiggingStatus MatchOrContinue(ObjectInfo sourceProperty, ObjectInfo targetProperty);

        protected virtual bool SetValue(ObjectInfo sourceProperty, ObjectInfo targetProperty, out object value)
        {
            value = sourceProperty.Value;
            return true;
        }

        sealed protected override void Inject(ObjectInfo sourceProperty, ObjectInfo targetProperty, Action<object> propertyInjector)
        {
            var matchProperty = TargetDiggingMatchProperty(sourceProperty, targetProperty);
            if (matchProperty != null)
            {
                object value;
                if (SetValue(sourceProperty, matchProperty, out value))
                {
                    value = EnsureObjectGraph(matchProperty, value);
                    propertyInjector(value);
                }
            }
        }

        private ObjectInfo TargetDiggingMatchProperty(ObjectInfo sourceProperty, ObjectInfo targetProperty)
        {
            var status = MatchOrContinue(sourceProperty, targetProperty);
            if (status == MatchDiggingStatus.Match) return targetProperty;
            if (status == MatchDiggingStatus.Continue)
            {
                var targetPropertyPropertiesDescriptors = PropertiesStorage.GetProperties(targetProperty.Type, true);
                foreach (var propertyDescriptor in targetPropertyPropertiesDescriptors)
                {
                    var targetPropertyProperty = new ObjectInfo
                                                     {
                                                         Container = targetProperty,
                                                         Descriptor = propertyDescriptor,
                                                         Name = propertyDescriptor.Name,
                                                         Type = propertyDescriptor.PropertyType,
                                                         Value = targetProperty.Value != null
                                                                 ? propertyDescriptor.GetValue(targetProperty.Value)
                                                                 : null
                                                     };
                    var matchProperty = TargetDiggingMatchProperty(sourceProperty, targetPropertyProperty);
                    if (matchProperty != null) return matchProperty;
                }
            }
            return null;
        }

        private static object EnsureObjectGraph(ObjectInfo targetProperty, object value)
        {
            Throw.IfArgumentNull(targetProperty, "targetProperty");
            Throw.IfNull(targetProperty.Container)
                .AnArgumentException("A property container cannot be null.", "targetProperty");

            targetProperty.Value = value; // This assignment seems useless but anyway it's harmless
            return EnsureObjectGraph(targetProperty.Container, targetProperty.Descriptor, value);
        }


        private static object EnsureObjectGraph(ObjectInfo container, PropertyDescriptor property, object value)
        {
            if (container.Container == null) return value;

            if (container.Value == null)
            {
                container.Value = ObjectFactory.CreateInstance(container.Type);
            }
            property.SetValue(container.Value, value);

            return EnsureObjectGraph(container.Container, container.Descriptor, container.Value);
        }
    }
}
