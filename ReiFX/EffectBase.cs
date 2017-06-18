// --------------------------------------------------
// ReiFX - EffectBase.cs
// --------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace ReiFX
{
    /// <summary>
    ///     Delegate for <see cref="EffectBase.OnEffectComplete" />
    /// </summary>
    public delegate void EffectCompleteDelegate(object sender, EffectCompleteEventArgs e);

    /// <summary>
    ///     Base Class for <see cref="Layer" /> Effects
    ///     <para />
    ///     Implement the <see cref="ProcessPixel" /> Method and
    ///     call <see cref="ApplyEffect(ReiFX.Layer)" />
    /// </summary>
    public abstract class EffectBase
    {
        /// <summary>
        ///     Raised when the Effect Pass is Complete
        /// </summary>
        public event EffectCompleteDelegate OnEffectComplete;

        private readonly Stopwatch _sw = new Stopwatch();
        private Rectangle _region = Rectangle.Empty;

        public EffectBase()
        {
            Threads = 0;
        }

        /// <summary>
        ///     Region to apply Effect into.
        /// </summary>
        public Rectangle Region
        {
            get { return _region; }
            set { _region = value; }
        }

        /// <summary>
        ///     Number of Threads to use:
        ///     <para>&#160;</para>
        ///     Positive Values: Use N Threads.
        ///     <para />
        ///     Zero: Automatic (Max).
        ///     <para />
        ///     Negative Values: Use Max-N Threads (1 Minimum)
        /// </summary>
        public int Threads { get; set; }

        /// <summary>
        ///     Applies the Effect on the specified <see cref="Layer" />.
        /// </summary>
        /// <param name="l">Layer</param>
        public void ApplyEffect(Layer l)
        {
            _sw.Restart();

            int dataSize = l.BitmapData.Length;
            var targetBytes = new byte[dataSize];
            Buffer.BlockCopy(l.BitmapData, 0, targetBytes, 0, dataSize);

            int numThreads = Threads <= 0
                ? Math.Max(1, Environment.ProcessorCount - Threads)
                : Threads;

            int startX, deltaX, startY, deltaY;
            if (Region.IsEmpty)
            {
                startX = 0;
                startY = 0;

                deltaX = l.Width;
                deltaY = (int) Math.Ceiling((double) l.Height / numThreads);
            }
            else
            {
                startX = Region.X;
                startY = Region.Top;

                deltaX = Region.Width;
                deltaY = (int) Math.Ceiling((double) Region.Height / numThreads);
            }

            var tasks = new Task[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                Rectangle rect = new Rectangle(startX, startY + (deltaY * i), deltaX, deltaY);
                tasks[i] = Task.Factory.StartNew(() => ProcessLayerRegion(l, rect, targetBytes));
            }

            Task.WaitAll(tasks);
            _sw.Stop();

            Buffer.BlockCopy(targetBytes, 0, l.BitmapData, 0, dataSize);

            if (OnEffectComplete != null)
                OnEffectComplete(this,
                    new EffectCompleteEventArgs
                    {
                        Threads = numThreads,
                        Ticks = _sw.ElapsedTicks,
                        Frequency = Stopwatch.Frequency,
                        EffectType = GetType(),
                    });
        }

        /// <summary>
        ///     Applies the Effect on the specified <see cref="Layer" /> on the specified SubRegion, Intersects with
        ///     <see cref="Region" />.
        /// </summary>
        /// <param name="l">Layer</param>
        /// <param name="subRegion">Sub Region</param>
        public void ApplyEffect(Layer l, Rectangle subRegion)
        {
            Rectangle oldReg = Region;
            Rectangle newReg;

            if (Region.IsEmpty)
            {
                newReg = subRegion;
            }
            else
            {
                int left = Math.Max(Region.Left, subRegion.Left);
                int right = Math.Min(Region.Right, subRegion.Right);
                int top = Math.Max(Region.Top, subRegion.Top);
                int bottom = Math.Min(Region.Bottom, subRegion.Bottom);
                newReg = new Rectangle(left, top, right - left, bottom - top);
            }

            Region = newReg;
            ApplyEffect(l);
            Region = oldReg;
        }

        /// <summary>
        ///     Process the <see cref="Layer" /> at the Specified Coordinates
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="p">XY Coordinates</param>
        /// <returns>Processed Color</returns>
        protected abstract Color ProcessPixel(Layer layer, Point p);

        private void ProcessLayerRegion(Layer l, Rectangle rect, byte[] target)
        {
            int startX = Math.Max(rect.Left, 0);
            int endX = Math.Min(rect.Right, l.Width);
            int startY = Math.Max(rect.Top, 0);
            int endY = Math.Min(rect.Bottom, l.Height);

            bool emptyRegion = Region.IsEmpty;

            for (int pY = startY; pY < endY; pY++)
            {
                for (int pX = startX; pX < endX; pX++)
                {
                    Point p = new Point(pX, pY);

                    if (!emptyRegion && !Region.Contains(p))
                        continue;

                    int arrIndex = l.GetIndexUnchecked(p);
                    Color col = ProcessPixel(l, p);

                    target[arrIndex + 0] = col.B;
                    target[arrIndex + 1] = col.G;
                    target[arrIndex + 2] = col.R;
                    target[arrIndex + 3] = col.A;
                }
            }
        }
    }

    /// <summary>
    ///     EffectComplete Event Args
    /// </summary>
    public class EffectCompleteEventArgs : EventArgs
    {
        /// <summary>
        ///     Effect Type
        /// </summary>
        public Type EffectType { get; set; }

        /// <summary>
        ///     Processor Frequency
        /// </summary>
        public long Frequency { get; set; }

        /// <summary>
        ///     Used Threads
        /// </summary>
        public int Threads { get; set; }

        /// <summary>
        ///     Ticks Elapsed
        /// </summary>
        public long Ticks { get; set; }
    }
}