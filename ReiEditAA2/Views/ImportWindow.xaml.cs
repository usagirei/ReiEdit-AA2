// --------------------------------------------------
// ReiEditAA2 - ImportWindow.xaml.cs
// --------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AA2Lib.Code;

namespace ReiEditAA2.Views
{
    /// <summary>
    ///     Interaction logic for ImportWindow.xaml
    /// </summary>
    internal partial class ImportWindow : Window
    {
        public static readonly DependencyProperty ImportedProperty = DependencyProperty.Register("Imported",
            typeof(CharacterFile),
            typeof(ImportWindow),
            new PropertyMetadata(default(CharacterFile), PropertyChangedCallback));

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target",
                typeof(CharacterFile),
                typeof(ImportWindow),
                new PropertyMetadata(default(CharacterFile)));

        public CharacterFile Imported
        {
            get { return (CharacterFile) GetValue(ImportedProperty); }
            set { SetValue(ImportedProperty, value); }
        }

        public ICommand MoveCommand { get; set; }

        public CharacterFile Target
        {
            get { return (CharacterFile) GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public ImportWindow()
        {
            InitializeComponent();
            MoveCommand = new RelayCommand<string>(DoMove);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterFile imported = Imported;
            if (PortraitFlag.IsChecked.HasValue && PortraitFlag.IsChecked.Value)
            {
                byte[] newCard, newThumb;
                newCard = new byte[imported.CardBytes.Length];
                newThumb = new byte[imported.ThumbBytes.Length];
                Buffer.BlockCopy(imported.CardBytes, 0, newCard, 0, newCard.Length);
                Buffer.BlockCopy(imported.ThumbBytes, 0, newThumb, 0, newThumb.Length);
                Target.SetCard(newCard);
                Target.SetThumbnail(newThumb);
            }
            foreach (string item in PropBoxB.Items)
            {
                Target[item] = imported[item];
            }
            Close();
        }

        private void MoveAllLeftButton_Click(object sender, RoutedEventArgs e)
        {
            MoveLeft(true);
        }

        private void MoveAllRightButton_Click(object sender, RoutedEventArgs e)
        {
            MoveRight(true);
        }

        private void MoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            MoveLeft(false);
        }

        private void MoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            MoveRight(false);
        }

        private void PropBoxA_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MoveRight(false);
            }
        }

        private void PropBoxB_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Delete)
            {
                MoveLeft(false);
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            SortLists();
        }

        private void DoMove(string key)
        {
            var toMove = PropBoxA.Items.Cast<string>()
                .Where(item => item.StartsWith(key))
                .ToList();

            if (!toMove.Any())
            {
                toMove = PropBoxB.Items.Cast<string>()
                    .Where(item => item.StartsWith(key))
                    .ToList();

                toMove.ForEach
                (s =>
                {
                    PropBoxB.Items.Remove(s);
                    PropBoxA.Items.Add(s);
                });
            }
            else
            {
                toMove.ForEach
                (s =>
                {
                    PropBoxA.Items.Remove(s);
                    PropBoxB.Items.Add(s);
                });
            }
        }

        private void MoveLeft(bool all)
        {
            var toMove = (all
                ? PropBoxB.Items.Cast<string>()
                : PropBoxB.SelectedItems.Cast<string>()).ToList();

            toMove.ForEach
            (s =>
            {
                PropBoxB.Items.Remove(s);
                PropBoxA.Items.Add(s);
            });
        }

        private void MoveRight(bool all)
        {
            var toMove = (all
                ? PropBoxA.Items.Cast<string>()
                : PropBoxA.SelectedItems.Cast<string>()).ToList();

            toMove.ForEach
            (s =>
            {
                PropBoxA.Items.Remove(s);
                PropBoxB.Items.Add(s);
            });
        }

        private void SortLists()
        {
            var newa = PropBoxA.Items.Cast<string>()
                .OrderBy(s => s)
                .ToList();
            var newb = PropBoxB.Items.Cast<string>()
                .OrderBy(s => s)
                .ToList();

            PropBoxA.Items.Clear();
            PropBoxB.Items.Clear();

            newa.ForEach(s => PropBoxA.Items.Add(s));
            newb.ForEach(s => PropBoxB.Items.Add(s));
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ImportWindow self = dependencyObject as ImportWindow;
            if (self == null)
                return;
            CharacterFile cf = dependencyPropertyChangedEventArgs.NewValue as CharacterFile;
            if (cf == null)
                return;
            foreach (string key in cf.CharAttributes.Keys)
            {
                self.PropBoxA.Items.Add(key);
            }
        }
    }
}