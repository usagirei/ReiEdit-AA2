// --------------------------------------------------
// ReiEditAA2 - FileSystemCharacterViewModelProvider.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using AA2Lib.Code;
using ReiEditAA2.ViewModels;
using ReiEditAA2.Views;
using MessageBox = System.Windows.MessageBox;

namespace ReiEditAA2.Code.CharacterViewModelProviders
{
    internal class FileSystemCharacterViewModelProvider : ICharacterViewModelProvider
    {
        private readonly List<FileSystemWatcher> _fsWatchers = new List<FileSystemWatcher>();
        private readonly bool _subDirs;
        private FSProviderTools _editor;
        private string _lastCreated;
        private string[] _paths;
        private bool _preventFinish;
        private bool _preventWndClose;
        private List<CharacterViewModel> _vmCache;

        public string[] Paths
        {
            get { return _paths; }
            set
            {
                _paths = value;
                InitFSWatcher();
            }
        }

        public FileSystemCharacterViewModelProvider(bool includeSubdirs, params string[] paths)
        {
            _subDirs = includeSubdirs;
            Paths = paths;
        }

        private void FSWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            CharacterFile cf = CharacterFile.Load(e.FullPath);
            if (cf == null)
                return;
            CharacterViewModel viewmodel = CreateViewModel(cf, e.FullPath);

            if (_lastCreated != null && e.FullPath.Equals(_lastCreated))
            {
                _lastCreated = null;
                OnCharacterAdded(new CharacterViewModelProviderEventArgs(viewmodel));
                return;
            }
            OnCharacterUpdated(new CharacterViewModelProviderEventArgs(viewmodel, e.FullPath));
        }

        private void FSWatcher_Created(object sender, FileSystemEventArgs e)
        {
            _lastCreated = e.FullPath;
        }

