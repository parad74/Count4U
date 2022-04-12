using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using NLog;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Count4U.Model.Lib.MultiPoint;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Repository.Product.WrapperMulti;
using System.Reactive.Linq;
using Count4U.Common.Helpers.Ftp;
using System.Net;
using Count4U.Common.Constants;
using Count4U.Model.Audit;
using Count4U.Model.Main;
using Count4U.Common.Web;
using System.Xml.Linq;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model.Interface.Main;

namespace Count4U.Common.Web
{
	public class ToFtpViewModel : CBIContextBaseViewModel
    {
		protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();//: BasePdaViewModel
		private readonly ILog _logImport;
		private readonly IZip _zip;
		protected bool _isChecked;
		protected bool _onlyZip;
		private readonly FtpFolderProFile _ftpFolderProfile;

		private string _host;
		private string _user;
		private string _password;
		private bool _enableSsl;
		private string _toFolder;
		private string _hostRootFolder; // @"mINV";
		private string _toPdaFolder;
		private string _fromPdaFolder;
		private string _toPdaPath;
		private string _fromPdaPath;
		private string _relativeObjectPath;

		//private string _hostToApp = @"ToApp";	// ToPDA
		private string FromFolder = ""; //ЗАглушка !!!
		private string _fromPath;
		private string _fromCustomerProfilePath;
		private string _fromInventorProfilePath;
		private string _infoString;

		protected readonly DelegateCommand _sendToFtpCommand;
		protected readonly IDBSettings _dbSettings;
		protected readonly IEventAggregator _eventAggregator;
		protected readonly IWrapperMultiRepository _wrapperMultiRepository;
		protected readonly IUserSettingsManager _userSettingsManager;
		protected readonly IServiceLocator _serviceLocator;
		protected readonly ICustomerConfigRepository _customerConfigRepository;
		
		protected string _execButtonText;
		protected readonly DelegateCommand _closeCommand;
		protected readonly DelegateCommand _openCommand;
		protected readonly DelegateCommand _openProfileCommand;
		protected readonly DelegateCommand _openFtpCommand;
		
		protected readonly ObservableCollection<FromFtpItemViewModel> _items;
		protected bool _isProgressBarVisible;
		protected bool _isExecButtonVisible;

		protected IConnectionDB _connection;

		protected readonly InteractionRequest<OpenFolderDialogNotification> _openFolderRequest;

		protected readonly DelegateCommand _browseCommand;

		protected IObservable<long> observCountingChecked;
		protected IDisposable disposeObservCountingChecked;

		protected bool _includeProfile;

		public ToFtpViewModel(
			IContextCBIRepository contextCbiRepository,
			IDBSettings dbSettings,
			IEventAggregator eventAggregator,
			IServiceLocator serviceLocator,
			IUserSettingsManager userSettingsManager,
			IConnectionDB connection,
			ICustomerConfigRepository customerConfigRepository,
			IZip zip,
			ILog logImport,
			FtpFolderProFile ftpFolderProfile)
			: base(contextCbiRepository)
		{
			this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._eventAggregator = eventAggregator;
			this._dbSettings = dbSettings;
			this._zip = zip;
			this._logImport = logImport;
			this._ftpFolderProfile = ftpFolderProfile;
			this._customerConfigRepository = customerConfigRepository;
			
			this._onlyZip = false;
			this._includeProfile = true;
			this._openFolderRequest = new InteractionRequest<OpenFolderDialogNotification>();

			this._sendToFtpCommand = new DelegateCommand(this.SendToFtpCommandExecuted, this.SendToFtpCommandCanExecute);
			this._openCommand = new DelegateCommand(this.OpenCommandExecuted, this.OpenCommandCanExecute);
			this._openProfileCommand = new DelegateCommand(this.OpenProfileCommandExecuted, this.OpenProfileCommandCanExecute);
			this._openFtpCommand = new DelegateCommand(this.OpenFtpCommandExecuted, this.OpenFtpCommandCanExecute);
			this._execButtonText = Localization.Resources.View_UploadToPda_btnStart;
			this._items = new ObservableCollection<FromFtpItemViewModel>();
			this._closeCommand = new DelegateCommand(CloseCommandExecute);
			this._connection = connection;

		//	Host = userSettingsManager.HostGet().Trim('\\');
			
			this.Host = _userSettingsManager.HostFtpGet(out _enableSsl);
			this.User = userSettingsManager.UserGet();
			this.Password = userSettingsManager.PasswordGet();

		}

		public bool IncludeProfile
		{
			get { return this._includeProfile; }
			set
			{
				this._includeProfile = value;
				RaisePropertyChanged(() => IncludeProfile);
			}
		}

		public ILog LogImport
		{
			get { return _logImport; }
		} 

