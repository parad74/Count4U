using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.Section;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System.Windows;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.SectionControl
{
    public class SearchSectionViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ISectionRepository _sectionRepository;

        private readonly ObservableCollection<SectionItemViewModel> _list;
        private SectionItemViewModel _chooseCurrent;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly DelegateCommand _moreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchSectionViewModel(IContextCBIRepository contextCbiRepository,
            ISectionRepository sectionRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager
            )
            : base(contextCbiRepository)
        {
            _sectionRepository = sectionRepository;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;

            _list = new ObservableCollection<SectionItemViewModel>();
            _moreCommand = new DelegateCommand(MoreCommandExecuted);
        }

        public FrameworkElement View { get; set; }

        public ObservableCollection<SectionItemViewModel> List
        {
            get { return _list; }
        }

        public SectionItemViewModel ChooseCurrent
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
            get { return String.Format(Localization.Resources.ViewModel_SearchSectionTotal, _itemsTotal); }
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

            Sections sections = _sectionRepository.GetSections(sp, base.GetDbPath);

            if (sections == null) return;

            List<SectionItemViewModel> toAdd = new List<SectionItemViewModel>();

            foreach (Count4U.Model.Count4U.Section section in sections)
            {
                SectionItemViewModel viewModel = new SectionItemViewModel(section);
                toAdd.Add(viewModel);
            }

            Utils.RunOnUI(() =>
            {
                _list.Clear();
                toAdd.ForEach(r => _list.Add(r));
                this.ItemsTotal = (int)sections.TotalCount;

                if ((sections.TotalCount > 0)
                    && (sections.Count == 0)) //do not show empty space - move on previous page
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
                SectionFilterData filterData = _searchFieldViewModel.BuildFilterData() as SectionFilterData;
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
                SectionFilterData filterData = _searchFieldViewModel.BuildFilterData() as SectionFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.SectionAddEditDeleteOpen(_regionManager, query);

        }
    }
}