// --------------------------------------------------
// ReiEditAA2 - SaveFileCharacterViewModelProvider.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using AA2Lib;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.ViewModels;
using ReiEditAA2.Views;

namespace ReiEditAA2.Code.CharacterViewModelProviders
{
    internal class SaveFileCharacterViewModelProvider : ICharacterViewModelProvider
    {
        private HeaderEditor _editor;
        private bool _preventFinish;
        private bool _preventWndClose;

        public string SavePath { get; private set; }

        private List<CharacterReference> Characters { get; set; }
        private byte[] FooterBytes { get; set; }

        private DataBlockWrapperBuffer Header { get; set; }
        private byte[] HeaderBytes { get; set; }
        private List<CharacterViewModel> InstanceCache { get; set; }

        private DataBlockWrapper PlayerSeat { get; set; }
        private Timer SaveTimeouTimer { get; set; }

        public SaveFileCharacterViewModelProvider(string savePath)
        {
            InstanceCache = new List<CharacterViewModel>();
            SaveTimeouTimer = new Timer(FlushSave, null, Timeout.Infinite, Timeout.Infinite);
            SavePath = savePath;

            var fileBytes = File.ReadAllBytes(savePath);

            //Header
            XDocument scanDocument = Core.LoadHeaderDataXDocument();

            Header = new DataBlockWrapperBuffer(fileBytes, scanDocument);
            HeaderBytes = new byte[Header.DataBytes.Length];
            Buffer.BlockCopy(Header.DataBytes, 0, HeaderBytes, 0, HeaderBytes.Length);

            //Characters
            int lastChar;
            int numStudents = Header.GetAttribute<int>("HEADER_BOYS") + Header.GetAttribute<int>("HEADER_GIRLS");
            Characters = ReadCharacters(fileBytes, numStudents, out lastChar);

            int footerSize = fileBytes.Length - lastChar;
            FooterBytes = new byte[footerSize];
            Buffer.BlockCopy(fileBytes, lastChar, FooterBytes, 0, footerSize);

            //Player Seat Hax
            var pSeat = new byte[4];
            Buffer.BlockCopy(FooterBytes, FooterBytes.Length - 4, pSeat, 0, 4);
            PlayerSeat = new DataBlockWrapper(pSeat, 0, new SeatDataBlock(), block => Header.HasChanges = true);

            Header.SaveChanges = () =>
            {
                if (!Header.HasChanges)
                    return false;
                Buffer.BlockCopy(Header.DataBytes, 0, HeaderBytes, 0, HeaderBytes.Length);
                Buffer.BlockCopy(PlayerSeat.DataSource, 0, FooterBytes, FooterBytes.Length - 4, 4);
                StartSaveTimeout();
                Header.HasChanges = false;
                return true;
            };
            Header.Reload = () =>
            {
                Buffer.BlockCopy(HeaderBytes, 0, Header.DataBytes, 0, Header.DataBytes.Length);
                Buffer.BlockCopy(FooterBytes, FooterBytes.Length - 4, pSeat, 0, 4);
                foreach (DataBlockWrapper block in Header.Attributes.Values)
                {
                    block.DataSource = Header.DataBytes;
                    block.RaisePropertyChanged(String.Empty);
                }
                PlayerSeat.RaisePropertyChanged(String.Empty);
                Header.HasChanges = false;
            };
        }

        protected virtual void OnCharacterUpdated(CharacterViewModelProviderEventArgs args)
        {
            CharacterProviderDelegate handler = CharacterUpdated;
            if (handler != null)
                handler(this, args);
        }

        private CharacterViewModel CreateViewModel(CharacterReference reference)
        {
            XDocument scanDocument = Core.LoadPlayDataXDocument();

            CharacterFile cf = CharacterFile.Load(reference.CharBytes);

            CharacterViewModel vm = new CharacterViewModel(cf, reference);
            DataBlockWrapperBuffer playData = new DataBlockWrapperBuffer(reference.PlayBytes, scanDocument);

            vm.ExtraData.Add("PLAY_SEAT", reference.Seat);
            vm.ExtraData.Add("PLAY_DATA", playData);

            vm.SaveCommand = new RelayCommand(() => SaveViewModel(vm), () => SaveViewModelCanExecute(vm));
            vm.ReloadCommand = new RelayCommand(() => ReloadViewModel(vm));

            InstanceCache.Add(vm);

            return vm;
        }

