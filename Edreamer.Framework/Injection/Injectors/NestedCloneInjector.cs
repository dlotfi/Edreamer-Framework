using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection.Injectors
{
    public class NestedCloneInjector : PlaneValueInjectorBase
    {
        private IEnumerable<Type> _excludedTypes;
        protected IEnumerable<Type> ExcludedTypes
        {
            get
            {
                return _excludedTypes ??
                       (_excludedTypes = new List<Type>(CollectionHelpers.EmptyIfNull(Parameter as IEnumerable<Type>)));
            }
        }

        protected override bool Match(ObjectInfo sourceProperty, ObjectInfo targetProperty)
        {
            return sourceProperty.Name.EqualsIgnoreCase(targetProperty.Name) &&
                sourceProperty.Value != null &&
                !ExcludedTypes.Contains(targetProperty.Type);
        }

        // ToDo [08272115]: Revise the implementation of NestedCloneInjector. It has poor performance and some bugs
        protected override bool SetValue(ObjectInfo sourceProperty, ObjectInfo targetProperty, out object value)
        {
            object targetPropertyValue;
            if (ConversionHelpers.TryConvert(sourceProperty.Value, targetProperty.Type, out targetPropertyValue))
            {
                value = targetPropertyValue;
                return true;
            }

            if (sourceProperty.Type.IsGenericType && targetProperty.Type.IsGenericType)
            {
                // Handles IEnumerable<>, ICollection<>, IList<> and List<> target properties
                if (sourceProperty.Type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)) &&
                    targetProperty.Type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    var targetUnderlyingType = targetProperty.Type.GetGenericArguments().First();
                    var targetListType = typeof(List<>).MakeGenericType(targetUnderlyingType);
                    var targetList = (IList)Activator.CreateInstance(targetListType);
                    var directConversion = true;
                    var nestedConversion = false;
                    foreach (var sourceItem in sourceProperty.Value as IEnumerable)
                    {
                        object targetItem;
                        if (directConversion && ConversionHelpers.TryConvert(sourceItem, targetUnderlyingType, out targetItem))
                        {
                            targetList.Add(targetItem);
                        }
                        else
                        {
                            directConversion = false;
                            if (nestedConversion || (targetUnderlyingType.IsComplex() && ObjectFactory.ConstructorExists(targetUnderlyingType)))
                            {
                                targetItem = ObjectFactory.CreateInstance(targetUnderlyingType);
                                IEnumerable<string> injectedProperties;
                                targetItem = Injector.Into(targetItem)
                                    .ForAny(i => i.Inject<NestedCloneInjector>(null, sourceItem))
                                    .GetValue(out injectedProperties);
                                if (injectedProperties.Any())
                                {
                                    targetList.Add(targetItem);
                                    nestedConversion = true;
                                }
                            }
                        }
                        if (!directConversion && !nestedConversion) break;
                    }
                    if (directConversion || nestedConversion)
                    {
                        value = targetList;
                        return true;
                    }
                }

                value = null;
                return false;
            }

            if (targetProperty.Type.IsComplex() && ObjectFactory.ConstructorExists(targetProperty.Type))
            {
                targetPropertyValue = ObjectFactory.CreateInstance(targetProperty.Type);
                IEnumerable<string> injectedProperties;
                targetPropertyValue = Injector.Into(targetPropertyValue)
                    .ForAny(i => i.Inject<NestedCloneInjector>(null, sourceProperty.Value))
                    .GetValue(out injectedProperties);
                if (injectedProperties.Any())
                {
                    value = targetPropertyValue;
                    return true;
                }
            }

            value = null;
            return false;
        }

        protected override void CheckParameter(object injectorParameter)
        {
            base.CheckParameter(injectorParameter);
            Throw.If(injectorParameter != null && !(injectorParameter is IEnumerable<Type>))
                .A<InjectionException>("NestedCloneInjector needs an IEnumerable<Type> parameter specifying excluded types.");
        }
    }
}
