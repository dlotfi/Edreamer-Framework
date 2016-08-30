// Based on Orchard CMS

using System.Collections.Generic;
using System.Web;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.Resources
{
    public class ResourceRegistrarContext
    {
        private readonly string _modulesPath;

        public ISet<ResourceDefinition> Resources { get; private set; }
        public string ModuleName { get; private set; }

        public ResourceRegistrarContext(string moduleName, string modulesPath)
        {
            Throw.IfArgumentNullOrEmpty(moduleName, "moduleName");
            Throw.IfArgumentNullOrEmpty(modulesPath, "modulesPath");
            _modulesPath = modulesPath;
            Resources = new HashSet<ResourceDefinition>();
            ModuleName = moduleName;
        }

        public virtual ResourceDefinition DefineResource(string resourceType, string resourceName)
        {
            Throw.IfArgumentNullOrEmpty(resourceType, "resourceType");
            Throw.IfArgumentNullOrEmpty(resourceName, "resourceName");
            var basePath = _modulesPath + ModuleName;
            var definition = new ResourceDefinition(basePath, resourceType, resourceName);
            Resources.Add(definition);
            return definition;
        }

        public ResourceDefinition DefineScript(string name)
        {
            return DefineResource("script", name);
        }

        public ResourceDefinition DefineStyle(string name)
        {
            return DefineResource("stylesheet", name);
        }
    }
}
