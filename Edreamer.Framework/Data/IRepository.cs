using System;
using System.Linq.Expressions;

namespace Edreamer.Framework.Data
{
    public interface IRepository<T> : IQueryRepository<T>
        where T : class
    {
        void Attach(T entity);
        void Add(T entity);
        void Update(T entity);
        void Update(T entity, Expression<Func<T, object>> properties, PropertyInclusion inclusion = PropertyInclusion.Include);
        void Remove(T entity);
        void Remove(byte[] timestamp, params object[] keyValues);

        event EventHandler<SavingChangeEventArgs<T>> SavingChange;
    }

    public enum PropertyInclusion
    {
        Include,
        Exclude
    }
}
