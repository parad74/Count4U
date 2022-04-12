using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Configuration.Dynamic;
using Count4U.Configuration.Interfaces;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.Events;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using System.Windows;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Practices.ServiceLocation;
using NLog;
using System.Reactive.Linq;
using System.Reactive;
using Codeplex.Reactive;
using Count4U.Common.Extensions;
using System.Diagnostics;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Media;
using Count4U.Model.Interface;
using System.IO;
using System.Threading;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductListDetailsViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private Object documentHeaderItemsClearLock = new Object();
		private Object documentHeaderItemsAddLock = new Object();
		private Object documentHeaderItemsRefreshLock = new Object();

		private Object inventProductItemsClearLock = new Object();
		private Object inventProductItemsAddLock = new Object();
		private Object inventProductItemsRefreshLock = new Object();
		private Stopwatch _stopwatch;

        private static readonly System.Type InventProductType = typeof(Count4U.Model.Count4U.InventProduct);

        #region Constants

        private const ViewDomainContextEnum ReportContextItur = ViewDomainContextEnum.ItursItur;
        private const ViewDomainContextEnum ReportContextDocument = ViewDomainContextEnum.ItursIturDoc;
        private const ViewDomainContextEnum ReportContextInventProduct = ViewDomainContextEnum.ItursIturDocPDA;

        #endregion

        #region Private fields
        private readonly IServiceLocator _serviceLocator;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IDocumentHeaderRepository _documentHeaderRepository;
        private readonly IInventProductRepository _inventProductRepository;
        private readonly IStatusIturRepository _statusIturRepository;
        private readonly IStatusIturGroupRepository _statusGroupRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IIturRepository _iturRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly UICommandRepository _commandRepository;
        private readonly UICommandRepository<DocumentHeaderItemViewModel> _commandRepositoryDocHeader;
		private readonly UICommandRepository<InventProductItemViewModel> _commandRepositoryObjectInventProduct;
        private readonly EditorTemplateComboViewModel _productEditorComboViewModel;
		private readonly ITemporaryInventoryRepository _temporaryInventoryRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
		protected readonly IDBSettings _dbSettings;

        //document headers
        private readonly DelegateCommand _searchCommand;
        private readonly DelegateCommand _reportCommand;
        private readonly DelegateCommand _editDocumentHeaderCommand;
		private readonly DelegateCommand _changeIturNameCommand;
		private readonly DelegateCommand _cloneDocumentHeaderCommand;
		
        private readonly DelegateCommand _addDocumentHeaderCommand;
        private readonly DelegateCommand _deleteDocumentHeaderCommand;
        private readonly DelegateCommand _reportDocumentHeaderCommand;
        private readonly DelegateCommand _refreshDocumentHeaderCommand;


        private readonly DelegateCommand<DocumentHeaderItemViewModel> _resetErrorDocumentHeaderNotSelectedCommand;
        private readonly DelegateCommand<DocumentHeaderItemViewModel> _clearDocumentHeaderNotSelectedCommand;
		private readonly DelegateCommand _resetErrorDocumentHeaderCommand;

        private DelegateCommand _iturDisableCommand;
        private string _iturEnableDisableStatus;
        private bool _iturEnabled;

        //invent products
        private readonly DelegateCommand _addInventProductCommand;
        private readonly DelegateCommand _deleteInventProductCommand;
		private readonly DelegateCommand _deleteAllCheckSelectInventProductCommand;
        private readonly DelegateCommand _editInventProductCommand;
        private readonly DelegateCommand _cloneCommand;
		private readonly DelegateCommand _copyCommand;
        private readonly DelegateCommand _resetInventProductCommand;
        private readonly DelegateCommand _reportInventProductCommand;
        private readonly DelegateCommand _refreshInventProductCommand;
        private readonly DelegateCommand _resetQuantityInventProductCommand;

        private readonly DelegateCommand<InventProductItemViewModel> _editCommand;
        private readonly DelegateCommand<InventProductItemViewModel> _cancelCommand;
        private readonly DelegateCommand<InventProductItemViewModel> _commitCommand;
		private readonly DelegateCommand<InventProductItemViewModel> _showImageCommand;

		private readonly DelegateCommand<InventProductPropertyStrItemViewModel> _editDetailGridCommand;
		private readonly DelegateCommand<InventProductPropertyStrItemViewModel> _cancelDetailGridCommand;
		private readonly DelegateCommand<InventProductPropertyStrItemViewModel> _commitDetailGridCommand;


        private bool _isOnlyWithError;

        private readonly DelegateCommand _resetErrorInventProductSelectedCommand;

        private readonly ObservableCollection<DocumentHeaderItemViewModel> _documentHeaderItems;
        private readonly ObservableCollection<InventProductItemViewModel> _inventProductItems;
        private DocumentHeaderItemViewModel _documentHeaderSelectedItem;
        private InventProductItemViewModel _inventProductSelectedItem;

		private bool _checkSelect;

        private readonly DelegateCommand _nextIturCommand;
        private readonly DelegateCommand _prevIturCommand;

		private readonly DelegateCommand _nextErpIturCommand;
		private readonly DelegateCommand _prevErpIturCommand;

        private readonly DelegateCommand _iturReportCommand;
        private readonly DelegateCommand _iturRefreshCommand;

        private readonly DelegateCommand _printReportCommand;
		//private readonly DelegateCommand _printReportByLocationCodeCommand;



        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

        private readonly ObservableCollection<string> _searchItems;
        private string _searchExpression;
        private string _searchItem;

        private int _itemsTotal;
        private int _pageCurrent;
        private int _pageSize;

		private string _iturFastPrintReportCode;
		
        private Itur _itur;

        //blue region
        private readonly ObservableCollection<InventProductIturInfoItem> _iturInfo;
        private string _iturStatusColor;
        private ReactiveProperty<string> _totalNumberOfItems;

        private List<string> _iturCodes;
        private bool _iturCodesNotFull;
        private List<string> _itursCodesFullCache;

        private bool _isGeneratedByCode;
        private bool _isDocumentSelectionGenerateProductsBuild;

        private readonly ReportButtonViewModel _reportButtonItur;
        private readonly ReportButtonViewModel _reportButtonDocument;
        private readonly ReportButtonViewModel _reportButtonInventProduct;

        private Action _afterBuildAction;

        private bool _isBusy;

        private string _currentIturCode;

		private readonly IUnityContainer _unityContainer;
		private string _adapterFromPDA;
		private string _adapterCode;

		private bool _isDataGridProperyVisibility;
		//private bool _isPhotoProperyVisibility;
		private readonly DispatcherTimer _timer;

		private InventProductPropertyStrItemViewModel _detailSelectedItem;
		private readonly ObservableCollection<InventProductPropertyStrItemViewModel> _detailItems;
		//private Dictionary<string, string> _dictionaryNamePropertyStr;
		private Dictionary<string, PropertyStr> _dictionaryObjectPropertyStr;
		private IPropertyStrRepository _propertyStrRepository;

        #endregion

        #region Constructor

        public InventProductListDetailsViewModel(
            IServiceLocator serviceLocator,
            IDocumentHeaderRepository documentHeaderRepository,
            IInventProductRepository inventProductRepository,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IIturRepository iturRepository,
            IUserSettingsManager userSettingsManager,
            IRegionManager regionManager,
            IStatusIturRepository statusIturRepository,
            IStatusIturGroupRepository statusGroupRepository,
            INavigationRepository navigationRepository,
            ILocationRepository locationRepository,
            ReportButtonViewModel reportButtonItur,
            ReportButtonViewModel reportButtonDocument,
            ReportButtonViewModel reportButtonInventProduct,
            UICommandRepository commandRepository,
			IUnityContainer unityContainer,
            UICommandRepository<DocumentHeaderItemViewModel> commandRepositoryDocHeader,
		   UICommandRepository<InventProductItemViewModel> commandRepositoryObjectInventProduct,
            EditorTemplateComboViewModel productEditorComboViewModel,
            ModalWindowLauncher modalWindowLauncher,
			IPropertyStrRepository propertyStrRepository,
			IDBSettings dbSettings)
            : base(contextCBIRepository)
        {
            this._serviceLocator = serviceLocator;
            this._productEditorComboViewModel = productEditorComboViewModel;
            this._commandRepositoryDocHeader = commandRepositoryDocHeader;
			this._commandRepositoryObjectInventProduct = commandRepositoryObjectInventProduct;
            this._commandRepository = commandRepository;
            this._reportButtonItur = reportButtonItur;
            this._reportButtonInventProduct = reportButtonInventProduct;
            this._reportButtonDocument = reportButtonDocument;
            this._locationRepository = locationRepository;
            this._navigationRepository = navigationRepository;
            this._statusGroupRepository = statusGroupRepository;
            this._statusIturRepository = statusIturRepository;
            this._regionManager = regionManager;
            this._userSettingsManager = userSettingsManager;
            this._iturRepository = iturRepository;
			this._temporaryInventoryRepository = temporaryInventoryRepository;
            this._eventAggregator = eventAggregator;
            this._inventProductRepository = inventProductRepository;
            this._documentHeaderRepository = documentHeaderRepository;
            this._modalWindowLauncher = modalWindowLauncher;
			this._unityContainer = unityContainer;
			this._propertyStrRepository = propertyStrRepository;
			this._dbSettings = dbSettings;

            this._searchItems = new ObservableCollection<string>() { ComboValues.FindInventProduct.FilterMakat, ComboValues.FindInventProduct.FilterBarcode, ComboValues.FindInventProduct.FilterProduct };
            this._searchItem = ComboValues.FindInventProduct.FilterBarcode;

            this._documentHeaderItems = new ObservableCollection<DocumentHeaderItemViewModel>();
            this._inventProductItems = new ObservableCollection<InventProductItemViewModel>();
			this._detailItems = new ObservableCollection<InventProductPropertyStrItemViewModel>();


            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._addInventProductCommand = _commandRepository.Build(enUICommand.Add, this.AddInventProductCommandExecuted, this.AddInventProductCommandCanExecute);
            this._deleteInventProductCommand = _commandRepository.Build(enUICommand.Delete, this.DeleteInventProductCommandExecute, this.DeleteInventProductCommandCanExecute);
			this._deleteAllCheckSelectInventProductCommand = _commandRepository.Build(enUICommand.MultiDelete, this.DeleteAllCheckSelectInventProductCommandExecuted, this.DeleteAllCheckSelectInventProductCommandCanExecuted);
            this._editInventProductCommand = _commandRepository.Build(enUICommand.Edit, this.EditInventProductCommandExecuted, this.EditInventProductCommandCanExecute);
            this._cloneCommand = _commandRepository.Build(enUICommand.Clone, this.CloneCommandExecuted, this.CloneCommandCanExecute);
			this._copyCommand = _commandRepository.Build(enUICommand.Copy, this.CopyCommandExecuted, this.CopyCommandCanExecute);
					  
	        this._addDocumentHeaderCommand = _commandRepository.Build(enUICommand.Add, this.AddDocumentHeaderCommandExecuted);
            this._deleteDocumentHeaderCommand = _commandRepository.Build(enUICommand.Delete, this.DeleteDocumentHeaderCommandExecuted, this.DeleteDocumentHeaderCommandCanExecute);
            this._editDocumentHeaderCommand = _commandRepository.Build(enUICommand.Edit, this.EditDocumentHeaderCommandExecuted, this.EditDocumentHeaderCommandCanExecute);
			this._cloneDocumentHeaderCommand = _commandRepository.Build(enUICommand.Clone, this.CloneDocumentHeaderCommandExecuted, this.CloneDocumentHeaderCommandCanExecute);
		

            this._resetErrorInventProductSelectedCommand = _commandRepository.Build(enUICommand.ResetBit, this.ResetErrorInventProductSelectedExecuted, this.ResetErrorInventProductSelectedCanExecute);

            this._nextIturCommand = new DelegateCommand(this.NextIturCommandExecuted, this.NextIturCommandCanExecute);
            this._prevIturCommand = new DelegateCommand(this.PrevIturCommandExecuted, this.PrevIturCommandCanExecute);

			this._nextErpIturCommand = new DelegateCommand(this.NextErpIturCommandExecuted, this.NextErpIturCommandCanExecute);
			this._prevErpIturCommand = new DelegateCommand(this.PrevErpIturCommandExecuted, this.PrevErpIturCommandCanExecute);


            this._reportDocumentHeaderCommand = new DelegateCommand(this.ReportDocumentHeaderCommandExecuted, ReportDocumentHeaderCanExecute);
            this._reportInventProductCommand = new DelegateCommand(this.ReportInventProductCommandExecuted);
            this._iturReportCommand = new DelegateCommand(this.ReportIturCommandExecuted);

            this._iturInfo = new ObservableCollection<InventProductIturInfoItem>();

            this._iturRefreshCommand = _commandRepository.Build(enUICommand.RefreshStatus, this.IturRefreshCommandExecuted);
			this._changeIturNameCommand = _commandRepository.Build(enUICommand.ChangeIturName, this.ChangeIturNameCommandExecuted);


            this._refreshDocumentHeaderCommand = _commandRepository.Build(enUICommand.RefreshStatus, this.DocumentHeaderRefreshCommandExecuted);
            this._refreshInventProductCommand = _commandRepository.Build(enUICommand.RefreshStatus, this.InventProductRefreshCommandExecuted);
            this._resetInventProductCommand = _commandRepository.Build(enUICommand.Reset, this.ResetInventProductCommandExecuted, ResetProductCommandCanExecute);
            this._resetQuantityInventProductCommand = _commandRepository.Build(enUICommand.Undo, ResetQuantityInventProductCommandExecuted, ResetQuantityInventProductCommandCanExecute);
			this._resetErrorDocumentHeaderCommand = _commandRepository.Build(enUICommand.Reset, ResetErrorDocumentHeaderExecuted, ResetErrorDocumentHeaderCanExecuted);

            this._resetErrorDocumentHeaderNotSelectedCommand = _commandRepositoryDocHeader.Build(enUICommand.Reset, ResetErrorDocumentHeaderNotSelectedExecuted);
	          this._clearDocumentHeaderNotSelectedCommand = _commandRepositoryDocHeader.Build(enUICommand.Clear, ClearDocumentHeaderNotSelectedExecuted);
            this._isDocumentSelectionGenerateProductsBuild = true;
            this._searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });
            this._reportCommand = _commandRepository.Build(enUICommand.Report, delegate { });

			this._printReportCommand = _commandRepository.Build(enUICommand.PrintReport, PrintReportCommandExecuted);
		//	this._printReportByLocationCodeCommand = _commandRepository.Build(enUICommand.PrintReportByLocationCode, PrintReportByLocationCodeCommandExecuted);

			this._editCommand = new DelegateCommand<InventProductItemViewModel>(EditCommandExecuted);
			this._cancelCommand = new DelegateCommand<InventProductItemViewModel>(CancelCommandExecuted);
			this._commitCommand = new DelegateCommand<InventProductItemViewModel>(CommitCommandExecuted);
			this._showImageCommand = _commandRepositoryObjectInventProduct.Build(enUICommand.ShowImage, this.ShowImageCommandExecuted);

			this._editDetailGridCommand = new DelegateCommand<InventProductPropertyStrItemViewModel>(EditDetailGridCommandExecuted);
			this._cancelDetailGridCommand = new DelegateCommand<InventProductPropertyStrItemViewModel>(CancelDetailGridCommandExecuted);
			this._commitDetailGridCommand = new DelegateCommand<InventProductPropertyStrItemViewModel>(CommitDetailGridCommandExecuted);

			this._iturDisableCommand = new DelegateCommand(IturDisableCommandExecuted);

			

			this._isDataGridProperyVisibility = false;
			//this._isPhotoProperyVisibility = false;
			

			this._timer = new DispatcherTimer();
			this._timer.Interval = TimeSpan.FromMilliseconds(this._userSettingsManager.DelayGet());
			this._timer.Tick += Timer_Tick;

			this._checkSelect = false;

        }

        #endregion

      
        #region public properties
		private string _goToErpIturCode;
		public string GoToErpIturCode
		{
			get { return this._goToErpIturCode; }
			set
			{
				string val = value;
				Itur itur = this._iturRepository.GetIturByErpIturCode(value, base.GetDbPath);
				if (itur == null)
				{
					return;
				} 
				this._goToErpIturCode = val;
				this.RaisePropertyChanged(() => this.GoToErpIturCode);
				if (this._currentIturCode != itur.IturCode)
				{
					this.CurrentIturCode = itur.IturCode;
					//this.RaisePropertyChanged(() => this.CurrentIturCode);
				}
			}
		}

		public bool CheckSelect
		{
			get { return _checkSelect; }
			set
			{
				_checkSelect = value;
				RaisePropertyChanged(() => CheckSelect);
				foreach (InventProductItemViewModel item in this._inventProductItems)
				{
					item.CheckSelect = this._checkSelect;
				}
				this._deleteAllCheckSelectInventProductCommand.RaiseCanExecuteChanged();
			}
		}

		private void DeleteAllCheckSelectInventProductCommandExecuted()
		{
			string message = Localization.Resources.ViewModel_msgDeleteAllCheckSelect;

				MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

				if (messageBoxResult == MessageBoxResult.Yes)
				{
					//List<string> branchaesCodeList = this._inventProductRepository.GetBranchCodeListByCustomerCode(currentCustomerCode);
					using (new CursorWait())
					{
						foreach (InventProductItemViewModel item in this._inventProductItems)
						{
							if (item.CheckSelect == true)
							{
								this._inventProductRepository.Delete(item.InventProduct, base.GetDbPath);
							}
						}
					}
				}
				InventProductsBuild();
		}

		public bool IsDataGridProperyVisibility
		{
			get
			{
				return this._isDataGridProperyVisibility;
			}
			set
			{
				this._isDataGridProperyVisibility = value;
				RaisePropertyChanged(() => this.IsDataGridProperyVisibility);
			}
		}

		//public bool IsPhotoProperyVisibility
		//{
		//	get
		//	{
		//		return this._isPhotoProperyVisibility;
		//	}
		//	set
		//	{
		//		this._isPhotoProperyVisibility = value;
		//		RaisePropertyChanged(() => this.IsPhotoProperyVisibility);
		//	}
		//}
		

		public string RemarkColor
		{
			get
			{
				var color = this._userSettingsManager.InventProductMarkColorGet();
				return "200,200,200";
			}
		}

        public DataGrid InventProductDataGrid { get; set; }

        public ObservableCollection<DocumentHeaderItemViewModel> DocumentHeaderItems
        {
			get { return this._documentHeaderItems; }
        }

        public ObservableCollection<InventProductItemViewModel> InventProductItems
        {
			get { return this._inventProductItems; }
        }

        public string SearchItem
        {
            get { return this._searchItem; }
            set
            {
                this._searchItem = value;

                this.RaisePropertyChanged(() => this.SearchItem);

                this._pageCurrent = 1;
				this.RaisePropertyChanged(() => this.PageCurrent);
                this.InventProductsBuild();
            }
        }

        public string SearchExpression
        {
            get { return this._searchExpression; }
            set
            {
                this._searchExpression = value;

                this.RaisePropertyChanged(() => this.SearchExpression);

                this._pageCurrent = 1;
				this.RaisePropertyChanged(() => this.PageCurrent);

                this.InventProductsBuild();
            }
        }

		public string IturFastPrintReportCode
		{
			get {
				this._iturFastPrintReportCode = this.GetReportFastPrintCode();
				string tooltip = this._commandRepository.GetTitle(enUICommand.PrintReport) + UtilsConvert.LTRMark + " " + this._iturFastPrintReportCode + " " +  UtilsConvert.LTRMark;
				return tooltip; 
			}
		}

        public ObservableCollection<string> SearchItems
        {
            get { return this._searchItems; }
        }

        public DocumentHeaderItemViewModel DocumentHeaderSelectedItem
        {
            get { return this._documentHeaderSelectedItem; }
            set
            {
				var tmpPageCurrent = this.PageCurrent;
			//	if (this._documentHeaderSelectedItem != value) { tmpPageCurrent = 1; }
                this._documentHeaderSelectedItem = value;

                this._deleteDocumentHeaderCommand.RaiseCanExecuteChanged();
                this._editDocumentHeaderCommand.RaiseCanExecuteChanged();

                this._addInventProductCommand.RaiseCanExecuteChanged();
                this._editInventProductCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.DocumentHeaderSelectedItem);

                this._reportDocumentHeaderCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.IsAnyDocumentSelected);
                this._resetInventProductCommand.RaiseCanExecuteChanged();

                if (this._isDocumentSelectionGenerateProductsBuild)
                {
                    Task.Factory.StartNew(() =>
                        {
							this.PageCurrent = tmpPageCurrent;//1; //builds list		 ?? надо добавлять в каждом случае отдельно

							if (this._afterBuildAction != null)
                            {
								this._afterBuildAction();
								this._afterBuildAction = null;
                            }
						}).LogTaskFactoryExceptions("DocumentHeaderSelectedItem");
                }
            }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return this._yesNoRequest; }
        }

        public int ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => ItemsTotal);
            }
        }

        public int PageCurrent
        {
            get { return this._pageCurrent; }
            set
            {
                this._pageCurrent = value;
				Utils.RunOnUI(() => this.RaisePropertyChanged(() => this.PageCurrent));
                this.InventProductsBuild();
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

        public DelegateCommand AddInventProductCommand
        {
            get { return this._addInventProductCommand; }
        }

        public DelegateCommand EditInventProductCommand
        {
            get { return this._editInventProductCommand; }
        }

        public DelegateCommand CloneCommand
        {
			get { return this._cloneCommand; }
        }

		public DelegateCommand CopyCommand
		{
			get { return this._copyCommand; }
		}

		public DelegateCommand CloneDocumentHeaderCommand
		{
			get { return this._cloneDocumentHeaderCommand; }
		}

        public DelegateCommand DeleteInventProductCommand
        {
            get { return this._deleteInventProductCommand; }
        }

		public DelegateCommand DeleteAllCheckSelectInventProductCommand
        {
			get { return this._deleteAllCheckSelectInventProductCommand; }
        }

		

        public InventProductItemViewModel InventProductSelectedItem
        {
            get { return this._inventProductSelectedItem; }
            set
            {
				//if (value != null)
				//{
					this._inventProductSelectedItem = value;
					this.RaisePropertyChanged(() => this.InventProductSelectedItem);
				//}
                this._editInventProductCommand.RaiseCanExecuteChanged();
                this._deleteInventProductCommand.RaiseCanExecuteChanged();
				this._deleteAllCheckSelectInventProductCommand.RaiseCanExecuteChanged();
                this._cloneCommand.RaiseCanExecuteChanged();
				this._copyCommand.RaiseCanExecuteChanged();

				this._showImageCommand.RaiseCanExecuteChanged();

                this._resetErrorInventProductSelectedCommand.RaiseCanExecuteChanged();
                this._resetQuantityInventProductCommand.RaiseCanExecuteChanged();
            }
        }

		private void ShowImageCommandExecuted(InventProductItemViewModel itemInventProduct)
		{
		//	_editSelectedCommand.Execute(_current);
		}


		public void SelectedItemSet(InventProductItemViewModel item)
		{
			this._timer.Stop();
			this._timer.Start();
		}
		
		void Timer_Tick(object sender, EventArgs e)
		{
			if (IsDataGridProperyVisibility == true)
			{
				using (new CursorWait())
				{
					this._timer.Stop();
					this.BuildDetailsItems();
				}
			}
			
		}

		public ObservableCollection<InventProductPropertyStrItemViewModel> DetailItems
		{
			get { return this._detailItems; }
		}

		public InventProductPropertyStrItemViewModel DetailSelectedItem
		{
			get { return this._detailSelectedItem; }
			set { this._detailSelectedItem = value; }
		}

		private string GetPropertyStrName(DomainObjectTypeEnum domainPropertyStr, string propertyStr1Code)
		{
			string name = "";
			if (string.IsNullOrWhiteSpace(propertyStr1Code) == true) return name;
			if (string.IsNullOrWhiteSpace(domainPropertyStr.ToString()) == true) return name;
			if (this._dictionaryObjectPropertyStr == null) return name;

			string key = domainPropertyStr.ToString() + "|" + propertyStr1Code;
			if (this._dictionaryObjectPropertyStr.ContainsKey(key) == true)
			{
				if (string.IsNullOrWhiteSpace(this._dictionaryObjectPropertyStr[key].Name) == false)
				{
					name = this._dictionaryObjectPropertyStr[key].Name;
				}
			}
			return name;
		}

		private PropertyStr GetPropertyStrObject(DomainObjectTypeEnum domainPropertyStr, string propertyStrCode)
		{
			PropertyStr retPropertyStr = new PropertyStr();
			retPropertyStr.DomainObject = domainPropertyStr.ToString();
			if (string.IsNullOrWhiteSpace(propertyStrCode) == true) return retPropertyStr;
			retPropertyStr.PropertyStrCode = propertyStrCode;
			if (string.IsNullOrWhiteSpace(domainPropertyStr.ToString()) == true) return retPropertyStr;
			if (this._dictionaryObjectPropertyStr == null) return retPropertyStr;

			string key = domainPropertyStr.ToString() + "|" + propertyStrCode;
			if (this._dictionaryObjectPropertyStr.ContainsKey(key) == true)
			{
				if (this._dictionaryObjectPropertyStr[key] != null)
					retPropertyStr = this._dictionaryObjectPropertyStr[key];
			}
			return retPropertyStr;
		}

		private string GetPropertyStrTitle(DomainObjectTypeEnum domainPropertyStr, Dictionary<string, PropertyLink> detailPropertyLinkDictionary)
		{
			//Dictionary<string, PropertyLink> detailPropertyLinkDictionary = _editorComboViewModel.EditorTemplateCurrent.DetailPropertyLinkDictionary;
			string title = domainPropertyStr.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(title) == true)
			{
				var detailPropertyLink = detailPropertyLinkDictionary[title];
				title = detailPropertyLink.DefaultEditorLabel;
				if (string.IsNullOrWhiteSpace(detailPropertyLink.CodeLocalizationEditorLabel) == false)
				{
					title = UtilsMisc.LocalizationFromLocalizationKey(detailPropertyLink.CodeLocalizationEditorLabel);
				}
			}
			return title;
		}

		private void BuildDetailsItems()
		{
			this._detailItems.Clear();

			if (this._inventProductSelectedItem == null)
				return;

			InventProduct master = _inventProductSelectedItem.InventProduct;
			Dictionary<string, PropertyLink> detailPropertyLinkDictionary = _productEditorComboViewModel.EditorTemplateCurrent.DetailPropertyLinkDictionary;
			string propertyStrCode = "";
			string propertyStrTitle = "";
			string propertyStrName = "";
			PropertyStr propertyStrObject = new PropertyStr();
			InventProductPropertyStrItemViewModel viewModel;
			//================ PropertyStr1 ===========
			string propertyN = DomainObjectTypeEnum.PropertyStr1.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr1;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr1, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr1, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr1, propertyStrCode);
				
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr1.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr2 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr2.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr2;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr2, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr2, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr2, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr2.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr3 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr3.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr3;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr3, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr3, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr3, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr3.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr4 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr4.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr4;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr4, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr4, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr4, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr4.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr5 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr5.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr5;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr5, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr5, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr5, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr5.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr6 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr6.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr6;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr6, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr6, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr6, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr6.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr7 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr7.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr7;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr7, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr7, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr7, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr7.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr8 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr8.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr8;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr8, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr8, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr8, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr8.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr9 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr9.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr9;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr9, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr9, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr9, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr9.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr10 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr10.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr10;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr10, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr10, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr10, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr10.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr11 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr11.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr11;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr11, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr11, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr11, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr11.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr12 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr12.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr12;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr12, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr12, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr12, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr12.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr13 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr13.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr13;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr13, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr13, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr13, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr13.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr14 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr14.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr14;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr14, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr14, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr14, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr14.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr15 ===========

			propertyN = DomainObjectTypeEnum.PropertyStr15.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr15;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr15, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr15, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr15, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr15.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr16 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr16.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr16;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr16, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr16, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr16, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr16.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr17 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr17.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr17;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr17, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr17, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr17, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr17.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr18 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr18.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr18;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr18, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr18, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr18, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr18.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr19 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr19.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr19;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr19, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr19, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr19, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr19.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
			//================ PropertyStr20 ===========
			propertyN = DomainObjectTypeEnum.PropertyStr20.ToString();
			if (detailPropertyLinkDictionary.ContainsKey(propertyN) == true)
			{
				propertyStrCode = master.IPValueStr20;
				propertyStrTitle = this.GetPropertyStrTitle(DomainObjectTypeEnum.PropertyStr20, detailPropertyLinkDictionary);
				propertyStrName = this.GetPropertyStrName(DomainObjectTypeEnum.PropertyStr20, propertyStrCode);
				propertyStrObject = this.GetPropertyStrObject(DomainObjectTypeEnum.PropertyStr20, propertyStrCode);
				viewModel = new InventProductPropertyStrItemViewModel(DomainObjectTypeEnum.PropertyStr20.ToString(),
					propertyStrTitle, propertyStrCode, propertyStrName, propertyStrObject, master);
				this._detailItems.Add(viewModel);
			}
		
		}

        public Itur Itur
        {
            get { return this._itur; }
        }

        public ObservableCollection<InventProductIturInfoItem> IturInfo
        {
            get { return this._iturInfo; }
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

        public DelegateCommand AddDocumentHeaderCommand
        {
            get { return this._addDocumentHeaderCommand; }
        }

        public DelegateCommand DeleteDocumentHeaderCommand
        {
            get { return this._deleteDocumentHeaderCommand; }
        }

        public DelegateCommand EditDocumentHeaderCommand
        {
            get { return this._editDocumentHeaderCommand; }
        }

        public DelegateCommand NextIturCommand
        {
            get { return this._nextIturCommand; }
        }

        public DelegateCommand PrevIturCommand
        {
            get { return this._prevIturCommand; }
        }

		public DelegateCommand NextErpIturCommand
		{
			get { return this._nextErpIturCommand; }
		}

		public DelegateCommand PrevErpIturCommand
		{
			get { return this._prevErpIturCommand; }
		}

        public DelegateCommand ReportDocumentHeaderCommand
        {
            get { return this._reportDocumentHeaderCommand; }
        }

        public DelegateCommand ReportInventProductCommand
        {
            get { return this._reportInventProductCommand; }
        }

        public DelegateCommand IturReportCommand
        {
			get { return this._iturReportCommand; }
        }

        public DelegateCommand IturRefreshCommand
        {
			get { return this._iturRefreshCommand; }
        }

		  public DelegateCommand ChangeIturNameCommand
        {
			get { return this._changeIturNameCommand; }
        }

	      public DelegateCommand RefreshDocumentHeaderCommand
        {
			get { return this._refreshDocumentHeaderCommand; }
        }

        public DelegateCommand RefreshInventProductCommand
        {
			get { return this._refreshInventProductCommand; }
        }

        public DelegateCommand ResetErrorInventProductSelectedCommand
        {
			get { return this._resetErrorInventProductSelectedCommand; }
        }

        public DelegateCommand ResetInventProductCommand
        {
			get { return this._resetInventProductCommand; }
        }

        public DelegateCommand<DocumentHeaderItemViewModel> ResetErrorDocumentHeaderNotSelectedCommand
        {
			get { return this._resetErrorDocumentHeaderNotSelectedCommand; }
        }

		public DelegateCommand ResetErrorDocumentHeaderCommand
        {
			get { return this._resetErrorDocumentHeaderCommand; }
        }

		

        public bool IsOnlyWithError
        {
			get { return this._isOnlyWithError; }
            set
            {
				this._isOnlyWithError = value;
				this.RaisePropertyChanged(() => this.IsOnlyWithError);

				this.InventProductsBuild();
            }
        }

        public string IturStatusColor
        {
			get { return this._iturStatusColor; }
            set
            {
				this._iturStatusColor = value;
				this.RaisePropertyChanged(() => this.IturStatusColor);
            }
        }

        public ReactiveProperty<string> TotalNumberOfItems
        {
			get { return this._totalNumberOfItems; }
            set
            {
				this._totalNumberOfItems = value;
				this.RaisePropertyChanged(() => this.TotalNumberOfItems);
            }
        }

        public ReportButtonViewModel ReportButtonItur
        {
            get { return _reportButtonItur; }
        }

        public ReportButtonViewModel ReportButtonDocument
        {
			get { return this._reportButtonDocument; }
        }

        public ReportButtonViewModel ReportButtonInventProduct
        {
			get { return this._reportButtonInventProduct; }
        }

        public bool IsAnyDocumentSelected
        {
            get { return this._documentHeaderSelectedItem != null; }
        }

        public DelegateCommand<DocumentHeaderItemViewModel> ClearDocumentHeaderNotSelectedCommand
        {
			get { return this._clearDocumentHeaderNotSelectedCommand; }
        }

        public DelegateCommand SearchCommand
        {
			get { return this._searchCommand; }
        }

        public DelegateCommand ReportCommand
        {
			get { return this._reportCommand; }
        }

        public EditorTemplateComboViewModel ProductEditorComboViewModel
        {
			get { return this._productEditorComboViewModel; }
        }

        public DelegateCommand PrintReportCommand
        {
			get { return this._printReportCommand; }
        }

		//public DelegateCommand PrintReportByLocationCodeCommand
		//{
		//	get { return this._printReportByLocationCodeCommand; }
		//}

		

        public bool IsBusy
        {
			get { return this._isBusy; }
            set
            {
				this._isBusy = value;

				RaisePropertyChanged(() => this.IsBusy);

				_eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(this._isBusy);
            }
        }

        public DelegateCommand ResetQuantityInventProductCommand
        {
            get { return _resetQuantityInventProductCommand; }
        }

        public DelegateCommand<InventProductItemViewModel> EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand<InventProductItemViewModel> CancelCommand
        {
            get { return _cancelCommand; }
        }

        public DelegateCommand<InventProductItemViewModel> CommitCommand
        {
            get { return _commitCommand; }
        }


		public DelegateCommand<InventProductItemViewModel> ShowImageCommand
		{
			get { return _showImageCommand; }
		}

		public DelegateCommand<InventProductPropertyStrItemViewModel> EditDetailGridCommand
		{
			get { return _editDetailGridCommand; }
		}

		public DelegateCommand<InventProductPropertyStrItemViewModel> CancelDetailGridCommand
		{
			get { return _cancelDetailGridCommand; }
		}

		public DelegateCommand<InventProductPropertyStrItemViewModel> CommitDetailGridCommand
		{
			get { return _commitDetailGridCommand; }
		}

        public DelegateCommand IturDisableCommand
        {
            get { return _iturDisableCommand; }
            set { _iturDisableCommand = value; }
        }

        public string IturEnableDisableStatus
        {
            get { return _iturEnableDisableStatus; }
            set
            {
				this._iturEnableDisableStatus = value;
				RaisePropertyChanged(() => this.IturEnableDisableStatus);
            }
        }

        public bool IturEnabled
        {
			get { return this._iturEnabled; }
            set
            {
				this._iturEnabled = value;
				RaisePropertyChanged(() => this.IturEnabled);
            }
        }

        public string CurrentIturCode
        {
			get { return this._currentIturCode; }
            set
            {
				if (this._isGeneratedByCode) return;

				this._currentIturCode = value;
				this.RaisePropertyChanged(() => this.CurrentIturCode);

                bool found = false;
				if (this._iturCodesNotFull)
                {
					found = this.GetIturCodesAllCache().Contains(this._currentIturCode);

                    if (found)
                    {
						if (this._iturCodes.Contains(this._currentIturCode) == false)
                        {
							this._iturCodes.Add(this._currentIturCode);
							this._nextIturCommand.RaiseCanExecuteChanged();
							this._prevIturCommand.RaiseCanExecuteChanged();

							this._nextErpIturCommand.RaiseCanExecuteChanged();
							this._prevErpIturCommand.RaiseCanExecuteChanged();
                        }
                    }
                }
                else
                {
					found = this._iturCodes.Contains(this._currentIturCode);
                }

                if (found)
                {
                    using (new CursorWait())
                    {
						this.BuildPageInfo(this._currentIturCode);
						DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault();
                    }
                }
            }
        }

        #endregion

        #region methods

        ///////////////////////////////////////////////////////////////////////////////////////////        

        private void DocumentHeadersBuild()
        {
            Utils.RunOnUI(() =>
                {
                    this.ClearDocumentHeaders();
                });

			try
			{
				DocumentHeaders documentHeaders = this._documentHeaderRepository.GetDocumentHeadersByItur(this._itur, base.GetDbPath);

				List<DocumentHeaderItemViewModel> uiItems = new List<DocumentHeaderItemViewModel>();
				lock (documentHeaderItemsAddLock)
				{
					foreach (DocumentHeader document in documentHeaders.OrderByDescending(r => r.CreateDate))
					{
						DocumentHeaderItemViewModel viewModel = new DocumentHeaderItemViewModel(document, this._itur);
						viewModel.PropertyChanged += this.DocumentHeaderViewModel_PropertyChanged;
						this.SetDocumentTotalItem(document, viewModel);
						uiItems.Add(viewModel);
					}
					Utils.RunOnUI(() => uiItems.ForEach(r => this._documentHeaderItems.Add(r)));
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("DocumentHeadersBuild", exc);
				throw;
			}
        }

        private void ClearDocumentHeaders()
        {
			lock (documentHeaderItemsClearLock)
			{
				foreach (var dh in _documentHeaderItems)
				{
					dh.PropertyChanged -= this.DocumentHeaderViewModel_PropertyChanged;
				}
				
				this._documentHeaderSelectedItem = null;
				this._documentHeaderItems.Clear();
			}
        }

        private void SetDocumentTotalItem(DocumentHeader documentHeader, DocumentHeaderItemViewModel viewModel)
        {
            //viewModel.TotalItems = String.Format("{0:0.##}", this._inventProductRepository.GetSumQuantityEditByDocumentCode(documentHeader.DocumentCode, base.GetDbPath));

            //var ret = ResulteValueTokenReactiveProperty.SumQuantityEditByDocumentCode;
            //viewModel.TotalItems = ret.Value;
			if (documentHeader == null) return;
			if (viewModel == null) return;


			//ReactiveProperty<string> sumQuantityEdit = this._inventProductRepository.ReturnSumQuantityEditByDocumentCode(documentHeader.DocumentCode, base.GetDbPath);
			ReactiveProperty<string> sumQuantityEdit = this._inventProductRepository.ReturnStringSumQuantityEditByDocumentCode(documentHeader.DocumentCode, base.GetDbPath);
			
			//Utils.RunOnUIAsync(() => viewModel.TotalItems = String.Format("{0:0.##}", sumQuantityEdit.Value));
			viewModel.TotalItems = sumQuantityEdit;
        }

        private void DocumentHeadersRefresh()
		{
			this._isGeneratedByCode = true;
			try
			{
				//lock (documentHeaderItemsRefreshLock)
				lock (this.DocumentHeaderItems)
				{
					if (this.DocumentHeaderItems != null)
					{
						foreach (DocumentHeaderItemViewModel viewModel in this.DocumentHeaderItems)
						{
							if (viewModel == null) continue;
							if (viewModel.DocumentHeader == null) continue;
							DocumentHeader document = this._documentHeaderRepository.GetDocumentHeaderByCode(viewModel.DocumentHeader.DocumentCode, base.GetDbPath);
							if (document == null) continue;
							viewModel.DocumentHeaderSet(document, this._itur);
							this.SetDocumentTotalItem(document, viewModel);
						}
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("DocumentHeadersRefresh", exc);
			}
			this._isGeneratedByCode = false;
		}

        void DocumentHeaderViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsApprove" && this._isGeneratedByCode == false)
            {
                DocumentHeaderItemViewModel viewModel = sender as DocumentHeaderItemViewModel;
                if (viewModel == null) return;
                Utils.RunOnUI(() => Mouse.OverrideCursor = Cursors.Wait);
                Task.Factory.StartNew(() =>
                    {
					
                        this._documentHeaderRepository.Update(viewModel.DocumentHeader, base.GetDbPath);
						try
						{
							this._iturRepository.RefillApproveStatusBitByIturCode(viewModel.DocumentHeader.IturCode, base.GetDbPath);
						}
						catch (Exception exp)
						{
							_logger.ErrorException("DocumentHeaderViewModel_PropertyChanged: RefillApproveStatusBit :  ", exp);
						}

                        this._itur = this._iturRepository.GetIturByCode(this._itur.IturCode, GetDbPath);
                        this.DocumentHeadersRefresh();
                        Utils.RunOnUI(() =>
                            {
                                this.BuildIturInfo();
                                Mouse.OverrideCursor = null;
                            });
					}).LogTaskFactoryExceptions("DocumentHeaderViewModel_PropertyChanged");
            }
        }

        private bool DeleteDocumentHeaderCommandCanExecute()
        {
            return this._documentHeaderSelectedItem != null;
        }

        private void DeleteDocumentHeaderCommandExecuted()
        {
            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Title = String.Empty;
            notification.Settings = this._userSettingsManager;
            notification.Content = String.Format(Localization.Resources.Msg_Delete_Document_Header, this._documentHeaderSelectedItem.DocumentHeader.DocumentCode);
            this._yesNoRequest.Raise(notification, r =>
            {
                if (r.IsYes == true)
                {
                    using (new CursorWait())
                    {
                        string iturCode = this._documentHeaderSelectedItem.DocumentHeader.IturCode;

						if (this._adapterCode == ImportAdapterName.ImportPdaMerkavaDB3Adapter
							|| this._adapterCode == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
							|| this._adapterCode == ImportAdapterName.ImportPdaClalitSqliteAdapter
							|| this._adapterCode == ImportAdapterName.ImportPdaNativSqliteAdapter
							|| this._adapterCode == ImportAdapterName.ImportPdaYesXlsxAdapter
							|| 	this._adapterCode == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
							|| this._adapterCode == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
						{
							InventProducts inventProducts = this._inventProductRepository.GetInventProductsByDocumentCode(
								this._documentHeaderSelectedItem.DocumentHeader.DocumentCode, base.GetDbPath);
                            //Записать в темпоральную таблицу
                            TemporaryInventorys temporaryInventorys = new TemporaryInventorys();
                            foreach (var inventProduct in inventProducts)
							{
								TemporaryInventory temporaryInventory = new TemporaryInventory();
								temporaryInventory.OldUid = inventProduct.Barcode.CutLength(249); 
								string dateModified = (DateTime.Now).ConvertDateTimeToAndroid();
								temporaryInventory.DateModified = dateModified;
								temporaryInventory.OldSerialNumber = inventProduct.SerialNumber.CutLength(249);
								temporaryInventory.OldItemCode = inventProduct.Makat.CutLength(249);
								temporaryInventory.OldKey = this._itur.IturCode;
								temporaryInventory.OldLocationCode = this._itur.ERPIturCode.CutLength(249); 
								temporaryInventory.Description = "Delete InventProduct with DocHeader in Count4U :" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
								temporaryInventory.Operation = "DELETE";
								temporaryInventory.Domain = "InventProduct";
                                temporaryInventorys.Add(temporaryInventory);
                              }
                            this._temporaryInventoryRepository.Insert(temporaryInventorys, base.GetDbPath);
                        }

                        this._documentHeaderRepository.Delete(this._documentHeaderSelectedItem.DocumentHeader, base.GetDbPath);
				
						DocumentHeaders documentHeadersAfterdelete = this._documentHeaderRepository.GetDocumentHeadersByItur(this._itur, base.GetDbPath);
						if (documentHeadersAfterdelete.Count == 1)
						{
							DocumentHeader doc1 = documentHeadersAfterdelete[0];
							if (doc1 != null)
							{
								doc1.Approve = null;
								this._documentHeaderRepository.Update(doc1, base.GetDbPath);
							}
						}

						try
						{
							this._iturRepository.RefillApproveStatusBitByIturCode(iturCode, base.GetDbPath);
						}
						catch (Exception exp)
						{
							_logger.ErrorException("DeleteDocumentHeaderCommandExecuted : RefillApproveStatusBitByIturCode :  ", exp);
						}
                        this._itur = this._iturRepository.GetIturByCode(this._itur.IturCode, GetDbPath);
                        this.BuildIturInfo();
                        this.DocumentHeadersBuild();
                    }
                }
            });
        }

        private void AddDocumentHeaderCommandExecuted()
        {
            this._eventAggregator.GetEvent<DocumentHeaderAddEditEvent>().Publish(
                new DocumentHeaderAddEditEventPayload()
                {
                    DocumentHeader = null,
                    IturCode = this._itur.IturCode,
                    Context = CBIContext.History,
                    DbContext = Common.NavigationSettings.CBIDbContextInventor,
                });
        }

	

        private bool EditDocumentHeaderCommandCanExecute()
        {
            return this._documentHeaderSelectedItem != null;
        }

		private bool CloneDocumentHeaderCommandCanExecute()
		{
			return this._documentHeaderSelectedItem != null;
		}

        private void EditDocumentHeaderCommandExecuted()
        {
            this._eventAggregator.GetEvent<DocumentHeaderAddEditEvent>().Publish(
                new DocumentHeaderAddEditEventPayload()
                    {
                        DocumentHeader = this._documentHeaderSelectedItem.DocumentHeader,
                        IturCode = this._itur.IturCode,
                        Context = CBIContext.History,
                        DbContext = Common.NavigationSettings.CBIDbContextInventor,
                    });
        }

		private void ChangeIturNameCommandExecuted()
		{
			this._eventAggregator.GetEvent<IturNameChangeEvent>().Publish(
			   new IturNameChangeEventPayload()
			   {
				   Iturs = new Iturs() { this._itur },
				   Context = CBIContext.History,
				   DbContext = Common.NavigationSettings.CBIDbContextInventor,
			   });
			//Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

			//IturNameChangeEventPayload payload = new IturNameChangeEventPayload();
			//payload.Iturs = iturs;
			//payload.Context = base.Context;
			//payload.DbContext = base.CBIDbContext;

			//this._eventAggregator.GetEvent<IturNameChangeEvent>().Publish(payload);
		}


      

         private void ItursNameChanged(IturNameChangedEventPayload payload)
		{
			//this._isGeneratedByCode = true;
			Iturs iturs = new Iturs();

			if (payload.Iturs.Count > 0)
			{
				Itur itur = payload.Iturs[0];
				itur.Name = payload.Name;
				itur.ERPIturCode = payload.ERPCode;
				iturs.Add(itur);
			}
			//foreach (Itur itur in payload.Iturs)
			//{
			//	itur.Name = payload.Name;
			//	iturs.Add(itur);
			//}
			//this._itur.Name = payload.Name;
			this._iturRepository.Update(iturs, base.GetDbPath);
			//this.DocumentHeadersBuild();

			//this._isGeneratedByCode = false;
			

			using (new CursorWait())
			{
				var selectedDh = _documentHeaderSelectedItem;
				var selectedIp = _inventProductSelectedItem;

				if (selectedDh == null)
				{
					this.DocumentHeadersBuild();
				}
				else
				{
					this._isDocumentSelectionGenerateProductsBuild = false;
					this.DocumentHeadersBuild();
					this._isDocumentSelectionGenerateProductsBuild = true;
					this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault(r => r.DocumentHeader.ID == selectedDh.DocumentHeader.ID);
				}
				if (selectedIp != null)
					this.InventProductSelectedItem = this._inventProductItems.FirstOrDefault(r => r.InventProduct.ID == selectedIp.InventProduct.ID);
			}
		}

        private void DocumentHeaderAddedEdited(DocumentHeaderAddedEditedEventPayload payload)
        {
            if (payload.IsNew == true)
            {
                this.RefreshStatus();

                this._isDocumentSelectionGenerateProductsBuild = false;
				this.PageCurrent = 1;
                this.DocumentHeaderSelectedItem = _documentHeaderItems.FirstOrDefault(r => r.DocumentHeader.Code == payload.DocumentHeader.Code);
                this._isDocumentSelectionGenerateProductsBuild = true;
            }
            else
            {
                DocumentHeaderItemViewModel viewModel = this.DocumentHeaderItems.FirstOrDefault(r => r.DocumentHeader.Code == payload.DocumentHeader.Code);
                if (viewModel != null)
                {
					this.RefreshStatus();
                }
            }
        }

	

        private void ResetErrorDocumentHeaderNotSelectedExecuted(DocumentHeaderItemViewModel viewModel)
        {
            var dh = viewModel.DocumentHeader;
            if (dh.StatusDocHeaderBit != 0)
            {
                //dh.StatusApproveBit = 0;
				dh.StatusDocHeaderBit = 0;
                this._documentHeaderRepository.Update(dh, GetDbPath);
                viewModel.DocumentHeaderSet(dh, this._itur);
				this.SetDocumentTotalItem(dh, viewModel);

				this.RefreshStatus();
            }
        }

		private void ResetErrorDocumentHeaderExecuted()
		{
			if (this._documentHeaderSelectedItem == null) return;
			var dh = this._documentHeaderSelectedItem.DocumentHeader;
			if (dh == null) return;
			if (dh.StatusDocHeaderBit != 0)
			{
				//dh.StatusApproveBit = 0;
				dh.StatusDocHeaderBit = 0;
				this._documentHeaderRepository.Update(dh, GetDbPath);
				this._documentHeaderSelectedItem.DocumentHeaderSet(dh, this._itur);
				this.SetDocumentTotalItem(dh, this._documentHeaderSelectedItem);

				this.RefreshStatus();
			}
		}

		private void CloneDocumentHeaderCommandExecuted()
		{
			if (this._documentHeaderSelectedItem == null) return;
			var dh = this._documentHeaderSelectedItem.DocumentHeader;
			if (dh == null) return;

			Dictionary<string, string> query = new Dictionary<string, string>();

			//InventProduct ip = this._inventProductSelectedItem.InventProduct;
			//if (ip == null) return;
			string adapterCode = "";
			if(base.CurrentInventor != null) adapterCode = base.CurrentInventor.ImportPDAProviderCode;

			query.Add(Common.NavigationSettings.DocumentCode, dh.DocumentCode.ToString());
			query.Add(Common.NavigationSettings.AdapterName, adapterCode);
			Utils.AddContextToDictionary(query, base.Context);
			Utils.AddDbContextToDictionary(query, base.CBIDbContext);

			object result = _modalWindowLauncher.StartModalWindow(
				Common.ViewNames.DocumentHeaderCloneView,
				WindowTitles.DocumentHeaderClone,
				620, 280,
				ResizeMode.NoResize, query);

            this._itur = this._iturRepository.GetIturByCode(this._itur.IturCode, GetDbPath);
            this.BuildIturInfo();
            this.DocumentHeadersBuild();

			IIturAnalyzesRepository _iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			Dictionary<string, DocumentHeader> documentHeaderDictionary = _iturAnalyzesRepository.GetDocumentHeaderDictionary(GetDbPath, true);

			//--------
			//this._eventAggregator.GetEvent<DocumentHeaderAddEditEvent>().Publish(
			// new DocumentHeaderAddEditEventPayload()
			// {
			//	 DocumentHeader = this._documentHeaderSelectedItem.DocumentHeader,
			//	 IturCode = this._itur.IturCode,
			//	 Context = CBIContext.History,
			//	 DbContext = Common.NavigationSettings.CBIDbContextInventor,
			// });
			//-------

			//if (result != null)
			//{
			//	this.InventProductsBuild();
			//}
		}


     

        private void ClearDocumentHeaderNotSelectedExecuted(DocumentHeaderItemViewModel viewModel)
        {
            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Title = String.Empty;
            notification.Settings = this._userSettingsManager;
            notification.Content = Localization.Resources.Msg_ClearDocument;
            this._yesNoRequest.Raise(notification, r =>
            {
                if (r.IsYes == true)
                {
                    using (new CursorWait())
                    {
                        using (new CursorWait())
                        {
                            DocumentHeader dh = viewModel.DocumentHeader;
                            dh.Approve = false;
							this._documentHeaderRepository.Update(dh, base.GetDbPath);

                            this._inventProductRepository.DeleteAllByDocumentHeaderCode(dh.DocumentCode, this.GetDbPath);
                            this.RefreshStatus();
                            //this.BuildIturInfo();
                            this.InventProductsBuild();
                        }
                    }
                }
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////        

        private void InventProductsBuild()
        {
			var selectedIp = _inventProductSelectedItem;
			var pageCurrent = this._pageCurrent;
			bool mark = this._userSettingsManager.ShowMarkGet();
			string markPropertyName = this._userSettingsManager.InventProductPropertyMarkSelectedItemGet();
			string photoPropertyName = this._userSettingsManager.InventProductPropertyPhotoSelectedItemGet();
			bool propertyIsEmpty = this._userSettingsManager.PropertyIsEmptyGet();
			var markColor = this._userSettingsManager.InventProductMarkColorGet();
			var markColorString = ColorParser.ColorToString(markColor);

			//string currentCustomerCode = "000";
			//if (base.State.CurrentCustomer != null) currentCustomerCode = base.State.CurrentCustomer.Code;

			string currentInventorCode = "000";
			if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;


			//string importFolderPath = _dbSettings.ImportFolderPath();
			//string path = Path.Combine(importFolderPath, "Customer", currentCustomerCode);

			string importPDAFolder = this._dbSettings.ImportFolderPath();//inData в папке каждого инвентора
			//string targetPhotoFolder = importPDAFolder.Trim('\\') + @"\Inventor\" + currentInventorCode + @"\inData\photo";
			string path = Path.Combine(importPDAFolder, "Inventor", currentInventorCode, "inData", "photo");

			string sourseFolder = Path.GetFullPath(path);
				 
			if (_productEditorComboViewModel.EditorTemplateCurrent.DetailPart == 1)
			{
				IsDataGridProperyVisibility = true;
			}
			else
			{
				IsDataGridProperyVisibility = false;
			}
			RaisePropertyChanged(() => this.IsDataGridProperyVisibility);


			//if (_productEditorComboViewModel.EditorTemplateCurrent.Code == "Nativ")
			//{
			//	IsPhotoProperyVisibility = true;
			//}
			//else
			//{
			//	IsPhotoProperyVisibility = false;
			//}

			//RaisePropertyChanged(() => this.IsPhotoProperyVisibility);
					

            if (this._documentHeaderSelectedItem == null)
            {
                Utils.RunOnUI(ClearInventProducts);
                ItemsTotal = 0;
            }
            else
            {
                SelectParams sp = null;
                try
                {
                    sp = SelectParamsInventProductBuild();

                    InventProducts inventProducts = this._inventProductRepository.GetInventProducts(sp, base.GetDbPath);
					this._stopwatch = Stopwatch.StartNew();

                    Utils.RunOnUI(() =>
                        {
                            ClearInventProducts();

							lock (inventProductItemsAddLock)
							{
								foreach (InventProduct inventProduct in inventProducts)
								{
									inventProduct.ERPIturCode = _itur.ERPIturCode;
									InventProductItemViewModel viewModel = new InventProductItemViewModel(inventProduct, _productEditorComboViewModel,
										photoPropertyName, sourseFolder, mark, markPropertyName, propertyIsEmpty, markColorString);
									this._inventProductItems.Add(viewModel);
								}

								this._productEditorComboViewModel.DynamicRepository.FillObjectListWithDynamicProperties(this._inventProductItems, o => ((InventProductItemViewModel)o).InventProduct);
							}

                            this.ItemsTotal = (int)inventProducts.TotalCount;

							this._stopwatch.Stop();
							string timeSpent = String.Format("InventProductsBuild time: {0}", this._stopwatch.Elapsed.TotalSeconds.ToString());
							System.Diagnostics.Debug.Print(timeSpent);
							_logger.Info(timeSpent);

							

                            if ((inventProducts.TotalCount > 0) && (inventProducts.Count == 0)) //do not show empty space - move on previous page           
                            {
                                this.PageCurrent = this._pageCurrent - 1;
                            }
                        });


					if (selectedIp != null)
					{
						this.InventProductSelectedItem = this._inventProductItems.FirstOrDefault(r => r.InventProduct.ID == selectedIp.InventProduct.ID);
					}
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("InventProductsBuild", exc);
                    _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                    if (sp != null)
                        _logger.Error("SelectParams: {0}", sp.ToString());
                    throw;
                }
            }
        }

        private void ClearInventProducts()
        {
			lock (inventProductItemsClearLock)
			{
				this._inventProductItems.Clear();
			}
        }

        private SelectParams SelectParamsInventProductBuild()
        {
            SelectParams result = new SelectParams()
            {
                IsEnablePaging = true,
                CountOfRecordsOnPage = this._pageSize,
                CurrentPage = this._pageCurrent,
            };

			if (_documentHeaderSelectedItem != null)
			{
				result.FilterParams.Add("DocumentCode",
					new FilterParam() { Operator = FilterOperator.Equal, Value = this._documentHeaderSelectedItem.DocumentHeader.DocumentCode });
			}

            if (!String.IsNullOrEmpty(this._searchExpression))
            {
                if (this._searchItem == ComboValues.FindInventProduct.FilterProduct)
                {
                    result.FilterParams.Add("ProductName",
                                            new FilterParam() { Operator = FilterOperator.Contains, Value = this._searchExpression });
                }
                if (this._searchItem == ComboValues.FindInventProduct.FilterBarcode)
                {
                    result.FilterParams.Add("Barcode",
                                            new FilterParam() { Operator = FilterOperator.Contains, Value = this._searchExpression });
                }
                if (this._searchItem == ComboValues.FindInventProduct.FilterMakat)
                {
                    result.FilterParams.Add("Makat",
                                            new FilterParam() { Operator = FilterOperator.Contains, Value = this._searchExpression });
                }
            }

            if (_isOnlyWithError)
            {
                result.FilterParams.Add("StatusInventProductBit",
                new FilterParam() { Operator = FilterOperator.Greater, Value = 0 });
            }

            return result;
        }

        public void DeleteInventProductCommandExecute()
        {
            DeleteInventProduct(this._inventProductSelectedItem.InventProduct);
        }

        private bool DeleteInventProductCommandCanExecute()
        {
            return this._inventProductSelectedItem != null;
        }

		private bool DeleteAllCheckSelectInventProductCommandCanExecuted()
		{
			bool ret = false;
			foreach (InventProductItemViewModel item in this._inventProductItems)
			{
				if (item.CheckSelect == true)
				{
					ret = true;
					break;
				}
			}
			return ret;
		}

        private bool CloneCommandCanExecute()
        {
			return this._inventProductSelectedItem != null;
        }

		 private bool CopyCommandCanExecute()
        {
			return this._inventProductSelectedItem != null;
        }

	    private bool EditInventProductCommandCanExecute()
        {
            return this._inventProductSelectedItem != null;
        }

        private void EditInventProductCommandExecuted()
        {
			this.EditInventProduct(this._inventProductSelectedItem.InventProduct);
        }

        private void AddInventProductCommandExecuted()
        {
            this._eventAggregator.GetEvent<InventProductAddEvent>().Publish(
                new InventProductAddEventPayload()
                    {
                        DocumentCode = this.DocumentHeaderSelectedItem.DocumentHeader.DocumentCode,
                        Context = base.Context,
                        DbContext = base.CBIDbContext,
                    });
        }

        private bool ResetErrorInventProductSelectedCanExecute()
        {
            return this._inventProductSelectedItem != null;
        }

        private void ResetErrorInventProductSelectedExecuted()
        {
            if (this._inventProductSelectedItem == null) return;

            InventProduct iv = this._inventProductSelectedItem.InventProduct;
            iv.StatusInventProductBit = 0;
            this._inventProductRepository.Update(iv, GetDbPath);
            this._inventProductSelectedItem.InventProductSet(iv);

            this.RefreshStatus();
        }

        private void EditInventProduct(InventProduct inventProduct)
        {
            this._eventAggregator.GetEvent<InventProductEditEvent>().Publish(
               new InventProductEditEventPayLoad()
               {
                   InventProduct = inventProduct,
                   Context = base.Context,
                   DbContext = base.CBIDbContext,
               });
        }

        private void DeleteInventProduct(InventProduct inventProduct)
        {
            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Title = String.Empty;
            notification.Settings = this._userSettingsManager;
            notification.Content = String.Format(Localization.Resources.Msg_Delete_Invent_Product, inventProduct.ProductName, Environment.NewLine, inventProduct.Makat);
            this._yesNoRequest.Raise(notification, r =>
            {
                if (r.IsYes)
                {
					if (this._adapterCode == ImportAdapterName.ImportPdaMerkavaDB3Adapter 
						|| this._adapterCode == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
						||  this._adapterCode == ImportAdapterName.ImportPdaClalitSqliteAdapter 
						||  this._adapterCode == ImportAdapterName.ImportPdaNativSqliteAdapter
						|| this._adapterCode == ImportAdapterName.ImportPdaYesXlsxAdapter
						|| this._adapterCode == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
						|| this._adapterCode == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter
						)
						{
							//Записать в темпоральную таблицу
							TemporaryInventory temporaryInventory = new TemporaryInventory();
							temporaryInventory.OldUid = inventProduct.Barcode.CutLength(249); 
							string dateModified = (DateTime.Now).ConvertDateTimeToAndroid();
							temporaryInventory.DateModified = dateModified;
							temporaryInventory.OldSerialNumber = inventProduct.SerialNumber.CutLength(249); ;
							temporaryInventory.OldItemCode = inventProduct.Makat.CutLength(249);
							temporaryInventory.OldKey = this._itur.IturCode;
							temporaryInventory.OldLocationCode = this._itur.ERPIturCode.CutLength(249);
							temporaryInventory.Operation = "Delete InventProduct in Count4U :" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
							temporaryInventory.Operation = "DELETE";
							temporaryInventory.Domain = "InventProduct";
							this._temporaryInventoryRepository.Insert(temporaryInventory, base.GetDbPath);
						}

						this._inventProductRepository.Delete(inventProduct, base.GetDbPath);

                    this.RefreshStatus();
                }
            }
                );
        }

        private bool AddInventProductCommandCanExecute()
        {
            return this._documentHeaderSelectedItem != null;
        }

        private void InventProductAdded(InventProduct inventProduct)
        {
            this.RefreshStatus();
            this.InventProductSelectedItem = this._inventProductItems.FirstOrDefault(r => r.InventProduct.ID == inventProduct.ID);
        }

        private void InventoProductEdited(InventProduct inventProduct)
        {
            int page = _pageCurrent;
            this._inventProductSelectedItem = null;
            this.RefreshStatus();
            this._afterBuildAction = (() =>
                {
                    if (_pageCurrent != page)
                    {
                        PageCurrent = page;
                    }
                    this.InventProductSelectedItem = this._inventProductItems.FirstOrDefault(r => r.InventProduct.ID == inventProduct.ID);
                });
        }

        private bool ResetProductCommandCanExecute()
        {
            return this._documentHeaderSelectedItem != null;
        }

		private bool ResetErrorDocumentHeaderCanExecuted()
        {
            return this._documentHeaderSelectedItem != null;
        }
		

        private void ResetInventProductCommandExecuted()
        {
            using (new CursorWait())
            {
                this._inventProductRepository.ClearStatusBit(this._documentHeaderSelectedItem.DocumentHeader.DocumentCode, base.GetDbPath);
                this.RefreshStatus();
            }
        }

        #endregion

        public event EventHandler NeedScrollIntoView = delegate { };
        private void OnNeedScrollIntoView(EventArgs e)
        {
            EventHandler handler = NeedScrollIntoView;
            if (handler != null) handler(this, e);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			//this._dictionaryNamePropertyStr = this._propertyStrRepository.GetNamePropertyStrDictionaryByDomainAndCodeAsKey(base.GetDbPath);
			this._dictionaryObjectPropertyStr = this._propertyStrRepository.GetPropertyStrDictionaryByDomainAndCodeAsKey(base.GetDbPath);
            this._eventAggregator.GetEvent<InventProductAddedEvent>().Subscribe(InventProductAdded);
			this._eventAggregator.GetEvent<IturNameChangedEvent>().Subscribe(this.ItursNameChanged);
            
            this._eventAggregator.GetEvent<InventProductEditedEvent>().Subscribe(InventoProductEdited);
            this._eventAggregator.GetEvent<DocumentHeaderAddedEditedEvent>().Subscribe(DocumentHeaderAddedEdited);
            this._eventAggregator.GetEvent<PrintQueueRunningEvent>().Subscribe(PrintQueueStarted);
			
			this.InitEditorTemplateList();
			
			Task.Factory.StartNew(() =>
			{
				this.InitReportButtons(navigationContext);
			}).LogTaskFactoryExceptions("OnNavigatedTo::InitReportButtons");

            SelectParams iturSelectParams = UtilsConvert.GetObjectFromNavigation(navigationContext, this._navigationRepository, Common.NavigationObjects.IturSelectParams)
                                            as SelectParams;

            InventProductDetailsData navigationData = UtilsConvert.GetObjectFromNavigation(navigationContext, this._navigationRepository, Common.NavigationObjects.InventProductDetails)
                                            as InventProductDetailsData;

            if (navigationData != null)
            {
                iturSelectParams = navigationData.IturSelectParams;
                this._searchItem = navigationData.SearchItem;
                this._searchExpression = navigationData.SearchExpression;
            }

            this._pageSize = this._userSettingsManager.PortionInventProductsGet();

            Task.Factory.StartNew(() =>
                                      {
                                          Utils.RunOnUI(() => Utils.SetCursor());

                                          if (iturSelectParams != null)
                                          {
                                              this._iturCodes = this._iturRepository.GetIturs(iturSelectParams, base.GetDbPath).Select(r => r.IturCode).ToList();
                                              this._iturCodesNotFull = true;
                                          }
                                          else
                                          {
                                              this._iturCodes = this._iturRepository.GetIturCodes(base.GetDbPath).ToList();
                                              this._iturCodesNotFull = false;
                                          }

                                          if (navigationData == null) //opened from inventory form
                                          {
                                              String iturCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.IturCode).Value;
											  this.BuildPageInfo(iturCode);
											  this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault(); //also build list of IP
                                          }
                                          else //open from invent product list form
                                          {
                                              string iturCode = navigationData.IturCode;
                                              this.BuildPageInfo(iturCode);

                                              //do not build invent products list
                                              this._isDocumentSelectionGenerateProductsBuild = false;
											  this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault(r => r.DocumentCode == navigationData.DocumentCode);
                                              this._isDocumentSelectionGenerateProductsBuild = true;

                                              //find required invent product portion
											  SelectParams sp = this.SelectParamsInventProductBuild();
                                              sp.IsEnablePaging = false;

                                              InventProducts inventProducts = this._inventProductRepository.GetInventProducts(sp, base.GetDbPath);

                                              InventProduct inventProduct = inventProducts.FirstOrDefault(r => r.ID == navigationData.InventProductId);
                                              if (inventProduct != null)
                                              {
                                                  int index = inventProducts.IndexOf(inventProduct);
                                                  index++;
                                                  if (index != -1)
                                                  {
                                                      int portion = (int)Math.Ceiling(index / (double)this._pageSize);
                                                      this.PageCurrent = portion; //now build invent product list
                                                      this.InventProductSelectedItem = this._inventProductItems.FirstOrDefault(r => r.InventProduct.ID == navigationData.InventProductId);

                                                      OnNeedScrollIntoView(null);
                                                  }
                                              }
                                          }

                                          Utils.RunOnUI(() => Utils.SetCursor(false));
									  }).LogTaskFactoryExceptions("OnNavigatedTo");

			
			if (base.CurrentInventor != null) this._adapterCode = base.CurrentInventor.ImportPDAProviderCode;

			this._adapterFromPDA = this.GetAdapterNamefromPDA();
			RaisePropertyChanged(() => AdapterFromPDA);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<InventProductAddedEvent>().Unsubscribe(this.InventProductAdded);
            this._eventAggregator.GetEvent<InventProductEditedEvent>().Unsubscribe(this.InventoProductEdited);
			this._eventAggregator.GetEvent<IturNameChangedEvent>().Subscribe(this.ItursNameChanged);
            this._eventAggregator.GetEvent<DocumentHeaderAddedEditedEvent>().Unsubscribe(this.DocumentHeaderAddedEdited);
            this._eventAggregator.GetEvent<PrintQueueRunningEvent>().Unsubscribe(PrintQueueStarted);

            this._reportButtonItur.OnNavigatedFrom(navigationContext);
            this._reportButtonDocument.OnNavigatedFrom(navigationContext);
            this._reportButtonInventProduct.OnNavigatedFrom(navigationContext);

			this._isDocumentSelectionGenerateProductsBuild = false;
            this.InventProductDataGrid = null;
			this._inventProductSelectedItem = null;
			this._documentHeaderSelectedItem = null;
            //this.ClearInventProducts();
			this.ClearDocumentHeaders();
        }

        private void InitEditorTemplateList()
        {
			this._productEditorComboViewModel.DataGrid = this.InventProductDataGrid;
			this._productEditorComboViewModel.State = base.State;
			this._productEditorComboViewModel.ViewName = Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct;
			this._productEditorComboViewModel.RebuildAction = InventProductsBuild;
			this._productEditorComboViewModel.Init();
			//if (_productEditorComboViewModel.EditorTemplateCurrent.DetailPart == 1)
			//{
			//	IsDataGridProperyVisibility = true;
			//}
			//else
			//{
			//	IsDataGridProperyVisibility = false;
			//}
			//RaisePropertyChanged(() => this.IsDataGridProperyVisibility);

			//if (_productEditorComboViewModel.EditorTemplateCurrent.Code == "Nativ")
			//{
			//	IsPhotoProperyVisibility = true;
			//}
			//else
			//{
			//	IsPhotoProperyVisibility = false;
			//}

			//RaisePropertyChanged(() => this.IsPhotoProperyVisibility);
        }

        private void InitReportButtons(NavigationContext navigationContext)
        {
            this._reportButtonItur.OnNavigatedTo(navigationContext);
            this._reportButtonDocument.OnNavigatedTo(navigationContext);
            this._reportButtonInventProduct.OnNavigatedTo(navigationContext);

            Location location = null;

            //itur
            this._reportButtonItur.Initialize(this.ReportIturCommandExecuted, () =>
            {
                if (!String.IsNullOrEmpty(this._itur.LocationCode))
                    location = this._locationRepository.GetLocationByCode(this._itur.LocationCode, GetDbPath);
                SelectParams sp = ReportSelectParamsItur();
                return new Tuple<SelectParams, Itur, Location, DocumentHeader ,Device>(sp, this._itur, location, null, null);
            }, ReportContextItur);
																									
            //doc
            this._reportButtonDocument.Initialize(this.ReportDocumentHeaderCommandExecuted, () =>
            {
                SelectParams sp = ReportSelectParamsDocument();
                if (!String.IsNullOrEmpty(this._itur.LocationCode))
                    location = this._locationRepository.GetLocationByCode(this._itur.LocationCode, GetDbPath);
                return new Tuple<SelectParams, Itur, Location, DocumentHeader ,Device>(sp, this._itur, location, this._documentHeaderSelectedItem.DocumentHeader, null);
            }, ReportContextDocument);
																														  			  
            //invent product
            this._reportButtonInventProduct.Initialize(this.ReportInventProductCommandExecuted, () =>
            {
                SelectParams sp = ReportSelectParamsInventProduct();
                if (!String.IsNullOrEmpty(this._itur.LocationCode))
                    location = this._locationRepository.GetLocationByCode(this._itur.LocationCode, GetDbPath);
                return new Tuple<SelectParams, Itur, Location, DocumentHeader,Device>(sp, this._itur, location, null, null);
            }, ReportContextInventProduct);											  
        }


        void BuildPageInfo(string iturCode)
        {
            this._itur = this._iturRepository.GetIturByCode(iturCode, base.GetDbPath);
			if (this._itur == null) return;

            Utils.RunOnUI(() => Utils.MainWindowTitleSet(Localization.Resources.Domain_Itur + " " + this._itur.Number, this._eventAggregator));

            this.DocumentHeadersBuild();

            Utils.RunOnUI(() =>
                              {
                                  this._pageCurrent = 1;

								  this._currentIturCode = this._itur.IturCode;
								  this.RaisePropertyChanged(() => this.CurrentIturCode);

								  this.RaisePropertyChanged(() => this.Itur);

								  this.BuildIturInfo();
								  this.GoToErpIturCode = this._itur.ERPIturCode;

                                  this._nextIturCommand.RaiseCanExecuteChanged();
                                  this._prevIturCommand.RaiseCanExecuteChanged();

								  this._nextErpIturCommand.RaiseCanExecuteChanged();
								  this._prevErpIturCommand.RaiseCanExecuteChanged();
                              });
        }

        private void BuildIturInfo()
        {
            this._iturInfo.Clear();

            string locationName = String.Empty;
			var location = this._locationRepository.GetLocationByCode(this._itur.LocationCode, GetDbPath);  
            if (location != null)
                locationName = location.Name;

            this._iturInfo.Add(
                new InventProductIturInfoItem()
                    {
                        Value1 = Localization.Resources.ViewModel_InventProductListDetails_Location,
                        Value2 = String.IsNullOrEmpty(locationName) ? this._itur.LocationCode : locationName,
                    });

            this._iturInfo.Add(
                new InventProductIturInfoItem()
                    {
                        Value1 = Localization.Resources.ViewModel_InventProductListDetails_NumberPrefix,
                        Value2 = this._itur.NumberPrefix
                    });
            this._iturInfo.Add(
                new InventProductIturInfoItem()
                    {
                        Value1 = Localization.Resources.ViewModel_InventProductListDetails_NumberSuffix,
                        Value2 = this._itur.NumberSufix
                    });

            string val2 = IturDocValidate.ConvertIturStatusBit2Message(this._itur.StatusIturBit).Trim();
            this._iturInfo.Add(
                new InventProductIturInfoItem()
                    {
                        Value1 = Localization.Resources.ViewModel_InventProductListDetails_StatusBit,
                        Value2 = val2,
                        Tooltip = BitStatus.ToString(this._itur.StatusIturBit) + " : " + val2
                    });

            Dictionary<string, StatusIturGroup> statusIturGroupDictionary = this._statusGroupRepository.CodeStatusIturGroupDictionary;
            Dictionary<int, IturStatusGroupEnum> bitStatusIturGroupEnumDictionary = this._statusGroupRepository.BitStatusIturGroupEnumDictionary;

            string groupName = UtilsMisc.LocalizationFromLocalizationKey(this._statusGroupRepository.FromIturStatusGroupBitToLocalizationCode(this._itur.StatusIturGroupBit));
            val2 = String.Join(Environment.NewLine, groupName).Trim();
            //val2 = String.Join(Environment.NewLine, Bit2List.GetStatusGroupList(this._itur.StatusIturGroupBit)).Trim();


            this._iturInfo.Add(
               new InventProductIturInfoItem()
               {
                   Value1 = Localization.Resources.ViewModel_InventProductListDetails_StatusGroupBit,
                   Value2 = val2,
                   Tooltip = this._itur.StatusIturGroupBit.ToString() + " : " + val2
               });


            //val2 = String.Join(Environment.NewLine, Bit2List.GetStatusList(this._itur.StatusDocHeaderBit, DomainStatusEnum.Doc)).Trim();
            val2 = _itur.ERPIturCode;
            this._iturInfo.Add(
               new InventProductIturInfoItem()
               {
                   Value1 = Localization.Resources.ViewModel_InventProductListDetails_StatusDocHeaderBit,
                   Value2 = val2,
                   //Tooltip = BitStatus.ToString(this._itur.StatusDocHeaderBit) + " : " + val2
                   Tooltip = _itur.ERPIturCode
               });

            //Dictionary<string, StatusIturGroup> allStatusGroups = this._statusGroupRepository.CodeStatusIturGroupDictionary;
            if (statusIturGroupDictionary != null)
            {
                StatusIturGroup statusGroup = statusIturGroupDictionary.Values.FirstOrDefault(r => r.Bit == _itur.StatusIturGroupBit);
                if (statusGroup != null)
                {
                    IturStatusColor = UtilsStatus.FromStatusGroupBitToColor(this._statusGroupRepository.BitStatusIturGroupEnumDictionary,
                                                                  statusGroup.Bit, this._userSettingsManager);
                }
            }
            bool isDisabled = _itur.Disabled.HasValue ? (bool)_itur.Disabled : false;
            this.IturEnableDisableStatus = isDisabled ? Localization.Resources.View_InventProductListDetails_tbEnable : Localization.Resources.View_InventProductListDetails_tbDisable;

            IturEnabled = !isDisabled;

            BuildTotalNumberOfItems();
        }

        private void BuildTotalNumberOfItems()
        {
			//ReactiveProperty<double> sumQuantityEdit = this._inventProductRepository.ReturnSumQuantityEditByDocumentCode(documentHeader.DocumentCode, base.GetDbPath);
			//viewModel.TotalItems = String.Format("{0:0.##}", sumQuantityEdit);

			//ERROR
			//////ReactiveProperty<string> sumQuantityEdit = this._inventProductRepository.ReturnSumQuantityEditByIturCode(this._itur.IturCode, base.GetDbPath);
			//////TotalNumberOfItems = sumQuantityEdit;
			//Utils.RunOnUIAsync(() => TotalNumberOfItems = sumQuantityEdit);

			//var n = this._inventProductRepository.GetSumQuantityEditByIturCode(this._itur.IturCode, base.GetDbPath);
			//Utils.RunOnUIAsync(() => TotalNumberOfItems = string.Format(Localization.Resources.ViewModel_InventProductListDetails_TotalItems + " {0:0.##}", n));
		}

        private bool PrevIturCommandCanExecute()
        {
			if (this._iturCodes == null)
                return false;

            return this._iturCodes.Count > 1;
        }

		private bool PrevErpIturCommandCanExecute()
		{
			if (this._iturCodes == null)
				return false;

			return this._iturCodes.Count > 1;
		}

        private void PrevIturCommandExecuted()
        {
            using (new CursorWait())
            {
                this._isGeneratedByCode = true;
                int index = this._iturCodes.IndexOf(this._itur.IturCode);
                if (index == 0)
                    index = this._iturCodes.Count - 1;
                else
                    index -= 1;
                this.BuildPageInfo(this._iturCodes[index]);
				this.PageCurrent = 1;
                this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault();
                this._isGeneratedByCode = false;
            }
        }

		private void PrevErpIturCommandExecuted()
		{
			if (string.IsNullOrWhiteSpace(this._goToErpIturCode) == true) return;
			Itur itur = this._iturRepository.GetIturByErpIturCode(_goToErpIturCode, base.GetDbPath);
			if (itur == null) return;

			using (new CursorWait())
			{
				this._isGeneratedByCode = true;
				int index = this._iturCodes.IndexOf(this._itur.IturCode);
				if (index == 0)
					index = this._iturCodes.Count - 1;
				else
					index -= 1;
				this.BuildPageInfo(this._iturCodes[index]);
				this.PageCurrent = 1;
				this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault();
				this._isGeneratedByCode = false;
			}
		}

        private bool NextIturCommandCanExecute()
        {
            if (_iturCodes == null)
                return false;

            //return this._iturCodes.IndexOf(this._itur.IturCode) < this._iturCodes.Count - 1;
            return this._iturCodes.Count > 1;
        }

		private bool NextErpIturCommandCanExecute()
		{
			if (_iturCodes == null)
				return false;

			//return this._iturCodes.IndexOf(this._itur.IturCode) < this._iturCodes.Count - 1;
			return this._iturCodes.Count > 1;
		}

        private void NextIturCommandExecuted()
        {
            using (new CursorWait())
            {
                this._isGeneratedByCode = true;
                int index = this._iturCodes.IndexOf(this._itur.IturCode);
                if (index == this._iturCodes.Count - 1)
                    index = 0;
                else
                    index += 1;
                this.BuildPageInfo(this._iturCodes[index]);
				this.PageCurrent = 1;
                this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault();
                this._isGeneratedByCode = false;
            }
        }

		private void NextErpIturCommandExecuted()
		{
			if (string.IsNullOrWhiteSpace(this._goToErpIturCode) == true) return;
			Itur itur = this._iturRepository.GetIturByErpIturCode(_goToErpIturCode, base.GetDbPath);
			if (itur == null) return;

			using (new CursorWait())
			{
				this._isGeneratedByCode = true;
		
				int index = this._iturCodes.IndexOf(this._itur.IturCode);
				if (index == this._iturCodes.Count - 1)
					index = 0;
				else
					index += 1;
				this.BuildPageInfo(this._iturCodes[index]);
				this.PageCurrent = 1;
				this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault();
				this._isGeneratedByCode = false;
			}
		}

        private SelectParams ReportSelectParamsDocument()
        {
            SelectParams sp = new SelectParams();
            sp.IsEnablePaging = false;
            sp.FilterParams.Add("IturCode",
                 new FilterParam() { Operator = FilterOperator.Equal, Value = this._itur.IturCode });

            if (this._documentHeaderSelectedItem != null)
            {
                sp.FilterParams.Add("DocumentCode",
                 new FilterParam() { Operator = FilterOperator.Equal, Value = this._documentHeaderSelectedItem.DocumentHeader.DocumentCode });
            }

            return sp;
        }

        private bool ReportDocumentHeaderCanExecute()
        {
            return this._documentHeaderSelectedItem != null;
        }

        private void ReportDocumentHeaderCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ReportContextDocument);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            SelectParams sp = ReportSelectParamsDocument();

            Utils.AddSelectParamsToQuery(query, sp);

            UtilsConvert.AddObjectToQuery(query, _navigationRepository, this._itur, Common.NavigationObjects.Itur);
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, this._documentHeaderSelectedItem.DocumentHeader, Common.NavigationObjects.DocumentHeader);
            if (!String.IsNullOrEmpty(this._itur.LocationCode))
            {
                Location location = this._locationRepository.GetLocationByCode(this._itur.LocationCode, GetDbPath);
                UtilsConvert.AddObjectToQuery(query, _navigationRepository, location, Common.NavigationObjects.Location);
            }

            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void ReportInventProductCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ReportContextInventProduct);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            SelectParams sp = SelectParamsInventProductBuild();

            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);

            UtilsConvert.AddObjectToQuery(query, _navigationRepository, this._itur, Common.NavigationObjects.Itur);
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, this._documentHeaderSelectedItem.DocumentHeader, Common.NavigationObjects.DocumentHeader);
            if (!String.IsNullOrEmpty(this._itur.LocationCode))
            {
                Location location = this._locationRepository.GetLocationByCode(this._itur.LocationCode, GetDbPath);
                UtilsConvert.AddObjectToQuery(query, _navigationRepository, location, Common.NavigationObjects.Location);
            }

            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private SelectParams ReportSelectParamsItur()
        {
            SelectParams sp = new SelectParams();
            sp.IsEnablePaging = false;
            sp.FilterParams.Add("IturCode",
                 new FilterParam() { Operator = FilterOperator.Equal, Value = _itur.IturCode });

            return sp;
        }

        private void ReportIturCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ReportContextItur);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            SelectParams sp = ReportSelectParamsItur();

            Utils.AddSelectParamsToQuery(query, sp);

            UtilsConvert.AddObjectToQuery(query, _navigationRepository, this._itur, Common.NavigationObjects.Itur);
            if (!String.IsNullOrEmpty(this._itur.LocationCode))
            {
                Location location = this._locationRepository.GetLocationByCode(this._itur.LocationCode, GetDbPath);
                UtilsConvert.AddObjectToQuery(query, _navigationRepository, location, Common.NavigationObjects.Location);
            }

            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private SelectParams ReportSelectParamsInventProduct()
        {
            SelectParams sp = SelectParamsInventProductBuild();
            sp.IsEnablePaging = false;

            return sp;
        }



        private void RefreshStatus()
        {
            using (new CursorWait())
            {
				try
				{
					this._iturRepository.RefillApproveStatusBitByIturCode(this._itur.IturCode, GetDbPath);
				}
				catch (Exception exp)
				{
					_logger.ErrorException("RefreshStatus: RefillApproveStatusBit :  ", exp);
				}
                this._itur = this._iturRepository.GetIturByCode(this._itur.IturCode, GetDbPath);
				this.BuildIturInfo();
                var selectedDh = _documentHeaderSelectedItem;
                var selectedIp = _inventProductSelectedItem;

                if (selectedDh == null)
                {
                    this.DocumentHeadersBuild();
                }
                else
                {
                    this._isDocumentSelectionGenerateProductsBuild = false;
					this.DocumentHeadersBuild();
					this._isDocumentSelectionGenerateProductsBuild = true;
                    this.DocumentHeaderSelectedItem = this._documentHeaderItems.FirstOrDefault(r => r.DocumentHeader.ID == selectedDh.DocumentHeader.ID);
                }
                if (selectedIp != null)
					this.InventProductSelectedItem = this._inventProductItems.FirstOrDefault(r => r.InventProduct.ID == selectedIp.InventProduct.ID);
            }
        }


	


        private void IturRefreshCommandExecuted()
        {
            this.RefreshStatus();
        }

        private void DocumentHeaderRefreshCommandExecuted()
        {
			this.RefreshStatus();
        }

        private void InventProductRefreshCommandExecuted()
        {
			this.RefreshStatus();
        }

		// 	фоновая печать из формы Itur-DocumenHeader 
        private void PrintReportCommandExecuted()
        {
            IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();
            Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrint(ViewDomainContextEnum.ItursItur, 
				base.CurrentCustomer.Code, base.CurrentBranch.Code, base.CurrentInventor.Code);
            if (report != null && this._itur != null)
            {
		        this.RunPrintReportByIturCode(this._itur.IturCode, report);
            }
        }

		private string GetReportFastPrintCode()
		{
			IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();
			Count4U.GenerationReport.Report report = null;
			try
			{
				report = generateReportRepository.GetReportFastPrint(ViewDomainContextEnum.ItursItur,
					 base.CurrentCustomer.Code, base.CurrentBranch.Code, base.CurrentInventor.Code);
			}
			catch { }
			if (report != null && this._itur != null)
			{
				return report.CodeReport;
			}
			else return "";
		}

		//private void PrintReportByLocationCodeCommandExecuted()
		//{
		//	//?? откуда вызывается - ни откуда пока
		//	IReportRepository generateReportRepository = this._serviceLocator.GetInstance<IReportRepository>();
		//	Count4U.GenerationReport.Report report = generateReportRepository.GetReportFastPrint(ViewDomainContextEnum.IturPrintMenuIT2_03);
		//	if (report != null && this._itur != null)
		//	{
		//		this.RunPrintReportByIturCode(this._itur.IturCode, report);
		//	}
		//}

		// Фоновая печать по IturCode ***
        private void RunPrintReportByIturCode(string iturCode, Count4U.GenerationReport.Report report)
        {
            if (string.IsNullOrWhiteSpace(iturCode) == true) return;

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

				SelectParams selectParams = new SelectParams();
				List<string> searchItur = new List<string> { iturCode };
				selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam()
				{
					Values = searchItur
				});
				args.SelectParams = selectParams;

                ImportPdaPrintQueue printQueue = this._serviceLocator.GetInstance<ImportPdaPrintQueue>();
                printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
                //this._generateReportRepository.RunSaveReport(args, @"C:\Temp\testReport\output1.txt", ReportFileFormat.Excel);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("RunPrintReport", exc);
            }
        }




        private void PrintQueueStarted(bool isStarted)
        {
			Utils.RunOnUI(() => this.IsBusy = isStarted);
        }

        private bool ResetQuantityInventProductCommandCanExecute()
        {
			return this._inventProductSelectedItem != null;
        }

        private void ResetQuantityInventProductCommandExecuted()
        {
            using (new CursorWait())
            {
                try
                {
					InventProduct ip = this._inventProductSelectedItem.InventProduct;
                    ip.QuantityEdit = ip.QuantityOriginal;
                    this._inventProductRepository.Update(ip, base.GetDbPath);

                    this.BuildTotalNumberOfItems();

                    string code = this._documentHeaderSelectedItem.DocumentHeader.DocumentCode;
                    DocumentHeader dbObj = this._documentHeaderRepository.GetDocumentHeaderByCode(code, base.GetDbPath);

                    this._isGeneratedByCode = true;
                    this._documentHeaderSelectedItem.DocumentHeaderSet(dbObj, this._itur);
                    this._isGeneratedByCode = false;

                    this._inventProductSelectedItem.InventProductSet(ip);
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("ResetQuantityInventProductCommandExecuted", exc);
                }
            }
        }

        private bool _isEditing;
		private bool _isDetailEditing;

        private void CommitCommandExecuted(InventProductItemViewModel item)
        {
            if (this._isEditing)
            {
                this.SaveInventProductViewModel(item);

                item.UpdateUIAfterDbSave();
				this.RefreshStatus();

                this._isEditing = false;
            }
        }

        private void CancelCommandExecuted(InventProductItemViewModel item)
        {
            this._isEditing = false;
        }

        private void EditCommandExecuted(InventProductItemViewModel item)
        {
            this._isEditing = true;
        }

        private void SaveInventProductViewModel(InventProductItemViewModel viewModel)
        {
            using (new CursorWait())
            {
                this._inventProductRepository.Update(viewModel.InventProduct, base.GetDbPath);

                this.BuildTotalNumberOfItems();

                if (this._documentHeaderSelectedItem != null)
                {
                    string code = this._documentHeaderSelectedItem.DocumentHeader.DocumentCode;
                    DocumentHeader dbObj = this._documentHeaderRepository.GetDocumentHeaderByCode(code, base.GetDbPath);
                    Utils.RunOnUIAsync(() =>
                        {
                            this._isGeneratedByCode = true;
                            this._documentHeaderSelectedItem.DocumentHeaderSet(dbObj, this._itur);
                            this._isGeneratedByCode = false;
                        }
                        );
                }
            }
        }

		private void CommitDetailGridCommandExecuted(InventProductPropertyStrItemViewModel item)
		{
			if (_isDetailEditing)
			{
				item.PropertyStrName = "";
				DomainObjectTypeEnum domain = DomainObjectTypeEnum.Unknown;
				bool ret = Enum.TryParse<DomainObjectTypeEnum>(item.DomainObject, out domain);
				if (ret == true)
				{
					item.PropertyStrName = this.GetPropertyStrName(domain, item.PropertyStrCode);
				}
				SavePropertyStrViewModel(item);

				item.UpdateUIAfterDbSave();
				_isDetailEditing = false;
			}
		}

		private void CancelDetailGridCommandExecuted(InventProductPropertyStrItemViewModel item)
		{
			_isDetailEditing = false;
		}

		private void EditDetailGridCommandExecuted(InventProductPropertyStrItemViewModel item)
		{
			_isDetailEditing = true;
		}


		private void SavePropertyStrViewModel(InventProductPropertyStrItemViewModel viewModel)
		{
			using (new CursorWait())
			{
				viewModel.InventProductUpdate();
				this._inventProductRepository.Update(viewModel.InventProduct, base.GetDbPath);
			}
		}


		private void CopyCommandExecuted()
        {
   			InventProduct ip = this._inventProductSelectedItem.InventProduct;
            if (ip == null) return;
			string copy = "Makat = " + ip.Makat + ", Barcode = " + ip.Barcode + ", SerialNumber = " + ip.SerialNumber + ", IturCode = " + ip.IturCode + ", ERPIturCode = " + ip.ERPIturCode;
			Clipboard.SetText(copy);
         }


        private void CloneCommandExecuted()
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

			InventProduct ip = this._inventProductSelectedItem.InventProduct;
            if (ip == null) return;
            query.Add(Common.NavigationSettings.InventProductId, ip.ID.ToString());
            Utils.AddContextToDictionary(query, base.Context);
            Utils.AddDbContextToDictionary(query, base.CBIDbContext);

            object result = _modalWindowLauncher.StartModalWindow(
                Common.ViewNames.InventProductCloneView,
                WindowTitles.InventProductClone,
                330, 220,
                ResizeMode.NoResize, query);

            if (result != null)
            {
                this.InventProductsBuild();
            }
        }

	
        private void IturDisableCommandExecuted()
        {
            using (new CursorWait())
            {
                bool disabled = !(_itur.Disabled.HasValue ? (bool)_itur.Disabled : false);

                Iturs iturs = new Iturs() { _itur };

                this._iturRepository.SetDisabledStatusBitByIturCode(iturs, disabled, base.GetDbPath);
                this._itur.Disabled = disabled;

                this._itur = this._iturRepository.GetIturByCode(this._itur.IturCode, base.GetDbPath);

				this.RefreshStatus();
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "CurrentIturCode":

                        if (!String.IsNullOrWhiteSpace(_currentIturCode))
                        {
                            bool found = false;

							if (this._iturCodesNotFull)
                            {
								found = this.GetIturCodesAllCache().Contains(_currentIturCode);
                            }
                            else
                            {
								found = this._iturCodes.Contains(_currentIturCode);
                            }
                            if (found == false)
                            {
                                return Localization.Resources.ViewModel_InventProductListDetails_tbNoSuchItur;
                            }
                        }
                        break;
                }

                return String.Empty;
            }
        }

        public string Error { get; private set; }

        private List<string> GetIturCodesAllCache()
        {
			if (this._itursCodesFullCache == null)
            {
				this._itursCodesFullCache = this._iturRepository.GetIturCodes(base.GetDbPath).ToList();
            }

			return this._itursCodesFullCache;
        }

	
    }
}