using System;
using System.Linq;
using System.Reflection;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Localization
{
    public class LocalizationCompositionInterceptor : ICompositionInterceptor
    {
        private readonly Lazy<ILocalizerProvider> _localizerProvider;

        // Lazily resolving dependencies to prevent cyclic dependencies
        public LocalizationCompositionInterceptor(Lazy<ILocalizerProvider> localizerProvider)
        {
            Throw.IfArgumentNull(localizerProvider, "localizerProvider");
            _localizerProvider = localizerProvider;
        }

        public object Intercept(object value)
        {
            var componentType = value.GetType();
            var localizerProperties = componentType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsAssignableFrom(typeof(Localizer)) && p.CanWrite);

            foreach (var property in localizerProperties)
            {
                var localizer = _localizerProvider.Value.GetLocalizer(value.GetType().FullName);
                property.SetValue(value, localizer, null);
            }

            return value;
        }
    }
}
