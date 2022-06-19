using System;

namespace HueManatee.Extensions
{
    internal static class DoubleExtensions
    {
        internal static bool ApproximatelyEquals(this double a, double b) => 
            Math.Abs(a - b) <= float.Epsilon;
    }
}
