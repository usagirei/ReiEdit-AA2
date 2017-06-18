// --------------------------------------------------
// ReiEditAA2 - UniformView.xaml.cs
// --------------------------------------------------

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AA2Lib;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using Microsoft.Win32;
using ReiEditAA2.Code;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views.Partial
{
    /// <summary>
    ///     Interaction logic for UniformView.xaml
    /// </summary>
    internal partial class UniformView : UserControl
    {
        public static readonly DependencyProperty ClothesViewModelProperty = DependencyProperty.Register("ClothesViewModel",
            typeof(ClothesViewModel),
            typeof(UniformView),
            new PropertyMetadata(default(ClothesViewModel), ClothesViewModelChanged));

        public ClothesViewModel ClothesViewModel
        {
            get { return (ClothesViewModel) GetValue(ClothesViewModelProperty); }
            set { SetValue(ClothesViewModelProperty, value); }
        }

        public UniformView()
        {
            InitializeComponent();
        }

        private void CopyCommand_Execute(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            CharacterFile @char = ClothesViewModel.CharacterViewModel.Character;
            string tgt = ClipboardHelper.GetSuitPrefix(ClothesViewModel.Kind);

            var clothBytes = new byte[92];
            ClothFile cf = ClothFile.Load(clothBytes);

            var attribs = cf.Attributes.Keys.Select
                (s =>
                {
                    string tgtKey = s.Replace("CLOTH", tgt);
                    if (@char.CharAttributes.ContainsKey(tgtKey))
                        return @char.CharAttributes[tgtKey];
                    return null;
                })
                .Where(wrapper => wrapper != null);
            String str = ClipboardHelper.GetAttributesString(attribs, ClipboardHelper.REIAA2_SUITFLAG);

            //[WXP] Replaced Lambda to new Func/Action
            Dispatcher.Invoke
            (new Action
            (() =>
            {
                string clothesPrefix = ClipboardHelper.GetSuitPrefix(ClothesViewModel.Kind);
                Clipboard.SetText(str.Replace(clothesPrefix, "CLOTH"));
            }));
        }

        private void Correct_OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            int suitVal = (int) ClothesViewModel.Suit.Value;
            string suitStr = ClipboardHelper.GetSuitPrefix(ClothesViewModel.Kind);

            string[] kData = {"IS_ONEPIECE", "IS_UNDERWEAR", "IS_SKIRT"};
            bool[][] bData =
            {
                //Uniform
                new[] {false, true, true},
                //Sport
                new[] {false, true, false},
                //Swimsuit
                new[] {true, false, false},
                //Formal
                new[] {false, true, true},
                //Blazer
                new[] {false, true, true},
                //Naked
                new[] {false, false, false},
                //Undies
                new[] {false, true, false},
                //Uniform2
                new[] {false, true, true}
            };

            CharacterFile @char = ClothesViewModel.CharacterViewModel.Character;
            for (int i = 0; i < 3; i++)
            {
                string targetKey = String.Format("{0}_{1}", suitStr, kData[i]);
                bool targetValue;
                try
                {
                    targetValue = bData[suitVal][i];
                }
                catch
                {
                    targetValue = false;
                }
                if (@char.CharAttributes.ContainsKey(targetKey))
                    @char.CharAttributes[targetKey].Value = targetValue;
            }
        }

        private void GenderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                return;
            try
            {
                BindingExpression socksExpr = SocksControl.GetBindingExpression(CharAttributeHelper.AttributeProperty);
                if (socksExpr != null)
                    socksExpr.UpdateTarget();
            }
            catch
            { }

            try
            {
                BindingExpression suitExpr = SuitControl.GetBindingExpression(CharAttributeHelper.AttributeProperty);
                if (suitExpr != null)
                    suitExpr.UpdateTarget();
            }
            catch
            { }

            try
            {
                BindingExpression inshoExpr = InShoesControl.GetBindingExpression(CharAttributeHelper.AttributeProperty);
                if (inshoExpr != null)
                    inshoExpr.UpdateTarget();
            }
            catch
            { }

            try
            {
                BindingExpression oushoExpr = OutShoesControl.GetBindingExpression
                    (CharAttributeHelper.AttributeProperty);
                if (oushoExpr != null)
                    oushoExpr.UpdateTarget();
            }
            catch
            { }
        }

        private void LoadCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            string clothPath = Path.Combine(Core.PlaySaveDir, "Cloth");

            OpenFileDialog opfd = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                Filter = "Cloth Files (*.cloth)|*.cloth",
                InitialDirectory = clothPath
            };
            opfd.CustomPlaces.Add(new FileDialogCustomPlace(clothPath));
            if (!opfd.ShowDialog(Window.GetWindow(this))
                .Value)
                return;

            var data = File.ReadAllBytes(opfd.FileName);
            ClothFile cf = ClothFile.Load(data);

            ApplyClothFile(cf);
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClipboardHelper.CanPasteAttributeString(ClipboardHelper.REIAA2_SUITFLAG);
            e.Handled = true;
        }

        private void PasteCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            String input = Clipboard.GetText();
            string clothesPrefix = ClipboardHelper.GetSuitPrefix(ClothesViewModel.Kind);

            ClipboardHelper.SetAttributesString(input.Replace("CLOTH", clothesPrefix), ClothesViewModel.CharacterViewModel.Character);
        }

        private void SaveCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            string clothPath = Path.Combine(Core.PlaySaveDir, "Cloth");
            SaveFileDialog svfd = new SaveFileDialog
            {
                AddExtension = true,
                Filter = "Cloth Files (*.cloth)|*.cloth",
                InitialDirectory = clothPath,
                FileName = ClothesViewModel.CharacterViewModel.Profile.Gender.Value.Equals((byte) 0)
                    ? "M_"
                    : "F_"
            };
            svfd.CustomPlaces.Add(new FileDialogCustomPlace(clothPath));
            if (!svfd.ShowDialog(Window.GetWindow(this))
                .Value)
                return;

            ClothFile cf = CreateClothFile();

            FileStream fs = File.OpenWrite(svfd.FileName);
            fs.Write(cf.RawData, 0, cf.RawData.Length);
            fs.Close();
        }

        private void ApplyClothFile(ClothFile cf)
        {
            string[] targets = {"UNIFORM", "SWIMSUIT", "SPORT", "CLUB"};
            int suit = (int) ClothesViewModel.Kind;
            CharacterFile @char = ClothesViewModel.CharacterViewModel.Character;
            foreach (DataBlockWrapper wrapperCloth in cf.Attributes.Values)
            {
                string tgtKey = wrapperCloth.Key.Replace("CLOTH", targets[suit]);
                if (!@char.CharAttributes.ContainsKey(tgtKey))
                    continue;
                DataBlockWrapper wrapperChar = @char.CharAttributes[tgtKey];
                if (wrapperChar.ReadOnly)
                    continue;

                wrapperChar.Value = wrapperCloth.Value;
            }
        }

        private ClothFile CreateClothFile()
        {
            string[] targets = {"UNIFORM", "SWIMSUIT", "SPORT", "CLUB"};
            int suit = (int) ClothesViewModel.Kind;
            CharacterFile @char = ClothesViewModel.CharacterViewModel.Character;
            var attribs = @char.CharAttributes.Where(pair => pair.Key.StartsWith(targets[suit]))
                .Select(pair => pair.Value);
            ClothFile cf = new ClothFile
            {
                RawData = new byte[92],
            };
            cf.Attributes["CLOTH_GENDER"].Value = ClothesViewModel.CharacterViewModel.Profile.Gender.Value;
            foreach (DataBlockWrapper wrapperChar in attribs)
            {
                string tgtKey = wrapperChar.Key.Replace(targets[suit], "CLOTH");
                if (!cf.Attributes.ContainsKey(tgtKey))
                    continue;
                DataBlockWrapper wrapperCloth = cf.Attributes[tgtKey];
                wrapperCloth.Value = wrapperChar.Value;
            }
            return cf;
        }

        private static void ClothesViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformView uv = d as UniformView;
            if (uv == null)
                return;

            ClothesViewModel ovm = e.OldValue as ClothesViewModel;
            ClothesViewModel nvm = e.NewValue as ClothesViewModel;

            if (ovm != null && !ovm.CharacterViewModel.IsDisposed)
            {
                ovm.CharacterViewModel.Profile.Gender.PropertyChanged -= uv.GenderOnPropertyChanged;
            }
            if (nvm != null)
            {
                nvm.CharacterViewModel.Profile.Gender.PropertyChanged += uv.GenderOnPropertyChanged;
            }
        }
    }
}