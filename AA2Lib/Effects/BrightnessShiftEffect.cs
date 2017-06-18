// --------------------------------------------------
// AA2Lib - BrightnessShiftEffect.cs
// --------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AA2Lib.Effects
{
    public class BrightnessShiftEffect : ShaderEffect
    {
        public static readonly DependencyProperty HueShiftProperty = DependencyProperty.Register("Shift",
            typeof(float),
            typeof(BrightnessShiftEffect),
            new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(BrightnessShiftEffect), 0);

        private static readonly PixelShader Shader = new PixelShader();

        public Brush Input
        {
            get { return (Brush) GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public float Shift
        {
            get { return (float) GetValue(HueShiftProperty); }
            set { SetValue(HueShiftProperty, value); }
        }

        static BrightnessShiftEffect()
        {
            // Associate _pixelShader with our compiled pixel shader
            Shader.UriSource = new Uri(@"pack://application:,,,/AA2Lib;component/Effects/Bin/ValShift.ps");
        }

        public BrightnessShiftEffect()
        {
            PixelShader = Shader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(HueShiftProperty);
        }
    }
}