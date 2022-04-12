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

namespace Count4U.Modules.Audit.ViewModels.Catalog
{
    public class CatalogFormViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IProductRepository _productRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IRegionManager _regionManager;
        private readonly UICommandRepository _commandRepository;
        private readonly INavigationRepository _navigationRepository;        

        private readonly DelegateCommand _addCommand;        
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _reportCommand;
		private readonly DelegateCommand _goToPageCommand;
		

        private int _pageSize;
        private int _pageCurrent;
		private int _goToPage;
        private int _itemsTotal;
		private int _pagesCount;

        private List<ProductItemViewModel> _selectedItems;
        private readonly ObservableCollection<ProductItemViewModel> _items;

        private ProductItemViewModel _detailSelectedItem;
        private readonly ObservableCollection<ProductItemViewModel> _detailItems;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

        private readonly ReportButtonViewModel _reportButton;

        private readonly DispatcherTimer _timer;

        private SearchFilterViewModel _searchFilterViewModel;

        public CatalogFormViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IProductRepository productRepository,
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
            this._productRepository = productRepository;
            this._eventAggregator = eventAggregator;

            this._addCommand = _commandRepository.Build(enUICommand.Add, AddCommandExecuted);         
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecute);
            this._importCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted);
            this._reportCommand = _commandRepository.Build(enUICommand.Report, ReportCommandExecuted);
			this._goToPageCommand = _commandRepository.Build(enUICommand.GoToPage, GoToPageCommandExecuted);

            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._items = new ObservableCollection<ProductItemViewModel>();
            this._detailItems = new ObservableCollection<ProductItemViewModel>();

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(this._userSettingsManager.DelayGet());
            this._timer.Tick += Timer_Tick;
        }     

        public string SearchFilterRegionKey { get; set; }

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
				int val = value;
				if (val >= PagesCount) val = PagesCount;
				if (val < 1) val = 1;
				this._pageCurrent = val;
				this._goToPage = val;
                this.RaisePropertyChanged(() => this.PageCurrent);
				this.RaisePropertyChanged(() => this.GoToPage);

                BuildMasterItems();
            }
        }



		public int GoToPage
        {
			get { return this._goToPage; }
            set
            {
				int val = value;
				if (val >= PagesCount) val = PagesCount;
				if (val < 1) val = 1;
				this._goToPage = val;
				this.RaisePropertyChanged(() => this.GoToPage);
				if (this._pageCurrent != val)
				{
					this._pageCurrent = val;
					this.RaisePropertyChanged(() => this.PageCurrent);
					BuildMasterItems();
				}
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

		public int PagesCount
        {
            get { return this._pagesCount; }
		}

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return this._deleteCommand; }
        }

        public ObservableCollection<ProductItemViewModel> Items
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

        public ObservableCollection<ProductItemViewModel> DetailItems
        {
            get { return this._detailItems; }
        }

        public ProductItemViewModel DetailSelectedItem
        {
            get { return this._detailSelectedItem; }
            set { this._detailSelectedItem = value; }
        }

        public DelegateCommand ReportCommand
        {
            get { return this._reportCommand; }
        }


		public DelegateCommand GoToPageCommand
        {
			get { return this._goToPageCommand; }
        }

        public ReportButtonViewModel ReportButton
        {
            get { return _reportButton; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<ProductAddedEditedEvent>().Subscribe(ProductAddedEdited);

            this._reportButton.OnNavigatedTo(navigationContext);
            this._reportButton.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = BuildMasterSelectParams();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Catalog);

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionProductsGet();

            InitSearchFilter(navigationContext);

			Task.Factory.StartNew(BuildMasterItems).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<ProductAddedEditedEvent>().Unsubscribe(ProductAddedEdited);

            this._reportButton.OnNavigatedFrom(navigationContext);
        }

        private void InitSearchFilter(NavigationContext navigationContext)
        {
            _searchFilterViewModel = Utils.GetViewModelFromRegion<SearchFilterViewModel>(Common.RegionNames.ProductSearchFilter + SearchFilterRegionKey, this._regionManager);

            _searchFilterViewModel.FilterAction = BuildMasterItems;

            _searchFilterViewModel.PopupExtSearch.NavigationData = new ProductFilterData();
            _searchFilterViewModel.PopupExtSearch.Region = Common.RegionNames.PopupSearchCatalogForm;
            _searchFilterViewModel.PopupExtSearch.ViewModel = this;
            _searchFilterViewModel.PopupExtSearch.Init();

            _searchFilterViewModel.PopupExtFilter.Region = Common.RegionNames.PopupFilterCatalogForm;
            _searchFilterViewModel.PopupExtFilter.ViewModel = this;
            _searchFilterViewModel.PopupExtFilter.View = Common.ViewNames.FilterView;
            _searchFilterViewModel.PopupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, _searchFilterViewModel.Filter, Common.NavigationObjects.Filter);
            _searchFilterViewModel.PopupExtFilter.Init();

            _searchFilterViewModel.Filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, true) as ProductFilterData;
            if (_searchFilterViewModel.Filter == null)
            {
                ProductFilterData filterData = new ProductFilterData();
                filterData.SortField = "Makat";
                filterData.SortDirection = enSortDirection.ASC;
                _searchFilterViewModel.Filter = filterData;                
            }
        }

        private SelectParams BuildMasterSelectParams()
        {
            SelectParams result = new SelectParams();

            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = this._pageSize;
            result.CurrentPage = this._pageCurrent;

            ProductFilterData productFilter = _searchFilterViewModel.Filter as ProductFilterData;
            if (productFilter != null)
            {
                productFilter.ApplyToSelectParams(result, _productRepository, base.GetDbPath);
            }

            return result;
        }

        private void BuildMasterItems()
        {
            SelectParams sp = null;

            try
            {
                sp = BuildMasterSelectParams();  

                Products products = this._productRepository.GetProducts(sp, base.GetDbPath);
                List<ProductItemViewModel> uiItems = new List<ProductItemViewModel>();
                foreach (Product product in products)
                {

                    ProductItemViewModel viewModel = new ProductItemViewModel(product);
                    uiItems.Add(viewModel);
                }

                Utils.RunOnUI(() =>
                                  {
                                      Utils.RunOnUI(() => this._items.Clear());

                                      uiItems.ForEach(r => _items.Add(r));
                                      ItemsTotal = (int)products.TotalCount;
									  _pagesCount = (int)Math.Ceiling((double)ItemsTotal / (double)PageSize);
                                  });

                if ((products.TotalCount > 0) && (products.Count == 0))	//do not show empty space - move on previous page                
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
                throw;
            }
        }

        public void SelectedItemsSet(List<ProductItemViewModel> list)
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
            this._eventAggregator.GetEvent<ProductAddEditEvent>().Publish(new ProductAddEditEventPayload() { Product = null, Context = base.Context, DbContext = base.CBIDbContext });
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
            string productNames = this._selectedItems.Select(r => r.Product.Makat).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z));
            productNames = productNames.Remove(productNames.Length - 1, 1);
            notification.Content = String.Format(Localization.Resources.Msg_Delete_Product, productNames);
            this._yesNoRequest.Raise(notification, r =>
            {
                if (r.IsYes == true)
                {
                    using (new CursorWait())
                    {
                        foreach (Product product in this._selectedItems.Select(z => z.Product))
                        {
                            this._productRepository.Delete(product.Makat, base.GetDbPath);
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
            Product product = this._selectedItems.First().Product;
            this._eventAggregator.GetEvent<ProductAddEditEvent>().Publish(new ProductAddEditEventPayload { Product = product, Context = base.Context, DbContext = base.CBIDbContext });
        }

        private void ProductAddedEdited(ProductAddedEditedEventPayload payload)
        {
            if (payload.IsNew)
            {
                BuildMasterItems();
            }
            else
            {
                ProductItemViewModel viewModel = this._items.FirstOrDefault(r => r.Product.Makat == payload.Product.Makat);
                if (viewModel != null)
                    viewModel.ProductSet(payload.Product);
            }
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


		private void GoToPageCommandExecuted()
        {
			this.PageCurrent = this.GoToPage;
			this.RaisePropertyChanged(() => this.PageCurrent);
			this.RaisePropertyChanged(() => this.GoToPage);
		}

        private void BuildDetailsItems()
        {
            this._detailItems.Clear();

            if (this._selectedItems.Count != 1)
                return;

            ProductItemViewModel viewModel = this._selectedItems.FirstOrDefault();
            if (viewModel == null) return;

            Product master = viewModel.Product;

            SelectParams selectParams = new SelectParams();
            try
            {
                selectParams.FilterParams.Add("ParentMakat", new FilterParam() { Operator = FilterOperator.Equal, Value = master.Makat });

                Products products = this._productRepository.GetProducts(selectParams, base.GetDbPath);
                foreach (Product product in products)
                {
                    viewModel = new ProductItemViewModel(product);
                    this._detailItems.Add(viewModel);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildDetailsItems", exc);
                _logger.Error("SelectParams: {0}", selectParams.ToString());
                throw;
            }
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Customer);
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