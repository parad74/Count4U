using System;
using System.Collections.Generic;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.FamilyControl
{
	public class SearchFamilyFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private readonly IRegionManager _regionManager;

        private DelegateCommand _searchCommand;
        private SortViewModel _sortViewModel;

        private string _code;
        private string _name;

        public SearchFamilyFieldViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
			_regionManager = regionManager; 
			_code = String.Empty;
			_name = String.Empty;

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

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
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
			sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Family>.GetPropertyInfo(r => r.FamilyCode));
			sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Family>.GetPropertyInfo(r => r.Name));

            _sortViewModel.Add(sortProperties);
        }

        public bool CanSearch()
        {
            return true;
        }

        public ViewDomainContextEnum GetReportContext()
        {
			return ViewDomainContextEnum.FamilySearch;
        }

        public IFilterData BuildFilterData()
        {
			FamilyFilterData result = new FamilyFilterData();

            result.Code = _code;
            result.Name = _name;

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			FamilyFilterData result = new FamilyFilterData();

			result.Code = _code;
			result.Name = _name;

			_sortViewModel.ApplyToFilterData(result);

			return result;
		}

        public void ApplyFilterData(IFilterData data)
        {
			FamilyFilterData familyData = data as FamilyFilterData;

            if (familyData == null) return;

            _sortViewModel.InitFromFilterData(familyData);

            Name = familyData.Name;
            Code = familyData.Code;
        }

        public void Reset()
        {
            _sortViewModel.Reset();

            Code = String.Empty;
            Name = String.Empty;
        }
    }
}