// --------------------------------------------------
// ReiFX - Color.Conversion.cs
// --------------------------------------------------

using System;

namespace ReiFX
{
    partial struct Color
    {
        private const float FLOAT_TOLERANCE = 0.001f;

        /// <summary>
        ///     Creates a <see cref="Color" /> instance from the RGB Color Space
        /// </summary>
        /// <param name="r">Red [0..255]</param>
        /// <param name="g">Green [0..255]</param>
        /// <param name="b">Blue [0..255]</param>
        /// <param name="a">Alpha [0..255]</param>
        /// <returns></returns>
        public static Color FromRgb(byte r, byte g, byte b, byte a = 255)
        {
            return new Color(r, g, b, a);
        }

        /// <summary>
        ///     Creates a <see cref="Color" /> instance from the HSL Color Space
        /// </summary>
        /// <param name="hue">Hue [0..360]</param>
        /// <param name="sat">Saturation [0..1]</param>
        /// <param name="light">Lightness [0..1]</param>
        /// <param name="alpha">Alpha [0..1]</param>
        /// <returns></returns>
        public static Color FromHsl(double hue, double sat, double light, double alpha = 1.0)
        {
            //light = Math.Min(1,light);
            if (hue < 0)
                while (hue < 0)
                    hue += 360;
            if (hue >= 360)
                while (hue >= 360)
                    hue -= 360;
            sat = sat > 1
                ? 1
                : sat < 0
                    ? 0
                    : sat;
            light = light > 1
                ? 1
                : light < 0
                    ? 0
                    : light;
            double chroma = (1 - Math.Abs((2 * light) - 1)) * sat;
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
            double m = light - (chroma / 2.0);
            return new Color((byte) ((red + m) * 255), (byte) ((green + m) * 255), (byte) ((blue + m) * 255), (byte) (alpha * 255));
        }

        /// <summary>
        ///     Creates a <see cref="Color" /> instance from the HSV Color Space
        /// </summary>
        /// <param name="hue">Hue [0..360]</param>
        /// <param name="sat">Saturation [0..1]</param>
        /// <param name="value">Value [0..1]</param>
        /// <param name="alpha">Alpha [0..1]</param>
        /// <returns></returns>
        public static Color FromHsv(double hue, double sat, double value, double alpha = 1.0)
        {
            if (hue < 0)
                while (hue < 0)
                    hue += 360;
            if (hue >= 360)
                while (hue >= 360)
                    hue -= 360;

            sat = sat > 1
                ? 1
                : sat < 0
                    ? 0
                    : sat;

            value = value > 1
                ? 1
                : value < 0
                    ? 0
                    : value;

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
            return new Color((byte) ((red + m) * 255), (byte) ((green + m) * 255), (byte) ((blue + m) * 255), (byte) (alpha * 255));
        }

        /// <summary>
        ///     Gets the HSL Compontents of the <see cref="Color" /> instance
        /// </summary>
        /// <param name="col">Color</param>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="l">Lightness</param>
        public static void ToHsl(Color col, out float h, out float s, out float l)
        {
            float max, min;
            float R = col.R / 255f;
            float G = col.G / 255f;
            float B = col.B / 255f;

            max = Math.Max(R, Math.Max(G, B));

            min = Math.Min(R, Math.Min(G, B));

            l = (max + min) / 2.0f;

            if (Math.Abs(max - min) < FLOAT_TOLERANCE)
            {
                h = 0;
                s = 0;
            }
            else
            {
                float delta = (max - min);
                if (Math.Abs(max - R) < FLOAT_TOLERANCE)
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
                else if (Math.Abs(max - G) < FLOAT_TOLERANCE)
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

        /// <summary>
        ///     Gets the HSV Components of the <see cref="Color" /> instance
        /// </summary>
        /// <param name="col">Color</param>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        public static void ToHsv(Color col, out float h, out float s, out float v)
        {
            float max, min;
            float R = col.R / 255f;
            float G = col.G / 255f;
            float B = col.B / 255f;

            max = Math.Max(R, Math.Max(G, B));
            min = Math.Min(R, Math.Min(G, B));

            v = max;

            if (Math.Abs(max - min) < FLOAT_TOLERANCE)
            {
                h = 0;
                s = 0;
            }
            else
            {
                if (Math.Abs(max - R) < FLOAT_TOLERANCE)
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
                else if (Math.Abs(max - G) < FLOAT_TOLERANCE)
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

        /// <summary>
        ///     Implicit Conversion to <see cref="System.Drawing.Color" />
        /// </summary>
        /// <param name="c">Color</param>
        /// <returns></returns>
        public static implicit operator System.Drawing.Color(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        ///     Implicit Conversion from <see cref="System.Drawing.Color" />
        /// </summary>
        /// <param name="c">Color</param>
        /// <returns></returns>
        public static implicit operator Color(System.Drawing.Color c)
        {
            return new Color(c.R, c.G, c.B, c.A);
        }

        /// <summary>
        ///     Implicit Conversion to <see cref="System.Windows.Media.Color" /> (WPF)
        /// </summary>
        /// <param name="c">Color</param>
        /// <returns></returns>
        public static implicit operator System.Windows.Media.Color(Color c)
        {
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        ///     Implicit Conversion from <see cref="System.Windows.Media.Color" /> (WPF)
        /// </summary>
        /// <param name="c">Color</param>
        /// <returns></returns>
        public static implicit operator Color(System.Windows.Media.Color c)
        {
            return new Color(c.R, c.G, c.B, c.A);
        }
    }
}