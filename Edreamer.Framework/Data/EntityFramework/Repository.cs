using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using Edreamer.Framework.Data.Infrastructure;
using Edreamer.Framework.Helpers;
using EntityState = System.Data.Entity.EntityState;

namespace Edreamer.Framework.Data.EntityFramework
{
    public class Repository<T> : RepositoryBase<T>
        where T : class
    {
        private readonly DataContext _context;
        private readonly ObjectContext _objectContext;
        private readonly IObjectSet<T> _objectSet;
        private ICollection<EntityProperty> _properties;

        public Repository(DataContext context)
        {
            Throw.IfArgumentNull(context, "context");
            _context = context;
            _objectContext = ((IObjectContextAdapter)context).ObjectContext;
            _objectContext.SavingChanges += OnContextSavingChanges;
            _context.SavedChanges += OnSavedChanges;

            var baseType = GetBaseEntityType();
            if (baseType == typeof(T))
            {
                _objectSet = _objectContext.CreateObjectSet<T>();
                Query = _objectSet;
            }
            else // Entity is inherited from another entity
            {
                _objectSet = (IObjectSet<T>)ObjectFactory.CreateInstance(typeof(ObjectSetProxy<,>).MakeGenericType(typeof(T), baseType), _objectContext);
                Query = (_objectSet as IObjectSetProxy<T>).GetObjectQuery(); //Include needs ObjectQuery<T> instance in order to work
            }
        }

        private void OnContextSavingChanges(object sender, EventArgs e)
        {
            // must call to make sure changes are detected
            _objectContext.DetectChanges();

            var changedEntries = _objectContext.ObjectStateManager
                .GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified)
                .Where(x => x.Entity != null && x.Entity.GetType() == typeof(T));

            foreach (var changedEntry in changedEntries)
            {
                var entry = changedEntry; // Prevents access to modified closure
                var entity = entry.Entity as T;
                SavingChangeEventArgs<T> eventArg;
                switch (entry.State)
                {
                    case EntityState.Added:
                        eventArg = new SavingChangeEventArgs<T>(ChangeType.Add, entity);
                        OnSavingChange(eventArg);
                        if (eventArg.Cancel)
                            entry.ChangeState(EntityState.Unchanged);
                        break;
                    case EntityState.Deleted:
                        eventArg = new SavingChangeEventArgs<T>(ChangeType.Delete, entity);
                        OnSavingChange(eventArg);
                        if (eventArg.Cancel)
                            entry.ChangeState(EntityState.Unchanged);
                        break;
                    case EntityState.Modified:
                        var modifiedProperties = entry.GetModifiedProperties();
                        eventArg = new SavingChangeEventArgs<T>(ChangeType.Modify, entity, modifiedProperties);
                        OnSavingChange(eventArg);
                        if (eventArg.Cancel)
                            entry.ChangeState(EntityState.Unchanged);
                        else if (!eventArg.ModifiedProperties.SetEquals(modifiedProperties))
                        {
                            entry.ApplyOriginalValues(eventArg.Entity); // Useful when some modified properties removed from the list
                            // By resetting original values, later change detections will not incorrectly mark properties as modified
                            // because of changes in their values
                            ChangeEntityState(entity, Infrastructure.EntityState.Modified, eventArg.ModifiedProperties);
                        }
                        break;
                }
            }
        }

        #region RepositoryBase Implementation
        protected override RepositoryBase<T> CreateRepository(IQueryable<T> query, bool including, bool tracking)
        {
            Throw.IfArgumentNull(query, "query");
            return new Repository<T>(_context)
                       {
                           Query = query,
                           Including = including,
                           Tracking = tracking
                       };
        }

        protected override IEnumerable<EntityProperty> Properties
        {
            get
            {
                if (_properties == null)
                {
                    var entityType = DbHelpers.GetEntityType<T>(_objectContext);
                    var properties = new List<EntityProperty>();
                    var keys = entityType.KeyMembers;
                    foreach (var property in entityType.Properties)
                    {
                        var keyIndex = keys.IndexOf(property);
                        var isForConcurrency = property.TypeUsage.Facets != null &&
                                               property.TypeUsage.Facets.Any(f => f.Name == "ConcurrencyMode" && (ConcurrencyMode)f.Value == ConcurrencyMode.Fixed);
                        properties.Add(new EntityProperty(property.Name, ConvertEdmTypeToClrType(property.TypeUsage.EdmType), keyIndex, isForConcurrency));
                    }
                    _properties = new List<EntityProperty>(properties);
                }
                return _properties;
            }
        }

