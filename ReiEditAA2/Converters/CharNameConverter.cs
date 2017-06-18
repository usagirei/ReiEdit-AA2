// --------------------------------------------------
// ReiEditAA2 - CharNameConverter.cs
// --------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using ReiEditAA2.Code;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Converters
{
    internal class CharNameConverter : Freezable, IValueConverter
    {
        public static readonly DependencyProperty CharacterCollectionProperty = DependencyProperty.Register("CharacterCollection",
            typeof(CharacterCollection),
            typeof(CharNameConverter),
            new PropertyMetadata(default(CharacterCollection)));

        public CharacterCollection CharacterCollection
        {
            get { return (CharacterCollection) GetValue(CharacterCollectionProperty); }
            set { SetValue(CharacterCollectionProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new CharNameConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || CharacterCollection == null)
                return null;
            int seatNum;
            if (value is string && parameter is string)
            {
                seatNum = System.Convert.ToInt32
                (Regex.Match(value as string, parameter as string)
                    .Groups["value"].Value);
            }
            else
            {
                seatNum = System.Convert.ToInt32(value);
            }
            CharacterViewModel seatedChar = (from character in CharacterCollection.Characters
                where character.ExtraData.ContainsKey("PLAY_SEAT")
                where (int) character.ExtraData["PLAY_SEAT"] == seatNum
                select character).FirstOrDefault();
            return seatedChar != null
                ? string.Format("[{0:D2}] {1}", seatNum, seatedChar.Profile.FullName)
                : string.Format("Nº{0:D2}", seatNum);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}