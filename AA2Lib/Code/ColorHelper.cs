// --------------------------------------------------
// AA2Lib - ColorHelper.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ReiFX;

namespace AA2Lib.Code
{
    public static class ColorHelper
    {
        private const string FORMAT34 = @"(?<Format>\w{1,})\((?<Data>(?:(?:[\d.]*?,)){2,3}([\d.]*?))\)";
        private const string HEX6 = @"(?<Format>#)(?<Data>[A-Fa-f0-9]{6})";
        private const string HEX8 = @"(?<Format>#)(?<Data>[A-Fa-f0-9]{8})";

        private static readonly Regex ColorRegex = new Regex(string.Format("{0}|{1}|{2}", HEX8, HEX6, FORMAT34), RegexOptions.Compiled);

        public static bool CanParseColor(string colorString)
        {
            string[] validDataTypes = {"rgb", "rgba", "hsl", "hsla", "hsv", "hsva", "#"};
            bool ismatch = ColorRegex.IsMatch(colorString);
            if (!ismatch)
                return false;

            Match m = ColorRegex.Match(colorString);
            string dataType = m.Groups["Format"].Value;
            return validDataTypes.Contains(dataType);
        }

        public static Color ColorFromHSL(double hue, double sat, double bright, double alpha = 1.0)
        {
            //bright = Math.Min(1,bright);
            hue %= 360.0f;
            sat = sat.Clamp(0, 1);
            bright = bright.Clamp(0, 1);
            double chroma = (1 - Math.Abs((2 * bright) - 1)) * sat;
            double hueIndex = hue / 60.0 % 6.0;
            double factor = chroma * (1 - Math.Abs((hueIndex % 2) - 1));
            double red = 0, green = 0, blue = 0;
            switch ((int) Math.Floor(hueIndex))
            {
                case 0:
                    red = chroma;
                    green = factor;
                    break;
                case 1:
                    red = factor;
                    green = chroma;
                    break;
                case 2:
                    green = chroma;
                    blue = factor;
                    break;
                case 3:
                    green = factor;
                    blue = chroma;
                    break;
                case 4:
                    red = factor;
                    blue = chroma;
                    break;
                case 5:
                    red = chroma;
                    blue = factor;
                    break;
            }
            double m = bright - (chroma / 2.0);
            byte alphaByte = Convert.ToByte(alpha * 255);
            byte redByte = Convert.ToByte((red + m) * 255);
            byte greenByte = Convert.ToByte((green + m) * 255);
            byte blueByte = Convert.ToByte((blue + m) * 255);
            return Color.FromRgb(redByte, greenByte, blueByte, alphaByte);
        }

        public static Color ColorFromHSV(double hue, double sat, double value, double alpha = 1.0)
        {
            double chroma = value * sat;
            double hueIndex = hue / 60.0 % 6.0;
            double factor = chroma * (1 - Math.Abs((hueIndex % 2) - 1));
            double red = 0, green = 0, blue = 0;
            switch ((int) Math.Floor(hueIndex))
            {
                case 0:
                    red = chroma;
                    green = factor;
                    break;
                case 1:
                    red = factor;
                    green = chroma;
                    break;
                case 2:
                    green = chroma;
                    blue = factor;
                    break;
                case 3:
                    green = factor;
                    blue = chroma;
                    break;
                case 4:
                    red = factor;
                    blue = chroma;
                    break;
                case 5:
                    red = chroma;
                    blue = factor;
                    break;
            }
            double m = value - chroma;
            byte alphaByte = Convert.ToByte(alpha * 255);
            byte redByte = Convert.ToByte((red + m) * 255);
            byte greenByte = Convert.ToByte((green + m) * 255);
            byte blueByte = Convert.ToByte((blue + m) * 255);
            return Color.FromRgb(redByte, greenByte, blueByte, alphaByte);
        }

