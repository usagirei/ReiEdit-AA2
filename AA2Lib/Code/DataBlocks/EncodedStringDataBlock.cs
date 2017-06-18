// --------------------------------------------------
// AA2Lib - EncodedStringDataBlock.cs
// --------------------------------------------------

using System;
using System.Text;

namespace AA2Lib.Code.DataBlocks
{
    public class EncodedStringDataBlock : IDataBlock
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
            var buffer = new byte[Size];
            int i;
            for (i = 0; i < Size; ++i)
            {
                byte srcByte = data[offset + Address + i];
                buffer[i] = (byte) (srcByte ^ 0xFF & 0xFF);
                if (buffer[i] == 0)
                    break;
            }
            return ShiftJisEncoding.GetString(buffer, 0, i);
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            if (!(obj is string) || obj.Equals(Read(data, offset)))
                return false;
            var buffer = new byte[Size];
            var strBytes = ShiftJisEncoding.GetBytes(obj as string);
            for (int i = 0; i < Size; ++i)
            {
                if (i < strBytes.Length)
                    buffer[i] = (byte) (strBytes[i] ^ 0xFF & 0xFF);
                else
                    buffer[i] = 0xFF;
            }
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