using Count4U.Model.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class LastCustomersListItem : NotificationObject
    {
        private readonly Customer _customer;
		private bool _checkSelect;

        public LastCustomersListItem(Customer customer)
        {
			if (customer == null) return; 
            this._customer = customer;

            Name = this._customer.Name;
            Code = this._customer.Code;
			ModifyDate = this._customer.ModifyDate.ToShortDateString() + " " + this._customer.ModifyDate.ToShortTimeString();
			_checkSelect = false;
        }

        public string Name { get; set; }
        public string Code { get; set; }
		public string ModifyDate { get; set; }


		public bool CheckSelect
		{
			get { return _checkSelect; }
			set
			{
				_checkSelect = value;
				RaisePropertyChanged(() => CheckSelect);
			}
		}

        public Customer Customer
        {
            get { return this._customer; }
        }
    }
}
