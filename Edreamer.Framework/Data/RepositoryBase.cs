using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Edreamer.Framework.Collections;
using Edreamer.Framework.Data.Infrastructure;
using Edreamer.Framework.Helpers;


namespace Edreamer.Framework.Data
{
    public abstract class RepositoryBase<T> : IRepository<T>
        where T : class
    {
        private TupleList<T, T> _detachedUpdatedEntities = new TupleList<T, T>();

        #region Properties
        protected bool Tracking { get; set; }
        protected bool Including { get; set; }
        protected IQueryable<T> Query { get; set; }
        #endregion

        #region Abstract Members
        protected abstract RepositoryBase<T> CreateRepository(IQueryable<T> query, bool including, bool tracking);  // Factory Method
        protected abstract IEnumerable<EntityProperty> Properties { get; }
        protected abstract EntityEntry<T> GetEntityEntry(T entity);
        protected abstract EntityEntry<T> GetEntityEntry(IEnumerable<object> keyValues);
        protected abstract IQueryable<T> SuppressTracking(IQueryable<T> query);
        protected abstract IQueryable<T> Include<TProperty>(IQueryable<T> query, Expression<Func<T, TProperty>> path);
        protected abstract void ChangeEntityState(T entity, EntityState newState, IEnumerable<string> modifiedProperties = null);
        protected abstract void AttachObject(T entity, bool added = false);
        protected abstract void DetachObject(T entity);
        #endregion

        #region IRepository Implementation

        #region IQueryRepository Implementation

        public IQueryRepository<T> WithTracking()
        {
            return CreateRepository(Query, Including, true);
        }

        public IQueryRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return CreateRepository(Include(Query, path), true, Tracking);
        }

        public T Find(params object[] keyValues)
        {
            Throw.IfArgumentNullOrEmpty(keyValues, "keyValues");
            CheckAndThrowExceptionIfKeysNotCorrect(keyValues);

            // Return tracked entity if exists and its state is added or no including is required
            if (Tracking)
            {
                var entry = GetEntityEntry(keyValues);
                if (entry != null && (entry.State == EntityState.Added || !Including))
                {
                    return entry.Entity;
                }
            }

            // Query the data source
            return this.SingleOrDefault(BuildFindByKeysPredicate(keyValues));
        }

