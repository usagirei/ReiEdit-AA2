// --------------------------------------------------
// ReiEditAA2 - HeaderEditor.xaml.cs
// --------------------------------------------------

using System.Windows;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Views
{
    /// <summary>
    ///     Interaction logic for HeaderEditor.xaml
    /// </summary>
    internal partial class HeaderEditor : Window
    {
        public static readonly DependencyProperty SaveHeaderViewModelProperty = DependencyProperty.Register("SaveHeaderViewModel",
            typeof(SaveHeaderViewModel),
            typeof(HeaderEditor),
            new PropertyMetadata(default(SaveHeaderViewModel)));

        public DataBlockWrapper PlayerSeatBlock { get; set; }

        public SaveHeaderViewModel SaveHeaderViewModel
        {
            get { return (SaveHeaderViewModel) GetValue(SaveHeaderViewModelProperty); }
            set { SetValue(SaveHeaderViewModelProperty, value); }
        }

        public HeaderEditor()
        {
            InitializeComponent();
        }
    }
}