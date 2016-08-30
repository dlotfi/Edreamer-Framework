// Based on Orchard CMS

using System;
using System.IO;
using System.Web.Hosting;

namespace Edreamer.Framework.Helpers
{
    public static class PathHelpers
    {
        public static string GetPhysicalPath(string path)
        {
            Throw.IfArgumentNullOrEmpty(path, "path");

            if (path.StartsWith("~") && HostingEnvironment.IsHosted)
            {
                return HostingEnvironment.MapPath(path);
            }
            return path;

            //if (VirtualPathUtility.IsAppRelative(path))
            //{
            //    var physicalPath = VirtualPathUtility.ToAbsolute(path, "/");
            //    physicalPath = physicalPath.Replace("/", "");
            //    physicalPath = physicalPath.Substring(1);
            //    physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, physicalPath);
            //}
        }

        public static string GetVirtualPath(string path)
        {
            Throw.IfArgumentNullOrEmpty(path, "path");

            if (!path.StartsWith("~") && HostingEnvironment.IsHosted)
            {
                path = "~" + path.TrimStart(StringComparison.OrdinalIgnoreCase, HostingEnvironment.ApplicationPhysicalPath);
                return path;
            }
            return path;
        }

        public static bool FileExists(string virtualPath)
        {
            return File.Exists(GetPhysicalPath(virtualPath));
        }
    }
}