// --------------------------------------------------
// AA2Lib - ComparisonConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace AA2Lib.Converters
{
    public class ComparisonConverter : IValueConverter
    {
        private object _returnFalse = false;
        private object _returnTrue = true;

        public object ReturnFalse
        {
            get { return _returnFalse; }
            set { _returnFalse = value; }
        }

        public object ReturnTrue
        {
            get { return _returnTrue; }
            set { _returnTrue = value; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter)
                ? ReturnTrue
                : ReturnFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}