        public static void ColorToHSL(Color col, out double h, out double s, out double l)
        {
            double max, min;
            double R = col.R / 255.0;
            double G = col.G / 255.0;
            double B = col.B / 255.0;

            max = Math.Max(R, Math.Max(G, B));

            min = Math.Min(R, Math.Min(G, B));

            l = (max + min) / 2.0f;

            if (max == min)
            {
                h = 0;
                s = 0;
            }
            else
            {
                double delta = (max - min);
                if (max == R)
                {
                    if (G >= B)
                    {
                        h = 60 * (G - B) / delta;
                    }
                    else
                    {
                        h = 60 * (G - B) / delta + 360;
                    }
                }
                else if (max == G)
                {
                    h = 60 * (B - R) / delta + 120;
                }
                else
                {
                    h = 60 * (R - G) / delta + 240;
                }

                s = delta / (1 - Math.Abs(2 * l - 1)); // Weird Colors at l < 0.5 
                //s = delta / (max + min);
                //s = delta / max;
            }
        }

        public static void ColorToHSV(Color col, out double h, out double s, out double v)
        {
            double max, min;
            double R = col.R / 255.0;
            double G = col.G / 255.0;
            double B = col.B / 255.0;

            max = Math.Max(R, Math.Max(G, B));
            min = Math.Min(R, Math.Min(G, B));

            v = max;

            if (max == min)
            {
                h = 0;
                s = 0;
            }
            else
            {
                if (max == R)
                {
                    if (G >= B)
                    {
                        h = 60 * (G - B) / (max - min);
                    }
                    else
                    {
                        h = 60 * (G - B) / (max - min) + 360;
                    }
                }
                else if (max == G)
                {
                    h = 60 * (B - R) / (max - min) + 120;
                }
                else
                {
                    h = 60 * (R - G) / (max - min) + 240;
                }
                s = (max - min) / max;
            }
        }

        public static bool TryParseColor(string colorString, out Color color)
        {
            try
            {
                Color errorColor = Color.FromRgb(0, 0, 0);

                if (!CanParseColor(colorString))
                {
                    color = errorColor;
                    return false;
                }
                Match match = ColorRegex.Match(colorString);
                string format = match.Groups["Format"].Value;
                string data = match.Groups["Data"].Value;
                byte[] dataBytes;
                double[] doubleValues;
                switch (format.ToLower())
                {
                    case "#":
                    {
                        dataBytes = Enumerable.Range(0, data.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(data.Substring(x, 2), 16))
                            .ToArray();
                        if (dataBytes.Length == 4)
                            errorColor = Color.FromRgb(dataBytes[1], dataBytes[2], dataBytes[3], dataBytes[0]);
                        if (dataBytes.Length == 3)
                            errorColor = Color.FromRgb(dataBytes[0], dataBytes[1], dataBytes[2]);
                        break;
                    }

                    case "rgb":
                    case "rgba":
                    {
                        dataBytes = data.Split(',')
                            .Select(s => Convert.ToByte(s))
                            .ToArray();
                        if (dataBytes.Length == 3)
                            errorColor = Color.FromRgb(dataBytes[0], dataBytes[1], dataBytes[2]);
                        if (dataBytes.Length == 4)
                            errorColor = Color.FromRgb(dataBytes[0], dataBytes[1], dataBytes[2], dataBytes[3]);
                        break;
                    }

                    case "hsv":
                    case "hsva":
                    {
                        doubleValues = data.Split(',')
                            .Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture))
                            .ToArray();
                        if (doubleValues.Length == 3)
                            errorColor = ColorFromHSV(doubleValues[0], doubleValues[1], doubleValues[2]);
                        if (doubleValues.Length == 4)
                            errorColor = ColorFromHSV(doubleValues[0], doubleValues[1], doubleValues[2], doubleValues[3]);
                        break;
                    }

                    case "hsl":
                    case "hsla":
                    {
                        doubleValues = data.Split(',')
                            .Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture))
                            .ToArray();
                        if (doubleValues.Length == 3)
                            errorColor = ColorFromHSL(doubleValues[0], doubleValues[1], doubleValues[2]);
                        if (doubleValues.Length == 4)
                            errorColor = ColorFromHSL(doubleValues[0], doubleValues[1], doubleValues[2], doubleValues[3]);
                        break;
                    }
                }
                color = errorColor;
                return true;
            }
            catch
            {
                color = Color.FromRgb(0, 0, 0);
                return false;
            }
        }
    }
}