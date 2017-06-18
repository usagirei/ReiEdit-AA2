// --------------------------------------------------
// ReiEditAA2 - SaveHeaderViewModel.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.Code.CharacterViewModelProviders;

namespace ReiEditAA2.ViewModels
{
    internal class SaveHeaderViewModel
    {
        private const string KEY_ACADEMY = "HEADER_ACADEMY";

        private const string KEY_BOYS = "HEADER_BOYS";
        private const string KEY_CLASS = "HEADER_CLASS";

        private const string KEY_CLUB = "HEADER_CLUBS[{0}]";
        private const string KEY_CLUB_NAME = "CLUB_NAME";
        private const string KEY_CLUB_TYPE = "CLUB_TYPE";

        private const string KEY_GAMEDAYS = "HEADER_GAMEDAYS";
        private const string KEY_GAMETIME = "HEADER_GAMETIME";
        private const string KEY_GAMEWEEKDAY = "HEADER_GAMEWEEKDAY";
        private const string KEY_GIRLS = "HEADER_GIRLS";

        private const string KEY_HOURS = "HEADER_HOURS";
        private const string KEY_MINS = "HEADER_MINS";
        private const string KEY_SECS = "HEADER_SECS";
        private const string KEY_VERSION = "HEADER_VERSION";
        private const string KEY_YEAR = "HEADER_YEAR";

        public DataBlockWrapper Academy
        {
            get { return SaveHeader.Attributes[KEY_ACADEMY]; }
        }

        public DataBlockWrapper Class
        {
            get { return SaveHeader.Attributes[KEY_CLASS]; }
        }

        public IEnumerable<ClubData> Clubs
        {
            get
            {
                return Enumerable.Range(0, 8)
                    .Select(i => String.Format(KEY_CLUB, i))
                    .Select
                    (s => new ClubData
                    {
                        ClubName = SaveHeader.Attributes[s + "." + KEY_CLUB_NAME],
                        ClubType = SaveHeader.Attributes[s + "." + KEY_CLUB_TYPE],
                    });
            }
        }

        public DataBlockWrapper GameDays
        {
            get { return SaveHeader.Attributes[KEY_GAMEDAYS]; }
        }

        public DataBlockWrapper GameWeekDay
        {
            get { return SaveHeader.Attributes[KEY_GAMEWEEKDAY]; }
        }

        public DataBlockWrapper PlayHours
        {
            get { return SaveHeader.Attributes[KEY_HOURS]; }
        }

        public DataBlockWrapper PlayMins
        {
            get { return SaveHeader.Attributes[KEY_MINS]; }
        }

        public DataBlockWrapper PlaySeconds
        {
            get { return SaveHeader.Attributes[KEY_SECS]; }
        }

        public DataBlockWrapper PlayerSeat { get; set; }

        public ICommand ReloadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public DataBlockWrapperBuffer SaveHeader { get; private set; }

        public DataBlockWrapper Version
        {
            get { return SaveHeader.Attributes[KEY_VERSION]; }
        }

        public DataBlockWrapper Year
        {
            get { return SaveHeader.Attributes[KEY_YEAR]; }
        }

        public SaveHeaderViewModel(DataBlockWrapperBuffer header, DataBlockWrapper pSeat)
        {
            PlayerSeat = pSeat;
            SaveCommand = new RelayCommand(() =>
                {
                    if (SaveHeader.SaveChanges != null)
                        SaveHeader.SaveChanges();
                },
                () => SaveHeader.HasChanges);
            ReloadCommand = new RelayCommand(() =>
                {
                    if (SaveHeader.Reload != null)
                        SaveHeader.Reload();
                },
                () => SaveHeader.HasChanges);
            SaveHeader = header;
        }

        internal class ClubData
        {
            public DataBlockWrapper ClubName { get; set; }
            public DataBlockWrapper ClubType { get; set; }
        }
    }
}