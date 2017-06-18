// --------------------------------------------------
// ReiEditAA2 - CharacterHairProvider.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using AA2Lib;

namespace ReiEditAA2.Code
{
    internal static class CharacterHairProvider
    {
        public static IEnumerable<CharacterHair> BackHairs
        {
            get
            {
                return Items.Where(item => item.Slot == CharacterHair.HairSlot.Back)
                    .OrderBy(hair => hair.Id);
            }
        }

        public static IEnumerable<CharacterHair> ExtensionHairs
        {
            get
            {
                return Items.Where(item => item.Slot == CharacterHair.HairSlot.Extension)
                    .OrderBy(hair => hair.Id);
            }
        }

        public static IEnumerable<CharacterHair> FrontHairs
        {
            get
            {
                return Items.Where(item => item.Slot == CharacterHair.HairSlot.Front)
                    .OrderBy(hair => hair.Id);
            }
        }

        public static IList<CharacterHair> Items { get; private set; }

        public static IEnumerable<CharacterHair> SideHairs
        {
            get
            {
                return Items.Where(item => item.Slot == CharacterHair.HairSlot.Side)
                    .OrderBy(hair => hair.Id);
            }
        }

        static CharacterHairProvider()
        {
            Stream schemaStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"ReiEditAA2.XML.random.xsd");

            if (schemaStream == null)
                return;

            Items = new List<CharacterHair>();

            StreamReader srSchema = new StreamReader(schemaStream, Encoding.GetEncoding("utf-8"));
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", XmlReader.Create(srSchema));

            string rndDir = Path.Combine(Core.StartupPath, "XML", "Random");
            if (!Directory.Exists(rndDir))
                return;

            var files = Directory.EnumerateFiles(rndDir, "*.xml");

            foreach (string file in files)
            {
                XDocument xDocument = XDocument.Load(file);
                bool errors = false;
                xDocument.Validate(schemaSet,
                    (o, e) =>
                    {
                        Debug.Print("{0}", e.Message);
                        errors = true;
                    },
                    true);
                if (errors)
                    continue;

                XElement randomRoot = xDocument.Element("random");
                if (randomRoot == null)
                    continue;

                XElement itemsRoot = randomRoot.Element("hairs");
                if (itemsRoot == null)
                    continue;

                var xmlElements = itemsRoot.Elements("hair");

                var randomItems = from element in xmlElements
                    let valueValue = element.Value
                    let kindValue = element.Attribute("kind")
                        .Value
                    let idValue = element.Attribute("id")
                        .Value.ToByte()
                    let flippable = Boolean.Parse
                    (element.Attribute("flip")
                        .Value)
                    select new CharacterHair
                    {
                        Slot =
                            (CharacterHair.HairSlot)
                            Enum.Parse(typeof(CharacterHair.HairSlot), kindValue, true),
                        Id = idValue,
                        Flippable = flippable
                    };

                foreach (CharacterHair item in randomItems)
                {
                    if (!Items.Contains(item))
                        Items.Add(item);
                }

                //Items = Items.OrderBy(hair => hair.Id)
            }
        }

        public class CharacterHair
        {
            public enum HairSlot
            {
                Front = 0,
                Side = 1,
                Back = 2,
                Extension = 3
            }

            public bool Flippable { get; set; }

            public byte Id { get; set; }
            public HairSlot Slot { get; set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((CharacterHair) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Id.GetHashCode() * 397) ^ (int) Slot;
                }
            }

            public override string ToString()
            {
                return String.Format("{0,-3:D} {1}",
                    Id,
                    Flippable
                        ? "[Flip]"
                        : "");
            }

            protected bool Equals(CharacterHair other)
            {
                return Id == other.Id && Slot == other.Slot;
            }
        }
    }
}