        private void FlushSave(object state)
        {
            int backNum = 0;
            string backName;
            while (File.Exists(backName = String.Format("{0}.{1:D3}", SavePath, backNum++)))
            { }

            File.Copy(SavePath, backName);
            FileStream fs = File.Open(SavePath, FileMode.Create);

            fs.Write(HeaderBytes, 0, HeaderBytes.Length);

            foreach (CharacterReference @ref in Characters)
            {
                var seat = BitConverter.GetBytes(@ref.Seat);
                byte gender = (@ref.Female
                    ? (byte) 1
                    : (byte) 0);

                fs.WriteByte(gender);
                fs.Write(seat, 0, 4);
                fs.Write(@ref.CharBytes, 0, @ref.CharBytes.Length);
                fs.Write(@ref.PlayBytes, 0, @ref.PlayBytes.Length);
            }

            fs.Write(FooterBytes, 0, FooterBytes.Length);
            fs.Close();

            _preventFinish = false;
        }

        private List<CharacterReference> ReadCharacters(byte[] saveData, int count, out int stopIndex)
        {
            var references = new List<CharacterReference>();

            XDocument scanDocument = Core.LoadPlayDataXDocument();

            int index = 0;
            byte[] pngBytes = {0x89, 0x50, 0x4e, 0x47}; // ‰PNG
            byte[] endBytes = {0x49, 0x45, 0x4e, 0x44}; // IEND
            while (count > 0)
            {
                //First PNG Mark
                int startIndex = BufferIndexOf(saveData, pngBytes, index);
                if (startIndex == -1)
                    break;
                //Find another PNG, the Thumbnail
                int thumbIndex = BufferIndexOf(saveData, pngBytes, startIndex + 1);
                //And its IEND Mark
                int endIndex = BufferIndexOf(saveData, endBytes, thumbIndex + 1);
                //Play Data index will be after the IEND: [4]Self+[4]Junk+[4]Identifier
                int playIndex = endIndex + 4 + 4 + 4;

                //Seat is 4 Bytes behind the Start
                int playSeat = BitConverter.ToInt32(saveData, startIndex - 4);
                //Another Gender identifier at 5 bytes behind
                bool isFemale = saveData[startIndex - 5] == 1;

                CharacterReference @ref = new CharacterReference();

                // From index until Start of PlayData. Includes Identifier mark
                int charLenght = playIndex - startIndex;
                var charBytes = new byte[charLenght];
                Buffer.BlockCopy(saveData, startIndex, charBytes, 0, charLenght);

                // Scan PlayData Lenght via Loader, TODO: Create a Size Only Scanner, this EATS Memory
                int playLenght;
                // Probable Temporary Fix, release strings and shit
                var temp = Core.LoadDataBlocks(saveData, playIndex, scanDocument, out playLenght);
                temp.OfType<EnumDataBlock>()
                    .ForEach
                    (block =>
                    {
                        if (block is SeatDataBlock)
                            return;
                        block.Enum.Clear();
                    });
                temp.Clear();

                var playBytes = new byte[playLenght];
                Buffer.BlockCopy(saveData, playIndex, playBytes, 0, playLenght);

                @ref.CharBytes = charBytes;
                @ref.PlayBytes = playBytes;
                @ref.Seat = playSeat;
                @ref.Female = isFemale;

                index = playIndex + playLenght;
                count--;

                references.Add(@ref);
            }
            stopIndex = index;
            return references;
        }

        private void ReloadViewModel(CharacterViewModel vm)
        {
            CharacterReference @ref = (CharacterReference) vm.Metadata;

            CharacterViewModel nvm = CreateViewModel(@ref);

            OnCharacterUpdated(new CharacterViewModelProviderEventArgs(nvm, @ref));
        }

