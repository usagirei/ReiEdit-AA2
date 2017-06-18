// --------------------------------------------------
// AA2Lib - ColorConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;
using ReiFX;

namespace AA2Lib.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color))
                return null;
            return (System.Windows.Media.Color) (Color) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is System.Windows.Media.Color))
                return null;
            return (Color) (System.Windows.Media.Color) value;
        }
    }
}