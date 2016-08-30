using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Caching;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Mvc.ViewEngine;

namespace Edreamer.Framework.Mvc.WebParts
{
    public class PartialStore: IPartialStore
    {
        private readonly IViewLocationProvider _viewLocationProvider;

        protected IDictionary<string, string> Paths
        {
            get { return Cache.Get("Paths", ctx => new Dictionary<string, string>()); }
        }

        public ICache Cache { get; set; }

        public PartialStore(IViewLocationProvider viewLocationProvider)
        {
            Throw.IfArgumentNull(viewLocationProvider, "viewLocationProvider");
            _viewLocationProvider = viewLocationProvider;
        }

        public string GetPartialViewPath(string area, string viewPath)
        {
            Throw.IfArgumentNullOrEmpty(area, "area");
            Throw.IfArgumentNullOrEmpty(viewPath, "viewPath");

            string path;
            lock (Paths)
            {
                if (Paths.TryGetValue(area + ":" + viewPath, out path))
                    return path;

                path = _viewLocationProvider.AreaPartialViewLocationFormats
                    .Select(l => l.FormatWith(viewPath, null, area))
                    .Where(PathHelpers.FileExists)
                    .FirstOrDefault();
                if (path != null)
                {
                    Paths.Add(area + ":" + viewPath, path);
                }
            }
            return path;
        }
    }
}
