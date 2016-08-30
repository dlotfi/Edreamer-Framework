using System;
using System.Collections.Generic;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.Resources
{
    public class ResourcesWorkContext : IWorkContextStateProvider
    {
        public Func<IWorkContext, object> Get(string name)
        {
            if (name.EqualsIgnoreCase("RequiredResources"))
            {
                var result = new HashSet<RequireSettings>();
                return ctx => result;
            }
            else if (name.EqualsIgnoreCase("IncludedResources"))
            {
                var result = new HashSet<ResourceDefinition>();
                return ctx => result;
            }
            return null;
        }
    }
}
