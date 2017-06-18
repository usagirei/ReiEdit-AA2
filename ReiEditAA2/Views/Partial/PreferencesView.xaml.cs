// --------------------------------------------------
// ReiEditAA2 - PreferencesView.xaml.cs
// --------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views.Partial
{
    /// <summary>
    ///     Interaction logic for PreferencesView.xaml
    /// </summary>
    internal partial class PreferencesView : UserControl
    {
        public static readonly DependencyProperty CharacterViewModelProperty = DependencyProperty.Register("CharacterViewModel",
            typeof(CharacterViewModel),
            typeof(PreferencesView),
            new PropertyMetadata(default(CharacterViewModel)));

        public CharacterViewModel CharacterViewModel
        {
            get { return (CharacterViewModel) GetValue(CharacterViewModelProperty); }
            set { SetValue(CharacterViewModelProperty, value); }
        }

        public PreferencesView()
        {
            InitializeComponent();
        }
    }
}