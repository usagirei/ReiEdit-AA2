// --------------------------------------------------
// ReiFX - Color.InstanceOperations.cs
// --------------------------------------------------

namespace ReiFX
{
    partial struct Color
    {
        /// <summary>
        ///     Adds Color B to this <see cref="Color" />, keeping Alpha value
        /// </summary>
        /// <param name="b">Color B</param>
        /// <returns></returns>
        public Color Add(Color b)
        {
            return Add(this, b);
        }

        /// <summary>
        ///     Adds Scalar B to this <see cref="Color" />, keeping Alpha value
        /// </summary>
        /// <param name="b">Scalar B</param>
        /// <returns></returns>
        public Color Add(byte b)
        {
            return Add(this, b);
        }

        /// <summary>
        ///     Alpha Blends this <see cref="Color" /> with Color B
        /// </summary>
        /// <param name="b">Color B</param>
        /// <returns></returns>
        public Color Combine(Color b)
        {
            return Combine(this, b);
        }

        /// <summary>
        ///     Divides this <see cref="Color" /> with Scalar B, keeping Alpha value
        /// </summary>
        /// <param name="b">Scalar B</param>
        /// <returns></returns>
        public Color Divide(byte b)
        {
            return Multiply(this, b);
        }

        /// <summary>
        ///     Multiplies Color B with this <see cref="Color" />, keeping Alpha value
        /// </summary>
        /// <param name="b">Color B</param>
        /// <returns></returns>
        public Color Multiply(Color b)
        {
            return Multiply(this, b);
        }

        /// <summary>
        ///     Multiplies Scalar B with this <see cref="Color" />, keeping Alpha value
        /// </summary>
        /// <param name="b">Scalar B</param>
        /// <returns></returns>
        public Color Multiply(byte b)
        {
            return Multiply(this, b);
        }

        /// <summary>
        ///     Subtracts Color B from this <see cref="Color" />, keeping Alpha value
        /// </summary>
        /// <param name="b">Color B</param>
        /// <returns></returns>
        public Color Subtract(Color b)
        {
            return Subtract(this, b);
        }

        /// <summary>
        ///     Subtracts Scalar B from this <see cref="Color" />, keeping Alpha value
        /// </summary>
        /// <param name="b">Scalar B</param>
        /// <returns></returns>
        public Color Subtract(byte b)
        {
            return Subtract(this, b);
        }

        /// <summary>
        ///     Gets the HSL Compontents of this <see cref="Color" /> instance
        /// </summary>
        /// <param name="col">Color</param>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="l">Lightness</param>
        public void ToHsl(out float h, out float s, out float l)
        {
            ToHsl(this, out h, out s, out l);
        }

        /// <summary>
        ///     Gets the HSV Components of this <see cref="Color" /> instance
        /// </summary>
        /// <param name="col">Color</param>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        public void ToHsv(out float h, out float s, out float v)
        {
            ToHsv(this, out h, out s, out v);
        }
    }
}