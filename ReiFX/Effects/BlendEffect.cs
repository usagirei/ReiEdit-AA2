// --------------------------------------------------
// ReiFX - BlendEffect.cs
// --------------------------------------------------

using System.Drawing;

namespace ReiFX.Effects
{
    /// <summary>
    ///     Effect to Blend Two <see cref="Layer" />s
    /// </summary>
    public class BlendEffect : EffectBase
    {
        /// <summary>
        ///     Blend Modes
        /// </summary>
        public enum BlendMode
        {
            /// <summary>
            ///     Alpha Blends
            /// </summary>
            AlphaBlend,

            /// <summary>
            ///     Replaces with Overlay
            /// </summary>
            Replace,

            /// <summary>
            ///     Adds Overlay
            /// </summary>
            Add,

            /// <summary>
            ///     Subtracts Overlay
            /// </summary>
            Subtract,

            /// <summary>
            ///     Multiplies with Overlay
            /// </summary>
            Multiply
        }

        /// <summary>
        ///     Overlay Source
        /// </summary>
        public enum OverlaySource
        {
            /// <summary>
            ///     Target <see cref="BlendEffect.OverlayLayer" />
            /// </summary>
            Layer,

            /// <summary>
            ///     Target <see cref="BlendEffect.OverlayColor" />
            /// </summary>
            Color
        }

        /// <summary>
        ///     Blend Mode
        /// </summary>
        public BlendMode Mode { get; set; }

        /// <summary>
        ///     Offset
        /// </summary>
        public Point Offset { get; set; }

        /// <summary>
        ///     Overlay Color
        /// </summary>
        public Color OverlayColor { get; set; }

        /// <summary>
        ///     Overlay Layer
        /// </summary>
        public Layer OverlayLayer { get; set; }

        /// <summary>
        ///     Overlay Source
        /// </summary>
        public OverlaySource Source { get; set; }

        /// <summary>
        ///     Creates a new <see cref="BlendEffect" /> Instance with
        ///     <see cref="Mode" /> = <see cref="BlendMode.AlphaBlend" /> and
        ///     <see cref="Source" /> = <see cref="OverlaySource.Layer" />
        /// </summary>
        public BlendEffect()
        {
            Source = OverlaySource.Layer;
            Mode = BlendMode.AlphaBlend;
        }

        protected override Color ProcessPixel(Layer layer, Point p)
        {
            Color background = layer.GetPixel(p);
            Color overlay;

            switch (Source)
            {
                case OverlaySource.Color:
                {
                    overlay = OverlayColor;
                    break;
                }
                case OverlaySource.Layer:
                {
                    Rectangle targetRect = new Rectangle(0, 0, OverlayLayer.Width, OverlayLayer.Height);
                    Point targetPt = new Point(p.X - Offset.X, p.Y - Offset.Y);

                    if (!targetRect.Contains(targetPt) || OverlayLayer.IsTransparent(targetPt))
                        return background;

                    overlay = OverlayLayer.GetPixel(targetPt);
                    break;
                }
                default:
                {
                    overlay = new Color();
                    break;
                }
            }

            switch (Mode)
            {
                default:
                    return background;
                case BlendMode.Replace:
                    return new Color(overlay, background.A);
                case BlendMode.AlphaBlend:
                    return background & overlay;
                case BlendMode.Add:
                    return background + overlay;
                case BlendMode.Subtract:
                    return background - overlay;
                case BlendMode.Multiply:
                    return background * overlay;
            }
        }
    }

    /// <summary>
    ///     Effect to Change <see cref="Layer" /> Opacity
    /// </summary>
    public class OpacityEffect : EffectBase
    {
        /// <summary>
        ///     Opacity Value
        /// </summary>
        public float Opacity { get; set; }

        protected override Color ProcessPixel(Layer layer, Point p)
        {
            Color @in = layer.GetPixel(p);
            byte newAlpha = (byte) (@in.A * Opacity);
            Color @out = new Color(@in, newAlpha);
            return @out;
        }
    }
}