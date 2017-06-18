// --------------------------------------------------
// ReiFX - Layer.cs
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
    /// <summary>
    ///     ReiFX Layer
    ///     <para />
    ///     Holds the 32bpp ARGB Bitmap Bytes, its Width, Height and Stride, and 3 General Purpose Flags
    ///     (<see cref="Layer.XFlag" />, <see cref="Layer.YFlag" />, <see cref="Layer.ZFlag" />).
    ///     <para />
    ///     Provides Methods to Get and Set the Color at specified Coordinates and
    ///     <para />
    ///     built-in Explicit Conversion from and to <see cref="Bitmap" /> and <see cref="BitmapSource" />
    /// </summary>
    public partial class Layer : ICloneable
    {
        [Flags]
        public enum LayerFlags
        {
            None = 0,
            XFlag = 1,
            YFlag = 2,
            ZFlag = 4,
            Wrap = 8
        }

        private Graphics _graphics;
        private GCHandle _graphicsHandle;

        /// <summary>
        ///     Byte Array Containing the Bitmap Raw Data
        /// </summary>
        public byte[] BitmapData { get; private set; }

        /// <summary>
        ///     Layer Flags
        /// </summary>
        public LayerFlags Flags { get; set; }

        /// <summary>
        ///     Layer Height
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        ///     Layer Raw Data Stride
        /// </summary>
        public int Stride { get; private set; }

        /// <summary>
        ///     Layer Width
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        ///     Wraps Overflow Coordinates instead of Clamping
        /// </summary>
        public bool Wrap
        {
            get { return Flags.HasFlag(LayerFlags.Wrap); }
            set
            {
                if (value)
                    Flags |= LayerFlags.Wrap;
                else
                    Flags &= ~LayerFlags.Wrap;
            }
        }

        /// <summary>
        ///     General Purpose Flag
        /// </summary>
        public bool XFlag
        {
            get { return Flags.HasFlag(LayerFlags.XFlag); }
            set
            {
                if (value)
                    Flags |= LayerFlags.XFlag;
                else
                    Flags &= ~LayerFlags.XFlag;
            }
        }

        /// <summary>
        ///     General Purpose Flag
        /// </summary>
        public bool YFlag
        {
            get { return Flags.HasFlag(LayerFlags.YFlag); }
            set
            {
                if (value)
                    Flags |= LayerFlags.YFlag;
                else
                    Flags &= ~LayerFlags.YFlag;
            }
        }

        /// <summary>
        ///     General Purpose Flag
        /// </summary>
        public bool ZFlag
        {
            get { return Flags.HasFlag(LayerFlags.ZFlag); }
            set
            {
                if (value)
                    Flags |= LayerFlags.ZFlag;
                else
                    Flags &= ~LayerFlags.ZFlag;
            }
        }

        /// <summary>
        ///     Layer Z-Index
        /// </summary>
        public byte ZIndex { get; set; }

        public override string ToString()
        {
            string x = XFlag
                ? "X"
                : null;

            string y = YFlag
                ? "Y"
                : null;

            string z = ZFlag
                ? "Z"
                : null;

            return string.Format("[{3}{4}{5}Layer: Z:{0} W:{1} H:{2}]", ZIndex, Width, Height, x, y, z);
        }

        /// <summary>
        ///     Gets or Creates Graphics Object
        /// </summary>
        public Graphics CreateGraphics()
        {
            if (_graphics == null)
            {
                _graphicsHandle = GCHandle.Alloc(BitmapData, GCHandleType.Pinned);
                IntPtr intPtr = _graphicsHandle.AddrOfPinnedObject();
                const PixelFormatGDI format = PixelFormatGDI.Format32bppArgb;
                Bitmap bmp = new Bitmap(Width, Height, Stride, format, intPtr);
                _graphics = Graphics.FromImage(bmp);
            }
            return _graphics;
        }

        /// <summary>
        ///     Releases Graphics Object
        /// </summary>
        public void FreeGraphics()
        {
            if (_graphics == null)
                return;
            _graphics.Dispose();
            _graphics = null;
            _graphicsHandle.Free();
        }

        /// <summary>
        ///     Calculates the Rectangular Area of the Layer
        /// </summary>
        /// <returns></returns>
        public Rectangle GetArea()
        {
            Rectangle rect = new Rectangle();

            for (int sX = 0; sX < Width; sX++)
            {
                bool found = false;
                for (int sY = 0; sY < Height; sY++)
                {
                    if (IsTransparent(sX, sY))
                        continue;
                    found = true;
                    break;
                }
                if (!found)
                    continue;
                rect.X = sX;
                break;
            }

            for (int sX = Width; sX > rect.Left; sX--)
            {
                bool found = false;
                for (int sY = 0; sY < Height; sY++)
                {
                    if (IsTransparent(sX, sY))
                        continue;
                    found = true;
                    break;
                }
                if (!found)
                    continue;
                rect.Width = Math.Min(Width, sX - rect.Left + 1);
                break;
            }

            for (int sY = 0; sY < Height; sY++)
            {
                bool found = false;
                for (int sX = rect.Left; sX < rect.Right; sX++)
                {
                    if (IsTransparent(sX, sY))
                        continue;
                    found = true;
                    break;
                }
                if (!found)
                    continue;
                rect.Y = sY;
                break;
            }

            for (int sY = Height; sY > rect.Top; sY--)
            {
                bool found = false;
                for (int sX = rect.Left; sX < rect.Right; sX++)
                {
                    if (IsTransparent(sX, sY))
                        continue;
                    found = true;
                    break;
                }
                if (!found)
                    continue;
                rect.Height = Math.Min(Height, sY - rect.Top + 1);
                break;
            }

            return rect;
        }

        /// <summary>
        ///     Gets the <see cref="BitmapData" /> Index for the Coordinates
        /// </summary>
        /// <param name="p">Coordinates</param>
        /// <returns></returns>
        public int GetIndex(Point p)
        {
            return GetIndex(p.X, p.Y);
        }

        /// <summary>
        ///     Gets the <see cref="BitmapData" /> Index for the Coordinates
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <returns></returns>
        public int GetIndex(int x, int y)
        {
            if (x >= Width)
            {
                if (Wrap)
                    do
                        x -= Width;
                    while (x >= Width);
                else
                    x = Width - 1;
            }
            else if (x < 0)
            {
                if (Wrap)
                    do
                        x += Width;
                    while (x < 0);
                else
                    x = 0;
            }

            if (y >= Height)
            {
                if (Wrap)
                    do
                        y -= Height;
                    while (y >= Height);
                else
                    y = Height - 1;
            }
            else if (y < 0)
            {
                if (Wrap)
                    do
                        y += Height;
                    while (y < 0);
                else
                    y = 0;
            }

            return GetIndexUnchecked(x, y);
        }

        /// <summary>
        ///     Gets the <see cref="BitmapData" /> Index for the Coordinates (Without Clamp or Wrap)
        /// </summary>
        /// <param name="p">Coordinates</param>
        /// <returns></returns>
        public int GetIndexUnchecked(Point p)
        {
            return GetIndexUnchecked(p.X, p.Y);
        }

        /// <summary>
        ///     Gets the <see cref="BitmapData" /> Index for the Coordinates (Without Clamp or Wrap)
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <returns></returns>
        public int GetIndexUnchecked(int x, int y)
        {
            return (x * 4 + y * Stride);
        }

        /// <summary>
        ///     Samples the Layer Color at the Coordinate
        /// </summary>
        /// <param name="p">Coordinate</param>
        /// <returns></returns>
        public Color GetPixel(Point p)
        {
            return GetPixel(p.X, p.Y);
        }

        /// <summary>
        ///     Samples the Layer Color at the Coordinate
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            int index = GetIndex(x, y);

            return new Color
            {
                B = BitmapData[index + 0],
                G = BitmapData[index + 1],
                R = BitmapData[index + 2],
                A = BitmapData[index + 3],
            };
        }

        /// <summary>
        ///     Checks if the Given Coordinate is Opaque
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <returns></returns>
        public bool IsOpaque(int x, int y)
        {
            int index = GetIndex(x, y);
            return BitmapData[index + 3] == 255;
        }

        /// <summary>
        ///     Checks if the Given Coordinate is Opaque
        /// </summary>
        /// <param name="p">Coordinates</param>
        /// <returns></returns>
        public bool IsOpaque(Point p)
        {
            return IsOpaque(p.X, p.Y);
        }

        /// <summary>
        ///     Checks if the Given Coordinate is Transparent
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <returns></returns>
        public bool IsTransparent(int x, int y)
        {
            int index = GetIndex(x, y);
            return BitmapData[index + 3] == 0;
        }

        /// <summary>
        ///     Checks if the Given Coordinate is Transparent
        /// </summary>
        /// <param name="p">Coordinates</param>
        /// <returns></returns>
        public bool IsTransparent(Point p)
        {
            return IsTransparent(p.X, p.Y);
        }

        /// <summary>
        ///     Sets the Color at the Specified Coordinates
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <param name="c">Color</param>
        public void SetPixel(int x, int y, Color c)
        {
            int index = GetIndex(x, y);

            BitmapData[index + 0] = c.B;
            BitmapData[index + 1] = c.G;
            BitmapData[index + 2] = c.R;
            BitmapData[index + 3] = c.A;
        }

        /// <summary>
        ///     Sets the Color at the Specified Coordinates
        /// </summary>
        /// <param name="p">Coordinates</param>
        /// <param name="c">Color</param>
        public void SetPixel(Point p, Color c)
        {
            SetPixel(p.X, p.Y, c);
        }

        /// <summary>
        ///     Creates a new <see cref="Layer" /> from a <see cref="Bitmap" />
        /// </summary>
        /// <param name="bmp">Source Bitmap</param>
        /// <returns></returns>
        public static Layer Create(Bitmap bmp)
        {
            Layer l = new Layer();

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormatGDI.Format32bppArgb);

            int dataSize = bmpd.Width + bmpd.Height * bmpd.Stride;

            l.BitmapData = new byte[dataSize];
            l.Width = bmpd.Width;
            l.Height = bmpd.Height;
            l.Stride = bmpd.Stride;

            Marshal.Copy(bmpd.Scan0, l.BitmapData, 0, dataSize);
            bmp.UnlockBits(bmpd);

            return l;
        }

        /// <summary>
        ///     Creates a Blank <see cref="Layer" />
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns></returns>
        public static Layer Create(int width, int height)
        {
            Layer l = new Layer();
            int stride = 4 * width;

            int dataSize = width + height * stride;

            l.BitmapData = new byte[dataSize];
            l.Width = width;
            l.Height = height;
            l.Stride = stride;

            return l;
        }

        /// <summary>
        ///     Creates a new <see cref="Layer" /> from a <see cref="BitmapSource" /> (WPF)
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Layer Create(BitmapSource bmp)
        {
            if (bmp.Format != PixelFormatWPF.Bgra32)
                throw new ArgumentException("BitmapSource Format must be Bgra32");

            Layer l = new Layer();

            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            int stride = 4 * width;
            int dataSize = width + height * stride;

            l.Stride = stride;
            l.Width = width;
            l.Height = height;
            l.BitmapData = new byte[dataSize];

            bmp.CopyPixels(l.BitmapData, stride, 0);

            return l;
        }

        /// <summary>
        ///     Creates a Copy of this <see cref="Layer" />
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Layer newLayer = new Layer
            {
                Width = Width,
                Height = Height,
                Stride = Stride,
                Wrap = Wrap,
                XFlag = XFlag,
                YFlag = YFlag,
                ZFlag = ZFlag,
                ZIndex = ZIndex,
                BitmapData = new byte[BitmapData.Length]
            };
            Buffer.BlockCopy(BitmapData, 0, newLayer.BitmapData, 0, BitmapData.Length);

            return newLayer;
        }
    }
}