        #region IEnumerable Implementation
        public IEnumerator<T> GetEnumerator()
        {
            //return Tracking ? Query.GetEnumerator() : SuppressTracking(Query).GetEnumerator();
            return this.Where(x => true).GetEnumerator(); //Adding Where to prevent recursive call
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region IQueryable Implementation
        public Type ElementType
        {
            get { return Query.ElementType; }
        }

        public Expression Expression
        {
            get
            {
                var query = Tracking ? Query : SuppressTracking(Query);
                return query.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get { return Query.Provider; }
        }
        #endregion

        #endregion

        public void Attach(T entity)
        {
            Throw.IfArgumentNull(entity, "entity");
            lock (this)
            {
                var entry = GetEntityEntry(entity);
                if (entry != null) return;
                entry = GetEntityEntry(GetKeyValues(entity));
                Throw.If(entry != null)
                    .A<InvalidOperationException>("Another entity with the same key is already attached.");
                AttachObject(entity);
            }
        }

        public void Add(T entity)
        {
            Throw.IfArgumentNull(entity, "entity");
            lock (this)
            {
                var entry = GetEntityEntry(entity);
                Throw.If(entry != null && entry.State == EntityState.Modified)
                        .A<InvalidOperationException>("It's not possible to add an entity already marked as modified.");
                if (entry == null)
                {
                    AttachObject(entity, true);
                }
                ChangeEntityState(entity, EntityState.Added);
            }
        }

        public void Update(T entity)
        {
            Update(entity, null, PropertyInclusion.Exclude);
        }

        public void Update(T entity, Expression<Func<T, object>> properties, PropertyInclusion inclusion = PropertyInclusion.Include)
        {
            Throw.IfArgumentNull(entity, "entity");

            // Specify which properties to include or exclude
            var targetProperties = (properties == null)
                                       ? (IEnumerable<string>) new string[0]
                                       : properties.FindProperties().Select(mi => mi.Name).ToList();
            if (inclusion == PropertyInclusion.Exclude)
            {
                targetProperties = Properties.Select(p => p.Name).Except(targetProperties, StringComparer.OrdinalIgnoreCase);
            }

            Throw.IfNot(targetProperties.Any())
                .AnArgumentException(inclusion == PropertyInclusion.Include
                    ? "You should at least include one property in update."
                    : "You cannot exclude all properties in update.", "properties");

            // Perform updating
            lock (this)
            {
                var anotherEntityWithTheSameKeyAlreadyAttached = false;
                var entry = GetEntityEntry(entity);
                if (entry == null)
                {
                    entry = GetEntityEntry(GetKeyValues(entity));
                    anotherEntityWithTheSameKeyAlreadyAttached = (entry != null);
                }
                Throw.If(entry != null && entry.State == EntityState.Deleted)
                    .A<InvalidOperationException>("It's not possible to update an entity already marked for deletion.");
                Throw.If(entry != null && entry.State == EntityState.Added)
                    .A<InvalidOperationException>("It's not possible to update an entity which has been added but not saved yet.");
                if (entry == null)
                {
                    AttachObject(entity);
                }
                else if (anotherEntityWithTheSameKeyAlreadyAttached)
                {
                    CopyProperties(entity, entry.Entity, targetProperties);
                    _detachedUpdatedEntities.Add(entry.Entity, entity);
                    entity = entry.Entity;
                }
                ChangeEntityState(entity, EntityState.Modified, targetProperties);
            }
        }

        public void Remove(T entity)
        {
            Throw.IfArgumentNull(entity, "entity");
            lock (this)
            {
                var entry = GetEntityEntry(entity);
                if (entry != null && entry.State == EntityState.Added)
                {
                    DetachObject(entry.Entity);
                }
                Attach(entity); // Nothing happens if already attached. Throws an exception if another entity with the same key exists.
                ChangeEntityState(entity, EntityState.Deleted);
            }
        }

        public void Remove(byte[] timestamp, params object[] keyValues)
        {
            Throw.IfArgumentNullOrEmpty(keyValues, "keyValues");
            CheckAndThrowExceptionIfKeysNotCorrect(keyValues);
            lock (this)
            {
                T entity;
                var entry = GetEntityEntry(keyValues);
                if (entry != null)
                {
                    entity = entry.Entity;
                }
                else
                {
                    var entityActivator = ObjectFactory.GetActivator<T>();
                    Throw.IfNull(entityActivator)
                        .A<RemoveByKeyException>("Entity of type '{0}' does not have a parameterless constructor.".FormatWith(typeof(T).Name));
                    entity = entityActivator();
                    AssignKeys(entity, keyValues);
                }
                AssignTimestamp(entity, timestamp);

                Remove(entity);
            }
        }

        public event EventHandler<SavingChangeEventArgs<T>> SavingChange;

        #endregion

        protected virtual void OnSavingChange(SavingChangeEventArgs<T> e)
        {
            if (SavingChange != null)
            {
                SavingChange(this, e);
            }
        }

        protected virtual void OnSavedChanges(object sender, EventArgs e)
        {
            // Update concurrency properties (timestamp) of detached updated entities
            var concurrencyProperties = Properties.Where(p => p.Concurrency).Select(p => p.Name).ToList();
            if (!concurrencyProperties.Any()) return;
            foreach (var detachedUpdatedEntity in _detachedUpdatedEntities)
            {
                var attachedEntity = detachedUpdatedEntity.Item1;
                var detachedEntity = detachedUpdatedEntity.Item2;
                CopyProperties(attachedEntity, detachedEntity, concurrencyProperties);
            }
        }

        protected Expression<Func<T, bool>> BuildFindByKeysPredicate(IEnumerable<object> keyValues)
        {
            Throw.IfArgumentNullOrEmpty(keyValues, "keyValues");
            var keyProperties = Properties.Where(p => p.KeyIndex >= 0).OrderBy(p => p.KeyIndex);
            var keys = CollectionHelpers.CombineKeysAndValues(keyProperties.Select(k => k.Name), keyValues).ToList();
            Expression condition = Expression.Constant(true);
            ParameterExpression entityType = Expression.Parameter(typeof(T), "ent");
            foreach (var key in keys)
            {
                Expression equality = Expression.Equal(Expression.Property(entityType, key.Key), Expression.Constant(key.Value));
                condition = Expression.AndAlso(condition, equality);
            }
            return Expression.Lambda<Func<T, bool>>(condition, entityType);
        }

        protected void CheckAndThrowExceptionIfKeysNotCorrect(params object[] keyValues)
        {
            var keyProperties = new List<EntityProperty>(Properties.Where(p => p.KeyIndex >= 0).OrderBy(p => p.KeyIndex));
            Throw.If(keyProperties.Count() != keyValues.Length)
                .AnArgumentException("Entity contains {0} keys but {1} keys have been passed.".FormatWith(keyProperties.Count(), keyValues.Length));

            for (int i = 0; i < keyValues.Length; i++)
            {
                Throw.IfNot(keyProperties[i].Type.IsAssignableFrom(keyValues[i].GetType()))
                .AnArgumentException("The value passed for the key '{0}' should be of type '{1}.".FormatWith(keyProperties[i].Name, keyProperties[i].Type.Name));
            }
        }

        protected void AssignTimestamp(T entity, byte[] timestamp)
        {
            Throw.IfArgumentNull(entity, "entity");

            var concurrencyProperties = Properties.Where(p => p.Concurrency).ToList();
            Throw.If(concurrencyProperties.Count() > 1)
                .A<InvalidOperationException>("Entity contains multiple properties for concurrency check.");
            var timestampProperty = concurrencyProperties.Where(p => p.Type == typeof(byte[])).Select(p => p.Name).FirstOrDefault();
            if (timestampProperty == null) return;
            Throw.IfArgumentNull(timestamp, "timestamp");

            var timestampPropertyInfo = typeof(T).GetPropertyInfo(timestampProperty, typeof(byte[]), PropertyAccessRequired.Set);
            Throw.IfNull(timestampPropertyInfo)
                .A<InvalidOperationException>("The entity does not have a non-readonly timestamp property named '{0}'.".FormatWith(timestampProperty));
            timestampPropertyInfo.SetValue(entity, timestamp, null);
        }

        protected void AssignKeys(T entity, IEnumerable<object> keyValues)
        {
            Throw.IfArgumentNull(entity, "entity");
            Throw.IfArgumentNullOrEmpty(keyValues, "keyValues");
            var keyProperties = Properties.Where(p => p.KeyIndex >= 0).OrderBy(p => p.KeyIndex);
            var keys = CollectionHelpers.CombineKeysAndValues(keyProperties, keyValues).ToList();

            foreach (var key in keys)
            {
                var keyPropertyInfo = typeof(T).GetPropertyInfo(key.Key.Name, key.Key.Type, PropertyAccessRequired.Set);
                Throw.IfNull(keyPropertyInfo)
                    .A<InvalidOperationException>("The entity does not have a key property named '{0}' of type '{1}.".FormatWith(key.Key.Name, key.Key.Type.Name));
                keyPropertyInfo.SetValue(entity, key.Value, null);
            }
        }

        protected IEnumerable<object> GetKeyValues(T entity)
        {
            Throw.IfArgumentNull(entity, "entity");
            var keyProperties = Properties.Where(p => p.KeyIndex >= 0).OrderBy(p => p.KeyIndex);
            var values = new List<object>();
            foreach (var keyProperty in keyProperties)
            {
                var keyPropertyInfo = typeof(T).GetPropertyInfo(keyProperty.Name, keyProperty.Type, PropertyAccessRequired.Get);
                Throw.IfNull(keyPropertyInfo)
                    .A<InvalidOperationException>("The entity does not have a key property named '{0}' of type '{1}.".FormatWith(keyProperty.Name, keyProperty.Type.Name));
                values.Add(keyPropertyInfo.GetValue(entity, null));
            }
            return values;
        }

        private static void CopyProperties(T source, T target, IEnumerable<string> properties)
        {
            Throw.IfArgumentNull(source, "source");
            Throw.IfArgumentNull(target, "target");
            Throw.IfArgumentNullOrEmpty(properties, "properties");
            foreach (var property in properties)
            {
                var propertyInfo = typeof(T).GetPropertyInfo(property, PropertyAccessRequired.All);
                var sourceValue = propertyInfo.GetValue(source, null);
                propertyInfo.SetValue(target, sourceValue, null);
            }
        }
    }
}
