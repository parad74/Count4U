using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Events.Configuration;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Misc;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.ProgressItem;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Commands;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Count4U.Model.Count4U;
using Microsoft.Practices.Unity;
using NLog;
using Type = System.Type;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Media;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Common.Extensions;
using System.Threading;
using System.IO;
using System.Reactive.Linq;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Filter.FilterTemplate;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Modules.Audit.ViewModels.Export;
using Count4U.Common.ViewModel.Adapters.Abstract;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model.Common;

namespace Count4U.Modules.Audit.ViewModels
{
	public class IturListDetailsViewModel : CBIContextBaseViewModel, IRegionMemberLifetime
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region constants

        #endregion

        #region fields
		public readonly IServiceLocator _serviceLocator;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IIturRepository _iturRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IStatusIturGroupRepository _statusGroupRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IInventProductRepository _inventProductRepository;
        private readonly ReportButtonViewModel _reportButtonViewModel;
        private readonly IUnityContainer _unityContainer;
        private readonly UICommandRepository _commandRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly IStatusIturGroupRepository _statusIturGroupRepository;
        private readonly ISessionRepository _sessionRepository;
		private readonly IReportIniRepository _reportIniRepository;
		private readonly IIniFileParser _iniFileParser;
		private readonly IReportRepository _reportRepository;
		private readonly IGenerateReportRepository _generateReportRepository;
		private readonly IDBSettings _dbSettings;
		private readonly IDocumentHeaderRepository _documentHeaderRepository;
		private readonly IImportIturBlukRepository _importIturBlukRepository;

		private IturDashboardItemViewModel _selectedItem;
		private ObservableCollection<IturDashboardItemViewModel> _items;			 //readonly
        private IList<IturDashboardItemViewModel> _selectedItems;
        private ICollectionView _itemsView;

        //  private readonly ObservableCollection<ItemFindViewModel> _sortByItems;
        private readonly ObservableCollection<ItemFindVisibilityViewModel> _groupByItems;
        //        private ItemFindViewModel _sortBySelectedItem;
        private ItemFindViewModel _groupBySelectedItem;

        private long _itemsTotal;
        private int _pageSize;
        private int _pageCurrent;
        private int _lastSessionCountItem;
		private int _lastSessionCountDocument;
        private double _lastSessionSumQuantityEdit;
		private int _quantityFilesInSourceFolder;
		private int _quantityFilesInMISFolder;
		private int _quantityFilesInUnsureFolder;
		private string _adapterFromPDA;

        private readonly DelegateCommand _dashboardCommand;

        private readonly UICommand _searchCommand;
        private readonly UICommand _reportCommand;
        private readonly UICommand _reportsCommand;
        private readonly UICommand _iturimAddEditCommand;
        private readonly UICommand _importFromPdaCommand;
		private readonly UICommand _devicePDAStatisticCommand;
		private readonly UICommand _deviceWorkerPDAStatisticCommand;
		private readonly UICommand _importFromPdaAutoCommand;
        private readonly UICommand _refreshCommand;
        private readonly UICommand _printReportCommand;
		private readonly UICommand _setDissableItursCommand;
		
		private readonly UICommand _printReportByLocationCodeCommand;
		private readonly UICommand _printReportByTagCommand;
		private readonly UICommand _eddEditLocatonItursCommand;
		private readonly UICommand _planogramCommand;
        private readonly UICommand _pdaCommand;
        private readonly UICommand _erpCommand;
        private readonly UICommand _changeStatusCommand;
        private readonly UICommand _updateCommand;

		private readonly DelegateCommand _deleteNoneCatalogCommand;

        private readonly DelegateCommand _changeLocationCommand;
		private readonly DelegateCommand _changeIturNameCommand;

		private readonly DelegateCommand<string> _iturListPrintByIndexCommand;
		private readonly DelegateCommand<string> _iturListExportErpByConfigCommand;
		protected UICommandRepository<IExportErpModuleInfo> _commandRepositoryExportErpModuleInfoObject;
		protected DelegateCommand _runExportErpByConfigCommand;

		//private readonly DelegateCommand _iturListPrintCommand;
		//private readonly DelegateCommand _iturListPrintIS0155Command;
		//private readonly DelegateCommand _iturListPrintIS0160Command;

	    private bool _isGrouping;
        private bool _isGroupingLocation;
        private bool _isGroupingStatus;
		private bool _isGroupingTag;
        private Locations _allLocations;
        private Dictionary<string, StatusIturGroup> _allStatusGroups;

        private bool? _isDisabled;

        private bool _isBusy;
        private string _busyText;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

		private readonly DelegateCommand _busyCancelCommand;
		private CancellationTokenSource _cancellationTokenSource;

        private IturAdvancedFilterData _filter;

        private bool _isNavigatedFirstTime;

        private readonly ObservableCollection<ItemFindViewModel> _modeItems;
        private ItemFindViewModel _modeSelectedItem;

        private readonly ObservableCollection<ItemFindViewModel> _locationItems;
        private ItemFindViewModel _locationSelected;

		private readonly ObservableCollection<ItemFindViewModel> _statusItems;
		private ItemFindViewModel _statusSelected;

		private readonly ObservableCollection<ItemFindViewModel> _tagItems;
		private ItemFindViewModel _tagSelected;

		private readonly ObservableCollection<FilterTemplateItemViewModel> _templateItems;
		private FilterTemplateItemViewModel _templateSelected;

		private readonly IFilterTemplateRepository _filterTemplateRepository;

        private bool _pagingControlVisible;
		private bool _templateVisible;

        private bool _locationList3Visible;
		private bool _tagList3Visible;
		private bool _statusList3Visible;

		private bool _statusList4Visible;
		private bool _tagList4Visible;
		private bool _locationList4Visible;
		

		

        private readonly DelegateCommand _upCommand;
        private readonly DelegateCommand _downCommand;

        private readonly ObservableCollection<ProgressItemViewModel> _progressItems;
        private double _progressDone;
        private string _progressDoneString;
        private string _progressDoneTotalItemsString;

		public string _reportIniFile;

		private string _misCommunicatorPath = "";
		private string _misiDnextDataPath = "";

		//private int _processLisner;
		protected string _sourcePath;
		protected string _misPath;
		protected string _misUnsurePath;
		private IObservable<long> observCountingFiles;
		private IDisposable disposeObservCountingFiles;
		
        #endregion

