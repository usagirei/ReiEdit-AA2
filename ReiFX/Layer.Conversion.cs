// --------------------------------------------------
// ReiFX - Layer.Conversion.cs
// --------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using PixelFormatGDI = System.Drawing.Imaging.PixelFormat;
using PixelFormatWPF = System.Windows.Media.PixelFormats;

namespace ReiFX
{
    partial class Layer
    {
        /// <summary>
        ///     Explicit Conversion from <see cref="Bitmap" />
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <returns></returns>
        public static explicit operator Layer(Bitmap bmp)
        {
            return Create(bmp);
        }

        /// <summary>
        ///     Explicit Conversion to <see cref="Bitmap" />
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <returns></returns>
        public static explicit operator Bitmap(Layer layer)
        {
            Bitmap bmp = new Bitmap(layer.Width, layer.Height, PixelFormatGDI.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpd = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormatGDI.Format32bppArgb);

            int dataSize = bmpd.Width + bmpd.Height * bmpd.Stride;
            if (dataSize != layer.BitmapData.Length)
                throw new InvalidOperationException("Data Size Mismatch");
            Marshal.Copy(layer.BitmapData, 0, bmpd.Scan0, dataSize);
            bmp.UnlockBits(bmpd);

            return bmp;
        }

        /// <summary>
        ///     Explicit Conversion to <see cref="BitmapSource" /> (WPF)
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <returns></returns>
        public static explicit operator BitmapSource(Layer layer)
        {
            int dataSize = layer.BitmapData.Length;
            var bitmapData = new byte[dataSize];
            Buffer.BlockCopy(layer.BitmapData, 0, bitmapData, 0, dataSize);

            int width = layer.Width;
            int height = layer.Height;
            int stride = layer.Stride;

            BitmapSource bmp = BitmapSource.Create(width, height, 96, 96, PixelFormatWPF.Bgra32, null, bitmapData, stride);

            return bmp;
        }

        /// <summary>
        ///     Explicit Conversion from <see cref="BitmapSource" /> (WPF)
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static explicit operator Layer(BitmapSource bmp)
        {
            return Create(bmp);
        }
    }
}