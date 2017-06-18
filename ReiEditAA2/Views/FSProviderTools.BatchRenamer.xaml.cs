// --------------------------------------------------
// ReiEditAA2 - FSProviderTools.BatchRenamer.xaml.cs
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
        public class BatchRenamer
        {
            public event EventHandler PropertyAdded;
            public event EventHandler PropertyChanged;

            public ICommand AddItemCommand { get; set; }
            public string Format { get; set; }
            public ICommand MoveDownCommand { get; set; }
            public ICommand MoveUpCommand { get; set; }
            public bool ParseEnum { get; set; }
            public ICommand PreviewCommand { get; set; }
            public List<string> Properties { get; private set; }
            public FileSystemCharacterViewModelProvider Provider { get; private set; }
            public ICommand RemoveItemCommand { get; set; }
            public ICommand RenameCommand { get; set; }
            public List<string> ValidProperties { get; private set; }

            public BatchRenamer(IEnumerable<string> validProps, FileSystemCharacterViewModelProvider provider)
            {
                ValidProperties = new List<string>
                {
                    "$",
                    "#"
                };
                ValidProperties.AddRange(validProps);

                Provider = provider;

                Properties = new List<string>
                {
                    "$"
                };

                Format = "{0}";

                AddItemCommand = new RelayCommand<string>(AddItem_OnExecute);
                MoveUpCommand = new RelayCommand<string>(MoveUp_OnExecute);
                MoveDownCommand = new RelayCommand<string>(MoveDown_OnExecute);
                RemoveItemCommand = new RelayCommand<string>(RemoveItem_OnExecute, RemoveItem_CanExecute);

                PreviewCommand = new RelayCommand(Preview_OnExecute, Rename_CanExecute);
                RenameCommand = new RelayCommand(Rename_OnExecute, Rename_CanExecute);
            }

            public IEnumerable<RenameInfo> GetNewNames(IEnumerable<CharacterViewModel> chars)
            {
                int counter = 1;
                string regexSearch = new string(Path.GetInvalidFileNameChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)), RegexOptions.Compiled);
                foreach (CharacterViewModel model in chars)
                {
                    string oldPath = model.Metadata as string;
                    string oldName = Path.GetFileNameWithoutExtension(oldPath);
                    string dir = Path.GetDirectoryName(oldPath);
                    string ext = Path.GetExtension(oldPath);

                    var formatParameters = Properties.Select
                        (s =>
                        {
                            if (s.Equals("$"))
                                return oldName;
                            if (s.Equals("#"))
                                return counter;
                            DataBlockWrapper db = model.Character.CharAttributes[s];
                            return db.IsEnum && ParseEnum
                                ? db.EnumValue
                                : db.Value;
                        })
                        .ToArray();

                    counter++;
                    string newName = string.Format(Format, formatParameters);
                    newName = r.Replace(newName, "");

                    yield return new RenameInfo
                    {
                        Path = dir,
                        Extension = ext,
                        OldName = oldName,
                        NewName = newName
                    };
                }
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

            private void Preview_OnExecute()
            {
                StringBuilder sb = new StringBuilder();

                var chars = Provider.GetCharacters();
                var newNames = GetNewNames(chars);

                newNames.ForEach
                (info =>
                {
                    sb.AppendFormat("\r\nOld:\t{0}\r\n", info.OldName);
                    sb.AppendFormat("New:\t{0}\r\n", info.NewName);
                });

                string temp = Path.Combine(Core.StartupPath, "preview.txt");
                TextWriter tw = new StreamWriter(temp, false);
                tw.Write(sb.ToString());
                tw.Close();
                Process.Start(temp);
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

            private bool Rename_CanExecute()
            {
                try
                {
                    var args = Enumerable.Repeat("", Properties.Count)
                        .Cast<object>()
                        .ToArray();
                    string.Format(Format, args);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private void Rename_OnExecute()
            {
                var chars = Provider.GetCharacters();
                var newNames = GetNewNames(chars);

                newNames.ForEach
                (info =>
                {
                    string oldPath = Path.Combine(info.Path, info.OldName + info.Extension);
                    string newPath = Path.Combine(info.Path, info.NewName + info.Extension);
                    if (oldPath.Equals(newPath))
                        return;
                    FileInfo fi = new FileInfo(oldPath);
                    if (!fi.Exists)
                        return;

                    if (File.Exists(newPath))
                    {
                        int cnt = 2;
                        do
                        {
                            newPath = Path.Combine(info.Path, info.NewName + "_" + (cnt++) + info.Extension);
                        }
                        while (File.Exists(newPath));
                    }
                    fi.MoveTo(newPath);
                });
            }

            public struct RenameInfo
            {
                public string Extension { get; set; }
                public string NewName { get; set; }
                public string OldName { get; set; }
                public string Path { get; set; }
            }
        }
    }
}