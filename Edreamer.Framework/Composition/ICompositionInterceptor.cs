using System.Collections.Generic;
using System.Collections.ObjectModel;
using MefContrib.Hosting.Interception;

namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// Represents an interceptor which can intercept exported values and objects whose imports are satisfied by the container.
    /// </summary>
    public interface ICompositionInterceptor
    {
        /// <summary>
        /// Intercepts an exported value or an object when it's imports are being satisfied.
        /// </summary>
        /// <param name="value">The value to be intercepted.</param>
        /// <returns>Intercepted value.</returns>
        object Intercept(object value);
    }

    internal class CompositionInterceptor: IExportedValueInterceptor
    {
        public ICollection<ICompositionInterceptor> Interceptors { get; private set; }

        public CompositionInterceptor()
        {
            Interceptors = new Collection<ICompositionInterceptor>();
        }

        public object Intercept(object value)
        {
            foreach (var interceptor in Interceptors)
            {
                value = interceptor.Intercept(value);
            }
            return value;
        }
    }
}