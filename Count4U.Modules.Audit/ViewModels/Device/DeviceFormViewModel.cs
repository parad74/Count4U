using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Common.ViewModel.SearchFilter;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.SelectionParams;
using NLog;
using Count4U.Common.Extensions;
using Count4U.Modules.Audit.ViewModels.Catalog;
using System.Threading;
using System.Windows.Forms.Integration;
using Microsoft.Reporting.WinForms;
using System.Windows.Input;
using Count4U.Model.Main;
using Count4U.Model.Audit;
using System.IO;

namespace Count4U.Modules.Audit.ViewModels.DevicePDA
{
    public class DeviceFormViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
      //  private readonly IProductRepository _productRepository;
		private readonly IDeviceRepository _deviceRepository;
		private readonly IDocumentHeaderRepository _documentHeaderRepository;
		private readonly IReportRepository _reportRepository;
		private readonly IGenerateReportRepository _generateReportRepository;
		
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IRegionManager _regionManager;
        private readonly UICommandRepository _commandRepository;
        private readonly INavigationRepository _navigationRepository;        

        private readonly DelegateCommand _addCommand;        
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _reportCommand;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;
		public bool _showDetails;
		public bool _showGraph;
		
		


		//private DateTime _startInventorDateTime;
		//private DateTime _endInventorDateTime;

		private List<DeviceItemViewModel> _selectedItems;
		private readonly ObservableCollection<DeviceItemViewModel> _items;

		private DocumentHeaderItemViewModel _detailSelectedItem;
		private readonly ObservableCollection<DocumentHeaderItemViewModel> _detailItems;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

        private readonly ReportButtonViewModel _reportButton;

        private readonly DispatcherTimer _timer;
		private readonly DelegateCommand _updateCommand;

        private SearchFilterViewModel _searchFilterViewModel;
		private DateTime _theLastForAllDevice;

		ReportViewer _reportViewer;
		private WindowsFormsHost _reportHost;
		bool busy;

