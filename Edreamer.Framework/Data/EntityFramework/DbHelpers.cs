using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Data.EntityFramework
{
    internal static class DbHelpers
    {
        public static EntityType GetEntityType<T>(ObjectContext context)
        {
            Throw.IfArgumentNull(context, "context");
            return GetCSpaceEntityType<T>(context.MetadataWorkspace);
        }

        public static ObjectStateEntry GetEntityEntry<T>(ObjectContext context, IEnumerable<KeyValuePair<string, object>> keys)
        {
            Throw.IfArgumentNull(context, "context");
            Throw.IfArgumentNullOrEmpty(keys, "keys");

            var entityType = GetCSpaceEntityType<T>(context.MetadataWorkspace);
            if (entityType == null)
                return null;

            var setName = GetEntitySetNames(context.MetadataWorkspace, entityType).SingleOrDefault();
            if (setName == null)
                return null;
            setName = context.DefaultContainerName + "." + setName;

            ObjectStateEntry foundEntry;
            if (context.ObjectStateManager.TryGetObjectStateEntry(new EntityKey(setName, keys), out foundEntry))
            {
                return foundEntry;
            }

            foundEntry = null;
            var addedEntries = context.ObjectStateManager.GetObjectStateEntries(EntityState.Added)
                .Where(e => !e.IsRelationship && (e.Entity != null) && typeof(T).IsAssignableFrom(e.Entity.GetType()));
            foreach (var entry in addedEntries)
            {
                if (keys.All(k => entry.CurrentValues[k.Key].Equals(k.Value)))
                {
                    Throw.If(foundEntry != null)
                        .A<InvalidOperationException>("Multiple entities were found in the Added state that match the given primary key values.");
                    foundEntry = entry;
                }
            }
           
            return foundEntry;
        }

        private static EntityType GetCSpaceEntityType<T>(MetadataWorkspace workspace)
        {
            Throw.IfArgumentNull(workspace, "workspace");

            // Make sure the assembly for "T" is loaded
            workspace.LoadFromAssembly(typeof(T).Assembly);

            // Try to get the ospace type and if that is found
            // look for the cspace type too.
            EntityType ospaceEntityType;
            if (workspace.TryGetItem(typeof(T).FullName, DataSpace.OSpace, out ospaceEntityType))
            {
                StructuralType cspaceEntityType;
                if (workspace.TryGetEdmSpaceType(ospaceEntityType, out cspaceEntityType))
                {
                    return cspaceEntityType as EntityType;
                }
            }
            return null;
        }

        private static IEnumerable<string> GetEntitySetNames(MetadataWorkspace workspace, EntityType entityType)
        {
            Throw.IfArgumentNull(workspace, "workspace");
            Throw.IfArgumentNull(entityType, "entityType");

            return from current in GetHierarchy(entityType)
                   from container in workspace.GetItems<EntityContainer>(DataSpace.CSpace)
                   from set in container.BaseEntitySets.OfType<EntitySet>().Where(e => e.ElementType == current)
                   select set.Name;
        }

        private static IEnumerable<EntityType> GetHierarchy(EntityType entityType)
        {
            Throw.IfArgumentNull(entityType, "entityType");

            while (entityType != null)
            {
                yield return entityType;
                entityType = entityType.BaseType as EntityType;
            }
        }
    }
}
