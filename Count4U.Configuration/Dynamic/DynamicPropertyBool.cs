using System;
using System.ComponentModel;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Configuration.Dynamic
{
    public class DynamicPropertyBool : DynamicProperty, IDataErrorInfo
    {
        private bool _value;

        public DynamicPropertyBool(bool value)
        {
            _value = value;
        }

        public string this[string columnName]
        {
            get { return String.Empty; }
        }

        public string Error { get; private set; }

        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(()=>Value);
            }
        }
    }
}