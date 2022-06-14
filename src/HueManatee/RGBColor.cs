using System.Drawing;
using System.Linq;

namespace HueManatee
{
    internal struct RGB
    {
        private readonly double _minRgb;
        private readonly double _maxRgb;

        internal double Red { get; }
        internal double Green { get; }
        internal double Blue { get; }

        internal RGB(Color color)
        {
            Red = color.R / 255.0;
            Green = color.G / 255.0;
            Blue = color.B / 255.0;
            
            var rgb = new double[] { Red, Green, Blue };
            _minRgb = rgb.Min();
            _maxRgb = rgb.Max();
        }

        internal int GetHue()
        {
            if (Red == Green && Green == Blue)
                return 0;

            var delta = _maxRgb - _minRgb;

            double hue;

            if (Red.ApproximatelyEquals(_maxRgb))
                hue = (Green - Blue) / delta * 60;

            else if (Green.ApproximatelyEquals(_maxRgb))
                hue = (2 + ((Blue - Red) / delta)) * 60;

            else
                hue = (4 + ((Red - Green) / delta)) * 60;

            return (int)((hue < 0 ? hue + 360 : hue) * 182.04f);
        }

        internal int GetSaturation()
        {
            if (_maxRgb.ApproximatelyEquals(_minRgb))
                return 0;

            return (int)(_maxRgb.ApproximatelyEquals(0f) ? 0f : 1f - (1f * _minRgb / _maxRgb)) * 255;
        }

        internal int GetBrightness()
        {
            return (int)new double[] { Red, Green, Blue }.Max() * 255;
        }
    }
}