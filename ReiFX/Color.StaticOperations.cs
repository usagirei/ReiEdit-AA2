// --------------------------------------------------
// ReiFX - Color.StaticOperations.cs
// --------------------------------------------------

namespace ReiFX
{
    partial struct Color
    {
        /// <summary>
        ///     Adds <see cref="Color" /> B to <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Second Color</param>
        /// <returns>Color A + Color B</returns>
        public static Color Add(Color a, Color b)
        {
            return new Color
            {
                A = a.A,
                R = (byte) (a.R + b.R).Clamp(0, 255),
                G = (byte) (a.G + b.G).Clamp(0, 255),
                B = (byte) (a.B + b.B).Clamp(0, 255),
            };
        }

        /// <summary>
        ///     Adds Scalar B to <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Scalar</param>
        /// <returns>ColorA + B</returns>
        public static Color Add(Color a, byte b)
        {
            return new Color
            {
                A = a.A,
                R = (byte) (a.R + b).Clamp(0, 255),
                G = (byte) (a.G + b).Clamp(0, 255),
                B = (byte) (a.B + b).Clamp(0, 255),
            };
        }

        /// <summary>
        ///     Alpha Blends two <see cref="Color" />s
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Second Color</param>
        /// <returns></returns>
        public static Color Combine(Color a, Color b)
        {
            float alphaFg = b.A / 255.0f;
            float alphaBg = a.A / 255.0f;

            float preRfg = b.R * alphaFg;
            float preGfg = b.G * alphaFg;
            float preBfg = b.B * alphaFg;

            float preRbg = a.R * alphaBg;
            float preGbg = a.G * alphaBg;
            float preBbg = a.B * alphaBg;

            float newAlpha = (alphaFg + alphaBg * (1 - alphaFg));

            byte newR = (byte) ((preRfg + preRbg * (1 - alphaFg)) / newAlpha).Clamp(0, 255);
            byte newG = (byte) ((preGfg + preGbg * (1 - alphaFg)) / newAlpha).Clamp(0, 255);
            byte newB = (byte) ((preBfg + preBbg * (1 - alphaFg)) / newAlpha).Clamp(0, 255);
            byte newA = (byte) (newAlpha * 255).Clamp(0, 255);

            return new Color(newR, newG, newB, newA);
        }

        /// <summary>
        ///     Divides <see cref="Color" /> A with Scalar B, keeping A Alpha Value
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Scalar</param>
        /// <returns>ColorA / B</returns>
        public static Color Divide(Color a, float b)
        {
            return new Color
            {
                A = a.A,
                R = (byte) (a.R / b).Clamp(0, 255),
                G = (byte) (a.G / b).Clamp(0, 255),
                B = (byte) (a.B / b).Clamp(0, 255),
            };
        }

        /// <summary>
        ///     Multiplies <see cref="Color" /> A with <see cref="Color" /> B, keeping A Alpha Value
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Second Color</param>
        /// <returns>ColorA * ColorB</returns>
        public static Color Multiply(Color a, Color b)
        {
            return new Color
            {
                A = a.A,
                R = (byte) ((a.R * b.R) / 255).Clamp(0, 255),
                G = (byte) ((a.G * b.G) / 255).Clamp(0, 255),
                B = (byte) ((a.B * b.B) / 255).Clamp(0, 255),
            };
        }

        /// <summary>
        ///     Multiplies <see cref="Color" /> A with Scalar B, keeping A Alpha Value
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Scalar</param>
        /// <returns>ColorA * B</returns>
        public static Color Multiply(Color a, float b)
        {
            return new Color
            {
                A = a.A,
                R = (byte) (a.R * b).Clamp(0, 255),
                G = (byte) (a.G * b).Clamp(0, 255),
                B = (byte) (a.B * b).Clamp(0, 255),
            };
        }

        /// <summary>
        ///     Subtracts <see cref="Color" /> B from <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Second Color</param>
        /// <returns>Color A - Color B</returns>
        public static Color Subtract(Color a, Color b)
        {
            return new Color
            {
                A = a.A,
                R = (byte) (a.R - b.R).Clamp(0, 255),
                G = (byte) (a.G - b.G).Clamp(0, 255),
                B = (byte) (a.B - b.B).Clamp(0, 255),
            };
        }

        /// <summary>
        ///     Subtracts Scalar B from <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        /// <param name="a">Base Color</param>
        /// <param name="b">Scalar</param>
        /// <returns>ColorA - B</returns>
        public static Color Subtract(Color a, byte b)
        {
            return new Color
            {
                A = a.A,
                R = (byte) (a.R - b).Clamp(0, 255),
                G = (byte) (a.G - b).Clamp(0, 255),
                B = (byte) (a.B - b).Clamp(0, 255),
            };
        }
    }
}