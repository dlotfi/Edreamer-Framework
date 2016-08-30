using System.Web;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.Resources
{
    public class RequiredResourceContext
    {
        private readonly IResourceTagBuilder _resourceTagBuilder;

        public RequiredResourceContext(ResourceDefinition resource, RequireSettings settings, IResourceTagBuilder resourceTagBuilder)
        {
            Throw.IfArgumentNull(resource, "resource");
            Throw.IfArgumentNull(settings, "settings");
            Throw.IfArgumentNull(resourceTagBuilder, "resourceTagBuilder");
            Resource = resource;
            Settings = settings;
            _resourceTagBuilder = resourceTagBuilder;
        }

        public ResourceDefinition Resource { get; private set; }
        public RequireSettings Settings { get; private set; }
        
        public IHtmlString GetHtml(bool absoluteUrl = false)
        {
            var resourcePath = Resource.ResolveUrl(Settings);
            var attributes = CollectionHelpers.MergeDictionaries(Resource.Attributes, Settings.Attributes);
            return _resourceTagBuilder.BuildResourceTag(Resource.Type, resourcePath, attributes, Settings.Condition, absoluteUrl);
        }
    }
}
