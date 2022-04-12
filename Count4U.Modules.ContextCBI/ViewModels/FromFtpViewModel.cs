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
using Count4U.Common.Helpers.Ftp;
using System.Net;
using Count4U.Common.ViewModel.Misc;
using Count4U.Common.Constants;
using Count4U.Model.Audit;
using Count4U.Common.ViewModel.Adapters;
using System.Reactive.Linq;

namespace Count4U.Common.Web
{
	public class FromFtpViewModel : CBIContextBaseViewModel
    {
		protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();//: BasePdaViewModel
		private readonly ILog _logImport;

		private readonly IZip _zip;
		//private string _path;
		//protected int _processValue;
		protected bool _isChecked;
		//protected bool _canBrowse;
		private bool _isBusy;

		private string _host;
		private string _user;
		private string _password;
		private bool _enableSsl;
		private string _fromPdaProcessPath;
		private string _inventoryImagesFolder;
		private string _inventorySignatureFolder;
		
		private string _hostmINV = @"mINV";
		private string _hostFromApp = @"FromApp";
		private string _inventoryImages = @"InventoryImages";
		private string _adapterName;

		private string _hostRootFolder; // @"mINV";
		private string _toPdaFolder;
		private string _fromPdaFolder;
		private string _toPdaPath;
		private string _fromPdaPath;
		private string _relativeObjectPath;
		private string _fromPhotoPath;
		private string _fromSignaturePath;

		protected readonly DelegateCommand _getFromFtpCommand;
		protected readonly IDBSettings _dbSettings;
		protected readonly IEventAggregator _eventAggregator;
		protected readonly IWrapperMultiRepository _wrapperMultiRepository;
		protected readonly IUserSettingsManager _userSettingsManager;
		protected readonly IServiceLocator _serviceLocator;
		protected string _execButtonText;
		protected readonly DelegateCommand _closeCommand;
		protected readonly DelegateCommand _openCommand;
		protected readonly ObservableCollection<FromFtpItemViewModel> _items;
		
		
		protected bool _isProgressBarVisible;
		protected bool _isExecButtonVisible;
		private readonly ObservableCollection<ItemFindViewModel> _sortItems;
		private ItemFindViewModel _sortItemSelected;
		private readonly ObservableCollection<ItemFindViewModel> _folderItems;
		private ItemFindViewModel _folderItemSelected;
		

		protected readonly InteractionRequest<OpenFolderDialogNotification> _openFolderRequest;

		protected readonly DelegateCommand _browseCommand;
		//protected DelegateCommand _execCommand;
		protected readonly DelegateCommand _openFtpCommand;

		protected IObservable<long> observCountingChecked;
		protected IDisposable disposeObservCountingChecked;

		private List<string> _sortBy;
		protected bool _backup;
		protected bool _copyPhoto;
		protected bool _copySignature;

		
		//private string _selectedSortBy;
		protected IConnectionDB _connection;

		public FromFtpViewModel(
			IContextCBIRepository contextCbiRepository,
			IDBSettings dbSettings,
			IEventAggregator eventAggregator,
			IServiceLocator serviceLocator,
			IUserSettingsManager userSettingsManager,
			IConnectionDB connection,
			IZip zip,
			ILog logImport)
			: base(contextCbiRepository)
		{
			this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._eventAggregator = eventAggregator;
			this._dbSettings = dbSettings;
			this._zip = zip;
			this._connection = connection;
			this._logImport = logImport;
			this._openFolderRequest = new InteractionRequest<OpenFolderDialogNotification>();
			//this._browseCommand = new DelegateCommand(BrowseCommandExecuted, BrowseCommandCanExecuted);


			this._getFromFtpCommand = new DelegateCommand(this.GetFromFtpCommandExecuted, this.GetFromFtpCommandCanExecute);
			this._execButtonText = Localization.Resources.View_UploadToPda_btnStart;
			//this._openCommand = new DelegateCommand(OpenCommandExecuted, OpenCommandCanExecute);
			this._items = new ObservableCollection<FromFtpItemViewModel>();
			this._folderItems = new ObservableCollection<ItemFindViewModel>();
			this._closeCommand = new DelegateCommand(CloseCommandExecute);
			//this._canBrowse = true;
			this._backup = false;
			this._copyPhoto = false;
			this._copySignature = false;
		
			//this.Host = userSettingsManager.HostGet().Trim('\\');
			
			this.Host = _userSettingsManager.HostFtpGet(out _enableSsl);
			this.User = userSettingsManager.UserGet();
			this.Password = userSettingsManager.PasswordGet();

			this._sortItems = new ObservableCollection<ItemFindViewModel>();
			this._sortItems.Add(new ItemFindViewModel() { Value = ComboValues.SortFileGrid.DateAndTime, Text = Localization.Resources.SortFileGrid_DateAndTime });
			this._sortItems.Add(new ItemFindViewModel() { Value = ComboValues.SortFileGrid.FileName, Text = Localization.Resources.SortFileGrid_FileName });
			this._sortItemSelected = this._sortItems.FirstOrDefault();

			this._folderItems = new ObservableCollection<ItemFindViewModel>();

			this._openFtpCommand = new DelegateCommand(this.OpenFtpCommandExecuted, this.OpenFtpCommandCanExecute);				
		}


