// --------------------------------------------------
// AA2Lib - Int32DataBlock.cs
// --------------------------------------------------

using System;

namespace AA2Lib.Code.DataBlocks
{
    public class Int32DataBlock : IDataBlock
    {
        private int _maxValue = int.MaxValue;
        private int _minValue = int.MinValue;
        private int _warnValue = int.MaxValue;

        public bool IsRanged { get; set; }

        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public int WarnValue
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
            get { return 4; }
            set { }
        }

        public Type DataType
        {
            get { return typeof(Int32); }
        }

        public object Read(byte[] data, int offset = 0)
        {
            return BitConverter.ToInt32(data, offset + Address);
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            if (!(obj is Int32) || obj.Equals(Read(data, offset)))
                return false;
            var bytes = BitConverter.GetBytes(((Int32) obj).Clamp(MinValue, MaxValue));
            Buffer.BlockCopy(bytes, 0, data, offset + Address, 4);

            if (CopyAddresses == null)
                return true;
            foreach (int copyAddress in CopyAddresses)
            {
                Buffer.BlockCopy(bytes, 0, data, offset + copyAddress, 4);
            }
            return true;
        }
    }
}