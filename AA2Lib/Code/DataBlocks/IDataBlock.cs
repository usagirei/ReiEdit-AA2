// --------------------------------------------------
// AA2Lib - IDataBlock.cs
// --------------------------------------------------

using System;

namespace AA2Lib.Code.DataBlocks
{
    public interface IDataBlock
    {
        int Address { get; set; }
        int[] CopyAddresses { get; set; }
        Type DataType { get; }
        string Key { get; set; }
        string Name { get; set; }
        bool ReadOnly { get; set; }
        int Size { get; set; }

        object Read(byte[] @data, int offset);
        bool Write(object obj, byte[] @data, int offset);
    }
}