		public DeviceFormViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            //IProductRepository productRepository,
			IDeviceRepository deviceRepository,
			IReportRepository reportRepository,
			IGenerateReportRepository	 generateReportRepository,
			 IDocumentHeaderRepository documentHeaderRepository,
            IUserSettingsManager userSettingsManager,
            IRegionManager regionManager,
            ReportButtonViewModel reportButton,
            UICommandRepository commandRepository,
            INavigationRepository navigationRepository
            )
            : base(contextCBIRepository)
        {
            _navigationRepository = navigationRepository;
            this._commandRepository = commandRepository;
            this._reportButton = reportButton;
            this._regionManager = regionManager;
            this._userSettingsManager = userSettingsManager;
            //this._productRepository = productRepository;
			this._documentHeaderRepository = documentHeaderRepository;
			this._deviceRepository = deviceRepository;
			this._reportRepository = reportRepository;
			this._generateReportRepository = generateReportRepository;
            this._eventAggregator = eventAggregator;

			this._addCommand = _commandRepository.Build(enUICommand.Add, AddCommandExecuted, DeleteCommandCanExecute);         
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecute);
			this._importCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted, DeleteCommandCanExecute);
			this._reportCommand = _commandRepository.Build(enUICommand.Report, ReportCommandExecuted, DeleteCommandCanExecute);
			this._updateCommand = new DelegateCommand(UpdateCommandExecuted);
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();


			  this._showDetails = false;
			  this._showGraph = true;
			  busy = false;

			this._items = new ObservableCollection<DeviceItemViewModel>();
			this._detailItems = new ObservableCollection<DocumentHeaderItemViewModel>();

			//============== ReportViewer ===================
			WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
			windowsFormsHost.HorizontalAlignment = HorizontalAlignment.Stretch;
			windowsFormsHost.VerticalAlignment = VerticalAlignment.Stretch;
			this._reportViewer = new ReportViewer();
			this._reportViewer.ShowToolBar = true;
			windowsFormsHost.Child = this._reportViewer;
			this.ReportHost = windowsFormsHost;

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(this._userSettingsManager.DelayGet());
            this._timer.Tick += Timer_Tick;
	        }

		private ReportViewer InitReportHost(ReportViewer reportViewer, string reportCode = "[Rep-DV1-103]")
		{
			ReportInfo reportInfo = new ReportInfo(this._reportRepository);
			reportInfo.ReportCode = reportCode;
			GenerateReportArgs args = reportInfo.BuildReportArgs(this.GetDbPath, ViewDomainContextEnum.Device);
			args.RefillReportDS = true;

			//Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
			try
			{
				List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList = this._generateReportRepository.FillReportDSList(args);
				this.ShowReportView(reportViewer, args, reportDSList);
				Mouse.OverrideCursor = null;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("GenerateReport", exc);
				Mouse.OverrideCursor = null;
			}
			return reportViewer;
		}

		private void ShowReportView(ReportViewer reportViewer ,GenerateReportArgs args, List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList)
		{
			if (args == null)
			{
				_logger.Error("Error : ShowReportView - object args is Null");		return;
			}

			if (args.Report == null)
			{
				_logger.Error("Error : ShowReportView - object Report is Null");  		return;
			}

			if (this._reportRepository.GetDomainContextDataSetDictionary().ContainsKey(args.Report.Path) == false)
			{
				_logger.Error("Error : ShowReportView - args.Report.Path not  ContainsKey in GetDomainContextDataSetDictionary()");	return;
			}											  
	
			string path = args.Report.Path;
			string reportFileName = args.Report.FileName;
			//	string codeReport = args.Report.CodeReport;
			//ViewDomainContextEnum viewDomainContextType = args.ViewDomainContextType;
			// ======================= CreateReport ====================
  
			if (reportViewer != null)
			{
				if ((reportDSList != null) && (reportDSList.Count > 0))
				{
					string reportTemplatePath = this._generateReportRepository.BuildReportFullPath(path, reportFileName);
					string reportFileName1 = Path.GetFileNameWithoutExtension(reportTemplatePath);
					reportViewer.LocalReport.ReportPath = reportTemplatePath;
					reportViewer.LocalReport.DataSources.Clear();
					foreach (var reportDS in reportDSList)
					{
						reportViewer.LocalReport.DataSources.Add(reportDS);
					}
					reportViewer.LocalReport.DisplayName = reportFileName1;

					reportViewer.RefreshReport();
				}
			}
		}



	
	
		public DelegateCommand UpdateCommand
		{
			get { return this._updateCommand; }
		}

		private void UpdateCommandExecuted()
		{
			BuildMasterItems();
		}


		public WindowsFormsHost ReportHost
		{
			get { return _reportHost; }
			set
			{
				_reportHost = value;
				RaisePropertyChanged(() => ReportHost);
			}
		}


		public bool ShowDetails
		{
			get { return this._showDetails; }
			set
			{
					this._showDetails = value;
					this._showGraph = !value;
					RaisePropertyChanged(() => ShowGraph);
					RaisePropertyChanged(() => ShowDetails);
					if (busy == false)
					{
						busy = true;
						BuildMasterItems();
						busy = false;
					}
			}
		}

		public bool ShowGraph
		{
			get { return this._showGraph; }
			set
			{
				this._showGraph = value;
				this._showDetails = !value;
				RaisePropertyChanged(() => ShowGraph);
				RaisePropertyChanged(() => ShowDetails);
				//BuildMasterItems();
			}
		}

        public string SearchFilterRegionKey { get; set; }

		public DateTime StartInventorDateTime
		{
			get { 
				//return _startInventorDateTime; 
				return this._userSettingsManager.StartInventorDateTimeGet();
			}
			set
			{
				//_startInventorDateTime = value;
				this._userSettingsManager.StartInventorDateTimeSet(value);
				this.RaisePropertyChanged(() => this.StartInventorDateTime);
				//Thread.Sleep(500);
				//BuildMasterItems();
			}
		}

		public DateTime EndInventorDateTime
		{
			get { 
				//return _endInventorDateTime; 
				return this._userSettingsManager.EndInventorDateTimeGet();
			}
			set
			{
				//_endInventorDateTime = value;
				this._userSettingsManager.EndInventorDateTimeSet(value);
				this.RaisePropertyChanged(() => this.EndInventorDateTime);
				//Thread.Sleep(500);
				//BuildMasterItems();
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

                BuildMasterItems();
            }
        }

        public int ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => this.ItemsTotal);
            }
        }

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return this._deleteCommand; }
        }

		public ObservableCollection<DeviceItemViewModel> Items
        {
            get
            {
                return this._items;
            }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return this._yesNoRequest; }
        }

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

		public ObservableCollection<DocumentHeaderItemViewModel> DetailItems
        {
            get { return this._detailItems; }
        }

		public DocumentHeaderItemViewModel DetailSelectedItem
        {
            get { return this._detailSelectedItem; }
            set { this._detailSelectedItem = value; }
        }

        public DelegateCommand ReportCommand
        {
            get { return this._reportCommand; }
        }

        public ReportButtonViewModel ReportButton
        {
            get { return _reportButton; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<DeviceAddedEditedEvent>().Subscribe(DeviceAddedEdited);

       

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionProductsGet();

            InitSearchFilter(navigationContext);

			this._userSettingsManager.StartInventorDateTimeSet(base.CurrentInventor.InventorDate);
			this.StartInventorDateTime = this._userSettingsManager.StartInventorDateTimeGet();

			this.CountTheLastForDevice();
			Thread.Sleep(500);
			//this.EndInventorDateTime = this._theLastForAllDevice;
			//this._userSettingsManager.EndInventorDateTimeSet(this._theLastForAllDevice);
			this._userSettingsManager.EndInventorDateTimeSet(DateTime.Now);
			this.EndInventorDateTime = this._userSettingsManager.EndInventorDateTimeGet();

			Device dev = new Device();
			dev.StartInventorDateTime = this.StartInventorDateTime;			//?
			dev.EndInventorDateTime = this.EndInventorDateTime;

			this._reportButton.OnNavigatedTo(navigationContext);
			this._reportButton.Initialize(this.ReportCommandExecuted, () =>
			{
				SelectParams sp = BuildMasterSelectParams();
				sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, dev);
			}, ViewDomainContextEnum.Device);


			//this._reportViewer = InitReportHost(this._reportViewer, "[Rep-DV1-103]");

		//	Task.Factory.StartNew(BuildMasterItems).LogTaskFactoryExceptions("OnNavigatedTo");
			BuildMasterItems();
			
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<DeviceAddedEditedEvent>().Unsubscribe(DeviceAddedEdited);

            this._reportButton.OnNavigatedFrom(navigationContext);
        }

        private void InitSearchFilter(NavigationContext navigationContext)
        {
			//_searchFilterViewModel = Utils.GetViewModelFromRegion<SearchFilterViewModel>(Common.RegionNames.ProductSearchFilter + SearchFilterRegionKey, this._regionManager);

			//_searchFilterViewModel.FilterAction = BuildMasterItems;

			//_searchFilterViewModel.PopupExtSearch.NavigationData = new ProductFilterData();
			//_searchFilterViewModel.PopupExtSearch.Region = Common.RegionNames.PopupSearchCatalogForm;
			//_searchFilterViewModel.PopupExtSearch.ViewModel = this;
			//_searchFilterViewModel.PopupExtSearch.Init();

			//_searchFilterViewModel.PopupExtFilter.Region = Common.RegionNames.PopupFilterCatalogForm;
			//_searchFilterViewModel.PopupExtFilter.ViewModel = this;
			//_searchFilterViewModel.PopupExtFilter.View = Common.ViewNames.FilterView;
			//_searchFilterViewModel.PopupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, _searchFilterViewModel.Filter, Common.NavigationObjects.Filter);
			//_searchFilterViewModel.PopupExtFilter.Init();

			//_searchFilterViewModel.Filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, true) as ProductFilterData;
			//if (_searchFilterViewModel.Filter == null)
			//{
			//	ProductFilterData filterData = new ProductFilterData();
			//	filterData.SortField = "Makat";
			//	filterData.SortDirection = enSortDirection.ASC;
			//	_searchFilterViewModel.Filter = filterData;                
			//}
        }

        private SelectParams BuildMasterSelectParams()
        {
            SelectParams result = new SelectParams();

            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = this._pageSize;
            result.CurrentPage = this._pageCurrent;
			//to do
			//ProductFilterData productFilter = _searchFilterViewModel.Filter as ProductFilterData;
			//if (productFilter != null)
			//{
			//	productFilter.ApplyToSelectParams(result, _deviceRepository, base.GetDbPath);
			//}

            return result;
        }


		private void CountTheLastForDevice()
		{

			this._theLastForAllDevice = this._deviceRepository.GetTheLastForDevice(base.GetDbPath);
	
		}

		private void BuildMasterItems()
		{
			SelectParams sp = null;
			Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
			try
			{
				sp = BuildMasterSelectParams();

				//Devices devices = this._deviceRepository.GetDevices(sp, base.GetDbPath, true);
				//DateTime startInventorDate = base.CurrentInventor.InventorDate;
				//DateTime endInventorDate = DateTime.Now;
				DateTime startInventorDateTime = this._userSettingsManager.StartInventorDateTimeGet();
				DateTime endInventorDateTime = this._userSettingsManager.EndInventorDateTimeGet();
				Devices devices = this._deviceRepository.RefillDeviceStatisticByDeviceCode(startInventorDateTime, endInventorDateTime, sp, base.GetDbPath);

				List<string> deviceCodeList = devices.Select(s => s.DeviceCode).ToList();

				List<DeviceItemViewModel> uiItems = new List<DeviceItemViewModel>();
				foreach (Device device in devices)
				{
					DeviceItemViewModel viewModel = new DeviceItemViewModel(_eventAggregator, device, startInventorDateTime, endInventorDateTime);
					uiItems.Add(viewModel);
				}

				Utils.RunOnUI(() =>
								  {
									  Utils.RunOnUI(() => this._items.Clear());

									  uiItems.ForEach(r => _items.Add(r));
									  ItemsTotal = (int)devices.Count();
								  });

				if ((devices.Count() > 0) && (devices.Count == 0))	//do not show empty space - move on previous page                
				{
					Utils.RunOnUI(() => PageCurrent = this._pageCurrent - 1);
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("BuildMasterItems", exc);
				_logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
				if (sp != null)
					_logger.Error("SelectParams: {0}", sp.ToString());
				Mouse.OverrideCursor = null;
				throw;
			}

			if (ShowDetails == false)
			{
				Utils.RunOnUI(() => this._reportViewer = InitReportHost(this._reportViewer, "[Rep-DV1-103]"));
			}
			Mouse.OverrideCursor = null;
		}

			
			
        

		public void SelectedItemsSet(List<DeviceItemViewModel> list)
        {
            this._selectedItems = list;

            this._deleteCommand.RaiseCanExecuteChanged();
            this._editCommand.RaiseCanExecuteChanged();

            this._timer.Stop();
            this._timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            using (new CursorWait())
            {
                this._timer.Stop();
                this.BuildDetailsItems();
            }
        }

        private void AddCommandExecuted()
        {
            this._eventAggregator.GetEvent<DeviceAddEditEvent>().Publish(new DeviceAddEditEventPayload() { Device = null, Context = base.Context, DbContext = base.CBIDbContext });
        }

        private bool DeleteCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }

        private void DeleteCommandExecuted()
        {
            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Title = String.Empty;
            notification.Settings = this._userSettingsManager;
            string productNames = this._selectedItems.Select(r => r.Device.DeviceCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z));
            productNames = productNames.Remove(productNames.Length - 1, 1);
            notification.Content = String.Format(Localization.Resources.Msg_Delete_Product, productNames);
            this._yesNoRequest.Raise(notification, r =>
            {
                if (r.IsYes == true)
                {
                    using (new CursorWait())
                    {
						foreach (Device device in this._selectedItems.Select(z => z.Device))
                        {
							this._deviceRepository.Delete(device, base.GetDbPath);
                        }

                        BuildMasterItems();
                    }
                }
            }
                );
        }

        private bool EditCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count == 1;
		
        }

        private void EditCommandExecuted()
        {
			Device device = this._selectedItems.First().Device;
			this._eventAggregator.GetEvent<DeviceAddEditEvent>().Publish(new DeviceAddEditEventPayload { Device = device, Context = base.Context, DbContext = base.CBIDbContext 
			,  PeriodFromStartDate = device.PeriodFromFirstToLast  , QuentetyEdit = device.QuantityEdit
			});
        }

		private void DeviceAddedEdited(DeviceAddedEditedEventPayload payload)
        {
			//if (payload.IsNew)
			//{
                BuildMasterItems();
			//}
			//else
			//{
			//	DeviceItemViewModel viewModel = this._items.FirstOrDefault(r => r.Device.DeviceCode == payload.Device.DeviceCode);
			//	if (viewModel != null)
			//		viewModel.DeviceSet(payload.Device);
			//}
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeCatalog);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

		private void BuildDetailsItems()
		{
			this._detailItems.Clear();
			if (this.ShowDetails == true)
			{

				if (this._selectedItems.Count != 1)
					return;

				DeviceItemViewModel viewModel = this._selectedItems.FirstOrDefault();
				if (viewModel == null) return;

				Device master = viewModel.Device;

				SelectParams selectParams = new SelectParams();
				try
				{
					selectParams.FilterParams.Add("Name", new FilterParam() { Operator = FilterOperator.Equal, Value = master.DeviceCode });

					DocumentHeaders docs = this._documentHeaderRepository.GetDocumentHeaders(selectParams, base.GetDbPath);
					foreach (DocumentHeader doc in docs)
					{
						DocumentHeaderItemViewModel viewModel1 = new DocumentHeaderItemViewModel(doc, master.DeviceCode, null);
						this._detailItems.Add(viewModel1);
					}
				}
				catch (Exception exc)
				{
					_logger.ErrorException("BuildDetailsItems", exc);
					_logger.Error("SelectParams: {0}", selectParams.ToString());
					throw;
				}


			}
			else
			{
				//	Utils.RunOnUI(() => this._reportViewer = InitReportHost(this._reportViewer, "[Rep-DV1-103]"));
			}
		}

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Inventor);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            SelectParams sp = this.BuildMasterSelectParams();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
                return String.Empty;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

      

        #endregion        

     
       
    }
}