using HueManatee.Extensions;
using System.Drawing;
using System.Linq;

namespace HueManatee
{
    internal struct RgbColor
    {
        private const int HueMaxValue = 65535;

        private readonly double _minRgb;
        private readonly double _maxRgb;
        private readonly double _delta;

        internal double R { get; set; }
        internal double G { get; set; }
        internal double B { get; set; }

        internal RgbColor(Color color)
        {
            R = color.R / 255.0;
            G = color.G / 255.0;
            B = color.B / 255.0;

            _minRgb = new double[] { R, G, B }.Min();
            _maxRgb = new double[] { R, G, B }.Max();
            _delta = _maxRgb - _minRgb;
        }

        internal int GetHue()
        {
            if (R == G && G == B)
            {
                return 0;
            }

            double hue;

            if (R.ApproximatelyEquals(_maxRgb))
            {
                hue = (G - B) / _delta * 60;
            }
            else if (G.ApproximatelyEquals(_maxRgb))
            {
                hue = (2 + ((B - R) / _delta)) * 60;
            }
            else
            {
                hue = (4 + ((R - G) / _delta)) * 60;
            }

            return (int)((((hue < 0 ? hue + 360 : hue) * 182.04f % HueMaxValue) + HueMaxValue) % HueMaxValue);
        }

        internal int GetSaturation()
        {
            if (_maxRgb.ApproximatelyEquals(_minRgb))
            {
                return 0;
            }

            return (int)((_maxRgb.ApproximatelyEquals(0f) ? 0f : 1f - (1f * _minRgb / _maxRgb)) * 255);
        }

        internal int GetBrightness() => 
            (int)(_maxRgb * 255);
    }
}