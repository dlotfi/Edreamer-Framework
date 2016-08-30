using System;
using System.Collections.Generic;
using System.Transactions;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Data
{
    public abstract class DataContextBase : IDataContext
    {
        protected IDictionary<Type, object> Repositories { get; private set; }
        //public ISet<IEnlistmentNotification> Participants { get; private set; }

        protected DataContextBase()
        {
            //Participants = new HashSet<IEnlistmentNotification>();
            Repositories = new Dictionary<Type, object>();
        }

        protected abstract int DoSaveChanges();
        protected abstract IRepository<T> CreateRepository<T>() where T : class;
        protected abstract void Dispose(bool disposing);

        public virtual int SaveChanges()
        {
            int affectedObjects;
            using (var scope = new TransactionScope())
            {
                affectedObjects = DoSaveChanges();
                //foreach (var participant in Participants)
                //{
                //    Throw.IfNull(participant).A<InvalidOperationException>("Cannot save changes when a null participant has been bound to the current data context.");
                //    Transaction.Current.EnlistVolatile(participant, EnlistmentOptions.None);
                //}
                scope.Complete();
            }
            return affectedObjects;
        }

        public virtual IRepository<T> Repository<T>() where T : class
        {
            if (!Repositories.ContainsKey(typeof(T)))
            {
                Repositories.Add(typeof(T), CreateRepository<T>());
            }
            return Repositories[typeof(T)] as IRepository<T>;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DataContextBase()
        {
            Dispose(false);
        }
    }
}
