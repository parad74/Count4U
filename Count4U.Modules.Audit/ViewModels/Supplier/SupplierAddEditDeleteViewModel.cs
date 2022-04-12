using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.SearchFilter;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.Section;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels.Supplier
{
    public class SupplierAddEditDeleteViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly UICommandRepository _commandRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ReportButtonViewModel _reportButtonViewModel;
        private readonly IServiceLocator _serviceLocator;
        private readonly INavigationRepository _navigationRepository;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _deleteAllCommand;
        private readonly DelegateCommand _importCommand;
        private readonly UICommand _reportCommand;
        private readonly UICommand _repairCommand;

        private readonly ObservableCollection<SupplierItemViewModel> _items;
        private SupplierItemViewModel _selectedItem;
        private List<SupplierItemViewModel> _selectedItems;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private SearchFilterViewModel _searchFilterViewModel;




        public SupplierAddEditDeleteViewModel(IContextCBIRepository contextCBIRepository,
                                             IEventAggregator eventAggregator,
                                             ISupplierRepository supplierRepository,
                                             IRegionManager regionManager,
                                             IUserSettingsManager _userSettingsManager,
                                             ModalWindowLauncher modalWindowLauncher,
                                             UICommandRepository commandRepository,
                                             ReportButtonViewModel reportButtonViewModel,
                                             IServiceLocator serviceLocator,
                                             INavigationRepository navigationRepository
            ) :
            base(contextCBIRepository)
        {
            this._navigationRepository = navigationRepository;
            this._serviceLocator = serviceLocator;
            this._reportButtonViewModel = reportButtonViewModel;
            this._supplierRepository = supplierRepository;
            this._commandRepository = commandRepository;
            this._modalWindowLauncher = modalWindowLauncher;
            this._userSettingsManager = _userSettingsManager;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._items = new ObservableCollection<SupplierItemViewModel>();

            this._addCommand = _commandRepository.Build(enUICommand.Add, this.AddCommandExecuted);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, this.DeleteCommandExecuted, this.DeleteCommandCanExecute);
            this._deleteAllCommand = _commandRepository.Build(enUICommand.DeleteAll, DeleteAllCommandExecuted, DeleteAllCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, this.EditCommandExecuted, this.EditCommandCanExecute);
            this._importCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted);
            this._reportCommand = _commandRepository.Build(enUICommand.Report, ReportCommandExecuted);
            this._repairCommand = _commandRepository.Build(enUICommand.RepairFromDb, RepairCommandExecuted);
        }

        public string SearchFilterRegionKey { get; set; }

        public ObservableCollection<SupplierItemViewModel> Items
        {
            get { return _items; }
        }

        public DelegateCommand ImportCommand
        {
            get { return _importCommand; }
        }

        public SupplierItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                this._deleteCommand.RaiseCanExecuteChanged();
                this._editCommand.RaiseCanExecuteChanged();
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

				Task.Factory.StartNew(Build).LogTaskFactoryExceptions("PageCurrent");
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

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
            get { return _reportButtonViewModel; }
        }

        public DelegateCommand ReportCommand
        {
            get { return this._reportCommand; }
        }

        public UICommand RepairCommand
        {
            get { return _repairCommand; }
        }

        public DelegateCommand DeleteAllCommand
        {
            get { return _deleteAllCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            InitReportButton(navigationContext);
            InitSearchFilter(navigationContext);

            this._pageSize = this._userSettingsManager.PortionSectionsGet();
            this._pageCurrent = 1;

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this.ReportButtonViewModel.OnNavigatedFrom(navigationContext);
        }

        private void InitReportButton(NavigationContext navigationContext)
        {
            this.ReportButtonViewModel.OnNavigatedTo(navigationContext);
            this.ReportButtonViewModel.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = BuildSelectParams();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Supplier);
        }

        private void InitSearchFilter(NavigationContext navigationContext)
        {
            _searchFilterViewModel = Utils.GetViewModelFromRegion<SearchFilterViewModel>(Common.RegionNames.SupplierAddEditDeleteSearchFilter + SearchFilterRegionKey, this._regionManager);

            _searchFilterViewModel.FilterAction = Build;

            _searchFilterViewModel.PopupExtSearch.NavigationData = new SupplierFilterData();
            _searchFilterViewModel.PopupExtSearch.Region = Common.RegionNames.PopupSearchSupplierAddEditDelete;
            _searchFilterViewModel.PopupExtSearch.ViewModel = this;
            _searchFilterViewModel.PopupExtSearch.Init();

            _searchFilterViewModel.PopupExtFilter.Region = Common.RegionNames.PopupFilterSupplierAddEditDelete;
            _searchFilterViewModel.PopupExtFilter.ViewModel = this;
            _searchFilterViewModel.PopupExtFilter.View = Common.ViewNames.FilterView;
            _searchFilterViewModel.PopupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, _searchFilterViewModel.Filter, Common.NavigationObjects.Filter);
            _searchFilterViewModel.PopupExtFilter.Init();

            _searchFilterViewModel.Filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, true) as SupplierFilterData;
            if (_searchFilterViewModel.Filter == null)
                _searchFilterViewModel.Filter = new SupplierFilterData();
        }

        private SelectParams BuildSelectParams()
        {
            SelectParams selectParams = new SelectParams();

            selectParams.IsEnablePaging = true;
            selectParams.CountOfRecordsOnPage = this._pageSize;
            selectParams.CurrentPage = this._pageCurrent;

            SupplierFilterData supplierFilter = _searchFilterViewModel.Filter as SupplierFilterData;
            if (supplierFilter != null)
            {
                supplierFilter.ApplyToSelectParams(selectParams);
            }

            return selectParams;
        }

        private void Build()
        {
            SelectParams selectParams = null;
            try
            {
                selectParams = BuildSelectParams();
                var suppliers = this._supplierRepository.GetSuppliers(selectParams, base.GetDbPath);

                Utils.RunOnUI(() =>
                {
                    this._items.Clear();

                    this.ItemsTotal = (int)suppliers.TotalCount;

                    foreach (Model.Count4U.Supplier supplier in suppliers)
                    {
                        SupplierItemViewModel viewModel = new SupplierItemViewModel(supplier);
                        this._items.Add(viewModel);
                    }

                    if ((suppliers.TotalCount > 0) && (this._items.Count == 0)) //do not show empty space - move on previous page                   
                    {
                        this.PageCurrent = this._pageCurrent - 1;
                    }

                    _deleteAllCommand.RaiseCanExecuteChanged();
                });
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                _logger.Error("SelectParams: {0}", selectParams);
                throw;
            }
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeSupplier);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void AddCommandExecuted()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, base.Context);
            Utils.AddDbContextToDictionary(settings, base.CBIDbContext);

            object result = this._modalWindowLauncher.StartModalWindow(
                Common.ViewNames.SupplierAddEditView,
                Common.Constants.WindowTitles.SupplierAdd,
                360, 360,
                ResizeMode.NoResize,
                settings,
                Application.Current.MainWindow,
                minWidth: 220, minHeight: 220);

            if (result != null)
            {
                Build();
            }
        }

        private bool EditCommandCanExecute()
        {
            return this._selectedItem != null &&
                !String.IsNullOrEmpty(_selectedItem.Code) &&
                _selectedItems != null &&
                 _selectedItems.Count == 1;
        }

        private void EditCommandExecuted()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, base.Context);
            Utils.AddDbContextToDictionary(settings, base.CBIDbContext);
            settings.Add(Common.NavigationSettings.SupplierCode, this._selectedItem.Code);

            object result = this._modalWindowLauncher.StartModalWindow(
                Common.ViewNames.SupplierAddEditView,
                Common.Constants.WindowTitles.SupplierEdit,
                360, 360,
                ResizeMode.NoResize,
                settings,
                Application.Current.MainWindow,
                minWidth: 220, minHeight: 220);

            if (result != null)
            {
                Model.Count4U.Supplier supplier = result as Model.Count4U.Supplier;
                if (supplier != null)
                {
                    SupplierItemViewModel viewodel = _items.FirstOrDefault(r => r.Code == supplier.SupplierCode);
                    if (viewodel != null)
                    {
                        viewodel.Update(supplier);
                    }
                }
            }
        }

        private bool DeleteCommandCanExecute()
        {
            return this._selectedItem != null && !String.IsNullOrEmpty(_selectedItem.Code);
        }

        private void DeleteCommandExecuted()
        {
            if (_selectedItems.Any() == false) return;

            string message = Localization.Resources.Msg_Delete_Suppliers;
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Information, _userSettingsManager);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    using (new CursorWait())
                    {
                        foreach (SupplierItemViewModel viewModel in _selectedItems)
                        {
                            var supplier = _supplierRepository.GetSupplierByCode(viewModel.Code, base.GetDbPath);
                            if (supplier != null)
                                _supplierRepository.Delete(supplier, base.GetDbPath);
                        }

                        Build();
                    }
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("DeleteCommandExecuted", exc);
                }
            }
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Supplier);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            var sp = this.BuildSelectParams();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void RepairCommandExecuted()
        {
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_Repair, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            using (new CursorWait())
            {
                this._supplierRepository.RepairCodeFromDB(base.GetDbPath);
                //IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
                //List<string> supplierCodeListFromProduct = productRepository.GetSupplierCodeList(base.GetDbPath);			//из
                //List<string> supplierCodeListFromSupplier = this._supplierRepository.GetSupplierCodeList(base.GetDbPath); //в
                //Dictionary<string, string> difference = new Dictionary<string, string>();

                //foreach (var supplierCodeFromProduct in supplierCodeListFromProduct)			   //из
                //{
                //    if (supplierCodeListFromSupplier.Contains(supplierCodeFromProduct) == false)		 //в
                //    {
                //        difference[supplierCodeFromProduct] = supplierCodeFromProduct;
                //    }
                //}

                //foreach (KeyValuePair<string, string> keyValuePair in difference)
                //{
                //    if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
                //    {
                //         Model.Count4U.Supplier newSupplier =  new Model.Count4U.Supplier();
                //         newSupplier.SupplierCode = keyValuePair.Value;
                //         newSupplier.Name = keyValuePair.Value;
                //         newSupplier.Description = "Repair from Product";
                //         this._supplierRepository.Insert(newSupplier, base.GetDbPath);
                //    }
                //}
                UtilsMisc.ShowMessageBox(Localization.Resources.Msg_RestoreDone, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
                Build();

            }
        }

        public void SetSelected(List<SupplierItemViewModel> items)
        {
            _selectedItems = items;

            _editCommand.RaiseCanExecuteChanged();
            _deleteCommand.RaiseCanExecuteChanged();
        }

        private bool DeleteAllCommandCanExecute()
        {
            return _items.Any();
        }

        private void DeleteAllCommandExecuted()
        {
            string message = Localization.Resources.ViewModel_SupplierAddEditDelete_deleteAll;
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Information, _userSettingsManager);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (new CursorWait())
                {
                    try
                    {
                        _supplierRepository.DeleteAll(base.GetDbPath);
                        Build();
                    }
                    catch (Exception exc)
                    {
                        _logger.ErrorException("DeleteAllCommandExecuted", exc);
                    }
                }
            }
        }

    }
}