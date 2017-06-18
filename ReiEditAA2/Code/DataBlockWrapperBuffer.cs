// --------------------------------------------------
// ReiEditAA2 - DataBlockWrapperBuffer.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AA2Lib;
using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.Code.CharacterViewModelProviders
{
    internal class DataBlockWrapperBuffer : IDisposable
    {
        public object this[string key]
        {
            get { return Attributes[key].Value; }
            set
            {
                Attributes[key].Value = value;
                HasChanges = true;
            }
        }

        public Dictionary<string, DataBlockWrapper> Attributes { get; private set; }
        public byte[] DataBytes { get; set; }
        public bool HasChanges { get; set; }
        public bool IsDisposed { get; set; }

        public Action Reload { get; set; }
        public Func<bool> SaveChanges { get; set; }

        public DataBlockWrapperBuffer(byte[] data, XDocument scanDocument)
        {
            int dataLen;
            var blocks = Core.LoadDataBlocks(data, 0, scanDocument, out dataLen);

            DataBytes = new byte[dataLen];
            Buffer.BlockCopy(data, 0, DataBytes, 0, dataLen);

            var wrappers = blocks.Select(block => new DataBlockWrapper(DataBytes, 0, block, WriteCallback));

            Attributes = wrappers.ToDictionary(wrapper => wrapper.Key, wrapper => wrapper);
        }

        public DataBlockWrapperBuffer(byte[] data, IEnumerable<IDataBlock> blocks)
        {
            int dataLen = data.Length;
            DataBytes = new byte[dataLen];
            Buffer.BlockCopy(data, 0, DataBytes, 0, dataLen);
            var wrappers = blocks.Select(block => new DataBlockWrapper(DataBytes, 0, block, WriteCallback));

            Attributes = wrappers.ToDictionary(wrapper => wrapper.Key, wrapper => wrapper);
        }

        public T GetAttribute<T>(string key)
        {
            return (T) this[key];
        }

        public DataBlockWrapper GetAttribute(string key)
        {
            return IsDisposed
                ? null
                : Attributes[key];
        }

        public string GetEnumValue(string key)
        {
            return Attributes[key].EnumValue;
        }

        private void WriteCallback(IDataBlock obj)
        {
            HasChanges = true;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            Attributes.ForEach(pair => pair.Value.Dispose());
            Attributes.Clear();
            IsDisposed = true;
        }
    }
}