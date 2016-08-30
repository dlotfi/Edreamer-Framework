using System;

namespace Edreamer.Framework.Context
{
    public interface IWorkContextStateProvider
    {
        Func<IWorkContext, object> Get(string name);
    }
}
