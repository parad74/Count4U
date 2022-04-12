using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Enums;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Report.ViewModels.ExportPda;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Count4U.Model;
using NLog;
using Count4U.GenerationReport;
using Count4U.Common.Extensions;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Modules.ContextCBI.Events;
using System.Xml.Linq;
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.ExportImport.Items;
using System.IO;
using System.Reactive.Linq;

namespace Count4U.Modules.Audit.ViewModels.Import
{
    public class ComplexOperationViewModel : ImportWithModulesBaseViewModel//, IConfirmNavigationRequest
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        //private readonly DelegateCommand _navigateToGridCommand;

        private readonly IUserSettingsManager _userSettingsManager;

        private readonly IImportAdapterRepository _importAdapterRepository;
      // private ExportPdaExtraSettingsViewModel _extraSettingsViewModel;
        private readonly List<string> _newDocumentCodeList;
		private readonly List<string> _newSessionCodeList;
        private readonly IIturRepository _iturRepository;
        private readonly DelegateCommand _setAsDefaultCommand;
		private readonly DelegateCommand _setAsDefaultLikeInCustomerCommand;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IInventorRepository _inventorRepository;
		private readonly IInventProductRepository _inventProductRepository;
		private readonly IProductRepository _productRepository;
		private readonly IAuditConfigRepository _auditConfigRepository;
        private readonly UICommandRepository _uiCommandRepository;
        private readonly IServiceLocator _serviceLocator;			
		private readonly ModalWindowLauncher _modalWindowLauncher;
		
      //  private bool _isNavigateBack;
//		private bool _isCopyFromSource;
        private bool _isContinueGrabFiles;
        private bool _isContinueGrabFilesEnabled;
		private string _adapterName;
		private static string TempSaveAdapterName = "";
		private readonly DelegateCommand _getFromFtpCommand;
		private readonly DelegateCommand _editCustomerOptionsCommand;
		private readonly DelegateCommand _openPackCommand;
		private readonly DelegateCommand _openUnpackCommand;
		private readonly DelegateCommand 	_createNewInventorCommand;
		private readonly DelegateCommand _createAndNavigateToProcessInventorCommand;
		private readonly DelegateCommand _changeCurrentInventorCommand;
		private readonly DelegateCommand _navigateToBranchContextCommand;
		private readonly DelegateCommand _navigateToProcessInventorCommand;
		private readonly UICommandRepository<FileItemViewModel> _commandRepositoryObject;
		

		bool _isCurrentInventorEnabled;
		bool _isChangeCurrentInventorEnabled;
		bool _fromCurrentInventor;
		bool _fromChangeCurrentInventor;

		protected readonly ObservableCollection<FileItemViewModel> _items;
		private FileItemViewModel _selectedItem;
		protected bool _isChecked;

		// Inventros Grid 
		protected IObservable<long> observCountingChecked;
		protected IDisposable disposeObservCountingChecked;

		private readonly DelegateCommand<FileItemViewModel> _auditNavigateCommand;
		private readonly DelegateCommand<FileItemViewModel> _editSelectedCommand;
		private readonly DelegateCommand<FileItemViewModel> _deleteSelectedCommand;
		private readonly DelegateCommand<FileItemViewModel> _inProcessSelectedCommand;
		private readonly DelegateCommand<FileItemViewModel> _setInProcessSelectedCommand;


		protected string _sourcePath;
		protected string _misPath;
		protected string _misUnsurePath;
		private IObservable<long> observCountingFiles;
		private IDisposable disposeObservCountingFiles;

		private long _itemsTotal;
		private double _sumQuantityEdit;
		private long _countProducts;
		protected string _sumQuantityEditString;
		protected string _countProductsString;
		
		private int _lastSessionCountItem;
		private int _lastSessionCountDocument;
		private double _lastSessionSumQuantityEdit;
		private int _quantityFilesInSourceFolder;
		private int _quantityFilesInMISFolder;
		private int _quantityFilesInUnsureFolder;
		private string _adapterFromPDA;


