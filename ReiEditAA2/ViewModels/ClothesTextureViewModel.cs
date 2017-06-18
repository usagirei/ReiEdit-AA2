// --------------------------------------------------
// ReiEditAA2 - ClothesTextureViewModel.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.ViewModels
{
    internal class ClothesTextureViewModel : IDisposable, INotifyPropertyChanged
    {
        private string _keyBrightness;
        private string _keyHue;
        private string _keyShadowBrightness;
        private string _keyShadowHue;
        private string _keyTexture;

        public DataBlockWrapper Brightness
        {
            get { return ClothesViewModel.CharacterViewModel.GetAttribute(_keyBrightness); }
        }

        public ClothesViewModel ClothesViewModel { get; set; }

        public DataBlockWrapper Hue
        {
            get { return ClothesViewModel.CharacterViewModel.GetAttribute(_keyHue); }
        }

        public bool IsUnderwear { get; private set; }

        public ClothesViewModel.ClothesKind Kind
        {
            get { return ClothesViewModel.Kind; }
        }

        public string SetName
        {
            get
            {
                return IsUnderwear
                    ? "Underwear"
                    : "Skirt";
            }
        }

        public DataBlockWrapper ShadowBrightness
        {
            get { return ClothesViewModel.CharacterViewModel.GetAttribute(_keyShadowBrightness); }
        }

        public DataBlockWrapper ShadowHue
        {
            get { return ClothesViewModel.CharacterViewModel.GetAttribute(_keyShadowHue); }
        }

        public DataBlockWrapper Suit
        {
            get { return ClothesViewModel.Suit; }
        }

        public DataBlockWrapper Texture
        {
            get { return ClothesViewModel.CharacterViewModel.GetAttribute(_keyTexture); }
        }

        public IEnumerable<TextureWatcher.TextureElement> TextureFilesList
        {
            get
            {
                return IsUnderwear
                    ? TextureWatcher.Instance[string.Format("sitagi{0:D2}", (int) Suit.Value)]
                    : TextureWatcher.Instance[string.Format("skirt{0:D2}", (int) Suit.Value)];
            }
        }

        public ClothesTextureViewModel(ClothesViewModel parent, bool underwear)
        {
            ClothesViewModel = parent;
            IsUnderwear = underwear;
            UpdateKeys();
        }

        private void SuitOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged("TextureFilesList");
        }

        public void UpdateKeys()
        {
            if (_keyTexture != null)
            {
                Dispose();
            }

            string suit = null;
            switch (Kind)
            {
                case ClothesViewModel.ClothesKind.Uniform:
                    suit = "UNIFORM";
                    break;
                case ClothesViewModel.ClothesKind.Sports:
                    suit = "SPORT";
                    break;
                case ClothesViewModel.ClothesKind.Swimsuit:
                    suit = "SWIMSUIT";
                    break;
                case ClothesViewModel.ClothesKind.Formal:
                    suit = "CLUB";
                    break;
            }
            string set = IsUnderwear
                ? "UNDERWEAR"
                : "SKIRT";

            _keyHue = String.Format("{0}_{1}_HUE", suit, set);
            _keyBrightness = String.Format("{0}_{1}_BRIGHTNESS", suit, set);
            _keyShadowHue = String.Format("{0}_{1}_SHADOW_HUE", suit, set);
            _keyShadowBrightness = String.Format("{0}_{1}_SHADOW_BRIGHTNESS", suit, set);
            _keyTexture = String.Format("{0}_{1}_TEXTURE", suit, set);

            Suit.PropertyChanged += SuitOnPropertyChanged;
        }

        //[WXP] CallerMemberName Removed 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Suit.PropertyChanged -= SuitOnPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}