        public IturListDetailsViewModel(
            IServiceLocator serviceLocator,
            IIturRepository iturRepository,
			 IDocumentHeaderRepository documentHeaderRepository,
            IEventAggregator eventAggregator,
            ILocationRepository locationRepository,
            IContextCBIRepository contextCBIRepository,
            IUserSettingsManager userSettingsManager,
            IStatusIturGroupRepository statusGroupRepository,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            IInventProductRepository inventProductRepository,
            ReportButtonViewModel reportButtonViewModel,
            IUnityContainer unityContainer,
            UICommandRepository uiCommandRepository,
			UICommandRepository<IExportErpModuleInfo> commandRepositoryExportErpModuleInfoObject,
            ModalWindowLauncher modalWindowLauncher,
            IStatusIturGroupRepository statusIturGroupRepository,
            ISessionRepository sessionRepository,
			IReportRepository reportRepository,
			IGenerateReportRepository generateReportRepository,
			IReportIniRepository reportIniRepository,
			 IIniFileParser iniFileParser,
			IFilterTemplateRepository filterTemplateRepository,
			IImportIturBlukRepository importIturBlukRepository,
			 IDBSettings dbSettings
            )
            : base(contextCBIRepository)
        {
            this._statusIturGroupRepository = statusIturGroupRepository;
			this._generateReportRepository = generateReportRepository;
			this._documentHeaderRepository = documentHeaderRepository;
			this._importIturBlukRepository = importIturBlukRepository;
			this._reportRepository = reportRepository;
			this._reportIniRepository = reportIniRepository;
			this._iniFileParser = iniFileParser;
            this._modalWindowLauncher = modalWindowLauncher;
            this._serviceLocator = serviceLocator;
            this._commandRepository = uiCommandRepository;
			this._commandRepositoryExportErpModuleInfoObject = commandRepositoryExportErpModuleInfoObject;
            this._unityContainer = unityContainer;
            this._reportButtonViewModel = reportButtonViewModel;
            this._inventProductRepository = inventProductRepository;
            this._navigationRepository = navigationRepository;
            this._statusGroupRepository = statusGroupRepository;
            this._regionManager = regionManager;
            this._userSettingsManager = userSettingsManager;
            this._locationRepository = locationRepository;
            this._eventAggregator = eventAggregator;
            this._iturRepository = iturRepository;
            this._sessionRepository = sessionRepository;
			this._dbSettings = dbSettings;
			this._filterTemplateRepository = filterTemplateRepository;
			

            this._searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });
            this._reportCommand = _commandRepository.Build(enUICommand.Report, delegate { });
            this._iturimAddEditCommand = _commandRepository.Build(enUICommand.IturAddEdit, this.IturimAddEditCommandExecuted);
            this._importFromPdaCommand = _commandRepository.Build(enUICommand.ImportFromPda, this.ImportFromPdaCommandExeucted);
			this._devicePDAStatisticCommand = _commandRepository.Build(enUICommand.DevicePDAStatistic, this.DevicePDAStatisticCommandExeucted);
			this._deviceWorkerPDAStatisticCommand = _commandRepository.Build(enUICommand.DeviceWorkerPDAStatistic, this.DeviceWorkerPDAStatisticCommandExeucted);

			this._importFromPdaAutoCommand = _commandRepository.Build(enUICommand.ImportFromPdaAuto, this.ImportFromPdaAutoCommandExeucted);
            this._refreshCommand = _commandRepository.Build(enUICommand.Refresh, this.RefreshCommandExecuted);
            this._changeLocationCommand = new DelegateCommand(ChangeLocationCommandExecuted, ChangeLocationCommandCanExecuted);
			this._changeIturNameCommand = new DelegateCommand(ChangeIturNameCommandExecuted);
            this._printReportCommand = _commandRepository.Build(enUICommand.PrintReport, PrintReportCommandExecuted);
			this._setDissableItursCommand = _commandRepository.Build(enUICommand.DissableIturs, SetDissableItursCommandExecuted);
			this._printReportByLocationCodeCommand = _commandRepository.Build(enUICommand.PrintReportByLocationCode, PrintReportByLocationCodeCommandExecuted);
			this._eddEditLocatonItursCommand = _commandRepository.Build(enUICommand.AddEditLocationItur, AddEditLocationIturCommandExecuted);

			this._printReportByTagCommand = _commandRepository.Build(enUICommand.PrintReportByTag, PrintReportByTagCommandExecuted);
			this._deleteNoneCatalogCommand = uiCommandRepository.Build(enUICommand.DeleteNoneCatalogMain, DeleteNoneCatalogCommandExecuted, DeleteNoneCatalogCommandCanExecute);

			//old
			//this._iturListPrintCommand = new DelegateCommand(IturListPrintCommandExecuted);
			//this._iturListPrintIS0155Command = new DelegateCommand(IturListPrintIS0155CommandExecuted);
			//this._iturListPrintIS0160Command = new DelegateCommand(IturListPrintIS0160CommandExecuted);
			this._iturListPrintByIndexCommand = new DelegateCommand<string>(IturListPrintByIndexCommandExecuted);
			this._iturListExportErpByConfigCommand = new DelegateCommand<string>(IturListExportErpByConfigCommandExecuted);
			//View_IturAddEditDelete_tpExportERPByItur
			this._runExportErpByConfigCommand =  uiCommandRepository.Build(enUICommand.ExportERP, RunExportErpByConfigCommandExecuted, RunExportErpByConfigCommandCanExecute);


			this._planogramCommand = _commandRepository.Build(enUICommand.Planogram, PlanogramCommandExecuted);
            this._pdaCommand = _commandRepository.Build(enUICommand.ExportPDAMain, PdaCommandExecuted);
            this._erpCommand = _commandRepository.Build(enUICommand.ExportERPMain, ErpCommandExecuted);
            this._changeStatusCommand = _commandRepository.Build(enUICommand.ChangeStatus, ChangeStatusCommandExecuted);
            this._updateCommand = _commandRepository.Build(enUICommand.UpdateMain, UpdateCommandExecuted);

			this._busyCancelCommand = new DelegateCommand(BusyCancelCommandExecuted);
			
            this._items = new ObservableCollection<IturDashboardItemViewModel>();

            this._dashboardCommand = new DelegateCommand(this.DashboardCommandExecuted);
            this._reportsCommand = _commandRepository.Build(enUICommand.Report, ReportsCommandExecuted);

            this._upCommand = new DelegateCommand(UpCommandExecuted);
            this._downCommand = new DelegateCommand(DownCommandExecuted);

            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._groupByItems = new ObservableCollection<ItemFindVisibilityViewModel>()
                    {
                        new ItemFindVisibilityViewModel() {Text = ComboValues.GroupItur.EmptyText, Value = ComboValues.GroupItur.EmptyValue, IsVisible = true},
                        new ItemFindVisibilityViewModel() {Text = ComboValues.GroupItur.LocationText, Value = ComboValues.GroupItur.LocationValue, IsVisible = true},
                        new ItemFindVisibilityViewModel() {Text = ComboValues.GroupItur.StatusText, Value = ComboValues.GroupItur.StatusValue, IsVisible = true},
						new ItemFindVisibilityViewModel() {Text = ComboValues.GroupItur.TagText, Value = ComboValues.GroupItur.TagValue, IsVisible = true},
			//			new ItemFindVisibilityViewModel() {Text = ComboValues.GroupItur.StatusNotEmptyText, Value = ComboValues.GroupItur.StatusNotEmptyValue, IsVisible = true},

                    };

            _modeItems = new ObservableCollection<ItemFindViewModel>()
                {
                    new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModePagedText, Value = ComboValues.IturListDetailsMode.ModePaged},
                    new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModeLocationText, Value = ComboValues.IturListDetailsMode.ModeLocation},
					new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModeTagText, Value = ComboValues.IturListDetailsMode.ModeTag},
					new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModeStatusText, Value = ComboValues.IturListDetailsMode.ModeStatus},
			
                };


			_modeSelectedItem = _modeItems.FirstOrDefault();
            _pagingControlVisible = true;
			_templateVisible = true;

            _locationList3Visible = false;
			_tagList3Visible = false;
			_statusList3Visible = false;

			_statusList4Visible = false;
			_tagList4Visible = false;
			_locationList4Visible = false;

            _locationItems = new ObservableCollection<ItemFindViewModel>();
			_statusItems = new ObservableCollection<ItemFindViewModel>();
			_tagItems = new ObservableCollection<ItemFindViewModel>();
			_templateItems = new ObservableCollection<FilterTemplateItemViewModel>();
			
            _isNavigatedFirstTime = true;

			_cancellationTokenSource = new CancellationTokenSource();
            _progressItems = new ObservableCollection<ProgressItemViewModel>();
            //GetSessionWithMaxDateCreated
            //can't unsubscribe in OnNavigatedFrom cause this have to be fired when VM is inactive
            this._eventAggregator.GetEvent<GroupConfigurationChangedEvent>().Subscribe(GroupConfChanged);
            this._eventAggregator.GetEvent<SortConfigurationChangedEvent>().Subscribe(SortConfChanged);

            this._pageCurrent = 1;
        }

		private bool ChangeLocationCommandCanExecuted()
		{
           string buildRegionName =String.Format("{0}_{1}", Common.RegionNames.ModalWindowRegion, "IturLocationChangeView");
		   if (this._regionManager.Regions.ContainsRegionWithName(buildRegionName) == true) return false;
		   return true;
		}

        #region properties

		
		public DelegateCommand BusyCancelCommand
		{
			get { return this._busyCancelCommand; }
		}

       	private void BusyCancelCommandExecuted()
		{	
			this._cancellationTokenSource.Cancel();
            ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
            printQueue.Stop();
			IsBusy = false;
			
		}
        public DelegateCommand DashboardCommand
        {
            get { return this._dashboardCommand; }
        }

        public ObservableCollection<IturDashboardItemViewModel> Items
        {
            get { return this._items; }
			set
			{
				this._items = value;
				this.RaisePropertyChanged(() => this.Items);
			}
        }

        public ICollectionView ItemsView
        {
            get
            {
                if (this._itemsView == null)
                {
                    this._itemsView = CollectionViewSource.GetDefaultView(this.Items);
                }
                return this._itemsView;
            }
        }


        public IturDashboardItemViewModel SelectedItem //double click on itur (or enter)
        {
            get { return this._selectedItem; }
            set
            {
                this._selectedItem = value;
                this.RaisePropertyChanged(() => this.SelectedItem);

                if (this._selectedItem != null)
                {
                    UriQuery query = new UriQuery();
                    Utils.AddContextToQuery(query, base.Context);
                    Utils.AddDbContextToQuery(query, base.CBIDbContext);
                    query.Add(NavigationSettings.IturCode, this._selectedItem.Itur.IturCode);
                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

                    SelectParams selectParams = BuildSelectParams();
                    UtilsConvert.AddObjectToQuery(query, this._navigationRepository, selectParams, NavigationObjects.IturSelectParams);

                    UtilsNavigate.InventProductDetailsOpen(this._regionManager, query);
                }
            }
        }

        public ObservableCollection<ItemFindVisibilityViewModel> GroupByItems
        {
            get { return _groupByItems; }
        }

        public ItemFindViewModel GroupBySelectedItem
        {
            get { return this._groupBySelectedItem; }
            set
            {
                this._groupBySelectedItem = value;
			
				ApplyVisibilityToGroupByItems();
                this.RaisePropertyChanged(() => this.GroupBySelectedItem);
				this.RaisePropertyChanged(() => this.IsFilterAnyField);
				this.RaisePropertyChanged(() => this.TemplateSelected);

				Mouse.OverrideCursor = Cursors.Wait;
                BuildOnBackgroundThread();
            }
        }

        public long ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => this.ItemsTotal);

                this.RaisePropertyChanged(() => this.ProgressTotalItems);
            }
        }

        public int LastSessionCountItem
        {
            get { return this._lastSessionCountItem; }
            set
            {
                this._lastSessionCountItem = value;
                this.RaisePropertyChanged(() => this.LastSessionCountItem);

                this.RaisePropertyChanged(() => this.LastSessionCountItemString);
            }
        }

		public int LastSessionCountDocument
        {
			get { return this._lastSessionCountDocument; }
            set
            {
				this._lastSessionCountDocument = value;
				this.RaisePropertyChanged(() => this.LastSessionCountDocument);

				this.RaisePropertyChanged(() => this.LastSessionCountDocumentsString);
            }
        }


	
        public double LastSessionSumQuantityEdit
        {
            get { return this._lastSessionSumQuantityEdit; }
            set
            {
                this._lastSessionSumQuantityEdit = value;
                this.RaisePropertyChanged(() => this.LastSessionSumQuantityEdit);

                this.RaisePropertyChanged(() => this.LastSessionSumQuantityEditString);
            }
        }

		public string AdapterFromPDA
		{
			get 
			{
				return _adapterFromPDA;
			}
		}

		private string GetAdapterNamefromPDA()
		{
			string adapterCode = base.CurrentInventor.ImportPDAProviderCode;
			var containerAdapters = this._unityContainer.ResolveAll<IImportModuleInfo>().Where(r => r.ImportDomainEnum == ImportDomainEnum.ImportInventProduct).ToList();
			string adapterFromPDAName = "none";
			if (containerAdapters != null)
			{
				List<IImportModuleInfo> result = containerAdapters.Where(z => z.Name.ToLower() == adapterCode.ToLower()).ToList();
				if (result != null && result.Count > 0) adapterFromPDAName = result.FirstOrDefault().Title;
			}
			string ret = String.Format(Localization.Resources.View_IturListDetails_tbAdapterFromPDA, adapterFromPDAName);
			return ret; 
		}
		

        public string ProgressTotalItems
        {
            get { return String.Format(Localization.Resources.View_IturListDetails_tbTotalStatus, _itemsTotal); }
        }

        public string LastSessionCountItemString
        {
            get { return String.Format(Localization.Resources.View_IturListDetails_tbLastSessionCountItem, this._lastSessionCountItem); }
        }

		
		public string LastSessionCountDocumentsString
        {
			get { return String.Format(Localization.Resources.View_IturListDetails_tbLastSessionCountDocuments, this._lastSessionCountDocument); }
        }

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
			if (base.CurrentInventor.ImportPDAProviderCode == Common.Constants.ImportAdapterName.ImportPdaMISAndDefaultAdapter
			|| base.CurrentInventor.ImportPDAProviderCode == Common.Constants.ImportAdapterName.ImportPdaMISAdapter)
			{
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

        public string LastSessionSumQuantityEditString
        {
            get 
			{
				try
				{
					long sum = Convert.ToInt64(this._lastSessionSumQuantityEdit);
					return String.Format(Localization.Resources.View_IturListDetails_tbLastSessionSumQuantityEdit, sum); 
				}
				catch { }
				return String.Format(Localization.Resources.View_IturListDetails_tbLastSessionSumQuantityEdit, 0); 
			}
        }

		public bool IsMisImportFromPDA
		{
			get 
			{
				bool isMisImportFromPDA =  false;
			//	bool isMisImportFromPDA = (base.CurrentInventor.ImportPDAProviderCode == "ImportPdaMISAdapter");
			//	return isMisImportFromPDA; 
				if (base.CurrentInventor.ImportPDAProviderCode == Common.Constants.ImportAdapterName.ImportPdaMISAndDefaultAdapter
				|| base.CurrentInventor.ImportPDAProviderCode == Common.Constants.ImportAdapterName.ImportPdaMISAdapter)
				{
					isMisImportFromPDA = true;
				}

				return isMisImportFromPDA;
			}
		}


        public int PageSize
        {
            get { return this._pageSize; }
            set
            {
                this._pageSize = value;
                this.RaisePropertyChanged(() => this.PageSize);
            }
        }

        public int PageCurrent
        {
            get { return this._pageCurrent; }
            set
            {
                this._pageCurrent = value;
                this.RaisePropertyChanged(() => this.PageCurrent);

                Mouse.OverrideCursor = Cursors.Wait;
                this.BuildOnBackgroundThread();
            }
        }

        public UICommand IturimAddEditCommand
        {
            get { return this._iturimAddEditCommand; }
        }

        public UICommand ImportFromPdaCommand
        {
            get { return this._importFromPdaCommand; }
        }

		public UICommand DevicePDAStatisticCommand
        {
			get { return this._devicePDAStatisticCommand; }
        }

		public UICommand DeviceWorkerPDAStatisticCommand
		{
			get { return this._deviceWorkerPDAStatisticCommand; }
		}

		public UICommand ImportFromPdaAutoCommand
        {
            get { return _importFromPdaAutoCommand; }
        }

        public UICommand RefreshCommand
        {
            get { return this._refreshCommand; }
        }

        public UICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public UICommand ReportsCommand
        {
            get { return _reportsCommand; }
        }

        public UICommand ReportCommand
        {
            get { return _reportCommand; }
        }

        public DelegateCommand ChangeLocationCommand
        {
            get { return _changeLocationCommand; }
        }

		public DelegateCommand ChangeIturNameCommand
        {
			get { return _changeIturNameCommand; }
		}


        public bool IsGrouping
        {
            get { return this._isGrouping; }
            set
            {
                this._isGrouping = value;
                this.RaisePropertyChanged(() => this.IsGrouping);
            }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
            get { return _reportButtonViewModel; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(this._isBusy);
            }
        }

        public UICommand PrintReportCommand
        {
            get { return _printReportCommand; }
        }


		public UICommand SetDissableItursCommand
        {
			get { return _setDissableItursCommand; }
        }

		public UICommand PrintReportByLocationCodeCommand
        {
			get { return _printReportByLocationCodeCommand; }
        }

		public UICommand PrintReportByTagCommand
        {
			get { return _printReportByTagCommand; }
        }


		public UICommand AddEditLocatonItursCommand
		{
			get { return _eddEditLocatonItursCommand; }
		}
		

		public DelegateCommand<string> IturListPrintByIndexCommand
		{
			get { return _iturListPrintByIndexCommand; }
		}


		public DelegateCommand<string> IturListExportErpByConfigCommand
		{
			get { return _iturListExportErpByConfigCommand; }
		}

		//Command="{Binding Path=RunExportErpByConfigCommand}" CommandParameter="{Binding Path=SelectedExportErp}"
		public DelegateCommand RunExportErpByConfigCommand
		{
			get { return this._runExportErpByConfigCommand; }
		}

		// ==================== Export Erp
		public void RunExportErpByConfigCommandExecuted()
		{
			if (this._selectedItems == null) return;

			List<string> iturCodesList = _selectedItems.Select(r => r.Code).ToList();
			ExportErpCommandInfo info = new ExportErpCommandInfo();
			info.IturCodeList = iturCodesList;
			info.LocationCodeList = new List<string>();
			RunExportErpByConfig(info);
		}



		public virtual void RunExportErpByConfig(ExportErpCommandInfo info)										   //IExportErpModuleInfo exportErpModuleInfo
		{
			if (base.CurrentInventor == null)	return;
	 
			ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;
			
			using (new CursorWait("RunExportErpByConfig"))
			{
				string contextCustomerAdapterName = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportERPAdapterCode;
				if (File.Exists(contextCustomerAdapterName) == true) return;

				bool isEcxist = IsEcxistAdapterErpConfigFileForCustomer(contextCustomerAdapterName);
				if (isEcxist == false) return;	 

				ExportErpModuleBaseViewModel exportErpModuleBaseViewModel = null;
				try
				{
					exportErpModuleBaseViewModel = this._serviceLocator.GetInstance<ExportErpModuleBaseViewModel>(contextCustomerAdapterName);
				}
				catch { }

				if (exportErpModuleBaseViewModel == null) return;	  

				ExportErpWithModulesViewModel exportErpWithModulesViewModel = this._serviceLocator.GetInstance<ExportErpWithModulesViewModel>();
				IExportErpModule viewModel = exportErpModuleBaseViewModel as IExportErpModule;
				if (exportErpWithModulesViewModel != null)
				{

					exportErpWithModulesViewModel.ExportErpFromConfigMode(viewModel, contextCustomerAdapterName, fromConfigXDoc, base.State, info);
					string exportErpLogPath = this.GetExportErpFolderPath("Inventor", base.CurrentInventor.Code);
					if (!Directory.Exists(exportErpLogPath)) return;
					Utils.OpenFolderInExplorer(exportErpLogPath);
				}
			}

			
		}

		public bool CanExportERPByItur
		{
			get { return RunExportErpByConfigCommandCanExecute(); }
		}

		public bool RunExportErpByConfigCommandCanExecute()
		{
			if (base.CurrentInventor == null) return false;
			string contextCustomerAdapterName = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportERPAdapterCode;
			if (File.Exists(contextCustomerAdapterName) == true) return false;

			bool isEcxist = IsEcxistAdapterErpConfigFileForCustomer(contextCustomerAdapterName);
			if (isEcxist == false) return false;

			return true;
		}

		private bool IsEcxistAdapterErpConfigFileForCustomer(string contextCustomerAdapterName)
		{
			string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			string adapterConfigFileName = @"\" + contextCustomerAdapterName + ".config";
			string configFilePath = configPath + adapterConfigFileName;
			if (File.Exists(configFilePath) == false) return false;	 
			return true;
		}

	
		public string GetExportErpFolderPath(string objectType, string objectCode)
		{
			if (base.CurrentInventor == null) return String.Empty;
			if (string.IsNullOrWhiteSpace(objectType) == true) return String.Empty;
			if (string.IsNullOrWhiteSpace(objectCode) == true) return String.Empty;
			string logPath = UtilsPath.ExportErpFolder(this._dbSettings, objectType, objectCode);
			return logPath;
		}


		//old
		//public DelegateCommand IturListPrintCommand
		//{
		//	get { return _iturListPrintCommand; }
		//}

		//public DelegateCommand IturListPrintIS0155Command
		//{
		//	get { return _iturListPrintIS0155Command; }
		//}

		//public DelegateCommand IturListPrintIS0160Command
		//{
		//	get { return _iturListPrintIS0160Command; }
		//}

		//public Visibility IturListPrintMenuVisiable
		//{
		//	get { return _iturListPrintMenuVisiable; }
		//	set { _iturListPrintMenuVisiable = value; }
		//}

		//public Visibility IturListPrintIS0155MenuVisiable
		//{
		//	get { return _iturListPrintIS0155MenuVisiable; }
		//	set { _iturListPrintIS0155MenuVisiable = value; }
		//}

		//public Visibility IturListPrintIS0160MenuVisiable
		//{
		//	get { return _iturListPrintIS0160MenuVisiable; }
		//	set { _iturListPrintIS0160MenuVisiable = value; }
		//}
        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                RaisePropertyChanged(() => BusyText);
            }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return _yesNoRequest; }
        }

        public IList<IturDashboardItemViewModel> SelectedItems
        {
            set
            {
                _selectedItems = value;

                if (_selectedItems.Count == 0)
                    this._isDisabled = false;
                if (_selectedItems.All(r => r.Itur.Disabled == true))
                    this._isDisabled = true;
                else if (_selectedItems.All(r => r.Itur.Disabled == false || r.Itur.Disabled == null))
                    this._isDisabled = false;
                else this._isDisabled = null;
            }
        }

        public bool? IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                this._isDisabled = value;
                Stopwatch stopwatch = Stopwatch.StartNew();

                RaisePropertyChanged(() => IsDisabled);
                Iturs iturs = new Iturs();
                using (new CursorWait())
                {
                    if (this._selectedItems == null || this._selectedItems.Count == 0) return;

                    foreach (IturDashboardItemViewModel iturViewModel in this._selectedItems)
                    {
                        Itur itur = iturViewModel.Itur;
                        iturs.Add(itur);
                    }

                    bool disabled = (this._isDisabled == null) ? false : Convert.ToBoolean(this._isDisabled);

                    stopwatch.Stop();
                    System.Diagnostics.Debug.Print(String.Format("GUI time: {0}", stopwatch.ElapsedTicks.ToString()));

                    stopwatch = Stopwatch.StartNew();
					// this._iturRepository.SetDisabledStatusBitByIturCode(iturs, disabled, base.GetDbPath);
					Iturs updatedAllItursInDB = this._iturRepository.SwitchDisabledStatusBitByIturCode(iturs, disabled, base.GetDbPath);
					this._importIturBlukRepository.ClearIturs(base.GetDbPath);
					this._importIturBlukRepository.InsertItursFromList(base.GetDbPath, updatedAllItursInDB.ToList());
					stopwatch.Stop();
                    System.Diagnostics.Debug.Print(String.Format("SetDisabled time: {0}", stopwatch.ElapsedTicks.ToString()));

                    stopwatch = Stopwatch.StartNew();
                    //update view
                    this.UpdateViewIturs(iturs);

                    stopwatch.Stop();
                    System.Diagnostics.Debug.Print(String.Format("IturViewModel.Update time: {0}", stopwatch.ElapsedTicks.ToString()));
					BuildOnBackgroundThread();		  //Marina 220202022
					Task.Factory.StartNew(() => ProgressItemsBuild()).LogTaskFactoryExceptions("IsDisabled");
                }
            }
        }

        public IturAdvancedFilterData Filter
        {
			get { return this._filter; }
        }

        public bool IsFilterAnyField
        {
			get 
			{ 
				return this._filter == null ? false : this._filter.IsAnyField();
			}
        }

		public bool IsFromFilter
		{
			get
			{
				return this._filter == null ? false : this._filter.IsFromFilter;
			}
		}

		public void RefreshModeSelectedItem_GroupBySelectedItem_TemplateSelected()
		{
			// =========== ModeSelectedItem
			//RaisePropertyChanged(() => this.ModeSelectedItem);			   ?
			//this.ApplyVisibilityToGroupByItems();
			//this.RaisePropertyChanged(() => IsFilterAnyField);
			//ApplyVisibilityToGroupByItems();

			if (this._filter != null)
			{
				if (this._filter.IsFromFilter == false)
				{
					this.InitFilter();
					if (this._templateSelected != null)
					{
						if (this._templateSelected.FileInfo != null)
						{
							//if (this._filter != null)
							//{
							IFilterData data = this._filterTemplateRepository.GetData(_templateSelected.FileInfo, this._filter.GetType()) as IFilterData;
							if (data != null)
							{
								this._filter = data as IturAdvancedFilterData;
								this._filter.IsFromFilter = false;
								this._pageCurrent = 1;
							}
							//}
						}
					}
				}
			}

			this.RaisePropertyChanged(() => this.TemplateSelected);				//2021
			this.RaisePropertyChanged(() => this.ModeSelectedItem);          //2021
			this.RaisePropertyChanged(() => this.GroupBySelectedItem);       //2021
			this.RaisePropertyChanged(() => this.IsFilterAnyField);                 //2021

			Mouse.OverrideCursor = Cursors.Wait;
			BuildOnBackgroundThread();
		}


		
		public ObservableCollection<ItemFindViewModel> ModeItems
        {
			get { return this._modeItems; }
        }

        public ItemFindViewModel ModeSelectedItem
        {
			get { return this._modeSelectedItem; }
            set
            {
				this._modeSelectedItem = value;
				this.RaisePropertyChanged(() => this.ModeSelectedItem);
				this.ApplyVisibilityToGroupByItems();
				this.RaisePropertyChanged(() => this.TemplateSelected);
				this.RaisePropertyChanged(() => this.GroupBySelectedItem);
				this.RaisePropertyChanged(() => this.IsFilterAnyField);

				Mouse.OverrideCursor = Cursors.Wait;
				this.BuildOnBackgroundThread();
            }
        }

        public ObservableCollection<ItemFindViewModel> LocationItems
        {
			get { return this._locationItems; }
        }


        public ItemFindViewModel LocationSelected
        {
			get { return this._locationSelected; }
            set
            {
				this._locationSelected = value;
				RaisePropertyChanged(() => this.LocationSelected);

				this.RaisePropertyChanged(() => this.GroupBySelectedItem);
				this.RaisePropertyChanged(() => this.TemplateSelected);
				RaisePropertyChanged(() => this.IsFilterAnyField);
				


				if (this._locationSelected != null)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
					this.BuildOnBackgroundThread();
                }
            }
        }

		public ObservableCollection<ItemFindViewModel> TagItems
		{
			get { return this._tagItems; }
		}

		public ObservableCollection<FilterTemplateItemViewModel> TemplateItems
		{
			get 
			{ 
				return _templateItems; 
			}
		}

		public ItemFindViewModel TagSelected
		{
			get { return this._tagSelected; }
			set
			{
				this._tagSelected = value;
				RaisePropertyChanged(() => this.TagSelected);
				RaisePropertyChanged(() => IsFilterAnyField);

				if (_tagSelected != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
					this.BuildOnBackgroundThread();
				}
			}
		}

		public FilterTemplateItemViewModel TemplateSelected
		{
			get { return this._templateSelected; }
			set
			{
				this._templateSelected = value;
				if(this._templateSelected == null)
				{
					//this._templateSelected = _templateItems.FirstOrDefault();
					return;
				}
				
				RaisePropertyChanged(() => this.TemplateSelected);
				// using (new CursorWait())
				// {
				//_eventAggregator.GetEvent<FilterEvent<IFilterData>>().Publish(_fieldViewModel.BuildFilterData());
				//}

				//	 try
				//{
				//	ISearchFieldViewModel searchFieldViewModel = GetFieldViewModel();
				//	IFilterData data = searchFieldViewModel.BuildFilterData();

				//	_filterTemplateRepository.Update(_selectedItem.FileInfo, data);
				//}
				//catch (Exception e)
				//{
				//	_logger.ErrorException("UpdateCommandExecuted", e);
				//}
				//if (_selectedItem != null)
				//{
				//	using (new CursorWait())
				//	{
				//		ISearchFieldViewModel searchFieldViewModel = GetFieldViewModel();
				//		IFilterData data = _filterTemplateRepository.GetData(_selectedItem.FileInfo, searchFieldViewModel.BuildFilterData().GetType()) as IFilterData;
				//		if (data != null)
				//			searchFieldViewModel.ApplyFilterData(data);
				//	}
				//}

				if (this._filter != null)
				{
					if (this._filter.IsFromFilter == false)
					{
						this.InitFilter();
						if (this._templateSelected != null)
						{
							if (this._templateSelected.FileInfo != null)
							{
								//if (this._filter != null)
								//{
								IFilterData data = this._filterTemplateRepository.GetData(_templateSelected.FileInfo, this._filter.GetType()) as IFilterData;
								if (data != null)
								{
									this._filter = data as IturAdvancedFilterData;
									this._filter.IsFromFilter = false;
									this._pageCurrent = 1;
								}
								//}
							}
						}
						RaisePropertyChanged(() => IsFilterAnyField);

						Mouse.OverrideCursor = Cursors.Wait;
						BuildOnBackgroundThread();
					}
				}
			}
		}

		public ObservableCollection<ItemFindViewModel> StatusItems
		{
			get { return _statusItems; }
		}

		public ItemFindViewModel StatusSelected
		{
			get { return _statusSelected; }
			set
			{
				_statusSelected = value;
				RaisePropertyChanged(() => StatusSelected);
				RaisePropertyChanged(() => IsFilterAnyField);

				if (_statusSelected != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
					BuildOnBackgroundThread();
				}
			}
		}

        public bool PagingControlVisible
        {
            get { return _pagingControlVisible; }
            set
            {
                _pagingControlVisible = value;
                RaisePropertyChanged(() => PagingControlVisible);
            }
        }


		public bool TemplateVisible
        {
			get { return _templateVisible; }
            set
            {
				_templateVisible = value;
				RaisePropertyChanged(() => TemplateVisible);
            }
        }

        public bool LocationList3Visible
        {
            get { return _locationList3Visible; }
            set
            {
                _locationList3Visible = value;
                RaisePropertyChanged(() => LocationList3Visible);
            }
        }

		public bool StatusList3Visible
        {
			get { return _statusList3Visible; }
            set
            {
				_statusList3Visible = value;
				RaisePropertyChanged(() => StatusList3Visible);
            }
        }

		  public bool TagList3Visible
		  {
			  get { return _tagList3Visible; }
			  set
			  {
				  _tagList3Visible = value;
				  RaisePropertyChanged(() => TagList3Visible);
			  }
		  }

		public bool StatusList4Visible
		{
			get { return _statusList4Visible; }
			set
			{
				_statusList4Visible = value;
				RaisePropertyChanged(() => StatusList4Visible);
			}
		}

		public bool TagList4Visible
		{
			get { return _tagList4Visible; }
			set
			{
				_tagList4Visible = value;
				RaisePropertyChanged(() => TagList4Visible);
			}
		}


		public bool LocationList4Visible
		{
			get { return _locationList4Visible; }
			set
			{
				_locationList4Visible = value;
				RaisePropertyChanged(() => LocationList4Visible);
			}
		}

        public DelegateCommand UpCommand
        {
            get { return _upCommand; }
        }

        public DelegateCommand DownCommand
        {
            get { return _downCommand; }
        }

        public ObservableCollection<ProgressItemViewModel> ProgressItems
        {
            get { return _progressItems; }
        }

        public double ProgressDone
        {
            get { return _progressDone; }
            set
            {
                _progressDone = value;
                RaisePropertyChanged(() => ProgressDone);
            }
        }

        public string ProgressDoneString
        {
            get { return _progressDoneString; }
            set
            {
                _progressDoneString = value;
                RaisePropertyChanged(() => ProgressDoneString);
            }
        }

        public string ProgressDoneTotalItemsString
        {
            get { return _progressDoneTotalItemsString; }
            set
            {
                _progressDoneTotalItemsString = value;
                RaisePropertyChanged(() => ProgressDoneTotalItemsString);
            }
        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			
            this._eventAggregator.GetEvent<LocationAddedEvent>().Subscribe(this.LocationAdded);
            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(IturFilter);
            this._eventAggregator.GetEvent<IturLocationChangedEvent>().Subscribe(LocationChanged);
			this._eventAggregator.GetEvent<IturQuantityEditChangedEvent>().Subscribe(this.QuantityEditChanged);
			this._eventAggregator.GetEvent<IturNameChangedEvent>().Subscribe(IturNameChanged);
            this._eventAggregator.GetEvent<PrintQueueRunningEvent>().Subscribe(PrintQueueStarted);
            this._eventAggregator.GetEvent<WindowPreviewKeyUpEvent>().Subscribe(WindowPreviewKeyUp);

            _filter = UtilsConvert.GetObjectFromNavigation(navigationContext, this._navigationRepository, Common.NavigationObjects.Filter, true) as IturAdvancedFilterData;
            if (_filter == null)
            {
                InitFilter();
            }
            RaisePropertyChanged(() => IsFilterAnyField);

            BuildLocations();
			BuildStatusGroup();
			BuildTags();
			BuildTemplates(_isNavigatedFirstTime);
			this._reportIniFile = this._reportIniRepository.CopyContextMenuReportTemplateIniFile(base.CurrentInventor.Code);

			if (_isNavigatedFirstTime)
			{
				this._locationSelected = this._locationItems.FirstOrDefault();
				this._statusSelected = this._statusItems.FirstOrDefault();
				this._tagSelected = this._tagItems.FirstOrDefault();
				//this._templateSelected = this._templateItems.FirstOrDefault();

				string mode = _userSettingsManager.IturModeGet();
				_modeSelectedItem = ModeItems.FirstOrDefault(r => r.Value == mode);
				ApplyVisibilityToGroupByItems(); 


				if (mode == ComboValues.IturListDetailsMode.ModePaged)
				{
					string group = _userSettingsManager.IturGroupGet();
					_groupBySelectedItem = GroupByItems.FirstOrDefault(r => r.Value == group);
					//RaisePropertyChanged(() => GroupBySelectedItem);	 //2021
				}

				//  if (mode != ComboValues.IturListDetailsMode.ModeTag)
				//{
				//	string group = _userSettingsManager.IturGroupGet();
				//	_groupBySelectedItem = GroupByItems.FirstOrDefault(r => r.Value == group);
				//	RaisePropertyChanged(() => GroupBySelectedItem);
				//}

				_isNavigatedFirstTime = false;
			}
			else 
			{
				if (_templateSelected != null)
				{
					
					ApplyVisibilityToGroupByItems(false, _templateSelected._name, true); //2021
					//ApplyVisibilityToGroupByItems(false, "OneDocIsApprove"); //2021
					
				}
			}
			this._reportButtonViewModel.OnNavigatedTo(navigationContext);
            this._reportButtonViewModel.Initialize(this.ReportsCommandExecuted, () =>
                                                                                   {
                                                                                       //SelectParams sp = BuildSelectParams();
                                                                                       SelectParams sp = new SelectParams();
                                                                                       sp.IsEnablePaging = false;
                                                                                       sp.SortParams = String.Empty;
																					   return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
                                                                                   }, ViewDomainContextEnum.Iturs);

            if (Common.State.GlobalState.BACK == false)
            {
                _pageCurrent = 1;
                RaisePropertyChanged(() => PageCurrent);
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
			observCountingFiles = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4)).Select(x => x);
			disposeObservCountingFiles = observCountingFiles.Subscribe(CountingFiles);
			RaisePropertyChanged(() => IsMisImportFromPDA);	

            if (this._pageSize != this._userSettingsManager.PortionItursGet())
                this.PageSize = this._userSettingsManager.PortionItursGet();

            Task.Factory.StartNew(() =>
                                      {
                                          Stopwatch sw = Stopwatch.StartNew();

                                          this._allLocations = this._locationRepository.GetLocations(base.GetDbPath);
										  this._allStatusGroups = this._statusGroupRepository.CodeStatusIturGroupDictionary;

                                          sw.Stop();
                                          System.Diagnostics.Debug.Print("itur list details: " + sw.Elapsed.TotalSeconds.ToString());

                                          BuildOnBackgroundThread();

                                          Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);
                                          ProgressItemsBuild();
									  }).LogTaskFactoryExceptions("OnNavigatedTo");

			this._adapterFromPDA = this.GetAdapterNamefromPDA();
			RaisePropertyChanged(() => AdapterFromPDA);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this.ReportButtonViewModel.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<LocationAddedEvent>().Unsubscribe(LocationAdded);
            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(IturFilter);
			this._eventAggregator.GetEvent<IturNameChangedEvent>().Unsubscribe(IturNameChanged);
			this._eventAggregator.GetEvent<IturQuantityEditChangedEvent>().Unsubscribe(this.QuantityEditChanged);
			this._eventAggregator.GetEvent<IturLocationChangedEvent>().Unsubscribe(LocationChanged);
            this._eventAggregator.GetEvent<PrintQueueRunningEvent>().Unsubscribe(PrintQueueStarted);
            this._eventAggregator.GetEvent<WindowPreviewKeyUpEvent>().Unsubscribe(WindowPreviewKeyUp);
			this._eventAggregator.GetEvent<GroupConfigurationChangedEvent>().Unsubscribe(GroupConfChanged);
			this._eventAggregator.GetEvent<SortConfigurationChangedEvent>().Unsubscribe(SortConfChanged);

			if (disposeObservCountingFiles != null) disposeObservCountingFiles.Dispose();

            this._items.Clear();
			//this._isNavigatedFirstTime = true;							   //2021
		}

        public bool KeepAlive
        {
            get { return true; }
        }

        public UICommand PlanogramCommand
        {
            get { return _planogramCommand; }
        }

        public UICommand PdaCommand
        {
            get { return _pdaCommand; }
        }

        public UICommand ErpCommand
        {
            get { return _erpCommand; }
        }

        public UICommand ChangeStatusCommand
        {
            get { return _changeStatusCommand; }
        }

        public UICommand UpdateCommand
        {
            get { return _updateCommand; }
        }

		public DelegateCommand DeleteNoneCatalogCommand
		{
			get { return _deleteNoneCatalogCommand; }
		}

		List<string> _refreshDocumentCodeList;
		public List<string> RefreshDocumentCodeList
		{
			get
			{
				return _refreshDocumentCodeList;
			}
		}

		private void DeleteNoneCatalogCommandExecuted()
		{
			_refreshDocumentCodeList = DeleteInventProductWithoutCatalog();
			if (_refreshDocumentCodeList == null) return;
			this._documentHeaderRepository.SetNullToApproveDocuments(_refreshDocumentCodeList, base.GetDbPath);
			BusyText = Localization.Resources.View_IturListDetails_busyContent;
			IsBusy = true;
			Task.Factory.StartNew(RefreshActionDocuments).LogTaskFactoryExceptions("RefreshCommandExecuted");
		}

		private bool DeleteNoneCatalogCommandCanExecute()
		{
			return true;
		}

		private List<string> DeleteInventProductWithoutCatalog()
		{
			List<string> documentCodeList = null;

			string message = String.Format(Localization.Resources.Msg_Delete_Invent_Product_NotExistInCatalog);

			MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				InventProducts inventproducts = this._inventProductRepository.GetInventProductNotExistInCatalog(base.GetDbPath);
				if (inventproducts == null) return null;
				try
				{
					SaveAllNotExistInCatalogToFile(inventproducts);
				}
				catch (Exception exp)
				{
					return documentCodeList;
				}

				documentCodeList = this._inventProductRepository.DeleteAllNotExistInCatalog(base.GetDbPath);
			}
			return documentCodeList;
		}


		private void SaveAllNotExistInCatalogToFile(InventProducts inventproducts)
		{
			if (inventproducts == null) return;
			if (base.State.CurrentInventor == null) return;
			
			string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора
			string path = Path.Combine(importPDAFolder, "Inventor", base.State.CurrentInventor.Code, "inData", "backup");
			DateTime dt = DateTime.Now;
			//string subfolder = dt.Year + @"\" + String.Format("{0:D2}", dt.Month) + @"\" + String.Format("{0:D2}", dt.Day);
			string createDate = String.Format("{0:D2}{1}{2:D2}{3}{4}", dt.Day, "_", dt.Month, "_", dt.Year);
			string createDate1 = String.Format("{0:D2}{1}{2:D2}{3}{4}", dt.Day, "/", dt.Month, "/", dt.Year);
			string fileName = "NotExistInCatalog_" + createDate +  ".txt";
	
			string filePath = CheckLogFilePath(path, fileName);
			if (string.IsNullOrWhiteSpace(filePath) == true) return;

			
			
			using (StreamWriter sw = File.AppendText(filePath))
			{
				try
				{
					foreach (InventProduct ip in inventproducts)
					{
						//"B|ITURCODE|ITURERP|MAKAT|Quantity|B|CreatedDate|00:00:00|PartialQuantity"
						string[] newRows = new string[] { "B" , ip.IturCode, ip.ERPIturCode, ip.Makat, 
						ip.QuantityEdit.ToString() , "B" , createDate1, "00:00:00", ip.QuantityInPackEdit.ToString() };
						string newRow = string.Join("|", newRows);
						sw.WriteLine(newRow);
					}
				}
				catch (Exception msg)
				{
					sw.WriteLine(msg);
				}
				sw.Flush();
			}

		}

		public string CheckLogFilePath(string path, string fileName)
		{
			string logFilePath = Path.Combine(path, fileName);
			if (File.Exists(logFilePath) == true)
			{
				return logFilePath;
			}

			if (string.IsNullOrWhiteSpace(path) == true)
				return "";
			if (string.IsNullOrWhiteSpace(fileName) == true)
				return "";

			try
			{
				if (Directory.Exists(path) == false)
				{
					Directory.CreateDirectory(path);
				}

				if (File.Exists(logFilePath) == false)
				{
					File.Create(logFilePath).Close();

					using (StreamWriter sw = File.AppendText(logFilePath))
					{
						try
						{
							//H|999|4d7775a4-2e6a-4c7e-9bac-c3b70d3d7be2|29/06/2018|00:00:00
							string iventorCode = "";
							DateTime dt = DateTime.Now;
							string date = String.Format("{0:D2}{1}{2:D2}{3}{4}", dt.Day, "/", dt.Month, "/", dt.Year);
							if (base.State.CurrentInventor != null) iventorCode = base.State.CurrentInventor.Code;
							string message1 = "H|999|" + iventorCode + "|" + date +  "|00:00:00";
							sw.WriteLine(message1);
						}
						catch (Exception msg)
						{
							sw.WriteLine(msg);
						}
						sw.Flush();
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
			return logFilePath;
		}


        private void UpdateViewIturs(Iturs iturs)
        {
            List<string> searchItur = iturs.Select(r => r.IturCode).Distinct().ToList();
            SelectParams sp = new SelectParams();
            if (searchItur.Count != 0)
            {
                sp.FilterStringListParams.Add("IturCode", new FilterStringListParam()
                {
                    Values = searchItur
                });
            }

            Iturs dbIturs = this._iturRepository.GetIturs(sp, base.GetDbPath);

            foreach (Itur dbItur in dbIturs)
            {
                IturDashboardItemViewModel iturViewModel = this._selectedItems.FirstOrDefault(r => r.Code == dbItur.IturCode);
                if (iturViewModel == null) continue;

                StatusIturGroup statusGroup = GetStatusIturGroupByItur(dbItur);
                if (statusGroup != null)
                {
                    string color = UtilsStatus.FromStatusGroupBitToColor(this._statusGroupRepository.BitStatusIturGroupEnumDictionary,
                                                                         statusGroup.Bit, this._userSettingsManager);
                    iturViewModel.StatusColor = color;
                }

                iturViewModel.Update(dbItur, statusGroup);
            }
        }

		private void BuildStatusGroup()
        {
			_logger.Trace("BuildStatusGroup");

			string previousStatusGroupCode = String.Empty;

			if (this._statusSelected != null)
            {
				previousStatusGroupCode = this._statusSelected.Value;
            }

            this._statusItems.Clear();
			this._statusItems.Add(new ItemFindViewModel() { Value = "8", Text = "Not Empty", FillColor = "" });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.Empty).ToString(), Text = IturStatusGroupEnum.Empty.ToString(), FillColor = GetStatusGroupcolor(IturStatusGroupEnum.Empty) });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.OneDocIsApprove).ToString(), Text = IturStatusGroupEnum.OneDocIsApprove.ToString(), FillColor = GetStatusGroupcolor(IturStatusGroupEnum.OneDocIsApprove) });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.AllDocsIsApprove).ToString(), Text = IturStatusGroupEnum.AllDocsIsApprove.ToString(), FillColor = GetStatusGroupcolor(IturStatusGroupEnum.AllDocsIsApprove) });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.NotApprove).ToString(), Text = IturStatusGroupEnum.NotApprove.ToString(), FillColor = GetStatusGroupcolor(IturStatusGroupEnum.NotApprove) });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.DisableAndNoOneDoc).ToString(), Text = IturStatusGroupEnum.DisableAndNoOneDoc.ToString(), FillColor = GetStatusGroupcolor(IturStatusGroupEnum.DisableAndNoOneDoc) });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.DisableWithDocs).ToString(), Text = IturStatusGroupEnum.DisableWithDocs.ToString(), FillColor = GetStatusGroupcolor(IturStatusGroupEnum.DisableWithDocs) });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.Error).ToString(), Text = IturStatusGroupEnum.Error.ToString(), FillColor = GetStatusGroupcolor(IturStatusGroupEnum.Error) });
			this._statusItems.Add(new ItemFindViewModel() { Value = ((int)IturStatusGroupEnum.None).ToString(), Text = "", FillColor = "" });
			


			if (String.IsNullOrWhiteSpace(previousStatusGroupCode) == false)
            {
				_statusSelected = this._statusItems.FirstOrDefault(r => r.Value == previousStatusGroupCode);
				//RaisePropertyChanged(() => StatusSelected);				  2021
            }
        }


		private void BuildLocations()
		{
			_logger.Trace("BuildLocations");

			string previousLocationCode = String.Empty;

			if (_locationSelected != null)
			{
				previousLocationCode = _locationSelected.Value;
			}

			_locationItems.Clear();
			_locationItems.Add(new ItemFindViewModel() { Value = "", Text = "" });
			foreach (Location location in _locationRepository.GetLocations(base.GetDbPath))
			{
				bool isAdd = true;
				if (_filter != null && _filter.IsLocation && !_filter.Locations.Any(r => r == location.Code))
				{
					isAdd = false;
				}

				if (isAdd == true)
					_locationItems.Add(new ItemFindViewModel() { Value = location.Code, Text = location.Name });

				//add empty
			}

			if (!String.IsNullOrWhiteSpace(previousLocationCode))
			{
				_locationSelected = _locationItems.FirstOrDefault(r => r.Value == previousLocationCode);
				RaisePropertyChanged(() => LocationSelected);
			}
		}

		private void BuildTags()
		{
			_logger.Trace("BuildTags");

			string previousBuildTag = String.Empty;

			if (_tagSelected != null)
			{
				previousBuildTag = _tagSelected.Value;
			}

			_tagItems.Clear();
			_tagItems.Add(new ItemFindViewModel() { Value = "", Text = "" });

			foreach (string tag in _iturRepository.GetTagList(base.GetDbPath))
			{
				bool isAdd = true;
				if (_filter != null && _filter.IsTag && !_filter.Tags.Any(r => r == tag))				  //??
				{
					isAdd = false;
				}

				if (isAdd == true)
				{
					if (string.IsNullOrWhiteSpace(tag) == false)
					{
						_tagItems.Add(new ItemFindViewModel() { Value = tag, Text = tag });
					}
				}
			}
			

			if (String.IsNullOrWhiteSpace(previousBuildTag) == false)
			{
				_tagSelected = _tagItems.FirstOrDefault(r => r.Value == previousBuildTag);
			}
			else
			{
				_tagSelected = _tagItems.FirstOrDefault(r => r.Value == "");
			}
			RaisePropertyChanged(() => TagSelected);
		}


		private void BuildTemplates(bool  isNavigatedFirstTime = false)
		{
			_logger.Trace("BuildTemplates");

			string previousBuildTemplate = String.Empty;

			if (isNavigatedFirstTime == false)
			{
				if (_templateSelected != null)
				{
					previousBuildTemplate = _templateSelected.Name;
				}
			}

		

			using (new CursorWait())
			{
				lock (TemplateItems)
				{
					_templateItems.Clear();

					FilterTemplateItemViewModel itemEmpty = new FilterTemplateItemViewModel();
					_templateItems.Add(itemEmpty);
				}

				//foreach (FileInfo fileInfo in _filterTemplateRepository.GetFiles(ViewDomainContextEnum.IturAdvancedSearch.ToString()))
				//{
				//	FilterTemplateItemViewModel item = new FilterTemplateItemViewModel(fileInfo, GetFilterDataType);
				//	_templateItems.Add(item);
				//}
				List<FilterTemplateItemViewModel> _templateItemsTemp = new List<FilterTemplateItemViewModel>();
				foreach (FileInfo fileInfo in _filterTemplateRepository.GetFiles(ViewDomainContextEnum.IturAdvancedSearch.ToString()))
				{
					FilterTemplateItemViewModel item = new FilterTemplateItemViewModel(fileInfo, GetFilterDataType);
					_templateItemsTemp.Add(item);
				}

				if (this._userSettingsManager.LanguageGet() == enLanguage.Hebrew)
				{
					foreach (FileInfo fileInfo in _filterTemplateRepository.GetFiles(ViewDomainContextEnum.IturAdvancedStatusSearch.ToString() + @"_he"))
					{
						FilterTemplateItemViewModel item = new FilterTemplateItemViewModel(fileInfo, GetFilterDataType);
						_templateItemsTemp.Add(item);
					}
				}
				else
				{
					foreach (FileInfo fileInfo in _filterTemplateRepository.GetFiles(ViewDomainContextEnum.IturAdvancedStatusSearch.ToString() + @"_en"))
					{
						FilterTemplateItemViewModel item = new FilterTemplateItemViewModel(fileInfo, GetFilterDataType);
						_templateItemsTemp.Add(item);
					}
				}
				var listSort = _templateItemsTemp.OrderBy(x => x.DisplayName).Select(e => e).ToList();

				foreach (FilterTemplateItemViewModel it in listSort)
				{
					_templateItems.Add(it);
				}

				//foreach (FileInfo fileInfo in _filterTemplateRepository.GetFiles(ViewDomainContextEnum.IturSearch.ToString()))
				//{
				//	FilterTemplateItemViewModel item = new FilterTemplateItemViewModel(fileInfo, GetFilterDataType);
				//	_templateItems.Add(item);
				//}
			}

			if (isNavigatedFirstTime == false)
			{
				if (String.IsNullOrWhiteSpace(previousBuildTemplate) == false)
				{
					_templateSelected = _templateItems.FirstOrDefault(r => r.Name == previousBuildTemplate);
					//RaisePropertyChanged(() => TemplateSelected);
				}
				else
				{
					_templateSelected = _templateItems.FirstOrDefault(r => r.Name == "");
					InitFilter();
					RaisePropertyChanged(() => TemplateSelected);
				}
			}
		}

		private Type GetFilterDataType()
		{
			IturAdvancedFilterData filter = new IturAdvancedFilterData();
			return filter.GetType(); 
		}


        private void IturFilter(IFilterData filter)
        {
            _filter = filter as IturAdvancedFilterData;
            RaisePropertyChanged(() => IsFilterAnyField);

			if (IsFromFilter == false)				// не из фильтра   (нет фильтра заданного)
			//	&& IsFilterAnyField == true)		// и поля заполнены
			{
				BuildLocations();
				BuildStatusGroup();
				BuildTags();
				BuildTemplates();
			}
			else	//задан фильтр
			{
				_statusSelected = this._statusItems.FirstOrDefault(r => r.Value == "");
				//RaisePropertyChanged(() => StatusSelected);							2021

				_locationSelected = _locationItems.FirstOrDefault(r => r.Value == "");
				//RaisePropertyChanged(() => LocationSelected);							2021

				_tagSelected = _tagItems.FirstOrDefault(r => r.Value == "");
				//RaisePropertyChanged(() => TagSelected);								   2021

				_templateSelected = _templateItems.FirstOrDefault(r => r.Name == "");
				//RaisePropertyChanged(() => TemplateSelected);						//		2021

				TemplateVisible = false;
				LocationList3Visible = false;
				TagList3Visible = false;
				StatusList3Visible = false;

				StatusList4Visible = false;
				TagList4Visible = false;
				LocationList4Visible = false;
			}
  
            if (this._locationSelected == null)
            {
				this._locationSelected = this._locationItems.FirstOrDefault();
               // RaisePropertyChanged(() => LocationSelected);				 2021
            }
			
			if (this._statusSelected == null)
			{
				this._statusSelected = _statusItems.FirstOrDefault();
				//RaisePropertyChanged(() => StatusSelected);			  2021
			}
			
			if (this._tagSelected == null)
			{
				this._tagSelected = _tagItems.FirstOrDefault();
				//RaisePropertyChanged(() => TagSelected);				 2021
			}


			if (this._templateSelected == null)					//2021
			{
				this._templateSelected = _templateItems.FirstOrDefault();
			}

			RaisePropertyChanged(() => TemplateSelected);           //   2021
			Mouse.OverrideCursor = Cursors.Wait;
            BuildOnBackgroundThread();
        }

		private SelectParams BuildSelectParams()
        {
            _logger.Trace("BuildSelectParams");
            SelectParams result = new SelectParams() ;

            {
                result.IsEnablePaging = true;
                result.CountOfRecordsOnPage = this._pageSize;
                result.CurrentPage = this._pageCurrent;
				result.SortParams = String.Empty;
            };

			//===============result - return 	SelectParams с фильтром
			if (_filter != null)
			{
				_filter.ApplyToSelectParams(result, _inventProductRepository, base.GetDbPath);			 //result - return 	SelectParams с фильтром
   
				if (String.IsNullOrWhiteSpace(result.SortParams))
				{
					string iturSort = _userSettingsManager.IturSortGet();
					string sortParams = String.Empty;

					if (!String.IsNullOrWhiteSpace(iturSort))
					{
						if (iturSort == ComboValues.SortItur.NumberValue)
						{
							sortParams = "Number ASC";
						}
						if (iturSort == ComboValues.SortItur.StatusValue)
						{
							sortParams = "StatusIturGroupBit ASC";
						}
						if (iturSort == ComboValues.SortItur.LocationValue)
						{
							sortParams = "LocationCode ASC";
						}
						if (iturSort == ComboValues.SortItur.IturERPCodeValue)
						{
							sortParams = "ERPIturCode ASC";
						}

						result.SortParams = sortParams;
					}
				}
			}

			if (_filter.IsFromFilter == true) return result;		 // если это диалог фильтра
			//===============  end обработки фильтра, если он был, то офильтровал данные в источнике

			// =============== определяем MODE ===============================================	 и меняем 	result - return 	SelectParams
			// =============== MODE  ==  ModePaged
            if (_modeSelectedItem.Value == ComboValues.IturListDetailsMode.ModePaged)
            {
				// =============== определяем GROUPBY == LocationValue  (для MODE  ==  ModePaged) 
                if (_groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.LocationValue)
                {
                    if (!String.IsNullOrWhiteSpace(result.SortParams))
                    {
                        result.SortParams = String.Format("{0}, {1}", "LocationCode ASC", result.SortParams);
                    }
                    else
                    {
                        result.SortParams = "LocationCode ASC";
                    }
                }

				// =============== определяем GROUPBY == StatusValue  (для MODE  ==  ModePaged) 
                if (_groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.StatusValue)
                {
                    if (!String.IsNullOrWhiteSpace(result.SortParams))
                    {
                        result.SortParams = String.Format("{0}, {1}", "StatusIturGroupBit ASC", result.SortParams);
                    }
                    else
                    {
                        result.SortParams = "StatusIturGroupBit ASC";
                    }
                }

				if (_groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.TagValue)
				{
					if (!String.IsNullOrWhiteSpace(result.SortParams))
					{
						result.SortParams = String.Format("{0}, {1}", "Tag ASC", result.SortParams);
					}
					else
					{
						result.SortParams = "Tag ASC";
					}
				}
            }
			// end =============== MODE  ==  ModePaged

			// =============== MODE  ==  ModeLocation
            else if (_modeSelectedItem.Value == ComboValues.IturListDetailsMode.ModeLocation)
            {
                result.IsEnablePaging = false;

                if (result.FilterStringListParams.ContainsKey("LocationCode"))
                {
                    result.FilterStringListParams.Remove("LocationCode");
                }

                if (_locationSelected != null)
                {
					if (String.IsNullOrWhiteSpace(_locationSelected.Value) == false)
					{
						result.FilterParams.Add("LocationCode", new FilterParam()
							{
								Operator = FilterOperator.Equal,
								Value = _locationSelected.Value
							});
					}
                }

				// =============== определяем GROUPBY == StatusValue  (для MODE  ==  ModeLocation) 
				if (this._groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.StatusValue)
				{
					if (result.FilterStringListParams.ContainsKey("StatusIturGroupBit"))
					{
						result.FilterStringListParams.Remove("StatusIturGroupBit");
					}

					if (this._statusSelected != null)
					{
						if (String.IsNullOrWhiteSpace(_statusSelected.Value) == false)
						{
							if (_statusSelected.Value != "7")	    //All
							{
								if (_statusSelected.Value == "8") //"Not Empty"
								{
									List<int> statuses = new List<int>() { 1, 2, 3, 4, 5, 6 }; //Empty == 0

									result.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
									{
										Values = statuses
									});
								}
								else
								{
									int status = 0;
									bool ret = Int32.TryParse(_statusSelected.Value, out status);
									if (ret == true)
									{
										List<int> statuses = new List<int>() { status };
										result.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
										{
											Values = statuses
										});
									}
								}

								if (!String.IsNullOrWhiteSpace(result.SortParams))
								{
									result.SortParams = String.Format("{0}, {1}", "StatusIturGroupBit ASC", result.SortParams);
								}
								else
								{
									result.SortParams = "StatusIturGroupBit ASC";
								}
							}
						}
					}

				}

				// =============== определяем GROUPBY == TagValue  (для MODE  ==  ModeLocation) 
				else if (this._groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.TagValue)
				{
					if (result.FilterStringListParams.ContainsKey("Tag"))
					{
						result.FilterStringListParams.Remove("Tag");
					}

					if (_tagSelected != null)
					{
						if (String.IsNullOrWhiteSpace(_tagSelected.Value) == false)
						{
							result.FilterParams.Add("Tag", new FilterParam()
								{
									Operator = FilterOperator.Equal,
									Value = _tagSelected.Value
								});
						}

					}
				}

		    }
			// end =============== MODE  ==  ModeLocation

			// =============== MODE  ==  ModeTag
			else if (_modeSelectedItem.Value == ComboValues.IturListDetailsMode.ModeTag)
			{
				result.IsEnablePaging = false;

				if (result.FilterStringListParams.ContainsKey("Tag"))
				{
					result.FilterStringListParams.Remove("Tag");
				}

				if (_tagSelected != null)
				{
					if (String.IsNullOrWhiteSpace(_tagSelected.Value) == false)
					{
						result.FilterParams.Add("Tag", new FilterParam()
						{
							Operator = FilterOperator.Equal,
							Value = _tagSelected.Value
						});
					}
				}
	
				// =============== определяем GROUPBY == StatusValue  (для MODE  ==  ModeTag) 
				if (this._groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.StatusValue)
				{
					if (result.FilterStringListParams.ContainsKey("StatusIturGroupBit"))
					{
						result.FilterStringListParams.Remove("StatusIturGroupBit");
					}

					if (this._statusSelected != null)
					{
						if (String.IsNullOrWhiteSpace(_statusSelected.Value) == false)
						{
							if (_statusSelected.Value != "7")	    //All
							{
								if (_statusSelected.Value == "8") //"Not Empty"
								{
									List<int> statuses = new List<int>() { 1, 2, 3, 4, 5, 6 }; //Empty == 0

									result.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
									{
										Values = statuses
									});
								}
								else
								{
									int status = 0;
									bool ret = Int32.TryParse(_statusSelected.Value, out status);
									if (ret == true)
									{
										List<int> statuses = new List<int>() { status };
										result.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
										{
											Values = statuses
										});
									}
								}

								if (!String.IsNullOrWhiteSpace(result.SortParams))
								{
									result.SortParams = String.Format("{0}, {1}", "StatusIturGroupBit ASC", result.SortParams);
								}
								else
								{
									result.SortParams = "StatusIturGroupBit ASC";
								}
							}
						}
					}

				}

				// =============== определяем GROUPBY == LocationValue  (для MODE  ==  ModeTag) 
				else if (this._groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.LocationValue)
				{
					if (result.FilterStringListParams.ContainsKey("LocationCode"))
					{
						result.FilterStringListParams.Remove("LocationCode");
					}

					if (_locationSelected != null)
					{
						if (String.IsNullOrWhiteSpace(_locationSelected.Value) == false)
						{
							result.FilterParams.Add("LocationCode", new FilterParam()
							{
								Operator = FilterOperator.Equal,
								Value = _locationSelected.Value
							});
						}
					}
				}

			}
			// end =============== MODE  ==  ModeTag

			// =============== MODE  ==  ModeStatus
			else if (_modeSelectedItem.Value == ComboValues.IturListDetailsMode.ModeStatus)
			{
				result.IsEnablePaging = false;

				if (result.FilterStringListParams.ContainsKey("StatusIturGroupBit"))
				{
					result.FilterStringListParams.Remove("StatusIturGroupBit");
				}
	   
				if (this._statusSelected != null)
				{
					if (String.IsNullOrWhiteSpace(_statusSelected.Value) == false)
					{
						if (_statusSelected.Value != "7")	  //All
						{
							if (_statusSelected.Value == "8") //"Not Empty"
							{
								List<int> statuses = new List<int>() { 1, 2, 3, 4, 5, 6 }; //Empty == 0

								result.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
								{
									Values = statuses
								});
							}
							else
							{
								int status = 0;
								bool ret = Int32.TryParse(_statusSelected.Value, out status);
								if (ret == true)
								{
									List<int> statuses = new List<int>() { status };
									result.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
									{
										Values = statuses
									});
								}
							}

							if (!String.IsNullOrWhiteSpace(result.SortParams))
							{
								result.SortParams = String.Format("{0}, {1}", "StatusIturGroupBit ASC", result.SortParams);
							}
							else
							{
								result.SortParams = "StatusIturGroupBit ASC";
							}
						}
					}
				}
				// =============== определяем GROUPBY == LocationValue  (для MODE  ==  ModeStatus) 
				if (this._groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.LocationValue)
				{
					if (result.FilterStringListParams.ContainsKey("LocationCode"))
					{
						result.FilterStringListParams.Remove("LocationCode");
					}

					if (_locationSelected != null)
					{
						if (String.IsNullOrWhiteSpace(_locationSelected.Value) == false)
						{
							result.FilterParams.Add("LocationCode", new FilterParam()
							{
								Operator = FilterOperator.Equal,
								Value = _locationSelected.Value
							});
						}
					}
				}
				// =============== определяем GROUPBY == TagValue  (для MODE  ==  ModeStatus) 
				else if (this._groupBySelectedItem != null && _groupBySelectedItem.Value == ComboValues.GroupItur.TagValue)
				{
					if (result.FilterStringListParams.ContainsKey("Tag"))
					{
						result.FilterStringListParams.Remove("Tag");
					}

					if (_tagSelected != null)
					{
						if (String.IsNullOrWhiteSpace(_tagSelected.Value) == false)
						{
							result.FilterParams.Add("Tag", new FilterParam()
							{
								Operator = FilterOperator.Equal,
								Value = _tagSelected.Value
							});
						}

					}
				}

			}
			// end =============== MODE  ==  ModeStatus
            return result;
        }

        private void BuildOnBackgroundThread()
        {
            _logger.Trace("BuildOnBackgroundThread");
			//ItursListBuild();
			Task.Factory.StartNew(ItursListBuild).LogTaskFactoryExceptions("BuildOnBackgroundThread");
        }

        private void ItursListBuild()
        {
            SelectParams selectParams = null;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                selectParams = BuildSelectParams();

                Iturs iturs;
				if (Filter != null &&
					((Filter.IsLocation && !Filter.Locations.Any()) ||
					(Filter.IsTag && !Filter.Tags.Any()) ||
					 (Filter.IsStatus && !Filter.Statuses.Any())))
				{
					iturs = new Iturs();
				}
				else
				{
					iturs = this._iturRepository.GetIturs(selectParams, base.GetDbPath);
				}

                Dictionary<string, int> totalLocationCache = new Dictionary<string, int>();
				Dictionary<string, int> emptyLocationCache = new Dictionary<string, int>();
				Dictionary<string, int> countedLocationCache = new Dictionary<string, int>();
				Dictionary<string, int> disabledLocationCache = new Dictionary<string, int>();
				Dictionary<string, double> doneLocationCache = new Dictionary<string, double>();
				

				Dictionary<int, int> totalStatusCache = new Dictionary<int, int>();
				Dictionary<string, int> totalTagCache = new Dictionary<string, int>();

                bool isShowERP = _userSettingsManager.ShowIturERPGet();

				int totalIturEmptyLoc = iturs.Count(r => r.LocationCode == "");
				int totalIturEmptyTag = iturs.Count(r => r.Tag == "");

				ObservableCollection<IturDashboardItemViewModel> uiItems = new ObservableCollection<IturDashboardItemViewModel>();
                foreach (Itur itur in iturs)
                {
                    Location location = this._allLocations.FirstOrDefault(r => r.Code == itur.LocationCode);
                    StatusIturGroup statusGroup = GetStatusIturGroupByItur(itur);

                    string color = String.Empty;

					// ====== Total Location
                    int totalIturSuchLoc = 0;
                    if (String.IsNullOrWhiteSpace(itur.LocationCode) == true)
                    {
						totalIturSuchLoc = totalIturEmptyLoc; //iturs.Count(r => r.LocationCode == itur.LocationCode);
                    }
                    else
                    {
                        if (totalLocationCache.ContainsKey(itur.LocationCode) == false)
                        {
                            totalLocationCache[itur.LocationCode] = iturs.Count(r => r.LocationCode == itur.LocationCode);
                        }
                        totalIturSuchLoc = totalLocationCache[itur.LocationCode];
                    }

					// ====== Empty Location
					int emptyIturSuchLoc = 0;
					if (String.IsNullOrWhiteSpace(itur.LocationCode) == true)
					{
						emptyIturSuchLoc = 0; //iturs.Count(r => r.LocationCode == itur.LocationCode);
					}
					else
					{
						if (emptyLocationCache.ContainsKey(itur.LocationCode) == false)
						{
							emptyLocationCache[itur.LocationCode] = iturs.Count(r => r.LocationCode == itur.LocationCode && r.StatusIturGroupBit == 0);         // Empty = 0
						}
						emptyIturSuchLoc = emptyLocationCache[itur.LocationCode];
					}


//Counted =>
//OneDocIsApprove = 1,
//AllDocsIsApprove = 2,
//NotApprove = 3,
//Error = 6
				// ====== Counted Itur in location
					int countedIturSuchLoc = 0;
					if (String.IsNullOrWhiteSpace(itur.LocationCode) == true)
					{
						countedIturSuchLoc = 0; //iturs.Count(r => r.LocationCode == itur.LocationCode);
					}
					else
					{
						if (countedLocationCache.ContainsKey(itur.LocationCode) == false)
						{
							var it = iturs.Where(r => r.LocationCode == itur.LocationCode).Select(r => r).ToList();
							countedLocationCache[itur.LocationCode] = 0;
							if (it != null)
							{
								countedLocationCache[itur.LocationCode] = it.Count(r => r.StatusIturGroupBit == 1 || r.StatusIturGroupBit == 2 || r.StatusIturGroupBit == 3 || r.StatusIturGroupBit == 6);     //1,2,3,6,
							}
						}
						countedIturSuchLoc = countedLocationCache[itur.LocationCode];
					}

					//Disabled => DisableAndNoOneDoc = 4,
					//DisableWithDocs = 5,
					// ====== disabled Itur in location
					int disabledIturSuchLoc = 0;
					if (String.IsNullOrWhiteSpace(itur.LocationCode) == true)
					{
						disabledIturSuchLoc = 0; //iturs.Count(r => r.LocationCode == itur.LocationCode);
					}
					else
					{
						if (disabledLocationCache.ContainsKey(itur.LocationCode) == false)
						{
							var it = iturs.Where(r => r.LocationCode == itur.LocationCode).Select(r => r).ToList();
							disabledLocationCache[itur.LocationCode] = 0;
							if (it != null)
							{
								disabledLocationCache[itur.LocationCode] = it.Count(r => r.StatusIturGroupBit == 5 || r.StatusIturGroupBit == 4);     //5,4
							}
						}
						disabledIturSuchLoc = disabledLocationCache[itur.LocationCode];
					}


					// ====== 	done % Itur in location
					double doneIturSuchLoc = 0;
					if (String.IsNullOrWhiteSpace(itur.LocationCode) == true)
					{
						doneIturSuchLoc = 0; //iturs.Count(r => r.LocationCode == itur.LocationCode);
					}
					else
					{
						if (doneLocationCache.ContainsKey(itur.LocationCode) == false)
						{
							doneLocationCache[itur.LocationCode] = 0;
		
							
							if (emptyLocationCache.ContainsKey(itur.LocationCode) == false)
							{
								emptyLocationCache[itur.LocationCode] = 0;
							}
							if (countedLocationCache.ContainsKey(itur.LocationCode) == false) 
							{
								countedLocationCache[itur.LocationCode] = 0;
							}
							if (disabledLocationCache.ContainsKey(itur.LocationCode) == false)
							{
								disabledLocationCache[itur.LocationCode] = 0;
							}
							//([Counted] + [Disabled Iturs]) / [Total]
							if (totalLocationCache.ContainsKey(itur.LocationCode) == true)
							{
								if (totalLocationCache[itur.LocationCode] != 0)
								{
									double total = (double)totalLocationCache[itur.LocationCode];
									int counted = countedLocationCache[itur.LocationCode] + disabledLocationCache[itur.LocationCode];
									double done = counted * 100 / total;
									doneLocationCache[itur.LocationCode] = Math.Round(done, 2);
								}
							}
						}
						doneIturSuchLoc = doneLocationCache[itur.LocationCode];
					}

					// ====== StatusIturGroupBit
					int totalIturSuchStat = 0;
                    if (statusGroup != null)
                    {
                        color = UtilsStatus.FromStatusGroupBitToColor(this._statusGroupRepository.BitStatusIturGroupEnumDictionary,
                                                                    statusGroup.Bit, this._userSettingsManager);

                        if (totalStatusCache.ContainsKey(itur.StatusIturGroupBit) == false)
                        {
                            totalStatusCache[itur.StatusIturGroupBit] = iturs.Count(r =>
                                                         {
                                                             StatusIturGroup group = GetStatusIturGroupByItur(r);
                                                             if (group == null)
                                                                 return false;

                                                             return group.Code == statusGroup.Code;
                                                         });
                        }

                        totalIturSuchStat = totalStatusCache[itur.StatusIturGroupBit];
                    }

					// ====== Tag
					int totalIturSuchTag = 0; //Todo

					if (String.IsNullOrWhiteSpace(itur.Tag) == true)
					{
						totalIturSuchTag = totalIturEmptyTag;
					}
					else
					{
						if (totalTagCache.ContainsKey(itur.Tag) == false)
						{
							totalTagCache[itur.Tag] = iturs.Count(r => r.Tag == itur.Tag);
						}
						totalIturSuchTag = totalTagCache[itur.Tag];
					}

					
			 IturDashboardItemViewModel iturViewModel = new IturDashboardItemViewModel(itur, location, statusGroup,
																							  totalIturSuchLoc, totalIturSuchStat, 
																							  totalIturSuchTag, emptyIturSuchLoc, countedIturSuchLoc, disabledIturSuchLoc, doneIturSuchLoc);

                    iturViewModel.StatusColor = color;
                    iturViewModel.ShowERP = isShowERP;
                    uiItems.Add(iturViewModel);
                }

				int delay = this._userSettingsManager.DelayGet();

                Utils.RunOnUI(() =>
                                  {
                                      this._items.Clear();
									  //this.Items = uiItems;
									  int i = 0;
									  foreach (IturDashboardItemViewModel item in uiItems)
									  {
										  if (i < delay)
										  {
											  this._items.Add(item);
										  }
										  else
										  {
											  break;
											}
										  i++;
									  }

                                      this.ItemsTotal = iturs.TotalCount;
                                  });

                if ((iturs.TotalCount > 0) && (iturs.Count == 0))	//do not show empty space - move to previous page              
                {
                    Utils.RunOnUI(() => this.PageCurrent = this._pageCurrent - 1);
                }

                Utils.RunOnUI(AddGrouping);

                stopwatch.Stop();
                Debug.Print("Iturs collection build: " + stopwatch.Elapsed.TotalSeconds.ToString());

                Utils.RunOnUI(() => Mouse.OverrideCursor = null);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("ItursListBuild", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                if (selectParams != null)
                    _logger.Error("SelectParams: {0}", selectParams.ToString());
            }
        }

        StatusIturGroup GetStatusIturGroupByItur(Itur itur)
        {
            return this._allStatusGroups.Values.FirstOrDefault(r => r.Bit == itur.StatusIturGroupBit);
        }

		string GetStatusGroupcolor(IturStatusGroupEnum statusIturGroup)
		{
		    int bit =  (int)statusIturGroup;
			string color = UtilsStatus.FromStatusGroupBitToColor(this._statusGroupRepository.BitStatusIturGroupEnumDictionary,
																 bit, this._userSettingsManager);
			return color;
		}

        void AddGrouping()
        {
            if (_groupBySelectedItem == null) return;

            if (_groupBySelectedItem.Value == ComboValues.GroupItur.EmptyValue)
            {
                if (this.ItemsView.GroupDescriptions.Count > 0)
                    this.ItemsView.GroupDescriptions.Clear();

                //this.ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("IturItemGroupEmpty"));
                this.IsGrouping = false;
                this._isGroupingLocation = false;
				this._isGroupingTag = false;
                this._isGroupingStatus = false;
            }

            if (_groupBySelectedItem.Value == ComboValues.GroupItur.LocationValue)
            {

				if (this.ItemsView.GroupDescriptions.Count == 0)
				{
					this.ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("IturItemGroupLocation"));
				}
				else
				{
					if (this._isGroupingLocation == false)
					{
						this.ItemsView.GroupDescriptions.Clear();
						this.ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("IturItemGroupLocation"));
					}
				}

                this.IsGrouping = true;
                this._isGroupingLocation = true;
				this._isGroupingTag = false;
                this._isGroupingStatus = false;
            }

            if (_groupBySelectedItem.Value == ComboValues.GroupItur.StatusValue)
            {

                if (this.ItemsView.GroupDescriptions.Count == 0)
                {
                    this.ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("IturItemGroupStatus"));
                }
                else
                {
                    if (this._isGroupingStatus == false)
                    {
                        this.ItemsView.GroupDescriptions.Clear();
                        this.ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("IturItemGroupStatus"));
                    }
                }
                this.IsGrouping = true;
                this._isGroupingLocation = false;
				this._isGroupingTag = false;
                this._isGroupingStatus = true;
            }

			if (_groupBySelectedItem.Value == ComboValues.GroupItur.TagValue)
			{

				if (this.ItemsView.GroupDescriptions.Count == 0)
				{
					this.ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("IturItemGroupTag"));
				}
				else
				{
					if (this._isGroupingTag == false)
					{
						this.ItemsView.GroupDescriptions.Clear();
						this.ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("IturItemGroupTag"));
					}
				}
				this.IsGrouping = true;
				this._isGroupingLocation = false;
				this._isGroupingTag = true;
				this._isGroupingStatus = false;
			}

        }

        private void IturimAddEditCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.History);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(query, GetHistoryAuditConfig());
            UtilsNavigate.IturimAddEditDeleteOpen(this._regionManager, query);
        }

        private void ImportFromPdaAutoCommandExeucted()
        {
			using (new CursorWait())
			{
				UriQuery uriQuery = new UriQuery();
				Utils.AddContextToQuery(uriQuery, CBIContext.History);
				Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
				Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
				uriQuery.Add(Common.NavigationSettings.AutoStartImportPda, String.Empty);
				UtilsNavigate.ImportFromPdaOpen(this._regionManager, uriQuery);
			}
        }

        private void ImportFromPdaCommandExeucted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportFromPdaOpen(this._regionManager, uriQuery);
        }


		private void DevicePDAStatisticCommandExeucted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.DevicePDAOpen(this._regionManager, uriQuery);
        }


		private void DeviceWorkerPDAStatisticCommandExeucted()
		{
			UriQuery uriQuery = new UriQuery();
			Utils.AddContextToQuery(uriQuery, CBIContext.History);
			Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
			Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
			UtilsNavigate.DeviceWorkerPDAOpen(this._regionManager, uriQuery);
		}

		private void LocationAdded(Location location)
        {
            this._allLocations = this._locationRepository.GetLocations(base.GetDbPath);

            BuildLocations();
        }

        private void RefreshCommandExecuted()
        {
            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Title = String.Empty;
            notification.Settings = this._userSettingsManager;
            notification.Content = String.Format(Localization.Resources.Msg_Recalculate_Itur_Status);
            this._yesNoRequest.Raise(notification, r =>
             {
                 if (r.IsYes == true)
                 {
                     BusyText = Localization.Resources.View_IturListDetails_busyContent;
                     IsBusy = true;
					 Task.Factory.StartNew(RefreshAction).LogTaskFactoryExceptions("RefreshCommandExecuted");
                 }
             });
        }

        private void RefreshAction()
        {
            this._iturRepository.RefillApproveStatusBit(base.GetDbPath);
            ItursListBuild();
            ProgressItemsBuild();

            Utils.RunOnUI(() => IsBusy = false);
        }

		private void RefreshActionDocuments()
        {
           // this._iturRepository.RefillApproveStatusBit(base.GetDbPath);

			this._iturRepository.RefillApproveStatusBit(RefreshDocumentCodeList, new List<string>(), base.GetDbPath);
            ItursListBuild();
            ProgressItemsBuild();

            Utils.RunOnUI(() => IsBusy = false);
        }
		

        private void DashboardCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.HomeDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        private void ReportsCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Iturs);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            //SelectParams sp = BuildSelectParams();
            SelectParams sp = new SelectParams();
            sp.IsEnablePaging = false;
            sp.SortParams = String.Empty;

            Utils.AddSelectParamsToQuery(query, sp);

            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void InitFilter()
        {
            _filter = new IturAdvancedFilterData();
            _filter.Field = ComboValues.FindItur.FilterIturNumber;
            _filter.InventProductBarcode = String.Empty;
            _filter.InventProductMakat = String.Empty;
            _filter.InventProductName = String.Empty;
            _filter.IsInventProduct = false;
            _filter.IsInventProductExpanded = true;
            _filter.IsLocation = false;
			_filter.IsTag = false;
			_filter.IsFromFilter = false;
            _filter.IsLocationExpanded = true;
			_filter.IsTagExpanded = true;
            _filter.IsStatus = false;
            _filter.IsStatusExpanded = true;
            _filter.Text = String.Empty;
            _filter.Statuses = null; //same as all checked
            _filter.Locations = null; //same as all checked
			_filter.Tags = null; //same as all checked
			_filter.Field = _userSettingsManager.IturFilterSelectedGet();
			_filter.SortField = "";
			_filter.SortDirection = enSortDirection.ASC;
			try
			{
				_filter.SortField = _userSettingsManager.IturFilterSortSelectedGet();
				_filter.SortDirection = _userSettingsManager.IturFilterSortAZSelectedGet() == ComboValues.FindIturSortAZ.SortDESC ? enSortDirection.DESC : enSortDirection.ASC;
			}
			catch { }
        }

        private void SortConfChanged(object o)
        {
            _isNavigatedFirstTime = true;
        }

        private void GroupConfChanged(object o)
        {
            _isNavigatedFirstTime = true;
        }

        private void ChangeLocationCommandExecuted()
        {
			if (this._selectedItems == null) return;
            Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

            IturLocationChangeEventPayload payload = new IturLocationChangeEventPayload();
            payload.Iturs = iturs;
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            this._eventAggregator.GetEvent<IturLocationChangeEvent>().Publish(payload);
        }

		private void ChangeIturPrefixCommandExecuted()
		{
			if (this._selectedItems == null) return;

			Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

			IturPrefixChangeEventPayload payload = new IturPrefixChangeEventPayload();
			payload.Iturs = iturs;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<IturPrefixChangeEvent>().Publish(payload);
		}

		private void ChangeIturNameCommandExecuted()
		{
			if (this._selectedItems == null) return;

			Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

			IturNameChangeEventPayload payload = new IturNameChangeEventPayload();
			payload.Iturs = iturs;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<IturNameChangeEvent>().Publish(payload);
		}

        private void LocationChanged(IturLocationChangedEventPayload payload)
        {
            using (new CursorWait())
            {
				//Iturs iturs = new Iturs();
				//foreach (Itur itur in payload.Iturs)
				//{
				//	//itur.LocationID = payload.Location.ID;
				//	itur.LocationCode = payload.Location.Code;
				//	itur.Name1 = payload.Location.Name;
				//	iturs.Add(itur);
				//}

				//this._iturRepository.Update(iturs, base.GetDbPath);

				Mouse.OverrideCursor = Cursors.Wait;
                this.BuildOnBackgroundThread();
            }
        }

		private void QuantityEditChanged(IturQuantityEditChangedEventPayload payload)
		{
			Task.Factory.StartNew(() => ProgressItemsBuild()).LogTaskFactoryExceptions("QuantityEditChanged");
		}

		private void IturNameChanged(IturNameChangedEventPayload payload)
		{
			using (new CursorWait())
			{
				Iturs iturs = new Iturs();
				if (payload.Iturs.Count > 0)
				{
					Itur itur = payload.Iturs[0];
					itur.Name = payload.Name;
					itur.ERPIturCode = "";
					if (payload.ERPCode != null)	itur.ERPIturCode = payload.ERPCode.CutLength(249);
					iturs.Add(itur);
				}
				//foreach (Itur itur in payload.Iturs)
				//{
				//	itur.Name = payload.Name;
				//	iturs.Add(itur);
				//}

				this._iturRepository.Update(iturs, base.GetDbPath);

				Mouse.OverrideCursor = Cursors.Wait;
				this.BuildOnBackgroundThread();
			}
		}
	

		// Фоновая печать по IturCodeList ***
        private void RunPrintReportByIturCodeList(List<string> iturCodes, Count4U.GenerationReport.Report report)
        {
            if (iturCodes == null || iturCodes.Any() == false) return;
            if (report == null) return;

            Itur itur = null;
            DocumentHeader documentHeader = null;
            Location location = null;
			InventProductSimpleFilterData filterData = new InventProductSimpleFilterData();
			filterData.IturCode = iturCodes.JoinRecord(",");

            if (iturCodes.Count == 1)
            {
                IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
                itur = iturRepository.GetIturByCode(iturCodes.First(), base.GetDbPath);

                if (itur != null)
                {
                    ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
                    location = locationRepository.GetLocationByCode(itur.LocationCode, base.GetDbPath);
                }
            }

            try
            {
                GenerateReportArgs args = new GenerateReportArgs();
                args.Customer = base.CurrentCustomer;
                args.Branch = base.CurrentBranch;
                args.Inventor = base.CurrentInventor;
                args.DbPath = base.GetDbPath;
                args.Report = report;
                args.Doc = documentHeader;
				args.Device = null;
                args.ViewDomainContextType = ViewDomainContextEnum.ItursIturDoc;
                args.Itur = itur;
				args.FilterData = filterData;
                args.Location = location;

                SelectParams spDoc = new SelectParams();
                spDoc.FilterStringListParams.Add("IturCode", new FilterStringListParam()
                    {
                        Values = iturCodes
                    });
                args.SelectParams = spDoc;

                ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
                printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });

            }
            catch (Exception exc)
            {
				_logger.ErrorException("RunPrintReportByIturCodeList", exc);
            }
        }
		////////////
		// Чтение из файла конфигурации и подготовка списка для контекстного меню  печати отчетов
		//private List<ReportInfo> BuildContextMenuReportInfoList(string reportN)
		//{
		//	//this._reports.Clear();
		//	List<ReportInfo> reportInfoList = new List<ReportInfo>();

		//	if (File.Exists(this._reportIniFile) == false)
		//	{
		//		this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentInventor.Code);
		//	}
		//	if (File.Exists(this._reportIniFile) == false) return reportInfoList;

		//	const string Context = ReportIniProperty.ContextMenuIturList;
		//	const string PrintKey = ReportIniProperty.Print;
		//	const string SecondPrintKey = ReportIniProperty.SecondPrint;
		//	const string SelectReportByKey = ReportIniProperty.SelectReportBy;
		//	const string ShowInContextMenu = ReportIniProperty.ShowInContextMenu;
		//	const string ReportNumInContextMenu = ReportIniProperty.ReportNumInContextMenu;

		//	foreach (IniFileData iniFileData in this._iniFileParser.Get(this._reportIniFile))
		//	{
		//		string reportCode = iniFileData.SectionName;

		//		if (String.IsNullOrWhiteSpace(reportCode) == true) continue;

		//		bool isContextMenuIturList = false;
		//		if (iniFileData.Data.ContainsKey(Context) == true)
		//		{
		//			isContextMenuIturList = iniFileData.Data[Context] == "1";
		//		}
		//		if (isContextMenuIturList == false) continue;

		//		bool isReportNumInContextMenu = false;
		//		if (iniFileData.Data.ContainsKey(ReportNumInContextMenu) == true)
		//		{
		//			isReportNumInContextMenu = iniFileData.Data[ReportNumInContextMenu] == reportN;
		//		}
		//		if (isContextMenuIturList == false) continue;

		//		string selectReportBy = "";
		//		if (iniFileData.Data.ContainsKey(SelectReportByKey) == true)
		//		{
		//			selectReportBy = iniFileData.Data[SelectReportByKey];
		//		}

		//		bool isPrint = false;
		//		if (iniFileData.Data.ContainsKey(PrintKey))
		//		{
		//			isPrint = iniFileData.Data[PrintKey] == "1";
		//		}

		//		bool isSecondPrint = false;
		//		if (iniFileData.Data.ContainsKey(SecondPrintKey) == true)
		//		{
		//			isSecondPrint = iniFileData.Data[SecondPrintKey] == "1";
		//		}

		//		bool isShowInContextMenu = false;
		//		if (iniFileData.Data.ContainsKey(ShowInContextMenu) == true)
		//		{
		//			isShowInContextMenu = iniFileData.Data[ShowInContextMenu] == "1";
		//		}
			

		//		string reportCodeBracket = String.Format("[{0}]", reportCode);
		//		Count4U.GenerationReport.Reports reports = this._reportRepository.GetReportByCodeReport(reportCodeBracket);
		//		Count4U.GenerationReport.Report report = null;
		//		if (reports != null)
		//		{
		//			report = reports.FirstOrDefault();
		//		}

		//		if (report == null)
		//		{
		//			_logger.Warn("BuildReports: Report is missing{0}", reportCode);
		//			continue;
		//		}

		//		ReportInfo item = new ReportInfo(this._reportRepository);
		//		item.ReportCode = reportCodeBracket;
		//		item.Print = isPrint;
		//		item.Print2 = isSecondPrint;
		//		item.param2 = selectReportBy;
		//		reportInfoList.Add(item);
		//	}
		//	return reportInfoList;
		//}
		// ----------ФОНОВАЯ ПЕЧАТЬ
		// фоновая печать из Main form по списку отчетов
		// из попап меню

		private void IturListPrintByIndexCommandExecuted(string reportN)
		{
			if (this._selectedItems == null) return;

			List<string> iturCodesList = _selectedItems.Select(r => r.Code).ToList();

			IReportInfoRepository reportInfoRepository= this._serviceLocator.GetInstance<IReportInfoRepository>();
		
			List<ReportInfo> reportInfoList = reportInfoRepository.BuildContextMenuReportInfoList(base.CurrentInventor.Code, this._reportIniFile, reportN);
			reportInfoList.ForEach(x => x.param1 = iturCodesList);
			this.PrintReportByIturCodesByReportInfo(reportInfoList);

			//Test
			 //    ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
				//Thread.Sleep(1000);
				//printQueue.Stop();

		}

		// not use
		private void IturListExportErpByConfigCommandExecuted(string reportN)
		{
			if (this._selectedItems == null) return;

			List<string> iturCodesList = _selectedItems.Select(r => r.Code).ToList();

			//IReportInfoRepository reportInfoRepository = this._serviceLocator.GetInstance<IReportInfoRepository>();

			//List<ReportInfo> reportInfoList = reportInfoRepository.BuildContextMenuReportInfoList(base.CurrentInventor.Code, this._reportIniFile, reportN);
			//reportInfoList.ForEach(x => x.param1 = iturCodesList);
			//this.PrintReportByIturCodesByReportInfo(reportInfoList);

		}

		//old
		//private void IturListPrintCommandExecuted()
		//{
		//	List<string> iturCodesList = _selectedItems.Select(r => r.Code).ToList();

		//	IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();
		//	Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrint(ViewDomainContextEnum.ItursItur);

		//	//foreach (string code in iturCodesList)  ???
		//	//{
		//	//	var list = new List<string>() { code };
		//	//	RunPrintReportByIturCodeList(list, report);
		//	//}
		//	RunPrintReportByIturCodeList(iturCodesList, report);
		//}

		//old
		// из попап меню
		//private void IturListPrintIS0155CommandExecuted()
		//{
		//	List<string> iturCodesList = _selectedItems.Select(r => r.Code).ToList();

		//	IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();
		//	Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrint(ViewDomainContextEnum.ItursFamilyPrintMenu); //todo

		//	RunPrintReportByIturCodeList(iturCodesList, report);
		//}

		//old
		// из попап меню
		//private void IturListPrintIS0160CommandExecuted()
		//{
		//	List<string> iturCodesList = _selectedItems.Select(r => r.Code).ToList();

		//	IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();
		//	Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrint(ViewDomainContextEnum.ItursPrintMenu60); //todo

		//	RunPrintReportByIturCodeList(iturCodesList, report);
		//}


		// кнопка меню dissable по итурам(из диалога, который вызывается в меню Main form)
		private void SetDissableItursCommandExecuted()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			Utils.AddContextToDictionary(dic, base.Context);
			Utils.AddDbContextToDictionary(dic, base.CBIDbContext);

			object result = _modalWindowLauncher.StartModalWindow(Common.ViewNames.IturSelectDissableView, Common.Constants.WindowTitles.SetDissableIturs, 500, 337, ResizeMode.NoResize, dic);

			DissableItursInfo dissableItursInfo = result as DissableItursInfo;

			using (new CursorWait())
			{
				this.SetDissableIturs(dissableItursInfo);
				this.BuildOnBackgroundThread();
				this.ProgressItemsBuild();
			}
		}

		

		private void AddEditLocationIturCommandExecuted()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			Utils.AddContextToDictionary(dic, base.Context);
			Utils.AddDbContextToDictionary(dic, base.CBIDbContext);

			object result = _modalWindowLauncher.StartModalWindow(Common.ViewNames.LocationItursChangeView, Common.Constants.WindowTitles.LocationItursChange, 620, 450, ResizeMode.NoResize, dic);

			////List<string> iturCodes = result as List<string>;
			//List<ReportInfo> reportInfoList = result as List<ReportInfo>;

			//PrintReportByLocationCodeByReportInfo(reportInfoList);
		}

		private void SetDissableIturs(DissableItursInfo dissableItursInfo)
		{
			if (dissableItursInfo == null)
				return;
			if(dissableItursInfo.IturCodes.Count == 0 )
				return;
			 SelectParams sp = new SelectParams();
	
                sp.FilterStringListParams.Add("IturCode", new FilterStringListParam()
                {
                    Values = dissableItursInfo.IturCodes
                });


            Iturs dbIturs = this._iturRepository.GetIturs(sp, base.GetDbPath);

			//	this._iturRepository.SetDisabledStatusBitByIturCode(dbIturs, dissableItursInfo.Dissable, base.GetDbPath);
			Iturs updatedAllItursInDB = this._iturRepository.SwitchDisabledStatusBitByIturCode(dbIturs, dissableItursInfo.Dissable, base.GetDbPath);
			this._importIturBlukRepository.ClearIturs(base.GetDbPath);
			this._importIturBlukRepository.InsertItursFromList(base.GetDbPath, updatedAllItursInDB.ToList());

		}

		// кнопка меню печать1 по итурам(из диалога, который вызывается в меню Main form)
		//IT2-03
        private void PrintReportCommandExecuted()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            Utils.AddContextToDictionary(dic, base.Context);
            Utils.AddDbContextToDictionary(dic, base.CBIDbContext);

			object result = _modalWindowLauncher.StartModalWindow(Common.ViewNames.IturSelectView, Common.Constants.WindowTitles.SetDissableIturs, 620, 450, ResizeMode.NoResize, dic);

			List<ReportInfo> reportInfoList = result as List<ReportInfo>;

			this.PrintReportByIturCodesByReportInfo(reportInfoList);
	    }


		//---------	 I - by IturCodes
		// reportInfo.param1  //List<string> iturCodes  
		private void PrintReportByIturCodesByReportInfo(List<ReportInfo> reportInfoList)
		{
			if (reportInfoList == null)
				return;

			IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();

			//Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrint(ViewDomainContextEnum.ItursItur);

			foreach (var reportInfo in reportInfoList)  //для 1 принтера
			{
				if (reportInfo.param1 != null) //List<string> iturCodes  
				{
					List<string> iturCodes = reportInfo.param1 as List<string>;
					Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrintByReportCode(reportInfo.ReportCode);
					string selectReportBy = reportInfo.param2 as string;

					if (selectReportBy == SelectReportByType.IturCodes)
					{
						foreach (string iturCode in iturCodes)
						{
	
							RunPrintReportByIturCode(iturCode, report, reportInfo.Print);
							if (string.IsNullOrWhiteSpace(this._userSettingsManager.PrinterGet()) == false)
							{
								RunPrintReportByIturCode(iturCode, report, reportInfo.Print2, this._userSettingsManager.PrinterGet());
							}
						}
					}
				}
			}
		}

		//кнопка печасть2 по Location (из диалога, который вызывается в меню Main form)
		private void PrintReportByLocationCodeCommandExecuted()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			Utils.AddContextToDictionary(dic, base.Context);
			Utils.AddDbContextToDictionary(dic, base.CBIDbContext);

			object result = _modalWindowLauncher.StartModalWindow(Common.ViewNames.LocationCodeSelectView, Common.Constants.WindowTitles.LocationSelect, 620, 450,  ResizeMode.NoResize, dic);

			//List<string> iturCodes = result as List<string>;
			List<ReportInfo> reportInfoList = result as List<ReportInfo>;

			PrintReportByLocationCodeByReportInfo(reportInfoList);
		}

		//----------   II - By Location Code
		//----------  reportInfo.param2 //	selectReportBy	 =	IturCodes |  LocationCode | LocationTag
		//----------  reportInfo.param1	 //	 LocationCode
		private void PrintReportByLocationCodeByReportInfo(List<ReportInfo> reportInfoList)
		{
			if (reportInfoList == null)
				return;

			IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();

			foreach (var reportInfo in reportInfoList)   //для 1 принтера
			{
				if (reportInfo.param1 != null) //LocationCode 
				{
					string param1 = reportInfo.param1 as string;
					
					Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrintByReportCode(reportInfo.ReportCode);
					string selectReportBy = reportInfo.param2 as string;

					if (selectReportBy == SelectReportByType.IturCodes)//if (reportInfo.ReportCode == "[Rep-IT2-03]")
					{
						List<string> iturCodes = this._iturRepository.GetIturCodesForLocationCode(param1, base.GetDbPath);		//param1 == LocatoinCode
						foreach (string iturCode in iturCodes)
						{
							// так для IS1- 70  	//var list = new List<string>() { iturCode };	//RunPrintReport(list, report);//так для IT2-03
							RunPrintReportByIturCode(iturCode, report, reportInfo.Print);
							if (string.IsNullOrWhiteSpace(_userSettingsManager.PrinterGet()) == false)
							{
								this.RunPrintReportByIturCode(iturCode, report, reportInfo.Print2, _userSettingsManager.PrinterGet());
							}
						}
					}
					else if (selectReportBy == SelectReportByType.LocationCode)	//else if (reportInfo.ReportCode == "[Rep-IS1-77L]")
					{
						this.RunPrintReportByLocationCode(param1, report, reportInfo.Print);													  //param1 == LocatoinCode
						if (string.IsNullOrWhiteSpace(_userSettingsManager.PrinterGet()) == false)
						{
							this.RunPrintReportByLocationCode(param1, report, reportInfo.Print2, this._userSettingsManager.PrinterGet());
						}
					}
					else if (selectReportBy == SelectReportByType.LocationTag)
					{
						bool include = reportInfo.param3 != null ? Convert.ToBoolean(reportInfo.param3) : false;
						this.RunPrintReportByLocationTag(param1, include, report, reportInfo.Print);													  //param1 == LocatoinCode
						if (string.IsNullOrWhiteSpace(_userSettingsManager.PrinterGet()) == false)
						{
							this.RunPrintReportByLocationTag(param1, include, report, reportInfo.Print2, this._userSettingsManager.PrinterGet());
						}
					}
					else if (selectReportBy == SelectReportByType.IturTag)
					{
						bool include = reportInfo.param3 != null ? Convert.ToBoolean(reportInfo.param3) : false;
						this.RunPrintReportByIturTag(param1, include, report, reportInfo.Print);													  //param1 == LocatoinCode
						if (string.IsNullOrWhiteSpace(_userSettingsManager.PrinterGet()) == false)
						{
							this.RunPrintReportByIturTag(param1, include, report, reportInfo.Print2, this._userSettingsManager.PrinterGet());
						}
					}

					else if (selectReportBy == SelectReportByType.SectionTag)
					{
						bool include = reportInfo.param3 != null ? Convert.ToBoolean(reportInfo.param3) : false;
						this.RunPrintReportBySectionTag(param1, include, report, reportInfo.Print);													  //param1 == SectionCode
						if (string.IsNullOrWhiteSpace(_userSettingsManager.PrinterGet()) == false)
						{
							this.RunPrintReportBySectionTag(param1, include, report, reportInfo.Print2, this._userSettingsManager.PrinterGet());
						}
					}
				}
			}//для 1 принтера
		}


		//кнопка печасть3 по Tag (из диалога, который вызывается в меню Main form)
		private void PrintReportByTagCommandExecuted()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			Utils.AddContextToDictionary(dic, base.Context);
			Utils.AddDbContextToDictionary(dic, base.CBIDbContext);

			object result = _modalWindowLauncher.StartModalWindow(Common.ViewNames.TagSelectView, Common.Constants.WindowTitles.TagSelect, 620, 450, ResizeMode.NoResize, dic);

			//List<string> iturCodes = result as List<string>;
			List<ReportInfo> reportInfoList = result as List<ReportInfo>;

			PrintReportByLocationCodeByReportInfo(reportInfoList);
		}

		//? Печать из формы печати "отчета из контестка IT2" 
		// IT2-03 из формы печати по локейшену
		private void RunPrintReportByIturCode(string iturCode, Count4U.GenerationReport.Report report, bool print = true, string printerName = "")
		{
			if (print == false) return;
			if (string.IsNullOrWhiteSpace(iturCode) == true) return;	
			ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
			if (_cancellationTokenSource.IsCancellationRequested)
			{
				printQueue.Stop();
				IsBusy = false;
				return;
			}

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Itur itur = iturRepository.GetIturByCode(iturCode, base.GetDbPath);

			DocumentHeader documentHeader = null;
			Location location = null;
			if (itur != null)
			{
				ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
				location = locationRepository.GetLocationByCode(itur.LocationCode, base.GetDbPath);
			}

			try
			{
				GenerateReportArgs args = new GenerateReportArgs();
				args.Customer = base.CurrentCustomer;
				args.Branch = base.CurrentBranch;
				args.Inventor = base.CurrentInventor;
				args.DbPath = base.GetDbPath;
				args.Report = report;
				args.Doc = documentHeader;
				args.Device = null;
				args.ViewDomainContextType = ViewDomainContextEnum.ItursIturDoc;
				args.Itur = itur;
				args.Location = location;
				args.PrinterName = printerName;

				SelectParams selectParams = new SelectParams();
				List<string> searchItur = new List<string> { iturCode };
				selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam()
				{
					Values = searchItur
				});
				args.SelectParams = selectParams;

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					printQueue.Stop();
					IsBusy = false;
					return;
				}
				else
				{
					printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
				}
				//this._generateReportRepository.RunSaveReport(args, @"C:\Temp\testReport\output1.txt", ReportFileFormat.Excel);
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunPrintReportByIturCode", exc);
			}
		}

		// Фоновая печать по LocationCode ***
		private void RunPrintReportByLocationCode(string locationCode, Count4U.GenerationReport.Report report, bool print = true, string printerName = "")
		{
			if (print == false) return;
			if (string.IsNullOrWhiteSpace(locationCode) == true) return;

			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//Itur itur = iturRepository.GetIturByCode(locationCode, base.GetDbPath);

			Location location = null;
			ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
			location = locationRepository.GetLocationByCode(locationCode, base.GetDbPath);
			

			try
			{
				GenerateReportArgs args = new GenerateReportArgs();
				args.Customer = base.CurrentCustomer;
				args.Branch = base.CurrentBranch;
				args.Inventor = base.CurrentInventor;
				args.DbPath = base.GetDbPath;
				args.Report = report;
				args.Doc = null;
				args.Device = null;
				args.ViewDomainContextType = ViewDomainContextEnum.Iturs;
				args.Itur = null;
				args.Location = location;
				args.PrinterName = printerName;

				SelectParams selectParams = new SelectParams();
			//	List<string> searchLocationCode = new List<string> { locationCode };
				List<string> searchLocationCode = new List<string>();

				if (string.IsNullOrEmpty(locationCode) == false)
				{
					string[] locationCodes = locationCode.Split(',');
					foreach (var code in locationCodes)
					{
						searchLocationCode.Add(code);
					}
				}
				else
				{
					searchLocationCode.Add("");
				}
				

				selectParams.FilterStringListParams.Add("LocationCode", new FilterStringListParam()
				{
					Values = searchLocationCode
				});
				args.SelectParams = selectParams;

				ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
				printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
				//this._generateReportRepository.RunSaveReport(args, @"C:\Temp\testReport\output1.txt", ReportFileFormat.Excel);
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunPrintReportByLocationCode", exc);
			}
		}


		// Фоновая печать по LocationTag  ***
		private void RunPrintReportByLocationTag(string tag, bool include, Count4U.GenerationReport.Report report, bool print = true, string printerName = "")
		{
			if (print == false) return;
			if (string.IsNullOrWhiteSpace(tag) == true) return;

			ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();

			List<string> searchLocationCode = new List<string> ();
			if (include == false)
			{
				searchLocationCode = locationRepository.GetLocationCodeListByTag(base.GetDbPath, tag);
			}
			else
			{
				searchLocationCode = locationRepository.GetLocationCodeListIncludedTag(base.GetDbPath, tag);
			}

			try
			{
				GenerateReportArgs args = new GenerateReportArgs();
				args.Customer = base.CurrentCustomer;
				args.Branch = base.CurrentBranch;
				args.Inventor = base.CurrentInventor;
				args.DbPath = base.GetDbPath;
				args.Report = report;
				args.Doc = null;
				args.Device = null;
				args.ViewDomainContextType = ViewDomainContextEnum.Location;
				args.Itur = null;
				args.Location = null;
				args.PrinterName = printerName;

				SelectParams selectParams = new SelectParams();
			
				selectParams.FilterStringListParams.Add("Code", new FilterStringListParam()
				{
					Values = searchLocationCode
				});
				args.SelectParams = selectParams;

				ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
				printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunPrintReportByLocationTag", exc);
			}
		}

		// Фоновая печать по IturTag  ***
		private void RunPrintReportByIturTag(string tag, bool include, Count4U.GenerationReport.Report report, bool print = true, string printerName = "")
		{
			if (print == false) return;
			if (string.IsNullOrWhiteSpace(tag) == true) return;

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();

			List<string> searchIturCode = new List<string>();
			if (include == false)
			{
				searchIturCode = iturRepository.GetIturCodeListByTag(base.GetDbPath, tag);
			}
			else
			{
				searchIturCode = iturRepository.GetIturCodeListIncludedTag(base.GetDbPath, tag);
			}

			try
			{
				GenerateReportArgs args = new GenerateReportArgs();
				args.Customer = base.CurrentCustomer;
				args.Branch = base.CurrentBranch;
				args.Inventor = base.CurrentInventor;
				args.DbPath = base.GetDbPath;
				args.Report = report;
				args.Doc = null;
				args.Device = null;
				args.ViewDomainContextType = ViewDomainContextEnum.ItursIturDoc;
				args.Itur = null;
				args.Location = null;
				args.PrinterName = printerName;

				SelectParams selectParams = new SelectParams();

				selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam()
				{
					Values = searchIturCode
				});
				args.SelectParams = selectParams;

				ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
				printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunPrintReportByIturTag", exc);
			}
		}

		// Фоновая печать по SectionTag  ***
		private void RunPrintReportBySectionTag(string tag, bool include, Count4U.GenerationReport.Report report, bool print = true, string printerName = "")
		{
			if (print == false) return;
			if (string.IsNullOrWhiteSpace(tag) == true) return;

			ISectionRepository sectionRepository = this._serviceLocator.GetInstance<ISectionRepository>();

			List<string> searchSectionCode = new List<string>();
			if (include == false)
			{
				searchSectionCode = sectionRepository.GetSectionCodeListByTag(base.GetDbPath, tag);
			}
			else
			{
				searchSectionCode = sectionRepository.GetSectionCodeListIncludedTag(base.GetDbPath, tag);
			}

			try
			{
				GenerateReportArgs args = new GenerateReportArgs();
				args.Customer = base.CurrentCustomer;
				args.Branch = base.CurrentBranch;
				args.Inventor = base.CurrentInventor;
				args.DbPath = base.GetDbPath;
				args.Report = report;
				args.Doc = null;
				args.Device = null;
				args.ViewDomainContextType = ViewDomainContextEnum.ItursIturDoc;
				args.Itur = null;
				args.Location = null;
				args.PrinterName = printerName;

				SelectParams selectParams = new SelectParams();

				selectParams.FilterStringListParams.Add("SectionCode", new FilterStringListParam()
				{
					Values = searchSectionCode
				});
				args.SelectParams = selectParams;

				ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
				printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunPrintReportBySectionTag", exc);
			}
		}

        private void PrintQueueStarted(bool isStarted)
        {
            Utils.RunOnUI(() =>
                {
                    BusyText = Localization.Resources.View_IturListDetails_busyContentPrinting;
                    IsBusy = isStarted;
                });
        }

		// from ModuleBaseViewModel
		protected Dictionary<ImportProviderParmEnum, string> ParseAdapterIniFile(string sectionName, string iniFileName)
		{
			string iniFilePath = Path.Combine(FileSystem.ExportModulesFolderPath(), iniFileName);
			Dictionary<ImportProviderParmEnum, string> result = new Dictionary<ImportProviderParmEnum, string>();
			foreach (ImportProviderParmEnum en in Enum.GetValues(typeof(ImportProviderParmEnum)))
			{
				result.Add(en, String.Empty);
			}

			var iniData = _iniFileParser.Get(iniFilePath, sectionName);

			if (iniData != null)
			{
				foreach (var kvp in iniData.Data)
				{
					string key = kvp.Key.Trim();
					string value = kvp.Value.Trim();

					ImportProviderParmEnum en;

					if (Enum.TryParse(key, true, out en))
					{
						if (result.ContainsKey(en))
							result[en] = value;
					}
				}
			}

			return result;
		}
		//===========================================================================

		private void ApplyVisibilityToGroupByItems(bool resetFilter = false, string templateSelected ="", bool back = false)
        {
            _logger.Trace("ApplyVisibilityToGroupByItems");

            if (_modeSelectedItem == null)
                return;

            ItemFindVisibilityViewModel locationGroupItem = _groupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.LocationValue);
			ItemFindVisibilityViewModel statusGroupItem = _groupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.StatusValue);
			ItemFindVisibilityViewModel tagGroupItem = _groupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.TagValue);

			//if (locationGroupItem == null)
			//	return;

			if (string.IsNullOrWhiteSpace(templateSelected) == true)
			{
				_templateSelected = _templateItems.FirstOrDefault(r => r.Name == "");
				InitFilter();       //2021
									//RaisePropertyChanged(() => TemplateSelected);		//		 2021
			}
			else
			{
				if (templateSelected != IturStatusGroupEnum.None.ToString())
				{
					_statusSelected = this._statusItems.FirstOrDefault(r => r.Text == templateSelected);           //		 2021
				}
				else
				{
					_statusSelected = this._statusItems.FirstOrDefault(r => r.Value == ((int)IturStatusGroupEnum.None).ToString());
				}
																											   //RaisePropertyChanged(() => StatusSelected);      //		 2021
				if (_statusSelected != null && back == true)
				{
					RefreshModeSelectedItem_GroupBySelectedItem_TemplateSelected();
				}
			}
			if (resetFilter)
			{
				InitFilter();
				//RaisePropertyChanged(() => TemplateSelected);	  	//		 2021
			}

			switch (_modeSelectedItem.Value)
            {
				case ComboValues.IturListDetailsMode.ModePaged:
					{
						// I - список по чему группировать
						{
							locationGroupItem.IsVisible = true;
							statusGroupItem.IsVisible = true;
							tagGroupItem.IsVisible = true;
						}

						// II - Paging
						{
							PagingControlVisible = true;
							TemplateVisible = true;
						}

						// III - 3 комбобох, должен совпадать с выбранным MODE	 == ModePaged
						{
							LocationList3Visible = false;
							TagList3Visible = false;
							StatusList3Visible = false;
						}

						// IIII - 4 комбобох, должен совпадать с выбранным Group 
						{
							StatusList4Visible = false;
							TagList4Visible = false;
							LocationList4Visible = false;
						}
	
						break;
					}
				case ComboValues.IturListDetailsMode.ModeLocation:
					{
						LocationList4Visible = false;  //вообще не используется здесь, передвинуто на 3 место

						// I - список по чему группировать
						{
							//Если выбран Mode == Location, а в GroupBy было Location => очищаем GroupBy, потому что недопустимое сочетание 2 комбобоксов 
							locationGroupItem.IsVisible = false;	//GroupBy == Location  в режиме ModeLocation недопустимое сочетание
							statusGroupItem.IsVisible = true;
							tagGroupItem.IsVisible = true;
						}

						// II - Paging
						{
							PagingControlVisible = false;
							TemplateVisible = false;
						}

						// III - 3 комбобох, должен совпадать с выбранным MODE	== ModeLocation
						{
							LocationList3Visible = true;
							TagList3Visible = false;
							StatusList3Visible = false;

							//Если выбран Mode == Location, а в GroupBy было Location => очищаем GroupBy, потому что недопустимое сочетание 2 комбобоксов 
							if (locationGroupItem != null)
							{
								if (_groupBySelectedItem == locationGroupItem)
								{
									_groupBySelectedItem = _groupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.EmptyValue);
									//RaisePropertyChanged(() => GroupBySelectedItem);			   2021
								}
							}
						}

						// IIII - 4 комбобох, должен совпадать с выбранным Group 			//MODE	== ModeLocation
						{
							StatusList4Visible = false;
							TagList4Visible = false;
							LocationList4Visible = false;

							if (tagGroupItem != null)
							{
								if (_groupBySelectedItem == tagGroupItem)
								{
									TagList4Visible = true;
									//RaisePropertyChanged(() => TagSelected);	   2021
								}
							}

							if (statusGroupItem != null)
							{
								if (_groupBySelectedItem == statusGroupItem)
								{
									StatusList4Visible = true;
									//RaisePropertyChanged(() => StatusSelected);	   2021
								}
							}
						}
						break;
					}
				case ComboValues.IturListDetailsMode.ModeTag:
					{
						TagList4Visible = false; //вообще не используется здесь, передвинуто на 3 место

						// I - список по чему группировать
						{
							tagGroupItem.IsVisible = false;	//GroupBy == Tag  в режиме ModeTag недопустимое сочетание
							locationGroupItem.IsVisible = true;
							statusGroupItem.IsVisible = true;
						}

						// II - Paging
						{
							PagingControlVisible = false;
							TemplateVisible = false;
						}

						// III - 3 комбобох, должен совпадать с выбранным MODE	 == ModeTag
						{
							LocationList3Visible = false;
							TagList3Visible = true;
							StatusList3Visible = false;

							if (tagGroupItem != null)
							{
								//Если выбран Mode == Tag, а в GroupBy было Tag => очищаем GroupBy, потому что недопустимое сочетание 2 комбобоксов 
								if (_groupBySelectedItem == tagGroupItem)
								{
									_groupBySelectedItem = _groupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.EmptyValue);
									//RaisePropertyChanged(() => GroupBySelectedItem);		 2021
								}
							}
						}

						// IIII - 4 комбобох, должен совпадать с выбранным Group 	   //MODE	 == ModeTag
						{
							StatusList4Visible = false;
							TagList4Visible = false;
							LocationList4Visible = false;

							//Если выбран Mode == Tag, а в GroupBy было location => визуализировать комбобох с локешенами на 4 позиции
							if (locationGroupItem != null)
							{
								if (_groupBySelectedItem == locationGroupItem)
								{
									LocationList4Visible = true;
									//RaisePropertyChanged(() => LocationSelected);		   2021
								}
							}

							//Если выбран Mode == Tag, а в GroupBy было location => визуализировать комбобох с локешенами на 4 позиции
							if (statusGroupItem != null)
							{
								if (_groupBySelectedItem == statusGroupItem)
								{
									StatusList4Visible = true;
									//RaisePropertyChanged(() => StatusSelected);		   2021
								}
							}

						}
						break;
					}

				case ComboValues.IturListDetailsMode.ModeStatus:
					{
						StatusList4Visible = false; //вообще не используется здесь, передвинуто на 3 место

						// I - список по чему группировать
						{
							tagGroupItem.IsVisible = true;	
							locationGroupItem.IsVisible = true;
							statusGroupItem.IsVisible = false;	 //GroupBy == Status  в режиме ModeStatus недопустимое сочетание
						}

						// II - Paging
						{
							PagingControlVisible = false;
							TemplateVisible = false;
						}

						// III - 3 комбобох, должен совпадать с выбранным MODE	 == ModeStatus
						{
							LocationList3Visible = false;
							TagList3Visible = false;
							StatusList3Visible = true;

							if (tagGroupItem != null)
							{
								//Если выбран Mode == Status, а в GroupBy было Status => очищаем GroupBy, потому что недопустимое сочетание 2 комбобоксов 
								if (_groupBySelectedItem == statusGroupItem)
								{
									_groupBySelectedItem = _groupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.EmptyValue);
									//RaisePropertyChanged(() => GroupBySelectedItem);			 2021
								}
							}
						}

						// IIII - 4 комбобох, должен совпадать с выбранным Group 	   //MODE	 == ModeStatus
						{
							StatusList4Visible = false;
							TagList4Visible = false;
							LocationList4Visible = false;

							//Если выбран Mode == ModeStatus, а в GroupBy было location => визуализировать комбобох с локешенами на 4 позиции
							if (locationGroupItem != null)
							{
								if (_groupBySelectedItem == locationGroupItem)
								{
									LocationList4Visible = true;
									//RaisePropertyChanged(() => LocationSelected);		  2021
								}
							}

							//Если выбран Mode == ModeStatus, а в GroupBy было Tag => визуализировать комбобох с локешенами на 4 позиции
							if (tagGroupItem != null)
							{
								if (_groupBySelectedItem == tagGroupItem)
								{
									TagList4Visible = true;
									//RaisePropertyChanged(() => TagSelected); 2021
								}
							}

						}
						break;
					}
            }
			
        }

        private void UpCommandExecuted()
        {
            if (_modeSelectedItem == null) return;

            if (_modeSelectedItem.Value == ComboValues.IturListDetailsMode.ModeLocation)
            {
                if (_locationSelected == null) return;

                var indexOf = _locationItems.IndexOf(_locationSelected);
                if (indexOf - 1 < 0)
					_locationSelected = _locationItems.LastOrDefault();
                else
					_locationSelected = _locationItems.ElementAt(indexOf - 1);
            }
            else
            {
                if (PageCurrent + 1 > MaxPages())
                    PageCurrent = 1;
                else
                    PageCurrent++;
            }
        }

        private void DownCommandExecuted()
        {
            if (_modeSelectedItem == null) return;

            if (_modeSelectedItem.Value == ComboValues.IturListDetailsMode.ModeLocation)
            {

                if (_locationSelected == null) return;

                var indexOf = _locationItems.IndexOf(_locationSelected);

                if (indexOf >= _locationItems.Count - 1)
					_locationSelected = _locationItems.FirstOrDefault();
                else
					_locationSelected = _locationItems.ElementAt(indexOf + 1);
            }
            else
            {
                if (PageCurrent - 1 <= 0)
                    PageCurrent = MaxPages();
                else
                    PageCurrent--;
            }
        }

        private int MaxPages()
        {
            return (int)Math.Ceiling((double)ItemsTotal / PageSize);
        }

        private void WindowPreviewKeyUp(Key key)
        {
            if (key == Key.PageUp)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                _upCommand.Execute();
            }
            if (key == Key.PageDown)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                _downCommand.Execute();
            }
        }

        private void ProgressItemsBuild()
        {
            _logger.Trace("ProgressItemsBuild");

            try
            {
                Dictionary<int, int> dbStatuses = this._iturRepository.GetIturTotalGroupByStatuses(base.GetDbPath);
                int totalIturs = this._iturRepository.GetItursTotal(base.GetDbPath);
				Dictionary<string, StatusIturGroup> codes = this._statusIturGroupRepository.CodeStatusIturGroupWithNoneDictionary;
				Dictionary<int, IturStatusGroupEnum> bitStatusIturGroupEnumDictionary = _statusIturGroupRepository.BitStatusIturGroupEnumWithNoneDictionary;//BitStatusIturGroupEnumDictionary;
                Session lastSession = this._sessionRepository.GetSessionWithMaxDateCreated(base.GetDbPath);
                LastSessionCountItem = 0;
				LastSessionCountDocument = 0;
                LastSessionSumQuantityEdit = 0;
                if (lastSession != null)
                {
                    LastSessionCountItem = lastSession.CountItem;
                    LastSessionSumQuantityEdit = lastSession.SumQuantityEdit;
					LastSessionCountDocument = lastSession.CountDocument;
                }

                InventProducts totalResult = _inventProductRepository.GetInventProductTotal(base.GetDbPath);
                double sumQuantityEdit = totalResult.SumQuantityEdit;

                Utils.RunOnUI(() =>
                    {
                        _progressItems.Clear();

                        foreach (KeyValuePair<string, StatusIturGroup> group in codes)
                        {
                            int groupBit = @group.Value.Bit;
                            int bitCaculated = dbStatuses.ContainsKey(groupBit) ? dbStatuses[groupBit] : 0;
                            string groupName = UtilsMisc.LocalizationFromLocalizationKey(group.Value.NameLocalizationCode);
                            string item = String.Format("{0}: {1}", groupName, bitCaculated);

                            Color color = Colors.Tomato;
                            IturStatusGroupEnum en = IturStatusGroupEnum.Empty;
                            if (bitStatusIturGroupEnumDictionary.ContainsKey(groupBit))
                            {
                                en = bitStatusIturGroupEnumDictionary[groupBit];

                                string colorString = this._userSettingsManager.StatusGroupColorGet(en.ToString());
                                color = String.IsNullOrEmpty(colorString) ?
                                            UserSettingsHelpers.StatusGroupDefaultColorGet(en) :
                                            UserSettingsHelpers.StringToColor(colorString);
                            }
                            else
                            {

                            }
							
                            ProgressItemViewModel viewModel = new ProgressItemViewModel();
                            viewModel.Text = item;
                            viewModel.En = en;
							viewModel.StatusBit = (int)en;
							viewModel.Count = bitCaculated;
                            viewModel.Color = new SolidColorBrush(color);
							viewModel.SelectCommand = new DelegateCommand<ProgressItemViewModel>(SelectCommandExecuted); 
							if (en == IturStatusGroupEnum.None)
							{
								viewModel.Text = String.Format("{0}: {1}", groupName, totalIturs);
								viewModel.Count = totalIturs;
							}
                            _progressItems.Add(viewModel);
                        }

		
						//ProgressItemViewModel viewModel1 = new ProgressItemViewModel();
						//viewModel1.Text = "All";
						//viewModel1.En = en;
						//viewModel1.StatusBit = (int)en;
						//viewModel1.Count = bitCaculated;
						//viewModel1.Color = new SolidColorBrush(color);
						//viewModel1.SelectCommand = new DelegateCommand<ProgressItemViewModel>(SelectCommandExecuted);
						//_progressItems.Add(viewModel1);

                        ProgressItemViewModel disabledAndNoOneDoc = _progressItems.FirstOrDefault(r => r.En == IturStatusGroupEnum.DisableAndNoOneDoc);
                        ProgressItemViewModel empty = _progressItems.FirstOrDefault(r => r.En == IturStatusGroupEnum.Empty);

                        int disabledAndNoOneDocCount = disabledAndNoOneDoc == null ? 0 : disabledAndNoOneDoc.Count;
                        int emptyCount = empty == null ? 0 : empty.Count;

                        double totalDone = 0;
                        double A = totalIturs - disabledAndNoOneDocCount;
                        double B = emptyCount;
                        if (A > 0)
                        {
                            totalDone = ((A - B) / A) * 100;
                        }

                        ProgressDone = totalDone;
                        string progressRounded = String.Format("{0:0.0}", totalDone);

                        ProgressDoneString = String.Format(Localization.Resources.ViewModel_IturListDetails_ProgressTotalString, progressRounded);

                        ProgressDoneTotalItemsString = String.Format(Localization.Resources.ViewModel_IturListDetails_ProgressDoneTotalItemsString, String.Format("{0:0,##}", sumQuantityEdit));

                    });
            }
            catch (Exception exc)
            {
                _logger.ErrorException("ProgressItemsBuild", exc);
            }
        }

		private void SelectCommandExecuted(ProgressItemViewModel viewModel)
		{
			int status = (int)viewModel.En;
			string statusGroup = viewModel.En.ToString();

	
			this._modeSelectedItem = _modeItems.FirstOrDefault(r => r.Value == ComboValues.IturListDetailsMode.ModePaged);
			//RaisePropertyChanged(() => this.ModeSelectedItem);		2021

			this._groupBySelectedItem = this._groupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.LocationValue);
			//RaisePropertyChanged(() => this.GroupBySelectedItem);		2021

			if (viewModel.En != IturStatusGroupEnum.None)
			{
				_templateSelected = _templateItems.FirstOrDefault(r => r._name == statusGroup);
			}
			else
			{
				_templateSelected = _templateItems.FirstOrDefault(r => r._name == "");
			}
			//RaisePropertyChanged(() => TemplateSelected);			  // 2021

			ApplyVisibilityToGroupByItems(true, statusGroup);

		   if (this._statusSelected != null)
		   {
				RefreshModeSelectedItem_GroupBySelectedItem_TemplateSelected();
				//RaisePropertyChanged(() => this.ModeSelectedItem);
				//RaisePropertyChanged(() => this.GroupBySelectedItem);
				//RaisePropertyChanged(() => this.TemplateSelected);
				//Mouse.OverrideCursor = Cursors.Wait;
			 //  BuildOnBackgroundThread();
		   }
		}

        private void PlanogramCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.PlanogramOpen(this._regionManager, uriQuery);
        }

        private void UpdateCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeUpdateCatalog);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void ErpCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(base.Context));
            UtilsNavigate.ExportErpWithModulesOpen(this._regionManager, uriQuery);
        }


        private void ChangeStatusCommandExecuted()
        {
            InventorStatusChangeEventPayload payload = new InventorStatusChangeEventPayload();
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            this._eventAggregator.GetEvent<InventorStatusChangeEvent>().Publish(payload);
        }


        private void PdaCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(base.Context));
            UtilsNavigate.ExportPdaWithModulesOpen(this._regionManager, uriQuery);
        }
    }
}