		public ComplexOperationViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUnityContainer container,
            IImportAdapterRepository importAdapterRepository,
            INavigationRepository navigationRepository,
            UICommandRepository commandRepository,
            IIturRepository iturRepository,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IInventorRepository inventorRepository,
			 IInventProductRepository inventProductRepository,
			 IProductRepository productRepository,
			IAuditConfigRepository auditConfigRepository,
            UICommandRepository uiCommandRepository,
			UICommandRepository<FileItemViewModel> commandRepositoryObject,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
			ModalWindowLauncher modalWindowLauncher	,
			DBSettings dbSettings
            )
			: base(contextCBIRepository, eventAggregator, regionManager, container, navigationRepository, commandRepository, dbSettings)
        {
			this._items = new ObservableCollection<FileItemViewModel>();
			this._modalWindowLauncher = modalWindowLauncher;
			this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._uiCommandRepository = uiCommandRepository;
			this._commandRepositoryObject = commandRepositoryObject;
			this._inventorRepository = inventorRepository;
			this._productRepository = productRepository;
			this._inventProductRepository = inventProductRepository;
			this._branchRepository = branchRepository;
			this._customerRepository = customerRepository;
			this._iturRepository = iturRepository;
            this._importAdapterRepository = importAdapterRepository;
			this._auditConfigRepository = auditConfigRepository;
			if (contextCBIRepository == null) throw new ArgumentNullException("contextCBIRepository");
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            if (regionManager == null) throw new ArgumentNullException("regionManager");
            if (importAdapterRepository == null) throw new ArgumentNullException("importAdapterRepository");

            base._progressText = Localization.Resources.ViewModel_ImportWithModulesBase_ImportProgress;

            base.ImportCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted, ImportCommandCanExecute);
			this._getFromFtpCommand = _commandRepository.Build(enUICommand.GetFromFtp, GetFromFtpCommandExecuted, GetFromFtpCommandCanExecute);

			this._editCustomerOptionsCommand = _commandRepository.Build(enUICommand.EditCustomerOptions, EditCustomerOptionsCommandExecuted, EditCustomerOptionsCommandCanExecute);
			this._openPackCommand = _commandRepository.Build(enUICommand.Pack, PackOpenCommandExecuted, PackOpenCommandCanExecute);
			this._openUnpackCommand = _commandRepository.Build(enUICommand.Unpack, UnpackOpenCommandExecuted, UnpackOpenCommandCanExecute);
			this._createNewInventorCommand = _commandRepository.Build(enUICommand.CreateNewInventor, CreateNewInventorCommandExecuted, CreateNewInventorCommandCanExecute);
			this._createAndNavigateToProcessInventorCommand = _commandRepository.Build(enUICommand.CreateAndNavigateToProcessInventor, CreateAndNavigateToProcessInventorCommandExecuted, CreateAndNavigateToProcessInventorCommandCanExecuted);
		//	this._changeCurrentInventorCommand = _commandRepository.Build(enUICommand.ChangeCurrentInventor, ChangeCurrentInventorCommandExecuted, ChangeCurrentInventorCommandCanExecute);
			this._navigateToBranchContextCommand = _commandRepository.Build(enUICommand.ChangeCurrentInventor, NavigateToBranchContextCommandExecuted, NavigateToBranchContextCommandCanExecute);
			this._navigateToProcessInventorCommand = _commandRepository.Build(enUICommand.ToProcessInventor, NavigateToInventorContextCommandExecuted, NavigateToInventorContextCommandCanExecute);
			 
			this._isCurrentInventorEnabled = true;
			this._isChangeCurrentInventorEnabled = true;
			this._fromCurrentInventor = false;
			this._fromChangeCurrentInventor = true;
			//this.AdapterComboboxVisible = true;

            this._newDocumentCodeList = new List<string>();
			this._newSessionCodeList = new List<string>();

            this._setAsDefaultCommand = new DelegateCommand(SetAsDefaultCommandExecuted, SetAsDefaultCommandCanExecute);
			this._setAsDefaultLikeInCustomerCommand = new DelegateCommand(SetAsDefaultLikeInCustomerCommandExecuted);
			

			base.NavigateToGridCommand = _uiCommandRepository.Build(enUICommand.FromImportToGrid, this.NavigateToGridCommandExecuted, base.NavigateToGridCommandCanExecute);
			//base.ImportCommand.RaiseCanExecuteChanged();


			this._newSessionCodeList = new List<string>();
			this._auditNavigateCommand = new DelegateCommand<FileItemViewModel>(this.AuditNavigateCommandExecuted);
			this._deleteSelectedCommand = _commandRepositoryObject.Build(enUICommand.Delete, DeleteSelectedCommandExecuted);
			this._setInProcessSelectedCommand = _commandRepositoryObject.Build(enUICommand.Accept, SetInProcessSelectedCommandExecuted);	
			this._inProcessSelectedCommand = _commandRepositoryObject.Build(enUICommand.Accept, InProcessSelectedCommandExecuted);			
					
        }

		//private void UpdateStatus(string status)
		//{
		//	Utils.RunOnUI(() => BusyContent = status);
		//}

		//private string _busyContent;
		//public string BusyContent
		//{
		//	get { return _busyContent; }
		//	set
		//	{
		//		_busyContent = value;
		//		RaisePropertyChanged(() => BusyContent);
		//	}
		//}

		//private bool _isBusy;
		//public bool IsBusy
		//{
		//	get { return _isBusy; }
		//	set
		//	{
		//		_isBusy = value;
		//		RaisePropertyChanged(() => IsBusy);
		//	}
		//}


	
		// ==================== Branch -> Incentors grid 

		public DelegateCommand<FileItemViewModel> DeleteSelectedCommand
		{
			get { return this._deleteSelectedCommand; }
		}

		public DelegateCommand<FileItemViewModel> InProcessSelectedCommand
		{
			get { return this._inProcessSelectedCommand; }
		}


		public DelegateCommand<FileItemViewModel> SetInProcessSelectedCommand
		{
			get { return this._setInProcessSelectedCommand; }
		}
		

		private void DeleteSelectedCommandExecuted(FileItemViewModel inventorItemViewModel)
		{
			if (string.IsNullOrWhiteSpace(inventorItemViewModel.ObjectCode) == false)
			{
				this._inventorRepository.Delete(inventorItemViewModel.ObjectCode);
				this._auditConfigRepository.DeleteByInventorCode(inventorItemViewModel.ObjectCode, CBIContext.History);
				//SelectedItem = SelectedItem;
				//	_deleteSelectedCommand.Execute(SelectedItem);
				this.Build();
			}

		}


		private void SetInProcessSelectedCommandExecuted(FileItemViewModel inventorItemViewModel)
		{
			if (string.IsNullOrWhiteSpace(inventorItemViewModel.ObjectCode) == false)
			{
				using (new CursorWait())
				{
					AuditConfig auditConfig = this._auditConfigRepository.GetAuditConfigByInventorCode(inventorItemViewModel.ObjectCode, CBIContext.History);
					base.ContextCBIRepository.SetProcessCBIConfig(CBIContext.History, auditConfig);
					//Utils.InventorChangedGlobalAction(base.Container, CBIContext.History, base.GetDbPath);

					this.Build();
				}
			}

		}

		private void SetCurrentAsProcessExecuted()	 //как пример
		{
			using (new CursorWait())
			{
				
				AuditConfig config = new AuditConfig(base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History, true));
				base.ContextCBIRepository.SetProcessCBIConfig(CBIContext.History, config);

				// TODO this._setCurrentAsProcessCommand.RaiseCanExecuteChanged();

				Utils.InventorChangedGlobalAction(base.Container, CBIContext.History, base.GetDbPath);

				//?? this._eventAggregator.GetEvent<ProcessInventorChangedEvent>().Publish(null);
			}
		}

		// первая колонка - ничего не делаем пока
		private void InProcessSelectedCommandExecuted(FileItemViewModel inventorItemViewModel)
		{
			if (string.IsNullOrWhiteSpace(inventorItemViewModel.ObjectCode) == false)
			{
			}

		}

	

		//public bool DeleteSelectedCommandCanExecute(FileItemViewModel item)
		//{
		//	if (item == null) return false;
		//	if (item.InProcess == true) return false;
		//	return true;
		//}

		//public bool InProcessSelectedCommandCanExecute(FileItemViewModel item)
		//{
		//	if (item == null) return false;
		//	if (item.InProcess == true) return true;
		//	return false;
		//}

		// переход к интвентору
		public void AuditNavigateCommandExecuted(FileItemViewModel item)
		{
			AuditConfig config = base.ContextCBIRepository.GetCBIConfigByInventorCode(CBIContext.History, item.ObjectCode);
			if (config != null)
			{
				base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, config);
				base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, config);
			}

			UriQuery query = new UriQuery();
			Utils.AddAuditConfigToQuery(query, GetHistoryAuditConfig());
			UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
		}

		public DelegateCommand<FileItemViewModel> AuditNavigateCommand
		{
			get { return this._auditNavigateCommand; }
		}

		public ObservableCollection<FileItemViewModel> Items
		{
			get { return this._items; }
		}

		//public override bool CanImport()
		//{
		//	//if (this._selectedItem == null) return false;
		//	var itemsCheck = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();
		//	var count = itemsCheck.Count();
		//	if (count == 2) return true;
		//	return false;
		//}

		public FileItemViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				RaisePropertyChanged(() => SelectedItem);
			}
		}

		protected void Build()
		{
			IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
			IDBSettings dbSettings = this._serviceLocator.GetInstance<IDBSettings>();
			string rootFolder = dbSettings.BuildCount4UDBFolderPath();


			this._items.Clear();
			Branch currentBranch = base.CurrentBranch;
			//Inventor currentInventor = base.CurrentInventor;

			//List<string> inventorCodeList = inventorRepository.GetInventorCodeListByBranchCode(currentBranch.Code);
			Inventors inventors = inventorRepository.GetInventorsByBranchCode(currentBranch.Code);
			List<string> pathList = new List<string>();

			//string inventorDBPath = PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
			//using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(inventorDBPath)))
			//string relativeInventorPath = base.ContextCBIRepository.BuildRelativeDbPath(cbiState.CurrentInventor);
			//relativeInventorPath = Path.Combine(_dbSettings.FolderApp_Data, relativeInventorPath);
			//string[] sourceInventorCodes = base.State.CurrentInventor.Manager.Split('|');

			//if (this.ShowSource == true || this.ShowDestination == true) //add
			//{
			//	try
			//	{
			//		if (this.ShowSource == true)
			//		{
			//			foreach (Inventor inventor in inventors)
			//			{
			//				if (inventor.Code == currentInventor.Code) continue;
			//				if (sourceInventorCodes.Contains(inventor.Code) == true)
			//				{
			//					FileItemViewModel item = NewFileItemViewModel(rootFolder, inventor);
			//					item.IsSource = true;
			//					item.IsDestination = false;
			//					item.IsCheckedEnabled = true;
			//					this._items.Add(item);
			//				}
			//			}
			//		}

			//		if (this.ShowDestination == true)
			//		{
			//			foreach (Inventor inventor in inventors)
			//			{
			//				if (inventor.Code == currentInventor.Code) continue;
			//				if (sourceInventorCodes.Contains(inventor.Manager) == true)
			//				{
			//					if (inventor.Manager.ToUpper() != inventor.Code.ToUpper())
			//					{
			//						FileItemViewModel item = NewFileItemViewModel(rootFolder, inventor);
			//						item.IsSource = false;
			//						item.IsDestination = true;
			//						item.IsCheckedEnabled = true;
			//						this._items.Add(item);
			//					}
			//				}
			//			}
			//		}
			//	}
			//	catch { }
			//}

			//else
			//{

			AuditConfig  inProcessAuditConfig = base.ContextCBIRepository.GetProcessCBIConfig(CBIContext.History);
			string inventorCodeInProcess =  "";
			 if (inProcessAuditConfig != null)
			 {
				inventorCodeInProcess =  inProcessAuditConfig.InventorCode;
			} 
		
			if (string.IsNullOrWhiteSpace(inventorCodeInProcess) == true)
			{
				if (inventors.Count() > 1)
				 {
					if (inventors[0] != null){
						inventorCodeInProcess = inventors[0].Code;
					}
				 }
			}


				foreach (Inventor inventor in inventors)
				{
					//if (currentInventor != null)
					//{
					//	if (inventor.Code == currentInventor.Code) continue;
					//}
					try
					{
						FileItemViewModel item = NewFileItemViewModel(rootFolder, inventor);
						//item.IsCheckedEnabled = true;
						
						if (inventor.Code == inventorCodeInProcess)
						{
							item.InProcess = true;
						}
						else
						{
							item.InProcess = false;
						}
						this._items.Add(item);
					}
					catch { }
				}
			//}
			//======================= TEST
			//	CreateTempInventor(inventorRepository);
			////=======================

			//if (base.RaiseCanImport != null)
			//	base.RaiseCanImport();
		}

		

		private FileItemViewModel NewFileItemViewModel(string rootFolder, Inventor inventor)
		{
			string relativeInventorPath = ContextCBIRepository.BuildRelativeDbPath(inventor);
			string fileDbPath = System.IO.Path.Combine(rootFolder, relativeInventorPath) + @"\Count4UDB.sdf";
			FileInfo fi = new FileInfo(fileDbPath);
			//pathList.Add(relativeInventorPath);
			FileItemViewModel item = new FileItemViewModel();
			item.InventorObject = inventor;
			item.Audit = this._auditConfigRepository.GetAuditConfigByInventorCode(inventor.Code.Trim(), CBIContext.History);
			item.File = relativeInventorPath + @"\Count4UDB.sdf";
			item.Path = relativeInventorPath;
			item.Description = inventor.Description;
			item.Manager = inventor.Manager;
			item.Date = fi.LastWriteTime.ToShortDateString() + "  " + fi.LastWriteTime.ToShortTimeString();
			item.DateTimeCreated = fi.LastWriteTime;
			item.Size = ((int)(fi.Length / 1024) + 1).ToString() + " Kb";
			item.Code = inventor.InventorDate.ToShortDateString() + "  " + inventor.InventorDate.ToShortTimeString();
			item.ObjectCode = inventor.Code.Trim();
			return item;
		}


		//private FileItemViewModel CreateNewInventor()
		//{
		//	if (base.CurrentCustomer == null) return null;
		//	IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
		//	IAuditConfigRepository auditConfigRepository = this._serviceLocator.GetInstance<IAuditConfigRepository>();
		//	AuditConfigs auditConfigs = new AuditConfigs();

		//	AuditConfig inProcessAuditConfig = base.ContextCBIRepository.GetProcessCBIConfig(CBIContext.History);
		//	string customerCodeInProcess = "";
		//	if (inProcessAuditConfig != null)
		//	{
		//		customerCodeInProcess = inProcessAuditConfig.CustomerCode;
		//	} 

		//	string newInventorCode = Guid.NewGuid().ToString();

		//	// И создаем снова	 AuditConfig
		//	AuditConfig cloneInProcessAuditConfig = auditConfigRepository.Clone(inProcessAuditConfig);			  // создаем копию с inProcessAuditConfig
		//	if (cloneInProcessAuditConfig == null) return null;

		//	cloneInProcessAuditConfig.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();
		//	cloneInProcessAuditConfig.InventorCode = newInventorCode;
		//	cloneInProcessAuditConfig.InventorName = oldInvetor.Name;
		//	cloneInProcessAuditConfig.InventorDate = oldInvetor.InventorDate;
		//	cloneInProcessAuditConfig.DBPath = AddLevel(oldInvetor.DBPath); //oldInvetor.DBPath + "_A";
		//	cloneInProcessAuditConfig.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();
		//	cloneInProcessAuditConfig.StatusInventorCode = StatusInventorEnum.Temp.ToString();

		//	//inventor.DBPath = auditConfig.DBPath;
		//	auditConfigs.Add(cloneInProcessAuditConfig);
		//	auditConfigRepository.Insert(auditConfigs);
		//	string parentInventorCode = oldInvetor.Manager;
		//	if (string.IsNullOrWhiteSpace(parentInventorCode) == true) parentInventorCode = oldInvetor.Code;

		//	Inventor newInvetor = oldInvetor.Clone(newInventorCode, parentInventorCode);
		//	if (newInvetor == null) return null;
		//	newInvetor.Manager = parentInventorCode;

		//	////////newInvetor.Description = AddLevel(oldInvetor.Description); //oldInvetor.Description + "_A";
		//	//base.ContextCBIRepository.CreateContextInventor(newInvetor, cloneMainAC, true, oldInvetor); все перенесла сюда

		//	string inheritFromDBPath = String.Empty;
		//	if (oldInvetor != null)
		//	{
		//		inheritFromDBPath = base.ContextCBIRepository.BuildRelativeDbPath(oldInvetor);
		//		//  string fullPath = BuildFullDbPath(domainObject);
		//	}

		//	inventorRepository.Insert(newInvetor, inheritFromDBPath);	//!!! Это копирование DB как раз

		//	base.ContextCBIRepository.GetImportFolderPath(newInvetor);		 //создание папки импорта личной


		//	IDBSettings dbSettings = this._serviceLocator.GetInstance<IDBSettings>();
		//	string rootFolder = dbSettings.BuildCount4UDBFolderPath();
		//	FileItemViewModel cloneItem = NewFileItemViewModel(rootFolder, newInvetor);
		//	cloneItem.IsCheckedEnabled = false;
		//	return cloneItem;
		//}

		public string AddLevel(string fromCode)
		{
			string toCode = fromCode + "^A";
			if (fromCode.Contains("^A") == false) return toCode;
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R^S^T^U^V^W^X^Y") == true) return fromCode + "^Z";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R^S^T^U^V^W^X") == true) return fromCode + "^Y";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R^S^T^U^V^W") == true) return fromCode + "^X";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R^S^T^U^V") == true) return fromCode + "^W";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R^S^T^U") == true) return fromCode + "^V";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R^S^T") == true) return fromCode + "^U";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R^S") == true) return fromCode + "^T";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q^R") == true) return fromCode + "^S";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P^Q") == true) return fromCode + "^R";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O^P") == true) return fromCode + "^Q";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N^O") == true) return fromCode + "^P";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M^N") == true) return fromCode + "^O";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L^M") == true) return fromCode + "^N";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K^L") == true) return fromCode + "^M";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J^K") == true) return fromCode + "^L";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I^J") == true) return fromCode + "^K";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H^I") == true) return fromCode + "^J";
			else if (fromCode.Contains("^A^B^C^D^E^F^G^H") == true) return fromCode + "^I";
			else if (fromCode.Contains("^A^B^C^D^E^F^G") == true) return fromCode + "^H";
			else if (fromCode.Contains("^A^B^C^D^E^F") == true) return fromCode + "^G";
			else if (fromCode.Contains("^A^B^C^D^E") == true) return fromCode + "^F";
			else if (fromCode.Contains("^A^B^C^D") == true) return fromCode + "^E";
			else if (fromCode.Contains("^A^B^C") == true) return fromCode + "^D";
			else if (fromCode.Contains("^A^B") == true) return fromCode + "^C";
			else if (fromCode.Contains("^A") == true) return fromCode + "^B";
			else return toCode;
		}

		public bool IsChecked
		{
			get { return this._isChecked; }
			set
			{
				this._isChecked = value;
				RaisePropertyChanged(() => IsChecked);
				foreach (FileItemViewModel item in this._items)
				{
					item.IsChecked = this._isChecked;
				}
			}
		}

		// ====================	END Branch -> Inventors Grid
		public bool IsCurrentInventorEnabled
		{
			get { return this._isCurrentInventorEnabled; }
			set
			{
				this.IsCurrentInventorEnabled = value;
				RaisePropertyChanged(() => IsCurrentInventorEnabled);
			}
		}

		public bool IsChangeCurrentInventorEnabled
		{
			get { return this._isChangeCurrentInventorEnabled; }
			set
			{
				this._isChangeCurrentInventorEnabled = value;
				RaisePropertyChanged(() => IsChangeCurrentInventorEnabled);
			}
		}

		private bool _adapterComboboxVisible = true; 
		public bool AdapterComboboxVisible
		{
			get { return this._adapterComboboxVisible; }
			set
			{
				this._adapterComboboxVisible = value;
				RaisePropertyChanged(() => AdapterComboboxVisible);
			}
		}


		//AdapterComboboxVisible показываем 
		public bool FromCurrentInventor
		{
			get { return this._fromCurrentInventor; }
			set
			{
				this._fromCurrentInventor = value;
				this.RaisePropertyChanged(() => this.FromCurrentInventor);
				
				//this.RaisePropertyChanged(() => this.AdapterComboboxVisible);
				this.AdapterComboboxVisible = value;

				if (value == true)
				{
					this._fromChangeCurrentInventor = false;
					NavigateToInventorContext(FromContext.FromBranchWithoutAction);
				}


				//this._setAsDefaultCommand.RaiseCanExecuteChanged();  ?? вернуть

				//base.ImportCommand.RaiseCanExecuteChanged();
			}
		}


		//AdapterComboboxVisible не показываем 
		public bool FromChangeCurrentInventor
		{
			get { return this._fromChangeCurrentInventor; }
			set
			{
				this._fromChangeCurrentInventor = value;
				//this.RaisePropertyChanged(() => this.AdapterComboboxVisible);
				this.AdapterComboboxVisible = !value;

				if (this._fromChangeCurrentInventor == true)
				{
					this._fromCurrentInventor = false;
					this.NavigateToBranchContextCommandExecuted();
					//this.ComplexOperationCommandExecuted();
				}

				//this.RaisePropertyChanged(() => this.FromChangeCurrentInventor);

				//base.ImportCommand.RaiseCanExecuteChanged();

			}
		}

	

        public DelegateCommand SetAsDefaultCommand
        {
            get { return _setAsDefaultCommand; }
        }

		 
		public DelegateCommand SetAsDefaultLikeInCustomerCommand
        {
            get { return _setAsDefaultLikeInCustomerCommand; }
        }

		//public DelegateCommand NavigateToGridCommand
		//{
		//	get { return _navigateToGridCommand; }
		//}

		//public bool IsNavigateBack
		//{
		//	get { return _isNavigateBack; }
		//	set
		//	{
		//		_isNavigateBack = value;
		//		RaisePropertyChanged(() => IsNavigateBack);

		//		_userSettingsManager.NavigateBackImportPdaFormSet(_isNavigateBack);
		//	}
		//}

		public DelegateCommand GetFromFtpCommand
		{
			get { return this._getFromFtpCommand; }
		}


		public DelegateCommand EditCustomerOptionsCommand
		{
			get { return this._editCustomerOptionsCommand; }
		}


		public DelegateCommand OpenPackCommand
		{
			get { return this._openPackCommand; }
		}

		public DelegateCommand OpenUnpackCommand
		{
			get { return this._openUnpackCommand; }
		}

		public DelegateCommand CreateNewInventorCommand
		{
			get { return this._createNewInventorCommand; }
		}

		
		public DelegateCommand CreateAndNavigateToProcessInventorCommand
		{
			get { return this._createAndNavigateToProcessInventorCommand; }
		}


		public DelegateCommand NavigateToProcessInventorCommand
		{
			get { return this._navigateToProcessInventorCommand; }
		}

		public DelegateCommand ChangeCurrentInventorCommand
		{
			get { return this._changeCurrentInventorCommand; }
		}

		public DelegateCommand NavigateToBranchContextCommand
		{
			get { return this._navigateToBranchContextCommand; }
		}

		private bool EditCustomerOptionsCommandCanExecute()
		{
			
#if DEBUG
			return true;
#else
			//string redaction = FileSystem.IsAppRedactionOffice() ? "OFFICE" : "LAPTOP";
			bool isRedactionOffice = FileSystem.IsAppRedactionOffice();
			return  isRedactionOffice;
#endif

		}

		
		private bool PackOpenCommandCanExecute()
		{
			return true;
		}

		private bool UnpackOpenCommandCanExecute()
		{
			return true;
		}

		private bool CreateNewInventorCommandCanExecute()
		{


#if DEBUG
			if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextBranch
			|| base.CBIDbContext == Common.NavigationSettings.CBIDbContextCustomer)
			{
				return true;
			}
			else return false;
