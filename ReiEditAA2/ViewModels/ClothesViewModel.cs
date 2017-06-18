// --------------------------------------------------
// ReiEditAA2 - ClothesViewModel.cs
// --------------------------------------------------

using System;
using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.ViewModels
{
    internal class ClothesViewModel
    {
        public enum ClothesKind
        {
            Uniform = 0,
            Swimsuit = 1,
            Sports = 2,
            Formal = 3
        }

        private string _keyBottomColor1;
        private string _keyBottomColor2;
        private string _keyInShoesF;
        private string _keyInShoesM;
        private string _keyIndoorShoesColor;
        private string _keyIsOnepiece;
        private string _keyIsSkirt;
        private string _keyIsUnderwear;

        private string _keyOutShoesF;
        private string _keyOutShoesM;
        private string _keyOutdoorShoesColor;

        private string _keyShortSkirt;
        private string _keySocksColor;
        private string _keySocksF;
        private string _keySocksM;

        private string _keySuitF;
        private string _keySuitM;

        private string _keyTopColor1;
        private string _keyTopColor2;
        private string _keyTopColor3;
        private string _keyTopColor4;

        private string _keyUnderwearColor;

        public DataBlockWrapper BottomColor1
        {
            get { return CharacterViewModel.GetAttribute(_keyBottomColor1); }
        }

        public DataBlockWrapper BottomColor2
        {
            get { return CharacterViewModel.GetAttribute(_keyBottomColor2); }
        }

        public CharacterViewModel CharacterViewModel { get; set; }

        public DataBlockWrapper IndoorShoes
        {
            get
            {
                if (CharacterViewModel.IsDisposed)
                    return null;
                bool ismale = CharacterViewModel.Profile.Gender.Value.Equals((byte) 0);
                return ismale
                    ? IndoorShoesM
                    : IndoorShoesF;
            }
        }

        public DataBlockWrapper IndoorShoesColor
        {
            get { return CharacterViewModel.GetAttribute(_keyIndoorShoesColor); }
        }

        public DataBlockWrapper IndoorShoesF
        {
            get { return CharacterViewModel.GetAttribute(_keyInShoesF); }
        }

        public DataBlockWrapper IndoorShoesM
        {
            get { return CharacterViewModel.GetAttribute(_keyInShoesM); }
        }

        public DataBlockWrapper IsOnePiece
        {
            get { return CharacterViewModel.GetAttribute(_keyIsOnepiece); }
        }

        public DataBlockWrapper IsSkirt
        {
            get { return CharacterViewModel.GetAttribute(_keyIsSkirt); }
        }

        public DataBlockWrapper IsUnderwear
        {
            get { return CharacterViewModel.GetAttribute(_keyIsUnderwear); }
        }

        public ClothesKind Kind { get; private set; }

        public DataBlockWrapper OutdoorShoes
        {
            get
            {
                if (CharacterViewModel.IsDisposed)
                    return null;
                bool ismale = CharacterViewModel.Profile.Gender.Value.Equals((byte) 0);
                return ismale
                    ? OutdoorShoesM
                    : OutdoorShoesF;
            }
        }

        public DataBlockWrapper OutdoorShoesColor
        {
            get { return CharacterViewModel.GetAttribute(_keyOutdoorShoesColor); }
        }

        public DataBlockWrapper OutdoorShoesF
        {
            get { return CharacterViewModel.GetAttribute(_keyOutShoesF); }
        }

        public DataBlockWrapper OutdoorShoesM
        {
            get { return CharacterViewModel.GetAttribute(_keyOutShoesM); }
        }

        public DataBlockWrapper ShortSkirt
        {
            get { return CharacterViewModel.GetAttribute(_keyShortSkirt); }
        }

        public ClothesTextureViewModel SkirtTexture { get; private set; }

        public DataBlockWrapper Socks
        {
            get
            {
                if (CharacterViewModel.IsDisposed)
                    return null;
                bool ismale = CharacterViewModel.Profile.Gender.Value.Equals((byte) 0);
                return ismale
                    ? SocksM
                    : SocksF;
            }
        }

        public DataBlockWrapper SocksColor
        {
            get { return CharacterViewModel.GetAttribute(_keySocksColor); }
        }

        public DataBlockWrapper SocksF
        {
            get { return CharacterViewModel.GetAttribute(_keySocksF); }
        }

        public DataBlockWrapper SocksM
        {
            get { return CharacterViewModel.GetAttribute(_keySocksM); }
        }

        public DataBlockWrapper Suit
        {
            get
            {
                if (CharacterViewModel.IsDisposed)
                    return null;
                bool ismale = CharacterViewModel.Profile.Gender.Value.Equals((byte) 0);
                return ismale
                    ? SuitM
                    : SuitF;
            }
        }

        public DataBlockWrapper SuitF
        {
            get { return CharacterViewModel.GetAttribute(_keySuitF); }
        }

        public DataBlockWrapper SuitM
        {
            get { return CharacterViewModel.GetAttribute(_keySuitM); }
        }

        public DataBlockWrapper TopColor1
        {
            get { return CharacterViewModel.GetAttribute(_keyTopColor1); }
        }

        public DataBlockWrapper TopColor2
        {
            get { return CharacterViewModel.GetAttribute(_keyTopColor2); }
        }

        public DataBlockWrapper TopColor3
        {
            get { return CharacterViewModel.GetAttribute(_keyTopColor3); }
        }

        public DataBlockWrapper TopColor4
        {
            get { return CharacterViewModel.GetAttribute(_keyTopColor4); }
        }

        public DataBlockWrapper UnderwearColor
        {
            get { return CharacterViewModel.GetAttribute(_keyUnderwearColor); }
        }

        public ClothesTextureViewModel UnderwearTexture { get; private set; }

        public ClothesViewModel(CharacterViewModel parent, ClothesKind kind)
        {
            CharacterViewModel = parent;
            Kind = kind;
            UpdateKeys();
            SkirtTexture = new ClothesTextureViewModel(this, false);
            UnderwearTexture = new ClothesTextureViewModel(this, true);
        }

        public void UpdateKeys()
        {
            string suit = null;
            switch (Kind)
            {
                case ClothesKind.Uniform:
                    suit = "UNIFORM";
                    break;
                case ClothesKind.Sports:
                    suit = "SPORT";
                    break;
                case ClothesKind.Swimsuit:
                    suit = "SWIMSUIT";
                    break;
                case ClothesKind.Formal:
                    suit = "CLUB";
                    break;
            }
            _keySuitF = String.Format("{0}_SUIT_F", suit);
            _keySuitM = String.Format("{0}_SUIT_M", suit);
            _keyTopColor1 = String.Format("{0}_TOP_1_COLOR", suit);
            _keyTopColor2 = String.Format("{0}_TOP_2_COLOR", suit);
            _keyTopColor3 = String.Format("{0}_TOP_3_COLOR", suit);
            _keyTopColor4 = String.Format("{0}_TOP_4_COLOR", suit);
            _keyBottomColor1 = String.Format("{0}_BOTTOM_1_COLOR", suit);
            _keyBottomColor2 = String.Format("{0}_BOTTOM_2_COLOR", suit);
            _keyUnderwearColor = String.Format("{0}_UNDERWEAR_COLOR", suit);
            _keyIndoorShoesColor = String.Format("{0}_INDOOR_SHOES_COLOR", suit);
            _keyOutdoorShoesColor = String.Format("{0}_OUTDOOR_SHOES_COLOR", suit);
            _keySocksColor = String.Format("{0}_SOCKS_COLOR", suit);
            _keySocksF = String.Format("{0}_SOCKS_F", suit);
            _keySocksM = String.Format("{0}_SOCKS_M", suit);
            _keyInShoesF = String.Format("{0}_INDOOR_SHOES_F", suit);
            _keyInShoesM = String.Format("{0}_INDOOR_SHOES_M", suit);
            _keyOutShoesF = String.Format("{0}_OUTDOOR_SHOES_F", suit);
            _keyOutShoesM = String.Format("{0}_OUTDOOR_SHOES_M", suit);
            _keyShortSkirt = String.Format("{0}_SHORT_SKIRT", suit);

            _keyIsUnderwear = String.Format("{0}_IS_UNDERWEAR", suit);
            _keyIsSkirt = String.Format("{0}_IS_SKIRT", suit);
            _keyIsOnepiece = String.Format("{0}_IS_ONEPIECE", suit);
        }
    }
}