		public bool IsBusy
		{
			get { return _isBusy; }
			set {
				_isBusy = value;
				this._getFromFtpCommand.RaiseCanExecuteChanged();	
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
			if (String.IsNullOrEmpty(this.FromPdaProcessPath))
				return;
			Utils.OpenFtpFolderInExplorer(Host, this.FromPdaProcessPath);
		}

		private bool OpenFtpCommandCanExecute()
		{
			if (String.IsNullOrEmpty(this.FromPdaProcessPath))
				return false;
			return true;
		}

		public ObservableCollection<ItemFindViewModel> SortItems
		{
			get { return _sortItems; }
		}

		public ItemFindViewModel SortItemSelected
		{
			get { return _sortItemSelected; }
			set
			{
				_sortItemSelected = value;
				RaisePropertyChanged(() => SortItemSelected);
				using (new CursorWait())
				{
					this.Build(false);
				}
			}
		}

		public ObservableCollection<ItemFindViewModel> FolderItems
		{
			get { return this._folderItems; }
		}

		public ItemFindViewModel FolderItemSelected
		{
			get { return _folderItemSelected; }
			set
			{
				if (value != null)
				{
					if (_folderItemSelected != value)
					{
						_folderItemSelected = value;
						RaisePropertyChanged(() => FolderItemSelected);

						this.FromPdaProcessPath = this.FromPdaPath.Trim('\\') + @"\" + FolderItemSelected.Text;
						Utils.RunOnUI(() => RaisePropertyChanged(() => FromPdaProcessPath));
						Thread.Sleep(1000);

						using (new CursorWait())					//TODO
						{
							this.Build(false);
						}
					}
				}
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

		
		public string FromPhotoPath
		{
			get { return _fromPhotoPath; }
			set { _fromPhotoPath = value; }
		}

		public string FromSignaturePath
		{
			get { return _fromSignaturePath; }
			set { _fromSignaturePath = value; }
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

		//OLD 
		//  ftp://ftp.boscom.com  /mINV  /InventoryImages/687/
		//this._hostmINV + @"\" + this._inventoryImages + @"\" + currentCustomerCode;
		//this._hostmINV = @"mINV";
		//this._inventoryImages = = @"InventoryImages";

		public bool CopyPhoto
		{
			get { return _copyPhoto; }
			set
			{
				_copyPhoto = value;
				RaisePropertyChanged(() => CopyPhoto);
			}
		}


		public bool CopySignature
		{
			get { return _copySignature; }
			set
			{
				_copySignature = value;
				RaisePropertyChanged(() => CopySignature);
			}
		}

		public bool Backup
		{
			get { return this._backup; }
			set
			{
				this._backup = value;
				
				Utils.RunOnUI(() => RaisePropertyChanged(() => Backup));
				if (this._backup == true)
				{
					Thread.Sleep(500);
					using (new CursorWait())
					{

						this.BuildFolderBackup(this.FromPdaPath);
						Utils.RunOnUI(() => RaisePropertyChanged(() => FolderItems));
					}
				}
				else
				{
					this._folderItems.Clear();
					this._folderItems.Add(new ItemFindViewModel() { Value = FtpFolderName.Process, Text = FtpFolderName.Process });
					this._folderItemSelected = this._folderItems.FirstOrDefault();
					Utils.RunOnUI(() => RaisePropertyChanged(() => FolderItemSelected));
				}
			
			
				//TODO
				//Utils.RunOnUI(() => RaisePropertyChanged(() => Backup));
				//Thread.Sleep(500);
				//Utils.RunOnUI(() => RaisePropertyChanged(() => FromPdaProcessPath));
				//Thread.Sleep(1000);

				//using (new CursorWait())
				//{
				//	this.Build();
				//}
			}
		}

		public string FromPdaProcessPath
		{
			get 
			{
				return _fromPdaProcessPath;
				//string currentCustomerCode = "";
				// if (base.State.CurrentCustomer != null) currentCustomerCode = base.State.CurrentCustomer.Code;
				//string addBackup  = "";
				//if (this._backup == true) addBackup = @"\" + "backup";
				//return this._hostmINV + @"\" + this._hostFromApp + @"\" + currentCustomerCode + addBackup;
			}
			set
			{
				_fromPdaProcessPath = value;
				RaisePropertyChanged(() => FromPdaProcessPath);
			}
		}

		//OLD
		//  ftp://ftp.boscom.com  /mINV  /InventoryImages/687/
		//this._hostmINV + @"\" + this._inventoryImages + @"\" + currentCustomerCode;
		//this._hostmINV = @"mINV";
		//this._inventoryImages = = @"InventoryImages";

		public string FromPhotoFolder
		{
			get
			{
				return this._inventoryImagesFolder;
				}
			set
			{
				this._inventoryImagesFolder = value;
				RaisePropertyChanged(() => FromPhotoFolder);
			}
		}


		public string FromSignatureFolder
		{
			get
			{
				return this._inventorySignatureFolder;
				}
			set
			{
				this._inventorySignatureFolder = value;
				RaisePropertyChanged(() => FromSignatureFolder);
			}
		}
		

		public string DownloadButtonText
		{
			get { return this._execButtonText; }
		}

		//public string Path
		//{
		//	get { return this._path; }
		//	set
		//	{
		//		this._path = value;
		//		RaisePropertyChanged(() => Path);

		//		this._getFromFtpCommand.RaiseCanExecuteChanged();
		//		this._openCommand.RaiseCanExecuteChanged();
		//	}
		//}


		public DelegateCommand GetFromFtpCommand
		{
			get { return this._getFromFtpCommand; }
		}

		//protected bool OpenCommandCanExecute()
		//{
		//	return Directory.Exists(_path);
		//}

		//protected void OpenCommandExecuted()
		//{
		//	Utils.OpenFolderInExplorer(_path);
		//}

		//public int ProcessValue
		//{
		//	get { return this._processValue; }
		//	set
		//	{
		//		this._processValue = value;
		//		RaisePropertyChanged(() => ProcessValue);
		//	}
		//}

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
				_getFromFtpCommand.RaiseCanExecuteChanged();
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

		//public DelegateCommand OpenCommand
		//{
		//	get { return this._openCommand; }
		//}

		//private bool BrowseCommandCanExecuted()
		//{
		//	return this._canBrowse;
		//}

		//private void BrowseCommandExecuted()
		//{
		//	OpenFolderDialogNotification notification = new OpenFolderDialogNotification();
		//	if (Directory.Exists(this._path) == true)
		//	{
		//		notification.FolderPath = this._path;
		//	}
		//	this._openFolderRequest.Raise(notification, FolderOpen);
		//}

		//private void FolderOpen(OpenFolderDialogNotification notification)
		//{
		//	if (notification.IsOk == true)
		//	{
		//		this._path = notification.FolderPath;
		//	}
		//}

		protected void BuildFolderBackup(string path)
		{
			List<string> sortFolder = new List<string>();
			List<FromFtpItemViewModel> listItem = new List<FromFtpItemViewModel>();

			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);
			FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(path);
			this._folderItems.Clear();
			if (statusCode == FtpStatusCode.PathnameCreated)
			{
				List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(path, this._host, this._user, this._password);
				if (listInfoDirectory.Count > 0)
				{
					foreach (FileDirectoryInfo fi in listInfoDirectory)
					{
						if (fi == null) continue;
						if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
						if (fi.IsDirectory == true)
						{
							string folderName = fi.Name;
							sortFolder.Add(folderName);
						}
					}
				}
			}

			List<string> returnSubFolder = new List<string>();
			foreach (string folder in sortFolder.OrderByDescending(x => x))
			{
				if (folder == FtpFolderName.Process) continue;
				List<string> list = GetSubFolder(path, folder);
				foreach (string li in list)
				{
					returnSubFolder.Add(li);
				}
			 }

			this._folderItems.Add(new ItemFindViewModel() { Value = FtpFolderName.Process, Text = FtpFolderName.Process });
			foreach (string folder in returnSubFolder)
			{
				if (folder == FtpFolderName.Process) continue;
				this._folderItems.Add(new ItemFindViewModel() { Value = folder, Text = folder });
			}
			this._folderItemSelected = this._folderItems.FirstOrDefault();
			RaisePropertyChanged(() => this.FolderItemSelected);

		}

		protected List<string> GetSubFolder(string path, string folder)
		{
			List<string> returnSubFolders = new List<string>();
			List<string> subFolders = new List<string>();

			List<FromFtpItemViewModel> listItem = new List<FromFtpItemViewModel>();
			path = path + @"\" + folder;
			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);
			FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(path);
			this._folderItems.Clear();
			if (statusCode == FtpStatusCode.PathnameCreated)
			{
				List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(path, this._host, this._user, this._password);
				if (listInfoDirectory.Count > 0)
				{
					foreach (FileDirectoryInfo fi in listInfoDirectory)
					{
						if (fi == null) continue;
						if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
						if (fi.IsDirectory == true)
						{
							string folderName = fi.Name;
							subFolders.Add(folderName);
						}
					}
				}
			}


			foreach (string sub in subFolders)
			{
				returnSubFolders.Add(folder + @"\" + sub); 
			}
			return returnSubFolders;
		}


		public void Build(bool isChecked)
		{
			List<FromFtpItemViewModel> listItem = new List<FromFtpItemViewModel>();

			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);
			FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(this.FromPdaProcessPath);
			if (statusCode == FtpStatusCode.PathnameCreated)
			{
				this.LogImport.AddSimple(MessageTypeEnum.TraceProvider, String.Format("On ftp opened folder [{0}]", this.FromPdaProcessPath));

				List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(this.FromPdaProcessPath, this._host, this._user, this._password);
				this.LogImport.AddSimple(MessageTypeEnum.TraceProvider, String.Format("In folder there are [{0}] files", listInfoDirectory.Count.ToString()));
				if (listInfoDirectory.Count > 0)
				{
					foreach (FileDirectoryInfo fi in listInfoDirectory)
					{
						if (fi == null) continue;
						if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
						if (fi.IsDirectory == true) continue;
						string fileName = fi.Name;
						FromFtpItemViewModel item = new FromFtpItemViewModel(this._serviceLocator);
						item.File = fileName;
						item.Size = fi.FileSize;
						try
						{
							item.DateTimeCreated = fi.DateTimeCreated;
							item.Date = fi.DateTimeCreated.ToShortDateString() + "  " + fi.DateTimeCreated.ToShortTimeString();
						}
						catch
						{
							item.Date = fi.Date;
						}
						item.IsChecked = isChecked;
						item.IsCheckedEnabled = true;

						listItem.Add(item);
					}

					string selectedSortBy = _sortItemSelected == null ? String.Empty : _sortItemSelected.Value;
					if (selectedSortBy == ComboValues.SortFileGrid.FileName)
					{
						listItem = listItem.OrderBy(e => e.File).Select(e => e).ToList();
					}
					else if (selectedSortBy == ComboValues.SortFileGrid.DateAndTime)
					{
						listItem = listItem.OrderByDescending(e => e.DateTimeCreated).Select(e => e).ToList();
					}

				}
			}
			else
			{
				this.LogImport.AddSimple(MessageTypeEnum.TraceProvider, String.Format("Ftp can't open with Status [{0}]", statusCode.ToString()));
			}


			this._items.Clear();
			foreach (var it in listItem)
			{
				bool zipfile = it.File.EndsWith(".zip");
				if (zipfile == true)
				{
					this._items.Add(it);  //".zip"
				}
			}

			_getFromFtpCommand.RaiseCanExecuteChanged();
		}

		//protected void UpdateTotalProgress()
		//{

		//	List<UploadToPdaItemViewModel> itemsInvolved = this._items.Where(r => r.IsChecked).Where(r => r.Cancelled == false && r.IsError == false).ToList();


		//	if (itemsInvolved.Count == 0)
		//	{
		//		this.ProcessValue = 0;
		//		return;
		//	}

		//	long doneTotalProgress = 0;
		//	if (itemsInvolved.Count == 0)
		//	{
		//		this.ProcessValue = 0;
		//		return;
		//	}


		//	foreach (UploadToPdaItemViewModel item in itemsInvolved)
		//	{
		//		doneTotalProgress = doneTotalProgress + item.Progress;
		//	}

		//	double progress = (double)doneTotalProgress / (double)(itemsInvolved.Count * 100);
		//	this.ProcessValue = 0;
		//	if (progress >= 0 && progress <= 1)
		//	{
		//		this.ProcessValue = (int)(progress * 100);
		//	}
		//}

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
			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AdapterName))
			{
				this._adapterName = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AdapterName).Value;
			}
			InitProperty(base.CurrentInventor, false, base.State, this._adapterName);

