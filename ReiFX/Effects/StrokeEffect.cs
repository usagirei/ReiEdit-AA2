// --------------------------------------------------
// ReiFX - StrokeEffect.cs
// --------------------------------------------------

using System;
using System.Drawing;

namespace ReiFX.Effects
{
    /// <summary>
    ///     Effect of a Round Stroke
    /// </summary>
    public class StrokeEffect : EffectBase
    {
        private int _strokeWidth;

        /// <summary>
        ///     Apply Source over Stroke
        /// </summary>
        public bool SourceOver { get; set; }

        /// <summary>
        ///     Stroke Color
        /// </summary>
        public Color StrokeColor { get; set; }

        /// <summary>
        ///     Stroke Width (Bigger = Slower)
        /// </summary>
        public int StrokeWidth
        {
            get { return _strokeWidth; }
            set
            {
                _strokeWidth = value;
                StrokeThreshold = 2.0 / value;
                StrokeGain = value / 2.0;
                StrokeWidthSquared = value * value;
            }
        }

        private double StrokeGain { get; set; }
        private double StrokeThreshold { get; set; }
        private int StrokeWidthSquared { get; set; }

        public StrokeEffect()
        {
            StrokeWidth = 2;
            StrokeColor = new Color(0, 0, 0);
            SourceOver = true;
        }

        protected override Color ProcessPixel(Layer layer, Point p)
        {
            Color stroke = StrokeColor;
            Color sample = layer.GetPixel(p);

            if (layer.IsOpaque(p.X, p.Y))
                return SourceOver
                    ? sample
                    : stroke;

            bool found = false;

            int maxDist = StrokeWidthSquared;
            int minDist = maxDist + 1;

            for (int x = 0; x <= StrokeWidth; x++)
            {
                for (int y = 0; y <= StrokeWidth; y++)
                {
                    int len = (x * x) + (y * y);
                    if (len >= minDist)
                        continue;

                    bool offA = layer.IsTransparent(p.X + x, p.Y + y);
                    bool offB = layer.IsTransparent(p.X - x, p.Y + y);
                    bool offC = layer.IsTransparent(p.X + x, p.Y - y);
                    bool offD = layer.IsTransparent(p.X - x, p.Y - y);
                    if (offA && offB && offC && offD)
                        continue;

                    found = true;
                    minDist = Math.Min(minDist, len);
                }
            }

            if (!found)
                return sample;

            double distDelta = 1.0 - (minDist / (double) maxDist);
            double strokeAlpha;

            if (distDelta < StrokeThreshold)
                strokeAlpha = distDelta * StrokeGain;
            else
                strokeAlpha = 1;

            stroke.A = (byte) (strokeAlpha * stroke.A);

            return SourceOver
                ? stroke & sample
                : stroke;
        }
    }
}