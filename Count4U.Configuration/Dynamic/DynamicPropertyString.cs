using System.ComponentModel;
using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Configuration.Dynamic
{
    public class DynamicPropertyString : DynamicProperty, IDataErrorInfo
    {
        private string _value;

        public DynamicPropertyString(string value)
        {
            Value = value;
        }

        public string this[string columnName]
        {
            get { return String.Empty; }
        }

        public string Error { get; private set; }

        public string Value
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