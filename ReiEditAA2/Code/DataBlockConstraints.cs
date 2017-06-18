// --------------------------------------------------
// ReiEditAA2 - DataBlockConstraints.cs
// --------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq;
using AA2Lib;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code
{
    internal class DataBlockConstraints : IDisposable
    {
        private bool _cancelBreast;
        private bool _cancelClub;
        private bool _cancelFaceType;
        private bool _cancelGlasses;
        private bool _cancelInt;
        private bool _cancelShoes;
        private bool _cancelSocks;
        private bool _cancelStr;
        private bool _cancelSuit;

        public CharacterViewModel CharacterViewModel { get; private set; }

        public DataBlockConstraints(CharacterViewModel parent)
        {
            CharacterViewModel = parent;
            //Intelligence Base<->Rank<->Value
            CharacterViewModel.Profile.Intelligence.PropertyChanged += IntelligenceOnPropertyChanged;
            CharacterViewModel.Profile.IntelligenceRank.PropertyChanged += IntelligenceRankOnPropertyChanged;
            CharacterViewModel.Profile.IntelligenceValue.PropertyChanged += IntelligenceValueOnPropertyChanged;
            //Strength Base<->Rank<->Value
            CharacterViewModel.Profile.Strength.PropertyChanged += StrengthOnPropertyChanged;
            CharacterViewModel.Profile.StrengthRank.PropertyChanged += StrengthRankOnPropertyChanged;
            CharacterViewModel.Profile.StrengthValue.PropertyChanged += StrengthValueOnPropertyChanged;
            //Club Rank<->Value
            CharacterViewModel.Profile.ClubRank.PropertyChanged += ClubRankOnPropertyChanged;
            CharacterViewModel.Profile.ClubValue.PropertyChanged += ClubValueOnPropertyChanged;
            //Breasts Size<->Rank
            CharacterViewModel.Body.BodyPart.BreastSize.PropertyChanged += BreastSizeOnPropertyChanged;
            CharacterViewModel.Body.BodyPart.BreastRank.PropertyChanged += BreastRankOnPropertyChanged;
            //Glasses Type<->HasGlasses
            CharacterViewModel.Body.FacePart.Glasses.PropertyChanged += GlassesOnPropertyChanged;
            CharacterViewModel.Body.FacePart.GlassesType.PropertyChanged += GlassesTypeOnPropertyChanged;
            //FaceType Male<->Female
            CharacterViewModel.Body.FacePart.FaceTypeF.PropertyChanged += FaceTypeFOnPropertyChanged;
            CharacterViewModel.Body.FacePart.FaceTypeM.PropertyChanged += FaceTypeMOnPropertyChanged;

            //IrisHighlight TextureName>->Flag
            CharacterViewModel.Body.FacePart.ExternalIrisTextureName.PropertyChanged +=
                ExternalIrisTextureNameOnPropertyChanged;
            CharacterViewModel.Body.FacePart.ExternalHighlightTextureName.PropertyChanged +=
                ExternalHighlightTextureNameOnPropertyChanged;

            /*
            //Suits Suit>->IsXYZ
            CharacterViewModel.UniformSuit.Suit.PropertyChanged += SuitOnPropertyChanged;
            CharacterViewModel.SportsSuit.Suit.PropertyChanged += SuitOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.Suit.PropertyChanged += SuitOnPropertyChanged;
            CharacterViewModel.ClubSuit.Suit.PropertyChanged += SuitOnPropertyChanged;
            */

            //Gender Related Locks
            CharacterViewModel.Profile.Gender.PropertyChanged += GenderOnPropertyChanged;

            //Socks  Male<->Female
            CharacterViewModel.ClubSuit.SocksF.PropertyChanged += SocksFOnPropertyChanged;
            CharacterViewModel.ClubSuit.SocksM.PropertyChanged += SocksMOnPropertyChanged;

            CharacterViewModel.SportsSuit.SocksF.PropertyChanged += SocksFOnPropertyChanged;
            CharacterViewModel.SportsSuit.SocksM.PropertyChanged += SocksMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.SocksF.PropertyChanged += SocksFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.SocksM.PropertyChanged += SocksMOnPropertyChanged;

            CharacterViewModel.UniformSuit.SocksF.PropertyChanged += SocksFOnPropertyChanged;
            CharacterViewModel.UniformSuit.SocksM.PropertyChanged += SocksMOnPropertyChanged;

            //Suit  Male<->Female
            CharacterViewModel.ClubSuit.SuitF.PropertyChanged += SuitFOnPropertyChanged;
            CharacterViewModel.ClubSuit.SuitM.PropertyChanged += SuitMOnPropertyChanged;

            CharacterViewModel.SportsSuit.SuitF.PropertyChanged += SuitFOnPropertyChanged;
            CharacterViewModel.SportsSuit.SuitM.PropertyChanged += SuitMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.SuitF.PropertyChanged += SuitFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.SuitM.PropertyChanged += SuitMOnPropertyChanged;

            CharacterViewModel.UniformSuit.SuitF.PropertyChanged += SuitFOnPropertyChanged;
            CharacterViewModel.UniformSuit.SuitM.PropertyChanged += SuitMOnPropertyChanged;

            //InShoes  Male<->Female
            CharacterViewModel.ClubSuit.IndoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.ClubSuit.IndoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            CharacterViewModel.SportsSuit.IndoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.SportsSuit.IndoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.IndoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.IndoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            CharacterViewModel.UniformSuit.IndoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.UniformSuit.IndoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            //OutShoes  Male<->Female
            CharacterViewModel.ClubSuit.OutdoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.ClubSuit.OutdoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            CharacterViewModel.SportsSuit.OutdoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.SportsSuit.OutdoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.OutdoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.OutdoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            CharacterViewModel.UniformSuit.OutdoorShoesF.PropertyChanged += ShoesFOnPropertyChanged;
            CharacterViewModel.UniformSuit.OutdoorShoesM.PropertyChanged += ShoesMOnPropertyChanged;

            //HairFlips
            CharacterViewModel.Body.HairPart.FrontHair.PropertyChanged += FrontHairOnPropertyChanged;
            CharacterViewModel.Body.HairPart.BackHair.PropertyChanged += BackHairOnPropertyChanged;
            CharacterViewModel.Body.HairPart.ExtensionHair.PropertyChanged += ExtensionHairOnPropertyChanged;
            CharacterViewModel.Body.HairPart.SideHair.PropertyChanged += SideHairOnPropertyChanged;
        }

        private void BackHairOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            DataBlockWrapper hair = CharacterViewModel.Body.HairPart.BackHair;
            DataBlockWrapper flip = CharacterViewModel.Body.HairPart.BackHairFlip;
            CharacterHairProvider.CharacterHair meta = CharacterHairProvider.BackHairs.FirstOrDefault
                (h => h.Id == (byte) hair.Value);
            if (meta == null)
                return;
            flip.Value = (bool) flip.Value & meta.Flippable;
        }

        private void BreastRankOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelBreast || args.PropertyName != "Value")
                return;
            byte br = (byte) CharacterViewModel.Body.BodyPart.BreastRank.Value;
            _cancelBreast = true;
            switch (br)
            {
                case 0:

                    CharacterViewModel.Body.BodyPart.BreastSize.Value = (byte) 20;
                    break;
                case 1:
                    CharacterViewModel.Body.BodyPart.BreastSize.Value = (byte) 50;
                    break;
                case 2:
                    CharacterViewModel.Body.BodyPart.BreastSize.Value = (byte) 80;
                    break;
            }
            _cancelBreast = false;
        }

        private void BreastSizeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelBreast || args.PropertyName != "Value")
                return;
            _cancelBreast = true;
            byte bs = (byte) CharacterViewModel.Body.BodyPart.BreastSize.Value;
            if (bs <= 30)
            {
                CharacterViewModel.Body.BodyPart.BreastRank.Value = (byte) 0;
            }
            else if (bs <= 69)
            {
                CharacterViewModel.Body.BodyPart.BreastRank.Value = (byte) 1;
            }
            else
            {
                CharacterViewModel.Body.BodyPart.BreastRank.Value = (byte) 2;
            }
            _cancelBreast = false;
        }

        private void ClubRankOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelClub || args.PropertyName != "Value")
                return;
            _cancelClub = true;
            //
            int value, @base;
            byte rank = (byte) CharacterViewModel.Profile.ClubRank.Value;
            ReseedFromRank(rank, out @base, out value);
            CharacterViewModel.Profile.ClubValue.Value = value;
            //
            _cancelClub = false;
        }

        private void ClubValueOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelClub || args.PropertyName != "Value")
                return;
            _cancelClub = true;
            //
            int value = (int) CharacterViewModel.Profile.ClubValue.Value;
            int @base;
            byte rank;
            ReseedFromValue(value, out rank, out @base);
            CharacterViewModel.Profile.ClubRank.Value = rank;
            //
            _cancelClub = false;
        }

        private void ExtensionHairOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            DataBlockWrapper hair = CharacterViewModel.Body.HairPart.ExtensionHair;
            DataBlockWrapper flip = CharacterViewModel.Body.HairPart.ExtensionHairFlip;
            CharacterHairProvider.CharacterHair meta = CharacterHairProvider.ExtensionHairs.FirstOrDefault
                (h => h.Id == (byte) hair.Value);
            if (meta == null)
                return;
            flip.Value = (bool) flip.Value & meta.Flippable;
        }

        private void ExternalHighlightTextureNameOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            string newName = CharacterViewModel.Body.FacePart.ExternalHighlightTextureName.Value as string;
            CharacterViewModel.Body.FacePart.ExternalHighlightTexture.Value = !String.IsNullOrWhiteSpace(newName);
        }

        private void ExternalIrisTextureNameOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            string newName = CharacterViewModel.Body.FacePart.ExternalIrisTextureName.Value as string;
            CharacterViewModel.Body.FacePart.ExternalIrisTexture.Value = !String.IsNullOrWhiteSpace(newName);
        }

        private void FaceTypeFOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelFaceType || args.PropertyName != "Value")
                return;
            _cancelFaceType = true;
            CharacterViewModel.Body.FacePart.FaceTypeM.RaisePropertyChanged(String.Empty);
            _cancelFaceType = false;
        }

        private void FaceTypeMOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelFaceType || args.PropertyName != "Value")
                return;
            _cancelFaceType = true;
            CharacterViewModel.Body.FacePart.FaceTypeF.RaisePropertyChanged(String.Empty);
            _cancelFaceType = false;
        }

        private void FrontHairOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            DataBlockWrapper hair = CharacterViewModel.Body.HairPart.FrontHair;
            DataBlockWrapper flip = CharacterViewModel.Body.HairPart.FrontHairFlip;
            CharacterHairProvider.CharacterHair meta = CharacterHairProvider.FrontHairs.FirstOrDefault
                (h => h.Id == (byte) hair.Value);
            if (meta == null)
                return;
            flip.Value = (bool) flip.Value & meta.Flippable;
        }

        private void GenderOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            bool isMale = CharacterViewModel.Profile.Gender.Value.Equals((byte) 0);
            if (args.PropertyName == "Value" && isMale)
            {
                //CharacterViewModel.Body.BodyPart.BreastRank.Value = (byte) 1;
                byte val = (byte) CharacterViewModel.Body.FacePart.FaceType.Value;
                byte max = 3;
                CharacterViewModel.Body.FacePart.FaceType.Value = Math.Min(val, max);
            }
        }

        private void GlassesOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelGlasses || args.PropertyName != "Value")
                return;
            _cancelGlasses = true;
            bool glass = (bool) CharacterViewModel.Body.FacePart.Glasses.Value;
            CharacterViewModel.Body.FacePart.GlassesType.Value = glass
                ? (byte) 1
                : (byte) 0;
            _cancelGlasses = false;
        }

        private void GlassesTypeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelGlasses || args.PropertyName != "Value")
                return;
            _cancelGlasses = true;
            byte type = (byte) CharacterViewModel.Body.FacePart.GlassesType.Value;
            CharacterViewModel.Body.FacePart.Glasses.Value = type > 0;
            _cancelGlasses = false;
        }

        private void IntelligenceOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelInt || args.PropertyName != "Value")
                return;
            _cancelInt = true;
            //
            byte @base = (byte) CharacterViewModel.Profile.Intelligence.Value;
            byte rank;
            int value;
            ReseedFromBase(@base, out rank, out value);
            CharacterViewModel.Profile.IntelligenceRank.Value = rank;
            CharacterViewModel.Profile.IntelligenceValue.Value = value;
            //
            _cancelInt = false;
        }

        private void IntelligenceRankOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelInt || args.PropertyName != "Value")
                return;
            _cancelInt = true;
            //
            byte rank = (byte) CharacterViewModel.Profile.IntelligenceRank.Value;
            int @base, value;
            ReseedFromRank(rank, out @base, out value);
            CharacterViewModel.Profile.IntelligenceValue.Value = value;
            CharacterViewModel.Profile.Intelligence.Value = @base;
            //
            _cancelInt = false;
        }

        private void IntelligenceValueOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelInt || args.PropertyName != "Value")
                return;
            _cancelInt = true;
            //
            int @base, value = (int) CharacterViewModel.Profile.IntelligenceValue.Value;
            byte rank;
            ReseedFromValue(value, out rank, out @base);
            CharacterViewModel.Profile.IntelligenceRank.Value = rank;
            CharacterViewModel.Profile.Intelligence.Value = @base;
            //
            _cancelInt = false;
        }

        private void ShoesFOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelShoes || args.PropertyName != "Value")
                return;
            _cancelShoes = true;
            CharacterViewModel.ClubSuit.IndoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.IndoorShoesM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.IndoorShoesM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.IndoorShoesM.RaisePropertyChanged(String.Empty);

            CharacterViewModel.ClubSuit.OutdoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.OutdoorShoesM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.OutdoorShoesM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.OutdoorShoesM.RaisePropertyChanged(String.Empty);
            _cancelShoes = false;
        }

        private void ShoesMOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelShoes || args.PropertyName != "Value")
                return;
            _cancelShoes = true;
            CharacterViewModel.ClubSuit.IndoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.IndoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.IndoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.IndoorShoesF.RaisePropertyChanged(String.Empty);

            CharacterViewModel.ClubSuit.OutdoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.OutdoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.OutdoorShoesF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.OutdoorShoesF.RaisePropertyChanged(String.Empty);
            _cancelShoes = false;
        }

        private void SideHairOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            DataBlockWrapper hair = CharacterViewModel.Body.HairPart.SideHair;
            DataBlockWrapper flip = CharacterViewModel.Body.HairPart.SideHairFlip;
            CharacterHairProvider.CharacterHair meta = CharacterHairProvider.SideHairs.FirstOrDefault
                (h => h.Id == (byte) hair.Value);
            if (meta == null)
                return;
            flip.Value = (bool) flip.Value & meta.Flippable;
        }

        private void SocksFOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelSocks || args.PropertyName != "Value")
                return;
            _cancelSocks = true;
            CharacterViewModel.ClubSuit.SocksM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.SocksM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.SocksM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.SocksM.RaisePropertyChanged(String.Empty);
            _cancelSocks = false;
        }

        private void SocksMOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelSocks || args.PropertyName != "Value")
                return;
            _cancelSocks = true;
            CharacterViewModel.ClubSuit.SocksF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.SocksF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.SocksF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.SocksF.RaisePropertyChanged(String.Empty);
            _cancelSocks = false;
        }

        private void StrengthOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelStr || args.PropertyName != "Value")
                return;
            _cancelStr = true;
            //
            byte @base = (byte) CharacterViewModel.Profile.Strength.Value;
            byte rank;
            int value;
            ReseedFromBase(@base, out rank, out value);
            CharacterViewModel.Profile.StrengthRank.Value = rank;
            CharacterViewModel.Profile.StrengthValue.Value = value;
            //
            _cancelStr = false;
        }

        private void StrengthRankOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelStr || args.PropertyName != "Value")
                return;
            _cancelStr = true;
            //
            byte rank = (byte) CharacterViewModel.Profile.StrengthRank.Value;
            int @base, value;
            ReseedFromRank(rank, out @base, out value);
            CharacterViewModel.Profile.StrengthValue.Value = value;
            CharacterViewModel.Profile.Strength.Value = @base;
            //
            _cancelStr = false;
        }

        private void StrengthValueOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelStr || args.PropertyName != "Value")
                return;
            _cancelStr = _cancelStr = true;
            //
            int @base, value = (int) CharacterViewModel.Profile.StrengthValue.Value;
            byte rank;
            ReseedFromValue(value, out rank, out @base);
            CharacterViewModel.Profile.StrengthRank.Value = rank;
            CharacterViewModel.Profile.Strength.Value = @base;
            //
            _cancelStr = _cancelStr = false;
        }

        private void SuitFOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelSuit || args.PropertyName != "Value")
                return;
            _cancelSuit = true;
            CharacterViewModel.ClubSuit.SuitM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.SuitM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.SuitM.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.SuitM.RaisePropertyChanged(String.Empty);
            _cancelSuit = false;
        }

        private void SuitMOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_cancelSuit || args.PropertyName != "Value")
                return;
            _cancelSuit = true;
            CharacterViewModel.ClubSuit.SuitF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SportsSuit.SuitF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.SwimsuitSuit.SuitF.RaisePropertyChanged(String.Empty);
            CharacterViewModel.UniformSuit.SuitF.RaisePropertyChanged(String.Empty);
            _cancelSuit = false;
        }

        public void ReseedFromBase(int @base, out byte rank, out int value)
        {
            byte @baseValue = (byte) @base;
            bool @boost = Core.Random.Next(0, 100) >= 50;
            int @offset = Core.Random.Next(-50, 50);
            byte @rankValue = 0;
            switch (@baseValue)
            {
                case 1:
                    @rankValue = (byte) (2 - (@boost
                                             ? 0
                                             : 1));
                    break;
                case 2:
                    @rankValue = (byte) (4 - (@boost
                                             ? 0
                                             : 1));
                    break;
                case 3:
                    @rankValue = (byte) (6 - (@boost
                                             ? 0
                                             : 1));
                    break;
                case 4:
                    @rankValue = 7;
                    break;
                case 5:
                    @rankValue = 8;
                    break;
            }
            int @valueValue = (@rankValue * 100) + 150 + offset;
            rank = @rankValue;
            value = @valueValue;
        }

        public void ReseedFromRank(byte rank, out int @base, out int value)
        {
            @base = 0;
            switch (rank)
            {
                case 8:
                    @base = 5;
                    break;
                case 7:
                    @base = 4;
                    break;
                case 6:
                case 5:
                    @base = 3;
                    break;
                case 4:
                case 3:
                    @base = 2;
                    break;
                case 2:
                case 1:
                    @base = 1;
                    break;
            }
            int @offset = Core.Random.Next(-50, 50);
            value = (rank * 100) + 150 + offset;
        }

        public void ReseedFromValue(int value, out byte rank, out int @base)
        {
            rank = (byte) (Math.Max(value - 150, 0) / 100);
            @base = 0;
            switch (rank)
            {
                case 8:
                    @base = 5;
                    break;
                case 7:
                    @base = 4;
                    break;
                case 6:
                case 5:
                    @base = 3;
                    break;
                case 4:
                case 3:
                    @base = 2;
                    break;
                case 2:
                case 1:
                    @base = 1;
                    break;
            }
        }

        private void SuitOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "Value")
                return;
            string[] data = {"IS_ONEPIECE", "IS_UNDERWEAR", "IS_SKIRT"};
            bool[][] bData =
            {
                //Uniform
                new[] {false, true, true},
                //Sport
                new[] {false, true, false},
                //Swimsuit
                new[] {true, false, false},
                //Formal
                new[] {false, true, true},
                //Blazer
                new[] {false, true, true},
                //Naked
                new[] {false, false, false},
                //Undies
                new[] {false, true, false}
            };
            DataBlockWrapper wrapper = (DataBlockWrapper) sender;
            string srcSuit = wrapper.Key.Remove(wrapper.Key.IndexOf('_'));
            int srcSuitValue = (int) wrapper.Value;
            CharacterFile @char = CharacterViewModel.Character;
            for (int prop = 0; prop < 3; prop++)
            {
                string targetKey = String.Format("{0}_{1}", srcSuit, data[prop]);
                bool targetValue;
                try
                {
                    targetValue = bData[srcSuitValue][prop];
                }
                catch
                {
                    targetValue = false;
                }
                if (@char.CharAttributes.ContainsKey(targetKey))
                    @char.CharAttributes[targetKey].Value = targetValue;
            }
        }

        public void Dispose()
        {
            CharacterViewModel.Profile.Intelligence.PropertyChanged -= IntelligenceOnPropertyChanged;
            CharacterViewModel.Profile.IntelligenceRank.PropertyChanged -= IntelligenceRankOnPropertyChanged;
            CharacterViewModel.Profile.IntelligenceValue.PropertyChanged -= IntelligenceValueOnPropertyChanged;
            //Strength Base<->Rank<->Value
            CharacterViewModel.Profile.Strength.PropertyChanged -= StrengthOnPropertyChanged;
            CharacterViewModel.Profile.StrengthRank.PropertyChanged -= StrengthRankOnPropertyChanged;
            CharacterViewModel.Profile.StrengthValue.PropertyChanged -= StrengthValueOnPropertyChanged;
            //Club Rank<->Value
            CharacterViewModel.Profile.ClubRank.PropertyChanged -= ClubRankOnPropertyChanged;
            CharacterViewModel.Profile.ClubValue.PropertyChanged -= ClubValueOnPropertyChanged;
            //Breasts Size<->Rank
            CharacterViewModel.Body.BodyPart.BreastSize.PropertyChanged -= BreastSizeOnPropertyChanged;
            CharacterViewModel.Body.BodyPart.BreastRank.PropertyChanged -= BreastRankOnPropertyChanged;
            //Glasses Type<->HasGlasses
            CharacterViewModel.Body.FacePart.Glasses.PropertyChanged -= GlassesOnPropertyChanged;
            CharacterViewModel.Body.FacePart.GlassesType.PropertyChanged -= GlassesTypeOnPropertyChanged;
            //FaceType Male<->Female
            CharacterViewModel.Body.FacePart.FaceTypeF.PropertyChanged -= FaceTypeFOnPropertyChanged;
            CharacterViewModel.Body.FacePart.FaceTypeM.PropertyChanged -= FaceTypeMOnPropertyChanged;

            //IrisHighlight TextureName>->Flag
            CharacterViewModel.Body.FacePart.ExternalIrisTextureName.PropertyChanged -=
                ExternalIrisTextureNameOnPropertyChanged;
            CharacterViewModel.Body.FacePart.ExternalHighlightTextureName.PropertyChanged -=
                ExternalHighlightTextureNameOnPropertyChanged;

            /*
            //Suits Suit>->IsXYZ
            CharacterViewModel.UniformSuit.Suit.PropertyChanged -= SuitOnPropertyChanged;
            CharacterViewModel.SportsSuit.Suit.PropertyChanged -= SuitOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.Suit.PropertyChanged -= SuitOnPropertyChanged;
            CharacterViewModel.ClubSuit.Suit.PropertyChanged -= SuitOnPropertyChanged;
            */

            //Gender Related Locks
            CharacterViewModel.Profile.Gender.PropertyChanged -= GenderOnPropertyChanged;

            //Socks  Male<->Female
            CharacterViewModel.ClubSuit.SocksF.PropertyChanged -= SocksFOnPropertyChanged;
            CharacterViewModel.ClubSuit.SocksM.PropertyChanged -= SocksMOnPropertyChanged;

            CharacterViewModel.SportsSuit.SocksF.PropertyChanged -= SocksFOnPropertyChanged;
            CharacterViewModel.SportsSuit.SocksM.PropertyChanged -= SocksMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.SocksF.PropertyChanged -= SocksFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.SocksM.PropertyChanged -= SocksMOnPropertyChanged;

            CharacterViewModel.UniformSuit.SocksF.PropertyChanged -= SocksFOnPropertyChanged;
            CharacterViewModel.UniformSuit.SocksM.PropertyChanged -= SocksMOnPropertyChanged;

            //Suit  Male<->Female
            CharacterViewModel.ClubSuit.SuitF.PropertyChanged -= SuitFOnPropertyChanged;
            CharacterViewModel.ClubSuit.SuitM.PropertyChanged -= SuitMOnPropertyChanged;

            CharacterViewModel.SportsSuit.SuitF.PropertyChanged -= SuitFOnPropertyChanged;
            CharacterViewModel.SportsSuit.SuitM.PropertyChanged -= SuitMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.SuitF.PropertyChanged -= SuitFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.SuitM.PropertyChanged -= SuitMOnPropertyChanged;

            CharacterViewModel.UniformSuit.SuitF.PropertyChanged -= SuitFOnPropertyChanged;
            CharacterViewModel.UniformSuit.SuitM.PropertyChanged -= SuitMOnPropertyChanged;

            //InShoes  Male<->Female
            CharacterViewModel.ClubSuit.IndoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.ClubSuit.IndoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            CharacterViewModel.SportsSuit.IndoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.SportsSuit.IndoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.IndoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.IndoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            CharacterViewModel.UniformSuit.IndoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.UniformSuit.IndoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            //OutShoes  Male<->Female
            CharacterViewModel.ClubSuit.OutdoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.ClubSuit.OutdoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            CharacterViewModel.SportsSuit.OutdoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.SportsSuit.OutdoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            CharacterViewModel.SwimsuitSuit.OutdoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.SwimsuitSuit.OutdoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            CharacterViewModel.UniformSuit.OutdoorShoesF.PropertyChanged -= ShoesFOnPropertyChanged;
            CharacterViewModel.UniformSuit.OutdoorShoesM.PropertyChanged -= ShoesMOnPropertyChanged;

            //HairFlips
            CharacterViewModel.Body.HairPart.FrontHair.PropertyChanged -= FrontHairOnPropertyChanged;
            CharacterViewModel.Body.HairPart.BackHair.PropertyChanged -= BackHairOnPropertyChanged;
            CharacterViewModel.Body.HairPart.ExtensionHair.PropertyChanged -= ExtensionHairOnPropertyChanged;
            CharacterViewModel.Body.HairPart.SideHair.PropertyChanged -= SideHairOnPropertyChanged;
        }
    }
}