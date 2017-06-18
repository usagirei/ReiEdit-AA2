// --------------------------------------------------
// AA2Lib - ThresholdConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace AA2Lib.Converters
{
    public class ThresholdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double input = System.Convert.ToDouble(value);
            double max = System.Convert.ToDouble(parameter);
            return input >= max;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}