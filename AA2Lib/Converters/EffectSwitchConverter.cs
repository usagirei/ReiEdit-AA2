// --------------------------------------------------
// AA2Lib - EffectSwitchConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Effects;

namespace AA2Lib.Converters
{
    public class EffectSwitchConverter : IValueConverter
    {
        public Effect EffectFalse { get; set; }
        public Effect EffectTrue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(bool))
                return null;
            bool sw = (bool) value;
            return sw
                ? EffectTrue
                : EffectFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}