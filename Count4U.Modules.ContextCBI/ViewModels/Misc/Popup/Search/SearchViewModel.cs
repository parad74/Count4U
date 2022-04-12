using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.FilterTemplate;
using Count4U.Common.ViewModel.Misc;
using Count4U.Common.ViewModel.SearchFilter;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Count4U.Model.SelectionParams;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Count4U.Common.Constants;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search
{
    public partial class SearchViewModel : CBIContextBaseViewModel
    {
        protected const string ObjectTypeCustomer = "Customer";
        private const string ObjectTypeBranch = "Branch";
        private const string ObjectTypeInventor = "Inventor";
        public const string ObjectTypeInventProduct = "InventProduct";
        private const string ObjectTypeInventProductAdvanced = "InventProductAdvanced";
		private const string ObjectTypeInventProductAdvancedAggregate = "InventProductAdvancedAggregate";
        private const string ObjectTypeItur = "Itur";
        private const string ObjectTypeIturAdvanced = "IturAdvanced";
        private const string ObjectTypeLocation = "Location";
        private const string ObjectTypeSection = "Section";
        private const string ObjectTypeSupplier = "Supplier";
        private const string ObjectTypeProduct = "Product";
		private const string ObjectTypeFamily = "Family";
		

        private readonly IRegionManager _regionManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _searchCommand;
        private readonly DelegateCommand _closeCommand;

        private readonly ObservableCollection<ItemFindViewModel> _objectTypeItems;
        private ItemFindViewModel _objectTypeSelectedItem;

        private ISearchFieldViewModel _searchFieldViewModel;
        private ISearchGridViewModel _searchGridViewModel;
        private FilterTemplateViewModel _filterTemplateViewModel;

        private readonly ReportButtonViewModel _reportButton;
        private bool _isBusy;

        private bool _isReportEnabled;        

        public SearchViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            ReportButtonViewModel reportButton,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _reportButton = reportButton;
            _navigationRepository = navigationRepository;
            _regionManager = regionManager;
            _searchCommand = new DelegateCommand(SearchCommandExecuted, SearchCommandCanExecute);
            _closeCommand = new DelegateCommand(CloseCommandExecuted);

            _objectTypeItems = new ObservableCollection<ItemFindViewModel>();

        }

        public FrameworkElement View { get; set; }
        public ContextMenu ReportContextMenu { get; set; }

        public ObservableCollection<ItemFindViewModel> ObjectTypeItems
        {
            get { return _objectTypeItems; }
        }

        public ItemFindViewModel ObjectTypeSelectedItem
        {
            get { return _objectTypeSelectedItem; }
            set
            {
                _objectTypeSelectedItem = value;
                RaisePropertyChanged(() => ObjectTypeSelectedItem);

                _searchCommand.RaiseCanExecuteChanged();

                if (_objectTypeSelectedItem != null)
                {
                    using (new CursorWait())
                    {
                        UriQuery query = new UriQuery();
                        Utils.AddContextToQuery(query, base.Context);
                        Utils.AddDbContextToQuery(query, base.CBIDbContext);

                        string fieldViewName = String.Empty;
                        string gridViewName = String.Empty;

                        switch (_objectTypeSelectedItem.Value)
                        {
                            case ObjectTypeCustomer:
                                gridViewName = Common.ViewNames.SearchCustomerView;
                                fieldViewName = Common.ViewNames.SearchCustomerFieldView;
                                break;
                            case ObjectTypeBranch:
                                gridViewName = Common.ViewNames.SearchBranchView;
                                fieldViewName = Common.ViewNames.SearchBranchFieldView;
                                break;
                            case ObjectTypeInventor:
                                gridViewName = Common.ViewNames.SearchInventorView;
                                fieldViewName = Common.ViewNames.SearchInventorFieldView;
                                break;
                            case ObjectTypeInventProduct:
                                gridViewName = Common.ViewNames.SearchInventProductView;
                                fieldViewName = Common.ViewNames.SearchInventProductFieldView;
                                break;
                            case ObjectTypeInventProductAdvanced:
                                gridViewName = Common.ViewNames.SearchInventProductAdvancedView;
                                fieldViewName = Common.ViewNames.SearchInventProductAdvancedFieldView;
                                break;
							case ObjectTypeInventProductAdvancedAggregate:
								gridViewName = Common.ViewNames.SearchInventProductAdvancedAggregateView;
								fieldViewName = Common.ViewNames.SearchInventProductAdvancedAggregateFieldView;
                                break;
                            case ObjectTypeItur:
                                gridViewName = Common.ViewNames.SearchIturView;
                                fieldViewName = Common.ViewNames.SearchIturFieldView;
                                break;
                            case ObjectTypeIturAdvanced:
                                gridViewName = Common.ViewNames.SearchIturAdvancedView;
                                fieldViewName = Common.ViewNames.SearchIturAdvancedFieldView;
                                break;
                            case ObjectTypeLocation:
                                gridViewName = Common.ViewNames.SearchLocationView;
                                fieldViewName = Common.ViewNames.SearchLocationFieldView;
                                break;
                            case ObjectTypeSection:
                                gridViewName = Common.ViewNames.SearchSectionView;
                                fieldViewName = Common.ViewNames.SearchSectionFieldView;
                                break;
                            case ObjectTypeSupplier:
                                gridViewName = Common.ViewNames.SearchSupplierView;
                                fieldViewName = Common.ViewNames.SearchSupplierFieldView;
                                break;
							case ObjectTypeFamily:
								gridViewName = Common.ViewNames.SearchFamilyView;
								fieldViewName = Common.ViewNames.SearchFamilyFieldView;
								break;
                            case ObjectTypeProduct:
                                gridViewName = Common.ViewNames.SearchProductView;
                                fieldViewName = Common.ViewNames.SearchProductFieldView;
                                break;
                        }

                        _regionManager.RequestNavigate(Common.RegionNames.SearchFieldGround, new Uri(fieldViewName + query, UriKind.Relative));
                        _regionManager.RequestNavigate(Common.RegionNames.SearchGridGround, new Uri(gridViewName + query, UriKind.Relative));

                        _searchFieldViewModel = Utils.GetViewModelFromRegion<ISearchFieldViewModel>(Common.RegionNames.SearchFieldGround, _regionManager);
                        _searchFieldViewModel.SearchCommand = _searchCommand;

                        _searchGridViewModel = Utils.GetViewModelFromRegion<ISearchGridViewModel>(Common.RegionNames.SearchGridGround, _regionManager);
                        _searchGridViewModel.IsBusy = b => Utils.RunOnUI(() => IsBusy = b);
                        _searchGridViewModel.SearchFieldViewModel = _searchFieldViewModel;

                        ViewDomainContextEnum viewDomainContextEnum = ViewDomainContextEnumFromObjectType();

                        this._reportButton.Initialize(this.ReportCommandExecuted, () =>
                            {
                                SelectParams sp = null;
                                sp = BuildReportSelectParams();
                                sp.IsEnablePaging = false;

								return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
                            }, 
                            viewDomainContextEnum,
                            ()=>_searchFieldViewModel.BuildFilterData());

                        _reportButton.BuildMenu(ReportContextMenu);

                        BuildFilterTemplate();

                        _searchCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                Mouse.OverrideCursor = _isBusy ? Cursors.Wait : null;
            }
        }

        public ReportButtonViewModel ReportButton
        {
            get { return _reportButton; }
        }

        public bool IsReportEnabled
        {
            get { return _isReportEnabled; }
            set
            {
                _isReportEnabled = value;
                RaisePropertyChanged(() => IsReportEnabled);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._reportButton.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<SearchModeChangedEvent>().Subscribe(SearchModeChanged);
            
            _filterTemplateViewModel = Utils.GetViewModelFromRegion<FilterTemplateViewModel>(Common.RegionNames.SearchFieldTemplate, _regionManager);

            BuildItems(navigationContext);          
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _reportButton.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<SearchModeChangedEvent>().Unsubscribe(SearchModeChanged);

            if (_searchFieldViewModel != null)
            {
                _searchFieldViewModel.OnNavigatedFrom(navigationContext);
            }

            if (_searchGridViewModel != null)
            {
                _searchGridViewModel.OnNavigatedFrom(navigationContext);
            }
        }

        private void BuildItems(NavigationContext navigationContext)
        {
            ItemFindViewModel customer = new ItemFindViewModel() { Text = Localization.Resources.ViewModel_SearchCustomer, Value = ObjectTypeCustomer };
            ItemFindViewModel branch = new ItemFindViewModel() { Text = Localization.Resources.ViewModel_SearchBranch, Value = ObjectTypeBranch };
            ItemFindViewModel inventor = new ItemFindViewModel() { Text = Localization.Resources.ViewModel_SearchInventor, Value = ObjectTypeInventor };
            ItemFindViewModel inventProduct = new ItemFindViewModel() { Text = Localization.Resources.ViewModel_SearchInventProduct, Value = ObjectTypeInventProduct };
            ItemFindViewModel inventProductAdvanced = new ItemFindViewModel() { Text = Localization.Resources.ViewModel_SearchInventProductAdvanced, Value = ObjectTypeInventProductAdvanced };
			ItemFindViewModel inventProductAdvancedAggregate = new ItemFindViewModel() { Text = Localization.Resources.ViewModel_SearchInventProductAdvancedAggregate, Value = ObjectTypeInventProductAdvancedAggregate };
            ItemFindViewModel itur = new ItemFindViewModel() { Text = Localization.Resources.Domain_Itur, Value = ObjectTypeItur };
            ItemFindViewModel iturAdvanced = new ItemFindViewModel() { Text = Localization.Resources.ViewModel_Search_IturAdvanced, Value = ObjectTypeIturAdvanced };
            ItemFindViewModel location = new ItemFindViewModel() {Text = Localization.Resources.Domain_Location, Value = ObjectTypeLocation};
            ItemFindViewModel section = new ItemFindViewModel() { Text = Localization.Resources.Domain_Section, Value = ObjectTypeSection };
            ItemFindViewModel supplier = new ItemFindViewModel() { Text = Localization.Resources.Domain_Supplier, Value = ObjectTypeSupplier };
            ItemFindViewModel product = new ItemFindViewModel() { Text = Localization.Resources.Domain_Product, Value = ObjectTypeProduct };
			ItemFindViewModel family = new ItemFindViewModel() { Text = Localization.Resources.Domain_Family, Value = ObjectTypeFamily };

            _objectTypeItems.Add(customer);
            _objectTypeItems.Add(branch);
            _objectTypeItems.Add(inventor);

            if (!navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.SearchWithOnlyCBI))
            {
                switch (base.CBIDbContext)
                {
                    case Common.NavigationSettings.CBIDbContextCustomer:
                        _objectTypeItems.Add(itur);
                        _objectTypeItems.Add(location);
                        _objectTypeItems.Add(section);
                        _objectTypeItems.Add(supplier);
						_objectTypeItems.Add(family);
                        break;
                    case Common.NavigationSettings.CBIDbContextBranch:
                        _objectTypeItems.Add(itur);
                        _objectTypeItems.Add(location);
                        _objectTypeItems.Add(section);
                        _objectTypeItems.Add(supplier);
						_objectTypeItems.Add(family);
                        break;
                    case Common.NavigationSettings.CBIDbContextInventor:
                        _objectTypeItems.Add(inventProduct);
                        _objectTypeItems.Add(inventProductAdvanced);
						_objectTypeItems.Add(inventProductAdvancedAggregate);
                        _objectTypeItems.Add(itur);
                        _objectTypeItems.Add(iturAdvanced);
                        _objectTypeItems.Add(product);
                        _objectTypeItems.Add(location);
                        _objectTypeItems.Add(section);
                        _objectTypeItems.Add(supplier);
						_objectTypeItems.Add(family);
                        break;
                }
            }

            ItemFindViewModel selectedItem = _objectTypeItems.FirstOrDefault();

            object navigationData = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.PopupSearch, true);

            CustomerSearchData customerData = navigationData as CustomerSearchData;
            if (customerData != null)
            {
                selectedItem = _objectTypeItems.FirstOrDefault(r => r.Value == ObjectTypeCustomer);
            }

            BranchSearchData branchData = navigationData as BranchSearchData;
            if (branchData != null)
            {
                selectedItem = _objectTypeItems.FirstOrDefault(r => r.Value == ObjectTypeBranch);
            }
            InventorSearchData inventorData = navigationData as InventorSearchData;
            if (inventorData != null)
            {
                selectedItem = _objectTypeItems.FirstOrDefault(r => r.Value == ObjectTypeInventor);
            }
            InventProductSearchData ipData = navigationData as InventProductSearchData;
            if (ipData != null)
            {
                //                _searchInventProductFieldViewModel.InventProductMakat = ipData.Makat;
                //                _searchInventProductFieldViewModel.InventProductBarcode = ipData.Barcode;
                //                _searchInventProductFieldViewModel.InventProductName = ipData.ProductName;

                selectedItem = _objectTypeItems.FirstOrDefault(r => r.Value == ObjectTypeInventProduct);
            }

            ObjectTypeSelectedItem = selectedItem;
        }

        private void CloseCommandExecuted()
        {
            UtilsPopup.Close(View, _eventAggregator);
        }

        private bool SearchCommandCanExecute()
        {
            if (_objectTypeSelectedItem == null)
                return false;

            if (_searchFieldViewModel != null)
            {
                bool r = _searchFieldViewModel.CanSearch();
                IsReportEnabled = r;

                if (_searchGridViewModel != null)
                {
                    _searchGridViewModel.CanSearch(r);
                }
                return r;
            }

            return false;
        }

        private void SearchCommandExecuted()
        {
            if (_objectTypeSelectedItem == null)
                return;

            IsBusy = true;

            Task.Factory.StartNew(() =>
                                      {
                                          _searchGridViewModel.Search();

                                          Utils.RunOnUI(() => IsBusy = false);
									  }).LogTaskFactoryExceptions("SearchCommandExecuted");

        }

        private SelectParams BuildReportSelectParams()
        {
            if (_objectTypeSelectedItem == null || _searchGridViewModel == null)
                return null;

            Func<SelectParams> buildSelectParams = null;

            buildSelectParams = _searchGridViewModel.BuildSelectParams;

            if (buildSelectParams != null)
            {
                SelectParams sp = buildSelectParams();
                sp.IsEnablePaging = false;

                if (_filterTemplateViewModel.SelectedItem != null)
                {
                    sp.FilterTemplate = _filterTemplateViewModel.SelectedItem.Name;
                }

                return sp;
            }
            return null;
        }

        private void ReportCommandExecuted()
        {
            UtilsPopup.Close(View);

            ViewDomainContextEnum viewDomainContextEnum = ViewDomainContextEnumFromObjectType();

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), viewDomainContextEnum);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            var sp = BuildReportSelectParams();
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private ViewDomainContextEnum ViewDomainContextEnumFromObjectType()
        {
            if (_searchFieldViewModel != null)
            {
                return _searchFieldViewModel.GetReportContext();
            }

            return ViewDomainContextEnum.Customer;
        }

        private void SearchModeChanged(object o)
        {
            ViewDomainContextEnum viewDomainContextEnum = ViewDomainContextEnumFromObjectType();
            _reportButton.Rebuild(viewDomainContextEnum);

            BuildFilterTemplate();
        }

        private void BuildFilterTemplate()
        {
            if (_filterTemplateViewModel != null)
            {
                ViewDomainContextEnum viewDomainContextEnum = ViewDomainContextEnumFromObjectType();
                _filterTemplateViewModel.Build(viewDomainContextEnum.ToString());
            }
        }
    }
}