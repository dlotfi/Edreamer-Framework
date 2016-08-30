using System.Drawing;

namespace Edreamer.Framework.Media.Image
{
    public class ImageManipulation
    {
        public ImageManipulation()
        {
            Quality = ImageQuality.High;
            FitMode = FitMode.Pad;
            Anchor = ContentAlignment.MiddleCenter;
            Flip = FlipType.None;
            Scale = ScaleMode.DownscaleOnly;
            CropRectangle = new RectangleF(0, 0, 0, 0);
            BackgroundColor = Color.White;
            PaddingColor = Color.White;
            Padding = new Sides(0);
            Margin = new Sides(0);
            BorderColor = Color.Black;
            Border = new Sides(0);
        }

        /// <summary>
        /// Sets the desired width of the image. (minus padding, borders, margins, and rotation). 
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Sets the desired height of the image. (minus padding, borders, margins, and rotation). 
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The jpeg encoding quality to use. The default and best value is High, you should normally leave it.
        /// </summary>
        public ImageQuality Quality { get; set; }

        /// <summary>
        /// Sets the fit mode for the image. The default value is Pad.
        /// </summary>
        public FitMode FitMode { get; set; }

        /// <summary>
        /// The degress to rotate the image clockwise. -360 to 360.
        /// </summary>
        public double Rotate { get; set; }

        /// <summary>
        /// How to anchor the image when cropping or adding whitespace to meet sizing requirements. The default value is MiddleCenter.
        /// </summary>
        public ContentAlignment Anchor { get; set; }

        /// <summary>
        /// Allows you to flip the entire resulting image vertically, horizontally, or both. The default value is None.
        /// </summary>
        public FlipType Flip { get; set; }

        /// <summary>
        /// Whether to downscale, upscale, upscale the canvas, or both upscale or downscale the image as needed.
        /// The default value is DownscaleOnly.
        /// </summary>
        public ScaleMode Scale { get; set; }

        /// <summary>
        /// The crop rectangle.
        /// </summary>
        public RectangleF CropRectangle { get; set; }

        /// <summary>
        /// The background color. The default is Color.White.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// The padding color. The default is Color.White
        /// </summary>
        public Color PaddingColor { get; set; }

        /// <summary>
        /// The width(s) of padding inside the image border.
        /// </summary>
        public Sides Padding { get; set; }

        /// <summary>
        /// The width(s) of the margin outside the image border.
        /// </summary>
        public Sides Margin { get; set; }

        /// <summary>
        /// The padding color. The default is Color.Black
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// The width(s) of the image border.
        /// </summary>
        public Sides Border { get; set; }

        /// <summary>
        /// The resulting image format to use. The default value is Jpg.
        /// When null value is set, the original format of the image is used (unless it is not a web safe format  - jpeg is the fallback in that scenario).
        /// </summary>
        public ImageFormat? Format { get; set; }
    }

}