#else
			//string redaction = FileSystem.IsAppRedactionOffice() ? "OFFICE" : "LAPTOP";
			bool isRedactionOffice = FileSystem.IsAppRedactionOffice();
			if (isRedactionOffice == false)
			{
				return false;
			}
			else
			{
				if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextBranch
				|| base.CBIDbContext == Common.NavigationSettings.CBIDbContextCustomer)
				{
					return true;
				}
				else return false;
			}
#endif


		}


		private bool CreateAndNavigateToProcessInventorCommandCanExecuted()
		{
			if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextBranch
			|| base.CBIDbContext == Common.NavigationSettings.CBIDbContextCustomer)
			{
				return true;
			}
			else return false;
		}

		private bool ChangeCurrentInventorCommandCanExecute()
		{
			return true;
		}
		private bool GetFromFtpCommandCanExecute()
		{
			//if (this._adapterName == ImportAdapterName.ImportPdaMerkavaDB3Adapter
			//	|| this._adapterName == ImportAdapterName.ImportPdaClalitSqliteAdapter
			//	|| this._adapterName == ImportAdapterName.ImportPdaNativSqliteAdapter
			//	|| this._adapterName == ImportAdapterName.ImportPdaYesXlsxAdapter
			//	|| this._adapterName == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
			//	|| this._adapterName == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
			//{
			//	return true;
			//}
			//else
			//{
				return false;
			//}

		}

		//	       private void CustomerEdit(CustomerEditEventPayload customer)
     

		private void EditCustomerOptionsCommandExecuted()
		{						  // InventorEditOptionsEventPayload
			base.EventAggregator.GetEvent<CustomerEditSettingsEvent>().Publish(new CustomerEditSettingsEventPayload() { Customer = base.CurrentCustomer, Context = CBIContext.History });
		}

		protected void GetFromFtpCommandExecuted()
		{
			//var settings = new Dictionary<string, string>();
			//Utils.AddContextToDictionary(settings, base.Context);
			//Utils.AddDbContextToDictionary(settings, base.CBIDbContext);
			//settings.Add(NavigationSettings.AdapterName, this._adapterName);

			// object result = _modalWindowLauncher.StartModalWindow(
			//			   Common.ViewNames.FromFtpView,
			//			   WindowTitles.FromFtp,
			//			   650, 450,
			//			   ResizeMode.CanResize, settings,
			//			   null,
			//			   650, 450);
      	}

		//public bool IsCopyFromSource
		//{
		//	get { return _isCopyFromSource; }
		//	set
		//	{
		//		_isCopyFromSource = value;
		//		RaisePropertyChanged(() => IsCopyFromSource);

		//		_userSettingsManager.CopyFromSourceSet(_isCopyFromSource);
		//	}
		//}

		public string AdapterName
		{
			get { return _adapterName; }
		}

		

        public bool IsContinueGrabFiles
        {
            get { return _isContinueGrabFiles; }
            set
            {
                _isContinueGrabFiles = value;
                RaisePropertyChanged(() => IsContinueGrabFiles);
            }
        }

        public bool IsContinueGrabFilesEnabled
        {
            get { return _isContinueGrabFilesEnabled; }
            set
            {
                _isContinueGrabFilesEnabled = value;
                RaisePropertyChanged(() => IsContinueGrabFilesEnabled);
            }
        }

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			this.EventAggregator.GetEvent<ComplexAdapterEvent>().Subscribe(NavigateToContext);
			this.EventAggregator.GetEvent<ComplexAdapterRecountProductEvent>().Subscribe(ReCountProducts);
			this.EventAggregator.GetEvent<ComplexAdapterRecountInventProductEvent>().Subscribe(ReCountSumQuantity);
			

			UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
			CBIContext? cbiContext = Utils.CBIContextFromNavigationParameters(navigationContext);
			Utils.SetCurrentAuditConfig(navigationContext, base._contextCBIRepository);
			if (cbiContext != null)
			{
				CBIContext? tempContext = cbiContext;
			}

			switch (base.CBIDbContext)
			{
				case Common.NavigationSettings.CBIDbContextCustomer:
					{
						//RegionManager.SetRegionName(backForward, backForwardRegionName);

						//Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
						//Utils.MainWindowTitleSet(WindowTitles.Catalog, this._eventAggregator);

						//UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
						//UtilsNavigate.BackForwardNavigate(this._regionManager, backForwardRegionName);
						//UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomer);
						//Utils.MainWindowTitleSet(WindowTitles.CustomerDashboard, base.EventAggregator);
							this. _fromChangeCurrentInventor = true;
						this. _fromCurrentInventor = false;
					}
					break;
				case Common.NavigationSettings.CBIDbContextBranch:
					{
						//	bool dbFileMissed = UtilsNavigate.DataFileMissed(navigationContext, base._contextCBIRepository, this._dbSettings, Window.GetWindow(this), this._userSettingsManager);
						//UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranch);
  						//Utils.MainWindowTitleSet(WindowTitles.BranchDashboard, base.EventAggregator);
						this. _fromChangeCurrentInventor = true;
						this. _fromCurrentInventor = false;

						this.Build();
					}
					break;
				case Common.NavigationSettings.CBIDbContextInventor:
					{
						//UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranchInventor);
						//Utils.MainWindowTitleSet(WindowTitles.InventorDashboard, base.EventAggregator);

						string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
						string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
						string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

						base.Adapters = new ObservableCollection<IImportModuleInfo>(
							Utils.GetImportAdapters(base.Container, this._importAdapterRepository, ImportDomainEnum.ComplexOperation,
													currentCustomerCode, currentBranchCode, currentInventorCode));


						string currentAdapter = GetSelectedAdapterNameByCBI();
						if (String.IsNullOrWhiteSpace(currentAdapter) == true)
						{
							this.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.IsDefault);
						}
						else
						{
							this.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.Name == currentAdapter);
						}

						this._fromChangeCurrentInventor = false;	   
						this._fromCurrentInventor = true;
					}
					break;
			}



			//MIS 
			string importMisPDAFolder = this._userSettingsManager.ImportPDAPathGet();	//"C:\MIS\"
			this._misPath = importMisPDAFolder.Trim('\\') + @"\IDnextData\fromHT"; //"C:\MIS\IDnextData\fromHT"

			//MIS 	 unsure
			//C:\MIS\IDnextData\fromHT\unsure
			this._misUnsurePath = this._misPath + @"\unsure";

			// importFolder/inData
			string importFolderPath = base.ContextCBIRepository.GetImportFolderPath(base.CurrentInventor);
			this._sourcePath = Path.Combine(importFolderPath, FileSystem.inData);
			observCountingFiles = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(6)).Select(x => x);
			disposeObservCountingFiles = observCountingFiles.Subscribe(CountingFiles);
			RaisePropertyChanged(() => IsMisImportFromPDA);	


			//this._extraSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaExtraSettingsViewModel>(Common.RegionNames.ExportPdaExtraSettings, this._regionManager);
			//if (this._extraSettingsViewModel != null)
			//{
			//	switch (base.CBIDbContext)
			//	{
			//		case Common.NavigationSettings.CBIDbContextCustomer:
			//			this._extraSettingsViewModel.SetCustomer(base.CurrentCustomer);
			//			break;
			//		case Common.NavigationSettings.CBIDbContextBranch:
			//			this._extraSettingsViewModel.SetBranch(base.CurrentBranch, enBranchAdapterInherit.InheritNothing);
			//			break;
			//		case Common.NavigationSettings.CBIDbContextInventor:
			//			this._extraSettingsViewModel.SetInventor(base.CurrentInventor, enInventorAdapterInherit.InheritNothing);
			//			break;
			//	}
			//}

			//_isNavigateBack = navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.NavigateBackCheckboxIsChecked);
			//_isNavigateBack = _userSettingsManager.NavigateBackImportPdaFormGet();
			//			_isCopyFromSource = _userSettingsManager.CopyFromSourceGet();

			//if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AutoStartImportPda))
			//{
			//	if (base.ImportCommand.CanExecute())
			//	{
			//		base.ImportCommand.Execute();
			//	}
			//}
		}
   

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
			
			this.EventAggregator.GetEvent<ComplexAdapterEvent>().Unsubscribe(NavigateToContext);
			this.EventAggregator.GetEvent<ComplexAdapterRecountProductEvent>().Unsubscribe(ReCountProducts);
			this.EventAggregator.GetEvent<ComplexAdapterRecountInventProductEvent>().Unsubscribe(ReCountSumQuantity);
					
			

			if (disposeObservCountingFiles != null) disposeObservCountingFiles.Dispose();
        }

		public void NavigateToContext(ComplexAdapterEventPayload to)		 
		{
			if (to.ToContext == ToContext.ToBranchWithoutAction)
			{
				NavigateToBranchContext();
			}
		}

        private bool ImportCommandCanExecute()
        {
			return false;
			//bool canImportByDynViewModel = base.DynViewModel == null ? true : base.DynViewModel.CanImport();
			//return !base.IsBusy && canImportByDynViewModel;
        }

		private bool NavigateToGridCommandCanExecute()
		{
			return false;
			//bool canImportByDynViewModel = base.DynViewModel == null ? true : base.DynViewModel.CanImport();
			//return !base.IsBusy && canImportByDynViewModel;
		}

        private void ImportCommandExecuted()
        {
            if (base.DynViewModel != null)
            {
                if (base.DynViewModel.CanImport())
                {
                    ImportFromPdaCommandInfo info = new ImportFromPdaCommandInfo();
                    info.IsWriteLogToFile = base.IsWriteLogToFile;
                    info.Callback = () => Utils.RunOnUI(() =>
                        {
                            base.Progress = String.Empty;
                            base.ProgressStep = String.Empty;
                            this.CollectNewImportDocumentList();

							//if (_isNavigateBack)
							//{
							//	if (UtilsNavigate.CanGoBack(base._regionManager))
							//		UtilsNavigate.GoBack(base._regionManager);
							//}
							//else
							//{
							//	ShowLog();
							//}
                        });
                    base.Cts = new CancellationTokenSource();                    
                    info.CancellationToken = base.Cts.Token;
                    info.IsContinueGrabFiles = _isContinueGrabFiles;

					//if (this._extraSettingsViewModel != null)
					//{
					//	GenerationReport.Report report = this._extraSettingsViewModel.SelectedReport;

					//	info.IsAutoPrint = this._extraSettingsViewModel.IsAutoPrint;
					//	info.Report = report;
					//}

                    base.DynViewModel.RunImportBase(info);
                }
            }
        }

		//public override void InitXDocumentConfig(XDocument configXDocument)
		//{
			
		//}   


        protected override void OnSelectedAdapterChanged()
        {
            base.OnSelectedAdapterChanged();

            this._setAsDefaultCommand.RaiseCanExecuteChanged();

            if (base.DynViewModel != null)
            {
                base.DynViewModel.UpdateBusyText = UpdateBusyText;
                base.DynViewModel.SetIsCancelOk = SetIsCancelOk;
                base.DynViewModel.InputFileFolderChanged = isDirectory =>
                    {                     
                        IsContinueGrabFilesEnabled = isDirectory;
                        if (!isDirectory)
                        {
                            IsContinueGrabFiles = false;
                        }
                    };

                TemplateAdapterFileFolderViewModel fileFolder = base.DynViewModel as TemplateAdapterFileFolderViewModel;
                if (fileFolder != null)
                {
                    if (fileFolder.IsDirectory == false)
                    {
                        IsContinueGrabFiles = false;    
                    }                    
                    IsContinueGrabFilesEnabled = fileFolder.IsDirectory;
                }

                TemplateAdapterOneFileViewModel oneFile = base.DynViewModel as TemplateAdapterOneFileViewModel;
                if (oneFile != null)
                {
                    IsContinueGrabFiles = false;
                    IsContinueGrabFilesEnabled = false;
                }

                TemplateAdapterTwoFilesViewModel twoFiles = base.DynViewModel as TemplateAdapterTwoFilesViewModel;
                if (twoFiles != null)
                {
                    IsContinueGrabFiles = false;
                    IsContinueGrabFilesEnabled = false;
                }

				_adapterName = base.DynViewModel.AdapterName;

			
            }
			this._getFromFtpCommand.RaiseCanExecuteChanged();
        }

        private void UpdateBusyText(string text)
        {
            Utils.RunOnUI(() =>
                {
                    if (String.IsNullOrWhiteSpace(text))
                    {
                        ProgressText = Localization.Resources.ViewModel_ImportWithModulesBase_ImportProgress;
                    }
                    else
                    {
                        ProgressText = text;
                    }
                });
        }

        private void SetIsCancelOk(bool ok)
        {
            Utils.RunOnUI(() =>
                {
                    base._isCancelOk = ok;
                    BusyCancelCommand.RaiseCanExecuteChanged();
                });
        }

        private void CollectNewImportDocumentList()
        {
            if (base.DynViewModel != null)
            {
                IImportPdaAdapter importPdaAdapter = base.DynViewModel as IImportPdaAdapter;
                if (importPdaAdapter != null)
                {
                    foreach (string code in importPdaAdapter.NewDocumentCodeList)
                    {
                        if (this._newDocumentCodeList.Contains(code) == false)
                        {
                            this._newDocumentCodeList.Add(code);
                        }
                    }
					
					foreach (string code in importPdaAdapter.NewSessionCodeList)
                    {
						if (this._newSessionCodeList.Contains(code) == false)
                        {
							this._newSessionCodeList.Add(code);
                        }
                    }
					
                }

				
            }
        }

		//public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
		//{
		//{
		//	base._isBusy = false;
		//	base.IsBusy = true;
		//	this.UpdateBusyText(Localization.Resources.View_IturListDetails_busyContent);
		//	this.SetIsCancelOk(false);
		//	_logger.Info("Task RefillApproveStatusBit ::  start");
		//	Task taskA = Task.Factory.StartNew(RefillApproveStatusBit, continuationCallback);
		//	taskA.ContinueWith(t => { _logger.Info("Task RefillApproveStatusBit ::  come to end -> OnlyOnRanToCompletion"); }, TaskContinuationOptions.OnlyOnRanToCompletion);
		//	taskA.ContinueWith(t => { _logger.Info("Task RefillApproveStatusBit ::  Canceled"); }, TaskContinuationOptions.OnlyOnCanceled);
		//	taskA.LogTaskFactoryExceptions("ConfirmNavigationRequest");
		//}

		//private void RefillApproveStatusBit(object state)
		//{
			//try
			//{
			//	//////this._iturRepository.RefillApproveStatusBit(this._newDocumentCodeList, this._newSessionCodeList,  base.GetDbPath);
			//	//////this._iturRepository.RefillApproveStatusBit(base.GetDbPath);
				

			//	ImportPdaPrintQueue queue = _serviceLocator.GetInstance<ImportPdaPrintQueue>();
			//	if (queue.IsPrinting)
			//	{
			//		Utils.RunOnUI(() =>
			//			{
			//				UpdateBusyText(Localization.Resources.ViewModel_ImportFromPda_Printing);
			//			});

			//		while (queue.IsPrinting)
			//		{
			//			Thread.Sleep(1000);
			//		}
			//	}
			//}
			//catch (Exception exc)
			//{
			//	_logger.ErrorException("RefillApproveStatusBit", exc);
			//}
			//finally
			//{
			//	Utils.RunOnUI(() =>
			//		{
			//			this.UpdateBusyText(String.Empty);
			//			base.IsBusy = false;
			//			this.SetIsCancelOk(true);

			//			Action<bool> continuationCallback = state as Action<bool>;
			//			if (continuationCallback != null)
			//				continuationCallback(true);
			//		});
			//}
      //  }

        private bool SetAsDefaultCommandCanExecute()
        {
			if (base.Adapters == null) return false;
            return base.Adapters.Any() && base.SelectedAdapter != null
                   && base.SelectedAdapter.Name != this.GetSelectedAdapterNameByCBI();
        }

        private void SetAsDefaultCommandExecuted()
        {
            using (new CursorWait())
            {
                try
                {
					string adapterName = "";
					if (base.SelectedAdapter != null) adapterName = base.SelectedAdapter.Name;
                    switch (base.CBIDbContext)
                    {
                        case NavigationSettings.CBIDbContextCustomer:
							base.CurrentCustomer.ComplexAdapterCode = adapterName;
                            this._customerRepository.Update(base.CurrentCustomer);
                            break;
                        case NavigationSettings.CBIDbContextBranch:
							base.CurrentBranch.ComplexAdapterCode = adapterName;
                            this._branchRepository.Update(base.CurrentBranch);
                            break;
                        case NavigationSettings.CBIDbContextInventor:
							base.CurrentInventor.ComplexAdapterCode = adapterName;
                            this._inventorRepository.Update(base.CurrentInventor);
                            break;

                    }

                    this._setAsDefaultCommand.RaiseCanExecuteChanged();
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("SetAsDefaultCommandExecuted", exc);
                }
            }
        }


		private void SetAsDefaultLikeInCustomerCommandExecuted()
		{
			using (new CursorWait())
			{
				try
				{
					Customer currentCustomer = this._customerRepository.GetCustomer(base.CurrentCustomer.Code);
					if (currentCustomer == null) return;
					string adapterName = currentCustomer.ComplexAdapterCode;
					switch (base.CBIDbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							base.CurrentCustomer.ComplexAdapterCode = adapterName;
							this._customerRepository.Update(base.CurrentCustomer);
							break;
						case NavigationSettings.CBIDbContextBranch:
							base.CurrentBranch.ComplexAdapterCode = adapterName;
							this._branchRepository.Update(base.CurrentBranch);
							break;
						case NavigationSettings.CBIDbContextInventor:
							base.CurrentInventor.ComplexAdapterCode = adapterName;
							this._inventorRepository.Update(base.CurrentInventor);
							break;

					}

					this._setAsDefaultCommand.RaiseCanExecuteChanged();
					this.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.Name == adapterName);
				}
				catch (Exception exc)
				{
					_logger.ErrorException("SetAsDefaultCommandExecuted", exc);
				}
			}
		}

        private string GetSelectedAdapterNameByCBI()
        {
			//switch (base.CBIDbContext)
			//{
				//case Common.NavigationSettings.CBIDbContextCustomer:
				//	return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ReportContext;
				//case Common.NavigationSettings.CBIDbContextBranch:
				//	return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ReportContext;
				//case Common.NavigationSettings.CBIDbContextInventor:
				//	return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ReportContext;
				string complexAdapterName = String.Empty; 
				if (base.CurrentInventor != null )  
				{
					complexAdapterName = base.CurrentInventor.ComplexAdapterCode;				
				}
				if (string.IsNullOrWhiteSpace(complexAdapterName) == false) return  complexAdapterName;

				if (base.CurrentBranch != null )  
				{
					complexAdapterName = base.CurrentBranch.ComplexAdapterCode;				
				}
				if (string.IsNullOrWhiteSpace(complexAdapterName) == false) return  complexAdapterName;

				if (base.CurrentCustomer != null )  
				{
					complexAdapterName = base.CurrentCustomer.ComplexAdapterCode;				
				}
				if (string.IsNullOrWhiteSpace(complexAdapterName) == false) return  complexAdapterName;
   	           return String.Empty;
        }


	
		private void NavigateToBranchContextCommandExecuted()
		{
			NavigateToBranchContext();
		}

		private void NavigateToBranchContext()
		{
			TempSaveAdapterName = "";
			if (SelectedAdapter != null) TempSaveAdapterName = SelectedAdapter.Name; //сохраняем текущий адаптер, при создании нового инвентора - его установим
			AuditConfig auditConfig = base.GetAuditConfigByCurrentContext();
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, auditConfig);
			base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearInventor();

			AuditConfig newMainAuditConfig = new AuditConfig(auditConfig);
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, newMainAuditConfig);
			base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearInventor();

			AuditConfig newCreateAuditConfig = new AuditConfig(auditConfig);
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.CreateInventor, newCreateAuditConfig);
			base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor).ClearInventor();

			UriQuery query = new UriQuery();
			Utils.AddAuditConfigToQuery(query, newMainAuditConfig);
			//UtilsNavigate.BranchDashboardOpen(CBIContext.Main, this._regionManager, query);		//    переход на бранч дашборд



			//Utils.AddContextToQuery(query, CBIContext.Main);
			//////Utils.AddContextToQuery(uriQuery, base.Context);
			//////Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
			//////Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());


			//Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextBranch);
			query.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ComplexModeObject);
			UtilsNavigate.ComplexOperationBranchViewOpen(this._regionManager, query);
		}

		//private void ComplexOperationCommandExecuted()
		//{
		//	UriQuery uriQuery = new UriQuery();
		//	//Utils.AddContextToQuery(uriQuery, base.Context);
		//	//Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
		//	uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ComplexModeObject);
		//	Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
		//	UtilsNavigate.ComplexOperationViewOpen(this._regionManager, uriQuery);
		//}

		//private void NavigateToBranchContext(NavigationContext navigationContext)
		//{
		//	Utils.SetCurrentAuditConfig(navigationContext, base._contextCBIRepository);

		////	bool dbFileMissed = UtilsNavigate.DataFileMissed(navigationContext, base._contextCBIRepository, this._dbSettings, Window.GetWindow(this), this._userSettingsManager);

		//	UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
		//	UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranch);
		//	RegionManager.SetRegionName(backForward, Common.RegionNames.BranchDashboardBackForward);
		//	UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.BranchDashboardBackForward);

		//	this._navigationContext = navigationContext;
		//	CBIContext? cbiContext = Utils.CBIContextFromNavigationParameters(navigationContext);
		//	if (cbiContext != null)
		//		Context = cbiContext.Value;

		//	Utils.MainWindowTitleSet(WindowTitles.BranchDashboard, this._eventAggregator);
		//}

		private void NavigateToInventorContextCommandExecuted()
		{
			NavigateToInventorContext(FromContext.FromBranchWithoutAction);
		}

		private void NavigateToInventorContext(FromContext fromContext)
		{
			var proc = base.ContextCBIRepository.GetProcessCBIConfig(CBIContext.History);
			if (proc == null) return;

			AuditConfig processAuditConfig = new AuditConfig(proc);

			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, processAuditConfig);

			AuditConfig newMainAuditConfig = new AuditConfig(processAuditConfig);
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, newMainAuditConfig);

			AuditConfig newCreateAuditConfig = new AuditConfig(processAuditConfig);
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.CreateInventor, newCreateAuditConfig);

			Inventor inventor = this._inventorRepository.GetInventorByCode(proc.InventorCode);
			if (inventor != null)
			{
				if (string.IsNullOrWhiteSpace(inventor.ComplexAdapterCode) == true)
				{
					inventor.ComplexAdapterCode = TempSaveAdapterName;
					this._inventorRepository.Update(inventor);
				}
			}

			UriQuery uriQuery = new UriQuery();
			Utils.AddAuditConfigToQuery(uriQuery, processAuditConfig);
			uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ComplexModeObject);

			uriQuery = Utils.AddFromContextToQuery(uriQuery, fromContext);

			UtilsNavigate.ComplexOperationViewOpen(this._regionManager, uriQuery);
		}

		//public static void OpenProcessInventoryExecute(IContextCBIRepository contextCbiRepository, IRegionManager regionManager)
		//{
		//	AuditConfig config = new AuditConfig(contextCbiRepository.GetProcessCBIConfig(CBIContext.History));
		//	contextCbiRepository.SetCurrentCBIConfig(CBIContext.History, config);

		//	UriQuery query = new UriQuery();
		//	Utils.AddContextToQuery(query, CBIContext.History);
		//	Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
		//	Utils.AddAuditConfigToQuery(query, config);

		//	UtilsNavigate.IturListDetailsOpen(regionManager, query);
		//}


		//public void AuditNavigateCommandExecuted(FileItemViewModel item)
		//{
		//	AuditConfig config = base.ContextCBIRepository.GetCBIConfigByInventorCode(CBIContext.History, item.ObjectCode);
		//	if (config != null)
		//	{
		//		base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, config);
		//		base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, config);
		//	}

		//	UriQuery query = new UriQuery();
		//	Utils.AddAuditConfigToQuery(query, GetHistoryAuditConfig());
		//	UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
		//}

		//private void AuditNavigateCommandExecuted(LastInventorsListItem item)
		//{
		//	base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, item.AuditConfig);
		//	base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, item.AuditConfig);

		//	Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);

		//	UriQuery query = new UriQuery();
		//	Utils.AddAuditConfigToQuery(query, GetHistoryAuditConfig());
		//	UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
		//}

		private void BranchDashboardNavigateCommandExecuted()
		{
			AuditConfig auditConfig = base.GetAuditConfigByCurrentContext();
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, auditConfig);
			base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearInventor();

			AuditConfig newMainAuditConfig = new AuditConfig(auditConfig);
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, newMainAuditConfig);
			base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearInventor();

			AuditConfig newCreateAuditConfig = new AuditConfig(auditConfig);
			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.CreateInventor, newCreateAuditConfig);
			base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor).ClearInventor();

			UriQuery query = new UriQuery();
			Utils.AddAuditConfigToQuery(query, newMainAuditConfig);
			UtilsNavigate.BranchDashboardOpen(CBIContext.Main, this._regionManager, query);
		}

		//private void NavigateAfterSave()
		//{
		//	try
		//	{
		//		using (new CursorWait())
		//		{
		//			AuditConfig auditConfig = base.GetAuditConfigByCurrentContext();
		//			base.ContextCBIRepository.SetProcessCBIConfig(CBIContext.History, auditConfig);
		//			base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, auditConfig);

		//			Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);

		//			InventorPostData data = new InventorPostData() { InventorCode = _inventor.Code, IsNew = true };
		//			UriQuery query = new UriQuery();
		//			Utils.AddContextToQuery(query, this.Context);
		//			Utils.AddDbContextToQuery(query, base.CBIDbContext);
		//			UtilsConvert.AddObjectToQuery(query, _navigationRepository, data, Common.NavigationObjects.InventorPost);

		//			_regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.InventorPostView + query, UriKind.Relative));
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("NavigateAfterSave", exc);
		//	}
		//}

		//public void InventorNavigate(InventorItemViewModel viewModel)
		//{
		//	UtilsPopup.Close(View);

		//	Customer customer = base.ContextCBIRepository.GetCustomerByCode(viewModel.Inventor.CustomerCode);
		//	base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetAuditConfigByCurrentContext());

		//	Branch branch = base.ContextCBIRepository.GetBranchByCode(viewModel.Inventor.BranchCode);
		//	base.ContextCBIRepository.SetCurrentBranch(branch, this.GetAuditConfigByCurrentContext());

		//	AuditConfig config = base.ContextCBIRepository.GetCBIConfigByInventorCode(CBIContext.History, this._inventorChooseCurrent.Inventor.Code);
		//	if (config != null)
		//	{
		//		base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, config);
		//		base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, config);
		//	}

		//	Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);

		//	UriQuery query = new UriQuery();
		//	Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
		//	UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
		//}

		private bool NavigateToInventorContextCommandCanExecute()
		{
			//if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor)
			//{
			//	return true;
			//}
			if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextBranch
				|| base.CBIDbContext == Common.NavigationSettings.CBIDbContextCustomer)
			{
				return true;
			}
			else return false;
		}

		private bool NavigateToBranchContextCommandCanExecute()
		{
			if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor)
			{
				return true;
			}
		
			else return false;
		}

		private void NavigateToGridCommandExecuted()
		{
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, base.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());


			UtilsNavigate.InventProductListOpen(this._regionManager, query);
		}

		private void PackOpenCommandExecuted()
		{
			UriQuery uriQuery = new UriQuery();
			Utils.AddContextToQuery(uriQuery, CBIContext.Main);
			Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextCustomer);
			Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
			UtilsNavigate.PackOpen(this._regionManager, uriQuery);
		}

		private void UnpackOpenCommandExecuted()
		{
			UriQuery uriQuery = new UriQuery();
			Utils.AddContextToQuery(uriQuery, CBIContext.Main);
			Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextCustomer);
			Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
			UtilsNavigate.UnpackOpen(this._regionManager, uriQuery);
		}



		private void CreateAndNavigateToProcessInventorCommandExecuted()
		{
			//CreateNewInventorCommandExecuted();

			string inventorCode = this.CreateCloneInProcessInventor(); //Create New Inventor

			if (string.IsNullOrWhiteSpace(inventorCode) == false)
			{
				AuditConfig auditConfig = this._auditConfigRepository.GetAuditConfigByInventorCode(inventorCode, CBIContext.History);
				base.ContextCBIRepository.SetProcessCBIConfig(CBIContext.History, auditConfig);
			}

			// FromBranchWithAction не срабатывает из-за SelectAdapter, где Withoutaction - !!! TODO пока Ирану не надо
		//	NavigateToInventorContext(FromContext.FromBranchWithAction); !!! TODO
			NavigateToInventorContext(FromContext.FromBranchWithoutAction);
		}

		//private void InheritInventorFromCustomerCommandExecuted()
		//{
		//	string inheritFromDBPath = String.Empty;

		//	Inventor sourceInventorDb = SelectedItem.InventorObject;
		//	if (sourceInventorDb != null)
		//	{
		//		inheritFromDBPath = ContextCBIRepository.BuildRelativeDbPath(sourceInventorDb);
		//		IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
		//		//inventorRepository.Insert(base.CurrentInventor, inheritFromDBPath);
		//		if (String.IsNullOrEmpty(inheritFromDBPath) == false)
		//		{
		//			DBSettings dbSettings = this._serviceLocator.GetInstance<DBSettings>();
		//			string folder = dbSettings.FolderInventor.Trim('\\') + @"\";
		//			string sourcePath = inventorRepository.Connection.BuildCount4UDBFilePath(inheritFromDBPath);
		//			inventorRepository.Connection.CopyDB(base.CurrentInventor.DBPath, folder, sourcePath);
		//		}

		//	}
		//}


		private string CreateCloneInProcessInventor()
		{
			IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
			ICustomerRepository customerRepository = this._serviceLocator.GetInstance<ICustomerRepository>();
			IAuditConfigRepository auditConfigRepository = this._serviceLocator.GetInstance<IAuditConfigRepository>();
			AuditConfigs auditConfigs = new AuditConfigs();

			AuditConfig inProcessAuditConfig = base.ContextCBIRepository.GetProcessCBIConfig(CBIContext.History);
			if (inProcessAuditConfig == null) return "";
			string inventorCode = inProcessAuditConfig.InventorCode;
			if (string.IsNullOrWhiteSpace(inventorCode) == true) return "";
			Inventor oldInvetor = inventorRepository.GetCurrent(inProcessAuditConfig);	   //Выбор инвентора с которого копируем все
			if (oldInvetor == null) return "";
			Customer currentCustomer = customerRepository.GetCurrent(inProcessAuditConfig);
			  if (currentCustomer == null) return "";

			AuditConfig mainAC = base.GetMainAuditConfig();  //currentAuditConfig
			if (mainAC == null) return "";
	
	
			// И создаем снова	 AuditConfig
			AuditConfig cloneMainAC = auditConfigRepository.Clone(mainAC);			  // создаем копию с AyditConfig
			if (cloneMainAC == null) return "";

			string newInventorCode = Utils.CodeNewGenerate();

			Inventor newInvetor = oldInvetor.Clone(newInventorCode, oldInvetor.Code);
			newInvetor.Code = newInventorCode;
			newInvetor.CreateDate = DateTime.Now;
			newInvetor.CompleteDate = DateTime.Now;
			newInvetor.LastUpdatedCatalog = DateTime.Now;
			newInvetor.InventorDate = DateTime.Now;
			newInvetor.Description = ""; 
			newInvetor.DBPath = this._contextCBIRepository.CreateNewDBPath(newInvetor, newInventorCode);
			newInvetor.Name = newInvetor.InventorDate.ToString();

			newInvetor.ExportCatalogAdapterCode = currentCustomer.ExportCatalogAdapterCode;
			newInvetor.ImportPDAProviderCode = currentCustomer.ImportPDAProviderCode;
			newInvetor.ExportERPAdapterCode = currentCustomer.ExportERPAdapterCode;
			newInvetor.ImportCatalogAdapterCode = currentCustomer.ImportCatalogProviderCode;
			newInvetor.ImportCatalogAdapterCode = currentCustomer.ImportCatalogProviderCode;
			newInvetor.ImportIturAdapterCode = currentCustomer.ImportIturProviderCode;
			newInvetor.ImportLocationAdapterCode = currentCustomer.ImportLocationProviderCode;
			newInvetor.ImportSectionAdapterCode = currentCustomer.ImportSectionAdapterCode;
			newInvetor.ImportSupplierAdapterCode = currentCustomer.ImportSupplierAdapterCode;
			newInvetor.UpdateCatalogAdapterCode = currentCustomer.UpdateCatalogAdapterCode;
			newInvetor.ImportFamilyAdapterCode = currentCustomer.ImportFamilyAdapterCode;		//ImportFamily


			if (newInvetor == null) return "";

			cloneMainAC.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();	 // ??
			cloneMainAC.Description = "";
			cloneMainAC.InventorCode = newInvetor.Code;
			cloneMainAC.InventorName = newInvetor.Name;
			cloneMainAC.InventorDate = newInvetor.InventorDate;
			cloneMainAC.DBPath = newInvetor.DBPath;
			
			cloneMainAC.StatusInventorCode = StatusInventorEnum.New.ToString();

			auditConfigs.Add(cloneMainAC);
			auditConfigRepository.Insert(auditConfigs);
		
			string inheritFromDBPath = String.Empty;
			inheritFromDBPath = base.ContextCBIRepository.BuildRelativeDbPath(currentCustomer);
			DateTime lastUpdatedCatalog = base.ContextCBIRepository.GetLastUpdatedCatalog(currentCustomer);
			newInvetor.LastUpdatedCatalog = lastUpdatedCatalog;

			inventorRepository.Insert(newInvetor, inheritFromDBPath);	//!!! Это копирование DB как раз
			base.ContextCBIRepository.GetImportFolderPath(newInvetor);		 //создание папки импорта личной
			return newInvetor.Code;
			}

		private void CreateNewInventorCommandExecuted()
		{
			AuditConfig mainAuditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
			AuditConfig createInventorConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
			Customer customer = base.ContextCBIRepository.GetCurrentCustomer(mainAuditConfig);
			Branch branch = base.ContextCBIRepository.GetCurrentBranch(mainAuditConfig);
			base.ContextCBIRepository.SetCurrentBranch(branch, createInventorConfig);

			base.EventAggregator.GetEvent<InventorAddEvent>().Publish(new InventorAddEventPayload()
			{
				IsCustomerComboVisible = false,
				IsBranchComboVisible = false,
				Context = CBIContext.CreateInventor	,
				WithoutNavigate = true ,
			});

			this.Build();

		}

		private void ChangeCurrentInventorCommandExecuted()
		{
			
			//UriQuery uriQuery = new UriQuery();
			//Utils.AddContextToQuery(uriQuery, CBIContext.Main);
			//Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextBranch);
			//Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));

		}

		// ============================== Statistik 
		//public string ProgressTotalItems
		//{
		//	get { return String.Format(Localization.Resources.View_IturListDetails_tbTotalStatus, _itemsTotal); }
		//}

		//public string LastSessionCountItemString
		//{
		//	get { return String.Format(Localization.Resources.View_IturListDetails_tbLastSessionCountItem, this._lastSessionCountItem); }
		//}


		//public string LastSessionCountDocumentsString
		//{
		//	get { return String.Format(Localization.Resources.View_IturListDetails_tbLastSessionCountDocuments, this._lastSessionCountDocument); }
		//}

		public string QuantityFilesInSourceFolderString
		{
			get { return String.Format(Localization.Resources.View_IturListDetails_tbQuantityFilesInSourceFolder, this.QuantityFilesInSourceFolder); }
		}

		public string QuantityFilesInMISFolderString
		{
			get { return String.Format(Localization.Resources.View_IturListDetails_tbQuantityFilesInMISFolder, this.QuantityFilesInMISFolder); }
		}


		public string QuantityFilesInUnsureFolderString
		{
			get { return String.Format(Localization.Resources.View_IturListDetails_tbQuantityFilesInUnsureFolder, this.QuantityFilesInUnsureFolder); }
		}

		public void CountingFiles(long x)
		{
			bool copyFromSource = _userSettingsManager.CountingFromSourceGet(); //слушать или нет

			if (copyFromSource == false)
			{
				this.QuantityFilesInSourceFolder = 0;
			}
			else if (string.IsNullOrWhiteSpace(this._sourcePath) == true)
			{
				this.QuantityFilesInSourceFolder = 0;
			}
			else if (Directory.Exists(this._sourcePath) == true)
			{
				DirectoryInfo dir = new System.IO.DirectoryInfo(this._sourcePath);
				this.QuantityFilesInSourceFolder = dir.GetFiles().Length;
			}
			else
			{
				this.QuantityFilesInSourceFolder = 0;
			}

			//C:\MIS\IDnextData\fromHT\unsure
			// Unsure
			//QuantityFilesInUnsureFolder

			//MIS
			//if (base.CurrentInventor.ImportPDAProviderCode == Common.Constants.ImportAdapterName.ImportPdaMISAndDefaultAdapter
			//|| base.CurrentInventor.ImportPDAProviderCode == Common.Constants.ImportAdapterName.ImportPdaMISAdapter)
			//{
				if (copyFromSource == false)
				{
					this.QuantityFilesInMISFolder = 0;
				}
				else if (string.IsNullOrWhiteSpace(this._misPath) == true)
				{
					this.QuantityFilesInMISFolder = 0;
				}
				else if (Directory.Exists(this._misPath) == true)
				{
					DirectoryInfo dir = new System.IO.DirectoryInfo(this._misPath);
					this.QuantityFilesInMISFolder = dir.GetFiles().Length;
				}
				else
				{
					this.QuantityFilesInMISFolder = 0;
				}


				//C:\MIS\IDnextData\fromHT\unsure
				// Unsure
				//QuantityFilesInUnsureFolder

				if (copyFromSource == false)
				{
					this.QuantityFilesInUnsureFolder = 0;
				}
				else if (string.IsNullOrWhiteSpace(this._misUnsurePath) == true)
				{
					this.QuantityFilesInUnsureFolder = 0;
				}
				else if (Directory.Exists(this._misUnsurePath) == true)
				{
					DirectoryInfo dir = new System.IO.DirectoryInfo(this._misUnsurePath);
					this.QuantityFilesInUnsureFolder = dir.GetFiles().Length;
				}
				else
				{
					this.QuantityFilesInUnsureFolder = 0;
				}

				//SumQuantityEdit = this.GetSumQuantityEdit(base.State.GetDbPath);
				//SumQuantityEditString = String.Format(Localization.Resources.ViewModel_IturListDetails_ProgressDoneTotalItemsString, String.Format("{0:0,##}", SumQuantityEdit));

				//CountProducts = this.GetCountProducts(base.State.GetDbPath);
				//CountProductsString = String.Format(Localization.Resources.ViewModel_IturListDetails_CountProductsString, CountProducts);
			//}
		}


		//public void NavigateToContext(ComplexAdapterEventPayload to)
		//{
		//	if (to.ToContext == ToContext.ToBranchWithoutAction)
		//	{
		//		NavigateToBranchContext();
		//	}
		//}

		public void ReCountProducts(ComplexAdapterRecountProductEventPayload recount)
		{
			CountProducts = this.GetCountProducts(base.State.GetDbPath);
			CountProductsString = String.Format(Localization.Resources.ViewModel_IturListDetails_CountProductsString, CountProducts);
		}

		public void ReCountSumQuantity(ComplexAdapterRecountInventProductEventPayload recount)		
		{
			  SumQuantityEdit = this.GetSumQuantityEdit(base.State.GetDbPath);
			  SumQuantityEditString = String.Format(Localization.Resources.ViewModel_IturListDetails_ProgressDoneTotalItemsString, String.Format("{0:0,##}", SumQuantityEdit));
		}


		public string SourcePath
		{
			get { return this._sourcePath; }
			//set
			//{
			//this._sourcePath = value;
			//this.RaisePropertyChanged(() => this.SourcePath);
			//CountingFiles(0);
			//}
		}

		//public string LastSessionSumQuantityEditString
		//{
		//	get { return String.Format(Localization.Resources.View_IturListDetails_tbLastSessionSumQuantityEdit, Convert.ToInt64(this._lastSessionSumQuantityEdit)); }
		//}

	

		public bool IsMisImportFromPDA
		{
			get
			{
				//bool isMisImportFromPDA = false;
				////	bool isMisImportFromPDA = (base.CurrentInventor.ImportPDAProviderCode == "ImportPdaMISAdapter");
				////	return isMisImportFromPDA; 
				//if (this.CurrentInventor.ImportPDAProviderCod  == Common.Constants.ImportAdapterName.ImportPdaMISAndDefaultAdapter
				//|| base.CurrentInventor.ImportPDAProviderCode == Common.Constants.ImportAdapterName.ImportPdaMISAdapter)
				//{
				//	isMisImportFromPDA = true;
				//}

				//return isMisImportFromPDA;
				return true;
			}
		}

		public int QuantityFilesInSourceFolder
		{
			get { return _quantityFilesInSourceFolder; }
			set
			{
				this._quantityFilesInSourceFolder = value;
				this.RaisePropertyChanged(() => this.QuantityFilesInSourceFolder);
				this.RaisePropertyChanged(() => this.QuantityFilesInSourceFolderString);
			}
		}

		public int QuantityFilesInMISFolder
		{
			get { return _quantityFilesInMISFolder; }
			set
			{
				this._quantityFilesInMISFolder = value;
				this.RaisePropertyChanged(() => this.QuantityFilesInMISFolder);
				this.RaisePropertyChanged(() => this.QuantityFilesInMISFolderString);
			}
		}


		public int QuantityFilesInUnsureFolder
		{
			get { return _quantityFilesInUnsureFolder; }
			set
			{
				this._quantityFilesInUnsureFolder = value;
				this.RaisePropertyChanged(() => this.QuantityFilesInUnsureFolder);
				this.RaisePropertyChanged(() => this.QuantityFilesInUnsureFolderString);
			}
		}

	

		//public long ItemsTotal
		//{
		//	get { return this._itemsTotal; }
		//	set
		//	{
		//		this._itemsTotal = value;
		//		this.RaisePropertyChanged(() => this.ItemsTotal);

		//		this.RaisePropertyChanged(() => this.ProgressTotalItems);
		//	}
		//}


		public double SumQuantityEdit
		{
			get { return this._sumQuantityEdit; }
			set
			{
				this._sumQuantityEdit = value;
				this.RaisePropertyChanged(() => this.SumQuantityEdit);
			}
		}

		public string SumQuantityEditString
		{
			get { return this._sumQuantityEditString; }
			set
			{
				this._sumQuantityEditString = value;
				this.RaisePropertyChanged(() => this.SumQuantityEditString);
			}
		}


		public long CountProducts
		{
			get { return this._countProducts; }
			set
			{
				this._countProducts = value;
				this.RaisePropertyChanged(() => this.CountProducts);
			}
		}

		public string CountProductsString
		{
			get { return this._countProductsString; }
			set
			{
				this._countProductsString = value;
				this.RaisePropertyChanged(() => this.CountProductsString);
			}
		}


		public double GetSumQuantityEdit(string pathCount4Udb)
		{
			InventProducts totalResult = this._inventProductRepository.GetInventProductTotal(pathCount4Udb);//base.GetDbPath);
			double sumQuantityEdit = totalResult.SumQuantityEdit;
			return sumQuantityEdit;
		}

		public long GetCountProducts(string pathCount4Udb)
		{
			long totalResult = this._productRepository.CountProduct(pathCount4Udb);//base.GetDbPath);
			return totalResult;
		}
		
		
		//public int LastSessionCountItem
		//{
		//	get { return this._lastSessionCountItem; }
		//	set
		//	{
		//		this._lastSessionCountItem = value;
		//		this.RaisePropertyChanged(() => this.LastSessionCountItem);

		//		this.RaisePropertyChanged(() => this.LastSessionCountItemString);
		//	}
		//}

		//public int LastSessionCountDocument
		//{
		//	get { return this._lastSessionCountDocument; }
		//	set
		//	{
		//		this._lastSessionCountDocument = value;
		//		this.RaisePropertyChanged(() => this.LastSessionCountDocument);

		//		this.RaisePropertyChanged(() => this.LastSessionCountDocumentsString);
		//	}
		//}



		//public double LastSessionSumQuantityEdit
		//{
		//	get { return this._lastSessionSumQuantityEdit; }
		//	set
		//	{
		//		this._lastSessionSumQuantityEdit = value;
		//		this.RaisePropertyChanged(() => this.LastSessionSumQuantityEdit);

		//		this.RaisePropertyChanged(() => this.LastSessionSumQuantityEditString);
		//	}
		//}
    }
}