		public DelegateCommand OpenFtpCommand
		{
			get { return _openFtpCommand; }
		}

		
		private void OpenFtpCommandExecuted()
		{
			if (String.IsNullOrEmpty(this.FromPath))
				return;
			Utils.OpenFtpFolderInExplorer(Host, this.ToFolder);
		}

		private bool OpenFtpCommandCanExecute()
		{
			if (String.IsNullOrEmpty(this.ToFolder))
				return false;
			return true;
		}

		public DelegateCommand OpenCommand
		{
			get { return _openCommand; }
		}

		public DelegateCommand OpenProfileCommand
		{
			get { return _openProfileCommand; }
		}
		

		//void OpenFtpFolderInExplorer(string host, string user, string password, string folder)

		private void OpenCommandExecuted()
		{
			if (String.IsNullOrEmpty(this.FromPath))
				return;

			//if (_isDirectory)
			//{
			Utils.OpenFolderInExplorer(this.FromPath);
			//}
			//else
			//{
			//	this.OpenPathAsFolder(_path);
			//}
		}

		
		private bool OpenCommandCanExecute()
		{
			if (String.IsNullOrEmpty(this.FromPath))
				return false;

			//if (_isDirectory)
			//{
			return this.IsPathOkForOpenAsFolder(this.FromPath);
			//}
			//else
			//{
			//	return this.IsPathOkForOpenAsFolder(_path);
			//}
		}


		private void OpenProfileCommandExecuted()
		{
			if (String.IsNullOrEmpty(this.FromCustomerProfilePath))
				return;

			//if (_isDirectory)
			//{
			Utils.OpenFolderInExplorer(this.FromCustomerProfilePath);
			//}
			//else
			//{
			//	this.OpenPathAsFolder(_path);
			//}
		}

		private bool OpenProfileCommandCanExecute()
		{
			if (String.IsNullOrEmpty(this.FromCustomerProfilePath))
				return false;

			//if (_isDirectory)
			//{
			return this.IsPathOkForOpenAsFolder(this.FromCustomerProfilePath);
			//}
			//else
			//{
			//	return this.IsPathOkForOpenAsFolder(_path);
			//}
		}

		protected bool IsPathOkForOpenAsFolder(string path)
		{
			try
			{
				if (String.IsNullOrEmpty(path))
					return false;

				if (Directory.Exists(path) == true) return true; //add

				string directory = Path.GetDirectoryName(path);

				if (String.IsNullOrWhiteSpace(directory))
					return false;

				return Directory.Exists(directory);
			}
			catch
			{
				return false;
			}
		}

		protected void OpenPathAsFolder(string path)
		{
			try
			{
				if (String.IsNullOrEmpty(path))
					return;

				string directory = Path.GetDirectoryName(path);

				if (String.IsNullOrWhiteSpace(directory))
					return;

				if (!Directory.Exists(directory)) return;

				Utils.OpenFolderInExplorer(directory);
			}
			catch
			{
				return;
			}
		}

		

		public IConnectionDB Connection
		{
			get { return this._connection; }
			set { this._connection = value; }
		}

		//mINV
		public string HostRootFolder
		{
			get { return _hostRootFolder; }
			set { _hostRootFolder = value; }
		}

		public string ToPdaFolder
		{
			get { return _toPdaFolder; }
			set { _toPdaFolder = value; }
		}

		public string FromPdaFolder
		{
			get { return _fromPdaFolder; }
			set { _fromPdaFolder = value; }
		}

		public string RelativeObjectPath   
		{
			get { return _relativeObjectPath; }
			set { _relativeObjectPath = value; }
		}

		public string ToPdaPath
		{
			get { return _toPdaPath; }
			set { _toPdaPath = value; }
		}

		public string FromPdaPath
		{
			get { return _fromPdaPath; }
			set { _fromPdaPath = value; }
		}


		public string Host
		{
			get { return _host; }
			set { _host = value;
			RaisePropertyChanged(() => Host);
			}
		}

		public string User
		{
			get { return _user; }
			set { _user = value;
			RaisePropertyChanged(() => User);
			}
		}

		public string Password
		{
			get { return _password; }
			set { _password = value;
			RaisePropertyChanged(() => Password);
			}
		}

		public string ToFolder
		{
			get
			{

				return this._toFolder;
			}
			set
			{
				this._toFolder = value;
				RaisePropertyChanged(() => ToFolder);
			}
		}

		public string FromPath
		{
			get 
			{
				return _fromPath;		 //todo
			}
			set
			{
				_fromPath = value;
				RaisePropertyChanged(() => FromPath);
				this._openCommand.RaiseCanExecuteChanged();
			}
		}


		public string FromCustomerProfilePath
		{
			get 
			{
				return _fromCustomerProfilePath;		 //todo
			}
			set
			{
				_fromCustomerProfilePath = value;
				RaisePropertyChanged(() => FromCustomerProfilePath);
				this._openProfileCommand.RaiseCanExecuteChanged();
			}
		}



