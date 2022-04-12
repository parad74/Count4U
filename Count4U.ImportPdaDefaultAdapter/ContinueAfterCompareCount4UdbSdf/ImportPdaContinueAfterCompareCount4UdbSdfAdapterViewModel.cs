using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Count4U.Common.Events;
using Count4U.Common.Constants;
using System.Collections.ObjectModel;
using Count4U.Model.ExportImport.Items;
using Count4U.Model.Audit;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Count4Mobile;
using System.Reactive.Linq;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Count4U.Common.Services.UICommandService;
using Count4U.Common;
using System.Xml.Linq;

namespace Count4U.ImportPdaContinueAfterCompareCount4UdbSdfAdapter
{
	public class ImportPdaContinueAfterCompareCount4UdbSdfAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
        private readonly IIturRepository _iturRepository;
		private readonly IInventorRepository _inventorIepository;
		private readonly IAuditConfigRepository _auditConfigRepository;
        private readonly IGenerateReportRepository _generateReportRepository;
		private readonly UICommandRepository<FileItemViewModel> _commandRepositoryObject;
        private readonly IUnityContainer _unityContainer;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        //private  List<string> _newDocumentCodeList;
		private readonly List<string> _newSessionCodeList;

		public string _fileName {get; set;}
        private bool _isAutoPrint;
        private Count4U.GenerationReport.Report _report;
        private bool _isContinueGrabFiles;
		private FileItemViewModel _selectedItem;
		private bool _withSerialNumber;
		private bool _showSource;
		private bool _showDestination;
		

		private bool _byMakat;
		
		//private AdapterFileWatcher _pathFileWatcher;

		protected readonly ObservableCollection<FileItemViewModel> _items;
		protected bool _isChecked;

		protected IObservable<long> observCountingChecked;
		protected IDisposable disposeObservCountingChecked;

		private readonly DelegateCommand<FileItemViewModel> _auditNavigateCommand;
		//protected bool _isDbInventories;
		//protected bool _isDbFile;


		private readonly DelegateCommand<FileItemViewModel> _editSelectedCommand;
		private readonly DelegateCommand<FileItemViewModel> _deleteSelectedCommand;

