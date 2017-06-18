// --------------------------------------------------
// AA2Lib - ColorPicker.xaml.cs
// --------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using AA2Lib.Code;
using ReiFX;

namespace AA2Lib.Controls
{
    /// <summary>
    ///     Interaction logic for ColorPicker.xaml
    /// </summary>
    [ContentProperty]
    public partial class ColorPicker : Button, INotifyPropertyChanged
    {
        public static DependencyProperty ColorProperty = DependencyProperty.Register("Color",
            typeof(Color),
            typeof(ColorPicker),
            new PropertyMetadata(default(Color), ColorPropertyChangedCallback));

        private Point _startPoint;

        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void ColorPicker_OnClick(object sender, RoutedEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = Color;
            var b = dialog.ShowDialog();
            if (b.HasValue && b.Value)
                Color = dialog.Color;
        }

        private void CopyCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            //Replaced Lambda to Func/Action
            Dispatcher.Invoke(new Action(() => Clipboard.SetText(Color.ToString())));
            e.Handled = true;
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool hasText = Clipboard.ContainsText();
            if (hasText)
            {
                string text = Clipboard.GetText();
                bool canParse = ColorHelper.CanParseColor(text);
                if (canParse)
                {
                    e.CanExecute = true;
                    e.Handled = true;
                    return;
                }
            }
            e.CanExecute = false;
            e.Handled = true;
        }

        private void PasteCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            bool hasText = Clipboard.ContainsText();
            if (!hasText)
                return;

            string text = Clipboard.GetText();
            Color c;
            bool parsed = ColorHelper.TryParseColor(text, out c);
            if (parsed)
                Color = c;
            e.Handled = true;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string) e.Data.GetData(DataFormats.StringFormat);
                if (ColorHelper.CanParseColor(dataString))
                    e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            string dataString = (string) e.Data.GetData(DataFormats.StringFormat);
            e.Effects = DragDropEffects.Copy;
            Color col;
            ColorHelper.TryParseColor(dataString, out col);
            Color = col;
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            Point point = e.GetPosition(null);
            Vector delta = point - _startPoint;
            if (!(delta.LengthSquared > 25))
                return;
            DataObject data = new DataObject();
            data.SetData(DataFormats.StringFormat, Color.ToString());

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private static void ColorPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ColorPicker cp = dependencyObject as ColorPicker;
            if (cp == null)
                return;
            CommandManager.InvalidateRequerySuggested();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}