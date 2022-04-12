using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Ftp;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.Web;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.DomainObject
{
    public class CustomerDashboardPartViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ICustomerRepository _customerRepository;
        private readonly UICommandRepository _commandRepository;
        private readonly IRegionManager _regionManager;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _moreCommand;
        private readonly DelegateCommand _propertiesCommand;
        private readonly DelegateCommand _packCommand;
		private readonly DelegateCommand _profileCommand;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly FtpFolderProFile _ftpFolderProfile;

        private string _name;
        private string _code;
        private string _phone;
        private string _fax;
        private string _mail;
        private BitmapImage _image;

        public CustomerDashboardPartViewModel(
            IContextCBIRepository contextCbiRepository,
            ICustomerRepository customerRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            UICommandRepository commandRepository,
			IUserSettingsManager userSettingsManager,
			FtpFolderProFile ftpFolderProfile)
            : base(contextCbiRepository)
        {
            _commandRepository = commandRepository;
            _regionManager = regionManager;
            _customerRepository = customerRepository;
            _eventAggregator = eventAggregator;
			_userSettingsManager = userSettingsManager;
			_ftpFolderProfile = ftpFolderProfile;					
            _editCommand = commandRepository.Build(enUICommand.Edit, EditCommandExecuted);
            _moreCommand = _commandRepository.Build(enUICommand.More, MoreCommandExecuted);

            _propertiesCommand = _commandRepository.Build(enUICommand.Properties, PropertiesCommandExecuted);
            _packCommand = _commandRepository.Build(enUICommand.Pack, PackCommandExecuted);
			_profileCommand = _commandRepository.Build(enUICommand.Profile, ProfileCommandExecuted);
        }

		public DelegateCommand ProfileCommand
		{
			get { return _profileCommand; }
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

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                RaisePropertyChanged(() => Phone);
            }
        }

        public string Fax
        {
            get { return _fax; }
            set
            {
                _fax = value;
                RaisePropertyChanged(() => Fax);
            }
        }

        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                RaisePropertyChanged(() => Mail);
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public DelegateCommand PropertiesCommand
        {
            get { return _propertiesCommand; }
        }

        public DelegateCommand PackCommand
        {
            get { return _packCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<CustomerEditedEvent>().Subscribe(CustomerEdited);

            Build();
        }    

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<CustomerEditedEvent>().Unsubscribe(CustomerEdited);
        }

        private void Build()
        {
            Customer customer = GetCustomer();

            if (customer == null) return;

            try
            {

                Name = customer.Name;
                Code = customer.Code;
                Phone = customer.Phone;
                Fax = customer.Fax;
                Mail = customer.Mail;

                if (!String.IsNullOrEmpty(customer.LogoPath))
                {
                    string logoPath = base.ContextCBIRepository.GetFolderLogoPath(customer);
                    if (Directory.Exists(logoPath))
                    {
                        string finalPath = Path.Combine(logoPath, customer.LogoPath);
                        if (File.Exists(finalPath))
                        {
                            FileInfo fi = new FileInfo(finalPath);

                            byte[] byteArray = UtilsImage.FromFileToByteArray(finalPath);
                            if (byteArray != null && byteArray.Length > 0)
                                this.Image = UtilsImage.ImageFromBuffer(byteArray);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
            }
        }

        private void EditCommandExecuted()
        {
            this._eventAggregator.GetEvent<CustomerEditEvent>().Publish(new CustomerEditEventPayload() { Customer = GetCustomer(), Context = CBIContext.Main });                    
        }

        private Customer GetCustomer()
        {
            return _customerRepository.GetCustomerByCode(base.CurrentCustomer.Code);
        }

        private void CustomerEdited(Customer customer)
        {
            Build();
        }

		public void ProfileCommandExecuted() //TODO 		 exportToPDAFolder form   _userSettingsManager
		{
			if (base.CurrentCustomer == null)
			{
				UtilsMisc.ShowMessageBox("CustomerProfileCreate ERROR : Customer is Null  ",
					MessageBoxButton.OK, MessageBoxImage.Error, _userSettingsManager);
				return;
			}

			MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_CreateProfile,
				MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);
			if (messageBoxResult == MessageBoxResult.No) return;
 
			//Customer currentCustomer = base.CurrentCustomer;
            Customer currentCustomer = this._contextCBIRepository.GetCustomerByCode(base.CurrentCustomer.Code);
//            Branch currentBranch = this._contextCBIRepository.GetBranchByCode(auditConfig.BranchCode);

            string rootFonderOnFtp = this._customerRepository.Connection.RootFolderFtp(); //mINV
	
			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";

				try
				{
					//FTP
					//mINV\Customer\<CustomerCode>
					//rootFonderOnFtp = "mINV" 
					string folderForCustomerObject = base._contextCBIRepository.BuildLongCodesPath(base.CurrentCustomer);
					this._ftpFolderProfile.CreatePathOnFtp(rootFonderOnFtp, folderForCustomerObject, ref messageCreateFolder);
				}
				catch (Exception exc)
				{
					_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
					UtilsMisc.ShowMessageBox("In process create Folder on FTP, happens ERROR : " + exc.Message,
					MessageBoxButton.OK, MessageBoxImage.Error, _userSettingsManager);
					return;
				}

                // WEB
                FtpCommandResult ftpCommandResult = new FtpCommandResult();
                string messageResponse = this._ftpFolderProfile.WebCreateProfileCustomer(currentCustomer, ref ftpCommandResult);

				string messageText = messageCreateFolder + messageResponse;
				_logger.Info("ProfileCreate : " + messageText);

				MessageBoxResult result = UtilsMisc.ShowMessageBox(messageText,
					MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
			}
		}

		
        private void MoreCommandExecuted()
        {
            UriQuery query = new UriQuery();            
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsNavigate.CustomerChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        private void PackCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextCustomer);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
            UtilsNavigate.PackOpen(this._regionManager, uriQuery);
        }

        private void PropertiesCommandExecuted()
        {
            ObjectPropertiesViewEventPayload payload = new ObjectPropertiesViewEventPayload();
            payload.Context = CBIContext.Main;
            payload.DbContext = Common.NavigationSettings.CBIDbContextCustomer;

            this._eventAggregator.GetEvent<ObjectPropertiesViewEvent>().Publish(payload);
        }
    }
}