// --------------------------------------------------
// AA2Lib - RegexConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace AA2Lib.Converters
{
    public class RegexConverter : IValueConverter
    {
        public string Match { get; set; }
        public string Replace { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            return input == null
                ? null
                : Regex.Replace(input, Match, Replace);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
            //throw new NotImplementedException();
        }
    }
}