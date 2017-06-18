// --------------------------------------------------
// AA2Lib - HueToFloatConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace AA2Lib.Converters
{
    public class HueToFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;
            double f = (int) value / 360.0;
            f %= 1.0;
            if (f < 0)
                f += 1.0;
            return f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}