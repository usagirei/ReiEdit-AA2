// --------------------------------------------------
// AA2Lib - Extensions.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace AA2Lib
{
    public static class Extensions
    {
        public static int Clamp(this int value, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(minValue, value), maxValue);
        }

        public static short Clamp(this short value, short minValue, short maxValue)
        {
            return Math.Min(Math.Max(minValue, value), maxValue);
        }

        public static byte Clamp(this byte value, byte minValue, byte maxValue)
        {
            return Math.Min(Math.Max(minValue, value), maxValue);
        }

        public static float Clamp(this float value, float minValue, float maxValue)
        {
            return Math.Min(Math.Max(minValue, value), maxValue);
        }

        public static double Clamp(this double value, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(minValue, value), maxValue);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable", "Enumerable can't be null");
            if (action == null)
                throw new ArgumentNullException("action", "Action can't be null");
            foreach (T element in enumerable)
                action(element);
        }

        public static byte ToByte(this string input)
        {
            byte i;

            bool parsed = input.StartsWith("0x")
                ? Byte.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i)
                : Byte.TryParse(input, out i);

            return parsed
                ? i
                : Byte.MaxValue;
        }

        public static short ToInt16(this string input)
        {
            short i;

            bool parsed = input.StartsWith("0x")
                ? Int16.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i)
                : Int16.TryParse(input, out i);

            return parsed
                ? i
                : Int16.MaxValue;
        }

        public static int ToInt32(this string input)
        {
            int i;

            bool parsed = input.StartsWith("0x")
                ? Int32.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i)
                : Int32.TryParse(input, out i);

            return parsed
                ? i
                : Int32.MaxValue;
        }
    }
}