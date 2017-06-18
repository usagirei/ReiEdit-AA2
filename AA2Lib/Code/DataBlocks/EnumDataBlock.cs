// --------------------------------------------------
// AA2Lib - EnumDataBlock.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;

namespace AA2Lib.Code.DataBlocks
{
    public class EnumDataBlock : IDataBlock
    {
        private int _address;
        private IDataBlock _internalBlock;
        private string _key;
        private int _size;

        public virtual Dictionary<int, string> Enum { get; set; }

        public virtual string EnumName { get; set; }

        public override string ToString()
        {
            const string format = @"[DataBlock Type:'{0}' Key:'{1}' Address:'0x{2:X4}' Size:'0x{3:X2}']";
            return string.Format(format, DataType.Name, Key, Address, Size);
        }

        private void ReloadInternal()
        {
            switch (Size)
            {
                case 1:
                    _internalBlock = new ByteDataBlock
                    {
                        Address = Address,
                        Key = "InternalByte_" + Key
                    };
                    break;
                case 2:
                    _internalBlock = new Int16DataBlock
                    {
                        Address = Address,
                        Key = "InternalWord_" + Key
                    };
                    break;
                case 4:
                    _internalBlock = new Int32DataBlock
                    {
                        Address = Address,
                        Key = "InternalDWord_" + Key
                    };
                    break;
                default:
                    _internalBlock = null;
                    break;
            }
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

        public Type DataType
        {
            get
            {
                switch (Size)
                {
                    case 1:
                        return typeof(Byte);
                    case 2:
                        return typeof(Int16);
                    case 4:
                        return typeof(Int32);
                }
                return null;
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
            get { return _size; }
            set
            {
                _size = value;
                ReloadInternal();
            }
        }

        public object Read(byte[] data, int offset = 0)
        {
            return _internalBlock.Read(data, offset);
        }

        public bool Write(object obj, byte[] data, int offset = 0)
        {
            bool written;
            object toWrite;
            switch (Size)
            {
                case 1:
                    toWrite = Convert.ToByte(obj);
                    break;
                case 2:
                    toWrite = Convert.ToInt16(obj);
                    break;
                case 4:
                    toWrite = Convert.ToInt32(obj);
                    break;
                default:
                    return false;
            }
            written = _internalBlock.Write(toWrite, data, offset);
            if (CopyAddresses == null)
                return written;
            int oldAddress = Address;
            foreach (int copyAddress in CopyAddresses)
            {
                _internalBlock.Address = copyAddress;
                _internalBlock.Write(toWrite, data, offset);
            }
            _internalBlock.Address = oldAddress;
            return written;
        }
    }

    public class SeatDataBlock : EnumDataBlock
    {
        public static event Action NamesChanged;

        private static readonly Dictionary<int, string> _enum;

        public override Dictionary<int, string> Enum
        {
            get { return _enum; }
        }

        public override string EnumName
        {
            get { return "SPECIAL_SEAT"; }
        }

        static SeatDataBlock()
        {
            _enum = new Dictionary<int, string>(25);
            for (int i = 0; i < 25; i++)
            {
                _enum.Add(i, String.Format("[{0:d2}] -- --", i));
            }
        }

        public SeatDataBlock()
        {
            Size = 4;
        }

        public static void ResetSeats()
        {
            for (int i = 0; i < 25; i++)
                _enum[i] = String.Format("[{0:d2}] -- --", i);
        }

        public static void SetSeat(int seat, string name)
        {
            _enum[seat] = String.Format("[{0:d2}] {1}", seat, name);
            OnNamesChanged();
        }

        private static void OnNamesChanged()
        {
            Action handler = NamesChanged;
            if (handler != null)
                handler();
        }
    }
}