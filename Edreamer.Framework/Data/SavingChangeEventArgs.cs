using System;
using System.Collections.Generic;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Data
{
    public class SavingChangeEventArgs<T>: EventArgs
        where T : class
    {
        public SavingChangeEventArgs(ChangeType changeType, T entity)
            : this(changeType, entity, null)
        {
        }

        public SavingChangeEventArgs(ChangeType changeType, T entity, IEnumerable<string> modifiedProperties)
        {
            Throw.IfArgumentNull(entity, "entity");
            ChangeType = changeType;
            Entity = entity;
            ModifiedProperties = new HashSet<string>(CollectionHelpers.EmptyIfNull(modifiedProperties));
        }

        public ChangeType ChangeType { get; private set; }
        public T Entity { get; set; }
        public ISet<string> ModifiedProperties { get; private set; }
        public bool Cancel { get; set; }
    }

    public enum ChangeType
    {
        Add,
        Delete,
        Modify
    }
}
