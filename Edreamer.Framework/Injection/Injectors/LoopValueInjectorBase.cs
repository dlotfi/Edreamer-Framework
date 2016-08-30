using System;
using System.Collections.Generic;
using System.Linq;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class LoopValueInjectorBase: ValueInjectorBase
    {
        protected abstract void Inject(ObjectInfo sourceProperty, ObjectInfo targetProperty, Action<object> propertyInjector);

        sealed protected override InjectionResult Inject(object target, IEnumerable<string> properties, object source)
        {
            properties = properties.ToList();

            var sourcePropertiesDescriptors = PropertiesStorage.GetProperties(source.GetType());
            var targetPropertiesDescriptors = PropertiesStorage.GetProperties(target.GetType(), true);

            // Using HashSet prevents duplicate properties to be inserted.
            // Note that a property can be injected multiple times from different source properties.
            var injectedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // It's important to enumerate target properties in outer loop because 'targetProperty' object
            // should be preserved between different source properties.
            foreach (var targetPropertyDescriptor in targetPropertiesDescriptors)
            {
                if (!properties.Contains(targetPropertyDescriptor.Name, StringComparer.OrdinalIgnoreCase)) continue;

                var targetProperty = new ObjectInfo
                                         {
                                             Container = new ObjectInfo { Type = target.GetType(), Value = target },
                                             Descriptor = targetPropertyDescriptor,
                                             Name = targetPropertyDescriptor.Name,
                                             Value = targetPropertyDescriptor.GetValue(target),
                                             Type = targetPropertyDescriptor.PropertyType
                                         };

                foreach (var sourcePropertyDescriptor in sourcePropertiesDescriptors)
                {
                    var sourceProperty = new ObjectInfo
                                             {
                                                 Container = new ObjectInfo { Type = source.GetType(), Value = source },
                                                 Descriptor = sourcePropertyDescriptor,
                                                 Name = sourcePropertyDescriptor.Name,
                                                 Value = sourcePropertyDescriptor.GetValue(source),
                                                 Type = sourcePropertyDescriptor.PropertyType
                                             };

                    var propertyDescriptor = targetPropertyDescriptor; // Prevents access to modified closure
                    Inject(sourceProperty, targetProperty,
                           value =>
                               {
                                   propertyDescriptor.SetValue(target, value);
                                   targetProperty.Value = value;
                                   injectedProperties.Add(propertyDescriptor.Name);
                               });
                    // Setting 'targetProperty.Value' is important because this 'targetProperty' object is used
                    // for other source properties. One of its importance points is in unflatting cenario where
                    // a target property may be filled with values of different source properties.
                }
            }

            return new InjectionResult { InjectedProperties = injectedProperties, Target = target };
        }
    }
}
