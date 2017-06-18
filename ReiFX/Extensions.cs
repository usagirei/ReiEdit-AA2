// --------------------------------------------------
// ReiFX - Extensions.cs
// --------------------------------------------------

using System;

namespace ReiFX
{
    internal static class Extensions
    {
        internal static T Clamp<T>(this T b, T min, T max)
            where T : IComparable
        {
            return b.CompareTo(min) < 0
                ? min
                : b.CompareTo(max) > 0
                    ? max
                    : b;
        }
    }
}