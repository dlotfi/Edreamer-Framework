using System.Collections.Generic;

namespace Edreamer.Framework.Mvc.ViewEngine
{
    public interface IViewLocationProvider
    {
        IEnumerable<string> AreaViewLocationFormats { get; }

        IEnumerable<string> AreaMasterLocationFormats { get; }

        IEnumerable<string> AreaPartialViewLocationFormats { get; }

        IEnumerable<string> LayoutLocationFormats { get; }

        IEnumerable<string> TemplateLocationFormats { get; }

        string GetBasePathFromVirtualPath(string viewPath);
    }
}
