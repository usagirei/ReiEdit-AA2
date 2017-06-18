// --------------------------------------------------
// AA2Lib - MultiplyEffect.cs
// --------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AA2Lib.Effects
{
    public class MultiplyEffect : ShaderEffect
    {
        public static readonly DependencyProperty Input2Property = RegisterPixelShaderSamplerProperty("Input2", typeof(MultiplyEffect), 1);
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(MultiplyEffect), 0);

        private static readonly PixelShader Shader = new PixelShader();

        public Brush Input
        {
            get { return (Brush) GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public Brush Input2
        {
            get { return (Brush) GetValue(Input2Property); }
            set { SetValue(Input2Property, value); }
        }

        static MultiplyEffect()
        {
            // Associate _pixelShader with our compiled pixel shader
            Shader.UriSource = new Uri(@"pack://application:,,,/AA2Lib;component/Effects/Bin/Multiply.ps");
        }

        public MultiplyEffect()
        {
            PixelShader = Shader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(Input2Property);
        }
    }
}