using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl.Grid
{
    public class SearchInventProductAdvancedGridSimpleViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
        private readonly INavigationRepository _navigationRepository;

        private readonly ObservableCollection<InventProductAdvancedItemSimpleViewModel> _list;
        private InventProductAdvancedItemSimpleViewModel _chooseCurrent;
        private readonly IRegionManager _regionManager;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;
        private double _itemsSumTotal;

        private ISearchFieldViewModel _searchFieldViewModel;

        private readonly DelegateCommand _moreCommand;

        private string _totalStringTooltip;

        private bool _isCanSearch;

        public SearchInventProductAdvancedGridSimpleViewModel(
            IContextCBIRepository contextCbiRepository,
            IIturAnalyzesRepository iturAnalyzesRepository,
            IRegionManager regionManager,
            INavigationRepository navigationRepository)
            : base(contextCbiRepository)
        {
            _navigationRepository = navigationRepository;
            _regionManager = regionManager;
            _iturAnalyzesRepository = iturAnalyzesRepository;
            _list = new ObservableCollection<InventProductAdvancedItemSimpleViewModel>();
            _moreCommand = new DelegateCommand(MoreCommandExecuted, MoreCommandCanExecute);
        }        

        public FrameworkElement View { get; set; }

        public ObservableCollection<InventProductAdvancedItemSimpleViewModel> List
        {
            get { return _list; }
        }

        public InventProductAdvancedItemSimpleViewModel ChooseCurrent
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

                if (IsBusy != null)
                    IsBusy(true);
                Task.Factory.StartNew(() =>
                {
                    BuildList();

                    Utils.RunOnUI(() =>
                    {
                        if (IsBusy != null)
                            IsBusy(false);
                    });
				}).LogTaskFactoryExceptions("PageCurrent");
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
                RaisePropertyChanged(() => TotalSumString);
            }
        }

        public string TotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchInventProductAdvancedGridSimple_tbTotal2, _itemsTotal); }
        }

        public string TotalSumString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchInventProductSum, _itemsSumTotal); }
        }

        public string TotalStringTooltip
        {
            get { return _totalStringTooltip; }
            set
            {
                _totalStringTooltip = value;
                RaisePropertyChanged(() => TotalStringTooltip);
            }
        }

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _pageCurrent = 1;
            _pageSize = 100;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        private void BuildList()
        {
            try
            {
                Utils.RunOnUI(() => _list.Clear());
                SelectParams sp = BuildSelectParams();

				Dictionary<object, object> parms = new Dictionary<object, object>();
				SelectParams sp1 = BuildSelectParams();
				sp1.IsEnablePaging = false;
				parms.Add("SelectParams", sp1);

				IturAnalyzesCollection collection = this._iturAnalyzesRepository.GetIturAnalyzesCollection(sp, base.GetDbPath, false, parms:parms);

                List<InventProductAdvancedItemSimpleViewModel> toAdd = new List<InventProductAdvancedItemSimpleViewModel>();
                foreach (IturAnalyzes analyze in collection)
                {
                    InventProductAdvancedItemSimpleViewModel viewModel = new InventProductAdvancedItemSimpleViewModel(analyze);
                    toAdd.Add(viewModel);
                }

                Utils.RunOnUI(() =>
                    {
                        toAdd.ForEach(r => _list.Add(r));
                        _itemsSumTotal = collection.SumQuantityEdit;
                        ItemsTotal = (int)collection.TotalCount;
                        if ((collection.TotalCount > 0) && (collection.Count == 0)) //do not show empty space - move on previous page           
                        {
                            PageCurrent = _pageCurrent - 1;
                        }
                    });
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildList", exc);
            }
        }

        public void Search()
        {
            _pageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => PageCurrent));
            BuildList();
        }

        public Action<bool> IsBusy { get; set; }

        public ISearchFieldViewModel SearchFieldViewModel
        {
            set { _searchFieldViewModel = value; }
        }
        
        public void CanSearch(bool isCanSearch)
        {
            _isCanSearch = isCanSearch;
            _moreCommand.RaiseCanExecuteChanged();            
        }

        public SelectParams BuildSelectParams()
        {
            SelectParams result = new SelectParams()
            {
                IsEnablePaging = true,
                CountOfRecordsOnPage = _pageSize,
                CurrentPage = _pageCurrent,
            };

            if (_searchFieldViewModel != null)
            {
                InventProductSimpleFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventProductSimpleFilterData;
                if (filterData != null)
                    filterData.ApplyToSelectParams(result);
            }

            TotalStringTooltip = result.ToString();

            return result;
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
                InventProductSimpleFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventProductSimpleFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            query.Add(Common.NavigationSettings.NoNeedToBuildAnalyzeTable, String.Empty);

            UtilsNavigate.InventProductListSimpleOpen(this._regionManager, query);
        }

        private bool MoreCommandCanExecute()
        {
            return _isCanSearch;
        }
    }
}