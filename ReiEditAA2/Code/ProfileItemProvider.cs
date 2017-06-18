// --------------------------------------------------
// ReiEditAA2 - ProfileItemProvider.cs
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
    internal static class ProfileItemProvider
    {
        public static IList<ProfileItem> Items { get; private set; }

        static ProfileItemProvider()
        {
            Stream schemaStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"ReiEditAA2.XML.random.xsd");

            if (schemaStream == null)
                return;

            Items = new List<ProfileItem>();

            StreamReader srSchema = new StreamReader(schemaStream, Encoding.GetEncoding("utf-8"));
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", XmlReader.Create(srSchema));

            var files = Directory.EnumerateFiles(Path.Combine(Core.StartupPath, "XML", "Random"), "*.xml");

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

                XElement itemsRoot = randomRoot.Element("items");
                if (itemsRoot == null)
                    continue;

                var xmlElements = itemsRoot.Elements("item");

                var randomItems = from element in xmlElements
                    let valueValue = element.Value
                    let kindValue = element.Attribute("kind")
                        .Value
                    let genderValue = element.Attribute("gender")
                        .Value
                    let personalities = element.Attribute("personality")
                        .Value.Split(';')
                        .Select(s => Convert.ToInt32(s))
                        .ToArray()
                    select new ProfileItem
                    {
                        Text = valueValue,
                        Personalities = personalities,
                        Type =
                            (ProfileItem.ItemType)
                            Enum.Parse(typeof(ProfileItem.ItemType), kindValue, true),
                        Gender =
                            (ProfileItem.ItemGender)
                            Enum.Parse(typeof(ProfileItem.ItemGender), genderValue, true)
                    };

                foreach (ProfileItem item in randomItems)
                {
                    Items.Add(item);
                }
            }
        }

        public static string GenerateFriendItem(int personality = -1, int gender = -1)
        {
            ProfileItem.ItemGender itemGender = (ProfileItem.ItemGender) gender;
            var selected = (from item in Items
                where item.Type == ProfileItem.ItemType.Friend
                where item.Gender == ProfileItem.ItemGender.Any || item.Gender == itemGender
                where item.Personalities.Contains(-1) || item.Personalities.Contains(personality)
                select item).ToArray();
            return GetRandomText(selected);
        }

        public static string GenerateLoversItem(int personality = -1, int gender = -1)
        {
            ProfileItem.ItemGender itemGender = (ProfileItem.ItemGender) gender;
            var selected = (from item in Items
                where item.Type == ProfileItem.ItemType.Lovers
                where item.Gender == ProfileItem.ItemGender.Any || item.Gender == itemGender
                where item.Personalities.Contains(-1) || item.Personalities.Contains(personality)
                select item).ToArray();
            return GetRandomText(selected);
        }

        public static string GenerateSexualItem(int personality = -1, int gender = -1)
        {
            ProfileItem.ItemGender itemGender = (ProfileItem.ItemGender) gender;
            var selected = (from item in Items
                where item.Type == ProfileItem.ItemType.Sexual
                where item.Gender == ProfileItem.ItemGender.Any || item.Gender == itemGender
                where item.Personalities.Contains(-1) || item.Personalities.Contains(personality)
                select item).ToArray();

            return GetRandomText(selected);
        }

        private static string GetRandomText(IList<ProfileItem> items)
        {
            int random = Core.Random.Next(items.Count);
            return items[random].Text;
        }

        public class ProfileItem
        {
            public enum ItemGender
            {
                Any,
                Male,
                Female,
            }

            public enum ItemType
            {
                Lovers,
                Friend,
                Sexual
            }

            public ItemGender Gender { get; set; }

            public int[] Personalities { get; set; }
            public string Text { get; set; }
            public ItemType Type { get; set; }
        }
    }
}