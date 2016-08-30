using System;
using System.Linq;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Media
{
    public class MediaPathProvider: IMediaPathProvider
    {
        private readonly ISettingsService _settingsService;
        private readonly IMimeDetector _mimeDetector;

        public MediaPathProvider(ISettingsService settingsService, IMimeDetector mimeDetector)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            Throw.IfArgumentNull(mimeDetector, "mimeDetector");
            _settingsService = settingsService;
            _mimeDetector = mimeDetector;
        }

        public string GetPath(Media media)
        {
            Throw.IfArgumentNull(media, "media");
            String path;
            switch ((media.TypeGroup ?? "").ToLower())
            {
                case "image": path = "Media\\Image\\"; break;
                case "video": path = "Media\\Video\\"; break;
                case "audio": path = "Media\\Audio\\"; break;
                default: path = "Media\\Other\\"; break;
            }
            var extension = _mimeDetector.GetExtension(media.Type);
            var unsafeExtensions = CollectionHelpers.EmptyIfNull(_settingsService.GetUnsafeExtensions());
            path = path + media.Id.ToString();
            if (!String.IsNullOrEmpty(extension) && !unsafeExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                return path + "." + extension.ToLower();
            return path;
        }
    }
}
