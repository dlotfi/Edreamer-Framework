// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Caching
{
    public interface ICacheContextAccessor
    {
        IAcquireContext Current { get; set; }
    }

    public class CacheContextAccessor : ICacheContextAccessor 
    {
        [ThreadStatic]
        private static IAcquireContext _threadInstance;

        public static IAcquireContext ThreadInstance 
        {
            get { return _threadInstance; }
            set { _threadInstance = value; }
        }

        public IAcquireContext Current 
        {
            get { return ThreadInstance; }
            set { ThreadInstance = value; }
        }
    }
}