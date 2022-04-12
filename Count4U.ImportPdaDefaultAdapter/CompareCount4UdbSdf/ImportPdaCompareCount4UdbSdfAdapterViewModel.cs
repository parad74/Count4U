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
using Count4U.Common;
using System.Xml.Linq;
using Count4U.Common.ViewModel.Misc;
using Count4U.Common.ViewModel.Filter.Sorting;
using Microsoft.Practices.Prism.Commands;

namespace Count4U.ImportPdaCompareCount4UdbSdfAdapter
{
	public class ImportPdaCompareCount4UdbSdfAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
        private readonly IIturRepository _iturRepository;		
		 private readonly IInventorRepository _inventorRepository;
		 public string _fileName {get; set;}
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        //private  List<string> _newDocumentCodeList;
		private readonly List<string> _newSessionCodeList;

		  private readonly DelegateCommand _sortDirectionCommand;

        private bool _isAutoPrint;
        private Count4U.GenerationReport.Report _report;
        private bool _isContinueGrabFiles;
		private FileItemViewModel _selectedItem;
		private bool _withSerialNumber;
		private bool _withProperty10;
		private bool _byMakat;
		private bool _byBarcode;

	    private bool _isDesc;

		private readonly ObservableCollection<ItemFindViewModel> _sortItems;
		private ItemFindViewModel _sortItemSelected;
		
       
		
		//private AdapterFileWatcher _pathFileWatcher;

		protected readonly ObservableCollection<FileItemViewModel> _items;
		protected bool _isChecked;

		protected IObservable<long> observCountingChecked;
		protected IDisposable disposeObservCountingChecked;
		//protected bool _isDbInventories;
		//protected bool _isDbFile;

