using System.Collections.Generic;
using System.Web;

namespace Edreamer.Framework.UI.Resources
{
    public interface IResourceTagBuilder
    {
        IHtmlString BuildResourceTag(string type, string path, IDictionary<string, string> attributes, string condition, bool absoluteUrl);
    }
}
