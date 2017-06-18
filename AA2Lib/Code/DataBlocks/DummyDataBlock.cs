// --------------------------------------------------
// AA2Lib - DummyDataBlock.cs
// --------------------------------------------------

using System;

namespace AA2Lib.Code.DataBlocks
{
    public class DummyDataBlock : IDataBlock
    {
        private static readonly int[] EmptyArray = new int[0];

        public object DummyValue { get; set; }

        public override string ToString()
        {
            const string format = @"[DataBlock Type:'{0}' Key:'{1}' Address:'0x{2:X4}' Size:'0x{3:X2}']";
            string dataTypeName = DataType == null
                ? "Null"
                : DataType.Name;
            return string.Format(format, dataTypeName, Key, Address, Size);
        }

        public int Address { get; set; }

        public int[] CopyAddresses
        {
            get { return EmptyArray; }
            set { }
        }

        public Type DataType
        {
            get
            {
                return DummyValue == null
                    ? null
                    : DummyValue.GetType();
            }
        }

        public string Key { get; set; }
        public int Size { get; set; }
        public string Name { get; set; }

        public bool ReadOnly
        {
            get { return true; }
            set { }
        }

        public object Read(byte[] data, int offset)
        {
            return DummyValue;
        }

        public bool Write(object obj, byte[] data, int offset)
        {
            DummyValue = obj;
            return true;
        }
    }
}