        private void FSWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            OnCharacterUpdated(new CharacterViewModelProviderEventArgs(null, e.OldFullPath, e.FullPath));
        }

        protected virtual void OnCharacterAdded(CharacterViewModelProviderEventArgs args)
        {
            _vmCache.Add(args.Model);

            CharacterProviderDelegate handler = CharacterAdded;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void OnCharacterUpdated(CharacterViewModelProviderEventArgs args)
        {
            CharacterViewModel @char = _vmCache.FirstOrDefault(model => args.OldMetadata.Equals(model.Metadata));
            if (@char != null)
                @char.Metadata = args.NewMetadata ?? args.OldMetadata;

            CharacterProviderDelegate handler = CharacterUpdated;
            if (handler != null)
                handler(this, args);
        }

        private CharacterViewModel CreateViewModel(CharacterFile cf, string file)
        {
            CharacterViewModel cvm = new CharacterViewModel(cf, file);
            if (File.Exists(file))
            {
                DateTime lastWriteTime = new FileInfo(file).LastWriteTime;
                cvm.ExtraData.Add("FILE_MOD", lastWriteTime);
            }
            cvm.SaveCommand = new RelayCommand(() => SaveViewModel(cvm), () => SaveViewModelCanExecute(cvm));
            cvm.ReloadCommand = new RelayCommand(() => ReloadViewModel(cvm));
            return cvm;
        }

        private void InitFSWatcher()
        {
            if (_fsWatchers.Any())
                _fsWatchers.ForEach(watcher => watcher.Dispose());
            _fsWatchers.Clear();

            foreach (string path in Paths)
            {
                FileSystemWatcher fsWatcher = new FileSystemWatcher(path)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = _subDirs,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                    Filter = "*.png",
                };

                fsWatcher.Changed += FSWatcher_Changed;
                fsWatcher.Renamed += FSWatcher_Renamed;
                fsWatcher.Created += FSWatcher_Created;

                _fsWatchers.Add(fsWatcher);
            }
        }

        private void ReloadViewModel(CharacterViewModel model)
        {
            string filePath = (string) model.Metadata;
            CharacterFile cf = CharacterFile.Load(filePath);
            if (cf == null)
                return;
            CharacterViewModel newViewModel = new CharacterViewModel(cf, filePath);
            OnCharacterUpdated(new CharacterViewModelProviderEventArgs(newViewModel, filePath));
        }

        private void Report(int current, int max, string message)
        {
            if (CharacterLoaded != null)
                CharacterLoaded(this, new CharacterLoadedDelegateArgs(current, max, message));
        }

        private void SaveViewModel(CharacterViewModel model)
        {
            FileStream fs = null;
            try
            {
                // Disable Watchers Temporarily
                _fsWatchers.ForEach(watcher => watcher.EnableRaisingEvents = false);
                //_fsWatcher.EnableRaisingEvents = false;
                fs = new FileStream(model.Metadata as string, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1024 * 16);

                // Card Bytes
                fs.Write(model.Character.CardBytes, 0, model.Character.CardBytes.Length);
                // Data Bytes
                fs.Write(model.Character.DataBytes, 0, model.Character.DataBytes.Length);
                // Thumb Lenght
                fs.Write(BitConverter.GetBytes(model.Character.ThumbBytes.Length), 0, 4);
                // Thumb Bytes
                fs.Write(model.Character.ThumbBytes, 0, model.Character.ThumbBytes.Length);
                // Variable-sized remainder can be appended to the thumb (used by tools like AAU override asset blobs)
                fs.Write(model.Character.RawBytes, 0, model.Character.RawBytes.Length);
                int lastBytes = model.Character.ThumbBytes.Length + 4 + model.Character.DataBytes.Length + model.Character.RawBytes.Length + 4;
                // Identifier Mark
                fs.Write(BitConverter.GetBytes(lastBytes), 0, 4);
                // Make Sure Flushed
                fs.Flush();
                

                model.Character.DataChanges = false;
                model.Character.CardChanges = false;
                model.Character.ThumbChanges = false;
            }
            catch
            { }
            finally
            {
                //Close Stream and ReEnable Watchers
                if (fs != null)
                    fs.Close();
                _fsWatchers.ForEach(watcher => watcher.EnableRaisingEvents = true);
            }
        }

        private bool SaveViewModelCanExecute(CharacterViewModel characterViewModel)
        {
            return characterViewModel != null && characterViewModel.HasChanges;
        }

        private IEnumerable<CharacterViewModel> ScanDirectories()
        {
            SearchOption searchMode = _subDirs
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            var files = Paths.SelectMany(s => Directory.GetFiles(s, "*.png", searchMode))
                .ToArray();

            var queue = new Queue<string>(files);

            List<CharacterViewModel> characters = new List<CharacterViewModel>();

            int loaded = 0;
            int max = queue.Count;

            Action<object> taskAction = (obj) =>
            {
                string file = (string) obj;
                CharacterFile cf = CharacterFile.Load(file);
                if (cf == null)
                    return;
                CharacterViewModel vm = CreateViewModel(cf, file);
                characters.Add(vm);
            };

            Task[] tasks = new Task[Math.Min(Environment.ProcessorCount, max)];
            for (int i = 0; i < tasks.Length; i++)
            {
                string pull = queue.Dequeue();
                tasks[i] = Task.Factory.StartNew(taskAction, pull);
            }

            while (queue.Any())
            {
                int finished = Task.WaitAny(tasks);
                string pull = queue.Dequeue();
                string file = (string) tasks[finished].AsyncState;
                Report(loaded++, max, Path.GetFileName(file));
                tasks[finished] = Task.Factory.StartNew(taskAction, pull);
            }

            Task.WaitAll(tasks);

            /*
             * int max = files.Length;
            for (int i = 0; i < max; i++)
            {
                string file = files[i];
                CharacterFile cf = CharacterFile.Load(file);
                if (cf == null)
                    continue;
                CharacterViewModel vm = CreateViewModel(cf, file);
                Report(i, max, Path.GetFileName(file));
                characters.Add(vm);
            }
            */

            return characters;
        }

        public event CharacterLoadedDelegate CharacterLoaded;

        public IEnumerable<CharacterViewModel> GetCharacters()
        {
            return _vmCache ?? (_vmCache = ScanDirectories()
                       .ToList());
        }

        public Dictionary<string, string> GetSortableProperties()
        {
            return new Dictionary<string, string>
            {
                {"Modification Date", "ExtraData[FILE_MOD]"},
                {"Name", "Profile.FullName"},
                {"Gender", "Profile.Gender.Value"},
                {"Personality", "Profile.Personality.Value"},
            };
        }

        public string DefaultSortProperty
        {
            get { return "ExtraData[FILE_MOD]"; }
        }

        public ListSortDirection DefaultSortDirection
        {
            get { return ListSortDirection.Descending; }
        }

        public void Initialize(Window wnd)
        {
            _preventWndClose = true;
            _editor = new FSProviderTools(this);
            _editor.Owner = wnd;
            _editor.Closing += (sender, args) => args.Cancel = _preventWndClose;
            _editor.Show();
            double newLeft = SystemParameters.PrimaryScreenWidth - _editor.Width;
            double newTop = SystemParameters.PrimaryScreenHeight - _editor.Height;
            _editor.Left = newLeft;
            _editor.Top = newTop;
        }

        public bool Finish()
        {
            _preventWndClose = false;
            _editor.Close();
            return true;
        }

        public event CharacterProviderDelegate CharacterAdded;

        public event CharacterProviderDelegate CharacterUpdated;

        public void Dispose()
        {
            _fsWatchers.ForEach
            (watcher =>
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            });
        }
    }
}