		public string FromInventorProfilePath
		{
			get 
			{
				return _fromInventorProfilePath;		 //todo
			}
			set
			{
				_fromInventorProfilePath = value;
				RaisePropertyChanged(() => FromInventorProfilePath);
			}
		}


		public string InfoString
		{
			get 
			{
				return _infoString;		 //todo
			}
			set
			{
				_infoString = value;
				RaisePropertyChanged(() => InfoString);
			}
		}

		public string DownloadButtonText
		{
			get { return this._execButtonText; }
		}

	
		public DelegateCommand SendToFtpCommand
		{
			get { return this._sendToFtpCommand; }
		}
	
		public bool IsChecked
		{
			get { return this._isChecked; }
			set
			{
				this._isChecked = value;
				RaisePropertyChanged(() => IsChecked);
				foreach (FromFtpItemViewModel item in this._items)
					{
						item.IsChecked = this._isChecked;
					}
				_sendToFtpCommand.RaiseCanExecuteChanged();
			}
		}

		public bool OnlyZip
		{
			get { return this._onlyZip; }
			set
			{
				this._onlyZip = value;
				RaisePropertyChanged(() => OnlyZip);
				Build(false);
			}
		}

		

		public ObservableCollection<FromFtpItemViewModel> Items
		{
			get { return this._items; }
		}

		public DelegateCommand CloseCommand
		{
			get { return this._closeCommand; }
		}

		public string ExecButtonText
		{
			get { return this._execButtonText; }
			set
			{
				this._execButtonText = value;
				RaisePropertyChanged(() => ExecButtonText);
			}
		}

		public bool IsProgressBarVisible
		{
			get { return this._isProgressBarVisible; }
			set
			{
				this._isProgressBarVisible = value;
				RaisePropertyChanged(() => IsProgressBarVisible);
			}
		}

		public bool IsExecButtonVisible
		{
			get { return this._isExecButtonVisible; }
			set
			{
				this._isExecButtonVisible = value;
				RaisePropertyChanged(() => IsExecButtonVisible);
			}
		}


		//Path on comp
		protected void Build(bool isChecked)
		{
			this._items.Clear();
			this.FromCustomerProfilePath = this.GetImportFolderCustomerPath().Trim('\\') + @"\Profile";
			if (System.IO.Directory.Exists(this.FromCustomerProfilePath) == false)
			{
				Directory.CreateDirectory(this.FromCustomerProfilePath);
			}

			this.FromInventorProfilePath = this.GetImportFolderInventorPath().Trim('\\') + @"\Profile";
			if (System.IO.Directory.Exists(this.FromInventorProfilePath) == false)
			{
				Directory.CreateDirectory(this.FromInventorProfilePath);
			}

			this.FromPath = this.GetExportToPDAFolderPath(true);
			if (String.IsNullOrEmpty(this.FromPath) == true) return;
			if (System.IO.Directory.Exists(this.FromPath) == false) return;
	
			DirectoryInfo dir = new System.IO.DirectoryInfo(this.FromPath);
			FileInfo[] sourceFiles = dir.GetFiles();

			foreach (FileInfo fi in sourceFiles)
			{
				if (fi == null) continue;
				if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
				string fileName = fi.Name;
				bool addFile = false;
				if (_onlyZip == true)
				{
					if (Path.GetExtension(fileName) == ".zip")
					{
						addFile = true;
					}
				}
				else
				{
					addFile = true;
				}

				if (addFile == true)
				{
					FromFtpItemViewModel item = new FromFtpItemViewModel(this._serviceLocator);
					item.File = fileName;
					try
					{
						item.DateTimeCreated = fi.CreationTime;
						item.Date = fi.CreationTime.ToShortDateString() + "  " + fi.CreationTime.ToShortTimeString();
					}
					catch{}

					item.IsChecked = isChecked;
					item.IsCheckedEnabled = true;
					this._items.Add(item);
				}
			}

			this.LogImport.AddSimple(MessageTypeEnum.TraceProvider, String.Format("In data foder  [{0}] files", this._items.Count.ToString()));

			_sendToFtpCommand.RaiseCanExecuteChanged();
		}

		

		protected void CloseCommandExecute()
		{
			//this.BeforeClose();

			_eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
		}

		protected void BeforeClose()
		{
			using (new CursorWait())
			{
				try
				{
					_logger.Info("CloseCommandExecute start");
		
					_logger.Info("CloseCommandExecute:: Done() finish");
				}
				catch (Exception exc)
				{
					_logger.ErrorException("CloseCommandExecute :: StopButtonPress() + Done()", exc);
				}
			}
		}

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			InitProperty(base.CurrentInventor, false, base.State);
			RaisePropertyChanged(() => ToFolder);

