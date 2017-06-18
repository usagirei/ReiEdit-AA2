// --------------------------------------------------
// AA2Lib - ClothFile.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AA2Lib.Code.DataBlocks;

namespace AA2Lib.Code
{
    public class ClothFile : INotifyPropertyChanged
    {
        private Dictionary<string, DataBlockWrapper> _attributes;
        private byte[] _rawData;

        public object this[string key]
        {
            get { return Attributes[key].Value; }
            set
            {
                Attributes[key].Value = value;
                RaiseIndexerChange(key);
            }
        }

        public Dictionary<string, DataBlockWrapper> Attributes
        {
            get { return _attributes ?? (_attributes = InitAttributes()); }
        }

        public byte[] RawData
        {
            get { return _rawData; }
            set
            {
                _rawData = value;
                _attributes = null;
                OnPropertyChanged(String.Empty);
            }
        }

        public void RaiseIndexerChange(string name)
        {
            OnPropertyChanged("Item[]");
            OnPropertyChanged(string.Format("Item[{0}]", name));
        }

        public static ClothFile Load(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                ClothFile cf = Load(fs);
                fs.Close();
                return cf;
            }
            catch
            {
                return null;
            }
        }

        public static ClothFile Load(byte[] rawData)
        {
            try
            {
                ClothFile file = new ClothFile
                {
                    RawData = rawData
                };
                return file;
            }
            catch
            {
                return null;
            }
        }

        public static ClothFile Load(Stream stream)
        {
            try
            {
                var data = new byte[stream.Length];
                int pos = 0;
                while (stream.Length - stream.Position > 0)
                {
                    const int buffer = 128;
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

        //[WXP] CallerMemberName Removed

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private Dictionary<string, DataBlockWrapper> InitAttributes()
        {
            int dataLen;
            XDocument scanDocument = Core.LoadClothDataXDocument();
            var blocks = Core.LoadDataBlocks(RawData, 0, scanDocument, out dataLen);

            var wrappers = blocks.Select(block => new DataBlockWrapper(RawData, 0, block));

            return wrappers.ToDictionary(wrapper => wrapper.Key, wrapper => wrapper);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}