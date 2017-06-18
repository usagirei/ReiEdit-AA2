// --------------------------------------------------
// AA2Lib - DataBlockWrapper.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace AA2Lib.Code.DataBlocks
{
    public class DataBlockWrapper : INotifyPropertyChanged, IDisposable
    {
        private readonly Dictionary<int, EnumValuePair> _enumData;
        private int _dataOffset;
        private byte[] _dataSource;

        public int Address
        {
            get { return DataBlock.Address; }
        }

        public string AddressHex
        {
            get { return string.Format("0x{0:X4}", Address); }
        }

        public IDataBlock DataBlock { get; set; }

        public int DataOffset
        {
            get { return _dataOffset; }
            set
            {
                _dataOffset = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("DataOffset");
            }
        }

        public byte[] DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("DataSource");
            }
        }

        public Type DataType
        {
            get { return DataBlock.DataType; }
        }

        public Dictionary<int, EnumValuePair> EnumData
        {
            get
            {
                if (!IsEnum)
                    return null;
                return _enumData;
            }
        }

        public string EnumName
        {
            get
            {
                return IsEnum
                    ? ((EnumDataBlock) DataBlock).EnumName
                    : null;
            }
        }

        public string EnumValue
        {
            get
            {
                return IsEnum
                    ? EnumData[Convert.ToInt32(Value)].Value
                    : null;
            }
        }

        public bool IsDisposed { get; set; }

        public bool IsEnum
        {
            get { return DataBlock is EnumDataBlock; }
        }

        public bool IsRanged
        {
            get
            {
                try
                {
                    switch (DataType.Name)
                    {
                        case "Byte":
                            return ((ByteDataBlock) DataBlock).IsRanged;
                        case "Int16":
                            return ((Int16DataBlock) DataBlock).IsRanged;
                        case "Int32":
                            return ((Int32DataBlock) DataBlock).IsRanged;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public string Key
        {
            get { return DataBlock.Key; }
        }

        public int Length
        {
            get { return DataBlock.Size; }
        }

        public string Name
        {
            get { return DataBlock.Name; }
        }

        public DoubleCollection RangedThresholds
        {
            get
            {
                if (!IsRanged)
                    return new DoubleCollection
                    {
                        0,
                        1
                    };
                int min = 0, max = 1;
                switch (DataType.Name)
                {
                    case "Byte":
                        min = ((ByteDataBlock) DataBlock).WarnValue;
                        max = ((ByteDataBlock) DataBlock).MaxValue;
                        break;
                    case "Int16":
                        min = ((Int16DataBlock) DataBlock).WarnValue;
                        max = ((Int16DataBlock) DataBlock).MaxValue;
                        break;
                    case "Int32":
                        min = ((Int32DataBlock) DataBlock).WarnValue;
                        max = ((Int32DataBlock) DataBlock).MaxValue;
                        break;
                }
                return new DoubleCollection
                {
                    0,
                    (double) min / max,
                    1
                };
            }
        }

        public double RangedValue
        {
            get
            {
                int min, max, val;
                long delta;
                switch (DataType.Name)
                {
                    case "Byte":
                        min = ((ByteDataBlock) DataBlock).MinValue;
                        max = ((ByteDataBlock) DataBlock).MaxValue;
                        val = (byte) Value;
                        delta = max - min;
                        return (double) val / delta;
                    case "Int16":
                        min = ((Int16DataBlock) DataBlock).MinValue;
                        max = ((Int16DataBlock) DataBlock).MaxValue;
                        val = (short) Value;
                        delta = max - min;
                        return (double) val / delta;
                    case "Int32":
                        min = ((Int32DataBlock) DataBlock).MinValue;
                        max = ((Int32DataBlock) DataBlock).MaxValue;
                        val = (int) Value;
                        delta = max - min;
                        return (double) (val - min) / delta;
                }
                return Double.NaN;
            }
            set
            {
                if (ReadOnly)
                    return;
                int min, max;
                long delta;
                switch (DataType.Name)
                {
                    case "Byte":
                        min = ((ByteDataBlock) DataBlock).MinValue;
                        max = ((ByteDataBlock) DataBlock).MaxValue;
                        delta = max - min;
                        Value = (byte) (delta * value + min);
                        break;
                    case "Int16":
                        min = ((Int16DataBlock) DataBlock).MinValue;
                        max = ((Int16DataBlock) DataBlock).MaxValue;
                        delta = max - min;
                        Value = (short) (delta * value + min);
                        break;
                    case "Int32":
                        min = ((Int32DataBlock) DataBlock).MinValue;
                        max = ((Int32DataBlock) DataBlock).MaxValue;
                        delta = max - min;
                        Value = (int) (delta * value + min);
                        break;
                }
            }
        }

        public bool ReadOnly
        {
            get { return DataBlock.ReadOnly; }
        }

        public object Value
        {
            get { return DataBlock.Read(DataSource, DataOffset); }
            set
            {
                if (value == null)
                    return;
                bool written = DataBlock.Write(value, DataSource, DataOffset);
                if (!written)
                    return;
                if (WriteCallback != null)
                    WriteCallback(DataBlock);

                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("Value");
                if (IsEnum)
                    OnPropertyChanged("EnumValue");
                else if (IsRanged)
                {
                    OnPropertyChanged("RangedValue");
                    OnPropertyChanged("Warning");
                }
            }
        }

        public bool Warning
        {
            get
            {
                if (!IsRanged)
                    return false;
                int value = 0, threshold = 0;
                switch (DataType.Name)
                {
                    case "Byte":
                        threshold = ((ByteDataBlock) DataBlock).WarnValue;
                        value = (byte) Value;
                        break;
                    case "Int16":
                        threshold = ((Int16DataBlock) DataBlock).WarnValue;
                        value = (short) Value;
                        break;
                    case "Int32":
                        threshold = ((Int32DataBlock) DataBlock).WarnValue;
                        value = (int) Value;
                        break;
                }
                return value > threshold;
            }
        }

        public Action<IDataBlock> WriteCallback { get; set; }

        public bool Writeable
        {
            get { return !DataBlock.ReadOnly; }
        }

        public DataBlockWrapper(
            byte[] dataSource,
            int dataoffset,
            IDataBlock blocksource,
            Action<IDataBlock> writeCallback = null)
        {
            DataSource = dataSource;
            DataOffset = dataoffset;
            DataBlock = blocksource;
            WriteCallback = writeCallback;

            if (blocksource is SeatDataBlock)
                SeatDataBlock.NamesChanged += () =>
                {
                    EnumDataBlock block = blocksource as EnumDataBlock;
                    OnPropertyChanged("EnumData");
                    block.Enum.ForEach
                    (pair =>
                    {
                        if (_enumData.ContainsKey(pair.Key))
                            _enumData[pair.Key].Value = pair.Value;
                        else
                            _enumData.Add(pair.Key,
                                new EnumValuePair
                                {
                                    EnumName = block.EnumName,
                                    Value = pair.Value
                                });
                    });
                };

            if (blocksource is EnumDataBlock)
            {
                EnumDataBlock block = blocksource as EnumDataBlock;
                _enumData = block.Enum.ToDictionary(pair => pair.Key,
                    pair => new EnumValuePair
                    {
                        EnumName = block.EnumName,
                        Value = pair.Value
                    });
            }
        }

        public override string ToString()
        {
            const string format =
                @"[DataBlockWrapper Type:'{0}' Key:'{1}' Address:'0x{2:X4}' Size:'0x{3:X2}' Value:'{4}']";
            string dataTypeName = DataType == null
                ? "Null"
                : DataType.Name;
            if (IsEnum)
                return string.Format(format, dataTypeName, Key, Address, Length, EnumValue);
            if (IsRanged)
                return string.Format(format, dataTypeName, Key, Address, Length, RangedValue);
            return string.Format(format, dataTypeName, Key, Address, Length, Value);
        }

        public void RaisePropertyChanged(string name)
        {
            OnPropertyChanged(name);
            if (EnumData == null)
                return;
            foreach (EnumValuePair pair in EnumData.Values)
            {
                pair.RaisePropertyChanged(string.Empty);
            }
        }

        //[WXP] CallerMemberName Removed

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            if (DataBlock is SeatDataBlock)
                return;
            EnumDataBlock enumBlock = DataBlock as EnumDataBlock;
            if (enumBlock != null)
            {
                _enumData.Clear();
                enumBlock.Enum.Clear();
            }
            //throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public class EnumValuePair : INotifyPropertyChanged
        {
            private string _enumName;
            private string _value;

            public string EnumName
            {
                get { return _enumName; }
                set
                {
                    _enumName = value;
                    //[WXP] Doesn't support CallerMemberName
                    OnPropertyChanged("EnumName");
                }
            }

            public bool IsDisposed { get; set; }

            public string Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    //[WXP] Doesn't support CallerMemberName
                    OnPropertyChanged("Value");
                }
            }

            public override string ToString()
            {
                return string.Format("{{{0}}}", Value);
            }

            public void RaisePropertyChanged(string name)
            {
                if (IsDisposed)
                    return;
                OnPropertyChanged(name);
            }

            //[WXP] CallerMemberName Removed

            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (IsDisposed)
                    return;
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}