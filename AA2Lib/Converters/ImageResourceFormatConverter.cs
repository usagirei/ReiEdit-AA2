// --------------------------------------------------
// AA2Lib - ImageResourceFormatConverter.cs
// --------------------------------------------------

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AA2Lib.Converters
{
    public class ImageResourceFormatConverter : IMultiValueConverter
    {
        private static string[] _resourceNames;

        public static string[] ResourceNames
        {
            get { return _resourceNames ?? (_resourceNames = GetResourceNames()); }
        }

        public static string[] GetResourceNames()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resName = assembly.GetName()
                                 .Name + ".g.resources";
            using (Stream stream = assembly.GetManifestResourceStream(resName))
            {
                if (stream == null)
                    return new string[0];

                using (ResourceReader reader = new ResourceReader(stream))
                {
                    return reader.Cast<DictionaryEntry>()
                        .Select(entry => (string) entry.Key)
                        .ToArray();
                }
            }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string formatted = String.Format((string) parameter, values);
            string @ref = formatted.ToLower()
                .Replace('\\', '/');
            if (!ResourceNames.Contains(@ref))
                return null;

            const string baseUri = @"pack://application:,,,/AA2Lib;component\";
            Uri uri = new Uri(baseUri + formatted, UriKind.RelativeOrAbsolute);
            return new BitmapImage(uri);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}