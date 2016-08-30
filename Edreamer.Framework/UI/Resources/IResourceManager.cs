using System.Collections.Generic;

namespace Edreamer.Framework.UI.Resources
{
    public interface IResourceManager
    {
        IEnumerable<RequiredResourceContext> GetAllRequiredResources();
        RequiredResourceContext GetRequiredResource(RequireSettings settings);
        RequireSettings Require(string resourceType, string resourceName);
        RequireSettings Include(ResourceDefinition resource);
    }
}
