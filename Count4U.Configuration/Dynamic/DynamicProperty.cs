using System.ComponentModel;
using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Configuration.Dynamic
{
    public abstract class DynamicProperty : NotificationObject
    {
        public DynamicPropertyInfo Info { get; set; }

        public void RaisePropertyChanged()
        {
            RaisePropertyChanged("Value");
        }
    }
}