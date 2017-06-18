// --------------------------------------------------
// ReiEditAA2 - App.xaml.cs
// --------------------------------------------------

using System;
using System.Windows;
using System.Windows.Threading;
using AA2Lib.Code.ConfigurationManager;
using ReiEditAA2.Views;

namespace ReiEditAA2
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#if !DEBUG
        public App()
        {

            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            CrashWindow wnd = new CrashWindow(args.Exception);
            wnd.ShowDialog();
            Environment.Exit(0);
        }
#endif

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigurationExtension.LoadConfigs();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ConfigurationExtension.SaveConfigs();
        }
    }
}