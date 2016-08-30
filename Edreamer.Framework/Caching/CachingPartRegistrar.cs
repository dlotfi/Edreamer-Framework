using System.ComponentModel.Composition.Registration;
using Edreamer.Framework.Caching.VolatileProviders;
using Edreamer.Framework.Composition;

namespace Edreamer.Framework.Caching
{
    public class CachingPartRegistrar: IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<ICacheFactory>()
                .Export<ICacheFactory>()
                .SetAsSingleton();

            rb.ForTypesDerivedFrom<ICacheContextAccessor>()
                .Export<ICacheContextAccessor>()
                .SetAsSingleton();

            rb.ForTypesDerivedFrom<IClock>()
                .Export<IClock>();

            rb.ForTypesDerivedFrom<ISignals>()
                .Export<ISignals>();
        }
    }
}
