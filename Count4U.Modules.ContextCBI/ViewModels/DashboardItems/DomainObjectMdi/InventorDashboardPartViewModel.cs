using System;
using System.IO;
using System.Net;
using System.Text;
//using System.Web.Script.Serialization;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.Web;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
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
    public class InventorDashboardPartViewModel : CBIContextBaseViewModel
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IInventorRepository _inventorRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;	
		private readonly FtpFolderProFile _ftpFolderProfile;

        private string _code;
        private string _path;
        private string _date;
        private string _description;

        private string _branchCodeLocal;
        private string _branchERPCode;
		
		private string _host;
		private string _user;
		private string _password;
		private bool _enableSsl;

        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _moreCommand;
        private readonly UICommandRepository _commandRepository;

        private readonly DelegateCommand _propertiesCommand;
        private readonly DelegateCommand _packCommand;
		private readonly DelegateCommand _profileCommand;
		private readonly DelegateCommand _ftpCommand;

        public InventorDashboardPartViewModel(IContextCBIRepository contextCbiRepository,
            IInventorRepository inventorRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            UICommandRepository commandRepository,
            IUserSettingsManager userSettingsManager,
			FtpFolderProFile ftpFolderProfile)
            : base(contextCbiRepository)
        {
			this._userSettingsManager = userSettingsManager;
			this._commandRepository = commandRepository;
			this._regionManager = regionManager;
			this._eventAggregator = eventAggregator;
			this._inventorRepository = inventorRepository;
			this._ftpFolderProfile = ftpFolderProfile;
			this._editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted);
			this._moreCommand = _commandRepository.Build(enUICommand.More, MoreCommandExecuted);

			this._propertiesCommand = _commandRepository.Build(enUICommand.Properties, PropertiesCommandExecuted);
			this._packCommand = _commandRepository.Build(enUICommand.Pack, PackCommandExecuted);
			this._profileCommand = _commandRepository.Build(enUICommand.Profile, ProfileCommandExecuted);
			this._ftpCommand = _commandRepository.Build(enUICommand.Ftp, this.OpenFtpCommandExecuted, this.OpenFtpCommandCanExecute);

			//Host = userSettingsManager.HostGet().Trim('\\');

			this.Host =  userSettingsManager.HostFtpGet(out _enableSsl);
			this.User = userSettingsManager.UserGet();
			this.Password = userSettingsManager.PasswordGet();
        }


		public string Host
		{
			get { return _host; }
			set
			{
				_host = value;
				RaisePropertyChanged(() => Host);
			}
		}

		public string User
		{
			get { return _user; }
			set
			{
				_user = value;
				RaisePropertyChanged(() => User);
			}
		}

		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				RaisePropertyChanged(() => Password);
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

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
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

        public string BranchCodeLocal
        {
            get { return _branchCodeLocal; }
            set
            {
                _branchCodeLocal = value;
                RaisePropertyChanged(() => BranchCodeLocal);
            }
        }

        public string BranchERPCode
        {
            get { return _branchERPCode; }
            set
            {
                _branchERPCode = value;
                RaisePropertyChanged(() => BranchERPCode);
            }
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


		public DelegateCommand ProfileCommand
        {
			get { return _profileCommand; }
        }

   
		public DelegateCommand FtpCommand
        {
			get { return _ftpCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<InventorEditedEvent>().Subscribe(InventorEdited);

            Build();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<InventorEditedEvent>().Unsubscribe(InventorEdited);
        }

        private void Build()
        {
            Inventor inventor = GetInventor();
            if (inventor == null) return;

            Code = inventor.Code;
            Date = UtilsConvert.DateHebrewConvert(inventor.InventorDate, _userSettingsManager.LanguageGet());
            Path = base.ContextCBIRepository.CreateNewDBPath(inventor, this._code);
            Description = inventor.Description;

            Branch branch = base.ContextCBIRepository.GetBranchByCode(inventor.BranchCode);
            if (branch != null)
            {
                BranchCodeLocal = branch.BranchCodeLocal;
                BranchERPCode = branch.BranchCodeERP;
            }
        }

        private Inventor GetInventor()
        {
            return _inventorRepository.GetInventorByCode(base.CurrentInventor.Code);
        }

        private void InventorEdited(Inventor inventor)
        {
            Build();
        }

        private void EditCommandExecuted()
        {
            this._eventAggregator.GetEvent<InventorEditEvent>().Publish(new InventorEditEventPayload() { Inventor = GetInventor(), Context = CBIContext.History });
        }

        private void MoreCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsNavigate.InventorChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        private void PackCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.PackOpen(this._regionManager, uriQuery);
        }

		private void OpenFtpCommandExecuted()
		{
			string rootFonderOnFtp = this._inventorRepository.Connection.RootFolderFtp(); //mINV
			string folderForInventorObject = base._contextCBIRepository.BuildLongCodesPath(base.CurrentInventor).Trim(@"\".ToCharArray());// +@"\Profile";
	
			//if (String.IsNullOrEmpty(this.FromPath))
			//	return;
			Utils.OpenFtpFolderInExplorer(Host, rootFonderOnFtp + @"\"  +folderForInventorObject);
		}

		private bool OpenFtpCommandCanExecute()
		{
			return true;
		}

		public void ProfileCommandExecuted() //TODO 		 exportToPDAFolder form   _userSettingsManager
		{
			if (base.CurrentCustomer == null)
			{
				UtilsMisc.ShowMessageBox("CustomerProfileCreate ERROR : Customer is Null  ",
					MessageBoxButton.OK, MessageBoxImage.Error, _userSettingsManager);
				return;
			}

			if (base.CurrentInventor == null)
			{
				UtilsMisc.ShowMessageBox("CustomerProfileCreate ERROR : Inventor is Null  ",
					MessageBoxButton.OK, MessageBoxImage.Error, _userSettingsManager);
				return;
			}

			MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_CreateProfile,
				MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);
			if (messageBoxResult == MessageBoxResult.No) return;

			//Customer currentCustomer = base.CurrentCustomer;
			//Branch currentBranch = base.CurrentBranch;
			//Inventor currentInventor = base.CurrentInventor;

			Customer currentCustomer = this._contextCBIRepository.GetCustomerByCode(base.CurrentCustomer.Code);
			Branch currentBranch = this._contextCBIRepository.GetBranchByCode(base.CurrentBranch.Code);
			Inventor currentInventor = this._contextCBIRepository.GetInventorByCode(base.CurrentInventor.Code);

			string rootFonderOnFtp = this._inventorRepository.Connection.RootFolderFtp(); //mINV
			
			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";
			
				try
				{
					//rootFonderOnFtp = "mINV" 
					//mINV\Customer\<CustomerCode>
					string folderForCustomerObject = base._contextCBIRepository.BuildLongCodesPath(base.CurrentCustomer);
					this._ftpFolderProfile.CreatePathOnFtp(rootFonderOnFtp, folderForCustomerObject, ref messageCreateFolder);

					string folderForInventorObject = base._contextCBIRepository.BuildLongCodesPath(base.CurrentInventor).Trim(@"\".ToCharArray()) + @"\Profile";
					this._ftpFolderProfile.CreatePathOnFtp(rootFonderOnFtp, folderForInventorObject, ref messageCreateFolder);
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
				string messageResponse1 = this._ftpFolderProfile.WebCreateProfileCustomer(currentCustomer, ref ftpCommandResult);
				string messageResponse2 = this._ftpFolderProfile.WebCreateProfileInventor(currentCustomer, currentBranch, currentInventor, ref ftpCommandResult);

				string messageText = messageCreateFolder + messageResponse1 + messageResponse2;
				_logger.Info("ProfileCreate : " + messageText);

				MessageBoxResult result = UtilsMisc.ShowMessageBox(messageText,
					MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
			}
		}

		//private void ProfileCommandExecuted()
		//{
		//	MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_CreateProfile,
		//	MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);
		//	if (messageBoxResult == MessageBoxResult.No) return;

		//	using (new CursorWait("ProfileBuild"))
		//	{
		//		//string messageText = RemoteCreateProfileObject(base.CurrentCustomer,
		//			//@"http://api.inv.wp-funnel.com/v1/c4u/customers",
		//			//Encoding.UTF8);

		//		string messageCreateFolder = "Folder for Profile : " + Environment.NewLine;
		//		messageCreateFolder = messageCreateFolder + @"C:\" + Environment.NewLine;

		//		string messageResponse = "Response : " + Environment.NewLine;
		//		messageResponse = ProfileRESTRepository.RemoteCreateProfileInventor(base.CurrentInventor,
		//			@"http://api.inv.wp-funnel.com/v1/c4u/customers/:customer-code/inventories", Encoding.UTF8);

		//		string messageText = messageCreateFolder + messageResponse;

		//		MessageBoxResult result = UtilsMisc.ShowMessageBox(messageText,
		//			MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
		//	}
		//}

		//		Эти заголовки обязательны во всех запросах
		//Accept: application/json
		//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7

		//Для запросов POST/PATCH нужно добавлять ещё этот заголовок
		//Content-Type: application/json


		//http://api.inv.wp-funnel.com/v1/c4u/customers
		//Accept: application/json
		//Content-Type: application/json
		//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7
		//Host: api.inv.wp-funnel.com
		//		{
		//"name": "Customer1",
		//"code": "code1",
		//"email": "Customer@mail.ru",
		//"description": "Customer description"
		//}	
		//private string RemoteCreateProfileObject(Customer customer, string URL, Encoding outEncoding)
		//{
		//	string customerCode = customer.Code;
		//	string customerName = customer.Name;
		//	string customerMail = customer.Mail;
		//	string customerDescription = customer.Description;

		//	object newCustomer = new { name = customerName, code = customerCode, email = customerMail, description = customerDescription };

		//	var retCustomer = new JavaScriptSerializer().Serialize(newCustomer);

		//	string DATA = retCustomer;
		//	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
		//	request.Method = @"POST";
		//	request.Accept = @"application/json";
		//	request.ContentType = @"application/json";
		//	request.Headers.Add("X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7");
		//	request.ContentLength = DATA.Length;

		//	//Accept: application/json
		//	//Content-Type: application/json
		//	//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7
		//	//Host: api.inv.wp-funnel.com
   
		//	StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
		//	requestWriter.Write(DATA);
		//	requestWriter.Close();

		//	try
		//	{
		//		HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

		//		using (StreamReader stream = new StreamReader(
		//			webResponse.GetResponseStream(), outEncoding))
		//		{
		//			 string ret = stream.ReadToEnd();
		//			 return ret;
		//			// stream.Close();
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		Console.Out.WriteLine("-----------------");
		//		Console.Out.WriteLine(e.Message);
		//		return e.Message;
		//	}
		//}

        private void PropertiesCommandExecuted()
        {
            ObjectPropertiesViewEventPayload payload = new ObjectPropertiesViewEventPayload();
            payload.Context = base.Context;
            payload.DbContext = Common.NavigationSettings.CBIDbContextInventor;

            this._eventAggregator.GetEvent<ObjectPropertiesViewEvent>().Publish(payload);
        }
    }
}