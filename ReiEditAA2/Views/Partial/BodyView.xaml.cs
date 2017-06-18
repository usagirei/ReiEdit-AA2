// --------------------------------------------------
// ReiEditAA2 - BodyView.xaml.cs
// --------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReiEditAA2.Code;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views.Partial
{
    internal partial class BodyView : UserControl, IDisposable
    {
        public static readonly DependencyProperty CharacterViewModelProperty = DependencyProperty.Register("CharacterViewModel",
            typeof(CharacterViewModel),
            typeof(BodyView),
            new PropertyMetadata(default(CharacterViewModel), CharacterViewModelChanged));

        public CharacterViewModel CharacterViewModel
        {
            get { return (CharacterViewModel) GetValue(CharacterViewModelProperty); }
            set { SetValue(CharacterViewModelProperty, value); }
        }

        public BodyView()
        {
            InitializeComponent();
        }

        private void GenderOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Value")
                return;
            try
            {
                BindingExpression faceExpr = FaceTypeControl.GetBindingExpression(CharAttributeHelper.AttributeProperty);
                if (faceExpr != null)
                    faceExpr.UpdateTarget();
            }
            catch
            { }

            try
            {
                BindingExpression figureExpr = FigureControl.GetBindingExpression(CharAttributeHelper.AttributeProperty);
                if (figureExpr != null)
                    figureExpr.UpdateTarget();
            }
            catch
            { }
        }

        private static void CharacterViewModelChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            BodyView bv = dependencyObject as BodyView;
            if (bv == null)
                return;

            CharacterViewModel ovm = dependencyPropertyChangedEventArgs.OldValue as CharacterViewModel;
            CharacterViewModel nvm = dependencyPropertyChangedEventArgs.NewValue as CharacterViewModel;

            if (ovm != null && !ovm.IsDisposed)
            {
                ovm.Profile.Gender.PropertyChanged -= bv.GenderOnPropertyChanged;
                BindingOperations.ClearBinding(bv.IrisCombo, ComboBox.TextProperty);
                BindingOperations.ClearBinding(bv.HighlightCombo, ComboBox.TextProperty);
            }
            if (nvm != null)
            {
                nvm.Profile.Gender.PropertyChanged += bv.GenderOnPropertyChanged;
                //Text="{Binding Path=}"
                BindingOperations.SetBinding(bv.HighlightCombo, ComboBox.TextProperty, new Binding("ExternalHighlightTextureName.Value"));
                BindingOperations.SetBinding(bv.IrisCombo, ComboBox.TextProperty, new Binding("ExternalIrisTextureName.Value"));
            }
        }

        public void Dispose()
        {
            CharacterViewModel.Profile.Gender.PropertyChanged -= GenderOnPropertyChanged;
        }
    }
}