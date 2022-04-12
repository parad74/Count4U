using System;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchItemViewModel : NotificationObject
    {
        private Branch _branch;
        private string _address;
        private string _code;
        private string _contactPerson;
        private string _description;
        private string _fax;
        private string _mail;
        private string _name;
        private string _phone;

        private string _customerCode;
        private string _customerName;
        
        private string _codeLocal;
        private string _codeErp;

        public BranchItemViewModel(Branch branch, Customer customer)
        {
            this.UpdateViewModelWithBranch(branch);

			if (customer != null)
			{
				_customerCode = customer.Code;
				_customerName = customer.Name;
			}
			else
			{
				_customerCode = branch.CustomerCode;

			}
        }

        public string Address
        {
            get 
			{
				if (String.IsNullOrWhiteSpace(this._address) == true) return String.Empty;
				return this._address.Replace(Environment.NewLine, String.Empty).Replace(@"\r\n", String.Empty); 
			}
            set
            {
                this._address = value;
                this.RaisePropertyChanged(() => this.Address);
            }
        }

        public string Code
        {
            get { return this._code; }
            set
            {
                this._code = value;
                this.RaisePropertyChanged(() => this.Code);
            }
        }

        public string ContactPerson
        {
            get { return this._contactPerson; }
            set
            {
                this._contactPerson = value;
                this.RaisePropertyChanged(() => this.ContactPerson);
            }
        }

        public string Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

        public string Fax
        {
            get { return this._fax; }
            set
            {
                this._fax = value;
                this.RaisePropertyChanged(() => this.Fax);
            }
        }

        public string Mail
        {
            get { return this._mail; }
            set
            {
                this._mail = value;
                this.RaisePropertyChanged(() => this.Mail);
            }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        public string Phone
        {
            get { return this._phone; }
            set
            {
                this._phone = value;
                this.RaisePropertyChanged(() => this.Phone);
            }
        }

        public Branch Branch
        {
            get { return this._branch; }
        }

        public string CustomerCode
        {
            get { return _customerCode; }
            set
            {
                _customerCode = value;
                RaisePropertyChanged(() => CustomerCode);
            }
        }

        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                _customerName = value;
                RaisePropertyChanged(() => CustomerName);
            }
        }

        public string CodeLocal
        {
            get { return _codeLocal; }
            set
            {
                _codeLocal = value;
                RaisePropertyChanged(() => CodeLocal);
            }
        }

        public string CodeErp
        {
            get { return _codeErp; }
            set
            {
                _codeErp = value;
                RaisePropertyChanged(() => CodeErp);
            }
        }

        public void UpdateViewModelWithBranch(Branch branch)
        {
            if (branch == null) return;

            this._branch = branch;

            Address = this.Branch.Address;
            Code = this.Branch.Code;
			ContactPerson = "";//this._branch.ContactPerson; занято на Selected Adapter import  Family   //this.Branch.ContactPerson;
            Description = this.Branch.Description;
            Fax = this.Branch.Fax;
            Mail = this.Branch.Mail;
            Name = this.Branch.Name;
            Phone = this.Branch.Phone;
            CodeLocal = this.Branch.BranchCodeLocal;
            CodeErp = this.Branch.BranchCodeERP;
        }
    }
}