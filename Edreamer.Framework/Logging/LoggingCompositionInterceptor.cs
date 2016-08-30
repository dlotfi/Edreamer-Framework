using System;
using System.Linq;
using System.Reflection;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Logging
{
    public class LoggingCompositionInterceptor : ICompositionInterceptor
    {
        private readonly Lazy<ILoggerFactory> _loggerFactory;

        // Lazily resolving dependencies to prevent cyclic dependencies
        public LoggingCompositionInterceptor(Lazy<ILoggerFactory> loggerFactory)
        {
            Throw.IfArgumentNull(loggerFactory, "loggerFactory");
            _loggerFactory = loggerFactory;
        }

        public object Intercept(object value)
        {
            var componentType = value.GetType();

            // Look for settable properties of type "ILogger" 
            var loggerProperties = componentType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(ILogger) && p.CanWrite);

            foreach (var property in loggerProperties)
            {
                var logger = _loggerFactory.Value.CreateLogger(componentType);
                property.SetValue(value, logger, null);
            }

            return value;
        }
    }
}
