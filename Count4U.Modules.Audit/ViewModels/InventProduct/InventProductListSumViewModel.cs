using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.State;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl.Grid;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductListSumViewModel : CBIContextBaseViewModel
    {
        private const ViewDomainContextEnum ReportContext = ViewDomainContextEnum.InventProductSumAdvancedSearch;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
        private readonly ReportButtonViewModel _reportButtonViewModel;
        private readonly UICommandRepository _commandRepository;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;

        private readonly DelegateCommand _reportCommand;
        private readonly DelegateCommand _searchCommand;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly ObservableCollection<InventProductAdvancedItemSumViewModel> _items;
        private InventProductAdvancedItemSumViewModel _selectedItem;

        private InventProductSumFilterData _filter;

        private bool _isBusy;

        private bool _isTableBuild;

        public InventProductListSumViewModel(IContextCBIRepository contextCbiRepository,
            IUserSettingsManager userSettingsManager,
            INavigationRepository navigationRepository,
            IIturAnalyzesRepository iturAnalyzesRepository,
            ReportButtonViewModel reportButtonViewModel,
            UICommandRepository commandRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IIturAnalyzesSourceRepository iturAnalyzesSourceRepository)
            : base(contextCbiRepository)
        {
            _iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _commandRepository = commandRepository;
            _reportButtonViewModel = reportButtonViewModel;
            _iturAnalyzesRepository = iturAnalyzesRepository;
            _navigationRepository = navigationRepository;
            _userSettingsManager = userSettingsManager;

            _items = new ObservableCollection<InventProductAdvancedItemSumViewModel>();

            _reportCommand = _commandRepository.Build(enUICommand.Report, ReportCommandExecuted);
            _searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });
        }

        public InventProductSumFilterData Filter
        {
            get { return _filter; }
        }

        public bool IsFilterAnyField
        {
            get { return _filter == null ? false : _filter.IsAnyField(); }
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
                    BuildList();
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
            }
        }

        public ObservableCollection<InventProductAdvancedItemSumViewModel> Items
        {
            get { return _items; }
        }

        public InventProductAdvancedItemSumViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
            get { return _reportButtonViewModel; }
        }

        public DelegateCommand ReportCommand
        {
            get { return _reportCommand; }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                _eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(_isBusy);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(FilterData);

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionInventProductsGet();

            _filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, false) as InventProductSumFilterData;
            if (_filter == null)
                _filter = new InventProductSumFilterData();

            this.ReportButtonViewModel.OnNavigatedTo(navigationContext);
            this.ReportButtonViewModel.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = BuildSelectParams();

				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ReportContext);

            bool noNeedBuildAnalyzeTable = navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.NoNeedToBuildAnalyzeTable);
            if (GlobalState.BACK)
            {
                noNeedBuildAnalyzeTable = false;
            }

            if (_filter.IsAnyField() == false)
            {
                _iturAnalyzesSourceRepository.ClearIturAnalyzes(base.GetDbPath);
                BuildList();
                _isTableBuild = false;
            }
            else
            {
                if (noNeedBuildAnalyzeTable)
                {
                    _isTableBuild = true;
                    BuildList();
                }
                else
                {
                    BuildAnalyzeTableAndList();
                    _isTableBuild = true;
                }
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(FilterData);
        }      

        private void BuildList()
        {
            try
            {
                Utils.RunOnUI(() => _items.Clear());
                SelectParams sp = BuildSelectParams();
                IturAnalyzesCollection collection = this._iturAnalyzesRepository.GetIturAnalyzesSumCollection(sp, base.GetDbPath, false);

                List<InventProductAdvancedItemSumViewModel> toAdd = new List<InventProductAdvancedItemSumViewModel>();
                foreach (IturAnalyzes analyze in collection)
                {
                    InventProductAdvancedItemSumViewModel viewModel = new InventProductAdvancedItemSumViewModel(analyze);
                    toAdd.Add(viewModel);
                }

                Utils.RunOnUI(() =>
                {
                    toAdd.ForEach(r => _items.Add(r));
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

        private SelectParams BuildSelectParams()
        {
            SelectParams result = new SelectParams();

            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = this._pageSize;
            result.CurrentPage = this._pageCurrent;

            if (_filter != null)
            {
                _filter.ApplyToSelectParams(result);
            }

            return result;
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ReportContext);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            SelectParams sp = BuildSelectParams();

            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void FilterData(IFilterData filterData)
        {
            this._filter = filterData as InventProductSumFilterData;

            RaisePropertyChanged(() => IsFilterAnyField);

            if (_isTableBuild)
            {
                BuildList();
            }
            else
            {
                BuildAnalyzeTableAndList();
                _isTableBuild = true;
            }
        }

        private void BuildAnalyzeTableAndList()
        {
			AuditConfig auditConfig = base._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
			Inventor currentInventor = base._contextCBIRepository.GetCurrentInventor(auditConfig);
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
				InventProductUtils.BuildAnalyzeTableSum(_iturAnalyzesSourceRepository, _iturAnalyzesRepository, new CancellationTokenSource(), base.GetDbPath, currentInventor);
                BuildList();
                Utils.RunOnUI(() => IsBusy = false);
			}).LogTaskFactoryExceptions("BuildAnalyzeTableAndList");
        }
    }
}