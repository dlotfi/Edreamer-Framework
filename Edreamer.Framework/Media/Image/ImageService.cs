using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Edreamer.Framework.Helpers;
using ImageResizer;
using ImageResizer.Resizing;

namespace Edreamer.Framework.Media.Image
{
    public class ImageService : IImageService
    {
        private readonly IMediaService _mediaService;

        public ImageService(IMediaService mediaService)
        {
            Throw.IfArgumentNull(mediaService, "mediaService");
            _mediaService = mediaService;
        }

        public Stream ManipulateImage(Stream data, bool disposeSource, params ImageManipulation[] manipulations)
        {
            Throw.IfArgumentNull(data, "data");
            Stream src = new MemoryStream();
            data.CopyTo(src);
            src.Seek(0, SeekOrigin.Begin);
            if (disposeSource)
            {
                data.Dispose();
            }
            if (CollectionHelpers.IsNullOrEmpty(manipulations))
            {
                return src;
            }

            Stream dest = null;
            foreach (var m in manipulations)
            {
                dest = new MemoryStream();
                ImageBuilder.Current.Build(src, dest,
                    new ResizeSettings
                        {
                            Width = (m.Width == 0) ? -1 : m.Width,
                            Height = (m.Height == 0) ? -1 : m.Height,
                            Quality = GetQualityValue(m.Quality),
                            Mode = MapFitMode(m.FitMode),
                            Rotate = m.Rotate,
                            Anchor = m.Anchor,
                            Flip = MapFlip(m.Flip),
                            Scale = MapScale(m.Scale),
                            CropTopLeft = new PointF(m.CropRectangle.Left, m.CropRectangle.Top),
                            CropBottomRight = new PointF(m.CropRectangle.Right, m.CropRectangle.Bottom),
                            BackgroundColor = m.BackgroundColor,
                            PaddingColor = m.PaddingColor,
                            Padding = MapSides(m.Padding),
                            Margin = MapSides(m.Margin),
                            BorderColor = m.BackgroundColor,
                            Border = MapSides(m.Border),
                            Format = m.Format == null ? null : m.Format.ToString()
                        }, true);
                src = dest;
            }
            return dest;
        }

        public Media ManipulateImage(Media media, params ImageManipulation[] manipulations)
        {
            Throw.IfArgumentNull(media, "media");
            Throw.IfNull(media.Data).AnArgumentException("Data cannot be null.", "media");

            var manipulatedData = ManipulateImage(new MemoryStream(media.Data), false, manipulations);
            var manipulatedMedia = _mediaService.CreateMedia(manipulatedData, null, media.Id, false);
            return manipulatedMedia;
        }

        #region Mapping Methods

        private static int GetQualityValue(ImageQuality quality)
        {
            switch (quality)
            {
                case ImageQuality.Lowest: return 10;
                case ImageQuality.Low: return 20;
                case ImageQuality.Medium: return 55;
                case ImageQuality.High: return 90;
                case ImageQuality.Highest: return 100;
                default:
                    throw new ArgumentOutOfRangeException("quality");
            }
        }

        private static ImageResizer.FitMode MapFitMode(FitMode fitMode)
        {
            switch (fitMode)
            {
                case FitMode.Pad: return ImageResizer.FitMode.Pad;
                case FitMode.Crop: return ImageResizer.FitMode.Crop;
                case FitMode.Stretch: return ImageResizer.FitMode.Stretch;
                case FitMode.Max: return ImageResizer.FitMode.Max;
                default:
                    throw new ArgumentOutOfRangeException("fitMode");
            }
        }

        private static RotateFlipType MapFlip(FlipType flipType)
        {
            if (flipType.HasFlag(FlipType.FlipX) && flipType.HasFlag(FlipType.FlipY))
                return RotateFlipType.RotateNoneFlipXY;
            if (flipType.HasFlag(FlipType.FlipX))
                return RotateFlipType.RotateNoneFlipX;
            if (flipType.HasFlag(FlipType.FlipY))
                return RotateFlipType.RotateNoneFlipY;
            return RotateFlipType.RotateNoneFlipNone;
        }

        private static ImageResizer.ScaleMode MapScale(ScaleMode scaleMode)
        {
            switch (scaleMode)
            {
                case ScaleMode.DownscaleOnly: return ImageResizer.ScaleMode.DownscaleOnly;
                case ScaleMode.UpscaleOnly: return ImageResizer.ScaleMode.UpscaleOnly;
                case ScaleMode.Both: return ImageResizer.ScaleMode.Both;
                case ScaleMode.UpscaleCanvas: return ImageResizer.ScaleMode.UpscaleCanvas;
                default:
                    throw new ArgumentOutOfRangeException("scaleMode");
            }
        }

        private static BoxPadding MapSides(Sides sides)
        {
            return new BoxPadding(sides.Left, sides.Top, sides.Right, sides.Bottom);
        }

        #endregion
    }
}
