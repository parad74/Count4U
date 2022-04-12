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
using Count4U.Model.Audit;
using Count4U.Model;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductListSimpleViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUserSettingsManager _userSettingsManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
        private readonly UICommandRepository _commandRepository;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;

        private readonly DelegateCommand _reportCommand;
        private readonly DelegateCommand _searchCommand;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly ObservableCollection<InventProductAdvancedItemSimpleViewModel> _items;
        private InventProductAdvancedItemSimpleViewModel _selectedItem;

        private InventProductSimpleFilterData _filter;
        private readonly ReportButtonViewModel _reportButtonViewModel;

        private bool _isBusy;

        private bool _isTableBuild;

        public InventProductListSimpleViewModel(
            IContextCBIRepository contextCbiRepository,
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
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
			this._eventAggregator = eventAggregator;
			this._regionManager = regionManager;
			this._commandRepository = commandRepository;
			this._reportButtonViewModel = reportButtonViewModel;
			this._iturAnalyzesRepository = iturAnalyzesRepository;
			this._navigationRepository = navigationRepository;
			this._userSettingsManager = userSettingsManager;
			this._items = new ObservableCollection<InventProductAdvancedItemSimpleViewModel>();

			this._reportCommand = this._commandRepository.Build(enUICommand.Report, ReportCommandExecuted);
			this._searchCommand = this._commandRepository.Build(enUICommand.Search, delegate { });
        }

        public InventProductSimpleFilterData Filter
        {
			get { return this._filter; }
        }

        public bool IsFilterAnyField
        {
			get { return this._filter == null ? false : this._filter.IsAnyField(); }
        }

        public int PageSize
        {
			get { return this._pageSize; }
            set
            {
				this._pageSize = value;
				RaisePropertyChanged(() => this.PageSize);
            }
        }

        public int PageCurrent
        {
			get { return this._pageCurrent; }
            set
            {
				this._pageCurrent = value;
				RaisePropertyChanged(() => this.PageCurrent);

                using (new CursorWait())
                {
					this.BuildList();
                }
            }
        }

        public int ItemsTotal
        {
			get { return this._itemsTotal; }
            set
            {
				this._itemsTotal = value;
				RaisePropertyChanged(() => this.ItemsTotal);
            }
        }

        public ObservableCollection<InventProductAdvancedItemSimpleViewModel> Items
        {
			get { return this._items; }
        }

        public InventProductAdvancedItemSimpleViewModel SelectedItem
        {
			get { return this._selectedItem; }
            set
            {
				this._selectedItem = value;
				RaisePropertyChanged(() => this.SelectedItem);
            }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
			get { return this._reportButtonViewModel; }
        }

        public DelegateCommand ReportCommand
        {
			get { return this._reportCommand; }
        }

        public DelegateCommand SearchCommand
        {
			get { return this._searchCommand; }
        }

        public bool IsBusy
        {
			get { return this._isBusy; }
            set
            {
				this._isBusy = value;
                RaisePropertyChanged(() => IsBusy);

				this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(this._isBusy);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(FilterData);

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionInventProductsGet();

            _filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, false) as InventProductSimpleFilterData;
			if (this._filter == null)
				this._filter = new InventProductSimpleFilterData();

            this.ReportButtonViewModel.OnNavigatedTo(navigationContext);
            this.ReportButtonViewModel.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = BuildSelectParams();

				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.InventProductAdvancedSearch);

            bool noNeedBuildAnalyzeTable = navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.NoNeedToBuildAnalyzeTable);
            if (GlobalState.BACK)
            {
                noNeedBuildAnalyzeTable = false;
            }

            if (_filter.IsAnyField() == false)
            {
				this._iturAnalyzesSourceRepository.ClearIturAnalyzes(base.GetDbPath);
                BuildList();
				this._isTableBuild = false;
            }
            else
            {
                if (noNeedBuildAnalyzeTable)
                {
					this._isTableBuild = true;
                    BuildList();
                }
                else
                {
                    BuildAnalyzeTableAndList();
					this._isTableBuild = true;
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
				Utils.RunOnUI(() => this._items.Clear());
                SelectParams sp = BuildSelectParams();

			

                IturAnalyzesCollection collection = this._iturAnalyzesRepository.GetIturAnalyzesCollection(sp, base.GetDbPath, false);

                List<InventProductAdvancedItemSimpleViewModel> toAdd = new List<InventProductAdvancedItemSimpleViewModel>();
                foreach (IturAnalyzes analyze in collection)
                {
                    InventProductAdvancedItemSimpleViewModel viewModel = new InventProductAdvancedItemSimpleViewModel(analyze);
                    toAdd.Add(viewModel);
                }

                Utils.RunOnUI(() =>
                {
					toAdd.ForEach(r => this._items.Add(r));
                    ItemsTotal = (int)collection.TotalCount;
                    if ((collection.TotalCount > 0) && (collection.Count == 0)) //do not show empty space - move on previous page           
                    {
						PageCurrent = this._pageCurrent - 1;
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

			if (this._filter != null)
            {
				this._filter.ApplyToSelectParams(result);
            }

            return result;
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.InventProductAdvancedSearch);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            SelectParams sp = BuildSelectParams();

            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void FilterData(IFilterData filterData)
        {
            this._filter = filterData as InventProductSimpleFilterData;

			RaisePropertyChanged(() => this.IsFilterAnyField);

			if (this._isTableBuild)
            {
				this.BuildList();
            }
            else
            {
				this.BuildAnalyzeTableAndList();
				this._isTableBuild = true;
            }
        }

        private void BuildAnalyzeTableAndList()
        {
			AuditConfig auditConfig = base._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
			Inventor currentInventor = base._contextCBIRepository.GetCurrentInventor(auditConfig);

			this.IsBusy = true;
            Task.Factory.StartNew(() =>
            {
				InventProductUtils.BuildAnalyzeTableSimple(this._iturAnalyzesSourceRepository, this._iturAnalyzesRepository, new CancellationTokenSource(), base.GetDbPath, currentInventor);
				this.BuildList();
				Utils.RunOnUI(() => this.IsBusy = false);
			}).LogTaskFactoryExceptions("BuildAnalyzeTableAndList");
        }
    }
}