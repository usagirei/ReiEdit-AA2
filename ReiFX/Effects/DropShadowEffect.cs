// --------------------------------------------------
// ReiFX - DropShadowEffect.cs
// --------------------------------------------------

using System.Drawing;

namespace ReiFX.Effects
{
    /// <summary>
    ///     Effect of a Smooth Round Drop Shadow
    /// </summary>
    public class DropShadowEffect : EffectBase
    {
        /// <summary>
        ///     Shadow Color
        /// </summary>
        public Color ShadowColor { get; set; }

        /// <summary>
        ///     Shadow Radius (Bigger = Slower)
        /// </summary>
        public int ShadowSize { get; set; }

        public DropShadowEffect()
        {
            ShadowColor = new Color(0, 0, 0);
            ShadowSize = 3;
        }

        protected override Color ProcessPixel(Layer layer, Point p)
        {
            Color self = layer.GetPixel(p);
            if (self.A == 255)
                return self;

            int deltaScan = 2 * ShadowSize + 1;
            float maxScan = deltaScan * deltaScan;
            float deltaA = ShadowColor.A / maxScan;

            int count = 0;
            for (int x = -ShadowSize; x <= ShadowSize; x++)
            {
                for (int y = -ShadowSize; y <= ShadowSize; y++)
                {
                    Color offset = layer.GetPixel(p.X + x, p.Y + y);
                    if (offset.A > 0)
                        count++;
                }
            }

            Color shadowColor = ShadowColor;
            shadowColor.A = (byte) (deltaA * count).Clamp(0, 255);

            return shadowColor & self;
        }
    }
}