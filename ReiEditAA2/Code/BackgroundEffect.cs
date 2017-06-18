// --------------------------------------------------
// ReiEditAA2 - BackgroundEffect.cs
// --------------------------------------------------

using System;
using System.Drawing;
using ReiFX;
using Color = ReiFX.Color;

namespace ReiEditAA2.Code
{
    internal class BackgroundEffect : EffectBase
    {
        private double _angleRad;
        private double _cosAngle;
        private double _phase;
        private double _sinAngle;

        public Color Color { get; set; }

        public bool IsRainbow { get; set; }

        public double RainbowAngle
        {
            get { return (Math.PI * 2) * _angleRad; }
            set
            {
                _angleRad = value * Math.PI / 180f;
                _sinAngle = Math.Sin(_angleRad);
                _cosAngle = Math.Cos(_angleRad);
            }
        }

        public Point RainbowCenter { get; set; }

        public double RainbowPhase
        {
            get { return (Math.PI * 2) * _phase; }
            set { _phase = value * Math.PI / 180f; }
        }

        public bool RainbowRadial { get; set; }

        public BackgroundEffect()
        {
            RainbowAngle = 0;
            RainbowCenter = new Point(0, 0);
            RainbowPhase = 0;
            RainbowRadial = false;
            Color = new Color(255, 255, 255);
            IsRainbow = false;
        }

        protected override Color ProcessPixel(Layer layer, Point p)
        {
            Color bgCol;
            if (IsRainbow)
            {
                double angle;
                if (RainbowRadial)
                {
                    Point distancePoint = p - (Size) RainbowCenter;
                    angle = Math.Atan2(distancePoint.Y, distancePoint.X) + _phase;
                }
                else
                {
                    angle = (p.Y * _sinAngle + p.X * _cosAngle) / (layer.Height * _sinAngle + layer.Width * _cosAngle)
                            + _phase;
                    angle *= Math.PI * 2;
                }
                angle = angle * 180.0 / Math.PI;
                bgCol = Color.FromHsv(angle, 1d, 1d);
            }
            else
            {
                bgCol = Color;
            }

            bgCol.R = (byte) (50 + bgCol.R / 1.7);
            bgCol.G = (byte) (50 + bgCol.G / 1.7);
            bgCol.B = (byte) (50 + bgCol.B / 1.7);

            Color bgSample = layer.GetPixel(p);

            return bgCol + bgSample;
        }
    }
}