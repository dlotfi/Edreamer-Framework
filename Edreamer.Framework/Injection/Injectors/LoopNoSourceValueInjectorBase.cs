using System;
using System.Collections.Generic;
using System.Linq;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class LoopNoSourceValueInjectorBase: NoSourceValueInjectorBase
    {
        protected abstract void Inject(ObjectInfo targetProperty, Action<object> propertyInjector);

        protected override InjectionResult Inject(object target, IEnumerable<string> properties)
        {
            properties = properties.ToList();

            var targetPropertiesDescriptors = PropertiesStorage.GetProperties(target.GetType(), true);

            // Using HashSet prevents duplicate properties to be inserted. e.g. by calling propertyInjector multiple times.
            var injectedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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

                var propertyDescriptor = targetPropertyDescriptor; // Prevents access to modified closure
                Inject(targetProperty, value =>
                                           {
                                               propertyDescriptor.SetValue(target, value);
                                               injectedProperties.Add(propertyDescriptor.Name);
                                           });
            }

            return new InjectionResult { InjectedProperties = injectedProperties, Target = target };
        }
    }
}
