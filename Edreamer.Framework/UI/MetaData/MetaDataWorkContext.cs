using System;
using System.Collections.Generic;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.MetaData
{
    public class MetaDataWorkContext : IWorkContextStateProvider
    {
        public Func<IWorkContext, object> Get(string name)
        {
            if (name.EqualsIgnoreCase("IncludedMetaData"))
            {
                var result = new HashSet<MetaEntry>();
                return ctx => result;
            }
            return null;
        }
    }
}
