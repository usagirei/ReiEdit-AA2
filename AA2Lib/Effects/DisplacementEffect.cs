// --------------------------------------------------
// AA2Lib - DisplacementEffect.cs
// --------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AA2Lib.Effects
{
    public class DisplacementEffect : ShaderEffect
    {
        public static readonly DependencyProperty DisplaceBrushProperty =
            RegisterPixelShaderSamplerProperty("DisplaceBrush", typeof(DisplacementEffect), 1);

        public static readonly DependencyProperty DisplacementAmountProperty = DependencyProperty.Register("DisplacementAmount",
            typeof(double),
            typeof(DisplacementEffect),
            new UIPropertyMetadata(0.015D, PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register("GlowColor",
            typeof(Color),
            typeof(DisplacementEffect),
            new UIPropertyMetadata(Color.FromArgb(127, 255, 255, 255), PixelShaderConstantCallback(6)));

        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(DisplacementEffect), 0);

        public static readonly DependencyProperty LightColorProperty = DependencyProperty.Register("LightColor",
            typeof(Color),
            typeof(DisplacementEffect),
            new UIPropertyMetadata(Color.FromArgb(127, 255, 255, 255), PixelShaderConstantCallback(5)));

        public static readonly DependencyProperty LightHeightProperty = DependencyProperty.Register("LightHeight",
            typeof(double),
            typeof(DisplacementEffect),
            new UIPropertyMetadata(1D, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty LightPosProperty = DependencyProperty.Register("LightPos",
            typeof(Point),
            typeof(DisplacementEffect),
            new UIPropertyMetadata(new Point(1D, 1D), PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty ShadowColorProperty = DependencyProperty.Register("ShadowColor",
            typeof(Color),
            typeof(DisplacementEffect),
            new UIPropertyMetadata(Color.FromArgb(255, 90, 190, 200), PixelShaderConstantCallback(3)));

        private static readonly PixelShader Shader = new PixelShader();

        /// <summary>Displacement Brush</summary>
        public Brush DisplaceBrush
        {
            get { return ((Brush) (GetValue(DisplaceBrushProperty))); }
            set { SetValue(DisplaceBrushProperty, value); }
        }

        /// <summary>Displacement Amount</summary>
        public double DisplacementAmount
        {
            get { return ((double) (GetValue(DisplacementAmountProperty))); }
            set { SetValue(DisplacementAmountProperty, value); }
        }

        /// <summary>Glow Color </summary>
        public Color GlowColor
        {
            get { return ((Color) (GetValue(GlowColorProperty))); }
            set { SetValue(GlowColorProperty, value); }
        }

        public Brush Input
        {
            get { return ((Brush) (GetValue(InputProperty))); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>Light Color</summary>
        public Color LightColor
        {
            get { return ((Color) (GetValue(LightColorProperty))); }
            set { SetValue(LightColorProperty, value); }
        }

        public double LightHeight
        {
            get { return ((double) (GetValue(LightHeightProperty))); }
            set { SetValue(LightHeightProperty, value); }
        }

        /// <summary>Light Direction</summary>
        public Point LightPos
        {
            get { return ((Point) (GetValue(LightPosProperty))); }
            set { SetValue(LightPosProperty, value); }
        }

        /// <summary>Shadow</summary>
        public Color ShadowColor
        {
            get { return ((Color) (GetValue(ShadowColorProperty))); }
            set { SetValue(ShadowColorProperty, value); }
        }

        public DisplacementEffect()
        {
            Shader.UriSource = new Uri(@"pack://application:,,,/AA2Lib;component/Effects/Bin/Displacement.ps");
            PixelShader = Shader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(DisplaceBrushProperty);
            UpdateShaderValue(LightPosProperty);
            UpdateShaderValue(LightHeightProperty);
            UpdateShaderValue(DisplacementAmountProperty);
            UpdateShaderValue(ShadowColorProperty);
            UpdateShaderValue(LightColorProperty);
            UpdateShaderValue(GlowColorProperty);
        }
    }
}