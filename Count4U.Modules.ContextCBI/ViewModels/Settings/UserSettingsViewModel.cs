using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Events.Configuration;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Misc;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Script;
using Count4U.Modules.ContextCBI.ViewModels.Settings.Items;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System.Linq;
using NLog;
using Zen.Barcode;
using System.Drawing.Printing;
using Count4U.Configuration.Interfaces;
using Count4U.Configuration.Dynamic;
using Count4U.Model.Count4U.Translation;

namespace Count4U.Modules.ContextCBI.ViewModels.Settings
{
    public class UserSettingsViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string LoggingSetSimple = "Simple";
        private const string LoggingSetInfo = "Info";
        private const string LoggingSetDebug = "Debug";
        private const string LoggingSetConfigurable = "Configurable";

        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly DelegateCommand _resetCommand;
        private readonly ILog _iLog;

		private readonly DelegateCommand _openExportPDAPathCommand;
		private readonly DelegateCommand _openImportPDAPathCommand;
		private readonly DelegateCommand _openHostCommand;

        private readonly DelegateCommand _openExportTCPPathCommand;
        private readonly DelegateCommand _openImportTCPPathCommand;


        private int _portionItursDashboard;
        private int _portionItursList;
        private int _portionCBI;
        private int _portionInventProducts;
        private int _portionProducts;
        private int _portionSections;
        private int _portionSuppliers;
		private int _portionFamilys;

        private readonly ObservableCollection<StatusColorItemViewModel> _statusColors;
        private readonly ObservableCollection<StatusGroupColorItemViewModel> _statusGroupColors;

        private readonly ObservableCollection<EncodingItemViewModel> _encodingItems;
        private EncodingItemViewModel _encodingSelectedItem;

        private readonly ObservableCollection<LanguageItemViewModel> _languageItems;
        private LanguageItemViewModel _languageSelectedItem;

		//private readonly ObservableCollection<ExeptionFileNotDeleteItemViewModel> _uploadOptionsHT630_ExeptionFileNotDeleteItems;
		//private ExeptionFileNotDeleteItemViewModel _uploadOptionsHT630_ExeptionFileNotDeleteSelectedItems;

	
        private bool _isLogSimpleTrace;
        private bool _isLogTrace;
        private bool _isLogTraceParser;
        private bool _isLogTraceRepository;
        private bool _isLogTraceProvider;
        private bool _isLogWarning;
        private bool _isLogWarningParser;
        private bool _isLogWarningRepository;
        private bool _isLogTraceParserResult;
        private bool _isLogTraceRepositoryResult;
        private bool _isLogTraceProviderResult;

        private string _logSimpleTrace;
        private string _logTrace;
        private string _logTraceParser;
        private string _logTraceRepository;
        private string _logTraceProvider;
        private string _logWarning;
        private string _logWarningParser;
        private string _logWarningRepository;
        private string _logTraceParserResult;
        private string _logTraceProviderResult;
        private string _logTraceRepositoryResult;

        private readonly ObservableCollection<ItemFindViewModel> _loggingSetItems;
        private ItemFindViewModel _loggingSetSelectedItem;

        private readonly DelegateCommand _configurationSetAddCommand;
        private readonly ObservableCollection<ConfigurationSetItemViewModel> _configurationSetItems;
        private ConfigurationSetItemViewModel _configurationSetSelectedItem;

        private int _delay;

        private readonly ObservableCollection<ItemFindViewModel> _sortByItems;
        private readonly ObservableCollection<ItemFindViewModel> _groupByItems;
        private readonly ObservableCollection<ItemFindViewModel> _iturListModeItems;
        private ItemFindViewModel _sortBySelectedItem;
        private ItemFindViewModel _groupBySelectedItem;
        private ItemFindViewModel _iturListModeSelectedItem;

        private readonly ObservableCollection<string> _barcodeTypeItems;
        private string _barcodeTypeSelectedItem;

		private readonly ObservableCollection<string> _printerItems;
		private string _printerSelectedItem;

        private readonly ObservableCollection<CurrencyItemViewModel> _currencyItems;
        private CurrencyItemViewModel _currencySelectedItem;

        private string _barcodePrefix;
        private string _iturNamePrefix;

		private string _customerFilterCode;
		private string _customerFilterName;
		private bool _useCustomerFilter;
		private bool _searchDialogIsModal;

        private bool _autoNavigateBack;
		

        private string _mainDashboardBackground;
        private double _mainDashboardBackgroundOpacity;
        private readonly DelegateCommand _mainDashboardBackgroundBrowseCommand;

        private string _customerDashboardBackground;
        private double _customerDashboardBackgroundOpacity;
        private readonly DelegateCommand _customerDashboardBackgroundBrowseCommand;

        private string _branchDashboardBackground;
        private double _branchDashboardBackgroundOpacity;
        private readonly DelegateCommand _branchDashboardBackgroundBrowseCommand;

        private string _inventorDashboardBackground;
        private double _inventorDashboardBackgroundOpacity;
        private readonly DelegateCommand _inventorDashboardBackgroundBrowseCommand;

        private readonly ObservableCollection<ReportRepositoryItemViewModel> _reportRepositoryItems;
        private ReportRepositoryItemViewModel _reportRepositorySelectedItem;

        private bool _isShowERP;

		private bool _isPackDataFileCatalog;

        private readonly ObservableCollection<ItemFindViewModel> _iturFilterItems;
        private ItemFindViewModel _iturFilterSelected;

		private readonly ObservableCollection<ItemFindViewModel> _iturFilterSortItems;
		private ItemFindViewModel _iturFilterSortSelected;

		private readonly ObservableCollection<ItemFindViewModel> _iturFilterSortAZItems;
		private ItemFindViewModel _iturFilterSortAZSelected;

		

        private readonly ObservableCollection<ItemFindViewModel> _inventProductFocusFilterItems;
        private ItemFindViewModel _inventProductFocusFilterSelected;

        private string _reportAppName;
		private string _uploadOptionsHT630_ExeptionFileNotDelete;
		private string _uploadOptionsHT630_AfterUploadRunExeFileList;

        private Color _planEmptyColor;
        private Color _planZeroColor;
        private Color _planHundredColor;
		private Color _inventProductMarkColor;

        private int _uploadWakeupTime;
		//private string _uploadOptionsHT630_BaudratePDA;
		private bool _copyFromSource;
		private bool _forwardResendDate;
		private bool _copyFromHost;
		private bool _copyByCodeInventor;
		
		private bool _countingFromSource;
		private bool _forwardResendData;
		private string _exportPDAPath;
		private string _importPDAPath;
        private string _webServiceLink;
        private string _webServiceDeveloperLink;
        private bool _sendToFtpOffice;

        private string _exportTCPPath;
        private string _importTCPPath;
        private string _tcpServerPort;
        

        private bool _showMark;
		private bool _propertyIsEmpty;
		private bool _propertyIsNotEmpty;
		private bool _propertyIsEqual;
		private bool _propertyIsNotEqual;
		
		

		private string _host;
		private string _user;
		private string _password;

		private bool _uploadOptionsHT630_CurrentDataPDA;
		private bool _uploadOptionsHT630_DeleteAllFilePDA;
		private bool _uploadOptionsHT630_AfterUploadPerformWarmStart;
		private bool _uploadOptionsHT630_AfterUploadRunExeFileNeedDo;
		private bool _uploadOptionsHT630_BaudratePDA;

		private readonly ObservableCollection<string> _uploadOptionsHT630_BaudratePDAItems;
		private string _uploadOptionsHT630_BaudratePDASelectedItems;

		private readonly ObservableCollection<string> _uploadOptionsRunMemoryItems;
		private string _uploadOptionsRunMemorySelectedItems;

			//ItemsSource="{Binding Path=DomainObjectTypeItems}" 
		//							 SelectedItem="{Binding Path=DomainObjectTypeSelectedItem}"
		private readonly ObservableCollection<string> _domainObjectItems;
		private string _domainObjectSelectedItem;

		private bool _tagSubstring;

		private readonly ObservableCollection<string> _inventProductPropertyItems;	  //ToDo + Number
		private readonly ObservableCollection<string> _inventProductPropertyNumberItems;
		private string _inventProductPropertySelectedItem;		//OLD
		private string _inventProductPropertyMarkSelectedItem;
		 private string _inventProductPropertyFilterSelectedItem;				  //ToDo + Number
		 private string _inventProductPropertyFilterSelectedNumberItem;	

		private string _inventProductPropertyPhotoSelectedItem;

		private readonly ObservableCollection<string> _editorTemplateItems;
		private string _editorTemplateSelectedItem;		
				

		//private readonly ObservableCollection<string> _uploadOptionsHT630_ExeptionFileNotDeleteItems;
		//private string _uploadOptionsHT630_ExeptionFileNotDeleteSelectedItems;

		//private readonly ObservableCollection<string> _uploadOptionsHT630_AfterUploadRunExeFileItems;
		//private string _uploadOptionsHT630_AfterUploadRunExeFileSelectedItems;

		private readonly IEditorTemplateRepository _editorTemplateRepository;

