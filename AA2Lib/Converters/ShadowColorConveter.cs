// --------------------------------------------------
// AA2Lib - ShadowColorConveter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AA2Lib.Code;

namespace AA2Lib.Converters
{
    public class ShadowColorConveter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 0 || values[0] == DependencyProperty.UnsetValue)
                return null;
            double hue = 185 + (int) values[0];
            double saturation = (int) values[1] / 100.0;
            double value = 0.5 + (int) values[2] / 200.0;
            if (hue < 0)
                hue += 360.0;
            return (System.Windows.Media.Color) ColorHelper.ColorFromHSL(hue, saturation, value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}