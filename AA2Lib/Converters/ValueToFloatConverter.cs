// --------------------------------------------------
// AA2Lib - ValueToFloatConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace AA2Lib.Converters
{
    public class BrightnessToFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;
            double f = (int) value / 100.0;
            return f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}