// --------------------------------------------------
// AA2Lib - DataBlockTemplateSelector.cs
// --------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace AA2Lib.Code.DataBlocks
{
    public class DataBlockTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataBlockWrapper attr = item as DataBlockWrapper;
            if (attr == null || attr.DataType == null)
                return base.SelectTemplate(item, container);
            switch (attr.DataType.Name)
            {
                case "Byte":
                    if (attr.IsEnum)
                        return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.EnumDataTemplateKey];
                    if (attr.IsRanged)
                        return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.ByteFDataTemplateKey];
                    return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.ByteDataTemplateKey];
                case "Int16":
                    if (attr.IsEnum)
                        return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.EnumDataTemplateKey];
                    if (attr.IsRanged)
                        return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.Int16FDataTemplateKey];
                    return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.Int16DataTemplateKey];
                case "Int32":
                    if (attr.IsEnum)
                        return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.EnumDataTemplateKey];
                    if (attr.IsRanged)
                        return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.Int32FDataTemplateKey];
                    return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.Int32DataTemplateKey];
                case "Boolean":
                    return
                        (DataTemplate)
                        Core.SharedResources[SharedResources.DataTemplates.BooleanTrueFalseDataTemplateKey];
                case "String":
                    return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.StringDataTemplateKey];
                case "Color":
                    return (DataTemplate) Core.SharedResources[SharedResources.DataTemplates.ColorDataTemplateKey];
            }
            return base.SelectTemplate(item, container);
        }
    }
}