        public UserSettingsViewModel(IUserSettingsManager userSettingsManager,
            ILog iLog,
            IEventAggregator eventAggregator,
			IEditorTemplateRepository editorTemplateRepository,
            IContextCBIRepository contextCbiRepository)
            : base(contextCbiRepository)
        {
            this._eventAggregator = eventAggregator;
            this._iLog = iLog;
            this._userSettingsManager = userSettingsManager;
			this._editorTemplateRepository = editorTemplateRepository;

            this._resetCommand = new DelegateCommand(ResetCommandExecuted);
			this._openExportPDAPathCommand = new DelegateCommand(this.OpenExportPDAPathCommandExecuted, this.OpenExportPDAPathCommandCanExecute);
			this._openImportPDAPathCommand = new DelegateCommand(this.OpenImportPDAPathCommandExecuted, this.OpenImportPDAPathCommandCanExecute);
			this._openHostCommand = new DelegateCommand(this.OpenHostCommandExecuted, this.OpenHostCommandCanExecute);

            this._openExportTCPPathCommand = new DelegateCommand(this.OpenExportTCPPathCommandExecuted, this.OpenExportTCPPathCommandCanExecute);
            this._openImportTCPPathCommand = new DelegateCommand(this.OpenImportTCPPathCommandExecuted, this.OpenImportTCPPathCommandCanExecute);


            this._encodingItems = new ObservableCollection<EncodingItemViewModel>();
            this._statusColors = new ObservableCollection<StatusColorItemViewModel>();
            this._statusGroupColors = new ObservableCollection<StatusGroupColorItemViewModel>();
            this._barcodeTypeItems = new ObservableCollection<string>();

			this._printerItems = new ObservableCollection<string>();


            this._configurationSetAddCommand = new DelegateCommand(ConfigurationSetAddCommandExecuted);
            this._configurationSetItems = new ObservableCollection<ConfigurationSetItemViewModel>();

            this._loggingSetItems = new ObservableCollection<ItemFindViewModel>
                {
                        new ItemFindViewModel() {Value = LoggingSetSimple, Text = Localization.Resources.ViewModel_UserSettings_LoggingSetSimple},
                        new ItemFindViewModel() {Value = LoggingSetInfo, Text = Localization.Resources.ViewModel_UserSettings_LoggingSetInfo},
                        new ItemFindViewModel() {Value = LoggingSetDebug, Text = Localization.Resources.ViewModel_UserSettings_LoggingSetDebug},
                        new ItemFindViewModel() {Value = LoggingSetConfigurable, Text = Localization.Resources.ViewModel_UserSettings_LoggingSetConfigurable}
                                        };

            _sortByItems = new ObservableCollection<ItemFindViewModel>
                {
                        new ItemFindViewModel() { Text = ComboValues.SortItur.LocationText, Value = ComboValues.SortItur.LocationValue},
                        new ItemFindViewModel() { Text = ComboValues.SortItur.NumberText, Value = ComboValues.SortItur.NumberValue},
                        new ItemFindViewModel() { Text = ComboValues.SortItur.StatusText, Value = ComboValues.SortItur.StatusValue},
						 new ItemFindViewModel() { Text = ComboValues.SortItur.IturERPCodeText, Value = ComboValues.SortItur.IturERPCodeValue},
                    };

            _groupByItems = new ObservableCollection<ItemFindViewModel>
                {
                        new ItemFindViewModel() {Text = ComboValues.GroupItur.EmptyText, Value = ComboValues.GroupItur.EmptyValue},
                        new ItemFindViewModel() {Text = ComboValues.GroupItur.LocationText, Value = ComboValues.GroupItur.LocationValue},
                        new ItemFindViewModel() {Text = ComboValues.GroupItur.StatusText, Value = ComboValues.GroupItur.StatusValue},
						new ItemFindViewModel() {Text = ComboValues.GroupItur.TagText, Value = ComboValues.GroupItur.TagValue},
			//			new ItemFindViewModel() {Text = ComboValues.GroupItur.StatusNotEmptyText, Value = ComboValues.GroupItur.StatusNotEmptyValue},
                    };

            _iturListModeItems = new ObservableCollection<ItemFindViewModel>
                {
                    new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModePagedText, Value = ComboValues.IturListDetailsMode.ModePaged},
                    new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModeLocationText, Value = ComboValues.IturListDetailsMode.ModeLocation},
					 new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModeTagText, Value = ComboValues.IturListDetailsMode.ModeTag},
					 new ItemFindViewModel() {Text = ComboValues.IturListDetailsMode.ModeStatusText, Value = ComboValues.IturListDetailsMode.ModeStatus},
                };

            _currencyItems = new ObservableCollection<CurrencyItemViewModel>
                {
                    new CurrencyItemViewModel() {Symbol = Common.Constants.CurrencySymbol.SHEQEL},
                    new CurrencyItemViewModel() {Symbol = Common.Constants.CurrencySymbol.DOLLAR},
                    new CurrencyItemViewModel() {Symbol = Common.Constants.CurrencySymbol.EURO},
                    new CurrencyItemViewModel() {Symbol = Common.Constants.CurrencySymbol.RUBLE}
                };

            this._languageItems = new ObservableCollection<LanguageItemViewModel>()
                {
                     new LanguageItemViewModel() { Language = enLanguage.English, Text = "English" },
                     new LanguageItemViewModel() { Language = enLanguage.Italian, Text = "Italian" },
                    new LanguageItemViewModel() { Language = enLanguage.Hebrew, Text = "Hebrew" },
                     new LanguageItemViewModel() { Language = enLanguage.Russian, Text = "Russian" },
                };

			this._uploadOptionsHT630_BaudratePDAItems = new ObservableCollection<string>() { "57600", "38400", "19200", "9600" };
			//public enum MPMemory { RAM = 0x00, FLASH = 0x02, ROM = 0x03, NONE = 0x01 };
			this._uploadOptionsRunMemoryItems = new ObservableCollection<string>() { "RAM", "FLASH", "ROM", "NONE" };
			this._inventProductPropertyItems = new ObservableCollection<string>() {"SupplierCode", "SectionCode",
				"FamilyCode","SerialNumber",  "Code",
				"IPValueStr1","IPValueStr2","IPValueStr3",
				"IPValueStr4","IPValueStr5","IPValueStr6","IPValueStr7","IPValueStr8","IPValueStr9","IPValueStr10","IPValueStr11","IPValueStr12",
				"IPValueStr13","IPValueStr14","IPValueStr15","IPValueStr16","IPValueStr17","IPValueStr18","IPValueStr19","IPValueStr20"};
 
			this._inventProductPropertyNumberItems = new ObservableCollection<string>() {"QuantityEdit", "IPValueFloat1","IPValueFloat2",
				"IPValueFloat3","IPValueFloat4", "IPValueFloat5","IPValueInt1","IPValueInt2","IPValueInt3","IPValueInt4","IPValueInt5"};

			this._domainObjectItems = new ObservableCollection<string>() { "Location", "Itur", "Section" };

			this._editorTemplateItems = new ObservableCollection<string>();
			this._editorTemplateItems.Add("<none>");
			string viewName = Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct; //temp
			List<EditorTemplate> editorTemplates = this._editorTemplateRepository.Get(viewName);
			if (editorTemplates != null)
			{
				foreach (EditorTemplate editorTemplate in editorTemplates)
				{
					if (editorTemplate != null) this._editorTemplateItems.Add(editorTemplate.Code);
				}
			}
			
			//this._uploadOptionsHT630_ExeptionFileNotDeleteItems = new ObservableCollection<string>() { "JENG.EXE", "PDANUM.TXT", "UPDATEDU.TXT" };
			//this._uploadOptionsHT630_AfterUploadRunExeFileItems = new ObservableCollection<string>() { "OFFLINE.EXE" };

            this._logSimpleTrace = MessageTypeCaption.SimpleTrace;
            this._logTrace = MessageTypeCaption.Trace;
            this._logTraceParser = MessageTypeCaption.TraceParser;
            this._logTraceRepository = MessageTypeCaption.TraceRepository;
            this._logTraceProvider = MessageTypeCaption.TraceProvider;
            this._logWarning = MessageTypeCaption.Warning;
            this._logWarningParser = MessageTypeCaption.WarningParser;
            this._logWarningRepository = MessageTypeCaption.WarningRepository;
            this._logTraceParserResult = MessageTypeCaption.TraceParserResult;
            this._logTraceProviderResult = MessageTypeCaption.TraceProviderResult;
            this._logTraceRepositoryResult = MessageTypeCaption.TraceRepositoryResult;

            this.BuildBarcodeTypes();
			this.BuildPrinters();

			this._mainDashboardBackgroundBrowseCommand = new DelegateCommand(MainDashboardBackgroundBrowseCommandExecuted);
			this._customerDashboardBackgroundBrowseCommand = new DelegateCommand(CustomerDashboardBackgroundBrowseCommandExecuted);
			this._branchDashboardBackgroundBrowseCommand = new DelegateCommand(BranchDashboardBackgroundBrowseCommandExecuted);
			this._inventorDashboardBackgroundBrowseCommand = new DelegateCommand(InventorDashboardBackgroundBrowseCommandExecuted);

            _reportRepositoryItems = new ObservableCollection<ReportRepositoryItemViewModel>()
                {
                    //new ReportRepositoryItemViewModel() {Enum = IturAnalyzesRepositoryEnum.IturAnalyzesADORepository, Text = IturAnalyzesRepositoryCaption.IturAnalyzesADORepository},
                    new ReportRepositoryItemViewModel() {Enum = IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository, Text = IturAnalyzesRepositoryCaption.IturAnalyzesBulkRepository},
                    new ReportRepositoryItemViewModel() {Enum = IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository, Text = IturAnalyzesRepositoryCaption.IturAnalyzesReaderADORepository},
                };

			this._iturFilterItems = new ObservableCollection<ItemFindViewModel>();
			this._iturFilterItems.Add(new ItemFindViewModel() { Value = ComboValues.FindItur.FilterIturNumber, Text = Localization.Resources.Constant_Number });
			this._iturFilterItems.Add(new ItemFindViewModel() { Value = ComboValues.FindItur.FilterIturERP, Text = Localization.Resources.Constant_ERP });

			this._iturFilterSortAZItems = new ObservableCollection<ItemFindViewModel>();
			this._iturFilterSortAZItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSortAZ.SortASC, Text = Localization.Resources.Constant_AZ });
			this._iturFilterSortAZItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSortAZ.SortDESC, Text = Localization.Resources.Constant_ZA });

			this._iturFilterSortItems = new ObservableCollection<ItemFindViewModel>();
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterEmpty, Text = ComboValues.FindIturSort.Empty });
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterNumber, Text = ComboValues.FindIturSort.Number });
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterIturCode, Text = ComboValues.FindIturSort.IturCode });
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterNumberPrefix, Text = ComboValues.FindIturSort.NumberPrefix });
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterNumberSufix, Text = ComboValues.FindIturSort.NumberSufix });
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterLocationCode, Text = ComboValues.FindIturSort.LocationCode });
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterERPIturCode, Text = ComboValues.FindIturSort.ERPIturCode });
			this._iturFilterSortItems.Add(new ItemFindViewModel() { Value = ComboValues.FindIturSort.FilterStatusIturGroupBit, Text = ComboValues.FindIturSort.StatusIturGroupBit });

			this._inventProductFocusFilterItems = new ObservableCollection<ItemFindViewModel>()
                {
                    new ItemFindViewModel() {Value = FocusValues.InventProductSearch.Makat, Text = FocusValues.InventProductSearch.MakatText},
					new ItemFindViewModel() {Value = FocusValues.InventProductSearch.Code, Text = FocusValues.InventProductSearch.CodeText},
                    new ItemFindViewModel() {Value = FocusValues.InventProductSearch.CodeInputFromPDA, Text = FocusValues.InventProductSearch.CodeInputFromPDAText},
			        new ItemFindViewModel() {Value = FocusValues.InventProductSearch.SerialNumber, Text = FocusValues.InventProductSearch.SerialNumber},
                    new ItemFindViewModel() {Value = FocusValues.InventProductSearch.ProductName, Text = FocusValues.InventProductSearch.ProductNameText},
                    new ItemFindViewModel() {Value = FocusValues.InventProductSearch.IturCode, Text = FocusValues.InventProductSearch.IturCodeText},
                };

	


        }

        public DelegateCommand ResetCommand
        {
            get { return this._resetCommand; }
        }

		public DelegateCommand OpenImportPDAPathCommand
		{
			get { return this._openImportPDAPathCommand; }
		}

		public DelegateCommand OpenExportPDAPathCommand
		{
			get { return this._openExportPDAPathCommand; }
		}

		public DelegateCommand OpenHostCommand
		{
			get { return this._openHostCommand; }
		}

        public DelegateCommand OpenImportTCPPathCommand
        {
            get { return this._openImportTCPPathCommand; }
        }

        public DelegateCommand OpenExportTCPPathCommand
        {
            get { return this._openExportTCPPathCommand; }
        }



        private void OpenExportPDAPathCommandExecuted()
		{
			if (String.IsNullOrEmpty(_exportPDAPath))
				return;

			Utils.OpenFolderInExplorer(_exportPDAPath);
		}

		private bool OpenExportPDAPathCommandCanExecute()
		{
			if (String.IsNullOrEmpty(_exportPDAPath))
				return false;

			if (Directory.Exists(_exportPDAPath) == false) 
				return false;

			return true;
		}

		private void OpenImportPDAPathCommandExecuted()
		{
			if (String.IsNullOrEmpty(_importPDAPath))
				return;

			Utils.OpenFolderInExplorer(_importPDAPath);
		}

		private bool OpenImportPDAPathCommandCanExecute()
		{
			if (String.IsNullOrEmpty(_importPDAPath))
				return false;


			if (Directory.Exists(_importPDAPath) == false)
				return false;

			return true;
		}


        private void OpenExportTCPPathCommandExecuted()
        {
            if (String.IsNullOrEmpty(_exportTCPPath))
                return;

            Utils.OpenFolderInExplorer(_exportTCPPath);
        }

        private bool OpenExportTCPPathCommandCanExecute()
        {
            if (String.IsNullOrEmpty(_exportTCPPath))
                return false;

            if (Directory.Exists(_exportTCPPath) == false)
                return false;

            return true;
        }

        private void OpenImportTCPPathCommandExecuted()
        {
            if (String.IsNullOrEmpty(_importTCPPath))
                return;

            Utils.OpenFolderInExplorer(_importTCPPath);
        }

        private bool OpenImportTCPPathCommandCanExecute()
        {
            if (String.IsNullOrEmpty(_importTCPPath))
                return false;


            if (Directory.Exists(_importTCPPath) == false)
                return false;

            return true;
        }


        private void OpenHostCommandExecuted()
		{
			if (String.IsNullOrEmpty(_host))
				return;

			Utils.OpenFolderInExplorer(_host);
		}

		private bool OpenHostCommandCanExecute()
		{
			if (String.IsNullOrEmpty(_host))
				return false;
	 
			//if (Directory.Exists(_host) == false)			 //TODO
			//	return false;

			return true;
		}
        #region portion

        public int PortionItursDashboard
        {
            get { return this._portionItursDashboard; }
            set
            {
                this._portionItursDashboard = value;
                this.RaisePropertyChanged(() => this.PortionItursDashboard);

                this._userSettingsManager.PortionItursSet(this._portionItursDashboard);
            }
        }

        public int PortionItursList
        {
            get { return this._portionItursList; }
            set
            {
                this._portionItursList = value;
                this.RaisePropertyChanged(() => this.PortionItursList);

                this._userSettingsManager.PortionItursListSet(this._portionItursList);
            }
        }

        public int PortionCBI
        {
            get { return this._portionCBI; }
            set
            {
                this._portionCBI = value;
                this.RaisePropertyChanged(() => this.PortionCBI);

                this._userSettingsManager.PortionCBISet(this._portionCBI);
            }
        }

        public int PortionInventProducts
        {
            get { return this._portionInventProducts; }
            set
            {
                this._portionInventProducts = value;
                this.RaisePropertyChanged(() => this.PortionInventProducts);

                this._userSettingsManager.PortionInventProdutsSet(this._portionInventProducts);
            }
        }

        public int PortionProducts
        {
            get { return this._portionProducts; }
            set
            {
                this._portionProducts = value;
                this.RaisePropertyChanged(() => this.PortionProducts);

                this._userSettingsManager.PortionProdutsSet(this._portionProducts);
            }
        }

        public int PortionSections
        {
            get { return _portionSections; }
            set
            {
                _portionSections = value;
                this.RaisePropertyChanged(() => this.PortionSections);

                this._userSettingsManager.PortionSectionsSet(this._portionSections);
            }
        }

        public int PortionSuppliers
        {
            get { return _portionSuppliers; }
            set
            {
                _portionSuppliers = value;
                this.RaisePropertyChanged(() => this.PortionSuppliers);

                this._userSettingsManager.PortionSuppliersSet(this._portionSuppliers);
            }
        }


		public int PortionFamilys
        {
			get { return _portionFamilys; }
            set
            {
				_portionFamilys = value;
				this.RaisePropertyChanged(() => this.PortionFamilys);

				this._userSettingsManager.PortionFamilysSet(this._portionFamilys);
            }
        }

        #endregion

        #region colors

        public ObservableCollection<StatusColorItemViewModel> StatusColors
        {
            get
            {
                return this._statusColors;
            }
        }

        public ObservableCollection<StatusGroupColorItemViewModel> StatusGroupColors
        {
            get
            {
                return this._statusGroupColors;
            }
        }

        #endregion

        #region logging

        public bool IsLogSimpleTrace
        {
            get { return _isLogSimpleTrace; }
            set
            {
                _isLogSimpleTrace = value;
                RaisePropertyChanged(() => IsLogSimpleTrace);

                SetLogType(MessageTypeEnum.SimpleTrace, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }


        public bool IsLogTrace
        {
            get { return _isLogTrace; }
            set
            {
                _isLogTrace = value;
                RaisePropertyChanged(() => IsLogTrace);

                SetLogType(MessageTypeEnum.Trace, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogTraceParser
        {
            get { return _isLogTraceParser; }
            set
            {
                _isLogTraceParser = value;
                RaisePropertyChanged(() => IsLogTraceParser);

                SetLogType(MessageTypeEnum.TraceParser, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogTraceRepository
        {
            get { return _isLogTraceRepository; }
            set
            {
                _isLogTraceRepository = value;
                RaisePropertyChanged(() => IsLogTraceRepository);

                SetLogType(MessageTypeEnum.TraceRepository, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogTraceProvider
        {
            get { return _isLogTraceProvider; }
            set
            {
                _isLogTraceProvider = value;
                RaisePropertyChanged(() => IsLogTraceProvider);

                SetLogType(MessageTypeEnum.TraceProvider, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogWarning
        {
            get { return _isLogWarning; }
            set
            {
                _isLogWarning = value;
                RaisePropertyChanged(() => IsLogWarning);

                SetLogType(MessageTypeEnum.Warning, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogWarningParser
        {
            get { return _isLogWarningParser; }
            set
            {
                _isLogWarningParser = value;
                RaisePropertyChanged(() => IsLogWarningParser);

                SetLogType(MessageTypeEnum.WarningParser, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogWarningRepository
        {
            get { return _isLogWarningRepository; }
            set
            {
                _isLogWarningRepository = value;
                RaisePropertyChanged(() => IsLogWarningRepository);

                SetLogType(MessageTypeEnum.WarningRepository, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogTraceParserResult
        {
            get { return _isLogTraceParserResult; }
            set
            {
                _isLogTraceParserResult = value;
                RaisePropertyChanged(() => IsLogTraceParserResult);

                SetLogType(MessageTypeEnum.TraceParserResult, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogTraceRepositoryResult
        {
            get { return _isLogTraceRepositoryResult; }
            set
            {
                _isLogTraceRepositoryResult = value;
                RaisePropertyChanged(() => IsLogTraceRepositoryResult);

                SetLogType(MessageTypeEnum.TraceRepositoryResult, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public bool IsLogTraceProviderResult
        {
            get { return _isLogTraceProviderResult; }
            set
            {
                _isLogTraceProviderResult = value;
                RaisePropertyChanged(() => IsLogTraceProviderResult);

                SetLogType(MessageTypeEnum.TraceProviderResult, value);
                this.SelectLoggingSetFromCheckBoxes();
            }
        }

        public string LogSimpleTrace
        {
            get { return _logSimpleTrace; }
        }

        public string LogTrace
        {
            get { return _logTrace; }
        }

        public string LogTraceParser
        {
            get { return _logTraceParser; }
        }

        public string LogTraceRepository
        {
            get { return _logTraceRepository; }
        }

        public string LogTraceProvider
        {
            get { return _logTraceProvider; }
        }

        public string LogWarning
        {
            get { return _logWarning; }
        }

        public string LogWarningParser
        {
            get { return _logWarningParser; }
        }

        public string LogWarningRepository
        {
            get { return _logWarningRepository; }
        }

        public string LogTraceParserResult
        {
            get { return _logTraceParserResult; }
        }

        public string LogTraceProviderResult
        {
            get { return _logTraceProviderResult; }
        }

        public string LogTraceRepositoryResult
        {
            get { return _logTraceRepositoryResult; }
        }

        #endregion

        public ObservableCollection<EncodingItemViewModel> EncodingItems
        {
            get { return _encodingItems; }
        }

        public event EventHandler OnResetToDefault = delegate { };

        private void OnOnResetToDefault(EventArgs e)
        {
            EventHandler handler = OnResetToDefault;
            if (handler != null) handler(this, e);
        }

        public EncodingItemViewModel EncodingSelectedItem
        {
            get { return _encodingSelectedItem; }
            set
            {
                this._encodingSelectedItem = value;
                RaisePropertyChanged(() => EncodingSelectedItem);

                if (this._encodingSelectedItem != null)
                    this._userSettingsManager.GlobalEncodingSet(this._encodingSelectedItem.Encoding.CodePage);
            }
        }

        public ObservableCollection<LanguageItemViewModel> LanguageItems
        {
            get { return _languageItems; }
        }

		//public ObservableCollection<string> UploadOptionsHT630_ExeptionFileNotDeleteItems
		//{
		//	get { return this._uploadOptionsHT630_ExeptionFileNotDeleteItems; }
		//}

		//public ObservableCollection<string> UploadOptionsHT630_AfterUploadRunExeFileItems
		//{
		//	get { return this._uploadOptionsHT630_AfterUploadRunExeFileItems; }
		//}

		public ObservableCollection<string> UploadOptionsHT630_BaudratePDAItems
        {
			get { return this._uploadOptionsHT630_BaudratePDAItems; }
        }

		public ObservableCollection<string> DomainObjectItems
        {
			get { return this._domainObjectItems; }
        }

		public ObservableCollection<string> UploadOptionsRunMemoryItems
        {
			get { return this._uploadOptionsRunMemoryItems; }
        }

		public ObservableCollection<string> InventProductPropertyItems
        {
			get { return this._inventProductPropertyItems; }
        }

		public ObservableCollection<string> InventProductPropertyNumberItems
        {
			get { return this._inventProductPropertyNumberItems; }
        }
		

		public ObservableCollection<string> EditorTemplateItems
        {
			get { return this._editorTemplateItems; }
        }
		

        public LanguageItemViewModel LanguageSelectedItem
        {
            get { return _languageSelectedItem; }
            set
            {
                this._languageSelectedItem = value;
                RaisePropertyChanged(() => LanguageSelectedItem);

                if (this._languageSelectedItem != null)
                    this._userSettingsManager.LanguageSet(this._languageSelectedItem.Language);
            }
        }


		// public string UploadOptionsHT630_ExeptionFileNotDeleteSelectedItems
		//{
		//	get { return _uploadOptionsHT630_ExeptionFileNotDeleteSelectedItems; }
		//	set
		//	{
		//		this._uploadOptionsHT630_ExeptionFileNotDeleteSelectedItems = value;
		//		RaisePropertyChanged(() => UploadOptionsHT630_ExeptionFileNotDeleteSelectedItems);

		//		if (this._uploadOptionsHT630_ExeptionFileNotDeleteSelectedItems != null)
		//			this._userSettingsManager.UploadOptionsHT630_ExeptionFileNotDeleteSet(this._uploadOptionsHT630_ExeptionFileNotDeleteSelectedItems);
		//	}
		//}


		// public string UploadOptionsHT630_AfterUploadRunExeFileSelectedItems
		//{
		//	get { return _uploadOptionsHT630_AfterUploadRunExeFileSelectedItems; }
		//	set
		//	{
		//		this._uploadOptionsHT630_AfterUploadRunExeFileSelectedItems = value;
		//		RaisePropertyChanged(() => UploadOptionsHT630_AfterUploadRunExeFileSelectedItems);

		//		if (this._uploadOptionsHT630_AfterUploadRunExeFileSelectedItems != null)
		//			this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileSet(this._uploadOptionsHT630_AfterUploadRunExeFileSelectedItems);
		//	}
		//}



		 public string UploadOptionsHT630_BaudratePDASelectedItems
        {
			get { return _uploadOptionsHT630_BaudratePDASelectedItems; }
            set
            {
				this._uploadOptionsHT630_BaudratePDASelectedItems = value;
				RaisePropertyChanged(() => UploadOptionsHT630_BaudratePDASelectedItems);

				if (this._uploadOptionsHT630_BaudratePDASelectedItems != null)
					this._userSettingsManager.UploadOptionsHT630_BaudratePDAItemSet(this._uploadOptionsHT630_BaudratePDASelectedItems);
            }
        }

		 public string UploadOptionsRunMemorySelectedItems
        {
			get { return _uploadOptionsRunMemorySelectedItems; }
            set
            {
				this._uploadOptionsRunMemorySelectedItems = value;
				RaisePropertyChanged(() => UploadOptionsRunMemorySelectedItems);

				if (this._uploadOptionsRunMemorySelectedItems != null)
					this._userSettingsManager.UploadOptionsRunMemoryItemSet(this._uploadOptionsRunMemorySelectedItems);
            }
        }



		 public string DomainObjectSelectedItem
        {
			get { return _domainObjectSelectedItem; }
            set
            {
				this._domainObjectSelectedItem = value;
				RaisePropertyChanged(() => DomainObjectSelectedItem);

				if (this._domainObjectSelectedItem != null)
					this._userSettingsManager.DomainObjectSelectedItemSet(this._domainObjectSelectedItem);
            }
        }


		 public bool TagSubstring
		{
			get { return this._tagSubstring; }
			set
			{
				this._tagSubstring = value;
				RaisePropertyChanged(() => TagSubstring);

				this._userSettingsManager.TagSubstringSet(_tagSubstring);
			}
		}

		public string InventProductPropertyMarkSelectedItem
        {
			get { return _inventProductPropertyMarkSelectedItem; }
            set
            {
				this._inventProductPropertyMarkSelectedItem = value;
				RaisePropertyChanged(() => InventProductPropertyMarkSelectedItem);

				if (this._inventProductPropertyMarkSelectedItem != null)
					this._userSettingsManager.InventProductPropertyMarkSelectedItemSet(this._inventProductPropertyMarkSelectedItem);
            }
        }


		public string InventProductPropertyFilterSelectedItem
		{
			get { return _inventProductPropertyFilterSelectedItem; }
			set
			{
				this._inventProductPropertyFilterSelectedItem = value;
				RaisePropertyChanged(() => InventProductPropertyFilterSelectedItem);

				if (this._inventProductPropertyFilterSelectedItem != null)
					this._userSettingsManager.InventProductPropertyFilterSelectedItemSet(this._inventProductPropertyFilterSelectedItem);
			}
		}


		public string InventProductPropertyFilterSelectedNumberItem
		{
			get { return _inventProductPropertyFilterSelectedNumberItem; }
			set
			{
				this._inventProductPropertyFilterSelectedNumberItem = value;
				RaisePropertyChanged(() => InventProductPropertyFilterSelectedNumberItem);

				if (this._inventProductPropertyFilterSelectedNumberItem != null)
					this._userSettingsManager.InventProductPropertyFilterSelectedNumberItemSet(this._inventProductPropertyFilterSelectedNumberItem);
			}
		}


		public string InventProductPropertyPhotoSelectedItem
		{
			get { return _inventProductPropertyPhotoSelectedItem; }
			set
			{
				this._inventProductPropertyPhotoSelectedItem = value;
				RaisePropertyChanged(() => InventProductPropertyPhotoSelectedItem);

				if (this._inventProductPropertyPhotoSelectedItem != null)
					this._userSettingsManager.InventProductPropertyPhotoSelectedItemSet(this._inventProductPropertyPhotoSelectedItem);
			}
		}

		public string InventProductPropertySelectedItem
		{
			get { return _inventProductPropertySelectedItem; }
			set
			{
				this._inventProductPropertySelectedItem = value;
				RaisePropertyChanged(() => InventProductPropertySelectedItem);

				if (this._inventProductPropertySelectedItem != null)
					this._userSettingsManager.InventProductPropertySelectedItemSet(this._inventProductPropertySelectedItem);
			}
		}


		public string EditorTemplateSelectedItem
		{
			get { return _editorTemplateSelectedItem; }
			set
			{
				this._editorTemplateSelectedItem = value;
				RaisePropertyChanged(() => EditorTemplateSelectedItem);

				if (this._editorTemplateSelectedItem != null)
					this._userSettingsManager.EditorTemplateSelectedItemSet(this._editorTemplateSelectedItem);
			}
		}

        public ObservableCollection<ConfigurationSetItemViewModel> ConfigurationSetItems
        {
            get { return _configurationSetItems; }
        }

        public DelegateCommand ConfigurationSetAddCommand
        {
            get { return _configurationSetAddCommand; }
        }

        public ConfigurationSetItemViewModel ConfigurationSetSelectedItem
        {
            get { return _configurationSetSelectedItem; }
            set
            {
                _configurationSetSelectedItem = value;
                RaisePropertyChanged(() => ConfigurationSetSelectedItem);

                if (this._configurationSetSelectedItem != null)
                {
                    try
                    {
                        using (new CursorWait())
                        {
                            this._eventAggregator.GetEvent<ApplicationConfSetPreChangedEvent>().Publish(null);

                            this._userSettingsManager.AdminSetCurrentConfiguration(this._configurationSetSelectedItem.File);

                            this.InitFromConfig();

                            this.OnOnResetToDefault(null);

                            this._eventAggregator.GetEvent<ApplicationConfSetChangedEvent>().Publish(null);
                        }
                    }
                    catch (Exception exc)
                    {
                        _logger.ErrorException("ConfigurationSetSelectedItem", exc);
                    }
                }
            }
        }

        public ObservableCollection<ItemFindViewModel> LoggingSetItems
        {
            get { return _loggingSetItems; }
        }

        public ItemFindViewModel LoggingSetSelectedItem
        {
            get { return _loggingSetSelectedItem; }
            set
            {
                _loggingSetSelectedItem = value;
                RaisePropertyChanged(() => LoggingSetSelectedItem);

                SelectCheckBoxesFromLoggingSet();
            }
        }

        public int Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                RaisePropertyChanged(() => Delay);
				PortionItursDashboard = Delay;
				
		//		RaisePropertyChanged(() => PortionItursDashboard);

                this._userSettingsManager.DelaySet(this._delay);
            }
        }

        public ObservableCollection<ItemFindViewModel> SortByItems
        {
            get { return _sortByItems; }
        }

        public ObservableCollection<ItemFindViewModel> GroupByItems
        {
            get { return _groupByItems; }
        }

        public ItemFindViewModel SortBySelectedItem
        {
            get { return _sortBySelectedItem; }
            set
            {
                _sortBySelectedItem = value;
                RaisePropertyChanged(() => SortBySelectedItem);

                if (_sortBySelectedItem != null)
                    _userSettingsManager.IturSortSet(_sortBySelectedItem.Value);

                _eventAggregator.GetEvent<SortConfigurationChangedEvent>().Publish(null);
            }
        }

        public ItemFindViewModel GroupBySelectedItem
        {
            get { return _groupBySelectedItem; }
            set
            {
                _groupBySelectedItem = value;
                RaisePropertyChanged(() => GroupBySelectedItem);

                if (_groupBySelectedItem != null)
                    _userSettingsManager.IturGroupSet(_groupBySelectedItem.Value);

                _eventAggregator.GetEvent<GroupConfigurationChangedEvent>().Publish(null);
            }
        }

        public ObservableCollection<ItemFindViewModel> IturListModeItems
        {
            get { return _iturListModeItems; }
        }

        public ItemFindViewModel IturListModeSelectedItem
        {
            get { return _iturListModeSelectedItem; }
            set
            {
                _iturListModeSelectedItem = value;
                RaisePropertyChanged(() => IturListModeSelectedItem);

                if (_iturListModeSelectedItem != null)
                    _userSettingsManager.IturModeSet(_iturListModeSelectedItem.Value);
            }
        }

        public ObservableCollection<CurrencyItemViewModel> CurrencyItems
        {
            get { return _currencyItems; }
        }

        public CurrencyItemViewModel CurrencySelectedItem
        {
            get { return _currencySelectedItem; }
            set
            {
                _currencySelectedItem = value;
                RaisePropertyChanged(() => CurrencySelectedItem);

                if (_currencySelectedItem != null)
                    _userSettingsManager.CurrencySet(_currencySelectedItem.Symbol);
            }
        }

        public ObservableCollection<string> BarcodeTypeItems
        {
            get { return _barcodeTypeItems; }
        }

		public ObservableCollection<string> PrinterItems
		{
			get { return _printerItems; }
		}

        public string BarcodeTypeSelectedItem
        {
            get { return _barcodeTypeSelectedItem; }
            set
            {
                _barcodeTypeSelectedItem = value;
                RaisePropertyChanged(() => BarcodeTypeSelectedItem);

                if (_barcodeTypeSelectedItem != null)
                    _userSettingsManager.BarcodeTypeSet(_barcodeTypeSelectedItem);
            }
        }

		public string PrinterSelectedItem
        {
			get { return _printerSelectedItem; }
            set
            {
				_printerSelectedItem = value;
				RaisePropertyChanged(() => PrinterSelectedItem);

				if (_printerSelectedItem != null)
					_userSettingsManager.PrinterSet(_printerSelectedItem);
            }
        }
		

        public string BarcodePrefix
        {
            get { return _barcodePrefix; }
            set
            {
                _barcodePrefix = value;
                RaisePropertyChanged(() => BarcodePrefix);

                _userSettingsManager.BarcodePrefixSet(_barcodePrefix);
            }
        }

		public string CustomerFilterCode
        {
			get { return _customerFilterCode; }
            set
            {
				_customerFilterCode = value;
				RaisePropertyChanged(() => CustomerFilterCode);

				_userSettingsManager.CustomerFilterCodeSet(_customerFilterCode);
            }
        }

		public string CustomerFilterName
        {
			get { return _customerFilterName; }
            set
            {
				_customerFilterName = value;
				RaisePropertyChanged(() => CustomerFilterName);

				_userSettingsManager.CustomerFilterNameSet(_customerFilterName);
            }
        }

		

        public string IturNamePrefix
        {
            get { return _iturNamePrefix; }
            set
            {
                _iturNamePrefix = value;
                RaisePropertyChanged(() => IturNamePrefix);

                _userSettingsManager.IturNamePrefixSet(_iturNamePrefix);
            }
        }

        public bool AutoNavigateBack
        {
            get { return _autoNavigateBack; }
            set
            {
                _autoNavigateBack = value;
                RaisePropertyChanged(() => AutoNavigateBack);

                _userSettingsManager.NavigateBackImportPdaFormSet(_autoNavigateBack);
            }
        }


		public bool UseCustomerFilter
        {
			get { return _useCustomerFilter; }
            set
            {
				_useCustomerFilter = value;
				RaisePropertyChanged(() => UseCustomerFilter);

				_userSettingsManager.UseCustomerFilterSet(_useCustomerFilter);
            }
        }


		public bool SearchDialogIsModal
        {
			get { return _searchDialogIsModal; }
            set
            {
				_searchDialogIsModal = value;
				RaisePropertyChanged(() => SearchDialogIsModal);

				_userSettingsManager.SearchDialogIsModalSet(_searchDialogIsModal);
            }
        }
		

        public string MainDashboardBackground
        {
            get { return _mainDashboardBackground; }
            set
            {
                _mainDashboardBackground = value;
                RaisePropertyChanged(() => MainDashboardBackground);
                _userSettingsManager.DashboardHomeBackgroundSet(_mainDashboardBackground);
                _eventAggregator.GetEvent<BackgroundChangedEvent>().Publish(null);
            }
        }



		public string ImportPDAPath
        {
			get { return _importPDAPath; }
            set
            {
				_importPDAPath = value;
				RaisePropertyChanged(() => ImportPDAPath);
				_userSettingsManager.ImportPDAPathSet(_importPDAPath);
               // _eventAggregator.GetEvent<BackgroundChangedEvent>().Publish(null);
            }
        }

        public string ImportTCPPath
        {
            get { return _importTCPPath; }
            set
            {
                _importTCPPath = value;
                RaisePropertyChanged(() => ImportTCPPath);
                _userSettingsManager.ImportTCPPathSet(_importTCPPath);
                // _eventAggregator.GetEvent<BackgroundChangedEvent>().Publish(null);
            }
        }

        
        public string ExportTCPPath
        {
            get { return _exportTCPPath; }
            set
            {
                _exportTCPPath = value;
                RaisePropertyChanged(() => ExportTCPPath);
                _userSettingsManager.ExportTCPPathSet(_exportTCPPath);
                //	_eventAggregator.GetEvent<BackgroundChangedEvent>().Publish(null);
            }
        }

        public string TcpServerPort
        {
            get { return _tcpServerPort; }
            set
            {
                _tcpServerPort = value;
                RaisePropertyChanged(() => TcpServerPort);
                _userSettingsManager.TcpServerPortSet(_tcpServerPort);
                // _eventAggregator.GetEvent<BackgroundChangedEvent>().Publish(null);
            }
        }

        public string WebServiceLink
        {
            get { return _webServiceLink; }
            set
            {
                _webServiceLink = value;
                RaisePropertyChanged(() => WebServiceLink);
                _userSettingsManager.WebServiceLinkSet(_webServiceLink);
                
            }
        }


        private bool _useToo = false;
        public bool UseToo
        {
            get { return _useToo; }
            set
            {
                _useToo = value;
                RaisePropertyChanged(() => UseToo);
                _userSettingsManager.UseTooSet(_useToo);
            }
        }

        
        private bool _tcpServerOn = false;
        public bool TcpServerOn
        {
            get { return _tcpServerOn; }
            set
            {
                _tcpServerOn = value;
                RaisePropertyChanged(() => TcpServerOn);
                _userSettingsManager.UseTooSet(_tcpServerOn);
            }
        }




        public string WebServiceDeveloperLink
        {
            get { return _webServiceDeveloperLink; }
            set
            {
                _webServiceDeveloperLink = value;
                RaisePropertyChanged(() => WebServiceDeveloperLink);
                _userSettingsManager.WebServiceDeveloperLinkSet(_webServiceDeveloperLink);

            }
        }


        public string ExportPDAPath
		{
			get { return _exportPDAPath; }
			set
			{
				_exportPDAPath = value;
				RaisePropertyChanged(() => ExportPDAPath);
				_userSettingsManager.ExportPDAPathSet(_exportPDAPath);
			//	_eventAggregator.GetEvent<BackgroundChangedEvent>().Publish(null);
			}
		}


       

        public string Host
		{
			get { return _host; }
			set
			{
				_host = value;
				RaisePropertyChanged(() => Host);
				_userSettingsManager.HostSet(_host);
			}
		}

		public string User
		{
			get { return _user; }
			set
			{
				_user = value;
				RaisePropertyChanged(() => User);
				_userSettingsManager.UserSet(_user);
			}
		}

		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				RaisePropertyChanged(() => Password);
				_userSettingsManager.PasswordSet(_password);
			}
		}




        public double MainDashboardBackgroundOpacity
        {
            get { return _mainDashboardBackgroundOpacity; }
            set
            {
                _mainDashboardBackgroundOpacity = value;
                RaisePropertyChanged(() => MainDashboardBackgroundOpacity);
                _userSettingsManager.DashboardHomeBackgroundOpacitySet(_mainDashboardBackgroundOpacity);
                _eventAggregator.GetEvent<BackgroundChangedEvent>().Publish(null);
            }
        }

        public string CustomerDashboardBackground
        {
            get { return _customerDashboardBackground; }
            set
            {
                _customerDashboardBackground = value;
                RaisePropertyChanged(() => CustomerDashboardBackground);
                _userSettingsManager.DashboardCustomerBackgroundSet(_customerDashboardBackground);
            }
        }
        public double CustomerDashboardBackgroundOpacity
        {
            get { return _customerDashboardBackgroundOpacity; }
            set
            {
                _customerDashboardBackgroundOpacity = value;
                RaisePropertyChanged(() => CustomerDashboardBackgroundOpacity);
                _userSettingsManager.DashboardCustomerackgroundOpacitySet(_customerDashboardBackgroundOpacity);
            }
        }

        public string BranchDashboardBackground
        {
            get { return _branchDashboardBackground; }
            set
            {
                _branchDashboardBackground = value;
                RaisePropertyChanged(() => BranchDashboardBackground);
                _userSettingsManager.DashboardBranchBackgroundSet(_branchDashboardBackground);
            }
        }
        public double BranchDashboardBackgroundOpacity
        {
            get { return _branchDashboardBackgroundOpacity; }
            set
            {
                _branchDashboardBackgroundOpacity = value;
                RaisePropertyChanged(() => BranchDashboardBackgroundOpacity);
                _userSettingsManager.DashboardBranchBackgroundOpacitySet(_branchDashboardBackgroundOpacity);
            }
        }

        public string InventorDashboardBackground
        {
            get { return _inventorDashboardBackground; }
            set
            {
                _inventorDashboardBackground = value;
                RaisePropertyChanged(() => InventorDashboardBackground);
                _userSettingsManager.DashboardInventorBackgroundSet(_inventorDashboardBackground);
            }
        }
        public double InventorDashboardBackgroundOpacity
        {
            get { return _inventorDashboardBackgroundOpacity; }
            set
            {
                _inventorDashboardBackgroundOpacity = value;
                RaisePropertyChanged(() => InventorDashboardBackgroundOpacity);
                _userSettingsManager.DashboardInventorBackgroundOpacitySet(_inventorDashboardBackgroundOpacity);
            }
        }

        public DelegateCommand MainDashboardBackgroundBrowseCommand
        {
            get { return _mainDashboardBackgroundBrowseCommand; }
        }
        public DelegateCommand CustomerDashboardBackgroundBrowseCommand
        {
            get { return _customerDashboardBackgroundBrowseCommand; }
        }
        public DelegateCommand BranchDashboardBackgroundBrowseCommand
        {
            get { return _branchDashboardBackgroundBrowseCommand; }
        }
        public DelegateCommand InventorDashboardBackgroundBrowseCommand
        {
            get { return _inventorDashboardBackgroundBrowseCommand; }
        }

        public ObservableCollection<ReportRepositoryItemViewModel> ReportRepositoryItems
        {
            get { return _reportRepositoryItems; }
        }

        public ReportRepositoryItemViewModel ReportRepositorySelectedItem
        {
            get { return _reportRepositorySelectedItem; }
            set
            {
                _reportRepositorySelectedItem = value;
                RaisePropertyChanged(() => ReportRepositorySelectedItem);

                if (_reportRepositorySelectedItem != null)
                {

                    _userSettingsManager.ReportRepositorySet(_reportRepositorySelectedItem.Enum);
                }
            }
        }

        public bool IsShowERP
        {
            get { return _isShowERP; }
            set
            {
                _isShowERP = value;
                RaisePropertyChanged(() => IsShowERP);

                _userSettingsManager.ShotIturERPSet(_isShowERP);
            }
        }

		public bool IsPackDataFileCatalog
        {
			get { return _isPackDataFileCatalog; }
            set
            {
				_isPackDataFileCatalog = value;
				RaisePropertyChanged(() => IsPackDataFileCatalog);

				_userSettingsManager.PackDataFileCatalogSet(_isPackDataFileCatalog); //?
            }
        }

		

        public ObservableCollection<ItemFindViewModel> IturFilterItems
        {
            get { return _iturFilterItems; }
        }

        public ItemFindViewModel IturFilterSelected
        {
            get { return _iturFilterSelected; }
            set
            {
                _iturFilterSelected = value;
                RaisePropertyChanged(() => IturFilterSelected);

                if (_iturFilterSelected != null)
                {
                    _userSettingsManager.IturFilterSelectedSet(_iturFilterSelected.Value);
                }
            }
        }

		public ObservableCollection<ItemFindViewModel> IturFilterSortItems
		{
			get { return _iturFilterSortItems; }
		}

		public ItemFindViewModel IturFilterSortSelected
		{
			get { return _iturFilterSortSelected; }
			set
			{
				_iturFilterSortSelected = value;
				RaisePropertyChanged(() => IturFilterSortSelected);

				if (_iturFilterSortSelected != null)
				{
					_userSettingsManager.IturFilterSortSelectedSet(_iturFilterSortSelected.Value);
				}
			}
		}


		public ObservableCollection<ItemFindViewModel> IturFilterSortAZItems
		{
			get { return _iturFilterSortAZItems; }
		}

		public ItemFindViewModel IturFilterSortAZSelected
		{
			get { return _iturFilterSortAZSelected; }
			set
			{
				_iturFilterSortAZSelected = value;
				RaisePropertyChanged(() => IturFilterSortAZSelected);

				if (_iturFilterSortAZSelected != null)
				{
					_userSettingsManager.IturFilterSortAZSelectedSet(_iturFilterSortAZSelected.Value);
				}
			}
		}

        public ObservableCollection<ItemFindViewModel> InventProductFocusFilterItems
        {
            get { return _inventProductFocusFilterItems; }
        }

        public ItemFindViewModel InventProductFocusFilterSelected
        {
            get { return _inventProductFocusFilterSelected; }
            set
            {
                _inventProductFocusFilterSelected = value;
                RaisePropertyChanged(() => InventProductFocusFilterSelected);

                if (_inventProductFocusFilterSelected != null)
                {
                    _userSettingsManager.InventProductFilterFocusSet(_inventProductFocusFilterSelected.Value);
                }
            }
        }

        public string ReportAppName
        {
            get { return _reportAppName; }
            set
            {
                _reportAppName = value;
                RaisePropertyChanged(() => ReportAppName);

                _userSettingsManager.ReportAppNameSet(_reportAppName);
            }
        }


		public string UploadOptionsHT630_AfterUploadRunExeFileList
        {
			get { return this._uploadOptionsHT630_AfterUploadRunExeFileList; }
            set
            {
				this._uploadOptionsHT630_AfterUploadRunExeFileList = value;
				RaisePropertyChanged(() => this.UploadOptionsHT630_AfterUploadRunExeFileList);

				_userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileListSet(this._uploadOptionsHT630_AfterUploadRunExeFileList);
            }
        }

		

			

        public Color PlanEmptyColor
        {
            get { return _planEmptyColor; }
            set
            {
                _planEmptyColor = value;
                RaisePropertyChanged(() => PlanEmptyColor);

                _userSettingsManager.PlanEmptyColorSet(_planEmptyColor);
            }
        }

        public Color PlanZeroColor
        {
            get { return _planZeroColor; }
            set
            {
                _planZeroColor = value;
                RaisePropertyChanged(() => PlanZeroColor);

                _userSettingsManager.PlanZeroColorSet(_planZeroColor);
            }
        }

        public Color PlanHundredColor
        {
            get { return _planHundredColor; }
            set
            {
                _planHundredColor = value;
                RaisePropertyChanged(() => PlanHundredColor);

                _userSettingsManager.PlanHundredSet(_planHundredColor);
            }
        }


		public Color InventProductMarkColor
        {
			get { return _inventProductMarkColor; }
            set
            {
				_inventProductMarkColor = value;
				RaisePropertyChanged(() => InventProductMarkColor);

				_userSettingsManager.InventProductMarkColorSet(_inventProductMarkColor);
            }
        }
		

        public int UploadWakeupTime
        {
            get { return _uploadWakeupTime; }
            set
            {
				this._uploadWakeupTime = value;
                RaisePropertyChanged(() => UploadWakeupTime);

				this._userSettingsManager.UploadWakeupTimeSet(this._uploadWakeupTime);
            }
        }


		//public int UploadOptionsHT630_BaudratePDA
		//{
		//	get { return this._uploadOptionsHT630_BaudratePDA; }
		//	set
		//	{
		//		this._uploadOptionsHT630_BaudratePDA = value;
		//		RaisePropertyChanged(() => UploadOptionsHT630_BaudratePDA);

		//		this._userSettingsManager.UploadOptionsHT630_BaudratePDASet(this._uploadOptionsHT630_BaudratePDA);
		//	}
		//}

		public bool CopyFromSource
		{
			get { return this._copyFromSource; }
			set
			{
				this._copyFromSource = value;
				RaisePropertyChanged(() => CopyFromSource);

				this._copyByCodeInventor = value;
				RaisePropertyChanged(() => CopyByCodeInventor);

				this._userSettingsManager.CopyFromSourceSet(_copyFromSource);
				this._userSettingsManager.CopyByCodeInventorSet(_copyByCodeInventor);
			}
		}

		public bool CopyFromHost
		{
			get { return this._copyFromHost; }
			set
			{
				this._copyFromHost = value;
				RaisePropertyChanged(() => CopyFromHost);

				this._userSettingsManager.CopyFromHostSet(_copyFromHost);
			}
		}

		public bool ForwardResendDate
		{
			get { return this._forwardResendDate; }
			set
			{
				this._forwardResendDate = value;
				RaisePropertyChanged(() => ForwardResendDate);

				this._userSettingsManager.ForwardResendDataSet(_forwardResendDate);
			}
		}

		

		public bool CopyByCodeInventor
		{
			get { return this._copyByCodeInventor; }
			set
			{
				this._copyByCodeInventor = value;
				RaisePropertyChanged(() => CopyByCodeInventor);

				this._userSettingsManager.CopyByCodeInventorSet(_copyByCodeInventor); 
			}
		}



		public bool ShowMark
		{
			get { return this._showMark; }
			set
			{
				this._showMark = value;
				RaisePropertyChanged(() => ShowMark);

				this._userSettingsManager.ShowMarkSet(_showMark);
			}
		}


		public bool PropertyIsEmpty
		{
			get { return this._propertyIsEmpty; }
			set
			{
				this._propertyIsEmpty = value;
				RaisePropertyChanged(() => PropertyIsEmpty);

				this._propertyIsNotEmpty = (!this._propertyIsEmpty);
				RaisePropertyChanged(() => PropertyIsNotEmpty);

				this._propertyIsEqual = (!this._propertyIsEmpty);
				RaisePropertyChanged(() => PropertyIsEqual);

				this._propertyIsNotEqual = (!this._propertyIsEmpty);
				RaisePropertyChanged(() => PropertyIsNotEqual);

				this._userSettingsManager.PropertyIsEmptySet(_propertyIsEmpty);
			}
		}

 
		public bool PropertyIsNotEmpty
		{
			get { return this._propertyIsNotEmpty; }
			set
			{
				this._propertyIsNotEmpty = value;
				RaisePropertyChanged(() => PropertyIsNotEmpty); 

				this._propertyIsEmpty = (!this._propertyIsNotEmpty);
				RaisePropertyChanged(() => PropertyIsEmpty);

				this._propertyIsEqual = (!this._propertyIsNotEmpty);
				RaisePropertyChanged(() => PropertyIsEqual);

				this._propertyIsNotEqual = (!this._propertyIsNotEmpty);
				RaisePropertyChanged(() => PropertyIsNotEqual);

				this._userSettingsManager.PropertyIsEmptySet(_propertyIsEmpty);
			}
		}
				  /// <summary>
				  /// 
				  /// </summary>
		public bool PropertyIsEqual
		{
			get { return this._propertyIsEqual; }
			set
			{
				this._propertyIsEqual = value;
				RaisePropertyChanged(() => PropertyIsEqual);

				this._propertyIsNotEqual = (!this._propertyIsEqual);
				RaisePropertyChanged(() => PropertyIsNotEqual);

				this._propertyIsEmpty = (!this._propertyIsEqual);
				RaisePropertyChanged(() => PropertyIsEmpty);

				this._propertyIsNotEmpty = (!this._propertyIsEqual);
				RaisePropertyChanged(() => PropertyIsNotEmpty);

				this._userSettingsManager.PropertyIsEmptySet(_propertyIsEmpty);
			}
		}


		public bool PropertyIsNotEqual
		{
			get { return this._propertyIsNotEqual; }
			set
			{
				this._propertyIsNotEqual = value;
				RaisePropertyChanged(() => PropertyIsNotEqual);

				this._propertyIsEqual = (!this._propertyIsNotEqual);
				RaisePropertyChanged(() => PropertyIsEqual);

				this._propertyIsEmpty = (!this._propertyIsNotEqual);
				RaisePropertyChanged(() => PropertyIsEmpty);

				this._propertyIsNotEmpty = (!this._propertyIsNotEqual);
				RaisePropertyChanged(() => PropertyIsNotEmpty);

				this._userSettingsManager.PropertyIsEmptySet(_propertyIsEmpty);
			}
		}

			

		public bool CountingFromSource
		{
			get { return this._countingFromSource; }
			set
			{
				this._countingFromSource = value;
				RaisePropertyChanged(() => CountingFromSource);

				this._userSettingsManager.CountingFromSourceSet(_countingFromSource);
			}
		}


		public bool SendToFtpOffice
		{
			get { return this._sendToFtpOffice; }
			set
			{
				this._sendToFtpOffice = value;
				RaisePropertyChanged(() => SendToFtpOffice);

				this._userSettingsManager.SendToFtpOfficeSet(_sendToFtpOffice);
			}
		}

		public bool ForwardResendData
		{
			get { return this._forwardResendData; }
			set
			{
				this._forwardResendData = value;
				RaisePropertyChanged(() => ForwardResendData);

				this._userSettingsManager.ForwardResendDataSet(_forwardResendData);
			}
		}

	
		public bool UploadOptionsHT630_CurrentDataPDA
		{
			get { return this._uploadOptionsHT630_CurrentDataPDA; }
			set
			{
				this._uploadOptionsHT630_CurrentDataPDA = value;
				RaisePropertyChanged(() => UploadOptionsHT630_CurrentDataPDA);

				this._userSettingsManager.UploadOptionsHT630_CurrentDataPDASet(_uploadOptionsHT630_CurrentDataPDA);
			}
		}

		public bool UploadOptionsHT630_BaudratePDA
		{
			get { return this._uploadOptionsHT630_BaudratePDA; }
			set
			{
				this._uploadOptionsHT630_BaudratePDA = value;
				RaisePropertyChanged(() => UploadOptionsHT630_BaudratePDA);

				this._userSettingsManager.UploadOptionsHT630_BaudratePDASet(_uploadOptionsHT630_BaudratePDA);
			}
		}

		public bool UploadOptionsHT630_DeleteAllFilePDA
		{
			get { return this._uploadOptionsHT630_DeleteAllFilePDA; }
			set
			{
				this._uploadOptionsHT630_DeleteAllFilePDA = value;
				RaisePropertyChanged(() => UploadOptionsHT630_DeleteAllFilePDA);

				this._userSettingsManager.UploadOptionsHT630_DeleteAllFilePDASet(this._uploadOptionsHT630_DeleteAllFilePDA);
			}
		}


		public string UploadOptionsHT630_ExeptionFileNotDelete
		{
			get { return this._uploadOptionsHT630_ExeptionFileNotDelete; }
			set
			{
				this._uploadOptionsHT630_ExeptionFileNotDelete = value;
				RaisePropertyChanged(() => UploadOptionsHT630_ExeptionFileNotDelete);

				this._userSettingsManager.UploadOptionsHT630_ExeptionFileNotDeleteSet(this._uploadOptionsHT630_ExeptionFileNotDelete);
			}
		}

		public bool UploadOptionsHT630_AfterUploadPerformWarmStart
		{
			get { return this._uploadOptionsHT630_AfterUploadPerformWarmStart; }
			set
			{
				this._uploadOptionsHT630_AfterUploadPerformWarmStart = value;
				RaisePropertyChanged(() => UploadOptionsHT630_AfterUploadPerformWarmStart);

				this._userSettingsManager.UploadOptionsHT630_AfterUploadPerformWarmStartSet(this._uploadOptionsHT630_AfterUploadPerformWarmStart);
			}
		}
		
		public bool UploadOptionsHT630_AfterUploadRunExeFileNeedDo
		{
			get { return this._uploadOptionsHT630_AfterUploadRunExeFileNeedDo; }
			set
			{
				this._uploadOptionsHT630_AfterUploadRunExeFileNeedDo = value;
				RaisePropertyChanged(() => UploadOptionsHT630_AfterUploadRunExeFileNeedDo);

				this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileNeedDoSet(this._uploadOptionsHT630_AfterUploadRunExeFileNeedDo);
			}
		}
		

        void StatusColorGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Color")
            {
                StatusGroupColorItemViewModel viewModel = sender as StatusGroupColorItemViewModel;
                if (viewModel != null)
                {
                    string color = UserSettingsHelpers.ColorToString(viewModel.Color);
                    this._userSettingsManager.StatusGroupColorSet(viewModel.EnumStatusGroup.ToString(), color);
                }
            }
        }

        void StatusColorItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Color")
            {
                StatusColorItemViewModel viewModel = sender as StatusColorItemViewModel;
                if (viewModel != null)
                {
                    string color = UserSettingsHelpers.ColorToString(viewModel.Color);
                    this._userSettingsManager.StatusColorSet(viewModel.EnumStatus.ToString(), color);
                }
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this.BuildConfigItems();
            this._configurationSetSelectedItem = this._configurationSetItems.FirstOrDefault(r => r.File == this._userSettingsManager.AdminGetCurrentConfiguration());
            RaisePropertyChanged(() => ConfigurationSetSelectedItem);

            this.BuildEncoding();

            this.InitFromConfig();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void InitFromConfig()
        {
            this._portionItursDashboard = this._userSettingsManager.PortionItursGet();
            this._portionItursList = this._userSettingsManager.PortionItursListGet();
            this._portionCBI = this._userSettingsManager.PortionCBIGet();
            this._portionInventProducts = this._userSettingsManager.PortionInventProductsGet();
            this._portionProducts = this._userSettingsManager.PortionProductsGet();
            this._portionSections = this._userSettingsManager.PortionSectionsGet();
            this._portionSuppliers = this._userSettingsManager.PortionSuppliersGet();
			this._portionFamilys = this._userSettingsManager.PortionFamilysGet();
			

            RaisePropertyChanged(() => PortionItursDashboard);
            RaisePropertyChanged(() => PortionItursList);
            RaisePropertyChanged(() => PortionCBI);
            RaisePropertyChanged(() => PortionInventProducts);
            RaisePropertyChanged(() => PortionProducts);
            RaisePropertyChanged(() => PortionSections);
			RaisePropertyChanged(() => PortionFamilys);

            BuildColors();
            BuildColorGroups();

            Dictionary<MessageTypeEnum, bool> logMessages = UserSettingsHelpers.LogTypeListGet(this._userSettingsManager);

            this._isLogSimpleTrace = logMessages[MessageTypeEnum.SimpleTrace];
            this._isLogTrace = logMessages[MessageTypeEnum.Trace];
            this._isLogTraceParser = logMessages[MessageTypeEnum.TraceParser];
            this._isLogTraceRepository = logMessages[MessageTypeEnum.TraceRepository];
            this._isLogTraceProvider = logMessages[MessageTypeEnum.TraceProvider];
            this._isLogWarning = logMessages[MessageTypeEnum.Warning];
            this._isLogWarningParser = logMessages[MessageTypeEnum.WarningParser];
            this._isLogWarningRepository = logMessages[MessageTypeEnum.WarningRepository];
            this._isLogTraceParserResult = logMessages[MessageTypeEnum.TraceParserResult];
            this._isLogTraceRepositoryResult = logMessages[MessageTypeEnum.TraceRepositoryResult];
            this._isLogTraceProviderResult = logMessages[MessageTypeEnum.TraceProviderResult];

            RaisePropertyChanged(() => IsLogTrace);
            RaisePropertyChanged(() => IsLogTraceParser);
            RaisePropertyChanged(() => IsLogTraceRepository);
            RaisePropertyChanged(() => IsLogTraceProvider);
            RaisePropertyChanged(() => IsLogWarning);
            RaisePropertyChanged(() => IsLogWarningParser);
            RaisePropertyChanged(() => IsLogWarningRepository);
            RaisePropertyChanged(() => IsLogTraceParserResult);
            RaisePropertyChanged(() => IsLogTraceRepositoryResult);
            RaisePropertyChanged(() => IsLogTraceProviderResult);

            this.SelectLoggingSetFromCheckBoxes();

            //encoding
            this._encodingSelectedItem = this._encodingItems.FirstOrDefault();
            RaisePropertyChanged(() => EncodingSelectedItem);

            //language          
            this._languageSelectedItem = this._languageItems.FirstOrDefault(r => r.Language == this._userSettingsManager.LanguageGet());
            RaisePropertyChanged(() => LanguageSelectedItem);


			this._delay = this._userSettingsManager.DelayGet();
            RaisePropertyChanged(() => Delay);

            string sort = _userSettingsManager.IturSortGet();
            string group = _userSettingsManager.IturGroupGet();
            string mode = _userSettingsManager.IturModeGet();
            this._sortBySelectedItem = SortByItems.FirstOrDefault(r => r.Value == sort);
			this._groupBySelectedItem = GroupByItems.FirstOrDefault(r => r.Value == group);
			this._iturListModeSelectedItem = IturListModeItems.FirstOrDefault(r => r.Value == mode);

            RaisePropertyChanged(() => SortBySelectedItem);
            RaisePropertyChanged(() => GroupBySelectedItem);

			this._eventAggregator.GetEvent<GroupConfigurationChangedEvent>().Publish(null);
			this._eventAggregator.GetEvent<SortConfigurationChangedEvent>().Publish(null);

			char currencySymbol = this._userSettingsManager.CurrencyGet();
			this._currencySelectedItem = CurrencyItems.FirstOrDefault(r => r.Symbol == currencySymbol);

			string barcodeType = this._userSettingsManager.BarcodeTypeGet();
			this._barcodeTypeSelectedItem = BarcodeTypeItems.FirstOrDefault(r => r == barcodeType);

			string printerType = this._userSettingsManager.PrinterGet();
			this._printerSelectedItem = PrinterItems.FirstOrDefault(r => r == printerType);

			this._barcodePrefix = this._userSettingsManager.BarcodePrefixGet();
            RaisePropertyChanged(() => BarcodePrefix);

			this._customerFilterCode = this._userSettingsManager.CustomerFilterCodeGet();
            RaisePropertyChanged(() => CustomerFilterCode);

			this._customerFilterName = this._userSettingsManager.CustomerFilterNameGet();
			RaisePropertyChanged(() => CustomerFilterName);

			this._useCustomerFilter = _userSettingsManager.UseCustomerFilterGet();
			RaisePropertyChanged(() => UseCustomerFilter);

			this._searchDialogIsModal = _userSettingsManager.SearchDialogIsModalGet();
			RaisePropertyChanged(() => SearchDialogIsModal);

			this._iturNamePrefix = this._userSettingsManager.IturNamePrefixGet();
            RaisePropertyChanged(() => IturNamePrefix);


			this._tagSubstring = this._userSettingsManager.TagSubstringGet();
			RaisePropertyChanged(() => TagSubstring);

            this._autoNavigateBack = _userSettingsManager.NavigateBackImportPdaFormGet();
            RaisePropertyChanged(() => AutoNavigateBack);

			this._copyFromSource = _userSettingsManager.CopyFromSourceGet(); //true
			RaisePropertyChanged(() => CopyFromSource);

			this._forwardResendDate = _userSettingsManager.ForwardResendDataGet(); //true
			RaisePropertyChanged(() => ForwardResendData);

			this._copyFromHost = _userSettingsManager.CopyFromHostGet(); //false
			RaisePropertyChanged(() => CopyFromHost);

			this._countingFromSource = _userSettingsManager.CountingFromSourceGet(); //true
			RaisePropertyChanged(() => CountingFromSource);


			this._sendToFtpOffice = _userSettingsManager.SendToFtpOfficeGet(); //false
			RaisePropertyChanged(() => SendToFtpOffice);

			this._forwardResendData = _userSettingsManager.ForwardResendDataGet(); //false
			RaisePropertyChanged(() => ForwardResendData);


			this._showMark = _userSettingsManager.ShowMarkGet(); //false
			RaisePropertyChanged(() => ShowMark);

			this._propertyIsEmpty = _userSettingsManager.PropertyIsEmptyGet(); //false
			this._propertyIsNotEmpty = _propertyIsNotEmpty = !this._propertyIsEmpty;
			RaisePropertyChanged(() => PropertyIsEmpty);
			RaisePropertyChanged(() => PropertyIsNotEmpty);
			//RaisePropertyChanged(() => PropertyIsEqual);
			//RaisePropertyChanged(() => PropertyIsNotEqual);

			

			this._copyByCodeInventor = _userSettingsManager.CopyByCodeInventorGet(); //true
			RaisePropertyChanged(() => CopyByCodeInventor);			

			
			
			this._importPDAPath = this._userSettingsManager.ImportPDAPathGet();
			RaisePropertyChanged(() => ImportPDAPath);

            this._importTCPPath = this._userSettingsManager.ImportTCPPathGet();
            RaisePropertyChanged(() => ImportTCPPath);

            this._tcpServerPort = this._userSettingsManager.TcpServerPortGet();
            RaisePropertyChanged(() => TcpServerPort);

            

            this._tcpServerOn = this._userSettingsManager.TcpServerOnGet();
            RaisePropertyChanged(() => TcpServerOn);


            this._webServiceLink = this._userSettingsManager.WebServiceLinkGet();
            RaisePropertyChanged(() => WebServiceLink);


            this._useToo = this._userSettingsManager.UseTooGet();
            RaisePropertyChanged(() => UseToo);

    
            


            this._webServiceDeveloperLink = this._userSettingsManager.WebServiceDeveloperLinkGet();
            RaisePropertyChanged(() => WebServiceDeveloperLink);

            this._exportPDAPath = this._userSettingsManager.ExportPDAPathGet();
			RaisePropertyChanged(() => ExportPDAPath);

            this._exportTCPPath = this._userSettingsManager.ExportTCPPathGet();
            RaisePropertyChanged(() => ExportTCPPath);

            this._host = this._userSettingsManager.HostGet();
			RaisePropertyChanged(() => Host);

			this._user = this._userSettingsManager.UserGet();
			RaisePropertyChanged(() => User);

			this._password = this._userSettingsManager.PasswordGet();
			RaisePropertyChanged(() => Password);

			this._mainDashboardBackground = this._userSettingsManager.DashboardHomeBackgroundGet();
            RaisePropertyChanged(() => MainDashboardBackground);
			this._mainDashboardBackgroundOpacity = this._userSettingsManager.DashboardHomeBackgroundOpacityGet();
            RaisePropertyChanged(() => MainDashboardBackgroundOpacity);

			this._customerDashboardBackground = this._userSettingsManager.DashboardCustomerBackgroundGet();
            RaisePropertyChanged(() => CustomerDashboardBackground);
			this._customerDashboardBackgroundOpacity = this._userSettingsManager.DashboardCustomerBackgroundOpacityGet();
            RaisePropertyChanged(() => CustomerDashboardBackgroundOpacity);

			this._branchDashboardBackground = this._userSettingsManager.DashboardBranchBackgroundGet();
            RaisePropertyChanged(() => BranchDashboardBackground);
			this._branchDashboardBackgroundOpacity = this._userSettingsManager.DashboardBranchBackgroundOpacityGet();
            RaisePropertyChanged(() => BranchDashboardBackgroundOpacity);

			this._inventorDashboardBackground = this._userSettingsManager.DashboardInventorBackgroundGet();
            RaisePropertyChanged(() => InventorDashboardBackground);
			this._inventorDashboardBackgroundOpacity = this._userSettingsManager.DashboardInventorBackgroundOpacityGet();
            RaisePropertyChanged(() => InventorDashboardBackgroundOpacity);

			this._reportRepositorySelectedItem = this._reportRepositoryItems.FirstOrDefault(
				r => r.Enum == this._userSettingsManager.ReportRepositoryGet()
                );

            RaisePropertyChanged(() => ReportRepositorySelectedItem);

			this._isShowERP = _userSettingsManager.ShowIturERPGet();
            RaisePropertyChanged(() => IsShowERP);

			this._isPackDataFileCatalog = _userSettingsManager.PackDataFileCatalogGet();
            RaisePropertyChanged(() => IsPackDataFileCatalog);

			this._iturFilterSelected = _iturFilterItems.FirstOrDefault(r => r.Value == this._userSettingsManager.IturFilterSelectedGet());
            RaisePropertyChanged(() => IturFilterSelected);

			this._iturFilterSortSelected = _iturFilterSortItems.FirstOrDefault(r => r.Value == this._userSettingsManager.IturFilterSortSelectedGet());
			RaisePropertyChanged(() => IturFilterSortSelected);

			this._iturFilterSortAZSelected = _iturFilterSortAZItems.FirstOrDefault(r => r.Value == this._userSettingsManager.IturFilterSortAZSelectedGet());
			RaisePropertyChanged(() => IturFilterSortAZSelected);

			this._inventProductFocusFilterSelected = this._inventProductFocusFilterItems.FirstOrDefault(r => r.Value == this._userSettingsManager.InventProductFilterFocusGet());
            RaisePropertyChanged(() => InventProductFocusFilterSelected);

			this._reportAppName = this._userSettingsManager.ReportAppNameGet();
            RaisePropertyChanged(() => ReportAppName);

			this._uploadOptionsHT630_AfterUploadRunExeFileList = this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileListGet();
			RaisePropertyChanged(() => UploadOptionsHT630_AfterUploadRunExeFileList);
			
				
			this._planEmptyColor = this._userSettingsManager.PlanEmptyColorGet();
            RaisePropertyChanged(() => PlanEmptyColor);
			this._planZeroColor = this._userSettingsManager.PlanZeroColorGet();
            RaisePropertyChanged(() => PlanZeroColor);
			this._planHundredColor = this._userSettingsManager.PlanHundredColorGet();
            RaisePropertyChanged(() => PlanHundredColor);

			this._inventProductMarkColor = this._userSettingsManager.InventProductMarkColorGet();
			RaisePropertyChanged(() => InventProductMarkColor);


			this._domainObjectSelectedItem = this._domainObjectItems.FirstOrDefault(r => r == this._userSettingsManager.DomainObjectSelectedItemGet());
			RaisePropertyChanged(() => DomainObjectSelectedItem);


			this._uploadWakeupTime = this._userSettingsManager.UploadWakeupTimeGet();
            RaisePropertyChanged(()=>UploadWakeupTime);

			this._uploadOptionsHT630_BaudratePDASelectedItems = 
				this._uploadOptionsHT630_BaudratePDAItems.FirstOrDefault(r => r == this._userSettingsManager.UploadOptionsHT630_BaudratePDAItemGet());
			RaisePropertyChanged(() => UploadOptionsHT630_BaudratePDASelectedItems);

			this._uploadOptionsRunMemorySelectedItems = this._uploadOptionsRunMemoryItems.FirstOrDefault(r => r == this._userSettingsManager.UploadOptionsRunMemoryItemGet());
			RaisePropertyChanged(() => UploadOptionsRunMemorySelectedItems);

			this._inventProductPropertyMarkSelectedItem = this._inventProductPropertyItems.FirstOrDefault(r => r == this._userSettingsManager.InventProductPropertyMarkSelectedItemGet());
			RaisePropertyChanged(() => InventProductPropertyMarkSelectedItem);

			this._inventProductPropertyFilterSelectedItem = this._inventProductPropertyItems.FirstOrDefault(r => r == this._userSettingsManager.InventProductPropertyFilterSelectedItemGet());
			RaisePropertyChanged(() => InventProductPropertyFilterSelectedItem);

			this._inventProductPropertyFilterSelectedNumberItem = this._inventProductPropertyNumberItems.FirstOrDefault(r => r == this._userSettingsManager.InventProductPropertyFilterSelectedNumberItemGet());
			RaisePropertyChanged(() => InventProductPropertyFilterSelectedNumberItem);

			this._inventProductPropertyPhotoSelectedItem = this._inventProductPropertyItems.FirstOrDefault(r => r == this._userSettingsManager.InventProductPropertyPhotoSelectedItemGet());
			RaisePropertyChanged(() => InventProductPropertyPhotoSelectedItem);

			this._inventProductPropertySelectedItem = this._inventProductPropertyItems.FirstOrDefault(r => r == this._userSettingsManager.InventProductPropertySelectedItemGet());
			RaisePropertyChanged(() => InventProductPropertySelectedItem);

			this._editorTemplateSelectedItem = this._editorTemplateItems.FirstOrDefault(r => r == this._userSettingsManager.EditorTemplateSelectedItemGet());
			RaisePropertyChanged(() => EditorTemplateSelectedItem);

			

			//this._uploadOptionsHT630_ExeptionFileNotDeleteSelectedItems = this._uploadOptionsHT630_ExeptionFileNotDeleteItems.FirstOrDefault(r => r == this._userSettingsManager.UploadOptionsHT630_ExeptionFileNotDeleteGet());
			//RaisePropertyChanged(() => UploadOptionsHT630_ExeptionFileNotDeleteSelectedItems);

			//this._uploadOptionsHT630_AfterUploadRunExeFileSelectedItems = this._uploadOptionsHT630_AfterUploadRunExeFileItems.FirstOrDefault(r => r == this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileGet());
			//RaisePropertyChanged(() => UploadOptionsHT630_AfterUploadRunExeFileSelectedItems);

			this._copyFromSource = this._userSettingsManager.CopyFromSourceGet();
			RaisePropertyChanged(() => CopyFromSource);

			this._forwardResendData = _userSettingsManager.ForwardResendDataGet(); //false
			RaisePropertyChanged(() => ForwardResendData);

			this._copyFromHost = this._userSettingsManager.CopyFromHostGet();
			RaisePropertyChanged(() => CopyFromHost);

	  		this._sendToFtpOffice = this._userSettingsManager.SendToFtpOfficeGet();
			RaisePropertyChanged(() => SendToFtpOffice);

			this._countingFromSource = this._userSettingsManager.CountingFromSourceGet();
			RaisePropertyChanged(() => CountingFromSource);


			this._copyByCodeInventor = _userSettingsManager.CopyByCodeInventorGet(); //true
			RaisePropertyChanged(() => CopyByCodeInventor);			
			

			this._uploadOptionsHT630_CurrentDataPDA = this._userSettingsManager.UploadOptionsHT630_CurrentDataPDAGet();
			RaisePropertyChanged(() => UploadOptionsHT630_CurrentDataPDA);

			this._uploadOptionsHT630_BaudratePDA = this._userSettingsManager.UploadOptionsHT630_BaudratePDAGet();
			RaisePropertyChanged(() => UploadOptionsHT630_BaudratePDA);

			
			this._uploadOptionsHT630_DeleteAllFilePDA = this._userSettingsManager.UploadOptionsHT630_DeleteAllFilePDAGet();
			RaisePropertyChanged(() => UploadOptionsHT630_DeleteAllFilePDA);

			this._uploadOptionsHT630_ExeptionFileNotDelete = this._userSettingsManager.UploadOptionsHT630_ExeptionFileNotDeleteGet();
			RaisePropertyChanged(() => UploadOptionsHT630_ExeptionFileNotDelete);
	
			this._uploadOptionsHT630_AfterUploadPerformWarmStart = this._userSettingsManager.UploadOptionsHT630_AfterUploadPerformWarmStartGet();
			RaisePropertyChanged(() => UploadOptionsHT630_AfterUploadPerformWarmStart);

			this._uploadOptionsHT630_AfterUploadRunExeFileNeedDo = this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileNeedDoGet();
			RaisePropertyChanged(() => UploadOptionsHT630_AfterUploadRunExeFileNeedDo);

			

        }

        private void BuildColors()
        {
            foreach (var viewModel in this._statusColors)
            {
                viewModel.PropertyChanged -= StatusColorItemPropertyChanged;
            }

            this._statusColors.Clear();

            foreach (IturStatusEnum status in Enum.GetValues(typeof(IturStatusEnum)))
            {
                StatusColorItemViewModel viewModel = new StatusColorItemViewModel();
                viewModel.Status = UtilsStatus.FromIturStatusToString(status);
                string settingsColors = this._userSettingsManager.StatusColorGet(status.ToString());
                viewModel.Color = String.IsNullOrEmpty(settingsColors) ?
                                          UserSettingsHelpers.StatusDefaultColorGet(status) :
                                          UserSettingsHelpers.StringToColor(settingsColors);
                viewModel.EnumStatus = status;

                viewModel.PropertyChanged += StatusColorItemPropertyChanged;

                this._statusColors.Add(viewModel);
            }
        }

        private void BuildColorGroups()
        {
            foreach (var viewModel in this._statusGroupColors)
            {
                viewModel.PropertyChanged -= StatusColorGroup_PropertyChanged;
            }

            this._statusGroupColors.Clear();

            foreach (IturStatusGroupEnum statusGroup in Enum.GetValues(typeof(IturStatusGroupEnum)))
            {
                StatusGroupColorItemViewModel viewModel = new StatusGroupColorItemViewModel();
                viewModel.StatusGroup = UtilsStatus.FromIturStatusGroupToString(statusGroup);
                string settingsColors = this._userSettingsManager.StatusGroupColorGet(statusGroup.ToString());
                viewModel.Color = String.IsNullOrEmpty(settingsColors) ?
                                                                           UserSettingsHelpers.StatusGroupDefaultColorGet(statusGroup) :
                                                                           UserSettingsHelpers.StringToColor(settingsColors);
                viewModel.EnumStatusGroup = statusGroup;

                viewModel.PropertyChanged += StatusColorGroup_PropertyChanged;

                this._statusGroupColors.Add(viewModel);
            }
        }

        private void ResetCommandExecuted()
        {
            using (new CursorWait())
            {
                //portions
                this.PortionItursDashboard = CommonElement.DefaultItursPortion;
                this.PortionItursList = CommonElement.DefaultItursPortionList;
                this.PortionCBI = CommonElement.DefaultCBIPortion;
                this.PortionInventProducts = CommonElement.DefaultInventProductsPortion;
                this.PortionProducts = CommonElement.DefaultProductsPortion;
                this.PortionSections = CommonElement.DefaultSectionsPortion;
                this.PortionSuppliers = CommonElement.DefaultSuppliersPortion;
				this.PortionFamilys = CommonElement.DefaultFamilysPortion;
				

                //statuses
                foreach (IturStatusEnum status in Enum.GetValues(typeof(IturStatusEnum)))
                {
                    Color color = UserSettingsHelpers.StatusDefaultColorGet(status);
                    this._userSettingsManager.StatusColorSet(status.ToString(), UserSettingsHelpers.ColorToString(color));

                    StatusColorItemViewModel viewModel = this._statusColors.FirstOrDefault(r => r.EnumStatus == status);
                    if (viewModel != null)
                        viewModel.Color = color;
                }

                foreach (IturStatusGroupEnum statusGroup in Enum.GetValues(typeof(IturStatusGroupEnum)))
                {
                    Color color = UserSettingsHelpers.StatusGroupDefaultColorGet(statusGroup);
                    this._userSettingsManager.StatusGroupColorSet(statusGroup.ToString(), UserSettingsHelpers.ColorToString(color));

                    StatusGroupColorItemViewModel viewModel = this._statusGroupColors.FirstOrDefault(r => r.EnumStatusGroup == statusGroup);
                    if (viewModel != null)
                        viewModel.Color = color;
                }

                //ILog
                IsLogSimpleTrace = true;
                IsLogTrace = true;
                IsLogTraceParser = true;
                IsLogTraceRepository = true;
                IsLogTraceProvider = true;
                IsLogWarning = true;
                IsLogWarningParser = true;
                IsLogWarningRepository = true;
                IsLogTraceParserResult = true;
                IsLogTraceRepositoryResult = true;
                IsLogTraceProviderResult = true;

                this.EncodingSelectedItem = this._encodingItems.FirstOrDefault(r => r.Encoding == UserSettingsHelpers.GlobalEncodingDefaultGet());
                this.LanguageSelectedItem = this._languageItems.FirstOrDefault(r => r.Language == enLanguage.English);

                this.Delay = CommonElement.DefaultDelay;

                SortBySelectedItem = SortByItems.FirstOrDefault(r => r.Value == ComboValues.SortItur.NumberValue);
                GroupBySelectedItem = GroupByItems.FirstOrDefault(r => r.Value == ComboValues.GroupItur.LocationValue);
                IturListModeSelectedItem = IturListModeItems.FirstOrDefault(r => r.Value == ComboValues.IturListDetailsMode.ModePaged);

                CurrencySelectedItem = CurrencyItems.FirstOrDefault(r => r.Symbol == Common.Constants.CurrencySymbol.SHEQEL);

                BarcodeTypeSelectedItem = BarcodeTypeItems.FirstOrDefault(r => r == CommonElement.DefaultBarcodeType);
				PrinterSelectedItem = PrinterItems.FirstOrDefault(r => r == CommonElement.DefaultPrinter);

                BarcodePrefix = CommonElement.DefaultBarcodePrefix;
                IturNamePrefix = CommonElement.DefaultIturNamePrefix;

				CustomerFilterCode = CommonElement.DefaultCustomerFilterCode;
				CustomerFilterName = CommonElement.DefaultCustomerFilterName;
				UseCustomerFilter = CommonElement.DefaultUseCustomerFilter;
				SearchDialogIsModal = CommonElement.DefaultSearchDialogIsModal;

                AutoNavigateBack = CommonElement.DefaultNavigateBackImportPdaForm;

				CopyFromSource = CommonElement.DefaultCopyFromSource;
				CountingFromSource = CommonElement.DefaultCountingFromSource;
				CopyFromHost = CommonElement.DefaultCopyFromHost;
				ForwardResendData = CommonElement.DefaultForwardResendData;
				TagSubstring = CommonElement.DefaultTagSubstring;
				SendToFtpOffice = CommonElement.DefaultSendToFtpOffice;

				ShowMark = CommonElement.DefaultShowMark;

				ImportPDAPath = CommonElement.DefaultImportPDAPath;
				ExportPDAPath = CommonElement.DefaultExportPDAPath;
				Host = CommonElement.DefaultHost;
				User = CommonElement.DefaultUser;
				Password = CommonElement.DefaultPassword;
                WebServiceLink = CommonElement.DefaultWebServiceLink;
                WebServiceDeveloperLink = CommonElement.DefaultWebServiceDeveloperLink;
                UseToo = CommonElement.DefaultUseToo;
                TcpServerOn = CommonElement.DefaultTcpServerOn;

                MainDashboardBackground = String.Empty;
                MainDashboardBackgroundOpacity = CommonElement.DefaultDashboardBackgroundOpacity;
                CustomerDashboardBackground = String.Empty;
                CustomerDashboardBackgroundOpacity = CommonElement.DefaultDashboardBackgroundOpacity;
                BranchDashboardBackground = String.Empty;
                BranchDashboardBackgroundOpacity = CommonElement.DefaultDashboardBackgroundOpacity;
                InventorDashboardBackground = String.Empty;
                InventorDashboardBackgroundOpacity = CommonElement.DefaultDashboardBackgroundOpacity;
                //ReportRepositorySelectedItem = _reportRepositoryItems.FirstOrDefault(r => r.Enum == IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository);
				ReportRepositorySelectedItem = _reportRepositoryItems.FirstOrDefault(r => r.Enum == IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository);

                IsShowERP = CommonElement.DefaultShowIturERP;

				IsPackDataFileCatalog = CommonElement.DefaultPackDataFileCatalog;

                IturFilterSelected = _iturFilterItems.FirstOrDefault(r => r.Value == CommonElement.DefaultIturFilterSelected);
				IturFilterSortSelected = _iturFilterSortItems.FirstOrDefault(r => r.Value == CommonElement.DefaultIturFilterSortSelected);
				IturFilterSortAZSelected = _iturFilterSortAZItems.FirstOrDefault(r => r.Value == CommonElement.DefaultIturFilterSortAZSelected);

				
                InventProductFocusFilterSelected = _inventProductFocusFilterItems.FirstOrDefault(r => r.Value == CommonElement.DefaultInventProductFilterFocus);

                ReportAppName = CommonElement.DefaultReportAppName;
			
				

                PlanEmptyColor = ColorParser.StringToColor(CommonElement.DefaultPlanEmptyColor);
                PlanZeroColor = ColorParser.StringToColor(CommonElement.DefaultPlanZeroColor);
                PlanHundredColor = ColorParser.StringToColor(CommonElement.DefaultPlanHundredColor);
				InventProductMarkColor = ColorParser.StringToColor(CommonElement.DefaultInventProductMarkColor);

                UploadWakeupTime = CommonElement.DefaultUploadWakeupTime;
				UploadOptionsHT630_BaudratePDA = CommonElement.DefaultUploadOptionsHT630_BaudratePDA;
				//UploadOptionsHT630_BaudratePDASelectedItems = CommonElement.DefaultUploadOptionsHT630_BaudratePDAItem;
				UploadOptionsHT630_CurrentDataPDA = CommonElement.DefaultUploadOptionsHT630_CurrentDataPDA;
				UploadOptionsHT630_DeleteAllFilePDA = CommonElement.DefaultUploadOptionsHT630_DeleteAllFilePDA;
				UploadOptionsHT630_AfterUploadPerformWarmStart = CommonElement.DefaultUploadOptionsHT630_AfterUploadPerformWarmStart;
				UploadOptionsHT630_AfterUploadRunExeFileNeedDo = CommonElement.DefaultUploadOptionsHT630_AfterUploadRunExeFileNeedDo;
				UploadOptionsHT630_ExeptionFileNotDelete = CommonElement.DefaultUploadOptionsHT630_ExeptionFileNotDelete;
                OnOnResetToDefault(null);
            }
        }

        private void SetLogType(MessageTypeEnum type, bool isEnabled)
        {
            this._userSettingsManager.LogTypeSet(type.ToString(), isEnabled);

            this._iLog.SetIncludeMessageType(UserSettingsHelpers.LogTypeListGet(this._userSettingsManager).Where(r => r.Value).Select(r => r.Key).ToList());
        }

        private void BuildEncoding()
        {
            List<Encoding> list = new List<Encoding>();

            Encoding def = UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager);
            if (def != null)
                list.Add(def);

            foreach (Encoding enc in new List<Encoding>
                                       {
                                           Encoding.GetEncoding(862), Encoding.GetEncoding(1255),
                                           Encoding.ASCII, Encoding.Unicode, Encoding.UTF8, Encoding.UTF32, Encoding.UTF7
                                       })
            {
                if (!list.Contains(enc))
                    list.Add(enc);
            }

            foreach (Encoding enc in Encoding.GetEncodings().OrderBy(r => r.DisplayName).Select(r => r.GetEncoding()))
            {
                if (!list.Contains(enc))
                    list.Add(enc);
            }

            this._encodingItems.Clear();
            foreach (Encoding encoding in list)
            {
                EncodingItemViewModel item = new EncodingItemViewModel(encoding);
                this._encodingItems.Add(item);
            }
        }

        private void ConfigurationSetAddCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.ConfigurationSetAddView;
            payload.WindowTitle = WindowTitles.ConfigurationSetAdd;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);

            payload.Callback = r =>
            {
                string fileName = r as String;

                if (!String.IsNullOrWhiteSpace(fileName))
                {
                    string withExtension = String.Format("{0}{1}", fileName, FileSystem.ConfigSetFileExtension);
                    this._userSettingsManager.AdminAddConfiguration(withExtension);
                    ConfigurationSetItemViewModel viewModel = new ConfigurationSetItemViewModel
                                                                  {
                                                                      File = withExtension,
                                                                      Text = withExtension.Replace(FileSystem.ConfigSetFileExtension, String.Empty)
                                                                  };
                    this._configurationSetItems.Add(viewModel);
                    this.ConfigurationSetSelectedItem = viewModel;
                }
            };

            OnModalWindowRequest(payload);
        }

        private void BuildConfigItems()
        {
            foreach (string conf in this._userSettingsManager.AdminListConfiguration())
            {
                this._configurationSetItems.Add(new ConfigurationSetItemViewModel() { File = conf, Text = conf.Replace(FileSystem.ConfigSetFileExtension, String.Empty) });
            }
        }

        private void SelectLoggingSetFromCheckBoxes()
        {
            if (
                _isLogSimpleTrace == true &&
                _isLogTrace == false &&
                _isLogTraceParser == false &&
                _isLogTraceRepository == false &&
                _isLogTraceProvider == false &&
                _isLogWarning == false &&
                _isLogWarningParser == false &&
                _isLogWarningRepository == false &&
                _isLogTraceParserResult == false &&
                _isLogTraceRepositoryResult == false &&
                _isLogTraceProviderResult == false
                )
            {
                this._loggingSetSelectedItem = this._loggingSetItems.FirstOrDefault(r => r.Value == LoggingSetSimple);
                RaisePropertyChanged(() => LoggingSetSelectedItem);
                return;
            }

            if (
                  _isLogSimpleTrace == true &&
                _isLogTrace == true &&
              _isLogTraceParser == true &&
              _isLogTraceRepository == true &&
              _isLogTraceProvider == true &&
              _isLogWarning == true &&
              _isLogWarningParser == true &&
              _isLogWarningRepository == true &&
              _isLogTraceParserResult == true &&
              _isLogTraceRepositoryResult == true &&
              _isLogTraceProviderResult == true
              )
            {
                this._loggingSetSelectedItem = this._loggingSetItems.FirstOrDefault(r => r.Value == LoggingSetDebug);
                RaisePropertyChanged(() => LoggingSetSelectedItem);
                return;
            }

            if (
                    _isLogSimpleTrace == false &&
                _isLogTrace == false &&
             _isLogTraceParser == false &&
             _isLogTraceRepository == false &&
             _isLogTraceProvider == false &&
             _isLogWarning == true &&
             _isLogWarningParser == true &&
             _isLogWarningRepository == true &&
             _isLogTraceParserResult == true &&
             _isLogTraceRepositoryResult == true &&
             _isLogTraceProviderResult == true
             )
            {
                this._loggingSetSelectedItem = this._loggingSetItems.FirstOrDefault(r => r.Value == LoggingSetInfo);
                RaisePropertyChanged(() => LoggingSetSelectedItem);
                return;
            }

            //default
            this._loggingSetSelectedItem = this._loggingSetItems.FirstOrDefault(r => r.Value == LoggingSetConfigurable);
            RaisePropertyChanged(() => LoggingSetSelectedItem);
        }

        private void SelectCheckBoxesFromLoggingSet()
        {
            if (_loggingSetSelectedItem == null) return;

            using (new CursorWait())
            {
                switch (_loggingSetSelectedItem.Value)
                {
                    case LoggingSetSimple:
                        IsLogSimpleTrace = true;
                        IsLogTrace = false;
                        IsLogTraceParser = false;
                        IsLogTraceRepository = false;
                        IsLogTraceProvider = false;
                        IsLogWarning = false;
                        IsLogWarningParser = false;
                        IsLogWarningRepository = false;
                        IsLogTraceParserResult = false;
                        IsLogTraceRepositoryResult = false;
                        IsLogTraceProviderResult = false;
                        break;

                    case LoggingSetInfo:
                        IsLogSimpleTrace = false;
                        IsLogTrace = false;
                        IsLogTraceParser = false;
                        IsLogTraceRepository = false;
                        IsLogTraceProvider = false;
                        IsLogWarning = true;
                        IsLogWarningParser = true;
                        IsLogWarningRepository = true;
                        IsLogTraceParserResult = true;
                        IsLogTraceRepositoryResult = true;
                        IsLogTraceProviderResult = true;
                        break;

                    case LoggingSetDebug:
                        IsLogSimpleTrace = true;
                        IsLogTrace = true;
                        IsLogTraceParser = true;
                        IsLogTraceRepository = true;
                        IsLogTraceProvider = true;
                        IsLogWarning = true;
                        IsLogWarningParser = true;
                        IsLogWarningRepository = true;
                        IsLogTraceParserResult = true;
                        IsLogTraceRepositoryResult = true;
                        IsLogTraceProviderResult = true;
                        break;

                    case LoggingSetConfigurable:
                        Dictionary<MessageTypeEnum, bool> logMessages = UserSettingsHelpers.LogTypeListGet(this._userSettingsManager);

                        IsLogSimpleTrace = logMessages[MessageTypeEnum.SimpleTrace];
                        IsLogTrace = logMessages[MessageTypeEnum.Trace];
                        IsLogTraceParser = logMessages[MessageTypeEnum.TraceParser];
                        IsLogTraceRepository = logMessages[MessageTypeEnum.TraceRepository];
                        IsLogTraceProvider = logMessages[MessageTypeEnum.TraceProvider];
                        IsLogWarning = logMessages[MessageTypeEnum.Warning];
                        IsLogWarningParser = logMessages[MessageTypeEnum.WarningParser];
                        IsLogWarningRepository = logMessages[MessageTypeEnum.WarningRepository];
                        IsLogTraceParserResult = logMessages[MessageTypeEnum.TraceParserResult];
                        IsLogTraceRepositoryResult = logMessages[MessageTypeEnum.TraceRepositoryResult];
                        IsLogTraceProviderResult = logMessages[MessageTypeEnum.TraceProviderResult];
                        break;
                }
            }
        }

        private void BuildBarcodeTypes()
        {
            foreach (string name in Enum.GetNames(typeof(BarcodeSymbology)))
            {
                _barcodeTypeItems.Add(name);
            }
        }

		private void BuildPrinters()
		{
			foreach (string s in PrinterSettings.InstalledPrinters)
			{
				_printerItems.Add(s);
			}
		}

		//private void ExportPDAPathBrowseCommandExecuted()
		//{
		//	OpenBackgroundImageDialog(r => ExportPDAPath = r);
		//}
		//private void ImportPDAPathBrowseCommandExecuted()
		//{
		//	OpenBackgroundImageDialog(r => ImportPDAPath = r);
		//}

        private void MainDashboardBackgroundBrowseCommandExecuted()
        {
            OpenBackgroundImageDialog(r => MainDashboardBackground = r);
        }
        private void CustomerDashboardBackgroundBrowseCommandExecuted()
        {
            OpenBackgroundImageDialog(r => CustomerDashboardBackground = r);
        }
        private void BranchDashboardBackgroundBrowseCommandExecuted()
        {
            OpenBackgroundImageDialog(r => BranchDashboardBackground = r);
        }
        private void InventorDashboardBackgroundBrowseCommandExecuted()
        {
            OpenBackgroundImageDialog(r => InventorDashboardBackground = r);
        }

        private void OpenBackgroundImageDialog(Action<string> afterAction)
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();
            notification.DefaultExt = ".jpg";
            notification.Filter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

            Tuple<bool, string> dialogResult = UtilsMisc.OpenFileDialog(notification);
            bool isOk = dialogResult.Item1;
            string path = dialogResult.Item2;

            if (!isOk)
                return;

            afterAction(path);
        }
    }
}