		public ImportPdaContinueAfterCompareCount4UdbSdfAdapterViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
			IInventorRepository inventorIepository,
			IAuditConfigRepository auditConfigRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager,
			UICommandRepository<FileItemViewModel> commandRepositoryObject,
            IGenerateReportRepository generateReportRepository,
            IUnityContainer unityContainer) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
			this._items = new ObservableCollection<FileItemViewModel>();
		//	this._items.Add(new FileItemViewModel { File = "test", Size = "10", Date = "1.1.1070" });

            this._unityContainer = unityContainer;
			this._commandRepositoryObject = commandRepositoryObject;
            this._generateReportRepository = generateReportRepository;
            this._iturRepository = iturRepository;
			this._inventorIepository = inventorIepository;
			this._auditConfigRepository = auditConfigRepository;
			
           // this._newDocumentCodeList = new List<string>();
			this._newSessionCodeList = new List<string>();
			this._auditNavigateCommand = new DelegateCommand<FileItemViewModel>(this.AuditNavigateCommandExecuted);
			//this._editSelectedCommand = _commandRepositoryObject.Build(enUICommand.Edit, EditSelectedCommandExecuted);
			this._deleteSelectedCommand = _commandRepositoryObject.Build(enUICommand.Delete, DeleteSelectedCommandExecuted);

			
			//this._isDbInventories = true;
			//this._isDbFile = false;

			base.ParmsDictionary.Clear();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
        }

		//	if (this._items.Any(r => r.IsChecked) == false) TODO
		//				return false;


		public DelegateCommand<FileItemViewModel> DeleteSelectedCommand
		{
			get { return this._deleteSelectedCommand; }
		}

		private void DeleteSelectedCommandExecuted(FileItemViewModel inventorItemViewModel)
		{
			if (string.IsNullOrWhiteSpace(inventorItemViewModel.ObjectCode) == false)
			{
				this._inventorIepository.Delete(inventorItemViewModel.ObjectCode);
				this._auditConfigRepository.DeleteByInventorCode(inventorItemViewModel.ObjectCode, CBIContext.History);
				//SelectedItem = SelectedItem;
			//	_deleteSelectedCommand.Execute(SelectedItem);
				this.Build();
			}

		}

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

		public override bool CanImport()
		{
			//if (this._selectedItem == null) return false;
			var itemsCheck = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();
			var count = itemsCheck.Count();
			if (count == 2) return true;
			return false;
		}

		public FileItemViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				RaisePropertyChanged(() => SelectedItem);
				base.RaiseCanImport();
			}
		}

		protected void Build()
		{
			IInventorRepository inventorRepository = base.ServiceLocator.GetInstance<IInventorRepository>();
			IDBSettings dbSettings = base.ServiceLocator.GetInstance<IDBSettings>();
			string rootFolder = dbSettings.BuildCount4UDBFolderPath();


			this._items.Clear();
			Branch currentBranch = base.CurrentBranch;
			Inventor currentInventor = base.CurrentInventor;

			//List<string> inventorCodeList = inventorRepository.GetInventorCodeListByBranchCode(currentBranch.Code);
			Inventors inventors = inventorRepository.GetInventorsByBranchCode(currentBranch.Code);
			List<string> pathList = new List<string>();

			//string inventorDBPath = PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
			//using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(inventorDBPath)))
			//string relativeInventorPath = base.ContextCBIRepository.BuildRelativeDbPath(cbiState.CurrentInventor);
			//relativeInventorPath = Path.Combine(_dbSettings.FolderApp_Data, relativeInventorPath);
			string[] sourceInventorCodes = base.State.CurrentInventor.Manager.Split('|');

			if (this.ShowSource == true || this.ShowDestination == true) //add
			{
				try
				{
					if (this.ShowSource == true)
					{
						foreach (Inventor inventor in inventors)
						{
							if (inventor.Code == currentInventor.Code) continue;
							if (sourceInventorCodes.Contains(inventor.Code) == true)
							{
								FileItemViewModel item = NewFileItemViewModel(rootFolder, inventor);
								item.IsSource = true;
								item.IsDestination = false;
								item.IsCheckedEnabled = true;
								this._items.Add(item);
							}
						}
					}

					if (this.ShowDestination == true)
					{
						foreach (Inventor inventor in inventors)
						{
							if (inventor.Code == currentInventor.Code) continue;
							if (sourceInventorCodes.Contains(inventor.Manager) == true)
							{
								if (inventor.Manager.ToUpper() != inventor.Code.ToUpper())
								{
									FileItemViewModel item = NewFileItemViewModel(rootFolder, inventor);
									item.IsSource = false;
									item.IsDestination = true;
									item.IsCheckedEnabled = true;
									this._items.Add(item);
								}
							}
						}
					}
				}
				catch { }
			}

			else
			{
				foreach (Inventor inventor in inventors)
				{
					if (inventor.Code == currentInventor.Code) continue;
					try
					{
						FileItemViewModel item = NewFileItemViewModel(rootFolder, inventor);
						item.IsCheckedEnabled = true;
						this._items.Add(item);
					}
					catch { }
				}
			}
			//======================= TEST
			//	CreateTempInventor(inventorRepository);
			////=======================

			if (base.RaiseCanImport != null)
				base.RaiseCanImport();
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


		private FileItemViewModel CreateTempCloneInventor(FileItemViewModel item)
		{
			if (item == null) return null;
			IInventorRepository inventorRepository = base.ServiceLocator.GetInstance<IInventorRepository>();
			IAuditConfigRepository auditConfigRepository = base.ServiceLocator.GetInstance<IAuditConfigRepository>();
			AuditConfigs auditConfigs = new AuditConfigs();

		
			Inventor oldInvetor = item.InventorObject;	   //Выбор инвентора с которого копируем все
			if (oldInvetor == null) return null;
				

			AuditConfig mainAC = base.GetMainAuditConfig();  //currentAuditConfig
			if (mainAC == null) return null;

			string newInventorCode = AddLevel(oldInvetor.Code); 

			// проверить есть ли в этом бранче инветнор с  inventor.Code + "_Temp";			TODO
			// удалить  если есть  инветор TODO
			inventorRepository.Delete(newInventorCode);
			// удалить  если есть AuditConfig	   c Таким 	 inventor.Code + "_Temp"		  TODO
			auditConfigRepository.DeleteByInventorCode(newInventorCode, CBIContext.History);

			// И создаем снова	 AuditConfig
			AuditConfig cloneMainAC = auditConfigRepository.Clone(mainAC);			  // создаем копию с AyditConfig
			if (cloneMainAC == null) return null;

			cloneMainAC.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();
			cloneMainAC.Description = AddLevel(oldInvetor.Description);//oldInvetor.Description + "_A";
			cloneMainAC.InventorCode = newInventorCode;
			cloneMainAC.InventorName = oldInvetor.Name;
			cloneMainAC.InventorDate = oldInvetor.InventorDate;
			cloneMainAC.DBPath = AddLevel(oldInvetor.DBPath); //oldInvetor.DBPath + "_A";
			cloneMainAC.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();
			cloneMainAC.StatusInventorCode = StatusInventorEnum.Temp.ToString();

			//inventor.DBPath = auditConfig.DBPath;
			auditConfigs.Add(cloneMainAC);
			auditConfigRepository.Insert(auditConfigs);
			string parentInventorCode = oldInvetor.Manager;
			if (string.IsNullOrWhiteSpace(parentInventorCode) == true) parentInventorCode = oldInvetor.Code;

			Inventor newInvetor = oldInvetor.Clone(newInventorCode, parentInventorCode);
			if (newInvetor == null) return null;
			newInvetor.Manager = parentInventorCode;

			newInvetor.Description = AddLevel(oldInvetor.Description); //oldInvetor.Description + "_A";
			//base.ContextCBIRepository.CreateContextInventor(newInvetor, cloneMainAC, true, oldInvetor); все перенесла сюда

			string inheritFromDBPath = String.Empty;
			if (oldInvetor != null)
			{
				inheritFromDBPath = base.ContextCBIRepository.BuildRelativeDbPath(oldInvetor);
				//  string fullPath = BuildFullDbPath(domainObject);
			}

			inventorRepository.Insert(newInvetor, inheritFromDBPath);	//!!! Это копирование DB как раз
	
			base.ContextCBIRepository.GetImportFolderPath(newInvetor);		 //создание папки импорта личной


			IDBSettings dbSettings = base.ServiceLocator.GetInstance<IDBSettings>();
			string rootFolder = dbSettings.BuildCount4UDBFolderPath();
			FileItemViewModel cloneItem = NewFileItemViewModel(rootFolder, newInvetor);
			cloneItem.IsCheckedEnabled = false;
			return cloneItem;
		}

		//public bool IsDbInventories
		//{
		//	get { return this._isDbInventories; }
		//	set
		//	{
		//		this._isDbInventories = value;
		//		this.RaisePropertyChanged(() => this.IsDbInventories);

		//		this._isDbFile = !this._isDbInventories;
		//		this.RaisePropertyChanged(() => this.IsDbFile);

		//		if (_pathFileWatcher != null)
		//		{
		//			_pathFileWatcher.IsFile = _isDbFile;
		//		}

		//		this.Build();
		//		this.RaisePropertyChanged(() => this.Items);

		//		this.RaisePropertyChanged(() => this.Path);

		//		if (base.RaiseCanImport != null)
		//			base.RaiseCanImport();

		//		base.OpenCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//public bool IsDbFile
		//{
		//	get { return this._isDbFile; }
		//	set
		//	{
		//		this._isDbFile = value;
		//		this.RaisePropertyChanged(() => this.IsDbFile);

		//		this._isDbInventories = !this._isDbFile;
		//		this.RaisePropertyChanged(() => this.IsDbInventories);

		//		if (value == true)
		//		{
		//			this._items.Clear();
		//			this.RaisePropertyChanged(() => this.Items);

		//		}

		//		if (_pathFileWatcher != null)
		//		{
		//			this._pathFileWatcher.IsFile = _isDbFile;
		//		}

		//		this.RaisePropertyChanged(() => this.Path);

		//		if (base.RaiseCanImport != null)
		//			base.RaiseCanImport();

		//		//if (base.InputFileFolderChanged != null)
		//		//	base.InputFileFolderChanged(_isDirectory);

		//		base.OpenCommand.RaiseCanExecuteChanged();
		//	}
		//}

		public string AddLevel( string fromCode) 
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
				if (base.RaiseCanImport != null)
					base.RaiseCanImport();
			}
		}

		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
						this.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						//if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

        protected override void RunImport(ImportCommandInfo info)
        {
            this.Import();
		}

        protected override void RunClear()
        {
            this.Clear();
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return _yesNoRequest; }
        }

        public List<string> NewDocumentCodeList
        {
            get
			{
				ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
				List<string> newDocumentCodeList = sessionRepository.GetDocumentHeaderCodeList(this._newSessionCodeList, base.GetDbPath);

				return newDocumentCodeList; 
			}
        }

		public List<string> NewSessionCodeList
		{
			get { return this._newSessionCodeList; }
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
		//	this._pathFileWatcher = new AdapterFileWatcher(this, TypedReflection<ImportPdaCount4UdbSdfAdapterViewModel>.GetPropertyInfo(r => r.Path), this._isDbFile);

          //  this._newDocumentCodeList.Clear();
			this._newSessionCodeList.Clear();

			observCountingChecked = Observable.Timer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1)).Select(x => x);
			disposeObservCountingChecked = observCountingChecked.Subscribe(CountingChecked);
        }


		public void CountingChecked(long x)
		{

			if (base.RaiseCanImport != null)
				base.RaiseCanImport();
		}

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
			if (disposeObservCountingChecked != null) disposeObservCountingChecked.Dispose();
			//this._pathFileWatcher.Clear();
        }

        protected override void ProcessImportInfo(ImportCommandInfo info)
        {
            ImportFromPdaCommandInfo pdaInfo = info as ImportFromPdaCommandInfo;
            if (pdaInfo != null)
            {
                this._report = pdaInfo.Report as Count4U.GenerationReport.Report;
                this._isAutoPrint = pdaInfo.IsAutoPrint;
                this._isContinueGrabFiles = pdaInfo.IsContinueGrabFiles;
            }

			_logger.Info("OnNavigatedFrom  + Clear");
        }

        #region Implementation of IImportAdapter

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
            //init GUI
            this._fileName = FileSystem.inData;
			this.PathFilter = "*.sdf|*.sdf|All files (*.*)|*.*";
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;
            StepTotal = 1;
            Session = 0;
			this._withSerialNumber = true;
			this._byMakat = false;
			this._showSource = true;
			this._showDestination = true;
			
        }

		public bool WithSerialNumber
		{
			get { return _withSerialNumber; }
			set
			{
				this._withSerialNumber = value;

				RaisePropertyChanged(() => WithSerialNumber);
			}
		}

		public bool ShowSource
		{
			get { return _showSource; }
			set
			{
				this._showSource = value;

				RaisePropertyChanged(() => ShowSource);
				this.Build();
			}
		}


		public bool ShowDestination
		{
			get { return _showDestination; }
			set
			{
				this._showDestination = value;

				RaisePropertyChanged(() => ShowDestination);
				this.Build();
			}
		}

		

		public bool ByMakat
		{
			get { return _byMakat; }
			set
			{
				this._byMakat = value;

				RaisePropertyChanged(() => ByMakat);
			}
		}

        public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName) + @"\Count4UDB.sdf";
			this.Build();
			this.RaisePropertyChanged(() => this.Items);
	
        }

		public override void Import()																				//ImportIturFromDBBlukProvider
		{
			//ClearAll();
			string sourceDbPath = base.GetDbPath;

			//ImportIturFromDBProvider
			string newSessionCode = Guid.NewGuid().ToString();
			this._newSessionCodeList.Add(newSessionCode);
			base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("NewSessionCode : {0}", newSessionCode));

			IMakatRepository makatRepository = base.ServiceLocator.GetInstance<IMakatRepository>();
			makatRepository.ProductMakatDictionaryRefill(base.GetDbPath, true);

			string currentInventorCode = "";
			if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;
			List<FileItemViewModel> itemsCheck1 = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();
			if (itemsCheck1.Count() != 2) return;
			if (currentInventorCode == "") return;
			 //----------------------------------------------------------------------- создаем клоны инвенторов с расширением Temp
			List<FileItemViewModel> itemsCheck = new List<FileItemViewModel>();
				foreach (FileItemViewModel item in itemsCheck1)
				{
					FileItemViewModel cloneItem = CreateTempCloneInventor(item);
					if (cloneItem != null)
					{
						itemsCheck.Add(cloneItem);
					}
					else
					{
						itemsCheck.Add(item);
					}
				}

				 //-----------------------------------------------------------------------
				   
			IImportProvider providerLocation = this.GetProviderInstance(ImportProviderEnum.ImportLocationFromDBADOProvider);
			providerLocation.ProviderEncoding = base.Encoding;

			IImportProvider providerItur = this.GetProviderInstance(ImportProviderEnum.ImportIturFromDBBlukProvider);
	        providerItur.ProviderEncoding = base.Encoding;

			IImportProvider providerDocumentHeader = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderFromDBBlukProvider);
            providerDocumentHeader.ProviderEncoding = base.Encoding;

			IImportProvider providerInventProduct = this.GetProviderInstance(ImportProviderEnum.ImportInventProductAfterCompareFromDbBulkProvider);
			providerInventProduct.ProviderEncoding = base.Encoding;
			
                bool firstRun = true;
                base.Session = 0;
                while (true)
                {
                    if (base.CancellationToken.IsCancellationRequested)
                        break;

					if (firstRun == false)
                    {
#if DEBUG
                        break;
#else
                        if (_isContinueGrabFiles == false)
                            break;
#endif
                    }

                    base.Session++;
					int k = 4;
					base.StepTotal = itemsCheck.Count * k;
                    base.StepCurrent = 0;
			
				   //================================================================================================
					//из текущей БД (compare) в  новые БД	 - проверяем все ли есть
					foreach (FileItemViewModel item in itemsCheck)	 // это источники которые надо заполнить из компаре. Компаре является текущей
					{
						base.StepCurrent++;
						providerLocation.Parms.Clear();
						providerLocation.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						providerLocation.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						providerLocation.Parms[ImportProviderParmEnum.DBPath] = item.Path;
						providerLocation.FromPathFile = sourceDbPath;	  //Компаре является текущей и источником
						providerLocation.ToPathDB = item.Path;				 // destination
						providerLocation.Import();
					}

					//из текущей БД (compare) в  новые БД	 - проверяем все ли есть	 
					foreach (FileItemViewModel item in itemsCheck)	 // это источники которые надо заполнить из компаре. Компаре является текущей
					{
						base.StepCurrent++;
						providerItur.Parms.Clear();
						providerItur.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						providerItur.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						providerItur.Parms[ImportProviderParmEnum.DBPath] = item.Path;
						providerItur.FromPathFile = sourceDbPath;	  //Компаре является текущей
						providerItur.ToPathDB = item.Path;				  // destination
						providerItur.Import();
					}


					foreach (FileItemViewModel item in itemsCheck)
					{
						base.StepCurrent++;
						providerDocumentHeader.ToPathDB = item.Path;
						providerDocumentHeader.Clear();			

						providerDocumentHeader.Parms.Clear();
						providerDocumentHeader.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						providerDocumentHeader.Parms[ImportProviderParmEnum.Approve] = "1";
						providerDocumentHeader.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						providerDocumentHeader.Parms[ImportProviderParmEnum.DBPath] = item.Path;
						
						if (item.InventorObject != null)
						{
							providerDocumentHeader.Parms[ImportProviderParmEnum.Suffix] = item.InventorObject.Code.Replace(item.InventorObject.Manager, "");
						}

						providerDocumentHeader.FromPathFile = sourceDbPath;	  //Компаре является текущей
						providerDocumentHeader.ToPathDB = item.Path;
						providerDocumentHeader.Import();
					}

				

					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
					GC.Collect();

					foreach (FileItemViewModel item in itemsCheck)
					{
						base.StepCurrent++;
						providerInventProduct.ToPathDB = item.Path;
						providerInventProduct.Clear();

						providerInventProduct.Parms.Clear();
						providerInventProduct.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						providerInventProduct.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						providerInventProduct.Parms[ImportProviderParmEnum.DBPath] = item.Path;
						providerInventProduct.FromPathFile = sourceDbPath;	  //Компаре является текущей
						providerInventProduct.ToPathDB = item.Path;
						if (item.InventorObject != null)
						{
							providerInventProduct.Parms[ImportProviderParmEnum.Suffix] = item.InventorObject.Code.Replace(item.InventorObject.Manager, "");
						}
						providerInventProduct.Import();
						base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Import InventProduct to Path : {0}", item.Path));


						// один импорт => одна сессия для каждой копии БД
						List<string> currentSessionCodeList = new List<string>();
						currentSessionCodeList.Add(newSessionCode);
						ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
						List<string> currentDocumentCodeList = sessionRepository.Insert(currentSessionCodeList, item.Path);
						this._iturRepository.RefillApproveStatusBit(currentDocumentCodeList, currentSessionCodeList, item.Path);

						base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import : {0}", currentDocumentCodeList.Count));
						IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
					long countDocumentWithoutError = documentHeaderRepository.GetCountDocumentWithoutError(currentSessionCodeList, item.Path);
						base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import correct : {0}", countDocumentWithoutError));
					long countDocumentWithError = documentHeaderRepository.GetCountDocumentWithError(currentSessionCodeList, item.Path);
						base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import with error:  {0}", countDocumentWithError));

						currentDocumentCodeList = new List<string>();
					}

					if (base.CancellationToken.IsCancellationRequested)
						break;

                    firstRun = false;
                }
        
			

				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				GC.Collect();

				//base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format(" End DeleteAllDocumentsWithoutAnyInventProduct"));
				//IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();
				//iturRepository.RefillApproveStatusBit(base.GetDbPath);
			//  -----/
			 
			//foreach (var documentCode in currentDocumentCodeList)
			//{
			//	RunPrintReport(documentCode);
			//}

			        

            FileLogInfo fileLogInfo = new FileLogInfo();
			string pathLogFile= System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') );
		   if (!Directory.Exists(pathLogFile))	  Directory.CreateDirectory(pathLogFile);
			fileLogInfo.Directory = pathLogFile;

            base.SaveFileLog(fileLogInfo);
			Utils.RunOnUI(() => this.Build());
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			string getDbPath = base.GetDbPath;
			//ClearAll(getDbPath);
            UpdateLogFromILog();
        }

		private void ClearAll(string getDbPath)
		{
			//string getDbPath = base.GetDbPath;

			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationFromDBADOProvider);
			provider.ToPathDB = getDbPath;		//  base.GetDbPath;
			provider.Clear();

			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportIturFromDBBlukProvider);
			provider1.ToPathDB = getDbPath;		//  base.GetDbPath;
			provider1.Clear();

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderFromDBBlukProvider);
			provider2.ToPathDB = getDbPath;		//  base.GetDbPath;
			provider2.Clear();

			IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductFromDbBulkProvider);
			provider3.ToPathDB = getDbPath;		//  base.GetDbPath;
			provider3.Clear();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.Collect();

			base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "InventProduct"));
			this._iturRepository.ClearStatusBit(base.GetDbPath);
		}

        #endregion

        public void RunPrintReport(string documentCode)
        {
			//if (this._isAutoPrint == false) return;

			//if (string.IsNullOrWhiteSpace(documentCode) == true) return;
			//IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
			//DocumentHeader documentHeader = documentHeaderRepository.GetDocumentHeaderByCode(documentCode, base.GetDbPath);
			//if (documentHeader == null) return;

			//IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();
			//Itur itur = iturRepository.GetIturByDocumentCode(documentCode, base.GetDbPath);

			//Location location = null;
			//if (itur != null)
			//{
			//	ILocationRepository locationRepository = base.ServiceLocator.GetInstance<ILocationRepository>();
			//	location = locationRepository.GetLocationByCode(itur.LocationCode, base.GetDbPath);
			//}

			//try
			//{
			//	GenerateReportArgs args = new GenerateReportArgs();
			//	args.Customer = base.CurrentCustomer;
			//	args.Branch = base.CurrentBranch;
			//	args.Inventor = base.CurrentInventor;
			//	args.DbPath = base.GetDbPath;
			//	args.Report = this._report;
			//	args.Doc = documentHeader;
			//	args.ViewDomainContextType = ViewDomainContextEnum.ItursIturDoc;
			//	args.Itur = itur;
			//	args.Location = location;

			//	SelectParams spDoc = new SelectParams();
			//	List<string> searchDoc = new List<string> { documentCode };
			//	List<string> searchItur = new List<string> { documentHeader.IturCode };
			//	spDoc.FilterStringListParams.Add("DocumentCode", new FilterStringListParam()
			//	{
			//		Values = searchDoc
			//	});
			//	spDoc.FilterStringListParams.Add("IturCode", new FilterStringListParam()
			//	{
			//		Values = searchItur
			//	});
			//	args.SelectParams = spDoc;

			//	//this._generateReportRepository.RunPrintReport(args);
			//	//this._generateReportRepository.RunSaveReport(args, @"C:\Temp\testReport\output1.txt", ReportFileFormat.Excel);

			//	ImportPdaPrintQueue printQueue = _unityContainer.Resolve<ImportPdaPrintQueue>();
			//	printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
			//}
			//catch (Exception exc)
			//{
			//	_logger.ErrorException("RunPrintReport", exc);
			//}
        }
    }
}


