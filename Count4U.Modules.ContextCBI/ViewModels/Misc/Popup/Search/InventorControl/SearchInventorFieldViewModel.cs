using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.GenerationReport;
using Count4U.Model.Audit;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.InventorControl
{
    public class SearchInventorFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IBranchRepository _branchRepository;
        private readonly ICustomerRepository _customerRepository;

        private string _code;
        private DateTime? _from;
        private DateTime? _to;
        private string _description;

        private DelegateCommand _searchCommand;

        private readonly ObservableCollection<Customer> _customerList;
        private Customer _customerListSelected;
        private readonly ObservableCollection<Branch> _branchList;
        private Branch _branchListSelected;

        private string _filterCustomer;
        private string _filterBranch;

        private SortViewModel _sortViewModel;

        public SearchInventorFieldViewModel(
            IContextCBIRepository contextCbiRepository,
            IBranchRepository branchRepository,
            ICustomerRepository customerRepository,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
            _regionManager = regionManager;
            _customerRepository = customerRepository;
            _branchRepository = branchRepository;
			_code = String.Empty;
			_description = String.Empty;

            _customerList = new ObservableCollection<Customer>();
            _branchList = new ObservableCollection<Branch>();
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public bool CanSearch()
        {
            return true;
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.InventorSearch;
        }

        public DateTime? From
        {
            get
            {
                if ((this._from != null)
                    && (this._from.Value.Year < 1900))
                {
                    return new DateTime(1900, 1, 1);
                }
                return this._from;
            }
            set
            {
                _from = value;
                RaisePropertyChanged(() => From);
            }
        }

        public DateTime? To
        {
            get
            {
                if ((this._to != null)
                   && (this._to.Value.Year < 1900))
                {
                    return new DateTime(1900, 1, 1);
                }
                return this._to;
            }
            set
            {
                _to = value;
                RaisePropertyChanged(() => To);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public ObservableCollection<Customer> CustomerList
        {
            get { return _customerList; }
        }

        public Customer CustomerListSelected
        {
            get { return _customerListSelected; }
            set
            {
                _customerListSelected = value;
                RaisePropertyChanged(() => CustomerListSelected);

                BuildBranchList();
            }
        }

        public ObservableCollection<Branch> BranchList
        {
            get { return _branchList; }
        }

        public Branch BranchListSelected
        {
            get { return _branchListSelected; }
            set
            {
                _branchListSelected = value;
                RaisePropertyChanged(() => BranchListSelected);
            }
        }

        public string FilterCustomer
        {
            get { return _filterCustomer; }
            set
            {
                _filterCustomer = value;
                RaisePropertyChanged(() => FilterCustomer);

                BuildCustomerList();
            }
        }

        public string FilterBranch
        {
            get { return _filterBranch; }
            set
            {
                _filterBranch = value;
                RaisePropertyChanged(() => FilterBranch);

                BuildBranchList();
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildCustomerList();
            BuildBranchList();
            BuildSort();
        }

        private void BuildSort()
        {
            _sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, _regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.Code));
            sortProperties.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.CustomerCode));
            sortProperties.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.BranchCode));
            sortProperties.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.Description));
            sortProperties.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.InventorDate));

            _sortViewModel.Add(sortProperties);
        }

        private void BuildCustomerList()
        {
            _customerList.Clear();

            if (String.IsNullOrWhiteSpace(_filterCustomer))
            {
                Customer all = new Customer();
                all.Code = Common.Constants.ComboValues.AllValue.All;
                all.Name = Common.Constants.ComboValues.AllValue.AllName;
                _customerList.Add(all);
            }

            Customers customers = UtilsMisc.FilterCustomers(_customerRepository.GetCustomers(base.Context), _filterCustomer);
            if (customers != null)
            {
                foreach (Customer customer in customers)
                {
                    _customerList.Add(customer);
                }
            }

            CustomerListSelected = _customerList.FirstOrDefault();
        }

        private void BuildBranchList()
        {
            _branchList.Clear();

            SelectParams sp = new SelectParams();
            sp.SortParams = "Name";
            sp.IsEnablePaging = false;

            if (_customerListSelected != null)
            {
                if (String.IsNullOrWhiteSpace(_filterBranch))
                {
                    Branch all = new Branch();
                    all.Code = Common.Constants.ComboValues.AllValue.All;
                    all.Name = Common.Constants.ComboValues.AllValue.AllName;
                    _branchList.Add(all);
                }

                if (_customerListSelected.Code == Common.Constants.ComboValues.AllValue.All)
                {

                }
                else
                {
                    sp.FilterParams.Add("CustomerCode",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Equal,
                                            Value = _customerListSelected.Code
                                        });
                }

                Branches branches = UtilsMisc.FilterBranches(_branchRepository.GetBranches(sp), _filterBranch);
                if (branches != null)
                {
                    foreach (Branch branch in branches)
                    {
                        _branchList.Add(branch);
                    }
                }

                BranchListSelected = _branchList.FirstOrDefault();
            }
        }      

        public IFilterData BuildFilterData()
        {
            InventorFilterData result = new InventorFilterData();

            result.BranchCode = _branchListSelected == null ? String.Empty : _branchListSelected.Code;
            result.CustomerCode = _customerListSelected == null ? String.Empty : _customerListSelected.Code;
            result.Description = _description;
            result.From = _from;
            result.To = _to;
            result.Code = _code;

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			InventorFilterData result = new InventorFilterData();

			result.BranchCode = _branchListSelected == null ? String.Empty : _branchListSelected.Code;
			result.CustomerCode = _customerListSelected == null ? String.Empty : _customerListSelected.Code;
			result.Description = _description;
			result.From = _from;
			result.To = _to;
			result.Code = _code;

			_sortViewModel.ApplyToFilterData(result);

			return result;
		}

        public void Reset()
        {
            CustomerListSelected = null;
            BranchListSelected = null;
            From = null;
            To = null;
            Description = null;
            Code = String.Empty;

            _sortViewModel.Reset();
        }

        public void ApplyFilterData(IFilterData data)
        {
            InventorFilterData inventorData = data as InventorFilterData;

            if (inventorData == null) return;

            CustomerListSelected = _customerList.FirstOrDefault(r => r.Code == inventorData.CustomerCode);
            BranchListSelected = _branchList.FirstOrDefault(r => r.Code == inventorData.BranchCode);
            From = inventorData.From;
            To = inventorData.To;
            Description = inventorData.Description;
            Code = inventorData.Code;

            _sortViewModel.InitFromFilterData(inventorData);
        }
    }
}