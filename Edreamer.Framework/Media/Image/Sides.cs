namespace Edreamer.Framework.Media.Image
{
    public struct Sides
    {

        public Sides(double all) : this()
        {
            All = all;
        }

        public Sides(double top, double bottom, double left, double right): this()
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }

        public double All
        {
            get
            {
                if (Top == Left && Left == Bottom && Bottom == Right) return Top;
                return double.NaN;
            }
            set { Top = Left = Bottom = Right = value; }
        }
    }
}
