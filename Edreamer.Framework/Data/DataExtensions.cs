using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Data
{
    public static class DataExtensions
    {
        public static void Load<T>(this IQueryable<T> source)
        {
            Throw.IfArgumentNull(source, "source");
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                }
            }
        }
    }
}
