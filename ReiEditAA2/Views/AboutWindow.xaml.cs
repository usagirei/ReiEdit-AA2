// --------------------------------------------------
// ReiEditAA2 - AboutWindow.xaml.cs
// --------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;

namespace ReiEditAA2.Views
{
    /// <summary>
    ///     Interaction logic for AboutWindow.xaml
    /// </summary>
    internal partial class AboutWindow : Window
    {
        public static readonly DependencyProperty EnableClosingProperty =
            DependencyProperty.Register("EnableClosing", typeof(bool), typeof(AboutWindow), new PropertyMetadata(default(bool)));

        private bool _enableClosing;

        public bool EnableClosing
        {
            get { return (bool) GetValue(EnableClosingProperty); }
            set { SetValue(EnableClosingProperty, value); }
        }

        public AboutWindow()
        {
            InitializeComponent();
            EnableClosing = true;
            InfoPanel.DataContext = new ApplicationInfo();
            StreamResourceInfo info = Application.GetResourceStream(new Uri("Resources/About.txt", UriKind.Relative));
            StreamReader reader = new StreamReader(info.Stream, Encoding.UTF8);
            ExtraText.Text = reader.ReadToEnd();
            reader.Dispose();
        }

        private void AboutWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !EnableClosing;
        }

        private void AboutWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public class ApplicationInfo : INotifyPropertyChanged
        {
            private Assembly _assembly;
            private FileVersionInfo _fileInfo;

            public Assembly Assembly
            {
                get { return _assembly; }
                private set
                {
                    _assembly = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("AssemblyName");
                }
            }

            public AssemblyName AssemblyName
            {
                get { return _assembly.GetName(); }
            }

            public FileVersionInfo FileInfo
            {
                get { return _fileInfo; }
                set
                {
                    _fileInfo = value;
                    RaisePropertyChanged();
                }
            }

            public ApplicationInfo()
            {
                Assembly = Assembly.GetExecutingAssembly();
                FileInfo = FileVersionInfo.GetVersionInfo(_assembly.Location);
            }

            //[WXP] CallerMemberName Removed

            protected virtual void RaisePropertyChanged(string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}