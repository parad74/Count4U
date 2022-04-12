using System;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Model.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels
{
	public class InventorItemViewModel : NotificationObject
	{
        private readonly enLanguage _language;

		private Inventor _inventor;

		private string _name;
		private string _description;
		private string _createDate;
		private string _inventorDate;
		private string _status;
		private string _statusCode;

	    private string _code;
	    private string _customerCode;
	    private string _customerName;
	    private string _branchCode;
	    private string _branchName;
		private string _importCatalogAdapterCode;

	    public InventorItemViewModel(Inventor inventor, Customer customer, Branch branch, enLanguage language)
		{
	        _language = language;
	        this.UpdateViewModelWithInventor(inventor);

			if (customer != null)
			{
				_customerCode = customer.Code;
				_customerName = customer.Name;
				Customer = customer.Name;
			}
			else
			{
				_customerCode = inventor.CustomerCode;
			}
			if (branch != null)
			{
				_branchCode = branch.Code;
				_branchName = branch.Name;
				Branch = branch.Name;
			}
			else
			{
				_branchCode = inventor.BranchCode;
			}

		    _code = inventor.Code;
	    
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

		public string ImportCatalogAdapterCode
		{
			get { return _importCatalogAdapterCode; }
			set
			{
				_importCatalogAdapterCode = value;
				this.RaisePropertyChanged(() => this.ImportCatalogAdapterCode);
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

		public string CreateDate
		{
			get { return this._createDate; }
			set
			{
				this._createDate = value;
				this.RaisePropertyChanged(() => this.CreateDate);
			}
		}

		public string InventorDate
		{
			get { return this._inventorDate; }
			set
			{
				this._inventorDate = value;
				this.RaisePropertyChanged(() => this.InventorDate);
			}
		}

		public string Status
		{
			get { return this._status; }
			set
			{
				this._status = value;
				this.RaisePropertyChanged(() => this.Status);
			}
		}

		public string StatusCode
		{
			get { return this._statusCode; }
			set
			{
				this._statusCode = value;
				this.RaisePropertyChanged(() => this.StatusCode);
			}
		}

		public string Branch { get; set; }

		public string Customer { get; set; }

		public Inventor Inventor
		{
			get { return this._inventor; }
		}

	    public string CustomerCode
	    {
	        get { return _customerCode; }
	        set { _customerCode = value; }
	    }

	    public string CustomerName
	    {
	        get { return _customerName; }
	        set { _customerName = value; }
	    }

	    public string BranchCode
	    {
	        get { return _branchCode; }
	        set { _branchCode = value; }
	    }

	    public string BranchName
	    {
	        get { return _branchName; }
	        set { _branchName = value; }
	    }

		

	    public string Code
	    {
	        get { return _code; }
	        set
	        {
	            _code = value;
                RaisePropertyChanged(()=>Code);
	        }
	    }

	    public void UpdateViewModelWithInventor(Inventor inventor)
		{
			this._inventor = inventor;

			this.ImportCatalogAdapterCode = this.Inventor.ImportCatalogAdapterCode;
			this.Name = this.Inventor.Name;
            this.CreateDate = UtilsConvert.DateHebrewConvert(this.Inventor.CreateDate, _language);
            this.InventorDate = UtilsConvert.DateHebrewConvert(this.Inventor.InventorDate, _language);
			//this.CompleteDate = UtilsConvert.DateHebrewConvert(this.Inventor.InventorDate, _language);
			this.Status = this.Inventor.Status;
			this.StatusCode = this.Inventor.StatusCode;
	        this.Code = inventor.Code;
			this.Description = inventor.Description;
		}
	}
}