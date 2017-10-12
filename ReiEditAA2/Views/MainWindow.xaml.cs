// --------------------------------------------------
// ReiEditAA2 - MainWindow.xaml.cs
// --------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using AA2Lib;
using AA2Lib.Code;
using AA2Lib.Code.ConfigurationManager;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.Code;
using ReiEditAA2.Code.CharacterViewModelProviders;
using ReiEditAA2.Code.Modifiers;
using ReiEditAA2.Plugins;
using ReiEditAA2.ViewModels;
using Binding = System.Windows.Data.Binding;
using Clipboard = System.Windows.Clipboard;
using DataGrid = System.Windows.Controls.DataGrid;
using FileDialogCustomPlace = Microsoft.Win32.FileDialogCustomPlace;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Timer = System.Threading.Timer;

namespace ReiEditAA2.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private const string MESSAGE_ERROR_FINISH_PROVIDER = "Current CharacterProvider is busy, press OK to try again";

        private readonly ICommand _loadCustomDirCommand;

        private readonly ICommand _loadEditorSaveCommand;
        private readonly ICommand _configPathsCommand;
        private readonly ICommand _loadGameSaveCommand;
        private readonly ICommand _loadPluginsCommand;
        private readonly ICommand _reloadAllCommand;
        private readonly ICommand _saveAllCommand;
        private readonly ICommand _showAboutCommand;
        private AppDomain PluginsDomain;

        public ObservableCollection<PluginMenu> DynamicPlugins { get; set; }

        public ICommand LoadCustomDirectoryCommand
        {
            get { return _loadCustomDirCommand; }
        }

        public ICommand LoadEditorSaveCommand
        {
            get { return _loadEditorSaveCommand; }
        }

        public ICommand LoadGameSaveCommand
        {
            get { return _loadGameSaveCommand; }
        }

        public ICommand ConfigPathsCommand
        {
            get { return _configPathsCommand; }
        }

        public ICommand ReloadAllCommand
        {
            get { return _reloadAllCommand; }
        }

        public ICommand ReloadPluginsCommand
        {
            get { return _loadPluginsCommand; }
        }

        public ICommand SaveAllCommand
        {
            get { return _saveAllCommand; }
        }

        public ICommand ShowAboutCommand
        {
            get { return _showAboutCommand; }
        }

        public ObservableCollection<CharModifierBase> Tools { get; set; }
        private Window CardViewWindow { get; set; }

        public MainWindow()
        {
            Tools = new ObservableCollection<CharModifierBase>();
            DynamicPlugins = new ObservableCollection<PluginMenu>();

            _loadEditorSaveCommand = new RelayCommand<bool>(LoadEditorSave_Execute);
            _configPathsCommand = new RelayCommand<bool>(ConfigPaths_Execute);
            _loadGameSaveCommand = new RelayCommand(LoadGameSave_Execute);
            _loadCustomDirCommand = new RelayCommand<bool>(LoadCustomDir_Execute);
            _showAboutCommand = new RelayCommand(ShowAboutMenu_Execute);
            _saveAllCommand = new RelayCommand(SaveAll_Execute, SaveReloadAll_CanExecute);
            _reloadAllCommand = new RelayCommand(ReloadAll_Execute, SaveReloadAll_CanExecute);
            _loadPluginsCommand = new RelayCommand(LoadExternalPlugins_Execute);

            InitializeComponent();
            CardViewWindow = new Window
            {
                DataContext = CharactersControl,
                Title = "Card Viewer",
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow,
                ResizeMode = ResizeMode.NoResize,
                Content = new Image
                {
                    Width = 200,
                    Height = 300,
                },
                SizeToContent = SizeToContent.WidthAndHeight,
            };
            CardViewWindow.Closing += CardViewWindow_OnClosing;
            ((Image) CardViewWindow.Content).SetBinding(Image.SourceProperty, new Binding("SelectedValue.CardImage"));
        }

        private void CardViewWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            CardViewWindow.Hide();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DataGridCopyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg == null)
                e.CanExecute = false;
            else
                e.CanExecute = dg.SelectedItems.Count > 0;
            e.Handled = true;
        }

        private void DataGridCopyCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg == null)
                return;
            SetClipboardText
                (ClipboardHelper.GetAttributesString(dg.SelectedItems.Cast<DataBlockWrapper>(), ClipboardHelper.REIAA2_ATTRFLAG));
        }

        private void DataGridPasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg == null)
                return;
            CharacterViewModel model = dg.DataContext as CharacterViewModel;
            e.CanExecute = model != null && ClipboardHelper.CanPasteAttributeString(ClipboardHelper.REIAA2_ATTRFLAG);
            e.Handled = true;
        }

        private void DataGridPasteCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg == null)
                return;
            CharacterViewModel model = dg.DataContext as CharacterViewModel;
            if (model == null)
                return;
            ClipboardHelper.SetAttributesString(Clipboard.GetText(), model.Character);
            e.Handled = true;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadPlugins(Assembly.GetExecutingAssembly());

            CompileExternalPlugins();
            LoadExternalPlugins();

            const string sFirstRun = "FIRST_RUN";
            ConfigurationManager mng = ConfigurationManager.Instance;
            bool bFirstRun = mng[sFirstRun] == null || mng[sFirstRun].Value.Equals(true);
            if (!bFirstRun)
                return;

            AboutWindow wnd = new AboutWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                EnableClosing = false
            };

            Delay(2500)
                .ContinueWith
                (t =>
                {
                    if (mng[sFirstRun] == null)
                        mng.Configs.Add(sFirstRun, new ConfigurationItem(sFirstRun, false));
                    else
                        mng[sFirstRun].Value = false;
                    //[WXP] Replaced Lambda to new Func/Action
                    Dispatcher.Invoke(new Func<bool>(() => wnd.EnableClosing = true));
                });
            wnd.ShowDialog();
        }

        private void ViewCard_OnClick(object sender, RoutedEventArgs e)
        {
            CardViewWindow.Owner = this;
            CardViewWindow.Show();
        }

        private void ViewModelProviderOnCharacterLoaded(object o, CharacterLoadedDelegateArgs args)
        {
            SetTitle(string.Format("Loading: [{0}/{1}] {2}", args.Current, args.NumChars, args.Message));
            //throw new NotImplementedException();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CharacterCollection collection = DataContext as CharacterCollection;
            if (collection == null)
                return;
            while (!collection.ViewModelProvider.Finish())
            {
                MessageBoxResult msg = MessageBox.Show(MESSAGE_ERROR_FINISH_PROVIDER,
                    "Error",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Hand);
                if (msg == MessageBoxResult.Cancel)
                    return;
            }
            collection.ViewModelProvider.Dispose();
        }

        public void CompileExternalPlugins()
        {
            string pluginsDir = Path.Combine(Core.StartupPath, "Plugins");

            AppDomain compilerDomain = AppDomain.CreateDomain("compiler");
            PluginLoader compiler =
                (PluginLoader) compilerDomain.CreateInstanceAndUnwrap("ReiEditAA2", "ReiEditAA2.Plugins.PluginLoader");
            foreach (string file in Directory.EnumerateFiles(pluginsDir, "*.cs"))
            {
                bool compiled = compiler.CompilePlugins(file);
                if (!compiled)
                {
                    MessageBox.Show(string.Format("Compilation Errors on '{0}', Check DynamicPlugin.txt for more information",
                            Path.GetFileName(file)),
                        "Compilation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
            }
            AppDomain.Unload(compilerDomain);
        }

        public void LoadExternalPlugins()
        {
            string pluginsDir = Path.Combine(Core.StartupPath, "Plugins", "bin");

            PluginsDomain = AppDomain.CreateDomain("plugins");

            PluginLoader loader =
                (PluginLoader) PluginsDomain.CreateInstanceAndUnwrap("ReiEditAA2", "ReiEditAA2.Plugins.PluginLoader");

            DynamicPlugins.Clear();
            foreach (string file in Directory.EnumerateFiles(pluginsDir, "*.dll"))
            {
                var plugins = loader.LoadPlugins(file);
                plugins.Select(plugin => new PluginMenu(plugin))
                    .ForEach(modifier => DynamicPlugins.Add(modifier));
            }
        }

        public void LoadExternalPlugins_Execute()
        {
            UnloadPlugins();
            CompileExternalPlugins();
            LoadExternalPlugins();
            MessageBox.Show("Plugins Reloaded");
        }

        public void LoadPlugins(Assembly assembly)
        {
            var tools = (from t in Assembly.GetExecutingAssembly()
                        .GetTypes()
                    where !t.IsAbstract && t.IsClass
                    where t.Name != "PluginModifier"
                    where typeof(CharModifierBase).IsAssignableFrom(t)
                    select Activator.CreateInstance(t)).Cast<CharModifierBase>()
                .OrderBy(@base => @base.Name);
            tools.ForEach(@base => Tools.Add(@base));
        }

        public void SetClipboardText(string s)
        {
            if (!CheckAccess())
            {
                //[WXP] Replaced Lambda to new Func/Action
                Dispatcher.Invoke(new Action(() => SetClipboardText(s)));
                return;
            }
            Clipboard.SetText(s);
        }

        public void UnloadPlugins()
        {
            if (PluginsDomain != null)
            {
                DynamicPlugins.Clear();
                AppDomain.Unload(PluginsDomain);
            }
        }

        public static T FindAncestorOrSelf<T>(DependencyObject obj)
            where T : DependencyObject
        {
            while (obj != null)
            {
                T objTest = obj as T;
                if (objTest != null)
                    return objTest;
                obj = GetParent(obj);
            }

            return null;
        }

        public static DependencyObject GetParent(DependencyObject obj)
        {
            if (obj == null)
                return null;

            ContentElement ce = obj as ContentElement;
            if (ce == null)
                return VisualTreeHelper.GetParent(obj);

            DependencyObject parent = ContentOperations.GetParent(ce);
            if (parent != null)
                return parent;

            FrameworkContentElement fce = ce as FrameworkContentElement;
            return fce != null
                ? fce.Parent
                : null;
        }

        private void LoadCustomDir_Execute(bool subdirs)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "Select Directory to Load",
                ShowNewFolderButton = false,
                SelectedPath = Core.EditSaveDir,
            };
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            SeatDataBlock.ResetSeats();
            CharacterCollection collection = DataContext as CharacterCollection;
            if (collection != null)
            {
                while (!collection.ViewModelProvider.Finish())
                {
                    MessageBoxResult msg = MessageBox.Show(MESSAGE_ERROR_FINISH_PROVIDER,
                        "Error",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Hand);
                    if (msg == MessageBoxResult.Cancel)
                        return;
                }
                collection.ViewModelProvider.Dispose();
            }

            ICharacterViewModelProvider viewModelProvider = subdirs
                ? new FileSystemCharacterViewModelProvider(true, fbd.SelectedPath)
                : new FileSystemCharacterViewModelProvider(false, fbd.SelectedPath);

            viewModelProvider.Initialize(this);

            viewModelProvider.CharacterLoaded += ViewModelProviderOnCharacterLoaded;
            CharacterCollection characterCollection = new CharacterCollection(Dispatcher, viewModelProvider);
            viewModelProvider.CharacterLoaded -= ViewModelProviderOnCharacterLoaded;
            SetTitle();
            DataContext = characterCollection;

            SortBox.SelectedValue = viewModelProvider.DefaultSortProperty;
            OrderBox.SelectedValue = viewModelProvider.DefaultSortDirection;
            CharactersControl.SelectedIndex = 0;
        }

        private void ConfigPaths_Execute(bool play)
        {
            if (play)
                Core.GetPlayPath();
            else
                Core.GetEditPath();
        }

        private void LoadEditorSave_Execute(bool subdirs)
        {
            SeatDataBlock.ResetSeats();
            CharacterCollection collection = DataContext as CharacterCollection;
            if (collection != null)
            {
                while (!collection.ViewModelProvider.Finish())
                {
                    MessageBoxResult msg = MessageBox.Show(MESSAGE_ERROR_FINISH_PROVIDER,
                        "Error",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Hand);
                    if (msg == MessageBoxResult.Cancel)
                        return;
                }
                collection.ViewModelProvider.Dispose();
            }

            ICharacterViewModelProvider viewModelProvider;
            if (subdirs)
            {
                viewModelProvider = new FileSystemCharacterViewModelProvider(true, Core.EditSaveDir);
            }
            else
            {
                viewModelProvider = new FileSystemCharacterViewModelProvider(false,
                    Path.Combine(Core.EditSaveDir, "Male"),
                    Path.Combine(Core.EditSaveDir, "Female"));
            }

            viewModelProvider.CharacterLoaded += ViewModelProviderOnCharacterLoaded;
            viewModelProvider.Initialize(this);

            CharacterCollection characterCollection = new CharacterCollection(Dispatcher, viewModelProvider);
            viewModelProvider.CharacterLoaded -= ViewModelProviderOnCharacterLoaded;
            SetTitle();
            DataContext = characterCollection;

            SortBox.SelectedValue = viewModelProvider.DefaultSortProperty;
            OrderBox.SelectedValue = viewModelProvider.DefaultSortDirection;
            CharactersControl.SelectedIndex = 0;
        }

        private void LoadGameSave_Execute()
        {
            SeatDataBlock.ResetSeats();

            string saveDir = Path.Combine(Core.PlaySaveDir, "class");
            OpenFileDialog opfd = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                Filter = "Save Files (*.sav)|*.sav",
                InitialDirectory = saveDir
            };
            opfd.CustomPlaces.Add(new FileDialogCustomPlace(saveDir));
            if (!opfd.ShowDialog(this)
                .Value)
                return;

            CharacterCollection collection = DataContext as CharacterCollection;
            if (collection != null)
            {
                while (!collection.ViewModelProvider.Finish())
                {
                    MessageBoxResult msg = MessageBox.Show(MESSAGE_ERROR_FINISH_PROVIDER,
                        "Error",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Hand);
                    if (msg == MessageBoxResult.Cancel)
                        return;
                }
                collection.Dispose();
            }

            ICharacterViewModelProvider viewModelProvider = new SaveFileCharacterViewModelProvider(opfd.FileName);
            viewModelProvider.Initialize(this);

            viewModelProvider.CharacterLoaded += ViewModelProviderOnCharacterLoaded;
            CharacterCollection characterCollection = new CharacterCollection(Dispatcher, viewModelProvider);
            viewModelProvider.CharacterLoaded -= ViewModelProviderOnCharacterLoaded;
            SetTitle();
            DataContext = characterCollection;

            SortBox.SelectedValue = viewModelProvider.DefaultSortProperty;
            OrderBox.SelectedValue = viewModelProvider.DefaultSortDirection;
            CharactersControl.SelectedIndex = 0;

            characterCollection.Characters.ForEach
            (model =>
            {
                int seat = (int) model.ExtraData["PLAY_SEAT"];
                SeatDataBlock.SetSeat(seat, model.Profile.FullName);
                model.Character.PropertyChanged += (sender, args) =>
                {
                    if (!args.PropertyName.Contains("NAME"))
                        return;

                    string name = model.Profile.FullName;
                    SeatDataBlock.SetSeat(seat, name);
                };
            });
        }

        private void ReloadAll_Execute()
        {
            CharacterCollection collection = CharactersControl.DataContext as CharacterCollection;
            if (collection == null)
                return;
            var hasChanges = collection.Characters.Where(model => model.HasChanges);
            foreach (CharacterViewModel model in hasChanges)
            {
                model.ReloadCommand.Execute(null);
            }
        }

        private void SaveAll_Execute()
        {
            CharacterCollection collection = CharactersControl.DataContext as CharacterCollection;
            if (collection == null)
                return;
            var hasChanges = collection.Characters.Where(model => model.HasChanges);
            foreach (CharacterViewModel model in hasChanges)
            {
                model.SaveCommand.Execute(null);
            }
        }

        private bool SaveReloadAll_CanExecute()
        {
            CharacterCollection collection = CharactersControl.DataContext as CharacterCollection;
            if (collection == null)
                return false;
            var hasChanges = collection.Characters.Where(model => model.HasChanges);
            return hasChanges.Any();
        }

        private void SetTitle(string message = null)
        {
            if (!CheckAccess())
            {
                //[WXP] Replaced Lambda to new Func/Action
                Dispatcher.Invoke(new Action(() => SetTitle(message)));
                return;
            }
            Title = String.IsNullOrWhiteSpace(message)
                ? string.Format("ReiEdit AA2 Beta")
                : string.Format("ReiEdit AA2 Beta - {0}", message);
        }

        private void ShowAboutMenu_Execute()
        {
            AboutWindow wnd = new AboutWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            wnd.ShowDialog();
        }

        private static Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
            return tcs.Task;
        }
    }
}