// --------------------------------------------------
// AA2Lib - SharedResources.DataTemplates.xaml.cs
// --------------------------------------------------

using System.Windows;

namespace AA2Lib
{
    public partial class SharedResources
    {
        public static class DataTemplates
        {
            public static ResourceKey BooleanNameDataTemplateKey { get; private set; }
            public static ResourceKey BooleanTrueFalseDataTemplateKey { get; private set; }

            public static ResourceKey ByteDataTemplateKey { get; private set; }

            public static ResourceKey ByteFDataTemplateKey { get; private set; }

            public static ResourceKey ColorDataTemplateKey { get; private set; }

            public static ResourceKey EnumDataTemplateKey { get; private set; }

            public static ResourceKey Int16DataTemplateKey { get; private set; }

            public static ResourceKey Int16FDataTemplateKey { get; private set; }

            public static ResourceKey Int32DataTemplateKey { get; private set; }

            public static ResourceKey Int32FDataTemplateKey { get; private set; }

            public static ResourceKey StringDataTemplateKey { get; private set; }

            static DataTemplates()
            {
                BooleanTrueFalseDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "BooleanTFDataTemplate");
                BooleanNameDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "BooleanNameDataTemplate");
                ByteDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "ByteDataTemplate");
                ByteFDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "ByteFDataTemplate");
                ColorDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "ColorDataTemplate");
                EnumDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "EnumDataTemplate");
                Int16DataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "Int16DataTemplate");
                Int16FDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "Int16FDataTemplate");
                Int32DataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "Int32DataTemplate");
                Int32FDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "Int32FDataTemplate");
                StringDataTemplateKey = new ComponentResourceKey(typeof(SharedResources), "StringDataTemplate");
            }
        }
    }
}