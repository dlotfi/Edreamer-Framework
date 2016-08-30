// Based on Orchard CMS

namespace Edreamer.Framework.Caching
{
    public interface IVolatileToken
    {
        bool IsCurrent { get; }
    }
}