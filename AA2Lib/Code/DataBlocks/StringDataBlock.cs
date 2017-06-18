// --------------------------------------------------
// AA2Lib - StringDataBlock.cs
// --------------------------------------------------

using System;
using System.Text;

namespace AA2Lib.Code.DataBlocks
{
    public class StringDataBlock : IDataBlock
    {
        public static Encoding ShiftJisEncoding = Encoding.GetEncoding(932);

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
        public int Size { get; set; }

        public Type DataType
        {
            get { return typeof(String); }
        }

        public object Read(byte[] data, int offset = 0)
        {
            string str = ShiftJisEncoding.GetString(data, offset + Address, Size);
            int indexZero = str.IndexOf('\0');
            return indexZero != -1
                ? str.Remove(indexZero)
                : str;
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            if (!(obj is string) || obj.Equals(Read(data, offset)))
                return false;

            var bytes = Encoding.GetEncoding(932)
                .GetBytes((string) obj);
            var buffer = new byte[Size];
            Buffer.BlockCopy(bytes, 0, buffer, 0, Math.Min(bytes.Length, Size));
            Buffer.BlockCopy(buffer, 0, data, offset + Address, Size);
            if (CopyAddresses == null)
                return true;

            foreach (int copyaddress in CopyAddresses)
            {
                Buffer.BlockCopy(buffer, 0, data, offset + copyaddress, Size);
            }
            return true;
        }
    }
}