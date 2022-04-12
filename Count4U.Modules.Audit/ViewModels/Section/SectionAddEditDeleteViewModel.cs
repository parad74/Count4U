using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.SearchFilter;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.Extensions;
using System.Windows.Threading;
using Count4U.Modules.Audit.Events;

namespace Count4U.Modules.Audit.ViewModels.Section
{																		  // см CatalogFormViewModel
    public class SectionAddEditDeleteViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly ISectionRepository _sectionRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly UICommandRepository _commandRepository;
        private readonly IServiceLocator _serviceLocator;
        private readonly INavigationRepository _navigationRepository;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _deleteAllCommand;
		private readonly DelegateCommand _changeSectionTagCommand;
        private readonly UICommand _reportCommand;
        private readonly UICommand _repairCommand;

        private readonly ReportButtonViewModel _reportButtonViewModel;

        private readonly ObservableCollection<SectionItemViewModel> _items;
        private SectionItemViewModel _selectedItem;
		private List<SectionItemViewModel> _selectedItems;

		private SectionItemViewModel _detailSelectedItem;
		private readonly ObservableCollection<SectionItemViewModel> _detailItems;

		private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

		private readonly DispatcherTimer _timer;
        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private SearchFilterViewModel _searchFilterViewModel;

        public SectionAddEditDeleteViewModel(IContextCBIRepository contextCBIRepository,
                                             IEventAggregator eventAggregator,
                                             ISectionRepository sectionRepository,
                                             IRegionManager regionManager,
                                             IUserSettingsManager _userSettingsManager,
                                             ModalWindowLauncher modalWindowLauncher,
                                             UICommandRepository commandRepository,
                                             ReportButtonViewModel reportButtonViewModel,
                                             IServiceLocator serviceLocator,
                                             INavigationRepository navigationRepository
            ) :
            base(contextCBIRepository)
        {
            this._navigationRepository = navigationRepository;
            this._serviceLocator = serviceLocator;
            this._reportButtonViewModel = reportButtonViewModel;
            this._commandRepository = commandRepository;
            this._modalWindowLauncher = modalWindowLauncher;
            this._userSettingsManager = _userSettingsManager;
            this._regionManager = regionManager;
            this._sectionRepository = sectionRepository;
            this._eventAggregator = eventAggregator;
            this._items = new ObservableCollection<SectionItemViewModel>();
			this._detailItems = new ObservableCollection<SectionItemViewModel>();
			this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._addCommand = _commandRepository.Build(enUICommand.Add, this.AddCommandExecuted);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, this.DeleteCommandExecuted, this.DeleteCommandCanExecute);
            this._deleteAllCommand = _commandRepository.Build(enUICommand.DeleteAll, DeleteAllCommandExecuted, DeleteAllCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, this.EditCommandExecuted, this.EditCommandCanExecute);
            this._importCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted);
            this._reportCommand = _commandRepository.Build(enUICommand.Report, ReportCommandExecuted);
            this._repairCommand = _commandRepository.Build(enUICommand.RepairFromDb, RepairCommandExecuted);
			this._changeSectionTagCommand = _commandRepository.Build(enUICommand.ChangeLocationTag, this.ChangeSectionTagCommandExecuted, this.ChangeSectionTagCommandCanExecute);


			this._timer = new DispatcherTimer();
			this._timer.Interval = TimeSpan.FromMilliseconds(this._userSettingsManager.DelayGet());
			this._timer.Tick += Timer_Tick;

        }


		//public void SelectedItemsSet(SectionItemViewModel item)
		//this._selectedItem = item;
		public void SelectedItemsSet(List<SectionItemViewModel> list)		
		{

			this._selectedItems = list;
			if (this._selectedItems != null)
			{
				if (this._selectedItems.Count() > 0) this.SelectedItem = this._selectedItems.First();
			}

			this._deleteCommand.RaiseCanExecuteChanged();
			this._editCommand.RaiseCanExecuteChanged();
			this._changeSectionTagCommand.RaiseCanExecuteChanged();
		
			this._timer.Stop();
			this._timer.Start();
		}

		private void BuildDetailsItems()
		{
			this._detailItems.Clear();
			if (this._selectedItems == null) return;
			if (this._selectedItems.Count() < 1) return;
			this.SelectedItem = this._selectedItems.First();

			if (this._selectedItem == null) return;
			if (String.IsNullOrEmpty(this._selectedItem.Code) == true) return;


			SectionItemViewModel viewModel = this._selectedItem;
			if (viewModel == null) return;

			Count4U.Model.Count4U.Section master = viewModel.Section;

			SelectParams selectParams = new SelectParams();
			try
			{
				selectParams.FilterParams.Add("ParentSectionCode", new FilterParam() { Operator = FilterOperator.Equal, Value = master.SectionCode });

				Count4U.Model.Count4U.Sections sections = this._sectionRepository.GetSections(selectParams, base.GetDbPath);
				foreach (var section in sections)
				{
					viewModel = new SectionItemViewModel(section);
					this._detailItems.Add(viewModel);
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("BuildDetailsItems", exc);
				_logger.Error("SelectParams: {0}", selectParams.ToString());
				throw;
			}
		}


		void Timer_Tick(object sender, EventArgs e)
		{
			using (new CursorWait())
			{
				this._timer.Stop();
				this.BuildDetailsItems();
			}
		}

        public string SearchFilterRegionKey { get; set; }

        public ObservableCollection<SectionItemViewModel> Items
        {
            get { return _items; }
        }

        public DelegateCommand ImportCommand
        {
            get { return _importCommand; }
        }

		public SectionItemViewModel DetailSelectedItem
		{
			get { return this._detailSelectedItem; }
			set { this._detailSelectedItem = value; }
		}

		public ObservableCollection<SectionItemViewModel> DetailItems
		{
			get { return this._detailItems; }
		}

        public SectionItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

				this._deleteCommand.RaiseCanExecuteChanged();
				this._editCommand.RaiseCanExecuteChanged();
            }
        }

        public int PageSize
        {
            get { return this._pageSize; }
            set
            {
                this._pageSize = value;
                this.RaisePropertyChanged(() => this.PageSize);
            }
        }

        public int PageCurrent
        {
            get { return this._pageCurrent; }
            set
            {
                this._pageCurrent = value;
                this.RaisePropertyChanged(() => this.PageCurrent);

				Task.Factory.StartNew(Build).LogTaskFactoryExceptions("PageCurrent");
            }
        }

        public int ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => this.ItemsTotal);
            }
        }

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return this._deleteCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        public DelegateCommand ReportCommand
        {
            get { return this._reportCommand; }
        }

        public DelegateCommand RepairCommand
        {
            get { return _repairCommand; }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
            get { return _reportButtonViewModel; }
        }

        public DelegateCommand DeleteAllCommand
        {
            get { return _deleteAllCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._eventAggregator.GetEvent<SectionTagChangedEvent>().Subscribe(this.SectionTagChanged);

            InitSearchFilter(navigationContext);
            InitReportButton(navigationContext);

            this._pageSize = this._userSettingsManager.PortionSectionsGet();
            this._pageCurrent = 1;

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
			this._eventAggregator.GetEvent<SectionTagChangedEvent>().Unsubscribe(this.SectionTagChanged);

            this._reportButtonViewModel.OnNavigatedFrom(navigationContext);
        }

        private void InitReportButton(NavigationContext navigationContext)
        {
            this.ReportButtonViewModel.OnNavigatedTo(navigationContext);
            this.ReportButtonViewModel.Initialize(this.ReportCommandExecuted, () =>
            {
                return new Tuple<SelectParams, Itur, Location, DocumentHeader,Device>(null, null, null, null, null);
            }, ViewDomainContextEnum.Section);
																												 
        }

        private void InitSearchFilter(NavigationContext navigationContext)
        {
            _searchFilterViewModel = Utils.GetViewModelFromRegion<SearchFilterViewModel>(Common.RegionNames.SectionAddEditDeleteSearchFilter + SearchFilterRegionKey, this._regionManager);

            _searchFilterViewModel.FilterAction = Build;

            _searchFilterViewModel.PopupExtSearch.NavigationData = new SectionFilterData();
            _searchFilterViewModel.PopupExtSearch.Region = Common.RegionNames.PopupSearchSectionAddEditDelete;
            _searchFilterViewModel.PopupExtSearch.ViewModel = this;
            _searchFilterViewModel.PopupExtSearch.Init();

            _searchFilterViewModel.PopupExtFilter.Region = Common.RegionNames.PopupFilterSectionAddEditDelete;
            _searchFilterViewModel.PopupExtFilter.ViewModel = this;
            _searchFilterViewModel.PopupExtFilter.View = Common.ViewNames.FilterView;
            _searchFilterViewModel.PopupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, _searchFilterViewModel.Filter, Common.NavigationObjects.Filter);
            _searchFilterViewModel.PopupExtFilter.Init();

            _searchFilterViewModel.Filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, true) as SectionFilterData;
            if (_searchFilterViewModel.Filter == null)
                _searchFilterViewModel.Filter = new SectionFilterData();
        }


        private SelectParams BuildMasterSelectParams()
        {
            SelectParams selectParams = new SelectParams();

            selectParams.IsEnablePaging = true;
            selectParams.CountOfRecordsOnPage = this._pageSize;
            selectParams.CurrentPage = this._pageCurrent;
		//	selectParams.FilterParams.Add("TypeCode", new FilterParam() { Operator = FilterOperator.Equal, Value = TypeSectionEnum.S.ToString() });

			List<string> typeCodes = new List<string>() { TypeSectionEnum.S.ToString() , ""};
			selectParams.FilterStringListParams.Add("TypeCode", new FilterStringListParam() { Values = typeCodes });

            SectionFilterData sectionFilter = _searchFilterViewModel.Filter as SectionFilterData;
            if (sectionFilter != null)
            {
                sectionFilter.ApplyToSelectParams(selectParams);
            }

            return selectParams;
        }

        private void Build()
        {
            SelectParams selectParams = null;
            try
            {
                selectParams = BuildMasterSelectParams();
	
                var sections = this._sectionRepository.GetSections(selectParams, base.GetDbPath);

                Utils.RunOnUI(() =>
                                  {
                                      this._items.Clear();

                                      this.ItemsTotal = (int)sections.TotalCount;

                                      foreach (Model.Count4U.Section section in sections)
                                      {
                                          SectionItemViewModel viewModel = new SectionItemViewModel(section);
                                          this._items.Add(viewModel);
                                      }

                                      if ((sections.TotalCount > 0) && (this._items.Count == 0)) //do not show empty space - move on previous page                   
                                      {
                                          this.PageCurrent = this._pageCurrent - 1;
                                      }

                                      _deleteAllCommand.RaiseCanExecuteChanged();
                                  });
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                _logger.Error("SelectParams: {0}", selectParams);
                throw;
            }
        }


		public DelegateCommand ChangeSectionTagCommand
		{
			get { return this._changeSectionTagCommand; }

		}
	

		private void ChangeSectionTagCommandExecuted()
		{
			Sections sections = new Sections();
			if (this._selectedItems != null)
			{
				if (this._selectedItems.Count() > 0)  sections = Sections.FromEnumerable(this._selectedItems.Select(r => r.Section));
			}

			SectionTagChangeEventPayload payload = new SectionTagChangeEventPayload();
			payload.Sections = sections;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<SectionTagChangeEvent>().Publish(payload);
			Build();
		}

		private bool ChangeSectionTagCommandCanExecute()
		{
			//return true;
			return this._selectedItems != null && this._selectedItems.Count > 0;
		}


		private void SectionTagChanged(SectionTagChangedEventPayload payload)
		{
			Sections sections = new Sections();
			foreach (Count4U.Model.Count4U.Section section in payload.Sections)
			{
				section.Tag = payload.Tag;
				//location.Location = payload.Location.Name;
				sections.Add(section);
			}
			this._sectionRepository.Update(sections, base.GetDbPath);
		}

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeSection);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void AddCommandExecuted()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, base.Context);
            Utils.AddDbContextToDictionary(settings, base.CBIDbContext);

            object result = this._modalWindowLauncher.StartModalWindow(
                Common.ViewNames.SectionAddEditView,
                Common.Constants.WindowTitles.SectionAdd,
                360, 360,
                ResizeMode.NoResize,
                settings,
                Application.Current.MainWindow,
                minWidth: 220, minHeight: 220);

            if (result != null)
            {
                Build();
            }
        }

        private bool EditCommandCanExecute()
        {
			return this._selectedItems != null && this._selectedItems.Count == 1;
            //return this._selectedItem != null && !String.IsNullOrEmpty(_selectedItem.Code);
        }

        private void EditCommandExecuted()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, base.Context);
            Utils.AddDbContextToDictionary(settings, base.CBIDbContext);
			if (_selectedItems == null) return;
			if (this._selectedItems.Count() == 0) return;
			this._selectedItem = this._selectedItems.First();
  
            settings.Add(Common.NavigationSettings.SectionCode, this._selectedItem.Code);

            object result = this._modalWindowLauncher.StartModalWindow(
                Common.ViewNames.SectionAddEditView,
                Common.Constants.WindowTitles.SectionEdit,
                360, 360,
                ResizeMode.NoResize,
                settings,
                Application.Current.MainWindow,
                minWidth: 220, minHeight: 220);

            if (result != null)
            {
                Model.Count4U.Section section = result as Model.Count4U.Section;
                if (section != null)
                {
                    SectionItemViewModel viewodel = _items.FirstOrDefault(r => r.Code == section.SectionCode);
                    if (viewodel != null)
                    {
                        viewodel.Update(section);
                    }
                }
            }
        }

        private bool DeleteCommandCanExecute()
        {
            //return this._selectedItem != null && !String.IsNullOrEmpty(_selectedItem.Code);
			return this._selectedItems != null && this._selectedItems.Count == 1;
        }

        private void DeleteCommandExecuted()
        {
			if (_selectedItems == null) return;
			if (this._selectedItems.Count() == 0) return;
			this._selectedItem = this._selectedItems.First();

            if (_selectedItem == null) return;

            string message = String.Format(Localization.Resources.Msg_Delete_Section, _selectedItem.Name);

            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Warning, _userSettingsManager);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    using (new CursorWait())
                    {
                        Count4U.Model.Count4U.Section section = _sectionRepository.GetSectionByCode(_selectedItem.Code, base.GetDbPath);
                        if (section != null)
                            _sectionRepository.Delete(section, base.GetDbPath);
                        Build();
                    }
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("DeleteCommandExecuted", exc);
                }
            }
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Section);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            var sp = this.BuildMasterSelectParams();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void RepairCommandExecuted()
        {
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_Repair, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            using (new CursorWait())
            {
                this._sectionRepository.RepairCodeFromDB(base.GetDbPath);
                //IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
                //List<string> sectionCodeListFromProduct = productRepository.GetSectionCodeList(base.GetDbPath);			//из
                //List<string> sectionCodeListFromSection = this._sectionRepository.GetSectionCodeList(base.GetDbPath); //в
                //Dictionary<string, string> difference = new Dictionary<string, string>();

                //foreach (var sectionCodeFromProduct in sectionCodeListFromProduct)			   //из
                //{
                //    if (sectionCodeListFromSection.Contains(sectionCodeFromProduct) == false)		 //в
                //    {
                //        difference[sectionCodeFromProduct] = sectionCodeFromProduct;
                //    }
                //}

                //foreach (KeyValuePair<string, string> keyValuePair in difference)
                //{
                //    if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
                //    {
                //        Count4U.Model.Count4U.Section sectionNew = new Count4U.Model.Count4U.Section();
                //        sectionNew.SectionCode = keyValuePair.Value;
                //        sectionNew.Name = keyValuePair.Value;
                //        sectionNew.Description = "Repair from Product"; 
                //        this._sectionRepository.Insert(sectionNew, base.GetDbPath);
                //    }
                //}
                UtilsMisc.ShowMessageBox(Localization.Resources.Msg_RestoreDone, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
                Build();
            }
        }

        private bool DeleteAllCommandCanExecute()
        {
            return _items.Any();
        }

        private void DeleteAllCommandExecuted()
        {
            string message = Localization.Resources.ViewModel_SectionAddEditDelete_deleteAll;
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Information, _userSettingsManager);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (new CursorWait())
                {
                    try
                    {
                        IImportProvider provider = this._serviceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportSectionDefaultADOProvider.ToString());
                        provider.ToPathDB = base.GetDbPath;
                        provider.Clear();
                        Build();
                    }
                    catch (Exception exc)
                    {
                        _logger.ErrorException("DeleteAllCommandExecuted", exc);
                    }
                }
            }
        }
    }
}