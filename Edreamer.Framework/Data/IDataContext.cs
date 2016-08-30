using System;
using System.Collections.Generic;
using System.Transactions;

namespace Edreamer.Framework.Data
{
    public interface IDataContext: IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        int SaveChanges();
        //ISet<IEnlistmentNotification> Participants { get; }
    }
}
