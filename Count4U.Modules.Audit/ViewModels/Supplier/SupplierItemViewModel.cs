using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels.Supplier
{
    public class SupplierItemViewModel : NotificationObject
    {
         private string _code;
        private string _name;
        private string _description;

        public SupplierItemViewModel(Count4U.Model.Count4U.Supplier supplier)
        {
            Update(supplier);
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public void Update(Count4U.Model.Count4U.Supplier supplier)
        {
            Code = supplier.SupplierCode;
            Name = supplier.Name;
            Description = supplier.Description;
        } 
    }
}