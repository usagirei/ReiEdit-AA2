// --------------------------------------------------
// ReiEditAA2 - TextureView.xaml.cs
// --------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views.Partial
{
    /// <summary>
    ///     Interaction logic for TextureView.xaml
    /// </summary>
    internal partial class TextureView : UserControl
    {
        public static readonly DependencyProperty TextureViewModelProperty = DependencyProperty.Register("TextureViewModel",
            typeof(ClothesTextureViewModel),
            typeof(TextureView),
            new PropertyMetadata(null, PropertyChangedCallback));

        public static readonly DependencyProperty ViewHalfShadowProperty =
            DependencyProperty.Register("ViewHalfShadow", typeof(bool), typeof(TextureView), new PropertyMetadata(default(bool)));

        public ClothesTextureViewModel TextureViewModel
        {
            get { return (ClothesTextureViewModel) GetValue(TextureViewModelProperty); }
            set { SetValue(TextureViewModelProperty, value); }
        }

        public bool ViewHalfShadow
        {
            get { return (bool) GetValue(ViewHalfShadowProperty); }
            set { SetValue(ViewHalfShadowProperty, value); }
        }

        public TextureView()
        {
            InitializeComponent();
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            ComboTextures.SelectedIndex = 0;
        }

        private void SuitOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (!CheckAccess())
            {
                //[WXP] Replaced Lambda to new Func/Action
                Dispatcher.Invoke(new Action(() => SuitOnPropertyChanged(sender, propertyChangedEventArgs)));
                return;
            }
            ComboTextures.SelectedIndex = Math.Min((int) TextureViewModel.Texture.Value, ComboTextures.Items.Count - 1);
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            TextureView view = dependencyObject as TextureView;
            ClothesTextureViewModel newViewModel = (ClothesTextureViewModel) args.NewValue;
            ClothesTextureViewModel oldViewModel = (ClothesTextureViewModel) args.OldValue;
            if (view == null)
                return;
            if (oldViewModel != null && !oldViewModel.ClothesViewModel.CharacterViewModel.IsDisposed)
                oldViewModel.Suit.PropertyChanged -= view.SuitOnPropertyChanged;
            if (newViewModel != null)
                newViewModel.Suit.PropertyChanged += view.SuitOnPropertyChanged;
        }
    }
}