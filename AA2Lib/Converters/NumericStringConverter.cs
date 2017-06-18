// --------------------------------------------------
// AA2Lib - NumericStringConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace AA2Lib.Converters
{
    public class NumericStringConverter : IValueConverter
    {
        public enum NumberTypeEnum
        {
            Byte,
            Int16,
            Int32
        }

        private NumberTypeEnum _numberType = NumberTypeEnum.Byte;

        public NumberTypeEnum NumberType
        {
            get { return _numberType; }
            set { _numberType = value; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (NumberType)
            {
                case NumberTypeEnum.Byte:
                    if (value is Byte)
                        return ((Byte) value).ToString(CultureInfo.InvariantCulture);
                    break;
                case NumberTypeEnum.Int16:
                    if (value is Int16)
                        return ((Int16) value).ToString(CultureInfo.InvariantCulture);
                    break;
                case NumberTypeEnum.Int32:
                    if (value is Int32)
                        return ((Int32) value).ToString(CultureInfo.InvariantCulture);
                    break;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (s == null)
                return null;
            switch (NumberType)
            {
                case NumberTypeEnum.Byte:
                    return s.ToByte()
                        .Clamp(Byte.MinValue, Byte.MaxValue);
                case NumberTypeEnum.Int16:
                    return s.ToInt16()
                        .Clamp(Int16.MinValue, Int16.MaxValue);
                case NumberTypeEnum.Int32:
                    return s.ToInt32()
                        .Clamp(Int32.MinValue, Int32.MaxValue);
            }
            return null;
        }
    }
}