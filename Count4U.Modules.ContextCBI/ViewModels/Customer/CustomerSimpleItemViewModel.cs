using Count4U.Common.Helpers;
using Count4U.Model.Main;
using System;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class CustomerSimpleItemViewModel
    {
        private readonly Customer _customer;

        private string _fancyName;

        public CustomerSimpleItemViewModel(Customer customer)
        {
            _customer = customer;

          //  _fancyName =  UtilsConvert.HebrewText(String.Format("{0} [{1}]", _customer.Name, _customer.Code));                        
            _fancyName = UtilsConvert.CustomerFancyName(_customer);                
        }

        public Customer Customer
        {
            get { return _customer; }
        }

        public string FancyName
        {
            get { return _fancyName; }
            set { _fancyName = value; }
        }
    }
}