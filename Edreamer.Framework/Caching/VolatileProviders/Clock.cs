// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Caching.VolatileProviders
{
    public interface IClock 
    {
        /// <summary>
        /// Each retrieved value is cached during the specified amount of time.
        /// </summary>
        IVolatileToken When(TimeSpan duration);

        /// <summary>
        /// The cache is active until the specified time. Each subsequent access won't be cached.
        /// </summary>
        IVolatileToken WhenUtc(DateTime absoluteUtc);
    }

    public class Clock : IClock
    {
        public IVolatileToken When(TimeSpan duration)
        {
            return new AbsoluteExpirationToken(duration);
        }

        public IVolatileToken WhenUtc(DateTime absoluteUtc)
        {
            return new AbsoluteExpirationToken(absoluteUtc);
        }

        public class AbsoluteExpirationToken : IVolatileToken
        {
            private readonly DateTime _invalidateUtc;

            public AbsoluteExpirationToken(DateTime invalidateUtc)
            {
                _invalidateUtc = invalidateUtc;
            }

            public AbsoluteExpirationToken(TimeSpan duration)
            {
                _invalidateUtc = DateTime.UtcNow.Add(duration);
            }

            public bool IsCurrent
            {
                get {
                    return DateTime.UtcNow < _invalidateUtc;
                }
            }
        }
    }
}
