// --------------------------------------------------
// ReiEditAA2 - CharacterViewModel.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using AA2Lib;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using Microsoft.Win32;
using ReiEditAA2.Code;
using ReiEditAA2.Code.CharacterViewModelProviders;
using ReiEditAA2.Plugins;

namespace ReiEditAA2.ViewModels
{
    internal class CharacterViewModel : MarshalByRefObject, INotifyPropertyChanged, IDisposable
    {
        private readonly PluginCharacterProxy _proxy;
        private Dictionary<string, object> _extraData;

        private ICommand _reloadCommand;
        private ICommand _saveAsCommand;
        private ICommand _saveCommand;

        public BodyViewModel Body { get; set; }
        public CharacterFile Character { get; set; }
        public ClothesViewModel ClubSuit { get; private set; }

        public IEnumerable<DataBlockWrapper> Compatibility
        {
            get
            {
                int[] order =
                {
                    19, 14, 24, 5, 0,
                    //
                    20, 15, 10, 6, 1,
                    //
                    21, 16, 11, 7, 2,
                    //
                    22, 17, 12, 8, 3,
                    //
                    23, 18, 13, 9, 4
                };
                var compats = Character.CharAttributes.Values.Where(value => value.Key.StartsWith("COMPATIBILITY_"));
                return order.Select(compats.ElementAt);
            }
        }

        public Dictionary<string, object> ExtraData
        {
            get { return _extraData; }
            set { _extraData = value; }
        }

        public bool HasChanges
        {
            get
            {
                if (Character == null)
                    return false;
                bool changes = Character.HasChanges;
                if (ExtraData.ContainsKey("PLAY_DATA"))
                {
                    changes |= ((DataBlockWrapperBuffer) ExtraData["PLAY_DATA"]).HasChanges;
                }
                return changes;
            }
        }

        public bool IsDisposed { get; set; }

        public object Metadata { get; set; }
        public CharacterCollection ParentCollection { get; set; }

        public IEnumerable<DataBlockWrapper> Preferences
        {
            get { return Character.CharAttributes.Values.Where(value => value.Key.StartsWith("PREFERENCE_")); }
        }

        public IEnumerable<DataBlockWrapper> Pregnancy
        {
            get { return Character.CharAttributes.Values.Where(value => value.Key.StartsWith("PREGNANCY_")); }
        }

        public ProfileViewModel Profile { get; private set; }

        public PluginCharacterProxy Proxy
        {
            get { return _proxy; }
        }

        public ICommand ReloadCommand
        {
            get { return _reloadCommand; }
            set
            {
                _reloadCommand = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("ReloadCommand");
            }
        }

        public ICommand SaveAsCommand
        {
            get { return _saveAsCommand; }
            set
            {
                _saveAsCommand = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("SaveAsCommand");
            }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
            set
            {
                _saveCommand = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("SaveCommand");
            }
        }

        public ClothesViewModel SportsSuit { get; private set; }
        public ClothesViewModel SwimsuitSuit { get; private set; }

        public IEnumerable<DataBlockWrapper> Traits
        {
            get { return Character.CharAttributes.Values.Where(value => value.Key.StartsWith("TRAIT_")); }
        }

        public ClothesViewModel UniformSuit { get; private set; }
        private DataBlockConstraints Constraints { get; set; }

        ~CharacterViewModel()
        {
            Dispose();
        }

        public CharacterViewModel(CharacterFile cf, object metadata)
        {
            _extraData = new Dictionary<string, object>();
            _proxy = new PluginCharacterProxy(this);
            Metadata = metadata;
            Character = cf;
            Character.PropertyChanged += FileOnPropertyChanged;

            Profile = new ProfileViewModel(this);

            Body = new BodyViewModel(this);

            UniformSuit = new ClothesViewModel(this, ClothesViewModel.ClothesKind.Uniform);
            SwimsuitSuit = new ClothesViewModel(this, ClothesViewModel.ClothesKind.Swimsuit);
            SportsSuit = new ClothesViewModel(this, ClothesViewModel.ClothesKind.Sports);
            ClubSuit = new ClothesViewModel(this, ClothesViewModel.ClothesKind.Formal);

            Constraints = new DataBlockConstraints(this);

            SaveAsCommand = new RelayCommand(SaveAs_Execute, SaveAs_CanExecute);
        }

        private void FileOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (String.IsNullOrEmpty(args.PropertyName))
            {
                OnPropertyChanged(String.Empty);
            }
        }

        public DataBlockWrapper GetAttribute(string attr)
        {
            return IsDisposed
                ? null
                : Character.CharAttributes[attr];
        }

        public void RaiseChangeCheck()
        {
            OnPropertyChanged("HasChanges");
        }

        //[WXP] CallerMemberName Removed 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SaveAs_CanExecute()
        {
            return true;
            return !HasChanges;
        }

        private void SaveAs_Execute()
        {
            bool male = Profile.Gender.Value.Equals((byte) 0);
            string dir = Path.Combine(Core.EditSaveDir,
                male
                    ? "Male"
                    : "Female");
            OpenFileDialog opfl = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                DefaultExt = ".png",
                Filter = "PNG Files (*.png)|*.png",
                FilterIndex = 0,
                FileName = Profile.FullName,
                InitialDirectory = dir
            };
            opfl.CustomPlaces.Add(new FileDialogCustomPlace(Path.Combine(Core.EditSaveDir, "Male")));
            opfl.CustomPlaces.Add(new FileDialogCustomPlace(Path.Combine(Core.EditSaveDir, "Female")));
            var showDialog = opfl.ShowDialog();
            if (showDialog == null || !showDialog.Value)
                return;
            string path = opfl.FileName;
            FileStream fs = File.OpenWrite(path);

            // Card Bytes
            fs.Write(Character.CardBytes, 0, Character.CardBytes.Length);
            // Data Bytes
            fs.Write(Character.DataBytes, 0, Character.DataBytes.Length);
            // Thumb Lenght
            fs.Write(BitConverter.GetBytes(Character.ThumbBytes.Length), 0, 4);
            // Thumb Bytes
            fs.Write(Character.ThumbBytes, 0, Character.ThumbBytes.Length);
            // Identifier Mark
            int lastBytes = Character.ThumbBytes.Length + 4 + Character.DataBytes.Length + 4;
            fs.Write(BitConverter.GetBytes(lastBytes), 0, 4);
            // Make Sure Flushed
            fs.Flush();

            fs.Close();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            Character.Dispose();
            Profile.Dispose();
            Constraints.Dispose();
            Character.PropertyChanged -= FileOnPropertyChanged;
            Character = null;
            IsDisposed = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}