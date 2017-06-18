// --------------------------------------------------
// ReiFX - MirrorEffect.cs
// --------------------------------------------------

using System.Drawing;

namespace ReiFX.Effects
{
    /// <summary>
    ///     Effect to Mirror a <see cref="Layer" /> on its Axes
    /// </summary>
    public class MirrorEffect : EffectBase
    {
        /// <summary>
        ///     Mirror Horizontal
        /// </summary>
        public bool MirrorX { get; set; }

        /// <summary>
        ///     Mirror Vertical
        /// </summary>
        public bool MirrorY { get; set; }

        protected override Color ProcessPixel(Layer layer, Point p)
        {
            return layer.GetPixel(MirrorX
                    ? (layer.Width - 1) - p.X
                    : p.X,
                MirrorY
                    ? (layer.Width - 1) - p.Y
                    : p.Y);
        }
    }
}