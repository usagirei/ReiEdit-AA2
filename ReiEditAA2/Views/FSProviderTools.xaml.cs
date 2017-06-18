// --------------------------------------------------
// ReiEditAA2 - FSProviderTools.xaml.cs
// --------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using AA2Lib;
using ReiEditAA2.Code.CharacterViewModelProviders;

namespace ReiEditAA2.Views
{
    /// <summary>
    ///     Interaction logic for FSProviderTools.xaml
    /// </summary>
    internal partial class FSProviderTools : Window
    {
        public InfoExporter Exporter { get; private set; }
        public FileSystemCharacterViewModelProvider Provider { get; private set; }
        public BatchRenamer Renamer { get; private set; }

        public FSProviderTools(FileSystemCharacterViewModelProvider provider)
        {
            InitializeComponent();
            XDocument xdoc = Core.LoadCharDataXDocument();
            var elements = xdoc.Element("dataset")
                .Element("datablocks")
                .Elements("datablock");
            var props = elements.Select
                (element => element.Attribute("key")
                    .Value)
                .ToList();

            Provider = provider;

            Renamer = new BatchRenamer(props, Provider);
            Renamer.PropertyAdded += (sender, args) =>
            {
                RenameAddTextBox.SelectedItem = null;
                RenameAddTextBox.Focus();
            };
            Renamer.PropertyChanged += (sender, args) => RenamePropsBox.Items.Refresh();

            Exporter = new InfoExporter(props, Provider);
            Exporter.PropertyAdded += (sender, args) =>
            {
                ExportAddTextBox.SelectedItem = null;
                ExportAddTextBox.Focus();
            };
            Exporter.PropertyChanged += (sender, args) => ExportPropsBox.Items.Refresh();
        }

        private void ExportAddTextBox_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter)
                return;
            string key = (string) ExportAddTextBox.SelectedItem;
            if (Exporter.AddItemCommand.CanExecute(key))
                Exporter.AddItemCommand.Execute(key);
        }

        private void ExportBox_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            ListBox renamePropsBox = sender as ListBox;
            if (renamePropsBox == null)
                return;

            string key = renamePropsBox.SelectedItem == null
                ? String.Empty
                : (string) renamePropsBox.SelectedItem;

            if (e.Key == Key.Delete)
            {
                if (Exporter.RemoveItemCommand.CanExecute(key))
                    Exporter.RemoveItemCommand.Execute(key);
                return;
            }

            if (e.Key != Key.System || Keyboard.Modifiers != ModifierKeys.Alt)
                return;

            switch (e.SystemKey)
            {
                case Key.Up:
                    if (Exporter.MoveUpCommand.CanExecute(key))
                        Exporter.MoveUpCommand.Execute(key);
                    return;
                case Key.Down:
                    if (Exporter.MoveDownCommand.CanExecute(key))
                        Exporter.MoveDownCommand.Execute(key);
                    return;
            }
        }

        private void RenameBox_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            ListBox renamePropsBox = sender as ListBox;
            if (renamePropsBox == null)
                return;

            string key = renamePropsBox.SelectedItem == null
                ? String.Empty
                : (string) renamePropsBox.SelectedItem;

            if (e.Key == Key.Delete)
            {
                if (Renamer.RemoveItemCommand.CanExecute(key))
                    Renamer.RemoveItemCommand.Execute(key);
                return;
            }

            if (e.Key != Key.System || Keyboard.Modifiers != ModifierKeys.Alt)
                return;

            switch (e.SystemKey)
            {
                case Key.Up:
                    if (Renamer.MoveUpCommand.CanExecute(key))
                        Renamer.MoveUpCommand.Execute(key);
                    return;
                case Key.Down:
                    if (Renamer.MoveDownCommand.CanExecute(key))
                        Renamer.MoveDownCommand.Execute(key);
                    return;
            }
        }

        private void RenameAddTextBox_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter)
                return;
            string key = (string) RenameAddTextBox.SelectedItem;
            if (Renamer.AddItemCommand.CanExecute(key))
                Renamer.AddItemCommand.Execute(key);
        }
    }
}