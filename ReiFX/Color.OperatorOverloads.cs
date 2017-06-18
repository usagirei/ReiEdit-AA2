// --------------------------------------------------
// ReiFX - Color.OperatorOverloads.cs
// --------------------------------------------------

namespace ReiFX
{
    partial struct Color
    {
        /// <summary>
        ///     Adds <see cref="Color" /> B to <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        public static Color operator +(Color a, Color b)
        {
            return Add(a, b);
        }

        /// <summary>
        ///     Adds Scalar B to <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        public static Color operator +(Color a, byte b)
        {
            return Add(a, b);
        }

        /// <summary>
        ///     Alpha Blends Colors A with <see cref="Color" /> B
        /// </summary>
        public static Color operator &(Color a, Color b)
        {
            return Combine(a, b);
        }

        /// <summary>
        ///     Divides <see cref="Color" /> A with Scalar B, keeping A Alpha Value
        /// </summary>
        public static Color operator /(Color a, float b)
        {
            return Divide(a, b);
        }

        /// <summary>
        ///     Checks Equality Between <see cref="Color" />s A and B
        /// </summary>
        public static bool operator ==(Color a, Color b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///     Checks Inequality Between <see cref="Color" />s A and B
        /// </summary>
        public static bool operator !=(Color a, Color b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        ///     Multiplies <see cref="Color" /> A with <see cref="Color" /> B, keeping A Alpha Value
        /// </summary>
        public static Color operator *(Color a, Color b)
        {
            return Multiply(a, b);
        }

        /// <summary>
        ///     Multiplies <see cref="Color" /> A with Scalar B, keeping A Alpha Value
        /// </summary>
        public static Color operator *(Color a, float b)
        {
            return Multiply(a, b);
        }

        /// <summary>
        ///     Subtracts <see cref="Color" /> B from <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        public static Color operator -(Color a, Color b)
        {
            return Subtract(a, b);
        }

        /// <summary>
        ///     Subtracts Scalar B from <see cref="Color" /> A, keeping A Alpha value
        /// </summary>
        public static Color operator -(Color a, byte b)
        {
            return Subtract(a, b);
        }
    }
}