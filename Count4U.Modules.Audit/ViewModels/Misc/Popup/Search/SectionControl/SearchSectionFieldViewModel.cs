using System;
using System.Collections.Generic;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.SectionControl
{
    public class SearchSectionFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private readonly IRegionManager _regionManager;

        private string _code;
        private string _name;
		private string _tag;

        private DelegateCommand _searchCommand;

        private SortViewModel _sortViewModel;

        public SearchSectionFieldViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
            _regionManager = regionManager;
			_code = String.Empty;
			_name = String.Empty;
			_tag = String.Empty;

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

		public string Tag
		{
			get { return _tag; }
			set
			{
				_tag = value;
				RaisePropertyChanged(() => Tag);
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
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Section>.GetPropertyInfo(r => r.SectionCode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Section>.GetPropertyInfo(r => r.Name));
			sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Section>.GetPropertyInfo(r => r.Tag));

            _sortViewModel.Add(sortProperties);
        }

        public bool CanSearch()
        {
            return true;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.SectionSearch;
        }       

        public IFilterData BuildFilterData()
        {
            SectionFilterData result = new SectionFilterData();

            result.Code = _code;
            result.Name = _name;
			result.Tag = _tag;

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			SectionFilterData result = new SectionFilterData();

			result.Code = _code;
			result.Name = _name;
			result.Tag = _tag;

			_sortViewModel.ApplyToFilterData(result);

			return result;
		}

        public void ApplyFilterData(IFilterData data)
        {
            SectionFilterData sectionData = data as SectionFilterData;

            if (sectionData == null) return;

            Code = sectionData.Code;
            Name = sectionData.Name;
			Tag = sectionData.Tag;

            _sortViewModel.InitFromFilterData(sectionData);
        }

        public void Reset()
        {
            Code = String.Empty;
            Name = String.Empty;
			Tag = String.Empty;
            _sortViewModel.Reset();
        }
    }
}