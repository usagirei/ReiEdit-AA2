// --------------------------------------------------
// AA2Lib - ColorDataBlock.cs
// --------------------------------------------------

using System;
using ReiFX;

namespace AA2Lib.Code.DataBlocks
{
    public class ColorDataBlock : IDataBlock
    {
        private int _address;
        private Int32DataBlock _internalBlock;
        private string _key;

        public override string ToString()
        {
            const string format = @"[DataBlock Type:'{0}' Key:'{1}' Address:'0x{2:X4}' Size:'0x{3:X2}']";
            return string.Format(format, DataType.Name, Key, Address, Size);
        }

        private void ReloadInternal()
        {
            _internalBlock = new Int32DataBlock
            {
                Address = Address,
                Key = "Internal_" + Key
            };
        }

        public Type DataType
        {
            get { return typeof(Color); }
        }

        public string Name { get; set; }
        public bool ReadOnly { get; set; }

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                ReloadInternal();
            }
        }

        public int Address
        {
            get { return _address; }
            set
            {
                _address = value;
                ReloadInternal();
            }
        }

        public int[] CopyAddresses { get; set; }

        public int Size
        {
            get { return _internalBlock.Size; }
            set { }
        }

        public object Read(byte[] data, int offset = 0)
        {
            int cdata = (int) _internalBlock.Read(data, offset);
            byte a = (byte) (cdata >> 24 & 0xFF);
            byte r = (byte) (cdata >> 16 & 0xFF);
            byte g = (byte) (cdata >> 8 & 0xFF);
            byte b = (byte) (cdata & 0xFF);
            return Color.FromRgb(r, g, b, a);
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            if (!(obj is Color) || obj.Equals(Read(data)))
                return false;
            Color color = (Color) obj;
            int cdata = (color.A << 24) + (color.R << 16) + (color.G << 8) + color.B;
            bool written = _internalBlock.Write(cdata, data, offset);
            if (CopyAddresses == null)
                return written;
            int oldAddress = Address;
            foreach (int copyAddress in CopyAddresses)
            {
                _internalBlock.Address = copyAddress;
                _internalBlock.Write(cdata, data, offset);
            }
            _internalBlock.Address = oldAddress;
            return written;
        }
    }
}