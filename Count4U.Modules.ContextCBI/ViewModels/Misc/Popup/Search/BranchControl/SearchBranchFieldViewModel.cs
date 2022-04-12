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
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.BranchControl
{
    public class SearchBranchFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IRegionManager _regionManager;

        private string _name;
        private string _phone;
        private string _address;
        private string _codeLocal;
        private string _codeERP;
        private string _contactPerson;

        private DelegateCommand _searchCommand;

        private readonly ObservableCollection<Customer> _customerList;
        private Customer _customerListSelected;

        private SortViewModel _sortViewModel;

        public SearchBranchFieldViewModel(
            IContextCBIRepository contextCbiRepository,
            ICustomerRepository customerRepository,
            IRegionManager regionManager)
            :base(contextCbiRepository)
        {
            _regionManager = regionManager;
            _customerRepository = customerRepository;
			_name = String.Empty;
			_phone = String.Empty;
			_address = String.Empty;
			_codeERP = String.Empty;
			_codeLocal = String.Empty;
			_contactPerson = String.Empty;
	
            _customerList = new ObservableCollection<Customer>();
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                RaisePropertyChanged(() => Phone);
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged(() => Address);
            }
        }

        public string CodeLocal
        {
            get { return _codeLocal; }
            set
            {
                _codeLocal = value;
                RaisePropertyChanged(() => CodeLocal);
            }
        }

        public string CodeERP
        {
            get { return _codeERP; }
            set
            {
                _codeERP = value;
                RaisePropertyChanged(() => CodeERP);
            }
        }

        public string ContactPerson
        {
            get { return _contactPerson; }
            set
            {
                _contactPerson = value;
                RaisePropertyChanged(() => ContactPerson);
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
            }
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

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.BranchSearch;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildCustomerList();
            BuildSort();
        }

        private void BuildSort()
        {
            _sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, _regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.Code));
            sortProperties.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.CustomerCode));
            sortProperties.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.Name));
            sortProperties.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.BranchCodeLocal));
            sortProperties.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.BranchCodeERP));
            sortProperties.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.ContactPerson));
            sortProperties.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.Phone));

            _sortViewModel.Add(sortProperties);
        }

        private void BuildCustomerList()
        {
            Customer all = new Customer();
            all.Code = Common.Constants.ComboValues.AllValue.All;
            all.Name = Common.Constants.ComboValues.AllValue.AllName;
            _customerList.Add(all);

            foreach (Customer customer in _customerRepository.GetCustomers(base.Context))
            {
                _customerList.Add(customer);
            }

            _customerListSelected = all;
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
        {
            BranchFilterData result = new BranchFilterData();

            result.Address = _address;
            result.CodeERP = _codeERP;
            result.CodeLocal = _codeLocal;
            result.ContactPerson = _contactPerson;
            result.CustomerCode = _customerListSelected == null ? string.Empty : _customerListSelected.Code;
            result.Name = _name;
            result.Phone = _phone;

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

		public IFilterData BuildFilterData()
		{
			BranchFilterData result = new BranchFilterData();

			result.Address = _address;
			result.CodeERP = _codeERP;
			result.CodeLocal = _codeLocal;
			result.ContactPerson = _contactPerson;
			result.CustomerCode = _customerListSelected == null ? string.Empty : _customerListSelected.Code;
			result.Name = _name;
			result.Phone = _phone;

			_sortViewModel.ApplyToFilterData(result);

			return result;
		}

        public void Reset()
        {
            Address = String.Empty;
            CodeERP = String.Empty;
            CodeLocal = String.Empty;
            ContactPerson = String.Empty;
            CustomerListSelected = null;
            Name = String.Empty;
            Phone = String.Empty;
            _sortViewModel.Reset();
        }

        public void ApplyFilterData(IFilterData data)
        {
            BranchFilterData branchData = data as BranchFilterData;

            if (branchData == null) return;

            Address = branchData.Address;
            CodeERP = branchData.CodeERP;
            CodeLocal = branchData.CodeLocal;
            ContactPerson = branchData.ContactPerson;
            CustomerListSelected = _customerList.FirstOrDefault(r => r.Code == branchData.CustomerCode);
            Name = branchData.Name;
            Phone = branchData.Phone;

            _sortViewModel.InitFromFilterData(branchData);
        }
    }
}