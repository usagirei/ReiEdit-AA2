// --------------------------------------------------
// ReiEditAA2 - ClipboardHelper.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.ViewModels;
using ReiFX;

namespace ReiEditAA2.Code
{
    internal static class ClipboardHelper
    {
        public const string REIAA2_ATTRFLAG = "#REIEDITAA2ATTRIBUTES";
        public const string REIAA2_SUITFLAG = "#REIEDITAA2SUITDATA";

        public static bool CanPasteAttributeString(string prefix)
        {
            if (!Clipboard.ContainsText())
                return false;

            string data = Clipboard.GetText();
            return data.IndexOf(prefix, StringComparison.Ordinal) == 0;
        }

        public static string GetAttributesString(IEnumerable<DataBlockWrapper> attributes, string prefix)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix);
            sb.AppendLine(";");
            foreach (DataBlockWrapper attr in attributes)
            {
                sb.AppendFormat("{0}:{1}='{2}';\r\n", attr.DataType.Name, attr.Key, attr.Value);
            }
            return sb.ToString();
        }

        public static string GetSuitPrefix(ClothesViewModel.ClothesKind kind)
        {
            string src = null;
            switch (kind)
            {
                case ClothesViewModel.ClothesKind.Uniform:
                    src = "UNIFORM";
                    break;
                case ClothesViewModel.ClothesKind.Swimsuit:
                    src = "SWIMSUIT";
                    break;
                case ClothesViewModel.ClothesKind.Sports:
                    src = "SPORT";
                    break;
                case ClothesViewModel.ClothesKind.Formal:
                    src = "CLUB";
                    break;
            }
            return src;
        }

        public static void SetAttributesString(string attributes, CharacterFile character)
        {
            Regex regex = new Regex("$(?<Type>.*?):(?<Key>.*?)='(?<Data>.*?)';",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);
            foreach (Match match in regex.Matches(attributes))
            {
                string type = match.Groups["Type"].Value.Trim('\r', '\n');
                string key = match.Groups["Key"].Value.Trim('\r', '\n');
                string data = match.Groups["Data"].Value.Trim('\r', '\n');
                if (!character.CharAttributes.ContainsKey(key))
                    continue;
                switch (type)
                {
                    case "Boolean":
                        bool @bool;
                        if (Boolean.TryParse(data, out @bool))
                            character[key] = @bool;
                        break;
                    case "Byte":
                        byte @byte;
                        if (Byte.TryParse(data, out @byte))
                            character[key] = @byte;
                        break;
                    case "Int16":
                        short @int16;
                        if (Int16.TryParse(data, out @int16))
                            character[key] = @int16;
                        break;
                    case "Int32":
                        int @int32;
                        if (Int32.TryParse(data, out @int32))
                            character[key] = @int32;
                        break;
                    case "Color":
                        Color @color;
                        if (ColorHelper.TryParseColor(data, out @color))
                            character[key] = @color;
                        break;
                    case "String":
                        character[key] = data;
                        break;
                }
            }
        }
    }
}