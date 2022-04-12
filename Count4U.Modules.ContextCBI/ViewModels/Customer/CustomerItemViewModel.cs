using System;
using System.IO;
using System.Windows.Media.Imaging;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels
{
	public class CustomerItemViewModel : NotificationObject
	{
        private readonly IContextCBIRepository _contextCbiRepository;
		private Customer _customer;
		private string _address;
		private string _contactPerson;
		private string _description;
		private string _fax;
		private string _mail;
		private string _name;
		private string _phone;
        private BitmapImage _image;
	    private string _code;

	    public CustomerItemViewModel(Customer customer, IContextCBIRepository contextCBIRepository)
	    {
	        this._contextCbiRepository = contextCBIRepository;
	        this.UpdateViewModelViewCustomer(customer);
	    }

	    public void UpdateViewModelViewCustomer(Customer customer)
		{
			this._customer = customer;

			this.Address = this.Customer.Address;
			this.ContactPerson = "";// this.Customer.ContactPerson; "";//this._customer.ContactPerson; занято на Selected Adapter import  Family
			this.Description = this.Customer.Description;
			this.Fax = this.Customer.Fax;
			this.Mail = this.Customer.Mail;
			this.Name = this.Customer.Mail;
			this.Name = this.Customer.Name;
			this.Phone = this.Customer.Phone;
	        this.Code = this.Customer.Code;

	        Utils.RunOnUI(() =>
	                          {
	                              if (!String.IsNullOrEmpty(this._customer.LogoPath))
	                              {
	                                  string logoPath = this._contextCbiRepository.GetFolderLogoPath(this._customer);
	                                  if (Directory.Exists(logoPath))
	                                  {
	                                      string finalPath = Path.Combine(logoPath, this._customer.LogoPath);
	                                      if (File.Exists(finalPath))
	                                      {
	                                          byte[] byteArray = UtilsImage.FromFileToByteArray(finalPath);
	                                          if (byteArray != null && byteArray.Length > 0)
	                                              this.Image = UtilsImage.ImageFromBuffer(byteArray);
	                                      }
	                                  }
	                              }
	                              else
	                              {
	                                  this.Image = null;
	                              }
	                          });
		}


		public string Address
		{
			get
			{
				if (this._address == null) return String.Empty;
				return this._address.Replace(Environment.NewLine, String.Empty).Replace(@"\r\n", String.Empty); 
			}
			set
			{
				this._address = value;
				this.RaisePropertyChanged(() => this.Address);
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

		public Customer Customer
		{
			get { return this._customer; }
		}

	    public BitmapImage Image
	    {
	        get { return this._image; }
	        set
	        {
	            this._image = value;
                RaisePropertyChanged(()=>Image);
	        }
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
	}
}