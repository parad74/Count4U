using System;
using System.Collections.ObjectModel;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.Section;
using Count4U.Modules.Audit.ViewModels.Supplier;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Generic;
using Count4U.Modules.Audit.ViewModels.Family;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.FamilyControl
{
	public class SearchFamilyViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
         private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
		private readonly IFamilyRepository _familyRepository;

		private readonly ObservableCollection<FamilyItemViewModel> _list;
		private FamilyItemViewModel _chooseCurrent;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly DelegateCommand _moreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchFamilyViewModel(IContextCBIRepository contextCbiRepository,
			IFamilyRepository familyRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager
            )
            : base(contextCbiRepository)
        {
			_familyRepository = familyRepository;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;

			_list = new ObservableCollection<FamilyItemViewModel>();
            _moreCommand = new DelegateCommand(MoreCommandExecuted);
        }

        public FrameworkElement View { get; set; }

		public ObservableCollection<FamilyItemViewModel> List
        {
            get { return _list; }
        }

		public FamilyItemViewModel ChooseCurrent
        {
            get { return _chooseCurrent; }
            set
            {
                _chooseCurrent = value;
                RaisePropertyChanged(() => ChooseCurrent);
            }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RaisePropertyChanged(() => PageSize);
            }
        }

        public int PageCurrent
        {
            get { return _pageCurrent; }
            set
            {
                _pageCurrent = value;
                RaisePropertyChanged(() => PageCurrent);

                using (new CursorWait())
                {
                    Build();
                }
            }
        }

        public int ItemsTotal
        {
            get { return _itemsTotal; }
            set
            {
                _itemsTotal = value;
                RaisePropertyChanged(() => ItemsTotal);
                RaisePropertyChanged(() => TotalString);
            }
        }

        public string TotalString
        {
			get { return String.Format(Localization.Resources.ViewModel_SearchFamilyTotal, _itemsTotal); }
        }

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _pageCurrent = 1;
            //_pageSize = this._userSettingsManager.PortionItursGet();
            _pageSize = 50;
        }

        private void Build()
        {
            Utils.RunOnUI(() => _list.Clear());

            SelectParams sp = SelectParamsBuild();

			Familys familys = _familyRepository.GetFamilys(sp, base.GetDbPath);

            if (familys == null) return;

			List<FamilyItemViewModel> toAdd = new List<FamilyItemViewModel>();

			foreach (Count4U.Model.Count4U.Family family in familys)
            {
				FamilyItemViewModel viewModel = new FamilyItemViewModel(family);
                toAdd.Add(viewModel);
            }

            Utils.RunOnUI(() =>
            {
                _list.Clear();
                toAdd.ForEach(r => _list.Add(r));
                this.ItemsTotal = (int)familys.TotalCount;

                if ((familys.TotalCount > 0)
                    && (familys.Count == 0)) //do not show empty space - move on previous page
                {
                    this.PageCurrent = this._pageCurrent - 1;
                }
            });
        }

        private SelectParams SelectParamsBuild()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Name ASC";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = _pageSize;
            result.CurrentPage = _pageCurrent;

            if (_searchFieldViewModel != null)
            {
				FamilyFilterData filterData = _searchFieldViewModel.BuildFilterData() as FamilyFilterData;
                if (filterData != null)
                {
                    filterData.ApplyToSelectParams(result);
                }
            }

            return result;
        }

        public void Search()
        {
            _pageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => PageCurrent));
            Build();
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
            return SelectParamsBuild();
        }

        private void MoreCommandExecuted()
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            if (_searchFieldViewModel != null)
            {
				FamilyFilterData filterData = _searchFieldViewModel.BuildFilterData() as FamilyFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

			UtilsNavigate.FamilyAddEditDeleteOpen(_regionManager, query);

        } 
    }
}