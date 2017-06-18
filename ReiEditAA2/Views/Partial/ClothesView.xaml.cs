// --------------------------------------------------
// ReiEditAA2 - ClothesView.xaml.cs
// --------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views.Partial
{
    /// <summary>
    ///     Interaction logic for ClothesView.xaml
    /// </summary>
    internal partial class ClothesView : UserControl
    {
        public static readonly DependencyProperty CharacterViewModelProperty = DependencyProperty.Register("CharacterViewModel",
            typeof(CharacterViewModel),
            typeof(ClothesView),
            new PropertyMetadata(default(CharacterViewModel)));

        public CharacterViewModel CharacterViewModel
        {
            get { return (CharacterViewModel) GetValue(CharacterViewModelProperty); }
            set { SetValue(CharacterViewModelProperty, value); }
        }

        public ClothesView()
        {
            InitializeComponent();
        }
    }
}