			ExportCommandInfo infoExportDefault = UtilsExport.GetExportPdaCommandInfoDefaultData(ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter, this._userSettingsManager);
			//Customer customer = base.CurrentCustomer;
			if (base.State.CurrentCustomer != null)
			{
				string adapter = base.State.CurrentCustomer.ExportCatalogAdapterCode;
				ExportCommandInfo infoCustomer =
					UtilsExport.GetExportPdaCustomerData(this._customerConfigRepository, base.State.CurrentCustomer, infoExportDefault, adapter);
				infoExportDefault = infoCustomer;
				IncludeProfile = infoExportDefault.IncludeProfile;
			}

			observCountingChecked = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)).Select(x => x);
            disposeObservCountingChecked = observCountingChecked.Subscribe(CountingChecked);

		}

		public void InitCBIState(CBIState state)
		{
			if (base.State == null) base.State = state;
		}

		public void InitProperty(Inventor currentInventor, bool isChecked, CBIState state)
		{
			if (base.State == null) base.State = state;
			//<root>>\mINV\Inventor\2018\1\1\f600da78-e211-48b8-a1db-fef077fea016\ToPda

			//string relativeDb = base._contextCBIRepository.BuildRelativeDbPath(base.CurrentInventor);
			string relativeDb = base._contextCBIRepository.BuildLongCodesPath(currentInventor);
			this.ToPdaFolder = relativeDb.Trim('\\') + @"\" + FtpFolderName.ToPda;		//Inventor\2018\1\1\<InventorCode>\ToPda
			this.FromPdaFolder = relativeDb.Trim('\\') + @"\" + FtpFolderName.FromPda;		//Inventor\2018\1\1\<InventorCode>\FromPda
			this.HostRootFolder = this._connection.RootFolderFtp();	   //mINV
			this.ToPdaPath = this._connection.RootFolderFtp(this.ToPdaFolder); //	 \mINV\Inventor\2018\1\1\<InventorCode>\ToPda
			this.FromPdaPath = this._connection.RootFolderFtp(this.FromPdaFolder); //	 \mINV\Inventor\2018\1\1\<InventorCode>\ToPda
			this.RelativeObjectPath = this._connection.RootFolderFtp(relativeDb);  //	 \mINV\Inventor\2018\1\1\<InventorCode>

			//this.HostRootFolder = this._connection.RootComplexDataFolderFtp(); //mINV/ComplexData
			//this.ToPdaPath	= this._connection.RootComplexDataFolderFtp(this.ToPdaFolder); //	 \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>\ToPda
			//this.FromPdaPath = this._connection.RootComplexDataFolderFtp(this.FromPdaFolder); //	 \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>\ToPda
			//this.RelativeObjectPath = this._connection.RootComplexDataFolderFtp(relativeDb);  //	 \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>

			ToFolder = this.ToPdaPath.Trim('\\') + @"\" + FtpFolderName.Process;
	

			////tet
			////\mINV\ComplexData\Inventor\2018\1\1\<InventorCode>\ToPda\Process
			//string ToPdaProcessPath = this.ToPdaPath.Trim('\\') + @"\" + "Process";
			//DateTime today = DateTime.Now;
			//string ThreadCode = base.State.CurrentInventor.BranchCode;	 		//ThreadCode пока BranchCode
			//string todayString = today.Year + "_" + today.Month.ToString().PadLeft(2, '0') + "_" + today.Day.ToString().PadLeft(2, '0');
			////<root>\mINV\ComplexData\Inventor\YYYY\MM\DD\<InventorCode>\ToPDA\<ThreadCode>\<YYYY_MM_DD>\
			//string copyFolder = ThreadCode + @"\" + todayString;
			//string ToPdaCopyPath = this.ToPdaPath.Trim('\\') + @"\" + copyFolder;
			//string messageCreateFolder = "";
			//using (new CursorWait())
			//{
			//	FtpClient client = new FtpClient(Host, User, Password);
			//	// create folder \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>\ToPda
			//	// will be root	for work
			//	messageCreateFolder = client.CreatePathOnFtp(Host, this.HostRootFolder, this.ToPdaFolder, ref messageCreateFolder);
			//	// now  root for process	 ToPdaPath
			//	messageCreateFolder = client.CreatePathOnFtp(Host, this.ToPdaPath, "Process", ref messageCreateFolder);
			//	//  now  root for copy 	 ToPdaCopyPath
			//	messageCreateFolder = client.CreatePathOnFtp(Host, this.ToPdaPath, copyFolder, ref messageCreateFolder);
			//}
			////end test

			using (new CursorWait())
			{
				this.Build(isChecked);
			}
		}

		public void CountingChecked(long x)
		{
		//	bool conting = true; //слушать или нет
			_sendToFtpCommand.RaiseCanExecuteChanged();

			//if (conting == false)
			//{
			//	return;
			//}

			//if (this._items == null) return;
			//if (this._items.Count() < 1)
			//{
			//	return;
			//}

		//	var itemsCheck = this._items.Where(k => k.IsChecked == true ).Select(k => k).ToList();

			//if (itemsCheck.Count() < 1)
			//{
			//	//base.QuantityChecked = 0;
			//}
			//else
			//{
			//	//base.QuantityChecked = itemsCheck.LongCount();
			//}
			
		}


		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
			_logger.Info("OnNavigatedFrom");

			this.BeforeClose();
			if (disposeObservCountingChecked != null) disposeObservCountingChecked.Dispose();
			_logger.Info("OnNavigatedFrom  + Clear");
		}

		public string GetExportToPDAFolderPath(bool withCurrentDomainObject = true)
		{
			if (String.IsNullOrEmpty(base.CBIDbContext) == false)
			{
				object currentDomainObject = base.GetCurrentDomainObject();
				if (currentDomainObject != null)
				{
					return base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, withCurrentDomainObject);
				}
			}
			return String.Empty;
		}

		public string GetImportFolderCustomerPath()
		{
			if (String.IsNullOrEmpty(base.CBIDbContext) == false)
			{
				Customer customer = base.CurrentCustomer;
				if (customer != null)
				{
					return base.ContextCBIRepository.GetImportFolderPath(customer);
				}
			}
			return String.Empty;
		}



		public string GetImportFolderInventorPath()
		{
			if (String.IsNullOrEmpty(base.CBIDbContext) == false)
			{
				Inventor inventor = base.CurrentInventor;
				if (inventor != null)
				{
					return base.ContextCBIRepository.GetImportFolderPath(inventor);
				}
			}
			return String.Empty;
		}

			//string currentInventorCode = "";
			//if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;
			//if (currentInventorCode == "") return;

			//string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора

			//string inDataFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData";
			//if (Directory.Exists(inDataFolder) == false) Directory.CreateDirectory(inDataFolder); 

		private bool SendToFtpCommandCanExecute()
		{
			if (IncludeProfile == true) return true;
				if (this._items.Any(r => r.IsChecked) == false)
				return false;

				return true;
		}

		private void SendToFtpCommandExecuted()
		{
			if (base.State == null) return;
			
			if (this.IncludeProfile == true ) this.SendProfileToFtp(base.State.CurrentInventor);
			this.SendToPDAOnFtp(base.State.CurrentInventor);

			Utils.RunOnUI(() =>
			{
				InfoString = DateTime.Now.ToShortTimeString() + " >> Upload files finished ";
				RaisePropertyChanged(() => InfoString);
			});

			using (new CursorWait())
			{
				this.Build(false);
			}
		}

		public void SendProfileToFtp(Inventor currentInventor)
		{
			if (this.IncludeProfile == false) return;
			if (String.IsNullOrEmpty(this.FromCustomerProfilePath) == true) return;
			if (System.IO.Directory.Exists(this.FromCustomerProfilePath) == false) return;
			string profilePath = this.FromCustomerProfilePath.Trim('\\') + @"\profile.xml";
			if (System.IO.File.Exists(profilePath) == false) return;
			 
			if (System.IO.Directory.Exists(this.FromInventorProfilePath) == false)
			{
				Directory.CreateDirectory(this.FromInventorProfilePath);
			}
			string profileInventorPath = this.FromInventorProfilePath.Trim('\\') + @"\profile.xml";

			if (System.IO.File.Exists(profileInventorPath) == true)
			{
				System.IO.File.Delete(profileInventorPath);
			}
			System.IO.File.Copy(profilePath, profileInventorPath, true);
			UpdateProfile(profileInventorPath);

			//string toFtpPath = this.RelativeObjectPath.Trim('\\') + @"\" + FtpFolderName.Profile;
			string rootFolderOnFtp = _connection.RootFolderFtp(); //mINV
			this._ftpFolderProfile.InventorProfileSendFromLocalPathToFtp(currentInventor, rootFolderOnFtp, this.FromInventorProfilePath, "profile.xml");
		}

		public void UpdateProfile(string profilePath)
		{
			if (System.IO.File.Exists(profilePath) == true)
			{
				XDocument xdoc = XDocument.Load(profilePath);
				XElement infoXElement = xdoc.Descendants("InventoryProcessInformation").FirstOrDefault();
				if (infoXElement == null) return;

				//Customer
				Customer customer = base.CurrentCustomer;
				XElement infoCustomerXElement = infoXElement.Descendants("Customer").FirstOrDefault();
				if (infoCustomerXElement != null) infoCustomerXElement.Remove();

				if (customer != null)
				{
					XElement customerElement = new XElement("Customer",
						new XAttribute("name", customer.Name),
						new XAttribute("code", customer.Code));
					infoXElement.Add(customerElement);
				}

				 //	Branch
				Branch branch = base.CurrentBranch;
				XElement infoBranchXElement = infoXElement.Descendants("Branch").FirstOrDefault();
				if (infoBranchXElement != null) infoBranchXElement.Remove();

				if (branch != null)
				{
					XElement branchElement = new XElement("Branch",
						new XAttribute("name",""),
						new XAttribute("code", branch.Code));
					infoXElement.Add(branchElement);
				}

				// Inventor
				Inventor inventor = base.CurrentInventor;
				XElement infoInventorXElement = infoXElement.Descendants("Inventor").FirstOrDefault();
				if (infoInventorXElement != null) infoInventorXElement.Remove();
				XElement infoInventorXElementOld = infoXElement.Descendants("Inventory").FirstOrDefault();
				if (infoInventorXElementOld != null) infoInventorXElementOld.Remove();

				if (inventor != null)
				{
					DateTime cd = inventor.CreateDate;
					DateTime id = inventor.InventorDate;
					string inventorDate = cd.ToString("yyyy") + @"/" + cd.Month + @"/" + cd.Day;
					string createDate = id.ToString("yyyy") + @"/" + id.Month + @"/" + id.Day;

					XElement inventorElement = new XElement("Inventory",
						//new XAttribute("name", inventor.Name),
						new XAttribute("code", inventor.Code),
					//	new XAttribute("description", inventor.Description),
						new XAttribute("created_date", createDate),
						new XAttribute("inventor_date", inventorDate));
					
					
					infoXElement.Add(inventorElement);
				}

				xdoc.Save(profilePath);
			}
		}

		public void SendToPDAOnFtp(Inventor currentInventor)
		{
			//Utils.RunOnUI(() =>
			//{
			//	InfoString = "START";
			//	RaisePropertyChanged(() => InfoString);
			//});
			if (currentInventor == null) return;
			string currentInventorCode = currentInventor.Code;

			var itemsCheck = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();
			if (itemsCheck.Count == 0) return;
			int countfile = 0; 
		    string messageCreateFolder = "" ;

			DateTime today = DateTime.Now;
			string prefix = currentInventor.Code.Substring(0, 1);
			string ThreadCode = today.Year + "-" + today.DayOfYear + "~" + prefix;   //base.State.CurrentInventor.BranchCode;	 		//ThreadCode пока BranchCode
			//string todayString = today.Year + "_" + today.Month.ToString().PadLeft(2, '0') + "_" + today.Day.ToString().PadLeft(2, '0');
			//string currentBackupFolder = ThreadCode + @"\" +  todayString;
			string ToPdaProcessPath = "";
			string ToPdaCopyPath = "";
			string ToPdaThreadCodeCopyFolder = "";
			

			string  First = "";
			string  Last = "";
			string  Current = "";

			//string folderForCustomerObject = base._contextCBIRepository.BuildLongCodesPath(base.CurrentCustomer);
			//this._ftpFolderProfile.CreatePathOnFtp(rootFonderOnFtp, folderForCustomerObject, ref messageCreateFolder);
			using (new CursorWait())
			{
				FtpClient client = new FtpClient(Host, User, Password, this._enableSsl);
				// create folder \mINV\Inventor\2018\1\1\<InventorCode>\ToPda
				// will be root	for work
				string ToPdaPath1 = client.CreatePathOnFtp(Host, this.HostRootFolder, this.ToPdaFolder, ref messageCreateFolder);
				//root for process ToPdaPath ==	\mINV\Inventor\2018\1\1\<InventorCode>\ToPda
				//ToPdaProcessPath = \mINV\Inventor\2018\1\1\<InventorCode>\ToPda\Process
				ToPdaProcessPath = client.CreatePathOnFtp(Host, this.ToPdaPath, FtpFolderName.Process, ref messageCreateFolder);
				//root for copy ToPdaCopyPath == \mINV\Inventor\YYYY\MM\DD\<InventorCode>\ToPDA\<ThreadCode>\
				ToPdaThreadCodeCopyFolder = client.CreateThreadCodeFolderOnFtp(Host, this.ToPdaPath, ThreadCode, currentInventorCode, ref messageCreateFolder);
				//root for copy ToPdaCopyPath == \mINV\Inventor\YYYY\MM\DD\<InventorCode>\ToPDA\<ThreadCode>\<YYYY_MM_DD>\	  
				//ToPdaCopyPath  =  client.CreatePathOnFtp(Host, this.ToPdaPath.Trim('\\') + @"\" +ToPdaThreadCodeCopyPath, todayString, ref messageCreateFolder);


				string FromPdaPath1 = client.CreatePathOnFtp(Host, this.HostRootFolder, this.FromPdaFolder, ref messageCreateFolder);
				//root for process FromPdaPath ==	\mINV\Inventor\2018\1\1\<InventorCode>\FromPda
				//ToPdaProcessPath = \mINV\Inventor\2018\1\1\<InventorCode>\FromPda\Process
				string FromPdaProcessPath = client.CreatePathOnFtp(Host, this.FromPdaPath, FtpFolderName.Process, ref messageCreateFolder);


				//root for process ToPdaPath ==	\mINV\Inventor\2018\1\1\<InventorCode>\First
				//First = client.CreatePathOnFtp(Host, this.RelativeObjectPath, FtpFolderName.First, ref messageCreateFolder);
				//root for process ToPdaPath ==	\mINV\Inventor\2018\1\1\<InventorCode>\Last
				//Last = client.CreatePathOnFtp(Host, this.RelativeObjectPath, FtpFolderName.Last, ref messageCreateFolder);
				//root for process ToPdaPath ==	\mINV\Inventor\2018\1\1\<InventorCode>\Current
				//Current = client.CreatePathOnFtp(Host, this.RelativeObjectPath, FtpFolderName.Current, ref messageCreateFolder);
				_logger.Info("CreatePath with Result [" + messageCreateFolder + "]");
			}
 				
			try
			{
				using (new CursorWait())
				{
					FtpClient client = new FtpClient(Host, User, Password, this._enableSsl);
					//--------------- find or create folder on ftp  mINV\\ToApp\customerCode ----------------
					client.uri = Host;

					//ToPdaProcessPath   \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>\ToPda\Process
					//ToPdaCopyPath \mINV\ComplexData\Inventor\YYYY\MM\DD\<InventorCode>\ToPDA\<ThreadCode>\<YYYY_MM_DD>\
					//-------------	send to ftp
					string objectCodeFolder = this.GetExportToPDAFolderPath(true);
					if (Directory.Exists(objectCodeFolder) == false)
					{
						Directory.CreateDirectory(objectCodeFolder);
					}

					//очистить ftp folder
					this.ClearFolderOnFtp(ToPdaProcessPath);

					//backupFolder on Count4U
					string backupFolder = objectCodeFolder + @"\" + ToPdaThreadCodeCopyFolder;
					if (Directory.Exists(backupFolder) == false)
					{
						Directory.CreateDirectory(backupFolder);
					}

					client.uri = Host;
					string result7 = client.ChangeWorkingDirectory(ToPdaProcessPath);
					// to Process	
					List<string> filepathList = new List<string>();
					foreach (var item in itemsCheck)
					{
						string fileName1 = item.File;
						string fileName = item.File;
						if (fileName1.Contains(currentInventorCode) == true)
						{
							fileName = fileName1.Replace(currentInventorCode, ToPdaThreadCodeCopyFolder);
						}
	 			
						File.Copy(objectCodeFolder + @"\" + fileName1, backupFolder + @"\" + fileName, true);
						filepathList.Add(backupFolder + @"\" + fileName);		 // db3 на Count4U 
						countfile++;
   					}
				
					string exportDateTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
					string zipFile = ToPdaThreadCodeCopyFolder + "^" + exportDateTime + ".zip";				//	
					string zipPath = objectCodeFolder + @"\" + ToPdaThreadCodeCopyFolder + @"\" + zipFile;			 //на Count4U 	   
					this._zip.DoZipFile(filepathList, zipPath);

					string toFTPPath = zipFile;
					string result = client.SaveFileToFtp(zipPath, toFTPPath);
					_logger.Info("Save file [" + zipFile + "]" + " to ftp [" + toFTPPath + "]");
					_logger.Info("with Result [" + result + "]");

						

					// to Copy backup		
					client.uri = Host;
					//this.ToPdaPath
					//ToPdaThreadCodeCopyPath
					//FtpFolderName.Process
					string result8 = client.ChangeWorkingDirectory(this.ToPdaPath);
					List<string> fileListinProcess =  GetProfileFilesFromFtp(this.ToPdaPath+ @"\" + FtpFolderName.Process);
					foreach (var item in fileListinProcess)
					{
						string fileName = item;
						string toFTPPath1 = ToPdaThreadCodeCopyFolder + @"\" + fileName;
						string fromFTPPath1 = FtpFolderName.Process + @"\" + fileName;
						string result11 = client.CopyFileOnFtp(fromFTPPath1, toFTPPath1);
						_logger.Info("Save file [" + fromFTPPath1 + "]" + " to ftp [" + toFTPPath1 + "]");
						_logger.Info("with Result [" + result11 + "]");
					}

				}
			}
			catch (Exception exc)
			{
				string message = String.Format("SendToFtp");
				_logger.ErrorException(message, exc);
			}
		}

		public void ClearFolderOnFtp(string deleteFolder)
		{
			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);
			FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(deleteFolder);
			if (statusCode == FtpStatusCode.PathnameCreated)
			{
				List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(deleteFolder, this._host, this._user, this._password);
				if (listInfoDirectory.Count < 1) return;

				foreach (FileDirectoryInfo fi in listInfoDirectory)
				{
					if (fi == null) continue;
					if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
					if (fi.IsDirectory == true) continue;
					string fileName = fi.Name;
					string result =client.DeleteFile(fileName);
				}
			}
		}

		private List<string> GetProfileFilesFromFtp(string fromFtpFolder)
		{
			List<string> filepathList = new List<string>();
			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);
			client.uri = Host;
			FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(fromFtpFolder);		 
			if (statusCode == FtpStatusCode.PathnameCreated)
			{
				List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(fromFtpFolder, this._host, this._user, this._password);
				if (listInfoDirectory.Count >= 1)
				{
					foreach (FileDirectoryInfo fi in listInfoDirectory)
					{
						if (fi == null) continue;
						if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
						if (fi.IsDirectory == true) continue;
						filepathList.Add(fi.Name);
					}
				}
			}
			return filepathList;
		}




		//private void ____oldSendToFtpCommandExecuted()
		//{
		//	string currentInventorCode = "";
		//	if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;
		//	string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора

		//	var itemsCheck = this._items.Where(k => k.IsChecked == true ).Select(k => k).ToList();

		//	if (itemsCheck.Count() < 1) return;
		//	if (currentInventorCode == "") return;

		//	string targetFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData";
		//	if (Directory.Exists(targetFolder) == false) return;
		//	string targetZipFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\zip";
		//	if (Directory.Exists(targetZipFolder) == false) Directory.CreateDirectory(targetZipFolder);
			
		//	FtpClient client = new FtpClient(this._host, this._user, this._password);

		//	try
		//	{
		//		using (new CursorWait())
		//		{
		//			FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(this.FromFolder);
		//			if (statusCode == FtpStatusCode.PathnameCreated)
		//			{
		//				List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(this.FromFolder, this._host, this._user, this._password);
		//				if (listInfoDirectory.Count < 1) return;
		//				bool haveBackupFolder = false;
		//				FileDirectoryInfo backup = listInfoDirectory.FirstOrDefault(k => k.IsDirectory == true && k.Name == "backup");
		//				if (backup != null) haveBackupFolder = true;

		//				//foreach (FileDirectoryInfo fi in listInfoDirectory)
		//				//{
		//				//	if (fi == null) continue;
		//				//	if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
		//				//	if (fi.IsDirectory == true) continue;
		//				foreach (var item in itemsCheck)
		//				{
		//					string fileName = item.File;

		//					string inventorCode = currentInventorCode;
		//					string targetPath = "none";

		//					targetPath = targetFolder + @"\" + fileName; //Inventor
		//					if (System.IO.Path.GetExtension(fileName) == ".zip")
		//					{
		//						targetPath = targetZipFolder + @"\" + fileName; 
		//					}
						
		//					string fromFtpPathProfile = this.FromFolder;
		//					try
		//					{   //Inventor
		//						if (File.Exists(targetPath) == true) File.Delete(targetPath);				 //Count4U

		//						fromFtpPathProfile = this.FromFolder + @"\" + fileName;		  //  на ftp	  (from)
		//						FtpStatusCode result = client.GetFileFromFtp(fileName, targetPath);
		//						_logger.Info("Copy profile.xml from ftpServer[" + fromFtpPathProfile + "]");
		//						_logger.Info("with Result [" + result.ToString() + "]");

		//						if (haveBackupFolder == true)
		//						{
		//							//string result1 = client.SaveFileToFtp(targetPath, @"backup\" + fileName);
		//							string result1 = client.Rename(fileName, @"backup\" + fileName);
		//						}

		//						//string result2 = client.DeleteFile(fileName);
		//					}
		//					catch (Exception exc)
		//					{
		//						string message = String.Format("GetFromFtpCommandExecuted (File.Move to unsurePath) {0} => {1}", fromFtpPathProfile, targetPath);
		//						_logger.ErrorException(message, exc);
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("GetFromFtpCommandExecuted", exc);
		//	}

		//	//============== unzip ========
		//	foreach (var item in itemsCheck)
		//	{
		//		string fileName = item.File;

		//		//string oldFile = targetFolder + @"\" + fileName;
		//		//if (File.Exists(oldFile) == true)
		//		//{
		//		//	File.Delete(oldFile);
		//		//}

		//		string zipFile = targetZipFolder + @"\" + fileName; //Inventor

		//		if (File.Exists(zipFile) == true)
		//		{
		//			this._zip.ReadDb3FromZipFile(zipFile, targetFolder);
		//		}
		//	}
		//	//===============

		//	using (new CursorWait())
		//	{
		//		this.Build();
		//	}
	
		//}



	}
}