		public ImportPdaCompareCount4UdbSdfAdapterViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
			IInventorRepository inventorRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager,
            IGenerateReportRepository generateReportRepository,
            IUnityContainer unityContainer) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
			this._items = new ObservableCollection<FileItemViewModel>();
		//	this._items.Add(new FileItemViewModel { File = "test", Size = "10", Date = "1.1.1070" });

            this._unityContainer = unityContainer;
            this._generateReportRepository = generateReportRepository;
            this._iturRepository = iturRepository;
			this._inventorRepository = inventorRepository;
           // this._newDocumentCodeList = new List<string>();
			this._newSessionCodeList = new List<string>();
			this._sortItems = new ObservableCollection<ItemFindViewModel>();
			this._sortItems.Add(new ItemFindViewModel() { Value = ComboValues.SortFileGrid.Date, Text = Localization.Resources.View_UploadToPda_columnDateModify});
			this._sortItems.Add(new ItemFindViewModel() { Value = ComboValues.SortFileGrid.Description, Text = Localization.Resources.View_BranchChoose_columnDescription });

			
			//this._sortItems.Add(new ItemFindViewModel() { Value = ComboValues.SortFileGrid.DateAndTime, Text = Localization.Resources.SortFileGrid_DateAndTime });
			this._sortItems.Add(new ItemFindViewModel() { Value = ComboValues.SortFileGrid.FileName, Text = Localization.Resources.SortFileGrid_FileName });
			this._sortItemSelected = this._sortItems.FirstOrDefault();
			
			//this._isDbInventories = true;
			//this._isDbFile = false;

			 _isDesc = true;
			  _sortDirectionCommand = new DelegateCommand(SortDirectionCommandExecuted);

			base.ParmsDictionary.Clear();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
        }

		//	if (this._items.Any(r => r.IsChecked) == false) TODO
		//				return false;

		 private void SortDirectionCommandExecuted()
        {
            IsDesc = !_isDesc;
				using (new CursorWait())
				{
					this.Build();
				}
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
					this.Build();
				}
			}
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
			List<FileItemViewModel> listItem = new List<FileItemViewModel>();
			IInventorRepository inventorRepository = base.ServiceLocator.GetInstance<IInventorRepository>();
			IDBSettings dbSettings = base.ServiceLocator.GetInstance<IDBSettings>();
			string rootFolder = dbSettings.BuildCount4UDBFolderPath();

			Branch currentBranch = base.CurrentBranch;
			Inventor currentInventor = base.CurrentInventor;

			//List<string> inventorCodeList = inventorRepository.GetInventorCodeListByBranchCode(currentBranch.Code);
			Inventors inventors = inventorRepository.GetInventorsByBranchCode(currentBranch.Code);
			List<string> pathList = new List<string>();

			//string inventorDBPath = PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
			//using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(inventorDBPath)))
			//string relativeInventorPath = base.ContextCBIRepository.BuildRelativeDbPath(cbiState.CurrentInventor);
			//relativeInventorPath = Path.Combine(_dbSettings.FolderApp_Data, relativeInventorPath);

			foreach (Inventor inventor in inventors)
			{
				if (inventor.Code == currentInventor.Code)
					continue;
				string invCode = inventor.Code.ToLower();
				if (invCode.Contains("_temp") == true)
					continue;
				try
				{
					FileItemViewModel item = NewFileItemViewModel(rootFolder, inventor);
					item.IsCheckedEnabled = true;
					listItem.Add(item);
				}
				catch { }
			}
			//======================= TEST
			//	CreateTempInventor(inventorRepository);
			////=======================

			try
			{
				string selectedSortBy = _sortItemSelected == null ? String.Empty : _sortItemSelected.Value;
				if (IsDesc == true)
				{
					if (selectedSortBy == ComboValues.SortFileGrid.Date)
					{
						listItem = listItem.OrderByDescending(e => e.DateTimeCreated).Select(e => e).ToList();
					}
					else if (selectedSortBy == ComboValues.SortFileGrid.Description)
					{
						listItem = listItem.OrderByDescending(e => e.Description).Select(e => e).ToList();
					}
					else if (selectedSortBy == ComboValues.SortFileGrid.FileName)
					{
						listItem = listItem.OrderByDescending(e => e.File).Select(e => e).ToList();
					}
				}
				else
				{
					if (selectedSortBy == ComboValues.SortFileGrid.Date)
					{
						listItem = listItem.OrderBy(e => e.DateTimeCreated).Select(e => e).ToList();
					}
					else if (selectedSortBy == ComboValues.SortFileGrid.Description)
					{
						listItem = listItem.OrderBy(e => e.Description).Select(e => e).ToList();
					}
						else if (selectedSortBy == ComboValues.SortFileGrid.FileName)
					{
						listItem = listItem.OrderBy(e => e.File).Select(e => e).ToList();
					}
				}
			}
			catch { }

			this._items.Clear();
			foreach (var it in listItem)
			{
				this._items.Add(it);  //".zip"
			}

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

			string newInventorCode = oldInvetor.Code + "_Temp";

			// проверить есть ли в этом бранче инветнор с  inventor.Code + "_Temp";			TODO
			// удалить  если есть  инветор TODO
			inventorRepository.Delete(newInventorCode);
			// удалить  если есть AuditConfig	   c Таким 	 inventor.Code + "_Temp"		  TODO
			auditConfigRepository.DeleteByInventorCode(newInventorCode, CBIContext.History);

			// И создаем снова	 AuditConfig
			AuditConfig cloneMainAC = auditConfigRepository.Clone(mainAC);			  // создаем копию с AyditConfig
			if (cloneMainAC == null) return null;

			cloneMainAC.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();
			cloneMainAC.Description = oldInvetor.Description + "_Temp";
			cloneMainAC.InventorCode = newInventorCode;
			cloneMainAC.InventorName = oldInvetor.Name;
			cloneMainAC.InventorDate = oldInvetor.InventorDate;
			cloneMainAC.DBPath = oldInvetor.DBPath + "_Temp";
			cloneMainAC.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();
			cloneMainAC.StatusInventorCode = StatusInventorEnum.Temp.ToString();

			//inventor.DBPath = auditConfig.DBPath;
			auditConfigs.Add(cloneMainAC);
			auditConfigRepository.Insert(auditConfigs);

			Inventor newInvetor = oldInvetor.Clone(newInventorCode, oldInvetor.Code);
			if (newInvetor == null) return null;

			newInvetor.Description = oldInvetor.Description + "_Temp";
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
			//this._pathFileWatcher.Clear();
			if (disposeObservCountingChecked != null) disposeObservCountingChecked.Dispose();
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

		  public string AscImage
        {
            get
            {
                if (_userSettingsManager.LanguageGet() == enLanguage.Hebrew)
                    return @"/Count4U.Media;component/Icons/ascHe.png";

                return @"/Count4U.Media;component/Icons/asc.png";
            }
        }

        public string DescImage
        {
            get
            {
                if (_userSettingsManager.LanguageGet() == enLanguage.Hebrew)
                    return @"/Count4U.Media;component/Icons/descHe.png";

                return @"/Count4U.Media;component/Icons/desc.png";
            }
        }

		private enSortDirection GetSortDirection()
        {
            return _isDesc ? enSortDirection.DESC : enSortDirection.ASC;
        }

        private void SetSortDirection(enSortDirection sortDirection)
        {
            IsDesc = sortDirection == enSortDirection.DESC;
        }


		public bool IsDesc
        {
            get { return _isDesc; }
            set
            {
                _isDesc = value;
                RaisePropertyChanged(() => IsDesc);
            }
        }

		public DelegateCommand SortDirectionCommand
        {
            get { return _sortDirectionCommand; }
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
			this._withProperty10 = true;

			this._byMakat = true;
			this._byBarcode = false;
			if (base.State.CurrentCustomer != null)
			{
				if (base.State.CurrentCustomer.Tag3 == Common.Constants.Misc.MakatValue)
				{
					this.ByMakat = true;
				}
				else if (base.State.CurrentCustomer.Tag3 == Common.Constants.Misc.BarcodeValue)
				{
					this.ByBarcode = true;
				}
				else
				{
					this.ByMakat = true;
				}
			}
		                     
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

		public bool WithProperty10
		{
			get { return _withProperty10; }
			set
			{
				this._withProperty10 = value;

				RaisePropertyChanged(() => WithProperty10);
			}
		}
		

		public bool ByMakat
        {
            get { return _byMakat; }
            set
            {
                this._byMakat = value;
                RaisePropertyChanged(() => ByMakat);

                this._byBarcode = !value;
                RaisePropertyChanged(() => ByBarcode);
            }
        }

        public bool ByBarcode
        {
            get { return _byBarcode; }
            set
            {
                this._byBarcode = value;
                RaisePropertyChanged(() => ByBarcode);

                this._byMakat = !value;
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
			ClearAll();
			string sourceDBPath = "";
			//ImportIturFromDBProvider
			string newSessionCode = Guid.NewGuid().ToString();
			this._newSessionCodeList.Add(newSessionCode);
			base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("NewSessionCode : {0}", newSessionCode));

			IMakatRepository makatRepository = base.ServiceLocator.GetInstance<IMakatRepository>();
			makatRepository.ProductMakatDictionaryRefill(base.GetDbPath, true);

			string currentInventorCode = "";
			if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;
			List<FileItemViewModel> itemsCheck1 = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();
			sourceDBPath = itemsCheck1.Select(x => x.ObjectCode).ToArray().JoinRecord("|", true);

			if (itemsCheck1.Count() != 2) return;
			if (currentInventorCode == "") return;
			string getDbPath = base.GetDbPath;

			base.State.CurrentInventor.Manager = sourceDBPath.Trim('|').CutLength1(99);
			this._inventorRepository.Update(base.State.CurrentInventor);
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
			//!!!! Было по Макату - работает. Делаем по Makat & SNumber
			// InventProductUpdate2SumByIturMakatFromDBParser
			IImportProvider providerUpdate2SumByMakat = 
				this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2SumByIturMakatDbBulkProvider);
			providerUpdate2SumByMakat.ProviderEncoding = base.Encoding;
			//Делаем по Makat & SNumber
			//InventProductUpdate2SumByIturMakatSNumberFromDBParser
			IImportProvider providerUpdate2SumByMakatAndSNumber = 
				this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2SumByIturMakatSNumberDbBulkProvider);
			providerUpdate2SumByMakatAndSNumber.ProviderEncoding = base.Encoding;
			//Делаем по Makat & SNumber&PropertyStr10
			//InventProductUpdate2SumByIturMakatSNumberProp10FromDBParser
			IImportProvider providerUpdate2SumByMakatAndSNumberAndProp10 =
				this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2SumByIturMakatSNumberProp10DbBulkProvider);
			providerUpdate2SumByMakatAndSNumber.ProviderEncoding = base.Encoding;

			//Делаем по Barcode
			//InventProductUpdate2SumByIturBarcodeFromDBParser
			IImportProvider providerUpdate2SumByBarcode = 
				this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2SumByIturBarcodeDbBulkProvider);
			providerUpdate2SumByBarcode.ProviderEncoding = base.Encoding;
			//Делаем по Barcode & SNumber
			//InventProductUpdate2SumByIturBarcodeSNumberFromDBParser
			IImportProvider providerUpdate2SumByBarcodeAndSNumber = 
				this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2SumByIturBarcodeSNumberDbBulkProvider);
			providerUpdate2SumByBarcodeAndSNumber.ProviderEncoding = base.Encoding;

			// ВЕРСИЯ ПО УМОЛЧАНИЮ
			//Делаем по Barcode & SNumber & PropertyStr10
			//InventProductUpdate2SumByIturBarcodeSNumberProp10FromDBParser
			IImportProvider providerUpdate2SumByBarcodeAndSNumberAndProp10 =
				this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2SumByIturBarcodeSNumberProp10DbBulkProvider);
			providerUpdate2SumByBarcodeAndSNumber.ProviderEncoding = base.Encoding;

			//// --------------------- Заполняем Code	   - Делаем на предыдущем проходе
			////InventProductUpdateMakat2BarcodeDBParser
			//IImportProvider providerUpdateMakat2Barcode = this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdateMakat2BarcodeDbBulkProvider);
			//providerUpdateMakat2Barcode.ProviderEncoding = base.Encoding;

			////InventProductUpdate2MakatAndSNFromDBParser
			//IImportProvider providerUpdate2MakatAddSerialNumber = this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2MakatAndSNDbBulkProvider);
			//providerUpdate2MakatAddSerialNumber.ProviderEncoding = base.Encoding;

			////InventProductUpdate2BarcodeAndSNFromDBParser
			//IImportProvider providerUpdate2BarcodeAddSerialNumber = this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2BarcodeAndSNDbBulkProvider);
			//providerUpdate2BarcodeAddSerialNumber.ProviderEncoding = base.Encoding;
			////------------------------------------------------------------------------
		   
			IImportProvider providerLocation = this.GetProviderInstance(ImportProviderEnum.ImportLocationFromDBADOProvider);
			providerLocation.ProviderEncoding = base.Encoding;

			//IturFromDBParser1
			IImportProvider providerItur = this.GetProviderInstance(ImportProviderEnum.ImportIturFromDBBlukProvider);
	        providerItur.ProviderEncoding = base.Encoding;

			IImportProvider providerDocumentHeader = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderFromDBBlukProvider);
            providerDocumentHeader.ProviderEncoding = base.Encoding;

			//ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider
			//InventProductUpdateCompare2SumByIturMakatFromDBParser1 + 2
			IImportProvider providerInventProduct1 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider1);
			providerInventProduct1.ProviderEncoding = base.Encoding;
			
			IImportProvider providerInventProduct2 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider2);
			providerInventProduct2.ProviderEncoding = base.Encoding;


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

					//==========	WithSerialNumber
					if (WithSerialNumber == true)
					{
						// ========================= ByMakat And WithSerialNumber
						if (ByMakat == true)	 //Code = Makat|SerialNumber
						{
							// 1Step  - суммируем Quntety by Itur-Makat-SN. Must be in Itur one Doc	 в исходнике (Db)
							//InventProductUpdate2SumByIturMakatSNumberFromDBParser
							if (WithProperty10 == false)   //Code = Makat|SerialNumber
							{
								foreach (FileItemViewModel item in itemsCheck)
								{
									base.StepCurrent++;
									providerUpdate2SumByMakatAndSNumber.Parms.Clear();
									providerUpdate2SumByMakatAndSNumber.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
									providerUpdate2SumByMakatAndSNumber.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
									providerUpdate2SumByMakatAndSNumber.Parms[ImportProviderParmEnum.DBPath] = item.Path;
									providerUpdate2SumByMakatAndSNumber.FromPathFile = item.Path;
									providerUpdate2SumByMakatAndSNumber.ToPathDB = item.Path;
									providerUpdate2SumByMakatAndSNumber.Import();

								}

							}
							else	 //Code = Makat|SerialNumber|PropertyStr10
							{
								foreach (FileItemViewModel item in itemsCheck)
								{
									base.StepCurrent++;
									providerUpdate2SumByMakatAndSNumberAndProp10.Parms.Clear();
									providerUpdate2SumByMakatAndSNumberAndProp10.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
									providerUpdate2SumByMakatAndSNumberAndProp10.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
									providerUpdate2SumByMakatAndSNumberAndProp10.Parms[ImportProviderParmEnum.DBPath] = item.Path;
									providerUpdate2SumByMakatAndSNumberAndProp10.FromPathFile = item.Path;
									providerUpdate2SumByMakatAndSNumberAndProp10.ToPathDB = item.Path;
									providerUpdate2SumByMakatAndSNumberAndProp10.Import();

								}
							}
							if (base.CancellationToken.IsCancellationRequested)
								break;

							// 0Step  - Мeняем ключ Code = Makat|SerialNumber	 ?? контрольная проверка. Не надо 
							////InventProductUpdate2MakatAndSNFromDBParser
							//foreach (FileItemViewModel item in itemsCheck)
							//{
							//	base.StepCurrent++;
							//	providerUpdate2MakatAddSerialNumber.Parms.Clear();
							//	providerUpdate2MakatAddSerialNumber.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							//	providerUpdate2MakatAddSerialNumber.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
							//	providerUpdate2MakatAddSerialNumber.Parms[ImportProviderParmEnum.DBPath] = item.Path;
							//	providerUpdate2MakatAddSerialNumber.FromPathFile = item.Path;
							//	providerUpdate2MakatAddSerialNumber.ToPathDB = item.Path;
							//	providerUpdate2MakatAddSerialNumber.Import();
							//}

						}	// =============================//END ByMakat And WithSerialNumber

						 // ============================ByBarcode And WithSerialNumber
						else			    //Code = Barcode|SerialNumber
						{
							if (WithProperty10 == false)   //Code = Barcode|SerialNumber
							{
								foreach (FileItemViewModel item in itemsCheck)
								{
									base.StepCurrent++;
									providerUpdate2SumByBarcodeAndSNumber.Parms.Clear();
									providerUpdate2SumByBarcodeAndSNumber.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
									providerUpdate2SumByBarcodeAndSNumber.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
									providerUpdate2SumByBarcodeAndSNumber.Parms[ImportProviderParmEnum.DBPath] = item.Path;
									providerUpdate2SumByBarcodeAndSNumber.FromPathFile = item.Path;
									providerUpdate2SumByBarcodeAndSNumber.ToPathDB = item.Path;
									providerUpdate2SumByBarcodeAndSNumber.Import();

								}

							}
							else 		   //Code = Barcode|SerialNumber|PropertyStr10
							{
								foreach (FileItemViewModel item in itemsCheck)
								{
									base.StepCurrent++;
									providerUpdate2SumByBarcodeAndSNumberAndProp10.Parms.Clear();
									providerUpdate2SumByBarcodeAndSNumberAndProp10.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
									providerUpdate2SumByBarcodeAndSNumberAndProp10.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
									providerUpdate2SumByBarcodeAndSNumberAndProp10.Parms[ImportProviderParmEnum.DBPath] = item.Path;
									providerUpdate2SumByBarcodeAndSNumberAndProp10.FromPathFile = item.Path;
									providerUpdate2SumByBarcodeAndSNumberAndProp10.ToPathDB = item.Path;
									providerUpdate2SumByBarcodeAndSNumberAndProp10.Import();

								}
 
							}
							if (base.CancellationToken.IsCancellationRequested)
								break;
						}	   //END ByBarcode	 And WithSerialNumber
					}
					//==========	End WithSerialNumber

					//=========== Without SerialNumber
					else
					{
						// =========================== ============== By Makat
						if (ByMakat == true)	 //Code = Makat
						{
							// 1Step  - суммируем Quntety by Itur-Makat. Must be in Itur one Doc	 в исходнике (Db)
							//InventProductUpdate2SumByIturMakatFromDBParser
							foreach (FileItemViewModel item in itemsCheck)
							{
								base.StepCurrent++;
								providerUpdate2SumByMakat.Parms.Clear();
								providerUpdate2SumByMakat.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
								providerUpdate2SumByMakat.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
								providerUpdate2SumByMakat.Parms[ImportProviderParmEnum.DBPath] = item.Path;
								providerUpdate2SumByMakat.FromPathFile = item.Path;
								providerUpdate2SumByMakat.ToPathDB = item.Path;
								providerUpdate2SumByMakat.Import();

							}
							if (base.CancellationToken.IsCancellationRequested)
								break;

							// 0Step  - Мeняем ключ Code = Makat			 ?? контрольная проверка. Не надо 
							//InventProductUpdateMakat2BarcodeDBParser
							//foreach (FileItemViewModel item in itemsCheck)
							//{
							//	base.StepCurrent++;
							//	providerUpdateMakat2Barcode.Parms.Clear();
							//	providerUpdateMakat2Barcode.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							//	providerUpdateMakat2Barcode.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
							//	providerUpdateMakat2Barcode.Parms[ImportProviderParmEnum.DBPath] = item.Path;
							//	providerUpdateMakat2Barcode.FromPathFile = item.Path;
							//	providerUpdateMakat2Barcode.ToPathDB = item.Path;
							//	providerUpdateMakat2Barcode.Import();
							//}
							//if (base.CancellationToken.IsCancellationRequested)
							//	break;
						}
						else   // ========================================= By Barcode  ================
						{
							//Code = Barcode
							foreach (FileItemViewModel item in itemsCheck)
							{
								base.StepCurrent++;
								providerUpdate2SumByBarcode.Parms.Clear();
								providerUpdate2SumByBarcode.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
								providerUpdate2SumByBarcode.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
								providerUpdate2SumByBarcode.Parms[ImportProviderParmEnum.DBPath] = item.Path;
								providerUpdate2SumByBarcode.FromPathFile = item.Path;
								providerUpdate2SumByBarcode.ToPathDB = item.Path;
								providerUpdate2SumByBarcode.Import();

							}
							if (base.CancellationToken.IsCancellationRequested)
								break;
						}
					}
					//=========== End Without SerialNumber

					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
					GC.Collect();

				   //================================================================================================
					 //STEP 2
					//из 2 источников  в текущую БД	  //getDbPath
					foreach (FileItemViewModel item in itemsCheck)
					{
						base.StepCurrent++;
						providerLocation.Parms.Clear();
						providerLocation.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						providerLocation.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						providerLocation.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
						providerLocation.FromPathFile = item.Path;
						providerLocation.ToPathDB = getDbPath;		//  base.GetDbPath;
						providerLocation.Import();
					}

					//из 2 источников  в текущую БД	  //getDbPath
					foreach (FileItemViewModel item in itemsCheck)
					{
						base.StepCurrent++;
						providerItur.Parms.Clear();
						providerItur.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						providerItur.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						providerItur.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
						providerItur.FromPathFile = item.Path;
						providerItur.ToPathDB = getDbPath;		//  base.GetDbPath;
						providerItur.Import();
					}

					//из 2 источников  в текущую БД	  //getDbPath
					//foreach (FileItemViewModel item in itemsCheck)
					//{
					//	base.StepCurrent++;
					//	providerDocumentHeader.Parms.Clear();
					//	providerDocumentHeader.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					//	providerDocumentHeader.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
					//	providerDocumentHeader.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
					//	providerDocumentHeader.FromPathFile = item.Path;
					//	providerDocumentHeader.ToPathDB = getDbPath;		//  base.GetDbPath;
					//	providerDocumentHeader.Import();
					//}

					//очистим-удалим все DocumentHeaders в БД куда все сливаем (текущую)
					providerInventProduct1.ToPathDB = getDbPath;
					providerInventProduct1.Clear();

					providerDocumentHeader.ToPathDB = getDbPath;		//  base.GetDbPath;
					providerDocumentHeader.Clear();			   // очистка 

					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
					GC.Collect();

					////из 2 источников  в текущую БД	  //getDbPath
					// только 2 !!!

					if (itemsCheck.Count == 2)
					{
						
						{
							FileItemViewModel item1 = itemsCheck[0];		  //1 file
							FileItemViewModel item2 = itemsCheck[1];		   //2 file
							base.StepCurrent++;
							providerInventProduct1.Parms.Clear();
							providerInventProduct1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							providerInventProduct1.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
							providerInventProduct1.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;	   текущую БД
							providerInventProduct1.Parms[ImportProviderParmEnum.FileName1] = item1.Path;
							providerInventProduct1.Parms[ImportProviderParmEnum.FileName2] = item2.Path;
							providerInventProduct1.FromPathFile = item1.Path;		//из первой БД
							providerInventProduct1.ToPathDB = getDbPath;		//  base.GetDbPath;		  текущую БД
							providerInventProduct1.Parms[ImportProviderParmEnum.FileName4] = item1.Description;		 //descriptionInv1
							providerInventProduct1.Parms[ImportProviderParmEnum.FileName5] = item2.Description;		  //descriptionInv2
					
							providerInventProduct1.Import();

							base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Path : {0}", item1.File));
						}

						{
							FileItemViewModel item2 = itemsCheck[1];		  //2 file
							FileItemViewModel item1 = itemsCheck[0];		// 1 file
							base.StepCurrent++;
							providerInventProduct2.Parms.Clear();
							providerInventProduct2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							providerInventProduct2.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
							providerInventProduct2.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;	    текущую БД
							providerInventProduct2.Parms[ImportProviderParmEnum.FileName1] = item1.Path;
							providerInventProduct2.Parms[ImportProviderParmEnum.FileName2] = item2.Path;
							providerInventProduct2.FromPathFile = item2.Path;	   //из второй БД
							providerInventProduct2.ToPathDB = getDbPath;		//  base.GetDbPath;				 текущую БД
							providerInventProduct2.Parms[ImportProviderParmEnum.FileName4] = item1.Description;		 //descriptionInv1
							providerInventProduct2.Parms[ImportProviderParmEnum.FileName5] = item2.Description;		  //descriptionInv2

							providerInventProduct2.Import();
				

							base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Path : {0}", item2.File));
						}
					}

					if (base.CancellationToken.IsCancellationRequested)
						break;

                    firstRun = false;
                }
        
			// один импорт => одна сессия
			//List<string> currentSessionCodeList = new List<string>();
			//currentSessionCodeList.Add(newSessionCode);
			//ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
			//List<string> currentDocumentCodeList = sessionRepository.Insert(currentSessionCodeList, base.GetDbPath);

			//   -----		   вернуть
				IDocumentHeaderRepository docRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
				docRepository.DeleteAllDocumentsWithoutAnyInventProduct(base.GetDbPath);

				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				GC.Collect();

				base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format(" End DeleteAllDocumentsWithoutAnyInventProduct"));
				//IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();
				//iturRepository.RefillApproveStatusBit(base.GetDbPath);
			//  -----/
			 
			//foreach (var documentCode in currentDocumentCodeList)
			//{
			//	RunPrintReport(documentCode);
			//}

			//base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import : {0}", currentDocumentCodeList.Count));
			//IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
			//int countDocumentWithoutError = documentHeaderRepository.GetCountDocumentWithoutError(currentDocumentCodeList, base.GetDbPath);
			//base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import correct : {0}", countDocumentWithoutError));
			//int countDocumentWithError = documentHeaderRepository.GetCountDocumentWithError(currentDocumentCodeList, base.GetDbPath);
			//base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import with error:  {0}", countDocumentWithError));
         

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
			ClearAll();
            UpdateLogFromILog();
        }

		private void ClearAll()
		{
			string getDbPath = base.GetDbPath;

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


