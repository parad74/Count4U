using System.ComponentModel;
using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Configuration.Dynamic
{
    public class DynamicPropertyDouble : DynamicProperty, IDataErrorInfo
    {
        private double _value;

        public DynamicPropertyDouble(double value)
        {
            Value = value;
        }

        public string this[string columnName]
        {
            get { return String.Empty; }
        }

        public string Error { get; private set; }

        public double Value
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