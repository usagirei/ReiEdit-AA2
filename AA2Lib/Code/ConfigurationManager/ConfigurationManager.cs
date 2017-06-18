// --------------------------------------------------
// AA2Lib - ConfigurationManager.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace AA2Lib.Code.ConfigurationManager
{
    public class ConfigurationManager
    {
        private static readonly ConfigurationManager InternalInstance = new ConfigurationManager();
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        private readonly Dictionary<string, ConfigurationItem> _configs = new Dictionary<string, ConfigurationItem>();

        public static ConfigurationManager Instance
        {
            get { return InternalInstance; }
        }

        [IndexerName("Item")]
        public ConfigurationItem this[string key]
        {
            get { return GetConfigItem(key); }
            set { SetConfigItem(key, value); }
        }

        public Dictionary<string, ConfigurationItem> Configs
        {
            get { return _configs; }
        }

        static ConfigurationManager()
        { }

        private ConfigurationManager()
        { }

        public object GetItem(string key)
        {
            ConfigurationItem item = GetConfigItem(key);
            if (item == null)
                return null;
            return item.Value;
        }

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;
            XDocument xDoc = XDocument.Load(path);
            var configItems = xDoc.Element("configs")
                .Elements("config");

            foreach (XElement element in configItems)
            {
                string key = element.Attribute("key")
                    .Value;
                object value = DeserializeValue(element.Value);

                ConfigurationItem item = new ConfigurationItem(key, value);

                _configs.Add(key, item);
            }
        }

        public void Save(string path)
        {
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("configs",
                    from item in _configs.Values.Where(item => item.ShoudSave)
                    select new XElement("config", new XAttribute("key", item.Key), SerializeValue(item.Value))));

            xDoc.Save(path);
        }

        public void SetItem(string key, object value)
        {
            ConfigurationItem item = GetConfigItem(key);
            if (item == null)
                SetConfigItem(key, new ConfigurationItem(key, value));
            else
                item.Value = value;
        }

        private object DeserializeValue(string obj)
        {
            var bytes = Convert.FromBase64String(obj);
            MemoryStream ms = new MemoryStream(bytes);
            return _binaryFormatter.Deserialize(ms);
        }

        private ConfigurationItem GetConfigItem(string key)
        {
            return _configs.ContainsKey(key)
                ? _configs[key]
                : null;
        }

        private string SerializeValue(object obj)
        {
            MemoryStream ms = new MemoryStream();
            _binaryFormatter.Serialize(ms, obj);
            string b64 = Convert.ToBase64String(ms.ToArray());
            return b64;
        }

        private void SetConfigItem(string key, ConfigurationItem value)
        {
            if (_configs.ContainsKey(key))
            {
                if (value == null)
                    _configs.Remove(key);
                else
                    _configs[key] = value;
            }
            else
            {
                if (value != null)
                    _configs.Add(key, value);
            }
        }
    }
}