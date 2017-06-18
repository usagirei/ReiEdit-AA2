// --------------------------------------------------
// AA2Lib - TextureWatcher.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AA2Lib.Code
{
    public class TextureWatcher
    {
        private const string ANY_PATTERN = "*.bmp";
        private const string NULL_TEX_NAME = "NullTex_000.bmp";
        private const string SKIRT_PATTERN = "スカート_*.bmp";
        private const string UNDERWEAR_PATTERN = "下着_*.bmp";

        public static readonly TextureWatcher Instance = new TextureWatcher();

        public static readonly IEnumerable<TextureElement> NullTex = new[]
        {
            new TextureElement
            {
                FullPath = NULL_TEX_NAME
            }
        };

        private static readonly Dispatcher Dispatcher;
        private static readonly FileSystemWatcher EditTextureDirWatcher;
        private static readonly FileSystemWatcher PlayTextureDirWatcher;
        private static readonly Dictionary<string, ImageSource> TextureCache;
        private static readonly Dictionary<string, ObservableCollection<TextureElement>> TextureList;

        public ObservableCollection<TextureElement> this[string key]
        {
            get
            {
                return TextureList.ContainsKey(key)
                    ? TextureList[key]
                    : null;
            }
        }

        static TextureWatcher()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            TextureCache = new Dictionary<string, ImageSource>
            {
                {NULL_TEX_NAME.ToLowerInvariant(), CreateEmptyBitmap()}
            };

            TextureList = new Dictionary<string, ObservableCollection<TextureElement>>();

            foreach (string dir in Directory.EnumerateDirectories(Core.EditTextureDir))
            {
                var coll = new ObservableCollection<TextureElement>
                (Directory.EnumerateFiles(dir, ANY_PATTERN)
                    .Select(TextureElement.Create));

                string dirName = Path.GetFileName(dir);
                TextureList.Add(dirName, coll);
            }

            foreach (string dir in Directory.EnumerateDirectories(Core.PlayTextureDir, "skirt*"))
            {
                var coll = new ObservableCollection<TextureElement>
                (NullTex.Concat
                (Directory.EnumerateFiles(dir, SKIRT_PATTERN)
                    .Select(TextureElement.Create)));

                string dirName = Path.GetFileName(dir);
                TextureList.Add(dirName, coll);
            }

            foreach (string dir in Directory.EnumerateDirectories(Core.PlayTextureDir, "sitagi*"))
            {
                var coll = new ObservableCollection<TextureElement>
                (NullTex.Concat
                (Directory.EnumerateFiles(dir, UNDERWEAR_PATTERN)
                    .Select(TextureElement.Create)));

                string dirName = Path.GetFileName(dir);
                TextureList.Add(dirName, coll);
            }

            EditTextureDirWatcher = new FileSystemWatcher(Core.EditTextureDir, ANY_PATTERN)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite,
                IncludeSubdirectories = true
            };
            PlayTextureDirWatcher = new FileSystemWatcher(Core.PlayTextureDir, ANY_PATTERN)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite,
                IncludeSubdirectories = true
            };

            EditTextureDirWatcher.Changed += TextureDirWatcherOnChanged;
            EditTextureDirWatcher.Deleted += TextureDirWatcherOnChanged;
            EditTextureDirWatcher.Renamed += TextureDirWatcherOnChanged;
            EditTextureDirWatcher.Created += TextureDirWatcherOnChanged;

            PlayTextureDirWatcher.Changed += TextureDirWatcherOnChanged;
            PlayTextureDirWatcher.Deleted += TextureDirWatcherOnChanged;
            PlayTextureDirWatcher.Renamed += TextureDirWatcherOnChanged;
            PlayTextureDirWatcher.Created += TextureDirWatcherOnChanged;
        }

        private TextureWatcher()
        { }

        private static void TextureDirWatcherOnChanged(object sender, FileSystemEventArgs args)
        {
            string dir = Path.GetFileName(Path.GetDirectoryName(args.FullPath));
            if (dir == null)
                return;
            dir = dir.ToLower();

            WatcherChangeTypes changeType = args.ChangeType;
            string oldPath = changeType == WatcherChangeTypes.Renamed
                ? ((RenamedEventArgs) args).OldFullPath
                : args.FullPath;

            string newPath = args.FullPath;

            if (changeType == WatcherChangeTypes.Deleted || changeType == WatcherChangeTypes.Changed
                || changeType == WatcherChangeTypes.Renamed)
            {
                string pathli = oldPath.ToLowerInvariant();
                if (TextureCache.ContainsKey(pathli))
                    //[WXP] Replaced Lambda to new Func/Action  
                    Dispatcher.Invoke(new Func<bool>(() => TextureCache.Remove(pathli)));
            }

            var collection = TextureList.ContainsKey(dir)
                ? TextureList[dir]
                : null;

            if (collection == null)
                return;

            TextureElement element = collection.FirstOrDefault
                (textureElement => textureElement.FullPath.Equals(oldPath, StringComparison.OrdinalIgnoreCase));

            if (changeType == WatcherChangeTypes.Deleted && element != null)
                //[WXP] Replaced Lambda to new Func/Action
                Dispatcher.Invoke(new Func<bool>(() => collection.Remove(element)));

            if (changeType == WatcherChangeTypes.Created)
                //[WXP] Replaced Lambda to new Func/Action
                Dispatcher.Invoke(new Action(() => collection.Add(TextureElement.Create(newPath))));

            if ((changeType != WatcherChangeTypes.Renamed && changeType != WatcherChangeTypes.Changed)
                || element == null)
                return;

            if (changeType == WatcherChangeTypes.Renamed)
                element.FullPath = newPath;
            element.RaiseChanges();
        }

        public static ImageSource BitmapFromUri(Uri source)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.EndInit();
            return bitmap;
        }

        public static ImageSource GetImageSource(string path)
        {
            if (!File.Exists(path) && path != NULL_TEX_NAME)
                return null;
            string key = path.ToLowerInvariant();
            if (TextureCache.ContainsKey(key))
            {
                return TextureCache[key];
            }
            ImageSource src = BitmapFromUri(new Uri(path, UriKind.Absolute));
            TextureCache.Add(key, src);
            return src;
        }

        private static ImageSource CreateEmptyBitmap()
        {
            const int width = 8;
            const int height = width;
            const int stride = width / 8;
            var pixels = new byte[height * stride];

            // Try creating a new image with a custom palette.
            var colors = new List<Color>
            {
                Colors.Transparent
            };
            BitmapPalette myPalette = new BitmapPalette(colors);

            // Creates a new empty image with the pre-defined palette
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Indexed1, myPalette, pixels, stride);
        }

        public class TextureElement : INotifyPropertyChanged
        {
            private string _fullPath;

            public string FullPath
            {
                get { return _fullPath; }
                set
                {
                    _fullPath = value;
                    //[WXP] Doesn't support CallerMemberName
                    OnPropertyChanged("FullPath");
                }
            }

            public ImageSource Image
            {
                get { return GetImageSource(_fullPath); }
            }

            public string Name
            {
                get { return Path.GetFileName(_fullPath); }
            }

            public int Value
            {
                get
                {
                    if (Name.StartsWith("下着_"))
                        return int.Parse(Name.Substring(3, 3));
                    if (Name.StartsWith("スカート_"))
                        return int.Parse(Name.Substring(5, 3));
                    return 0;
                }
            }

            public void RaiseChanges()
            {
                OnPropertyChanged(String.Empty);
            }

            public static TextureElement Create(string s)
            {
                TextureElement element = new TextureElement
                {
                    FullPath = s,
                };
                return element;
            }

            //[WXP] CallerMemberName Removed

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}