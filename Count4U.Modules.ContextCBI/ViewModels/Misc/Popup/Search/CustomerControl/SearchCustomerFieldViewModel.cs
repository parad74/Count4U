using System.Collections.Generic;
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
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using System;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.CustomerControl
{
    public class SearchCustomerFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private readonly IRegionManager _regionManager;

        private string _name;
        private string _phone;
        private string _address;
        private string _code;
        private string _contactPerson;

        private DelegateCommand _searchCommand;

        private SortViewModel _sortViewModel;

        public SearchCustomerFieldViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
            _regionManager = regionManager;
			_name = String.Empty;
			_phone = String.Empty;
			_address = String.Empty;
			_code = String.Empty;
			_contactPerson = String.Empty;

        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);

                if (_searchCommand != null)
                    _searchCommand.RaiseCanExecuteChanged();
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                RaisePropertyChanged(() => Phone);

                if (_searchCommand != null)
                    _searchCommand.RaiseCanExecuteChanged();
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged(() => Address);

                if (_searchCommand != null)
                    _searchCommand.RaiseCanExecuteChanged();
            }
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);

                if (_searchCommand != null)
                    _searchCommand.RaiseCanExecuteChanged();
            }
        }

        public string ContactPerson
        {
            get { return _contactPerson; }
            set
            {
                _contactPerson = value;
                RaisePropertyChanged(() => ContactPerson);

                if (_searchCommand != null)
                    _searchCommand.RaiseCanExecuteChanged();

            }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildSort();
        }

        private void BuildSort()
        {
            _sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, _regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Code));
            sortProperties.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Name));
            sortProperties.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Description));
            sortProperties.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.ContactPerson));
            sortProperties.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Phone));
            sortProperties.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Address));

            _sortViewModel.Add(sortProperties);
        }

        public bool CanSearch()
        {
            return true;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.CustomerSearch;
        }
      
        public IFilterData BuildFilterData()
        {
            CustomerFilterData result = new CustomerFilterData();

            result.Address = _address;
            result.Code = _code;
            result.ContactPerson = _contactPerson;
            result.Name = _name;
            result.Phone = _phone;

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			CustomerFilterData result = new CustomerFilterData();

			result.Address = _address;
			result.Code = _code;
			result.ContactPerson = _contactPerson;
			result.Name = _name;
			result.Phone = _phone;

			_sortViewModel.ApplyToFilterData(result);

			return result;
		}

        public void ApplyFilterData(IFilterData data)
        {
            CustomerFilterData customerData = data as CustomerFilterData;

            if (customerData == null) return;

            Name = customerData.Name;
            Phone = customerData.Phone;
            Address = customerData.Address;
            Code = customerData.Code;
            ContactPerson = customerData.ContactPerson;

            _sortViewModel.InitFromFilterData(customerData);
        }

        public void Reset()
        {
            Name = String.Empty;
            Phone = String.Empty;
            Address = String.Empty;
            Code = String.Empty;
            ContactPerson = String.Empty;

            _sortViewModel.Reset();            
        }
    }
}