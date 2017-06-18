// --------------------------------------------------
// ReiEditAA2 - ProfileView.xaml.cs
// --------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views.Partial
{
    /// <summary>
    ///     Interaction logic for TraitSelector.xaml
    /// </summary>
    internal partial class ProfileView : UserControl
    {
        public static readonly DependencyProperty CharacterViewModelProperty = DependencyProperty.Register("CharacterViewModel",
            typeof(CharacterViewModel),
            typeof(ProfileView),
            new PropertyMetadata(default(CharacterViewModel)));

        public CharacterViewModel CharacterViewModel
        {
            get { return (CharacterViewModel) GetValue(CharacterViewModelProperty); }
            set { SetValue(CharacterViewModelProperty, value); }
        }

        public ProfileView()
        {
            InitializeComponent();
        }
    }
}