        private void SaveViewModel(CharacterViewModel vm)
        {
            CharacterReference @ref = (CharacterReference) vm.Metadata;

            CharacterFile cf = vm.Character;
            DataBlockWrapperBuffer pd = (DataBlockWrapperBuffer) vm.ExtraData["PLAY_DATA"];

            if (cf.HasChanges)
            {
                //[Var]Card Bytes + [3011]Data Bytes + [4]Thumb Lenght + [Var]Thumb Bytes + [4] Data Lenght Mark
                int newCharBytesLen = cf.CardBytes.Length + cf.DataBytes.Length + cf.ThumbBytes.Length + 8;
                var newCharData = new byte[newCharBytesLen];
                MemoryStream ms = new MemoryStream(newCharData);
                // Card Bytes
                ms.Write(cf.CardBytes, 0, cf.CardBytes.Length);
                // Data Bytes
                ms.Write(cf.DataBytes, 0, cf.DataBytes.Length);
                // Thumbnail Lenght
                ms.Write(BitConverter.GetBytes(cf.ThumbBytes.Length), 0, 4);
                // Thumbnail Lenght
                ms.Write(cf.ThumbBytes, 0, cf.ThumbBytes.Length);
                // Identifier Mark
                int lastBytes = cf.DataBytes.Length + cf.ThumbBytes.Length + 8;
                ms.Write(BitConverter.GetBytes(lastBytes), 0, 4);
                ms.Flush();
                @ref.CharBytes = newCharData;
                cf.CardChanges = cf.DataChanges = cf.ThumbChanges = false;
            }

            if (pd.HasChanges)
            {
                var newPlayBytes = new byte[pd.DataBytes.Length];
                Buffer.BlockCopy(pd.DataBytes, 0, newPlayBytes, 0, newPlayBytes.Length);
                @ref.PlayBytes = newPlayBytes;
                pd.HasChanges = false;
            }

            @ref.Female = (byte) vm.Profile.Gender.Value == 1;

            SaveTimeouTimer.Change(1000, Timeout.Infinite);
        }

        private bool SaveViewModelCanExecute(CharacterViewModel vm)
        {
            DataBlockWrapperBuffer pd = (DataBlockWrapperBuffer) vm.ExtraData["PLAY_DATA"];

            return pd.HasChanges || !vm.IsDisposed && vm.Character.HasChanges;
        }

        private void StartSaveTimeout()
        {
            _preventFinish = true;
            SaveTimeouTimer.Change(1000, Timeout.Infinite);
        }

        private static int BufferIndexOf(byte[] buffer, byte[] pattern, int start = 0)
        {
            for (int i = start; i < buffer.Length - pattern.Length; i++)
                if (PatternCheck(buffer, i, pattern))
                    return i;
            return -1;
        }

        private static bool PatternCheck(byte[] buffer, int nOffset, byte[] btPattern)
        {
            for (int x = 0; x < btPattern.Length; x++)
                if (btPattern[x] != buffer[nOffset + x])
                    return false;
            return true;
        }

        public void Dispose()
        {
            SaveTimeouTimer.Dispose();
            InstanceCache.ForEach
            (model =>
            {
                DataBlockWrapperBuffer wb = model.ExtraData["PLAY_DATA"] as DataBlockWrapperBuffer;
                wb.Dispose();
                model.Dispose();
            });
            //throw new NotImplementedException();
        }

        public event CharacterProviderDelegate CharacterAdded;
        public event CharacterProviderDelegate CharacterUpdated;

        public event CharacterLoadedDelegate CharacterLoaded;

        public IEnumerable<CharacterViewModel> GetCharacters()
        {
            return Characters.Select(CreateViewModel);
            //return Characters.Select(reference => reference.Character);
        }

        public Dictionary<string, string> GetSortableProperties()
        {
            return new Dictionary<string, string>
            {
                {"Seat Number", "ExtraData[PLAY_SEAT]"},
                {"Name", "Profile.FullName"},
                {"Gender", "Profile.Gender.Value"},
                {"Personality", "Profile.Personality.Value"},
            };
        }

        public string DefaultSortProperty
        {
            get { return "ExtraData[PLAY_SEAT]"; }
        }

        public ListSortDirection DefaultSortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        public void Initialize(Window window)
        {
            _preventWndClose = true;
            _editor = new HeaderEditor
            {
                SaveHeaderViewModel = new SaveHeaderViewModel(Header, PlayerSeat),
                //Owner = mainWindow,
            };
            _editor.Closing += (sender, args) => args.Cancel = _preventWndClose;
            _editor.Show();
            double newLeft = SystemParameters.PrimaryScreenWidth - _editor.Width;
            double newTop = SystemParameters.PrimaryScreenHeight - _editor.Height;
            _editor.Left = newLeft;
            _editor.Top = newTop;
        }

        public bool Finish()
        {
            if (_preventFinish)
                return false;
            _preventWndClose = false;
            _editor.Close();
            return true;
        }

        public class CharacterReference
        {
            public byte[] CharBytes { get; set; }
            public bool Female { get; set; }

            //public CharacterFile Character { get; set; }

            public byte[] PlayBytes { get; set; }

            //public DataBlockWrapperBuffer PlayData { get; set; }

            public int Seat { get; set; }

            public override string ToString()
            {
                return string.Format("Ref: Seat {0} - [{1} + {2}] Bytes", Seat, CharBytes.Length, PlayBytes.Length);
            }
        }
    }
}