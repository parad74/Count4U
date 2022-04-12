using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.CustomerControl
{
    public class SearchCustomerViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly ObservableCollection<CustomerItemViewModel> _customerList;
        private CustomerItemViewModel _customerChooseCurrent;

        private int _customersPageSize;
        private int _customersPageCurrent;
        private int _customersItemsTotal;

        private readonly DelegateCommand _customerMoreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchCustomerViewModel(IContextCBIRepository contextCbiRepository,
            ICustomerRepository customerRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager)
            : base(contextCbiRepository)
        {
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;
            _customerRepository = customerRepository;
            _customerList = new ObservableCollection<CustomerItemViewModel>();
            _customerMoreCommand = new DelegateCommand(CustomerMoreCommandExecuted);

        }

        public FrameworkElement View { get; set; }        

        public ObservableCollection<CustomerItemViewModel> CustomerList
        {
            get { return _customerList; }
        }

        public CustomerItemViewModel CustomerChooseCurrent
        {
            get { return _customerChooseCurrent; }
            set
            {
                _customerChooseCurrent = value;
                RaisePropertyChanged(() => CustomerChooseCurrent);
            }
        }

        public int CustomersPageSize
        {
            get { return _customersPageSize; }
            set
            {
                _customersPageSize = value;
                RaisePropertyChanged(() => CustomersPageSize);
            }
        }

        public int CustomersPageCurrent
        {
            get { return _customersPageCurrent; }
            set
            {
                _customersPageCurrent = value;
                RaisePropertyChanged(() => CustomersPageCurrent);

                BuildCustomer();
            }
        }

        public int CustomersItemsTotal
        {
            get { return _customersItemsTotal; }
            set
            {
                _customersItemsTotal = value;
                RaisePropertyChanged(() => CustomersItemsTotal);
                RaisePropertyChanged(() => CustomersTotalString);
            }
        }

        public string CustomersTotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchCustomerTotal, _customersItemsTotal); }
        }

        public DelegateCommand CustomerMoreCommand
        {
            get { return _customerMoreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _customersPageCurrent = 1;
            _customersPageSize = this._userSettingsManager.PortionCBIGet();            
        }

        private void BuildCustomer()
        {
            SelectParams sp = SelectParamsCustomer();

            Customers customers = _customerRepository.GetCustomers(sp);

            if (customers == null) return;

            List<CustomerItemViewModel> toAdd = new List<CustomerItemViewModel>();
            foreach (var customer in customers)
            {
                toAdd.Add(new CustomerItemViewModel(customer, base.ContextCBIRepository));
            }

            Utils.RunOnUI(() =>
            {
                _customerList.Clear();
                toAdd.ForEach(r => _customerList.Add(r));
                this.CustomersItemsTotal = (int)customers.TotalCount;

                if ((customers.TotalCount > 0)
                    && (customers.Count == 0)) //do not show empty space - move on previous page
                {
                    this.CustomersPageCurrent = this._customersPageCurrent - 1;
                }
            });
        }

        private SelectParams SelectParamsCustomer()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Name";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = _customersPageSize;
            result.CurrentPage = _customersPageCurrent;

            if (_searchFieldViewModel != null)
            {
                CustomerFilterData filterData = _searchFieldViewModel.BuildFilterData() as CustomerFilterData;
                if (filterData != null)
                    filterData.ApplyToSelectParams(result);
            }

            return result;
        }

        public void CustomerNavigate(CustomerItemViewModel viewModel)
        {
            UtilsPopup.Close(View);
            Customer customer = viewModel.Customer;
            base.ContextCBIRepository.SetCurrentCustomer(customer, GetAuditConfigByCurrentContext());

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.CustomerDashboardOpen(this.Context, this._regionManager, query);
        }

        private void CustomerMoreCommandExecuted()
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            if (_searchFieldViewModel != null)
            {
                CustomerFilterData filterData = _searchFieldViewModel.BuildFilterData() as CustomerFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.CustomerChooseOpen(CBIContext.Main, this._regionManager, query);

        }

        public void Search()
        {
            _customersPageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => CustomersPageCurrent));
            BuildCustomer();
        }

        public Action<bool> IsBusy { get; set; }

        public ISearchFieldViewModel SearchFieldViewModel
        {
            set { _searchFieldViewModel = value; }
        }

        public void CanSearch(bool isCanSearch)
        {
            
        }

        public SelectParams BuildSelectParams()
        {
            return SelectParamsCustomer();
        }
    }
}