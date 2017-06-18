// --------------------------------------------------
// ReiFX - Color.cs
// --------------------------------------------------

using System;

namespace ReiFX
{
    /// <summary>
    ///     ReiFX Color Structure
    ///     <para />
    ///     Contains Static, Instance Methods and Operators for Common Color Operations:
    ///     Add (+), Subtract (-), Multiply (*), Divide (/) and AlphaBlend (&amp;).
    ///     <para />
    ///     Has Built-in Implicit conversion from and to
    ///     <see cref="System.Drawing.Color" /> and <see cref="System.Windows.Media.Color" />.
    ///     <para />
    ///     Has Methods for Conversion from and to HSV and HSL Color Spaces.
    /// </summary>
    [Serializable]
    public partial struct Color
    {
        /// <summary>
        ///     Alpha Component
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        ///     Blue Component
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        ///     Green Component
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        ///     Red Component
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        ///     Creates a <see cref="Color" /> instance from the RGB Color Space
        /// </summary>
        /// <param name="r">Red [0..255]</param>
        /// <param name="g">Green [0..255]</param>
        /// <param name="b">Blue [0..255]</param>
        /// <param name="a">Alpha [0..255]</param>
        public Color(byte r, byte g, byte b, byte a = 255) : this()
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        ///     Creates a <see cref="Color" /> with a new Alpha Value
        /// </summary>
        /// <param name="c">Base Color</param>
        /// <param name="a">Alpha [0..255]</param>
        public Color(Color c, byte a) : this()
        {
            R = c.R;
            G = c.G;
            B = c.B;
            A = a;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Color && Equals((Color) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = A.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ R.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", A, R, G, B);
        }

        public bool Equals(Color other)
        {
            return A == other.A && B == other.B && G == other.G && R == other.R;
        }
    }
}