			using (new CursorWait())
			{
				this.Build(false);
			}
			RaisePropertyChanged(() => this.FromPdaProcessPath);
			RaisePropertyChanged(() => this.FolderItemSelected);

			observCountingChecked = Observable.Timer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1)).Select(x => x);
			disposeObservCountingChecked = observCountingChecked.Subscribe(CountingChecked);

		}

		public void InitProperty(Inventor currentInventor, bool isChecked, CBIState state, string adapterName)
		{
			if (base.State == null) base.State = state;
			if (adapterName == ImportAdapterName.ImportPdaNativSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
			{
				this._copyPhoto = true;
				this._copySignature = true;
			}
			else
			{
				this._copyPhoto = false;
				this._copySignature = false;
			}

			//string relativeDb = base._contextCBIRepository.BuildRelativeDbPath(base.CurrentInventor);
			string relativeDb = base._contextCBIRepository.BuildLongCodesPath(currentInventor);
			this.ToPdaFolder = relativeDb.Trim('\\') + @"\" + FtpFolderName.ToPda;		//Inventor\2018\1\1\<InventorCode>\ToPda
			this.FromPdaFolder = relativeDb.Trim('\\') + @"\" + FtpFolderName.FromPda;		//Inventor\2018\1\1\<InventorCode>\FromPda
			this.FromPhotoFolder = relativeDb.Trim('\\') + @"\" + FtpFolderName.Photo;		//Inventor\2018\1\1\<InventorCode>\Photo
			this.FromSignatureFolder = relativeDb.Trim('\\') + @"\" + FtpFolderName.Signature;		//Inventor\2018\1\1\<InventorCode>\Signature
			this.HostRootFolder = this._connection.RootFolderFtp(); //mINV 
			this.ToPdaPath = this._connection.RootFolderFtp(this.ToPdaFolder); //	 \mINV\Inventor\2018\1\1\<InventorCode>\ToPda
			this.FromPdaPath = this._connection.RootFolderFtp(this.FromPdaFolder); //	 \mINV\Inventor\2018\1\1\<InventorCode>\FromPda
			this.RelativeObjectPath = this._connection.RootFolderFtp(relativeDb);  //	 \mINV\Inventor\2018\1\1\<InventorCode>
			this.FromPhotoPath = this._connection.RootFolderFtp(this.FromPhotoFolder); //	 \mINV\Inventor\2018\1\1\<InventorCode>\Photo
			this.FromSignaturePath = this._connection.RootFolderFtp(this.FromSignatureFolder); //	 \mINV\Inventor\2018\1\1\<InventorCode>\Signature

			//this.HostRootFolder = this._connection.RootComplexDataFolderFtp(); //mINV/ComplexData
			//this.ToPdaPath = this._connection.RootComplexDataFolderFtp(this.ToPdaFolder); //	 \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>\ToPda
			//this.FromPdaPath = this._connection.RootComplexDataFolderFtp(this.FromPdaFolder); //	 \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>\ToPda
			//this.RelativeObjectPath = this._connection.RootComplexDataFolderFtp(relativeDb);  //	 \mINV\ComplexData\Inventor\2018\1\1\<InventorCode>

			this.FromPdaProcessPath = this.FromPdaPath.Trim('\\') + @"\" + FtpFolderName.Process;	  //  \mINV\Inventor\2018\1\1\<InventorCode>\FromPda\Process

			this._folderItems.Add(new ItemFindViewModel() { Value = FtpFolderName.Process, Text = FtpFolderName.Process });
			this._folderItemSelected = this._folderItems.FirstOrDefault();
				
			//using (new CursorWait())
			//{
			//	this.Build(isChecked);
			//}
		}


		public List<string> SortBy
		{
			get { return _sortBy; }
		}


		//public string SelectedSortBy
		//{
		//	get { return _selectedSortBy; }
		//	set
		//	{
		//		_selectedSortBy = value;
		//		RaisePropertyChanged(() => SelectedSortBy);
		//		using (new CursorWait())
		//		{
		//			this.Build();
		//		}
		//	}
		//}

		public void CountingChecked(long x)
		{
			//bool conting = true; //слушать или нет
			_getFromFtpCommand.RaiseCanExecuteChanged();

			//if (conting == false)
			//{
			//	return;
			//}

			//if (this._items == null) return;
			//if (this._items.Count() < 1)
			//{
			//	return;
			//}

			//var itemsCheck = this._items.Where(k => k.IsChecked == true ).Select(k => k).ToList();

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


		private bool GetFromFtpCommandCanExecute()
		{
		
			if (this._items.Any(r => r.IsChecked) == false)
			{
				//if ()
				return false;
			}
			//if (this.IsBusy == true) return false;
			return true;
		}

		private void GetFromFtpCommandExecuted()
		{
			if (base.State == null) return;

			this.GetFromFtp(base.State.CurrentInventor);
			//this.IsBusy = false;

			using (new CursorWait())
			{
				this.Build(false);
			}

			if (CopyPhoto == true)
			{
				using (new CursorWait())
				{
					GetPhotoFilesFromFtpCommandExecuted(this.FromPhotoPath, FtpFolderName.Photo.ToLower());
				}
			}


			if (CopySignature == true)
			{
				using (new CursorWait())
				{
					GetPhotoFilesFromFtpCommandExecuted(this.FromSignaturePath, FtpFolderName.Signature.ToLower());
				}
			}

			this._getFromFtpCommand.RaiseCanExecuteChanged();

		}

		public void GetFromFtp(Inventor currentInventor)
		{
			//if (this.IsBusy == true) return;
			//this.IsBusy = true;
			string currentInventorCode = "";
			if (currentInventor != null) currentInventorCode = currentInventor.Code;
			string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора

			var itemsCheck = this._items.Where(k => k.IsChecked == true ).Select(k => k).ToList();

			if (itemsCheck.Count() < 1) return;
			if (currentInventorCode == "") return;

			string targetInDataFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData";
			if (Directory.Exists(targetInDataFolder) == false) Directory.CreateDirectory(targetInDataFolder);
			string targetInDataZipFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\zip";
			if (Directory.Exists(targetInDataZipFolder) == false) Directory.CreateDirectory(targetInDataZipFolder);

			string messageCreateFolder = "";
			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);

			try
			{
				using (new CursorWait())
				{
					FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(this.FromPdaProcessPath);
					if (statusCode == FtpStatusCode.PathnameCreated)
					{
						List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(this.FromPdaProcessPath, this._host, this._user, this._password);
						if (listInfoDirectory.Count < 1) return;
						this.LogImport.AddSimple(MessageTypeEnum.TraceProvider, String.Format("In folder [{0}] selected [{1}] files for copy from ftp", this.FromPdaProcessPath, listInfoDirectory.Count.ToString()));

						foreach (var item in itemsCheck)
						{
							string fileName = item.File;
							string backupFolder = GetBackupFolder(fileName);

							//==================   create backup folder on  ftp
							string fromPdaThreadCodeCopyFolder = client.CreatePathOnFtp(Host, this.FromPdaPath, backupFolder, ref messageCreateFolder);

							//=================== 	copy   FROM Process (on ftp) TO backup folder	(on ftp)
							client.uri = Host;
							string result15 = client.ChangeWorkingDirectory(this.FromPdaPath);
							//string fromFTPPath1 = FtpFolderName.Process + @"\" + fileName;
							string fromFTPPath1 = this.FolderItemSelected.Text + @"\" + fileName;
							string toFTPPath1 = backupFolder + @"\" + fileName;
							if (fromFTPPath1.ToLower() != toFTPPath1.ToLower())
							{
								string result11 = client.CopyFileOnFtp(fromFTPPath1, toFTPPath1);
								this.LogImport.AddSimple(MessageTypeEnum.TraceProvider, String.Format("Backup to [{0}] with resultCode [{1}]", toFTPPath1, result11.Trim()));
								_logger.Info("Save file [" + fromFTPPath1 + "]" + " to ftp [" + toFTPPath1 + "]");
								_logger.Info("with Result [" + result11 + "]");
							}
							

							//=================  create backup folder on count4U
							string targetInDataBackupFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\" + backupFolder;
							if (Directory.Exists(targetInDataBackupFolder) == false) Directory.CreateDirectory(targetInDataBackupFolder);

							//====================== copy FROM Process (ftp)                TO inData\zip (Count4U)
							//====================== unzip  FROM inData\zip (Count4U)  TO   inData (Count4U)
							//====================== copy FROM inData (Count4U) TO  Count4U BackupFolder (Count4U)
							string targetInDataFilePath = targetInDataFolder + @"\" + fileName;
							string targetInDataZipFilePath = targetInDataZipFolder + @"\" + fileName;

							string fromFtpPathFile = this.FromPdaProcessPath;
							client.uri = Host;
							string result17 = client.ChangeWorkingDirectory(fromFtpPathFile);
							try
							{   //Inventor
								if (File.Exists(targetInDataZipFilePath) == false)
								{
									//copy FROM ftp process TO  Count4U inData\zip
									FtpStatusCode result = client.GetFileFromFtp(fileName, targetInDataZipFilePath);
									fromFtpPathFile = this.FromPdaProcessPath + @"\" + fileName;		  //  на ftp	  (process)
									_logger.Info("Copy file from ftpServer[" + fromFtpPathFile + "]");
									_logger.Info("with Result [" + result.ToString() + "]");
									this.LogImport.AddSimple(MessageTypeEnum.TraceProvider, String.Format("Copy file from ftpServer [{0}] with resultCode [{1}]", fromFtpPathFile, result.ToString()));

								}
								//unzip  FROM inData\zip TO  Count4U inData
								this._zip.ReadDb3FromZipFile(targetInDataZipFilePath, targetInDataFolder);

								//copy FROM Count4U inData TO  Count4U BackupFolder
								File.Copy(targetInDataZipFilePath, targetInDataBackupFolder + @"\" + fileName, true);

								//delete from process
								#if DEBUG
								//this.ClearFolderOnFtp(FromPdaProcessPath);		//TEST
								if (FromPdaProcessPath.Contains(FtpFolderName.Process) == true)
								{
									//this.DeleteFileInFolderOnFtp(fileName, FromPdaProcessPath);	   //TEST
									;
								}
								#else              
								//очистить ftp folder  FromPda\Process 
								if (FromPdaProcessPath.Contains(FtpFolderName.Process) == true)
								{
  									this.DeleteFileInFolderOnFtp(fileName, FromPdaProcessPath);
								}
								#endif

							}
							catch (Exception exc)
							{
								string message = String.Format("GetFromFtpCommandExecuted {0} ", fileName);
								this.LogImport.Add(MessageTypeEnum.Error, message);
								_logger.ErrorException(message, exc);
							}
						}
					}

				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("GetFromFtpCommandExecuted", exc);
				this.LogImport.Add(MessageTypeEnum.Error, exc.Message);
			}
			//===============
			//this.IsBusy = false;
		}

		private static string GetBackupFolder(string fileName)
		{
			string backupFolder = @"backupUnsure";
			string[] fileNamePart = fileName.Split('^');
			if (fileNamePart.Length >= 2)
			{
				if (string.IsNullOrWhiteSpace(fileNamePart[1]) == false)
				{
					backupFolder = fileNamePart[1].Trim();
					if (fileNamePart.Length >= 3)
					{
						if (string.IsNullOrWhiteSpace(fileNamePart[2]) == false)
						{
							if (fileNamePart[2].Trim() != ".zip")
							{
								string folderName = fileNamePart[2].Trim();
								if (folderName.Length > 10)
								{
									folderName = folderName.Substring(0, 10);
								}
								backupFolder = fileNamePart[1].Trim() + @"\" + folderName.Trim();
							}
							else
							{
								string folderName = fileNamePart[1].Trim();
								if (folderName.Length > 10)
								{
									folderName = folderName.Substring(0, 10);
								}
								backupFolder = fileNamePart[0].Trim() + @"\" + folderName.Trim();
							}
						}
					}
				}
			}
			return backupFolder;
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
					string result = client.DeleteFile(fileName);
				}
			}
		}

		public void DeleteFileInFolderOnFtp(string fileName, string fromFolder)
		{
			if (string.IsNullOrWhiteSpace(fileName) == true) return;
			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);
			FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(fromFolder);
			if (statusCode == FtpStatusCode.PathnameCreated)
			{
				List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(fromFolder, this._host, this._user, this._password);
				if (listInfoDirectory.Count < 1) return;
				string result = client.DeleteFile(fileName);
			}
		}

		//======================  Было копирование фото , параметризовала для копироания подписи, 	название пока оставила
		private void GetPhotoFilesFromFtpCommandExecuted(string fromFtpPath, string toInDataFolder)	
		{

			string currentInventorCode = "";
			if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;
			if (currentInventorCode == "") return;

			string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора

			string inDataFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData";
			if (Directory.Exists(inDataFolder) == false) Directory.CreateDirectory(inDataFolder); 

			string targetPhotoFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\" + toInDataFolder;//photo;
			if (Directory.Exists(targetPhotoFolder) == false) Directory.CreateDirectory(targetPhotoFolder);

			//==========	 old
			//string currentCustomerCode = "";
			//if (base.State.CurrentCustomer != null) currentCustomerCode = base.State.CurrentCustomer.Code;
			//string importFolderPath = _dbSettings.ImportFolderPath();
			//string path = Path.Combine(importFolderPath, "Customer", currentCustomerCode);
			//

			string targetFolder = Path.GetFullPath(targetPhotoFolder);

			try
			{
				if (Directory.Exists(targetFolder) == false)
					Directory.CreateDirectory(targetFolder);
			}
			catch { }
			if (Directory.Exists(targetFolder) == false) return;

			DirectoryInfo di = new DirectoryInfo(targetFolder);

			Dictionary<string,FileInfo>  dictionaryTargetFileInfo = di.GetFiles().ToDictionary(x => x.Name);

			//string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора	TODO 

			FtpClient client = new FtpClient(this._host, this._user, this._password, this._enableSsl);		   //TODO

			try
			{
				using (new CursorWait())
				{
					//FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(this.FromPhotoPath);
					FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(fromFtpPath);
					
					if (statusCode == FtpStatusCode.PathnameCreated)
					{
						//List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(this.FromPhotoPath, this._host, this._user, this._password);
						List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(fromFtpPath, this._host, this._user, this._password);
						if (listInfoDirectory.Count < 1) return;

						foreach (FileDirectoryInfo fi in listInfoDirectory)
						{
							if (fi == null) continue;
							if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
							if (fi.IsDirectory == true) continue;

							string fileName = fi.Name;
							if (dictionaryTargetFileInfo.ContainsKey(fileName) == true)
							{
								FileInfo fileinfo = dictionaryTargetFileInfo[fileName];
								long length = fileinfo.Length;
								long lengthFtp = fi.Size;
								if (length == lengthFtp) continue; 
							}
							string targetPath = "";

							targetPath = targetFolder + @"\" + fileName; //Inventor
							//if (System.IO.Path.GetExtension(fileName) == ".zip")
							//{
							//	targetPath = targetZipFolder + @"\" + fileName;
							//}

							//string fromFtpPathPhoto = this.FromPhotoPath;
							string fromFtpPathPhoto = fromFtpPath;
							try
							{   //Customer
								if (File.Exists(targetPath) == true) File.Delete(targetPath);				 //Count4U
									//continue;

								//fromFtpPathPhoto = this.FromPhotoPath + @"\" + fileName;		  //  на ftp	  (from)
								fromFtpPathPhoto = fromFtpPath + @"\" + fileName;	
								FtpStatusCode result = client.GetFileFromFtp(fileName, targetPath);
								_logger.Info("Copy" + fileName + "from ftpServer[" + fromFtpPathPhoto + "]  with Result [" + result.ToString() + "]");
								

								//if (haveBackupFolder == true)
								//{
								//	//string result1 = client.SaveFileToFtp(targetPath, @"backup\" + fileName);
								//	string result1 = client.Rename(fileName, @"backup\" + fileName);
								//}

								//string result2 = client.DeleteFile(fileName);
							}
							catch (Exception exc)
							{
								string message = String.Format("GetPhotoFilesFromFtpCommandExecuted {0} => {1}",
									fromFtpPathPhoto, targetPath);   //TODO
								_logger.ErrorException(message, exc);
							}
						}
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("GetPhotoFilesFromFtpCommandExecuted", exc);
			}

		}

		//CopyFromHostGet
		//public void CopyingHostFiles(long x)
		//{
		//	IUserSettingsManager userSettingsManager = this.Container.Resolve<IUserSettingsManager>();

		//	bool copyFromHost = userSettingsManager.CopyFromHostGet(); //копировать или нет
		//	if (copyFromHost == false) return;
		//	string customerCodeProcess = "";
		//	string inventorCodeProcess = "";
		//	//bool copyByCodeInventorOnly = userSettingsManager.CopyByCodeInventorGet();
		//	//if (copyByCodeInventorOnly == true)
		//	//{
		//	IServiceLocator serviceLocator = this.Container.Resolve<IServiceLocator>();
		//	IAuditConfigRepository audtiConfigRepository = serviceLocator.GetInstance<IAuditConfigRepository>();

		//	if (audtiConfigRepository != null)
		//	{
		//		AuditConfig ac = audtiConfigRepository.GetProcessConfig(CBIContext.History);
		//		customerCodeProcess = ac.CustomerCode;
		//		inventorCodeProcess = ac.InventorCode;
		//	}
		//	if (customerCodeProcess == "") return;
		//	if (inventorCodeProcess == "") return;
		//	string targetFolder = this._importPDAFolder.Trim('\\') + @"\Inventor\" + inventorCodeProcess + @"\inData";
		//	if (Directory.Exists(targetFolder) == false) return;

		//	//}

		//	//string _host = userSettingsManager.HostGet().Trim('\\');
		//	//string _hostUser = userSettingsManager.UserGet();
		//	//string _hostPassword = userSettingsManager.PasswordGet(); 
		//	//string _hostmINV = @"mINV";
		//	//string _hostFromApp = @"FromApp"; 

		//	FtpClient client = new FtpClient(this._host, this._user, this._password);

		//	try
		//	{
		//		string sourceFolder = this._hostmINV + @"\" + this._hostFromApp + @"\" + customerCodeProcess;
		//		FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(sourceFolder);
		//		if (statusCode == FtpStatusCode.PathnameCreated)
		//		{
		//			List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(sourceFolder, this._host, this._user, this._password);
		//			if (listInfoDirectory.Count < 1) return;

		//			bool haveBackupFolder = false;
		//			FileDirectoryInfo backup = listInfoDirectory.FirstOrDefault(k => k.IsDirectory == true && k.Name == "backup");
		//			if (backup != null) haveBackupFolder = true;

		//			foreach (FileDirectoryInfo fi in listInfoDirectory)
		//			{
		//				if (fi == null) continue;
		//				if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
		//				if (fi.IsDirectory == true) continue;
		//				string fileName = fi.Name;

		//				string inventorCode = inventorCodeProcess;
		//				string targetPath = "none";

		//				targetPath = targetFolder + @"\" + fileName; //Inventor
		//				string fromFtpPathProfile = sourceFolder;
		//				try
		//				{   //Inventor
		//					if (File.Exists(targetPath) == true) continue;//Вернуть //File.Delete(targetPath);				 //Count4U

		//					fromFtpPathProfile = sourceFolder + @"\" + fileName;		  //  на ftp	  (from)
		//					string result = client.GetFileFromFtp(fileName, targetPath);
		//					_logger.Info("Copy profile.xml from ftpServer[" + fromFtpPathProfile + "]");
		//					_logger.Info("with Result [" + result + "]");

		//					if (haveBackupFolder == true)
		//					{
		//						string result1 = client.SaveFileToFtp(targetPath, @"backup\" + fileName);
		//					}

		//					string result2 = client.DeleteFile(fileName);
		//				}
		//				catch (Exception exc)
		//				{
		//					string message = String.Format("CopyingHostFiles (File.Move to unsurePath) {0} => {1}", fromFtpPathProfile, targetPath);
		//					_logger.ErrorException(message, exc);
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("CopyingHostFiles", exc);
		//		throw;
		//	}
		//}


	}
}