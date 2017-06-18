// --------------------------------------------------
// ReiEditAA2 - CharacterCollection.cs
// --------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Threading;
using AA2Lib;
using ReiEditAA2.Code.CharacterViewModelProviders;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code
{
    internal class CharacterCollection : INotifyPropertyChanged, IDisposable
    {
        private readonly Dispatcher _uiDispatcher;
        private ObservableCollection<CharacterViewModel> _characters;

        private ListSortDirection _sortDirection;
        private ComboItem[] _sortDirections;
        private string _sortFilter;
        private ComboItem[] _sortProperties;
        private string _sortProperty;

        public ObservableCollection<CharacterViewModel> Characters
        {
            get { return _characters; }
            private set
            {
                _characters = value;
                CollectionView = CollectionViewSource.GetDefaultView(Characters);
                CollectionView.Filter = Filter;
                RefreshView();
            }
        }

        public ICollectionView CollectionView { get; set; }
        public bool IsDisposed { get; set; }

        public ListSortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                _sortDirection = value;
                OnPropertyChanged("SortDirection");
                RefreshView();
            }
        }

        public ComboItem[] SortDirections
        {
            get { return _sortDirections; }
            set
            {
                _sortDirections = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("SortDirections");
            }
        }

        public string SortFilter
        {
            get { return _sortFilter; }
            set
            {
                _sortFilter = value;
                CollectionView.Refresh();
            }
        }

        public ComboItem[] SortProperties
        {
            get { return _sortProperties; }
            private set
            {
                _sortProperties = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("SortProperties");
            }
        }

        public string SortProperty
        {
            get { return _sortProperty; }
            set
            {
                _sortProperty = value;
                OnPropertyChanged("SortProperty");
                RefreshView();
            }
        }

        public ICharacterViewModelProvider ViewModelProvider { get; private set; }

        public CharacterCollection(Dispatcher dispatcher, ICharacterViewModelProvider viewModelProvider)
        {
            _uiDispatcher = dispatcher;

            _sortProperties = viewModelProvider.GetSortableProperties()
                .Select(pair => new ComboItem(pair.Value, pair.Key))
                .ToArray();

            _sortProperty = viewModelProvider.DefaultSortProperty;
            _sortDirection = viewModelProvider.DefaultSortDirection;

            _sortDirections = new[]
            {
                new ComboItem(ListSortDirection.Ascending, "Ascending"),
                //
                new ComboItem(ListSortDirection.Descending, "Descending")
            };

            ViewModelProvider = viewModelProvider;
            ViewModelProvider.CharacterAdded += ProviderOnCharacterAdded;
            ViewModelProvider.CharacterUpdated += ProviderOnCharacterUpdated;

            Characters = new ObservableCollection<CharacterViewModel>(viewModelProvider.GetCharacters());
            foreach (CharacterViewModel model in Characters)
            {
                model.ParentCollection = this;
            }
        }

        private void ProviderOnCharacterAdded(object sender, CharacterViewModelProviderEventArgs args)
        {
            CharacterViewModel vm = null;
            if (args.OldMetadata != null)
            {
                vm = Characters.FirstOrDefault(model => args.OldMetadata.Equals(model.Metadata));
            }

            if (vm == null)
                //[WXP] Replaced Lambda to new Func/Action
                _uiDispatcher.Invoke(new Action(() => Characters.Insert(0, args.Model)));
            else
            {
                vm.Character.CardBytes = args.Model.Character.CardBytes;
                vm.Character.ThumbBytes = args.Model.Character.ThumbBytes;
                vm.Character.DataBytes = args.Model.Character.DataBytes;
            }

            _uiDispatcher.Invoke(new Action(RefreshView));
        }

        private void ProviderOnCharacterUpdated(object sender, CharacterViewModelProviderEventArgs args)
        {
            CharacterViewModel vm = Characters.FirstOrDefault(model => args.OldMetadata.Equals(model.Metadata));
            if (vm == null)
                return;
            if (args.Model != null)
            {
                vm.Character.CardBytes = args.Model.Character.CardBytes;
                vm.Character.ThumbBytes = args.Model.Character.ThumbBytes;
                vm.Character.DataBytes = args.Model.Character.DataBytes;

                vm.ExtraData.Clear();
                foreach (var pair in args.Model.ExtraData)
                {
                    vm.ExtraData.Add(pair.Key, pair.Value);
                }
            }
            if (args.NewMetadata != null)
                vm.Metadata = args.NewMetadata;

            _uiDispatcher.Invoke(new Action(RefreshView));
        }

        //[WXP] CallerMemberName Removed 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Filter(object o)
        {
            if (o == null || String.IsNullOrWhiteSpace(SortFilter))
                return true;
            CharacterViewModel model = (CharacterViewModel) o;
            const RegexOptions options =
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant;
            string name = string.Format("{0} {1}", model.Profile.FamilyName.Value, model.Profile.FirstName.Value);
            return Regex.IsMatch(name, SortFilter, options);
        }

        private void RefreshView()
        {
            if (CollectionView == null || SortProperty == null)
                return;
            CollectionView.SortDescriptions.Clear();
            CollectionView.SortDescriptions.Add(new SortDescription(SortProperty, SortDirection));
        }

        public void Dispose()
        {
            ViewModelProvider.Dispose();
            Characters.ForEach(model => model.Dispose());
            Characters.Clear();
            IsDisposed = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public struct ComboItem
        {
            public string Display { get; set; }
            public object Value { get; set; }

            public ComboItem(object value, string display) : this()
            {
                Value = value;
                Display = display;
            }

            public ComboItem(string displayValue) : this()
            {
                Value = displayValue;
                Display = displayValue;
            }
        }
    }
}