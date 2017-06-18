// --------------------------------------------------
// AA2Lib - Class1.cs
// Created: 28/07/2014 21:38
// --------------------------------------------------

#region Usings
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

#endregion

namespace AA2Lib.Converters
{

    public class ValueConverterChain : List<IValueConverter> , IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = value;
            foreach (IValueConverter converter in this)
                result = converter.Convert(result, targetType, parameter, culture);
            
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
            //throw new NotImplementedException();
        }
        #endregion
    }

}