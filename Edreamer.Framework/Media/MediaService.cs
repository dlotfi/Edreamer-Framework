using System;
using System.IO;
using System.Linq;
using Edreamer.Framework.Domain;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Media.Storage;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Media
{
    public class MediaService : IMediaService
    {
        private readonly IFrameworkDataContext _dataContext;
        private readonly IStorageProvider _storageProvider;
        private readonly IMediaPathProvider _mediaPathProvider;
        private readonly ISettingsService _settingsService;
        private readonly IMimeDetector _mimeDetector;

        public MediaService(IFrameworkDataContext dataContext, IStorageProvider storageProvider,
            IMediaPathProvider mediaPathProvider, ISettingsService settingsService, IMimeDetector mimeDetector)
        {
            Throw.IfArgumentNull(dataContext, "dataContext");
            Throw.IfArgumentNull(storageProvider, "storageProvider");
            Throw.IfArgumentNull(mediaPathProvider, "mediaPathProvider");
            Throw.IfArgumentNull(settingsService, "settingsService");
            Throw.IfArgumentNull(mimeDetector, "mimeDetector");
            _dataContext = dataContext;
            _storageProvider = storageProvider;
            _mediaPathProvider = mediaPathProvider;
            _settingsService = settingsService;
            _mimeDetector = mimeDetector;
        }


        public Media CreateMedia(Stream source, string name = null, int id = 0, bool disposeSource = true)
        {
            Throw.IfArgumentNull(source, "source");
            if (source.CanSeek)
            {
                source.Seek(0, SeekOrigin.Begin);
            }
            var data = source.ConvertToByteArray();
            var media = new Media
                {
                    Data = data,
                    Id = id,
                    Type = _mimeDetector.GetMimeType(data, name)
                };
            if (disposeSource)
            {
                source.Dispose();
            }
            return media;
        }

        public Media AddMedia(Media media)
        {
            // ToDo-Low [02061949]: Think of another method to add media to database in one step
            Throw.IfArgumentNull(media, "media");
            Throw.IfNull(media.Data).AnArgumentException("Data cannot be null.", "media");
            Throw.IfNot(IsMediaAcceptable(media)).AnArgumentException("This type of media is not acceptable", "media");
            var mediaEntity = new Domain.Media { Type = media.Type, Path = "-" };
            _dataContext.Media.Add(mediaEntity);
            _dataContext.SaveChanges();
            media.Id = mediaEntity.Id;
            mediaEntity.Path = _mediaPathProvider.GetPath(media);
            if (_storageProvider.FileExists(mediaEntity.Path))
            {
                _storageProvider.DeleteFile(mediaEntity.Path);
            }
            _storageProvider.SaveStream(mediaEntity.Path, new MemoryStream(media.Data));
            _dataContext.Media.Update(mediaEntity);
            _dataContext.SaveChanges();
            return media;
        }

        public void UpdateMedia(Media media)
        {
            Throw.IfArgumentNull(media, "media");
            Throw.IfNot(IsMediaAcceptable(media)).AnArgumentException("This type of media is not acceptable", "media");
            var mediaEntity = new Domain.Media
                                  {
                                      Id = media.Id,
                                      Path = _mediaPathProvider.GetPath(media),
                                      Type = media.Type
                                  };
            if (_storageProvider.FileExists(mediaEntity.Path))
            {
                _storageProvider.DeleteFile(mediaEntity.Path);
            }
            _storageProvider.SaveStream(mediaEntity.Path, new MemoryStream(media.Data));
            _dataContext.Media.Update(mediaEntity);
            _dataContext.SaveChanges();
        }

        public void DeleteMedia(int id)
        {
            _dataContext.Media.Remove(null, id);
            _dataContext.SaveChanges();
        }

        public Media GetMedia(int id)
        {
            var mediaEntity = _dataContext.Media.Find(id);
            var mediaFile = _storageProvider.GetFile(mediaEntity.Path);
            return CreateMedia(mediaFile.OpenRead(), mediaFile.GetName(), mediaEntity.Id);
        }

        public bool IsMediaAcceptable(Media media)
        {
            var acceptableMediaTypes = CollectionHelpers.EmptyIfNull(_settingsService.GetAcceptableMediaTypes());
            return acceptableMediaTypes.Contains("*") || acceptableMediaTypes.Contains(media.Type, StringComparer.OrdinalIgnoreCase);
        }
    }
}
