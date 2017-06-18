// --------------------------------------------------
// AA2Lib - Int16DataBlock.cs
// --------------------------------------------------

using System;

namespace AA2Lib.Code.DataBlocks
{
    public class Int16DataBlock : IDataBlock
    {
        private short _maxValue = short.MaxValue;
        private short _minValue = short.MinValue;
        private short _warnValue = short.MaxValue;

        public bool IsRanged { get; set; }

        public short MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public short MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public short WarnValue
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
            get { return 2; }
            set { }
        }

        public Type DataType
        {
            get { return typeof(Int16); }
        }

        public object Read(byte[] data, int offset = 0)
        {
            return BitConverter.ToInt16(data, offset + Address);
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            if (!(obj is Int16) || obj.Equals(Read(data, offset)))
                return false;
            var bytes = BitConverter.GetBytes(((Int16) obj).Clamp(MinValue, MaxValue));
            Buffer.BlockCopy(bytes, 0, data, offset + Address, 2);
            if (CopyAddresses == null)
                return true;
            foreach (int copyAddress in CopyAddresses)
            {
                Buffer.BlockCopy(bytes, 0, data, offset + copyAddress, 2);
            }
            return true;
        }
    }
}