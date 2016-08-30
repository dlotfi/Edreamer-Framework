using System;

namespace Edreamer.Framework.Media.Image
{
    /// <summary>
    /// How do deal with aspect ratio differences between the requested size and the original image's size.
    /// </summary>
    public enum FitMode
    {
        /// <summary>
        /// Padding is used if there is an aspect ratio difference. Use &anchor to override the MiddleCenter default.
        /// </summary>
        Pad,
        /// <summary>
        /// Cropping is used if there is an aspect ratio difference. Use &anchor to override the MiddleCenter default.
        /// </summary>
        Crop,
        /// <summary>
        /// If there is an aspect ratio difference, the image is stretched.
        /// </summary>
        Stretch,
        /// <summary>
        /// Width and height are considered maximum values. The resulting image may be smaller to maintain its aspect ratio.
        /// The image may also be smaller if the source image is smaller and &scale is set to DownscaleOnly.
        /// </summary>
        Max
    }

    /// <summary>
    /// How to flip the image.
    /// </summary>
    [Flags]
    public enum FlipType
    {
        /// <summary>
        /// Specifies no flipping.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies a horizontal flip.
        /// </summary>
        FlipX = 1,
        /// <summary>
        /// Specifies a vertical flip.
        /// </summary>
        FlipY = 2
    }

    /// <summary>
    /// The format of the image.
    /// </summary>
    public enum ImageFormat
    {
        Jpg,
        Png,
        Gif
    }

    /// <summary>
    /// The encoding quality.
    /// </summary>
    public enum ImageQuality
    {
        Lowest,
        Low,
        Medium,
        High,
        Highest
    }

    /// <summary>
    /// Controls whether the image is allowed to upscale, downscale, both, or if only the canvas gets to be upscaled.
    /// </summary>
    public enum ScaleMode
    {
        /// <summary>
        /// Only downsamples images - never enlarges. If an image is smaller than 'width' and 'height', the image coordinates are used instead.
        /// </summary>
        DownscaleOnly,
        /// <summary>
        /// Only upscales (zooms) images. If an image is larger than width and height, the image coordinates are used instead.
        /// </summary>
        UpscaleOnly,
        /// <summary>
        /// Upscales and downscales images according to width and height.
        /// </summary>
        Both,
        /// <summary>
        /// When the image is smaller than the requested size, padding is added instead of stretching the image
        /// </summary>
        UpscaleCanvas
    }
}
