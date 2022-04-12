using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.SearchFilter;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.Events;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.Constants;

namespace Count4U.Modules.Audit.ViewModels
{
    public class LocationAddEditDeleteViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceLocator _serviceLocator;

        private readonly IEventAggregator _eventAggregator;
        private readonly ILocationRepository _locationRepository;
        private readonly IRegionManager _regionManager;
        private readonly ReportButtonViewModel _reportButtonViewModel;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly UICommandRepository _commandRepository;
        private readonly INavigationRepository _navigationRepository;

        private readonly UICommand _addCommand;
		private readonly UICommand _multiAddCommand;
        private readonly UICommand _editCommand;
        private readonly UICommand _deleteCommand;
        private readonly UICommand _importCommand;
        private readonly UICommand _reportCommand;
        private readonly UICommand _repairCommand;
		private readonly DelegateCommand _changeLocationTagCommand;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly ObservableCollection<LocationItemViewModel> _items;
        private List<LocationItemViewModel> _selectedItems;

        private SearchFilterViewModel _searchFilterViewModel;

        public LocationAddEditDeleteViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            ILocationRepository locationRepository,
            IRegionManager regionManager,
            ReportButtonViewModel reportButtonViewModel,
            IUserSettingsManager _userSettingsManager,
            UICommandRepository uiCommandRepository,
            INavigationRepository navigationRepository
            ) :
            base(contextCBIRepository)
        {
            _navigationRepository = navigationRepository;
            this._commandRepository = uiCommandRepository;
            this._userSettingsManager = _userSettingsManager;
            this._reportButtonViewModel = reportButtonViewModel;
            this._regionManager = regionManager;
            this._locationRepository = locationRepository;
            this._eventAggregator = eventAggregator;
            this._serviceLocator = serviceLocator;

            this._addCommand = _commandRepository.Build(enUICommand.Add, this.AddCommandExecuted);
			this._multiAddCommand = _commandRepository.Build(enUICommand.MultiAdd, this.MultiAddCommandExecuted);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, this.DeleteCommandExecuted, this.DeleteCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, this.EditCommandExecuted, this.EditCommandCanExecute);
            this._importCommand = _commandRepository.Build(enUICommand.Import, this.ImportCommandExecuted);
            this._reportCommand = _commandRepository.Build(enUICommand.Report, ReportCommandExecuted);
            this._repairCommand = _commandRepository.Build(enUICommand.RepairFromDb, RepairCommandExecuted);
			this._changeLocationTagCommand = _commandRepository.Build(enUICommand.ChangeLocationTag, this.ChangeLocationTagCommandExecuted, this.ChangeLocationTagCommandCanExecute);

            this._eventAggregator.GetEvent<LocationAddedEvent>().Subscribe(this.LocationAdded);
            this._eventAggregator.GetEvent<LocationEditedEvent>().Subscribe(this.LocationEditedEvent);


            this._items = new ObservableCollection<LocationItemViewModel>();
        }

        public string SearchFilterRegionKey { get; set; }
       
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

                Build();
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

		public DelegateCommand MultiAddCommand
		{
			get { return this._multiAddCommand; }
		}

        public DelegateCommand DeleteCommand
        {
            get { return this._deleteCommand; }
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        public DelegateCommand ReportCommand
        {
            get { return this._reportCommand; }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
            get { return _reportButtonViewModel; }
        }

        public DelegateCommand RepairCommand
        {
            get { return _repairCommand; }
        }

        public ObservableCollection<LocationItemViewModel> Items
        {
            get { return _items; }
        }

		public DelegateCommand ChangeLocationTagCommand
		{
			get { return this._changeLocationTagCommand; }
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._eventAggregator.GetEvent<LocationTagChangedEvent>().Subscribe(this.LocationTagChanged);

            InitSearchFilter(navigationContext);
            InitReportButton(navigationContext);

            this._pageSize = 50;
            this._pageCurrent = 1;

            Build();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<LocationAddedEvent>().Unsubscribe(LocationAdded);
			this._eventAggregator.GetEvent<LocationTagChangedEvent>().Unsubscribe(this.LocationTagChanged);
			this._eventAggregator.GetEvent<LocationEditedEvent>().Unsubscribe(this.LocationEditedEvent);

			this._reportButtonViewModel.OnNavigatedFrom(navigationContext);
        }

        private void InitReportButton(NavigationContext navigationContext)
        {
            this._reportButtonViewModel.OnNavigatedTo(navigationContext);
            this._reportButtonViewModel.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = BuildSelectParams();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Location);
        }

        private void InitSearchFilter(NavigationContext navigationContext)
        {
            _searchFilterViewModel = Utils.GetViewModelFromRegion<SearchFilterViewModel>(Common.RegionNames.LocationAddEditDeleteSearchFilter + SearchFilterRegionKey, this._regionManager);

            _searchFilterViewModel.FilterAction = Build;

            _searchFilterViewModel.PopupExtSearch.NavigationData = new LocationFilterData();
            _searchFilterViewModel.PopupExtSearch.Region = Common.RegionNames.PopupSearchLocationAddEditDelete;
            _searchFilterViewModel.PopupExtSearch.ViewModel = this;
            _searchFilterViewModel.PopupExtSearch.Init();

            _searchFilterViewModel.PopupExtFilter.Region = Common.RegionNames.PopupFilterLocationAddEditDelete;
            _searchFilterViewModel.PopupExtFilter.ViewModel = this;
            _searchFilterViewModel.PopupExtFilter.View = Common.ViewNames.FilterView;
            _searchFilterViewModel.PopupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, _searchFilterViewModel.Filter, Common.NavigationObjects.Filter);
            _searchFilterViewModel.PopupExtFilter.Init();

            _searchFilterViewModel.Filter = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, true) as LocationFilterData;
            if (_searchFilterViewModel.Filter == null)
                _searchFilterViewModel.Filter = new LocationFilterData();
        }


        private void Build()
        {
            _items.Clear();

            SelectParams selectParams = BuildSelectParams();

            try
            {
                Locations locations = this._locationRepository.GetLocations(selectParams, base.GetDbPath);

                foreach (Location location in locations)
                {
                    _items.Add(new LocationItemViewModel(location));
                }

                this.ItemsTotal = (int)locations.TotalCount;

                if ((locations.TotalCount > 0)
                    && (this.Items.Count == 0))	//do not show empty space - move on previous page
                {
                    this.PageCurrent = this._pageCurrent - 1;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                _logger.Error("SelectParams: {0}", selectParams.ToString());
                throw;
            }

        }

		private void LocationTagChanged(LocationTagChangedEventPayload payload)
		{
			//Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;
			//Iturs iturs = new Iturs();
			//foreach (Itur itur in payload.Iturs)
			//{
			//	itur.LocationCode = payload.Location.Code;
			//	itur.Location = payload.Location.Name;
			//	iturs.Add(itur);
			//}

			//this._iturRepository.Update(iturs, base.GetDbPath);

			//foreach (Itur itur in payload.Iturs)
			//{
			//	IturItemViewModel itemViewModel = this._items.FirstOrDefault(r => r.Itur.IturCode == itur.IturCode);
			//	if (itemViewModel != null)
			//	{
			//		itemViewModel.UpdateViewModel(itur, payload.Location, statuses.Values.FirstOrDefault(r => r.Bit == itur.StatusIturBit));
			//	}
			//}

			Locations locations = new Locations();
			foreach (Location location in payload.Locations)
			{
				location.Tag = payload.Tag;
				//location.Location = payload.Location.Name;
				locations.Add(location);
			}
			this._locationRepository.Update(locations, base.GetDbPath);
		}

        public void SelectedItemsSet(List<LocationItemViewModel> items)
        {
            this._selectedItems = items;

            this._editCommand.RaiseCanExecuteChanged();
            this._deleteCommand.RaiseCanExecuteChanged();
			this._changeLocationTagCommand.RaiseCanExecuteChanged();
        }

		private void ChangeLocationTagCommandExecuted()
		{
			Locations locations = new Locations();
			if (this._selectedItems != null)
			{
				locations = Locations.FromEnumerable(this._selectedItems.Select(r => r.Location));
			}

			LocationTagChangeEventPayload payload = new LocationTagChangeEventPayload();
			payload.Locations = locations;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<LocationTagChangeEvent>().Publish(payload);
			Build();
		}

		private bool ChangeLocationTagCommandCanExecute()
		{
			//return true;
			return this._selectedItems != null && this._selectedItems.Count > 0;
		}

        private void AddCommandExecuted()
        {
            LocationAddedEventPayLoad payload = new LocationAddedEventPayLoad();  //!! need
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            this._eventAggregator.GetEvent<LocationAddEvent>().Publish(payload);
			
        }

		private void MultiAddCommandExecuted()
		{
			LocationMultiAddedEventPayLoad payload = new LocationMultiAddedEventPayLoad();  //!! need
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;
			payload.CountAdd = 500;

			this._eventAggregator.GetEvent<LocationMultiAddEvent>().Publish(payload);
		}

        private void LocationAdded(Location location)
        {
            Build();
        }

        private SelectParams BuildSelectParams()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Name";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = this._pageSize;
            result.CurrentPage = this._pageCurrent;

            LocationFilterData locationFilter = _searchFilterViewModel.Filter as LocationFilterData;
            if (locationFilter != null)
            {
                locationFilter.ApplyToSelectParams(result);
            }

            return result;
        }

        private bool DeleteCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }

        private void DeleteCommandExecuted()
        {
            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Title = String.Empty;
            notification.Settings = this._userSettingsManager;
            string locationNames = this._selectedItems.Select(r => r.Name).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z));
            locationNames = locationNames.Remove(locationNames.Length - 1, 1);
            string message = String.Format(Localization.Resources.Msg_Delete_Location, locationNames);

            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Warning, _userSettingsManager);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (LocationItemViewModel location in this._selectedItems)
                {
                    this._locationRepository.Delete(location.Location, base.GetDbPath);
                }

                Build();
            }
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeLocation);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private bool EditCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count == 1;
        }

        private void EditCommandExecuted()
        {
            Location location = this._selectedItems.First().Location;
            this._eventAggregator.GetEvent<LocationAddEvent>().Publish(new LocationAddedEventPayLoad()
                                                                           {
                                                                               AddUnknownLocation = false,
                                                                               Location = location,
                                                                               Context = base.Context,
                                                                               DbContext = base.CBIDbContext,
                                                                           });
        }

        private void LocationEditedEvent(Location location)
        {
            Build();
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Location);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            var sp = this.BuildSelectParams();
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
				this._locationRepository.RepairCodeFromDB(base.GetDbPath);
				//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
				//List<string> locationCodeListFromItur = iturRepository.GetLocationCodeList(base.GetDbPath);			//из
				//List<string> locationCodeListFromLocation = this._locationRepository.GetLocationCodeList(base.GetDbPath); //в
				//Dictionary<string, string> difference = new Dictionary<string, string>();

				//foreach (var locationCodeFromItur in locationCodeListFromItur)			   //из
				//{
				//    if (locationCodeListFromLocation.Contains(locationCodeFromItur) == false)		 //в
				//    {
				//        difference[locationCodeFromItur] = locationCodeFromItur;
				//    }
				//}

				//foreach (KeyValuePair<string, string> keyValuePair in difference)
				//{
				//    if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
				//    {
				//        Location locationNew = new Location();
				//        locationNew.Code = keyValuePair.Value;
				//        locationNew.Name = keyValuePair.Value; 
				//        locationNew.RestoreBit = true;
				//        locationNew.Description = "Repair from Itur"; 
				//        locationNew.Restore = DateTime.Now.ToString();
				//        this._locationRepository.Insert(locationNew, base.GetDbPath);
				//    }
				//}
                UtilsMisc.ShowMessageBox(Localization.Resources.Msg_RestoreDone, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
                Build();
            }
        }        
    }
}