        protected override EntityEntry<T> GetEntityEntry(T entity)
        {
            Throw.IfArgumentNull(entity, "entity");
            ObjectStateEntry entry;
            return _objectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry)
                      ? new EntityEntry<T>(entry.Entity as T, ConvertToEntityState(entry.State))
                      : null;
        }

        protected override EntityEntry<T> GetEntityEntry(IEnumerable<object> keyValues)
        {
            Throw.IfArgumentNullOrEmpty(keyValues, "keyValues");
            var keyNames = Properties.Where(p => p.KeyIndex >= 0).OrderBy(p => p.KeyIndex).Select(p => p.Name);
            var keyNamesAndValues = CollectionHelpers.CombineKeysAndValues(keyNames, keyValues);
            var entry = DbHelpers.GetEntityEntry<T>(_objectContext, keyNamesAndValues);
            return entry == null
                       ? null
                       : new EntityEntry<T>(entry.Entity as T, ConvertToEntityState(entry.State));
        }

        protected override IQueryable<T> SuppressTracking(IQueryable<T> query)
        {
            Throw.IfArgumentNull(query, "query");
            return query.AsNoTracking();
        }

        protected override IQueryable<T> Include<TProperty>(IQueryable<T> query, Expression<Func<T, TProperty>> path)
        {
            Throw.IfArgumentNull(query, "query");
            Throw.IfArgumentNull(path, "path");
            return query.Include(path);
        }

        protected override void AttachObject(T entity, bool added = false)
        {
            Throw.IfArgumentNull(entity, "entity");
            if (added)
            {
                _objectSet.AddObject(entity);
            }
            else
            {
                _objectSet.Attach(entity);
            }
        }

        protected override void DetachObject(T entity)
        {
            Throw.IfArgumentNull(entity, "entity");
            _objectSet.Detach(entity);
        }

        protected override void ChangeEntityState(T entity, Infrastructure.EntityState newState, IEnumerable<string> modifiedProperties = null)
        {
            Throw.IfArgumentNull(entity, "entity");
            if (newState == Infrastructure.EntityState.Modified) // Just mark modified properties and leave others.
            {                                                    // This is useful when e.g. updating an entity multiple times.   
                Throw.IfArgumentNull(modifiedProperties, "modifiedProperties");
                if (!modifiedProperties.Any()) return;
                var entityEntry = _context.Entry(entity);
                foreach (var modifiedProperty in modifiedProperties)
                {
                    try
                    {
                        entityEntry.Property(modifiedProperty).IsModified = true;
                    }
                    catch (Exception ex)
                    {
                        Throw.Now.A<PartialUpdateException>(
                            "An error occured while trying to access property '{0}' of the entity '{1}."
                            .FormatWith(modifiedProperty, typeof(T).Name),
                            ex, entity);
                    }
                }
            }
            else
            {
                _objectContext.ObjectStateManager.ChangeObjectState(entity, ConvertFromEntityState(newState));    
            }
        }
        #endregion

        #region Private Methods

        private static Type GetBaseEntityType()
        {
            //naive implementation that assumes the first class in the hierarchy derived from object is the "base" type used by EF
            var type = typeof(T);

            while (type.BaseType != typeof(object))
            {
                type = type.BaseType;
            }

            return type;
        }


        private static Type ConvertEdmTypeToClrType(EdmType edmType)
        {
            Throw.IfArgumentNull(edmType, "edmType");

            var primitiveType = edmType as PrimitiveType;
            if (primitiveType != null)
            {
                return primitiveType.ClrEquivalentType;
            }

            //ToDo-High [12131150]: Find a solution for converting non primitive edm types or eliminate the requirement to this conversion
            return  Type.GetType("Asaar.Common.{0}, Asaar.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null".FormatWith(edmType.Name)) ??
                    typeof(object);
        }

        private static Infrastructure.EntityState ConvertToEntityState(EntityState entityState)
        {
            switch (entityState)
            {
                case EntityState.Unchanged:
                    return Infrastructure.EntityState.Unchanged;
                case EntityState.Added:
                    return Infrastructure.EntityState.Added;
                case EntityState.Deleted:
                    return Infrastructure.EntityState.Deleted;
                case EntityState.Modified:
                    return Infrastructure.EntityState.Modified;
                default:
                    Throw.Now.AnArgumentException("Entity state '{0}' is not supported.".FormatWith(entityState.ToString()), "entityState");
                    return 0;
            }
        }

        private static EntityState ConvertFromEntityState(Infrastructure.EntityState entityState)
        {
            switch (entityState)
            {
                case Infrastructure.EntityState.Added:
                    return EntityState.Added;
                case Infrastructure.EntityState.Deleted:
                    return EntityState.Deleted;
                case Infrastructure.EntityState.Modified:
                    return EntityState.Modified;
                default:
                    return EntityState.Unchanged;
            }
        }

        #endregion
    }

    internal class ObjectSetProxy<TEntity, TBase> : IObjectSet<TEntity>, IObjectSetProxy<TEntity>
        where TEntity : class, TBase
        where TBase : class
    {
        private readonly IObjectSet<TBase> _baseSet;

        public ObjectSetProxy(ObjectContext context)
        {
            _baseSet = context.CreateObjectSet<TBase>();
        }

        public IQueryable<TEntity> GetObjectQuery()
        {
            return _baseSet.OfType<TEntity>();
        }

        public void AddObject(TEntity entity)
        {
            _baseSet.AddObject(entity);
        }

        public void Attach(TEntity entity)
        {
            _baseSet.Attach(entity);
        }

        public void DeleteObject(TEntity entity)
        {
            _baseSet.DeleteObject(entity);
        }

        public void Detach(TEntity entity)
        {
            _baseSet.Detach(entity);
        }

        #region IQueryable NotSupported
        public IEnumerator<TEntity> GetEnumerator()
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        public Type ElementType
        {
            get { throw new NotSupportedException(); }
        }

        public Expression Expression
        {
            get { throw new NotSupportedException(); }
        }

        public IQueryProvider Provider
        {
            get { throw new NotSupportedException(); }
        }
        #endregion
    }

    internal interface IObjectSetProxy<TEntity>
    {
        IQueryable<TEntity> GetObjectQuery();
    }
}
