// --------------------------------------------------
// AA2Lib - Core.DataTemplates.cs
// --------------------------------------------------

using System.Windows;

namespace AA2Lib
{
    public static partial class Core
    {
        public static class DataTemplates
        {
            public static DataTemplate BooleanNameDataTemplate
            {
                get
                {
                    return
                        (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.BooleanNameDataTemplateKey];
                }
            }

            public static DataTemplate BooleanTFDataTemplate
            {
                get
                {
                    return
                        (DataTemplate)
                        SharedResources[AA2Lib.SharedResources.DataTemplates.BooleanTrueFalseDataTemplateKey];
                }
            }

            public static DataTemplate ByteDataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.ByteDataTemplateKey]; }
            }

            public static DataTemplate ByteFDataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.ByteFDataTemplateKey]; }
            }

            public static DataTemplate ColorDataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.ColorDataTemplateKey]; }
            }

            public static DataTemplate EnumDataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.EnumDataTemplateKey]; }
            }

            public static DataTemplate Int16DataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.Int16DataTemplateKey]; }
            }

            public static DataTemplate Int16FDataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.Int16FDataTemplateKey]; }
            }

            public static DataTemplate Int32DataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.Int32DataTemplateKey]; }
            }

            public static DataTemplate Int32FDataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.Int32FDataTemplateKey]; }
            }

            public static DataTemplate StringDataTemplate
            {
                get { return (DataTemplate) SharedResources[AA2Lib.SharedResources.DataTemplates.StringDataTemplateKey]; }
            }
        }
    }
}