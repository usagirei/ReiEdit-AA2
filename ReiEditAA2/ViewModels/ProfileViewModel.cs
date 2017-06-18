// --------------------------------------------------
// ReiEditAA2 - ProfileViewModel.cs
// --------------------------------------------------

using System;
using System.ComponentModel;
using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.ViewModels
{
    internal class ProfileViewModel : IDisposable, INotifyPropertyChanged
    {
        private const string KEY_CLUB_RANK = "PROFILE_CLUB_RANK";
        private const string KEY_CLUB_VALUE = "PROFILE_CLUB_VALUE";
        private const string KEY_EXPERIENCE_ANAL = "PROFILE_EXPERIENCE_ANAL";
        private const string KEY_EXPERIENCE_VAGINAL = "PROFILE_EXPERIENCE_VAGINAL";
        private const string KEY_FAMILY_NAME = "PROFILE_FAMILY_NAME";
        private const string KEY_FIGHTSTYLE = "PROFILE_FIGHTING_STYLE";
        private const string KEY_FIRST_NAME = "PROFILE_FIRST_NAME";
        private const string KEY_GENDER = "PROFILE_GENDER";
        private const string KEY_INTELLIGENCE = "PROFILE_INTELLIGENCE";
        private const string KEY_INTELLIGENCE_RANK = "PROFILE_INTELLIGENCE_RANK";
        private const string KEY_INTELLIGENCE_VALUE = "PROFILE_INTELLIGENCE_VALUE";
        private const string KEY_ISRAINBOW = "RAINBOW_CARD";
        private const string KEY_ITEMFRIEND = "PROFILE_ITEM_FRIEND";
        private const string KEY_ITEMLOVERS = "PROFILE_ITEM_LOVERS";
        private const string KEY_ITEMSEXUAL = "PROFILE_ITEM_SEXUAL";
        private const string KEY_ORIENTATION = "PROFILE_SEXUAL_ORIENTATION";
        private const string KEY_PERSONALITY = "PROFILE_PERSONALITY";
        private const string KEY_PROFILE = "PROFILE_PROFILE";
        private const string KEY_SOCIABILITY = "PROFILE_SOCIABILITY";
        private const string KEY_STRENGTH = "PROFILE_STRENGTH";
        private const string KEY_STRENGTH_RANK = "PROFILE_STRENGTH_RANK";
        private const string KEY_STRENGTH_VALUE = "PROFILE_STRENGTH_VALUE";
        private const string KEY_VIRTUE = "PROFILE_VIRTUE";
        private const string KEY_VOICE_PITCH = "PROFILE_VOICE_PITCH";

        public DataBlockWrapper AnalExperience
        {
            get { return CharacterViewModel.GetAttribute(KEY_EXPERIENCE_ANAL); }
        }

        public CharacterViewModel CharacterViewModel { get; set; }

        public DataBlockWrapper ClubRank
        {
            get { return CharacterViewModel.GetAttribute(KEY_CLUB_RANK); }
        }

        public DataBlockWrapper ClubValue
        {
            get { return CharacterViewModel.GetAttribute(KEY_CLUB_VALUE); }
        }

        public DataBlockWrapper FamilyName
        {
            get { return CharacterViewModel.GetAttribute(KEY_FAMILY_NAME); }
        }

        public DataBlockWrapper FightStyle
        {
            get { return CharacterViewModel.GetAttribute(KEY_FIGHTSTYLE); }
        }

        public DataBlockWrapper FirstName
        {
            get { return CharacterViewModel.GetAttribute(KEY_FIRST_NAME); }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", FamilyName.Value, FirstName.Value); }
        }

        public DataBlockWrapper Gender
        {
            get { return CharacterViewModel.GetAttribute(KEY_GENDER); }
        }

        public DataBlockWrapper Intelligence
        {
            get { return CharacterViewModel.GetAttribute(KEY_INTELLIGENCE); }
        }

        public DataBlockWrapper IntelligenceRank
        {
            get { return CharacterViewModel.GetAttribute(KEY_INTELLIGENCE_RANK); }
        }

        public DataBlockWrapper IntelligenceValue
        {
            get { return CharacterViewModel.GetAttribute(KEY_INTELLIGENCE_VALUE); }
        }

        public bool IsDisposed { get; set; }

        public DataBlockWrapper IsRainbow
        {
            get { return CharacterViewModel.GetAttribute(KEY_ISRAINBOW); }
        }

        public DataBlockWrapper ItemFriends
        {
            get { return CharacterViewModel.GetAttribute(KEY_ITEMFRIEND); }
        }

        public DataBlockWrapper ItemLovers
        {
            get { return CharacterViewModel.GetAttribute(KEY_ITEMLOVERS); }
        }

        public DataBlockWrapper ItemSexual
        {
            get { return CharacterViewModel.GetAttribute(KEY_ITEMSEXUAL); }
        }

        public DataBlockWrapper Orientation
        {
            get { return CharacterViewModel.GetAttribute(KEY_ORIENTATION); }
        }

        public DataBlockWrapper Personality
        {
            get { return CharacterViewModel.GetAttribute(KEY_PERSONALITY); }
        }

        public DataBlockWrapper Profile
        {
            get { return CharacterViewModel.GetAttribute(KEY_PROFILE); }
        }

        public DataBlockWrapper Sociability
        {
            get { return CharacterViewModel.GetAttribute(KEY_SOCIABILITY); }
        }

        public DataBlockWrapper Strength
        {
            get { return CharacterViewModel.GetAttribute(KEY_STRENGTH); }
        }

        public DataBlockWrapper StrengthRank
        {
            get { return CharacterViewModel.GetAttribute(KEY_STRENGTH_RANK); }
        }

        public DataBlockWrapper StrengthValue
        {
            get { return CharacterViewModel.GetAttribute(KEY_STRENGTH_VALUE); }
        }

        public DataBlockWrapper VaginalExperience
        {
            get { return CharacterViewModel.GetAttribute(KEY_EXPERIENCE_VAGINAL); }
        }

        public DataBlockWrapper Virtue
        {
            get { return CharacterViewModel.GetAttribute(KEY_VIRTUE); }
        }

        public DataBlockWrapper VoicePitch
        {
            get { return CharacterViewModel.GetAttribute(KEY_VOICE_PITCH); }
        }

        public ProfileViewModel(CharacterViewModel parent)
        {
            CharacterViewModel = parent;
            FirstName.PropertyChanged += NameOnPropertyChanged;
            FamilyName.PropertyChanged += NameOnPropertyChanged;
        }

        private void NameOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged("FullName");
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
            if (IsDisposed)
                return;
            FirstName.PropertyChanged -= NameOnPropertyChanged;
            FamilyName.PropertyChanged -= NameOnPropertyChanged;
            IsDisposed = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}