// --------------------------------------------------
// AA2Lib - BoolDataBlock.cs
// --------------------------------------------------

using System;

namespace AA2Lib.Code.DataBlocks
{
    public class BoolDataBlock : IDataBlock
    {
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
            get { return typeof(Boolean); }
        }

        public object Read(byte[] data, int offset = 0)
        {
            return data[offset + Address] != 0;
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            if (!(obj is bool) || obj.Equals(Read(data, offset)))
                return false;
            byte boolByte = (bool) obj
                ? (byte) 1
                : (byte) 0;
            data[offset + Address] = boolByte;
            if (CopyAddresses == null)
                return true;
            foreach (int copyAddress in CopyAddresses)
            {
                data[offset + copyAddress] = boolByte;
            }
            return true;
        }
    }
}