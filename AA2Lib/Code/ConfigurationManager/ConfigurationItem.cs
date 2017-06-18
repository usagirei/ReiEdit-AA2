// --------------------------------------------------
// AA2Lib - ConfigurationItem.cs
// --------------------------------------------------

using System.ComponentModel;

namespace AA2Lib.Code.ConfigurationManager
{
    public sealed class ConfigurationItem : INotifyPropertyChanged
    {
        private object _default;
        private string _key;
        private object _value;

        public string Key
        {
            get { return _key; }
            private set
            {
                _key = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("Key");
            }
        }

        public bool ShoudSave
        {
            get { return Value != null; }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                //[WXP] Doesn't support CallerMemberName
                OnPropertyChanged("Value");
            }
        }

        public ConfigurationItem(string key, object value)
        {
            Key = key;
            Value = value;
        }

        //[WXP] CallerMemberName Removed

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}