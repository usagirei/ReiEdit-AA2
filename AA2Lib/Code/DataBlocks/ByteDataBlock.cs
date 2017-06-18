// --------------------------------------------------
// AA2Lib - ByteDataBlock.cs
// --------------------------------------------------

using System;

namespace AA2Lib.Code.DataBlocks
{
    public class ByteDataBlock : IDataBlock
    {
        private byte _maxValue = byte.MaxValue;
        private byte _minValue = byte.MinValue;
        private byte _warnValue = byte.MaxValue;

        public bool IsRanged { get; set; }

        public byte MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public byte MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public byte WarnValue
        {
            get { return _warnValue; }
            set { _warnValue = value; }
        }

        public override string ToString()
        {
            const string format = @"[DataBlock Type:'{0}' Key:'{1}' Address:'0x{2:X4}' Size:'0x{3:X2}']";
            return string.Format(format, DataType.Name, Key, Address, Size);
        }

        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public string Key { get; set; }
        public int Address { get; set; }
        public int[] CopyAddresses { get; set; }

        public int Size
        {
            get { return 1; }
            set { }
        }

        public Type DataType
        {
            get { return typeof(Byte); }
        }

        public object Read(byte[] data, int offset = 0)
        {
            return data[offset + Address];
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            if (!(obj is byte) || obj.Equals(Read(data, offset)))
                return false;

            byte byteToWrite = ((byte) obj).Clamp(MinValue, MaxValue);

            data[offset + Address] = byteToWrite;
            if (CopyAddresses == null)
                return true;
            foreach (int copyAddress in CopyAddresses)
            {
                data[offset + copyAddress] = byteToWrite;
            }
            return true;
        }
    }
}