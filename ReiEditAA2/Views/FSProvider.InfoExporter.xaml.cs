// --------------------------------------------------
// ReiEditAA2 - FSProvider.InfoExporter.xaml.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using AA2Lib;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.Code.CharacterViewModelProviders;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views
{
    partial class FSProviderTools : Window
    {
        public class InfoExporter
        {
            public event EventHandler PropertyAdded;
            public event EventHandler PropertyChanged;

            private string _delimiter;

            public ICommand AddItemCommand { get; set; }

            public string Delimiter
            {
                get { return Regex.Escape(_delimiter); }
                set { _delimiter = Regex.Unescape(value); }
            }

            public ICommand ExportCommand { get; set; }
            public ICommand MoveDownCommand { get; set; }
            public ICommand MoveUpCommand { get; set; }
            public bool ParseEnum { get; set; }
            public List<string> Properties { get; private set; }
            public FileSystemCharacterViewModelProvider Provider { get; private set; }
            public ICommand RemoveItemCommand { get; set; }
            public List<string> ValidProperties { get; private set; }

            public InfoExporter(IEnumerable<string> validProps, FileSystemCharacterViewModelProvider provider)
            {
                ValidProperties = new List<string>
                {
                    "$",
                };
                ValidProperties.AddRange(validProps);

                Provider = provider;

                Properties = new List<string>
                {
                    "$"
                };

                Delimiter = "\t";

                AddItemCommand = new RelayCommand<string>(AddItem_OnExecute);
                MoveUpCommand = new RelayCommand<string>(MoveUp_OnExecute);
                MoveDownCommand = new RelayCommand<string>(MoveDown_OnExecute);
                RemoveItemCommand = new RelayCommand<string>(RemoveItem_OnExecute, RemoveItem_CanExecute);

                ExportCommand = new RelayCommand(Export_OnExecute, Export_CanExecute);
            }

            protected virtual void OnPropertyAdded()
            {
                EventHandler handler = PropertyAdded;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }

            protected virtual void OnPropertyChanged()
            {
                EventHandler handler = PropertyChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }

            private void AddItem_OnExecute(string key)
            {
                if (Properties.Contains(key) || !KeyIsValid(key))
                    return;

                Properties.Add(key);
                OnPropertyAdded();
                OnPropertyChanged();
            }

            private bool Export_CanExecute()
            {
                return Properties.Any();
            }

            private void Export_OnExecute()
            {
                StringBuilder sb = new StringBuilder();

                var chars = Provider.GetCharacters();

                sb.AppendLine(string.Join(_delimiter, Properties));

                chars.ForEach(model => sb.AppendLine(string.Join(_delimiter, GetPropertyString(model))));

                string temp = Path.Combine(Core.StartupPath, "preview.txt");
                TextWriter tw = new StreamWriter(temp, false);
                tw.Write(sb.ToString());
                tw.Close();
                Process.Start(temp);
            }

            private string[] GetPropertyString(CharacterViewModel model)
            {
                string oldPath = model.Metadata as string;
                string oldName = Path.GetFileNameWithoutExtension(oldPath);
                return Properties.Select
                    (s =>
                    {
                        if (s.Equals("$"))
                            return oldName;
                        DataBlockWrapper db = model.Character.CharAttributes[s];
                        return ParseEnum && db.IsEnum
                            ? db.EnumValue
                            : db.Value.ToString();
                    })
                    .ToArray();
            }

            private bool KeyIsValid(string key)
            {
                return ValidProperties.Contains(key);
            }

            private void MoveDown_OnExecute(string s)
            {
                int index = Properties.IndexOf(s);
                if (index >= Properties.Count - 1)
                    return;
                string temp = Properties[index + 1];
                Properties[index + 1] = Properties[index];
                Properties[index] = temp;
                OnPropertyChanged();
            }

            private void MoveUp_OnExecute(string s)
            {
                int index = Properties.IndexOf(s);
                if (index <= 0)
                    return;
                string temp = Properties[index - 1];
                Properties[index - 1] = Properties[index];
                Properties[index] = temp;
                OnPropertyChanged();
            }

            private bool RemoveItem_CanExecute(string s)
            {
                return Properties.Contains(s);
            }

            private void RemoveItem_OnExecute(string s)
            {
                Properties.Remove(s);
                OnPropertyChanged();
            }
        }
    }
}