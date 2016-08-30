using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Data.Infrastructure
{
    public class EntityEntry<T>
        where T : class
    {
        public EntityEntry(T entity, EntityState state)
        {
            Throw.IfArgumentNull(entity, "entity");
            Entity = entity;
            State = state;
        }
        
        public T Entity { get; private set; }
        public EntityState State { get; private set; }
    }
}
