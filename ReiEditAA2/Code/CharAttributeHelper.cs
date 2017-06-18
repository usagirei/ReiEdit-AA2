// --------------------------------------------------
// ReiEditAA2 - CharAttributeHelper.cs
// --------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.Code
{
    internal class CharAttributeHelper : DependencyObject
    {
        public static readonly DependencyProperty AttributeProperty = DependencyProperty.RegisterAttached("Attribute",
            typeof(string),
            typeof(CharAttributeHelper),
            new PropertyMetadata(default(string), PropertyChangedCallback));

        public static readonly DependencyProperty CharacterProperty = DependencyProperty.RegisterAttached("Character",
            typeof(CharacterFile),
            typeof(CharAttributeHelper),
            new PropertyMetadata(default(CharacterFile), PropertyChangedCallback));

        public static readonly DependencyProperty DataTemplateProperty = DependencyProperty.RegisterAttached("DataTemplate",
            typeof(DataTemplate),
            typeof(CharAttributeHelper),
            new PropertyMetadata(default(DataTemplate), PropertyChangedCallback));

        private static readonly DataTemplateSelector TemplateSelector = new DataBlockTemplateSelector();

        public static string GetAttribute(DependencyObject element)
        {
            return (string) element.GetValue(AttributeProperty);
        }

        public static CharacterFile GetCharacter(DependencyObject element)
        {
            return (CharacterFile) element.GetValue(CharacterProperty);
        }

        public static DataTemplate GetDataTemplate(DependencyObject element)
        {
            return (DataTemplate) element.GetValue(DataTemplateProperty);
        }

        public static void SetAttribute(DependencyObject element, string value)
        {
            element.SetValue(AttributeProperty, value);
        }

        public static void SetCharacter(DependencyObject element, CharacterFile value)
        {
            element.SetValue(CharacterProperty, value);
        }

        public static void SetDataTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(DataTemplateProperty, value);
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ContentControl control = dependencyObject as ContentControl;
            if (control == null)
                return;
            DataTemplate overrideTemplate = GetDataTemplate(control);
            CharacterFile cf = GetCharacter(control);
            string at = GetAttribute(control);
            if (cf == null || string.IsNullOrEmpty(at) || !cf.CharAttributes.ContainsKey(at))
                return;
            DataBlockWrapper attribute = cf.CharAttributes[at];
            control.SetBinding(ContentControl.ContentProperty,
                new Binding
                {
                    Source = attribute,
                });
            BindingOperations.ClearBinding(control, ContentControl.ContentTemplateSelectorProperty);
            BindingOperations.ClearBinding(control, ContentControl.ContentTemplateProperty);
            if (overrideTemplate == null)
            {
                control.SetBinding(ContentControl.ContentTemplateSelectorProperty,
                    new Binding
                    {
                        Source = TemplateSelector
                    });
            }
            else
            {
                control.SetBinding(ContentControl.ContentTemplateProperty,
                    new Binding
                    {
                        Source = overrideTemplate
                    });
            }
        }
    }
}