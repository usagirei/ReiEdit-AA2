// --------------------------------------------------
// AA2Lib - CharacterFile.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using AA2Lib.Code.DataBlocks;

namespace AA2Lib.Code
{
    public class CharacterFile : INotifyPropertyChanged, IDisposable
    {
        private byte[] _cardBytes;

        private bool _cardChanged;
        private BitmapImage _cardImage;
        private Dictionary<string, DataBlockWrapper> _charAttributes;
        private byte[] _dataBytes;
        private bool _dataChanged;
        private byte[] _thumbBytes;
        private bool _thumbChanged;
        private BitmapImage _thumbImage;

        public object this[string key]
        {
            get { return CharAttributes[key].Value; }
            set
            {
                CharAttributes[key].Value = value;
                DataChanges = true;
                RaiseIndexerChange(key);
            }
        }

        public byte[] CardBytes
        {
            get { return _cardBytes; }
            set
            {
                _cardBytes = value;
                _cardImage = null;
                CardChanges = false;
                OnPropertyChanged("CardImage");
            }
        }

        public bool CardChanges
        {
            get { return _cardChanged; }
            set
            {
                _cardChanged = value;
                OnPropertyChanged("CardChanges");
            }
        }

        public BitmapImage CardImage
        {
            get
            {
                if (IsDisposed)
                    return null;
                if (_cardImage != null)
                    return _cardImage;
                try
                {
                    using (MemoryStream ms = new MemoryStream(CardBytes, 0, CardBytes.Length))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        _cardImage = bitmap;
                    }
                }
                catch
                {
                    return null;
                }

                return _cardImage;
            }
        }

        public Dictionary<string, DataBlockWrapper> CharAttributes
        {
            get { return _charAttributes ?? (_charAttributes = InitCharAttributes()); }
        }

        public byte[] DataBytes
        {
            get { return _dataBytes; }
            set
            {
                _dataBytes = value;
                DataChanges = false;
                if (_charAttributes != null)
                    UpdateCharAttributes();
                OnPropertyChanged(String.Empty);
            }
        }

        public bool DataChanges
        {
            get { return _dataChanged; }
            set
            {
                _dataChanged = value;
                OnPropertyChanged("DataChanges");
            }
        }

        public bool HasChanges
        {
            get { return _dataChanged || _cardChanged || _thumbChanged; }
        }

        public bool IsDisposed { get; set; }

        public byte[] ThumbBytes
        {
            get { return _thumbBytes; }
            set
            {
                _thumbBytes = value;
                _thumbImage = null;
                ThumbChanges = false;
                OnPropertyChanged("ThumbImage");
            }
        }

        public bool ThumbChanges
        {
            get { return _thumbChanged; }
            set
            {
                _thumbChanged = value;
                OnPropertyChanged("ThumbChanges");
            }
        }

        public BitmapImage ThumbImage
        {
            get
            {
                if (IsDisposed)
                    return null;
                if (_thumbImage != null)
                    return _thumbImage;
                try
                {
                    using (MemoryStream ms = new MemoryStream(ThumbBytes, 0, ThumbBytes.Length))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        _thumbImage = bitmap;
                    }
                }
                catch
                {
                    return null;
                }

                return _thumbImage;
            }
        }

        public T GetAttribute<T>(string key)
        {
            return (T) this[key];
        }

        public string GetEnumValue(string key)
        {
            return CharAttributes[key].EnumValue;
        }

        public void RaiseIndexerChange(string name)
        {
            OnPropertyChanged("Item[]");
            OnPropertyChanged(string.Format("Item[{0}]", name));
        }

        public void SetCard(byte[] pngData)
        {
            _cardBytes = new byte[pngData.Length];
            Buffer.BlockCopy(pngData, 0, _cardBytes, 0, _cardBytes.Length);
            _cardChanged = true;
            _cardImage = null;
            OnPropertyChanged("CardImage");
        }

        public void SetThumbnail(byte[] pngData)
        {
            _thumbBytes = new byte[pngData.Length];
            Buffer.BlockCopy(pngData, 0, _thumbBytes, 0, _thumbBytes.Length);
            _thumbChanged = true;
            _thumbImage = null;
            OnPropertyChanged("ThumbImage");
        }

        public static CharacterFile Load(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                CharacterFile cf = Load(fs);
                fs.Close();
                return cf;
            }
            catch
            {
                return null;
            }
        }

        public static CharacterFile Load(byte[] rawData)
        {
            try
            {
                int dataIdentifier = (rawData.Length < 3015)
                    ? 0
                    : BitConverter.ToInt32(rawData, rawData.Length - 4);

                const int cardOffset = 0;
                const int dataLenght = 3011;

                int dataOffset = rawData.Length - dataIdentifier;
                int thumbOffset = dataOffset + dataLenght + 4;

                int cardLenght = dataOffset;
                int thumbLenght = BitConverter.ToInt32(rawData, thumbOffset - 4);

                CharacterFile file = new CharacterFile();

                file.CardBytes = new byte[cardLenght];
                Buffer.BlockCopy(rawData, cardOffset, file.CardBytes, 0, file.CardBytes.Length);

                file.ThumbBytes = new byte[thumbLenght];
                Buffer.BlockCopy(rawData, thumbOffset, file.ThumbBytes, 0, file.ThumbBytes.Length);

                file.DataBytes = new byte[dataLenght];
                Buffer.BlockCopy(rawData, dataOffset, file.DataBytes, 0, file.DataBytes.Length);

                int version = BitConverter.ToInt32(file.DataBytes, 16);

                bool thousand = version <= 101;
                return thousand
                    ? null
                    : file;
            }
            catch
            {
                return null;
            }
        }

        public static CharacterFile Load(Stream stream)
        {
            try
            {
                var data = new byte[stream.Length];
                int pos = 0;
                while (stream.Length - stream.Position > 0)
                {
                    const int buffer = 1024 * 16;
                    int toRead = (int) Math.Min(buffer, stream.Length - stream.Position);
                    pos += stream.Read(data, pos, toRead);
                }
                return Load(data);
            }
            catch
            {
                return null;
            }
        }

        //[CallerMemberName]  Removed

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private Dictionary<string, DataBlockWrapper> InitCharAttributes()
        {
            int dataLen;
            XDocument scanDocument = Core.LoadCharDataXDocument();
            var blocks = Core.LoadDataBlocks(DataBytes, 0, scanDocument, out dataLen);

            var wrappers = blocks.Select(block => new DataBlockWrapper(DataBytes, 0, block, WriteCallback));

            return wrappers.ToDictionary(wrapper => wrapper.Key, wrapper => wrapper);
        }

        private void UpdateCharAttributes()
        {
            foreach (DataBlockWrapper wrapper in CharAttributes.Values)
            {
                wrapper.DataSource = DataBytes;
                wrapper.RaisePropertyChanged(String.Empty);
            }
        }

        private void WriteCallback(IDataBlock block)
        {
            DataChanges = true;
            RaiseIndexerChange(block.Key);
        }

        public void Dispose()
        {
            _charAttributes.ForEach(pair => pair.Value.Dispose());

            _cardBytes = null;

            _cardChanged = false;

            _dataBytes = null;
            _dataChanged = false;
            _thumbBytes = null;
            _thumbChanged = false;

            _thumbImage = null;
            _cardImage = null;

            IsDisposed = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}