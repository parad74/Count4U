using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.ViewModel;
using System.Linq;
using System.Collections.Generic;
using NLog;
using System.Drawing;
using System.Text;
using Count4U.Common.Web;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class CustomerFormViewModel : NotificationObject, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IContextCBIRepository _contextCBIRepository;
        private readonly ICustomerRepository _customerRepository;

        private readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
        private readonly InteractionRequest<Notification> _messageBoxRequest;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly DelegateCommand _browseCommand;
        private readonly DelegateCommand _clearImageCommand;
        private readonly DelegateCommand _codeGenerateCommand;
		private readonly FtpFolderProFile _ftpFolderProfile;

        private Customer _customer;

        private string _code;
        private string _filePath;
        private BitmapImage _image;
        private string _mail;
        private string _name;
        private string _description;
		private string _params;
        private string _address1;
        private string _address2;
        private string _phone;
        private string _fax;
        private string _contactPerson;

        private bool _isReadOnly;
        private bool _isNewMode;
        private bool _isCodeReadOnly;

		 private bool _askProfile;
		 private bool _createProfileAutomatically;
		
		//AskProfile

        private byte[] _imgBytes;
        private Customers _customers;

        public CustomerFormViewModel(
            IContextCBIRepository contextCBIRepository,
            ICustomerRepository customerRepository,
            IUserSettingsManager userSettingsManager,
			FtpFolderProFile ftpFolderProfile)
        {
			this._userSettingsManager = userSettingsManager;
            this._customerRepository = customerRepository;
            this._contextCBIRepository = contextCBIRepository;
			this._ftpFolderProfile = ftpFolderProfile;
            this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
            this._browseCommand = new DelegateCommand(this.BrowseCommandExecuted, this.BrowseCommandCanExecute);
            this._messageBoxRequest = new InteractionRequest<Notification>();
            this._clearImageCommand = new DelegateCommand(ClearImageCommandExecuted, ClearImageCommandCanExecute);
            this._codeGenerateCommand = new DelegateCommand(CodeGenerateCommandExecuted, CodeGenerateCommandCanExecute);
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

        public string Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

		public string Params
		{
			get { return this._params; }
			set
			{
				this._params = value;
				this.RaisePropertyChanged(() => this.Params);
			}
		}

        public string ContactPerson
        {
            get { return _contactPerson; }
            set
            {
                _contactPerson = value;
                RaisePropertyChanged(() => ContactPerson);
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

        public string Code
        {
            get { return this._code; }
            set
            {
                this._code = value;
                this.RaisePropertyChanged(() => this.Code);
            }
        }

        public string Address1
        {
            get { return this._address1; }
            set
            {
                this._address1 = value;
                RaisePropertyChanged(() => Address1);
            }
        }

        public string Address2
        {
            get { return this._address2; }
            set
            {
                this._address2 = value;
                RaisePropertyChanged(() => Address2);
            }
        }

        public string Phone
        {
            get { return this._phone; }
            set
            {
                this._phone = value;
                RaisePropertyChanged(() => Phone);
            }
        }

        public string Fax
        {
            get { return this._fax; }
            set
            {
                this._fax = value;
                RaisePropertyChanged(() => Fax);
            }
        }

        public DelegateCommand BrowseCommand
        {
            get { return this._browseCommand; }
        }

        public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
        {
            get { return this._fileChooseDilogRequest; }
        }

        public string FilePath
        {
            get { return this._filePath; }
            set
            {
                this._filePath = value;
                this.RaisePropertyChanged(() => this.FilePath);
            }
        }

        public BitmapImage Image
        {
            get { return this._image; }
            set
            {
                this._image = value;
                this.RaisePropertyChanged(() => this.Image);
            }
        }

        public InteractionRequest<Notification> MessageBoxRequest
        {
            get { return this._messageBoxRequest; }
        }

        public bool IsReadOnly
        {
            get { return this._isReadOnly; }
            set
            {
                this._isReadOnly = value;
                this.RaisePropertyChanged(() => this.IsReadOnly);
                this._browseCommand.RaiseCanExecuteChanged();
                _codeGenerateCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsCodeReadOnly
        {
            get { return this._isCodeReadOnly; }
            set
            {
                this._isCodeReadOnly = value;
                this.RaisePropertyChanged(() => this.IsCodeReadOnly);

                _codeGenerateCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand ClearImageCommand
        {
            get { return this._clearImageCommand; }
        }

        public bool IsNewMode
        {
            get { return this._isNewMode; }
        }


		public bool AskProfile
        {
			get { return this._askProfile; }
			set
			{
				this._askProfile = value;
				this.RaisePropertyChanged(() => this.AskProfile);
			}
        }


		public bool CreateProfileAutomatically
        {
			get { return this._createProfileAutomatically; }
			set
			{
				this._createProfileAutomatically = value;
				this.RaisePropertyChanged(() => this.CreateProfileAutomatically);
			}
        }

        public DelegateCommand CodeGenerateCommand
        {
            get { return this._codeGenerateCommand; }
        }

        public void CustomerSet(Customer customer, bool isReadOnly, bool isNewMode)
        {
            this.IsReadOnly = isReadOnly;
            this._customer = customer;
            this._isNewMode = isNewMode;

            this.IsCodeReadOnly = ((this._isReadOnly == true)
                || (this.IsNewMode == false));

            this.Name = this._customer.Name;
            this.Description = this._customer.Description;
			this.Params = this._customer.ImportCatalogAdapterParms;
			this.ContactPerson = "";//this._customer.ContactPerson; занято на Selected Adapter import  Family
            this.Mail = this._customer.Mail;

            Address1 = String.Empty;
            Address2 = String.Empty;
            if (String.IsNullOrEmpty(this._customer.Address) == false)
            {
                List<string> split = Helpers.AddressParseIntoMultiple(this._customer.Address);

                if (split.Count > 0)
                    this.Address1 = split[0];
                if (split.Count > 1)
                    this.Address2 = split[1];
            }

            this.Phone = this._customer.Phone;
            this.Fax = this._customer.Fax;
            this.Code = this._customer.Code;

            try
            {
                if (!String.IsNullOrEmpty(this._customer.LogoPath))
                {
                    string logoPath = this._contextCBIRepository.GetFolderLogoPath(this._customer);
                    if (Directory.Exists(logoPath))
                    {
                        string finalPath = Path.Combine(logoPath, this._customer.LogoPath);
                        if (File.Exists(finalPath))
                        {
                            FileInfo fi = new FileInfo(finalPath);

                            byte[] byteArray = UtilsImage.FromFileToByteArray(finalPath);
                            if (byteArray != null && byteArray.Length > 0)
                                this.Image = UtilsImage.ImageFromBuffer(byteArray);

                            this.FilePath = fi.FullName;
                        }
                    }
                }
                else
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(@"pack://application:,,,/Count4U.Media;component/Icons/photo.png");
                    img.EndInit();

                    this.Image = img;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("CustomerSet", exc);
            }

			this.CreateProfileAutomatically = false;
			if (string.IsNullOrWhiteSpace(this._customer.ComplexAdapterParametr3) == false)
			{
				this.CreateProfileAutomatically = this._customer.ComplexAdapterParametr3 == "1" ? true : false;
			}

        }

        public void ApplyChanges()
        {
            this._customer.Code = this.Code;
            this._customer.Name = this.Name;
            this._customer.Description = this.Description;
            this._customer.ImportCatalogAdapterParms = this.Params;
            // this._customer.ContactPerson = this.ContactPerson;		//this._customer.ContactPerson; занято на Selected Adapter import  Family
            this._customer.Mail = this.Mail;

            this._customer.Address = Helpers.AddressParseIntoSingle(new List<string> { this.Address1, this.Address2 }, 200);

            this._customer.Phone = String.IsNullOrEmpty(this.Phone) ? String.Empty : this.Phone.Trim(new[] { '+' });
            this._customer.Fax = String.IsNullOrEmpty(this.Fax) ? String.Empty : this.Fax.Trim(new[] { '+' });
            this._customer.ComplexAdapterParametr3 = this.CreateProfileAutomatically == true ? "1" : "0";

            if (this.IsNewMode)
            {
                this._customer.DBPath = this._customer.Code;
                //if (this.AskProfile == true)
                //{
                //	string rootFonderOnFtp = this._customerRepository.Connection.RootFolderFtp(); //mINV
                //	this._ftpFolderProfile.CustomerProfileCreate(this._customer, rootFonderOnFtp);
                //}
            }
            SaveLogoToCustomer();
        }

        public FtpCommandResult CheckFtpAfterApplyChanges()
        {
           // bool ret = true;
            FtpCommandResult ftpCommandResult = new FtpCommandResult();
            ftpCommandResult.Successful = SuccessfulEnum.Successful;
             if (this.IsNewMode)
            {
                this._customer.DBPath = this._customer.Code;
                if (this.AskProfile == true)
                {
                    string rootFonderOnFtp = this._customerRepository.Connection.RootFolderFtp(); //mINV
                    this._ftpFolderProfile.CustomerProfileCreate(this._customer, rootFonderOnFtp, ref ftpCommandResult);
                }
            }
            return ftpCommandResult;
        }


        //private void ProfileCreate(Customer newCustomer)
        //{
        //	using (new CursorWait("ProfileBuild"))
        //	{
        //		//string messageText = RemoteCreateProfileObject(base.CurrentCustomer,
        //		//@"http://api.inv.wp-funnel.com/v1/c4u/customers",
        //		//Encoding.UTF8);

        //		string host = _userSettingsManager.HostGet().Trim('\\');
        //		string user = _userSettingsManager.UserGet();
        //		string password = _userSettingsManager.PasswordGet();
        //		string messageCreateFolder = "";
        //		string exportToPDAFolder = this._customerRepository.Connection.ExportToPDAFolder().Trim('\\');

        //		try
        //		{
        //			messageCreateFolder = this._ftpFolderProfile.GreateFolderOnFtpInExportToPDA(exportToPDAFolder, newCustomer.Code);
        //			string folderCustomerCode = exportToPDAFolder + @"\" + newCustomer.Code;
        //			messageCreateFolder = messageCreateFolder +
        //				this._ftpFolderProfile.GreateFolderOnFtpInExportToPDA(folderCustomerCode, "profile");

        //		}
        //		catch (Exception exc)
        //		{
        //			_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
        //		}

        //		string messageResponse = "Response : " + Environment.NewLine;
        //		messageResponse = messageResponse + ProfileRESTRepository.RemoteCreateProfileCustomer(newCustomer,
        //			@"http://api.inv.wp-funnel.com/v1/c4u/customers", Encoding.UTF8);

        //		string messageText = messageCreateFolder + messageResponse;
        //		_logger.Info("ProfileCreate : " + messageText);
        //	}
        //}

        public void ClearLogo()
        {
            FilePath = String.Empty;
            Image = null;
        }

        private void SaveLogoToCustomer()
        {
            try
            {
                string logoPath = this._contextCBIRepository.GetFolderLogoPath(this._customer);

                //clear previous  

                if (String.IsNullOrEmpty(this._filePath))
                {
                    string prevLogName = this._customer.LogoPath;
                    if (!String.IsNullOrEmpty(prevLogName) && Directory.Exists(logoPath))
                    {
                        string finalPath = Path.Combine(logoPath, prevLogName);
                        if (File.Exists(finalPath))
                        {
                            File.Delete(finalPath);
                        }
                    }

                    this._customer.LogoPath = String.Empty;
                }
                else
                {
                    if (Directory.Exists(logoPath) && File.Exists(this._filePath))
                    {
                        DirectoryInfo di = new DirectoryInfo(logoPath);
                        FileInfo fi = new FileInfo(this._filePath);

                        string name = this._customer.Code;
                        string nameWithExtension = String.Format("{0}{1}", name, fi.Extension);

                        //string finalPath = Path.Combine(di.FullName, fi.Name);
                        string finalPath = Path.Combine(di.FullName, nameWithExtension);

                        try
                        {
                            //check that files are same
                            FileInfo finalInfo = new FileInfo(finalPath);
                            if (finalInfo.FullName != fi.FullName || finalInfo.Length != fi.Length)
                            {
                                File.Copy(this._filePath, finalPath, true);
                            }
                        }
                        catch (Exception exc)
                        {
                            _logger.ErrorException("File.Copy(this._filePath, finalPath, true);", exc);
                        }
                        //this._customer.LogoPath = fi.Name;
                        this._customer.LogoPath = nameWithExtension;
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Applying log to customer", exc);
            }
        }

        private void BrowseCommandExecuted()
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();
            notification.DefaultExt = ".jpg";
            notification.Filter =
                "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

            this._fileChooseDilogRequest.Raise(notification, this.FileUploadProcess);
        }

        private bool BrowseCommandCanExecute()
        {
            return !IsReadOnly;
        }

        private void FileUploadProcess(OpenFileDialogNotification notification)
        {
            if (notification.IsOK == false) return;

            this.FilePath = String.Empty;

            _logger.Info("FileUploadProcess started");

            try
            {
                string path = notification.FileName;

                this.FilePath = path;

                if (String.IsNullOrEmpty(IsPicOk()))
                {
                    this._imgBytes = UtilsImage.FromFileToByteArray(path);

                    if ((this._imgBytes != null)
                        && (this._imgBytes.Count() > 0))
                    {
                        this.Image = UtilsImage.ImageFromBuffer(this._imgBytes);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBoxNotification not = new MessageBoxNotification
                {
                    Image = MessageBoxImage.Error,
                    Content = String.Format(Localization.Resources.Msg_Incorrect_File_Format),
                    Settings = this._userSettingsManager,
                };
                this._messageBoxRequest.Raise(not);

                _logger.ErrorException("FileUploadProcess", exc);
            }
        }

        private void ClearImageCommandExecuted()
        {
            Image = null;
            FilePath = String.Empty;
            this._imgBytes = null;
        }


        private bool ClearImageCommandCanExecute()
        {
            return !IsReadOnly;
        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Code":
                        if (this._isNewMode)
                        {
                            string validation = CustomerValidate.CodeValidate(_code);
                            if (!String.IsNullOrEmpty(validation))
                                return validation;

                            if (this.IsCodeUnique() == false)
                                return Count4U.Model.ValidateMessage.Customer.CodeUnique;

                            if (UtilsConvert.HaveDosHebrewCharInWord(_code))
                            {
                                return Localization.Resources.Bit_HebrewCharsAreNotAllowed;
                            }
                        }
                        break;
                    case "Mail":
                        return CustomerValidate.MailValidate(this._mail);
                    case "Phone":
                        return CustomerValidate.PhoneValidate(this._phone);
                    case "Fax":
                        return CustomerValidate.FaxValidate(this._fax);
                    case "FilePath":
                        return IsPicOk();
                    case "Name":
                        return CustomerValidate.NameValidate(this._name);
                    case "ContactPerson":
                        return CustomerValidate.ContactPersonValidate(this._contactPerson);
                }
                return null;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        public bool IsFormValid()
        {
            if (IsCodeUnique() &&
                IsEmailValid() &&
                IsPhoneValid(Phone) &&
                IsFaxValid(Fax) &&
                String.IsNullOrEmpty(IsPicOk()) &&
                CustomerValidate.NameValidate(_name) == String.Empty &&
                CustomerValidate.ContactPersonValidate(_contactPerson) == String.Empty)
                return true;

            return false;
        }

        private bool IsCodeUnique()
        {
            if (!this._isNewMode)
                return true;

            if (String.IsNullOrWhiteSpace(this._code))
                return false;

            if (_customers == null)
            {
                SelectParams sp = new SelectParams();
                _customers = this._customerRepository.GetCustomers(sp);
            }

            return !_customers.Any(r => r.Code == this._code);
        }

        private bool IsEmailValid()
        {
            if (String.IsNullOrEmpty(Mail) == true) return true;

            return CustomerValidate.MailValidate(this._mail) == String.Empty;
        }

        private bool IsPhoneValid(string phone)
        {
            if (String.IsNullOrEmpty(phone))
                return true;

            return CustomerValidate.PhoneValidate(phone) == String.Empty;
        }

        private bool IsFaxValid(string fax)
        {
            if (String.IsNullOrEmpty(fax))
                return true;

            return CustomerValidate.FaxValidate(fax) == String.Empty;
        }

        private bool CodeGenerateCommandCanExecute()
        {
            return !IsReadOnly && !IsCodeReadOnly;
        }

        private void CodeGenerateCommandExecuted()
        {
            do
            {
                this._code = Utils.CodeNewGenerate(4);
            } while (!IsCodeUnique());

            RaisePropertyChanged(() => Code);
        }

        private string IsPicOk()
        {
            if (String.IsNullOrEmpty(this._filePath))
                return String.Empty;

            if (!File.Exists(this._filePath))
                return Count4U.Model.ValidateMessage.Customer.PicFileMissed;

            FileInfo fi = new FileInfo(this._filePath);
            if (UtilsConvert.HaveDosHebrewCharInWord(fi.Name))
            {
                return Localization.Resources.Bit_HebrewCharsAreNotAllowed;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(this._filePath)))
                {
                    System.Drawing.Image.FromStream(ms);
                }
            }
            catch (Exception)
            {
                return Count4U.Model.ValidateMessage.Customer.PicFileInvalid;
            }

            return String.Empty;
        }
    }
}