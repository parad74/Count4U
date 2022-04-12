using System.ComponentModel;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Configuration.Dynamic
{
    public class DynamicPropertyInt : DynamicProperty, IDataErrorInfo
    {
        private int _value;

        public DynamicPropertyInt(int value)
        {
            Value = value;
        }

        public string this[string columnName]
        {
            get { return string.Empty; }
        }

        public string Error { get; private set; }

        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }
    }
}