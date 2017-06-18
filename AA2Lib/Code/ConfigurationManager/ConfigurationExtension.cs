// --------------------------------------------------
// AA2Lib - ConfigurationExtension.cs
// --------------------------------------------------

using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace AA2Lib.Code.ConfigurationManager
{
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class ConfigurationExtension : MarkupExtension
    {
        private static readonly ConfigurationManager ConfigManager = ConfigurationManager.Instance;

        public object DefaultValue { get; set; }

        public object IgnoreValue { get; set; }
        public string Key { get; set; }

        public ConfigurationExtension()
        { }

        public ConfigurationExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            bool ignoreIsNull = IgnoreValue == null;
            bool defaultIsNull = DefaultValue == null;
            bool configItemIsNull = ConfigManager[Key] == null;

            //Prevent Default = Ignore
            if (!ignoreIsNull && !defaultIsNull && IgnoreValue.Equals(DefaultValue))
            {
                throw new Exception("IgnoreValue must not be equal DefaultValue");
            }
            // If ConfigurationItem does not exist, Set to Default 
            if (configItemIsNull)
            {
                ConfigManager[Key] = new ConfigurationItem(Key, DefaultValue);
            }
            // If Ignore != null, ConfigurationItem Exists, and ConfigurationItem Value == Ignore, Set to Default
            else if (!ignoreIsNull && ConfigManager[Key].Value.Equals(IgnoreValue))
            {
                ConfigManager[Key] = new ConfigurationItem(Key, DefaultValue);
            }

            Binding binding = new Binding
            {
                Source = ConfigManager,
                Path = new PropertyPath(string.Format("[{0}].Value", Key)),
                Mode = BindingMode.TwoWay,
            };
            return binding.ProvideValue(serviceProvider);
        }

        public static void LoadConfigs()
        {
            ConfigManager.Load(Path.Combine(Core.StartupPath, "XML", "config.xml"));
        }

        public static void SaveConfigs()
        {
            ConfigManager.Save(Path.Combine(Core.StartupPath, "XML", "config.xml"));
        }
    }
}