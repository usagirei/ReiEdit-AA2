// --------------------------------------------------
// ReiEditAA2 - KeyIndentConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ReiEditAA2.Converters
{
    internal class KeyIndentConverter : IValueConverter
    {
        public string IndentString { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;

            int dotIndents = string.IsNullOrWhiteSpace(input)
                ? 0
                : input.Count(c => c == '.');

            int keyIndents = string.IsNullOrWhiteSpace(input)
                ? 0
                : input.Count(c => c == '[');

            int indents = keyIndents + dotIndents;

            return String.Concat(Enumerable.Repeat(IndentString, indents)) + input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; //throw new NotImplementedException();
        }
    }
}