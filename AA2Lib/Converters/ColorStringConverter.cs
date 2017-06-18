// --------------------------------------------------
// AA2Lib - ColorStringConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;
using AA2Lib.Code;
using ReiFX;

namespace AA2Lib.Converters
{
    internal class ColorStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                if (targetType == typeof(string))
                {
                    return ((Color) value).ToString();
                }
                if (targetType == typeof(System.Windows.Media.Color))
                {
                    System.Windows.Media.Color mc = ((Color) value);
                    return mc;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return null;

            string input = (string) value;
            Color c;
            if (ColorHelper.TryParseColor(input, out c))
                return c;

            return null;
        }
    }
}