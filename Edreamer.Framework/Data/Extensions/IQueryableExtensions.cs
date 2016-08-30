using System.Linq;

namespace Edreamer.Framework.Data.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> source, int page, int pageSize)
        {
            //source = source is IOrderedQueryable<T> ? source : source.OrderBy(s => s);
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> source, int page, int pageSize, out int totalItemsCount)
        {
            //source = source is IOrderedQueryable<T> ? source : source.OrderBy(s => s);
            totalItemsCount = source.Count();
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
