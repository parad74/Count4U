using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using NLog;

namespace Count4U.Modules.Audit.ViewModels
{
    public class SearchIturFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;

        private readonly IStatusIturGroupRepository _statusIturGroupRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ILocationRepository _locationRepository;
		private readonly IIturRepository _iturRepository;
        private readonly LocationListViewModelBuilder _locationListViewModelBuilder;

        private string _number;
        private string _status;
        private DateTime? _date;
        private string _erp;
        private DelegateCommand _searchCommand;

        private bool _isFilterByLocation;
        private bool _isFilterByStatus;
		private bool _isFilterByTag;

        private readonly ObservableCollection<StatusGroupItemViewModel> _statusItems;
        private readonly ObservableCollection<LocationItemViewModel> _locationItems;
		private readonly ObservableCollection<TagItemViewModel> _tagItems;

        private bool _isLocationExpanded;
        private bool _isStatusExpanded;
		private bool _isTagExpanded;

        private SortViewModel _sortViewModel;

        private IturFilterData _filter;

		private bool _isCheckedLocations;
		private bool _isCheckedStatus;
		private bool _isCheckedTag;
//        private bool? _isDisabled;

		private string _findByLocationCode = String.Empty;
		private string _findByLocationTag = String.Empty;

		public SearchIturFieldViewModel(
			IContextCBIRepository contextCbiRepository,
			IRegionManager regionManager,
			IStatusIturGroupRepository statusIturGroupRepository,
			IUserSettingsManager userSettingsManager,
			ILocationRepository locationRepository,
			IIturRepository iturRepository,
			LocationListViewModelBuilder locationListViewModelBuilder)
			: base(contextCbiRepository)
		{
			_locationListViewModelBuilder = locationListViewModelBuilder;
			_locationRepository = locationRepository;
			_iturRepository = iturRepository;
			_userSettingsManager = userSettingsManager;
			_statusIturGroupRepository = statusIturGroupRepository;
			_regionManager = regionManager;
			_number = String.Empty;
			_status = String.Empty;
			_erp = String.Empty;

			_statusItems = new ObservableCollection<StatusGroupItemViewModel>();
			_locationItems = new ObservableCollection<LocationItemViewModel>();
			_tagItems = new ObservableCollection<TagItemViewModel>();

			_isLocationExpanded = true;
			_isStatusExpanded = true;
			_isTagExpanded = true;

			this.IsCheckedStatus = false;
			this.IsCheckedLocations = false;
			this.IsCheckedTag = false;
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


        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                RaisePropertyChanged(() => Number);
            }
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

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public DateTime? Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            }
        }

        public string ERP
        {
            get { return _erp; }
            set
            {
                _erp = value;
                RaisePropertyChanged(() => ERP);
            }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public ObservableCollection<StatusGroupItemViewModel> StatusItems
        {
            get { return _statusItems; }
        }

        public ObservableCollection<LocationItemViewModel> LocationItems
        {
            get { return _locationItems; }
        }


		public ObservableCollection<TagItemViewModel> TagItems
		{
			get { return _tagItems; }
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

        public bool IsFilterByStatus
        {
            get { return _isFilterByStatus; }
            set
            {
                _isFilterByStatus = value;
                RaisePropertyChanged(() => IsFilterByStatus);
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

//        public bool? IsDisabled
//        {
//            get { return _isDisabled; }
//            set
//            {
//                _isDisabled = value;
//                RaisePropertyChanged(() => IsDisabled);
//            }
//        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (_filter == null)
            {
                _filter = new IturFilterData();
            }

            BuildLocation(_filter);
            BuildStatus(_filter);
			BuildTag(_filter);
            BuildSort();
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

        public bool CanSearch()
        {
            return true;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.IturSearch;
        }

        private IturFilterData BuildFilterDataInner()
        {
            IturFilterData result = new IturFilterData();

            result.Number = _number;
            result.Status = _status;
            result.Date = _date;
            result.ERP = _erp;

            result.IsLocation = _isFilterByLocation;
            result.IsStatus = _isFilterByStatus;
			result.IsTag = _isFilterByTag;

            if (_isFilterByLocation)
            {
                result.Locations = _locationItems.Where(r => r.IsChecked).Select(r => r.Location.Code).ToList();
            }

            if (_isFilterByStatus)
            {
                result.Statuses = _statusItems.Where(r => r.IsChecked).Select(r => r.StatusIturGroup.Bit).ToList();
            }

			if (_isFilterByTag)
			{
				result.Tags = _tagItems.Where(r => r.IsChecked).Select(r => r.Tag).ToList();
			}

            result.IsLocationExpanded = _isLocationExpanded;
            result.IsStatusExpanded = _isStatusExpanded;
			result.IsTagExpanded = _isTagExpanded;

//            result.IsDisabled = _isDisabled;

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

        public IFilterData BuildFilterData()
        {
            return BuildFilterDataInner();
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			return BuildFilterDataInner();
		}

        public void ApplyFilterData(IFilterData data)
        {
            IturFilterData iturData = data as IturFilterData;

            if (iturData == null) return;

            Number = iturData.Number;
            Status = iturData.Status;
            Date = iturData.Date;
            ERP = iturData.ERP;

            IsLocationExpanded = iturData.IsLocationExpanded;
            IsStatusExpanded = iturData.IsStatusExpanded;
			IsTagExpanded = iturData.IsTagExpanded; 

            IsFilterByLocation = iturData.IsLocation;
            IsFilterByStatus = iturData.IsStatus;
			IsFilterByTag = iturData.IsTag;

            BuildLocation(iturData);
            BuildStatus(iturData);
			BuildTag(iturData);

//            IsDisabled = iturData.IsDisabled;

            _sortViewModel.InitFromFilterData(iturData);
        }

        public void Reset()
        {
            Number = String.Empty;
            Status = String.Empty;
            Date = null;
            ERP = String.Empty;

            IsFilterByLocation = false;
            IsFilterByStatus = false;
			IsFilterByTag = false;

//            IsDisabled = null;

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


            _sortViewModel.Reset();
        }

        private void BuildStatus(IturFilterData filter)
        {
            _statusItems.Clear();

            try
            {
                Dictionary<string, StatusIturGroup> codes = this._statusIturGroupRepository.CodeStatusIturGroupDictionary;
                foreach (KeyValuePair<string, StatusIturGroup> kvp in codes)
                {
                    StatusIturGroup statusIturGroup = kvp.Value;
                    string color =
                        UtilsStatus.FromStatusGroupBitToColor(this._statusIturGroupRepository.BitStatusIturGroupEnumDictionary,
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

        private void BuildLocation(IturFilterData filter)
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

		private void BuildTag(IturFilterData filter)
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
    }
}