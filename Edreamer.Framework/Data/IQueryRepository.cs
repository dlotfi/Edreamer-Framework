using System;
using System.Linq;
using System.Linq.Expressions;

namespace Edreamer.Framework.Data
{
    public interface IQueryRepository<T> : IQueryable<T>
        where T: class
    {
        IQueryRepository<T> WithTracking();
        IQueryRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> path);
        T Find(params object[] keyValues);
    }
}
