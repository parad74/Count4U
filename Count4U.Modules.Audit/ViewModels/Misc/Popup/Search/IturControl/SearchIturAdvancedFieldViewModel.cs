using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Common.ViewModel.Misc;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.IturControl
{
    public class SearchIturAdvancedFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IStatusIturGroupRepository _statusGroupRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ILocationRepository _locationRepository;
        private readonly IIturRepository _iturRepository;
        private readonly IRegionManager _regionManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly LocationListViewModelBuilder _locationListViewModelBuilder;

        private readonly DelegateCommand _viewLocationCommand;
        private readonly DelegateCommand<LocationItemViewModel> _addUnknownLocationCommand;

        private string _iturFilterText;
        private readonly ObservableCollection<ItemFindViewModel> _iturFilterItems;
        private ItemFindViewModel _iturFilterSelected;

        private bool _isFilterByLocation;
        private bool _isFilterByStatus;
		private bool _isFilterByTag;
        private bool _isFilterByInventProduct;

        private string _filterMakat;
        private string _filterBarcode;
        private string _filterProductName;

        private readonly ObservableCollection<StatusGroupItemViewModel> _statusItems;
        private readonly ObservableCollection<LocationItemViewModel> _locationItems;
		private readonly ObservableCollection<TagItemViewModel> _tagItems;
		

        private bool _isLocationExpanded;
        private bool _isStatusExpanded;
		private bool _isTagExpanded;
        private bool _isInventProductExpanded;

        private IturAdvancedFilterData _filter;

        private DelegateCommand _searchCommand;

        private SortViewModel _sortViewModel;
		private bool _isCheckedLocations;
		private bool _isCheckedStatus;
		private bool _isCheckedTag;

		private string _findByLocationCode = String.Empty;
		private string _findByLocationTag = String.Empty;

        public SearchIturAdvancedFieldViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IStatusIturGroupRepository statusGroupRepository,
            IUserSettingsManager userSettingsManager,
            IIturRepository iturRepository,
            ILocationRepository locationRepository,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            LocationListViewModelBuilder locationListViewModelBuilder
            )
            : base(contextCbiRepository)
        {
            _locationListViewModelBuilder = locationListViewModelBuilder;
            _navigationRepository = navigationRepository;
            _regionManager = regionManager;
            _iturRepository = iturRepository;
            _locationRepository = locationRepository;
            _userSettingsManager = userSettingsManager;
            _eventAggregator = eventAggregator;
            _statusGroupRepository = statusGroupRepository;
            _statusItems = new ObservableCollection<StatusGroupItemViewModel>();
            _locationItems = new ObservableCollection<LocationItemViewModel>();
			_tagItems = new ObservableCollection<TagItemViewModel>();

            _iturFilterText = String.Empty;
            _filterMakat = String.Empty;
            _filterBarcode = String.Empty;
            _filterProductName = String.Empty;

            _iturFilterItems = new ObservableCollection<ItemFindViewModel>();
            _iturFilterItems.Add(new ItemFindViewModel() { Value = ComboValues.FindItur.FilterIturNumber, Text = Localization.Resources.Constant_Number });
            _iturFilterItems.Add(new ItemFindViewModel() { Value = ComboValues.FindItur.FilterIturERP, Text = Localization.Resources.Constant_ERP });
            _iturFilterSelected = this._iturFilterItems.FirstOrDefault();

            _viewLocationCommand = new DelegateCommand(ViewLocationCommandExecuted);
            _addUnknownLocationCommand = new DelegateCommand<LocationItemViewModel>(AddUnknownLocationCommandExecuted);

            _isLocationExpanded = true;
            _isStatusExpanded = true;
            _isInventProductExpanded = true;
			_isTagExpanded = true;

			this.IsCheckedStatus = false;
			this.IsCheckedLocations = false;
			this.IsCheckedTag = false;
        }

        public FrameworkElement View { get; set; }

        public string IturFilterText
        {
            get { return _iturFilterText; }
            set
            {
                _iturFilterText = value;
                RaisePropertyChanged(() => IturFilterText);
            }
        }

        public ObservableCollection<ItemFindViewModel> IturFilterItems
        {
            get { return _iturFilterItems; }
        }

		public bool IsCheckedLocations
		{
			get { return _isCheckedLocations; }
			set
			{
				_isCheckedLocations = value;
				this.LocationItems.ToList().ForEach(r => r.IsChecked = value);
				RaisePropertyChanged(() => IsCheckedLocations);

			}
		}

		public bool IsCheckedStatus
		{
			get { return _isCheckedStatus; }
			set
			{
				_isCheckedStatus = value;
				this.StatusItems.ToList().ForEach(r => r.IsChecked = value);
				RaisePropertyChanged(() => IsCheckedStatus);

			}
		}

		public bool IsCheckedTag
		{
			get { return _isCheckedTag; }
			set
			{
				_isCheckedTag = value;
				this.TagItems.ToList().ForEach(r => r.IsChecked = value);
				RaisePropertyChanged(() => IsCheckedTag);

			}
		}

        public ItemFindViewModel IturFilterSelected
        {
            get { return _iturFilterSelected; }
            set
            {
                _iturFilterSelected = value;
                RaisePropertyChanged(() => IturFilterSelected);
            }
        }


		//public string FindByLocationCode
		//{
		//	get { return _findByLocationCode; }
		//	set
		//	{
		//		if (_findByLocationCode != value)
		//		{
		//			_findByLocationCode = value;
		//			RaisePropertyChanged(() => FindByLocationCode);

		//			BuildLocation();
		//		}
		//	}
		//}

		//public string FindByLocationTag
		//{
		//	get { return _findByLocationTag; }
		//	set
		//	{
		//		if (_findByLocationTag != value)
		//		{
		//			_findByLocationTag = value;
		//			RaisePropertyChanged(() => FindByLocationTag);

		//			BuildLocation();
		//		}
		//	}
		//}

        public string FilterMakat
        {
            get { return _filterMakat; }
            set
            {
                if (_filterMakat != value)
                {
                    _filterMakat = value;
                    RaisePropertyChanged(() => FilterMakat);

                }
            }
        }

        public string FilterBarcode
        {
            get { return _filterBarcode; }
            set
            {
                if (_filterBarcode != value)
                {
                    _filterBarcode = value;
                    RaisePropertyChanged(() => FilterBarcode);

                }
            }
        }

        public string FilterProductName
        {
            get { return _filterProductName; }
            set
            {
                if (_filterProductName != value)
                {
                    _filterProductName = value;
                    RaisePropertyChanged(() => FilterProductName);

                }
            }
        }

        public ObservableCollection<StatusGroupItemViewModel> StatusItems
        {
            get { return _statusItems; }
        }

        public bool IsFilterByLocation
        {
            get { return _isFilterByLocation; }
            set
            {
                _isFilterByLocation = value;
                RaisePropertyChanged(() => IsFilterByLocation);
            }
        }


		public bool IsFilterByTag
        {
			get { return _isFilterByTag; }
            set
            {
				_isFilterByTag = value;
				RaisePropertyChanged(() => IsFilterByTag);
            }
        }

        public bool IsFilterByStatus
        {
            get { return _isFilterByStatus; }
            set
            {
                _isFilterByStatus = value;
                RaisePropertyChanged(() => IsFilterByStatus);
            }
        }

        public bool IsFilterByInventProduct
        {
            get { return _isFilterByInventProduct; }
            set
            {
                _isFilterByInventProduct = value;
                RaisePropertyChanged(() => IsFilterByInventProduct);
            }
        }

        public ObservableCollection<LocationItemViewModel> LocationItems
        {
            get { return _locationItems; }
        }


		 public ObservableCollection<TagItemViewModel> TagItems
        {
            get { return _tagItems; }
        }

        public DelegateCommand ViewLocationCommand
        {
            get { return _viewLocationCommand; }
        }

        public DelegateCommand<LocationItemViewModel> AddUnknownLocationCommand
        {
            get { return _addUnknownLocationCommand; }
        }

        public bool IsLocationExpanded
        {
            get { return _isLocationExpanded; }
            set
            {
                _isLocationExpanded = value;
                RaisePropertyChanged(() => IsLocationExpanded);
            }
        }

        public bool IsStatusExpanded
        {
            get { return _isStatusExpanded; }
            set
            {
                _isStatusExpanded = value;
                RaisePropertyChanged(() => IsStatusExpanded);
            }
        }


		public bool IsTagExpanded
        {
			get { return _isTagExpanded; }
            set
            {
				_isTagExpanded = value;
				RaisePropertyChanged(() => IsTagExpanded);
            }
        }

        public bool IsInventProductExpanded
        {
            get { return _isInventProductExpanded; }
            set
            {
                _isInventProductExpanded = value;
                RaisePropertyChanged(() => IsInventProductExpanded);
            }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public bool CanSearch()
        {
            return true;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.IturAdvancedSearch;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<LocationAddedEvent>().Subscribe(LocationAdded);

            if (_filter == null)
            {
                _filter = new IturAdvancedFilterData();
            }

            BuildLocation(_filter);
            BuildStatus(_filter);
			BuildTag(_filter);
            BuildSort();

            SetIturFilterSelectedFromConfig();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<LocationAddedEvent>().Unsubscribe(LocationAdded);
        }

        private void BuildSort()
        {
            _sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, _regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.IturCode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.Number));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.NumberPrefix));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.NumberSufix));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.LocationCode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.ERPIturCode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.StatusIturGroupBit));

            _sortViewModel.Add(sortProperties);
        }

        private void BuildStatus(IturAdvancedFilterData filter)
        {
            _statusItems.Clear();

            try
            {
                Dictionary<string, StatusIturGroup> codes = this._statusGroupRepository.CodeStatusIturGroupDictionary;
                foreach (KeyValuePair<string, StatusIturGroup> kvp in codes)
                {
                    StatusIturGroup statusIturGroup = kvp.Value;
                    string color =
                        UtilsStatus.FromStatusGroupBitToColor(this._statusGroupRepository.BitStatusIturGroupEnumDictionary,
                                                              statusIturGroup.Bit, this._userSettingsManager);

					bool isChecked = false; //true;
                    if (filter != null && filter.Statuses != null)
                    {
                        isChecked = filter.Statuses.Any(r => r == statusIturGroup.Bit);
                    }
                    StatusGroupItemViewModel item = new StatusGroupItemViewModel(statusIturGroup);
                    item.IsChecked = isChecked;
                    item.BackgroundColor = color;
                    this._statusItems.Add(item);


                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildStatus", exc);
            }
        }

        private void BuildLocation(IturAdvancedFilterData filter)
        {
            _locationItems.Clear();

            try
            {

                Locations locations = _locationRepository.GetLocations(base.GetDbPath);

	

                _locationListViewModelBuilder.Build(_locationItems, base.GetDbPath,	 null,
                                                    location =>
                                                    {
                                                        bool isChecked = false;
                                                        if (filter != null && filter.Locations != null)
                                                        {
                                                            isChecked = filter.Locations.Any(r => r == location.Code);
                                                        }

                                                        return isChecked;
                                                    },
                                                    location =>
                                                    {
                                                        Location realLocation = locations.FirstOrDefault(r => r.Code == location.Code);
                                                        return realLocation == null;
                                                    }
                    );
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildItems", exc);
            }

        }

		private void BuildTag(IturAdvancedFilterData filter)
		{
			this._tagItems.Clear();

			try
			{
				List<string> tagList = this._iturRepository.GetTagList(base.GetDbPath);
				foreach (string tag in tagList)
				{
					bool isChecked = false; //true;
					if (filter != null && filter.Tags != null)
					{
						isChecked = filter.Tags.Any(r => r == tag);
					}
					TagItemViewModel item = new TagItemViewModel(tag);
					item.IsChecked = isChecked;
					this._tagItems.Add(item);
				}
		
			}
			catch (Exception exc)
			{
				_logger.ErrorException("BuildTag", exc);
			}

		
		}

        private void ViewLocationCommandExecuted()
        {
            ClosePopup();

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.History);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(query, GetHistoryAuditConfig());

            UtilsNavigate.LocationAddEditDeleteOpen(this._regionManager, query);
        }

        private void AddUnknownLocationCommandExecuted(LocationItemViewModel locationItemViewModel)
        {
            this._eventAggregator.GetEvent<LocationAddEvent>().Publish(new LocationAddedEventPayLoad()
            {
                AddUnknownLocation = true,
                Location = locationItemViewModel.Location,
                Context = CBIContext.History,
                DbContext = Common.NavigationSettings.CBIDbContextInventor,
            });
        }

        private void LocationAdded(Location location)
        {
            if (_filter != null)
                BuildLocation(_filter);
        }

        private void ClosePopup()
        {
            UtilsPopup.Close(View);
        }

        public IFilterData BuildFilterData()
        {
            IturAdvancedFilterData filter = new IturAdvancedFilterData();

			filter.IsFromFilter = true;		  //!!! Marina - temlate\filter

            filter.IsLocation = _isFilterByLocation;
            filter.IsStatus = _isFilterByStatus;
			filter.IsTag = _isFilterByTag;
            filter.IsInventProduct = _isFilterByInventProduct;

            filter.Text = _iturFilterText;
            filter.Field = _iturFilterSelected == null ? String.Empty : _iturFilterSelected.Value;

            if (_isFilterByLocation)
            {
                filter.Locations = _locationItems.Where(r => r.IsChecked).Select(r => r.Location.Code).ToList();
            }

            if (_isFilterByStatus)
            {
                filter.Statuses = _statusItems.Where(r => r.IsChecked).Select(r => r.StatusIturGroup.Bit).ToList();
            }

			if (_isFilterByTag)
			{
				filter.Tags = _tagItems.Where(r => r.IsChecked).Select(r => r.Tag).ToList();
			}

            if (_isFilterByInventProduct)
            {
                filter.InventProductName = _filterProductName;
                filter.InventProductBarcode = _filterBarcode;
                filter.InventProductMakat = _filterMakat;
            }

            filter.IsLocationExpanded = _isLocationExpanded;
            filter.IsStatusExpanded = _isStatusExpanded;
			filter.IsTagExpanded = _isTagExpanded;
			
            filter.IsInventProductExpanded = _isInventProductExpanded;
		
            _sortViewModel.ApplyToFilterData(filter);

            return filter;
        }


		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			IturAdvancedFilterData filter = new IturAdvancedFilterData();

			filter.IsLocation = _isFilterByLocation;
			filter.IsStatus = _isFilterByStatus;
			filter.IsTag = _isFilterByTag;
			filter.IsInventProduct = _isFilterByInventProduct;

			filter.Text = _iturFilterText;
			filter.Field = _iturFilterSelected == null ? String.Empty : _iturFilterSelected.Value;

			if (_isFilterByLocation)
			{
				filter.Locations = _locationItems.Where(r => r.IsChecked).Select(r => r.Location.Code).ToList();
			}

			if (_isFilterByStatus)
			{
				filter.Statuses = _statusItems.Where(r => r.IsChecked).Select(r => r.StatusIturGroup.Bit).ToList();
			}

			if (_isFilterByTag)
			{
				filter.Tags = _tagItems.Where(r => r.IsChecked).Select(r => r.Tag).ToList();
			}

			if (_isFilterByInventProduct)
			{
				filter.InventProductName = _filterProductName;
				filter.InventProductBarcode = _filterBarcode;
				filter.InventProductMakat = _filterMakat;
			}

			filter.IsLocationExpanded = _isLocationExpanded;
			filter.IsStatusExpanded = _isStatusExpanded;
			filter.IsTagExpanded = _isTagExpanded;
			filter.IsInventProductExpanded = _isInventProductExpanded;

			_sortViewModel.ApplyToFilterData(filter);

			return filter;
		}

        public void ApplyFilterData(IFilterData data)
        {
            IturAdvancedFilterData filter = data as IturAdvancedFilterData;
            if (filter == null) return;

            IsLocationExpanded = filter.IsLocationExpanded;
            IsStatusExpanded = filter.IsStatusExpanded;
			IsTagExpanded = filter.IsTagExpanded; 
            IsInventProductExpanded = filter.IsInventProductExpanded;

            IsFilterByLocation = filter.IsLocation;
            IsFilterByStatus = filter.IsStatus;
			IsFilterByTag = filter.IsTag;
            IsFilterByInventProduct = filter.IsInventProduct;

            IturFilterText = filter.Text;
			
            if (!String.IsNullOrEmpty(filter.Field))
            {
                IturFilterSelected = _iturFilterItems.FirstOrDefault(r => r.Value == filter.Field);
            }

            FilterBarcode = filter.InventProductBarcode;
            FilterMakat = filter.InventProductMakat;
            FilterProductName = filter.InventProductName;

            BuildLocation(filter);
            BuildStatus(filter);
			BuildTag(filter);

            _sortViewModel.InitFromFilterData(filter);

            _filter = filter;
        }

        public void Reset()
        {
            IturFilterText = String.Empty;
            IturFilterSelected = _iturFilterItems.FirstOrDefault();

            IsFilterByLocation = false;
            IsFilterByStatus = false;
			IsFilterByTag = false;
            IsFilterByInventProduct = false;

            FilterMakat = String.Empty;
            FilterBarcode = String.Empty;
            FilterProductName = String.Empty;

            foreach (StatusGroupItemViewModel statusGroupItemViewModel in _statusItems)
            {
				statusGroupItemViewModel.IsChecked = false;
            }

            foreach (LocationItemViewModel locationItemViewModel in _locationItems)
            {
                locationItemViewModel.IsChecked = false;
            }

			foreach (TagItemViewModel tagItemViewModel in _tagItems)
            {
				tagItemViewModel.IsChecked = false;
            }

            SetIturFilterSelectedFromConfig();

            _sortViewModel.Reset();
        }

        private void SetIturFilterSelectedFromConfig()
        {
            IturFilterSelected = _iturFilterItems.FirstOrDefault(r => r.Value == _userSettingsManager.IturFilterSelectedGet());
        }
    }
}