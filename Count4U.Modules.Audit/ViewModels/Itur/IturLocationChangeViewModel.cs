using System;
using System.Collections.Generic;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Interface.Count4U;
using System.Linq;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;

namespace Count4U.Modules.Audit.ViewModels
{
	public class IturLocationChangeViewModel : CBIContextBaseViewModel
	{
		private readonly ILocationRepository _locationRepository;
		private readonly IIturRepository _iturRepository;
		private readonly IEventAggregator _eventAggregator;
		private readonly IImportIturBlukRepository _importIturBlukRepository;

		private readonly DelegateCommand _cancelCommand;
		private readonly DelegateCommand _okCommand;
		private Iturs _iturs;

		private Location _selectedLocation;
		private Locations _locationsTo;

		private string _filterLocation;

		private string _iturList = String.Empty;
		private string _inIturCodes = String.Empty;
		//private string _inLocationCode = String.Empty;
		private string _inTextValue = String.Empty;
		private string _inTextPrefix = String.Empty;

		private Iturs _itursSelectedList;
		private Locations _locationsAll;

		private string _textValue;
		private bool _includeAllItursFromLocation;

		private bool _includeAllIturs;

		private string _textValuePlusPrefix;
		private string _plusPrefix;
		//private string _inPlusPrefix = String.Empty;      //TODO	 _inTextValue

		private bool _includeAllItursWithPrefix;
		private string _textPrefix;

		private bool _isPrefixAndNumber;

		private bool _isIturCode;
		private bool _isLocationAndNumber;

		public IturLocationChangeViewModel(IContextCBIRepository repository,
			ILocationRepository locationRepository,
			IIturRepository iturRepository,
			IImportIturBlukRepository importIturBlukRepository,
			IEventAggregator eventAggregator)
			: base(repository)
		{
			this._eventAggregator = eventAggregator;
			this._iturRepository = iturRepository;
			this._locationRepository = locationRepository;
			this._importIturBlukRepository = importIturBlukRepository;
			this._includeAllItursFromLocation = false;
			this._includeAllIturs = false;
			this._includeAllItursWithPrefix = false;
			this._isPrefixAndNumber = false;
			this._isLocationAndNumber = false;
			this._isIturCode = true;

			this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
			this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
			this._locationsAll = new Locations();
		}


		public bool IsIturCode
		{
			get { return _isIturCode; }
			set
			{
				this._isIturCode = value;
				if (value == true)                    //!!!
				{
					this._isLocationAndNumber = !value;
					this._includeAllItursFromLocation = !value;
					this._includeAllIturs = !value;
					this._includeAllItursWithPrefix = !value;
					this._isPrefixAndNumber = !value;

					this._iturList = _inIturCodes;
					this._textValue = "";
					this._textPrefix = "";
					this._plusPrefix = "";
					this._textValuePlusPrefix = "";

					this.RaisePropertyChanged(() => this.IncludeAllItursFromLocation);
					this.RaisePropertyChanged(() => this.IncludeAllIturs);
					this.RaisePropertyChanged(() => this.IsLocationAndNumber);
					this.RaisePropertyChanged(() => this.IncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsIturCode);


					this.RaisePropertyChanged(() => this.TextValue);
					this.RaisePropertyChanged(() => this.TextPrefix);
					this.RaisePropertyChanged(() => this.PlusPrefix);
					this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
					this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);


					this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);


					//	this.RaisePropertyChanged(() => this.EnabledIncludeAllIturs);
					//this._okCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IncludeAllIturs
		{
			get { return _includeAllIturs; }
			set
			{
				this._includeAllIturs = value;
				if (value == true)                    //!!!
				{
					this._isLocationAndNumber = !value;
					this._includeAllItursFromLocation = !value;
					this._includeAllItursWithPrefix = !value;
					this._isPrefixAndNumber = !value;
					this._isIturCode = !value;

					this._iturList = _inIturCodes;
					this._textValue = "";
					this._textPrefix = "";
					this._plusPrefix = "";
					this._textValuePlusPrefix = "";

					this.RaisePropertyChanged(() => this.IncludeAllIturs);
					this.RaisePropertyChanged(() => this.IncludeAllItursFromLocation);
					this.RaisePropertyChanged(() => this.IsLocationAndNumber);
					this.RaisePropertyChanged(() => this.IncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsIturCode);


					this.RaisePropertyChanged(() => this.TextValue);
					this.RaisePropertyChanged(() => this.TextPrefix);
					this.RaisePropertyChanged(() => this.PlusPrefix);
					this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
					this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);


					this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);


					//	this.RaisePropertyChanged(() => this.EnabledIncludeAllIturs);
					//this._okCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IsLocationAndNumber
		{
			get { return _isLocationAndNumber; }
			set
			{
				this._isLocationAndNumber = value;
				if (value == true)                    //!!!
				{
					this._isIturCode = !value;
					this._includeAllItursFromLocation = !value;
					this._includeAllIturs = !value;
					this._includeAllItursWithPrefix = !value;
					this._isPrefixAndNumber = !value;

					this._iturList = "";
					this._textValue = this._inTextValue;
					this._textPrefix = "";
					this._plusPrefix = "";
					this._textValuePlusPrefix = "";


					this.RaisePropertyChanged(() => this.IncludeAllItursFromLocation);
					this.RaisePropertyChanged(() => this.IncludeAllIturs);
					this.RaisePropertyChanged(() => this.IsLocationAndNumber);
					this.RaisePropertyChanged(() => this.IncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsIturCode);

					this.RaisePropertyChanged(() => this.TextValue);
					this.RaisePropertyChanged(() => this.TextPrefix);
					this.RaisePropertyChanged(() => this.PlusPrefix);
					this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
					this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);

					this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);
					//this.RaisePropertyChanged(() => this.EnabledIncludeAllIturs);
					//this._okCommand.RaiseCanExecuteChanged();
				}
			}
		}


		//public string ItursSelectedList 
		//{
		//	get {
		//		if (_itursSelectedList == null) return "";
		//		var entitys = this._itursSelectedList.Select(e => e.IturCode).Distinct().ToList();
		//		string ret = entitys.JoinRecord(",");
		//		return ret; 
		//	}
		//}

		public bool IncludeAllItursFromLocation //*
		{
			get { return this._includeAllItursFromLocation; }
			set
			{
				this._includeAllItursFromLocation = value;
				if (value == true)                    //!!!
				{
					this._isLocationAndNumber = !value;
					this._isIturCode = !value;
					this._includeAllIturs = !value;
					this._includeAllItursWithPrefix = !value;
					this._isPrefixAndNumber = !value;

					this._textValue = "";
					this._iturList = "";
					this._textPrefix = "";
					this._plusPrefix = "";
					this._textValuePlusPrefix = "";

					this.RaisePropertyChanged(() => this.IncludeAllItursFromLocation);
					this.RaisePropertyChanged(() => this.IncludeAllIturs);
					this.RaisePropertyChanged(() => this.IsLocationAndNumber);
					this.RaisePropertyChanged(() => this.IncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsIturCode);

					this.RaisePropertyChanged(() => this.TextValue);
					this.RaisePropertyChanged(() => this.TextPrefix);
					this.RaisePropertyChanged(() => this.PlusPrefix);
					this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
					this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);

					this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);
					//	this.RaisePropertyChanged(() => this.EnabledIncludeAllIturs);

					//this._okCommand.RaiseCanExecuteChanged();
				}
			}
		}



		public bool IncludeAllItursWithPrefix //*
		{
			get { return this._includeAllItursWithPrefix; }
			set
			{
				this._includeAllItursWithPrefix = value;
				if (value == true)                    //!!!
				{
					this._isLocationAndNumber = !value;       //TODO
					this._isIturCode = !value;
					this._includeAllItursFromLocation = !value;
					this._includeAllIturs = !value;
					this._isPrefixAndNumber = !value;

					this._textValue = "";
					this._iturList = "";
					this._textPrefix = this._inTextPrefix;
					this._plusPrefix = "";
					this._textValuePlusPrefix = "";


					this.RaisePropertyChanged(() => this.IncludeAllItursFromLocation);
					this.RaisePropertyChanged(() => this.IncludeAllIturs);
					this.RaisePropertyChanged(() => this.IsLocationAndNumber);
					this.RaisePropertyChanged(() => this.IncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsIturCode);


					this.RaisePropertyChanged(() => this.TextValue);
					this.RaisePropertyChanged(() => this.TextPrefix);
					this.RaisePropertyChanged(() => this.PlusPrefix);
					this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
					this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);

					this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);
				}
			}
		}


		public bool IsPrefixAndNumber
		{
			get { return _isPrefixAndNumber; }
			set
			{
				this._isPrefixAndNumber = value;
				if (value == true)                    //!!!
				{
					this._isIturCode = !value;
					this._includeAllItursFromLocation = !value;
					this._includeAllIturs = !value;
					this._includeAllItursWithPrefix = !value;

					this._iturList = "";
					this._textValue = "";
					this._textPrefix = "";
					this._plusPrefix = this._inTextPrefix;
					this._textValuePlusPrefix = this._inTextValue;


					this.RaisePropertyChanged(() => this.IncludeAllItursFromLocation);
					this.RaisePropertyChanged(() => this.IncludeAllIturs);
					this.RaisePropertyChanged(() => this.IsLocationAndNumber);
					this.RaisePropertyChanged(() => this.IncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsIturCode);

					this.RaisePropertyChanged(() => this.TextValue);
					this.RaisePropertyChanged(() => this.TextPrefix);
					this.RaisePropertyChanged(() => this.PlusPrefix);
					this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
					this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);

					this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
					this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);
				}
			}
		}

		public Locations LocationsAll
		{
			get { return _locationsAll; }
			//set
			//{
			//	_locations = value;
			//	RaisePropertyChanged(() => Locations);
			//}
		}

		public Locations LocationsTo
		{
			get { return _locationsTo; }
			set
			{
				_locationsTo = value;
				RaisePropertyChanged(() => LocationsTo);
			}
		}

		public Location SelectedLocation
		{
			get { return _selectedLocation; }
			set
			{
				_selectedLocation = value;
				RaisePropertyChanged(() => SelectedLocation);

				_okCommand.RaiseCanExecuteChanged();
			}
		}

		private Location _selectedAllFromLocation;
		public Location SelectedAllFromLocation
		{
			get { return _selectedAllFromLocation; }
			set
			{
				_selectedAllFromLocation = value;
				this.RaisePropertyChanged(() => SelectedAllFromLocation);
				this.RaisePropertyChanged(() => this.IturList);
				this.RaisePropertyChanged(() => this.TotalIturs);

				_okCommand.RaiseCanExecuteChanged();
			}
		}




		private Location _selectedLocationAndNumber;
		public Location SelectedLocationAndNumber
		{
			get { return _selectedLocationAndNumber; }
			set
			{
				this._selectedLocationAndNumber = value;
				this.RaisePropertyChanged(() => SelectedLocationAndNumber);
				this.RaisePropertyChanged(() => this.IturList);
				this.RaisePropertyChanged(() => this.TotalIturs);

				_okCommand.RaiseCanExecuteChanged();
			}
		}


		public string FilterLocation
		{
			get { return _filterLocation; }
			set
			{
				_filterLocation = value;
				RaisePropertyChanged(() => FilterLocation);
				using (new CursorWait())
				{
					BuildLocationCodeListWithFilter();
					RaisePropertyChanged(() => LocationsTo);
					//RaisePropertyChanged(() => SelectedLocation);
				}
			}
		}

		private void BuildLocationCodeFullList()
		{
			//this._locationsAll.Clear();
			this._locationsAll = this._locationRepository.GetLocations(base.GetDbPath);
			this._locationsTo = this._locationsAll;
			if (_locationsAll == null) return;

			string LocationCode = this._locationRepository.GetFistFromAllLocationCodeWithoutIturs(base.GetDbPath);

			if (string.IsNullOrWhiteSpace(LocationCode) == false)
			{
				this._selectedLocation = this._locationRepository.GetLocationByCode(LocationCode, base.GetDbPath);
			}
			else
			{
				this._selectedLocation = this._locationsAll.FirstOrDefault();
			}

			if (this._selectedAllFromLocation == null)
			{
				this._selectedAllFromLocation = this._selectedLocation;
			}

			if (this._selectedLocationAndNumber == null)
			{
				this._selectedLocationAndNumber = this._selectedLocation;
			}

		}

		private void BuildLocationCodeListWithFilter()
		{
			//this._locationsTo.Clear();
			var result = this._locationRepository.GetLocations(base.GetDbPath);

			Locations ret = UtilsMisc.FilterLocationsByName(result, _filterLocation);
			if (ret == null) return;
			var list = ret.OrderBy(x => x.Name).Select(x => x).AsEnumerable();
			if (list == null) return;
			this._locationsTo = Locations.FromEnumerable(list);
			if (this._locationsTo == null) return;

			string LocationCode = this._locationRepository.GetFistLocationCodeWithoutIturs(this._locationsTo, base.GetDbPath);

			if (string.IsNullOrWhiteSpace(LocationCode) == false)
			{
				this.SelectedLocation = this._locationRepository.GetLocationByCode(LocationCode, base.GetDbPath);
			}
			else
			{
				if (this._locationsTo.Count() > 0)
				{
					this._selectedLocation = this._locationsTo.FirstOrDefault();
				}
				else
				{
					this._selectedLocation = null;
				}
			}

			//if (this._selectedAllFromLocation == null)
			//{
			//	this._selectedAllFromLocation = this.SelectedLocation;
			//}

			//if (this._selectedLocationAndNumber == null)
			//{
			//	this._selectedLocationAndNumber = this.SelectedLocation;
			//}

		}

		public string TextValue//*
		{
			get { return this._textValue; }
			set
			{
				if (IsLocationAndNumber == true)
				{
					this._textValue = value;
					//this._inTextValue = value;
					this._iturList = "";
				}
				else
				{
					this._textValue = "";
				}

				this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.IturList);
				this.RaisePropertyChanged(() => this.TotalIturs);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public string TextPrefix//*										   //TODO
		{
			get { return this._textPrefix; }
			set
			{
				if (IncludeAllItursWithPrefix == true)
				{
					this._textPrefix = value;
					//this._inTextPrefix = value;		  //TODO
					this._iturList = "";
				}
				else
				{
					this._textPrefix = "";
				}

				//this.RaisePropertyChanged(() => this.PlusPrefix);          
				this.RaisePropertyChanged(() => this.TextPrefix);       //TODO

				//this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.IturList);
				this.RaisePropertyChanged(() => this.TotalIturs);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public string PlusPrefix//*										   //TODO
		{
			get { return this._plusPrefix; }
			set
			{
				if (IsPrefixAndNumber == true)
				{
					this._plusPrefix = value;
					//this._inPlusPrefix = value;
					this._iturList = "";
				}
				else
				{
					this._plusPrefix = "";
				}

				this.RaisePropertyChanged(() => this.PlusPrefix);
				//this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.IturList);
				this.RaisePropertyChanged(() => this.TotalIturs);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public string TextValuePlusPrefix//*
		{
			get { return this._textValuePlusPrefix; }
			set
			{
				if (IsPrefixAndNumber == true)
				{
					this._textValuePlusPrefix = value;
					//this._inTextValue = value;
					//this._textValue = "";
					this._iturList = "";
				}
				else
				{
					this._textValuePlusPrefix = "";
				}

				this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
				//this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.IturList);
				this.RaisePropertyChanged(() => this.TotalIturs);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		private void RefeshItursSelectedList()
		{
			this._itursSelectedList = null;
			if (IsLocationAndNumber == true)
			{
				if (this._selectedLocationAndNumber != null)
					this._itursSelectedList = this.ItursWithLocationAndNumber(this._selectedLocationAndNumber.Code, this._textValue);
			}
			else if (IncludeAllIturs == true)
			{
				this._itursSelectedList = this.AllIturs();
			}
			else if (IncludeAllItursFromLocation == true)
			{
				if (this._selectedAllFromLocation != null)
					this._itursSelectedList = ItursWithLocationCode(this._selectedAllFromLocation.Code);
			}
			else if (IncludeAllItursWithPrefix == true)              //TODO
			{
				this._itursSelectedList = ItursWithPrefix(this._textPrefix);
			}
			else if (IsPrefixAndNumber == true)                         //TODO
			{
				this._itursSelectedList = this.ItursWithPrefixAndNumber(this._plusPrefix, this._textValuePlusPrefix);
			}
			else
			{
				this._itursSelectedList = _iturs;
			}
		}

	public string IturList
		{
			get
			{
				try
				{
					//RefeshItursSelectedList();

					if (this._itursSelectedList == null)
					{
						this._iturList = "";
					}
					else
					{
						var entitys = this._itursSelectedList.Select(e => e.IturCode).Distinct().ToList();
						this._iturList = entitys.JoinRecord(",");
					}

					return _iturList;
				}
				catch
				{
					return "";
				}
			}
		}
	

		public string TotalIturs//*
		{
			get
			{
				try
				{
					RefeshItursSelectedList();

					return _itursSelectedList == null ? "0" : _itursSelectedList.Count.ToString();
				}
				catch
				{
					return "0";
				}
			}
		}

		private bool IsTextValid() //*
		{
			return CommaDashStringParser.IsValid(this._textValue);
		}

		private bool IsTextValuePlusPrefixValid() //*
		{
			return CommaDashStringParser.IsValid(this._textValuePlusPrefix);
		}

			

		public string this[string propertyName] //*
		{
			get
			{
				switch (propertyName)
				{
					case "TextValue":
						{
							if (this.IsTextValid() == false)
							{
								return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
							}
						}

						break;
					case "TextValuePlusPrefix":
						{
							if (this.IsTextValuePlusPrefixValid() == false)
							{
								return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
							}
						}

						break;
					case "TextPrefix":
						{
							int bit = this._textPrefix.PrefixValidate();
							if (bit != 0)
							{
								return IturValidate.Bit2PrefixErrorMessage(bit);
							}
						}
						//if (IsPrefixValid() == false)
						//{
						//	return "Prefix is not empty numeric string 4 characters maximum";
						//}

						break;

					case "PlusPrefix":
						{
							int bit = this._plusPrefix.PrefixValidate();
							if (bit != 0)
							{
								return IturValidate.Bit2PrefixErrorMessage(bit);
							}
						}
						//if (IsPrefixValid() == false)
						//{
						//	return "Prefix is not empty numeric string 4 characters maximum";
						//}

						break;
				}
				return null;
			}
		}

		private bool IsPrefixValid()
		{
			if (String.IsNullOrEmpty(this._textPrefix))
				return true;
			return this._textPrefix.Length <= 4 &&
				   Regex.IsMatch(this._textPrefix, @"^[0-9]+$");
		}

		public bool IsEnabledLocationAndNumber //*	  IsReadOnly
		{
			get
			{
				if (IsLocationAndNumber == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool IsEnabledIncludeAllItursFromLocation
		{
			get
			{
				if (IncludeAllItursFromLocation == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool IsEnabledPrefixAndNumber //*	  IsReadOnly
		{
			get
			{
				if (IsPrefixAndNumber == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public bool IsReadLocationAndNumber //*	  IsReadOnly
		{
			get
			{
				if (IsLocationAndNumber == true)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		//public bool IsReadPrefixAndNumber //*	  IsReadOnly
		//{
		//	get
		//	{
		//		if (IsPrefixAndNumber == true)
		//		{
		//			return false;
		//		}
		//		else
		//		{
		//			return true;
		//		}
		//	}
		//}

		public bool IsReadIncludeAllItursWithPrefix //*	  IsReadOnly
		{
			get
			{
				if (IncludeAllItursWithPrefix == true)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		public bool IsReadPrefixAndNumber //*	  IsReadOnly
		{
			get
			{
				if (IsPrefixAndNumber == true)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		public bool IsReadOnlyNumber //*	  IsReadOnly
		{
			get 
			{
				if (IsLocationAndNumber == true)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		//public bool EnabledIncludeAllIturs //*
		//{
		//	get
		//	{
		//		if (IsNumber == true)
		//		{
		//			return true; 
		//		}
		//		else
		//		{
		//			return false;
		//		}
			
		//	}
			
		//}

	

		public string Error //*
		{
			get { return string.Empty; }
		}

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			try
			{
				using (new CursorWait())
				{
					string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
					this._inIturCodes = commaSeparatedNumbers;
					this._iturList = _inIturCodes;


					string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();

					List<int> itursNumbers = new List<int>();
					Dictionary<string, Itur> iturDictionary = this._iturRepository.GetIturDictionary(base.GetDbPath, true);
					Dictionary<string, Location> locationDictionary = this._locationRepository.GetLocationDictionary(base.GetDbPath, true);

					this._iturs = new Iturs();
					foreach (string iturCode in split)
					{
						Itur itur = null;
						iturDictionary.TryGetValue(iturCode, out itur);
						if (itur != null)
						{
							this._inTextPrefix = itur.NumberPrefix;
							//this._inPlusPrefix = itur.NumberPrefix;

							//this._inLocationCode = itur.LocationCode;
							//Location location = this._locationRepository.GetLocationByCode(itur.LocationCode, base.GetDbPath);
							Location location = null;
							locationDictionary.TryGetValue(itur.LocationCode, out location);
							if (location != null)
							{
								if (this._selectedAllFromLocation == null)
									this._selectedAllFromLocation = location;
								if (this._selectedLocationAndNumber == null)
									this._selectedLocationAndNumber = location;
							}
							itursNumbers.Add(itur.Number);
							this._iturs.Add(itur);
						}
					}

					this._inTextValue = CommaDashStringParser.Reverse(itursNumbers); //*
																					 //this.TextValue = this._inTextValue;


					//this.ItursNumber = CommaDashStringParser.Reverse(itursNumbers);
					//this.Locations = this._locationRepository.GetLocations(base.GetDbPath);
					BuildLocationCodeFullList();

				}
			}
			catch (Exception ex)
			{
				string logmessage = ex.Message;
			}
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

		private Iturs ItursWithNumber(string textValue)
		{
			List<int> itursNumberList = CommaDashStringParser.Parse(textValue);
			Iturs itursAll = new Iturs();
			try
			{
				if (itursNumberList != null)
				{
					List<int> itursNumberListDistinct = itursNumberList.Distinct().ToList();

					foreach (int iturNumber in itursNumberListDistinct)
					{
						var iturs = this._iturRepository.GetItursByNumber(iturNumber, base.GetDbPath);
						itursAll.AddEnumerableIturs(iturs);
					}
				}
			}
			catch { }
			return itursAll;
		}

		private Iturs ItursWithLocationAndNumber(string locationCode, string textValue)
		{
			Iturs itursRet = new Iturs();
			try
			{
				List<int> itursNumberList = CommaDashStringParser.Parse(textValue);
				if (itursNumberList != null)
				{
					var itursWithLocationCode = this._iturRepository.GetItursByLocationCode(locationCode, base.GetDbPath);
					List<int> itursNumberListDistinct = itursNumberList.Distinct().ToList();

					foreach (int iturNumber in itursNumberListDistinct)
					{
						//var iturs = this._iturRepository.GetItursByNumber(iturNumber, base.GetDbPath);
						//var itursWithLocation = iturs.Where(x => x.LocationCode == locationCode).Select(x => x).ToList();
						List<Itur> itursWithLocation = itursWithLocationCode.Where(x => x.Number == iturNumber).Select(x => x).ToList();
						foreach (Itur itur in itursWithLocation)
						{
							itursRet.Add(itur);
						}
					}
				}
			}
			catch { }
			return itursRet;
		}

		private Iturs ItursWithLocationCode(string locationCode)
		{
			Iturs itursAll = this._iturRepository.GetItursByLocationCode(locationCode, base.GetDbPath);
			return itursAll;
		}

		
		private Iturs AllIturs()
		{
			Iturs itursAll = this._iturRepository.GetIturs( base.GetDbPath);
			return itursAll;
		}
		private void OkCommandExecuted()
		{
			Iturs itursAll = new Iturs();
			try
			{
				using (new CursorWait())
				{
					//if (IsLocationAndNumber == true)
					//{
					//	itursAll = ItursWithLocationAndNumber(this._selectedLocationAndNumber.Code, this._textValue);
					//}
					//else if (this.IncludeAllItursFromLocation == true)
					//{
					//	itursAll = ItursWithLocationCode(this._selectedAllFromLocation.Code);
					//}
					//else if (this.IncludeAllItursWithPrefix == true)
					//{
					//	itursAll = ItursWithPrefix(this._textPrefix);
					//}
					//else if (this.IsPrefixAndNumber == true)
					//{
					//	itursAll = this.ItursWithPrefixAndNumber(this._plusPrefix, this._textValuePlusPrefix);
					//}
					//else
					//{
					//	itursAll = _iturs;
					//}

					itursAll = this._itursSelectedList;

					Dictionary<string, Itur> iturDictionary = this._iturRepository.GetIturDictionary(base.GetDbPath, true);

					if (itursAll != null)
					{
						foreach (Itur itur in itursAll)
						{
							if (itur == null) continue;
							itur.LocationCode = SelectedLocation.Code;
							itur.Name1 = SelectedLocation.Name;
							iturDictionary[itur.IturCode] = itur;
						}
						//this._iturRepository.DeleteAllIturs(base.GetDbPath);
						this._importIturBlukRepository.ClearIturs(base.GetDbPath);

						List<Itur> iturList = new List<Itur>();
						foreach (KeyValuePair<string, Itur> keyValuePair in iturDictionary)
						{
							if (keyValuePair.Value == null) continue;
							iturList.Add(keyValuePair.Value);
						}

						this._importIturBlukRepository.InsertItursFromList(base.GetDbPath, iturList);
						
					}


					this._eventAggregator.GetEvent<IturLocationChangedEvent>().Publish(
						//new IturLocationChangedEventPayload { Iturs = Iturs.FromEnumerable(this._iturs), Location = SelectedLocation, AllChange = this.IncludeAllIturs }
						new IturLocationChangedEventPayload { Iturs = itursAll, Location = SelectedLocation, AllChange = this.IncludeAllItursFromLocation }
						);
				}
			}
			catch (Exception exc)
			{
				string logmessage = exc.Message;
			}
			finally
			{
				this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
			}
		}

		private Iturs ItursWithPrefixAndNumber(string plusPrefix, string textValuePlusPrefix)
		{
			Iturs itursAll = new Iturs();
			try
			{
				List<int> itursNumberList = CommaDashStringParser.Parse(textValuePlusPrefix);
				if (itursNumberList != null)
				{
					string prefix = plusPrefix.PadLeft(4, '0');
					var itursWithPrefix = this._iturRepository.GetItursByPrefix(prefix, base.GetDbPath);

					List<int> itursNumberListDistinct = itursNumberList.Distinct().ToList();
					using (new CursorWait())
					{
						foreach (int iturNumber in itursNumberListDistinct)
						{
							//var iturs = this._iturRepository.GetItursByNumber(iturNumber, base.GetDbPath);
							//itursAll.AddEnumerableIturs(iturs);
							List<Itur> itursWithPrefixList = itursWithPrefix.Where(x => x.Number == iturNumber).Select(x => x).ToList();
							foreach (Itur itur in itursWithPrefixList)
							{
									itursAll.Add(itur);
							}
						}
					}
				}
			}
			catch { }
			return itursAll;
		}

		private Iturs ItursWithPrefix(string plusPrefix)
		{
			try
			{
				string prefix = plusPrefix.PadLeft(4, '0');
				var itursWithPrefix = this._iturRepository.GetItursByPrefix(prefix, base.GetDbPath);
				return itursWithPrefix;
			}
			catch { }
			return new Iturs();
		}
		private bool OkCommandCanExecuted()
        {
			if (IsLocationAndNumber == true)
			{
				bool can = ((string.IsNullOrWhiteSpace(this._textValue) == false)//*
				&& (this.IsTextValid() == true) //*
				&& (_selectedLocation != null));
				return can; // && _iturs != null && _iturs.Count>0;
			}
			else if (IsPrefixAndNumber == true)
			{
				bool can = ((string.IsNullOrWhiteSpace(this._plusPrefix) == false)//*
				&& (string.IsNullOrWhiteSpace(this._textValuePlusPrefix) == false)
				&& (this.IsTextValuePlusPrefixValid() == true)
				 && (this._plusPrefix.PrefixValidate() == 0)
				 && (_selectedLocation != null));
				
				return can; 
			}
			else if(IncludeAllItursFromLocation == true)
			{
				bool can = ((SelectedAllFromLocation != null)
					&& (_selectedLocation != null));
				return can; //TODO
			}
			else if(IncludeAllItursWithPrefix == true)
			{
				bool can = ((string.IsNullOrWhiteSpace(this._textPrefix) == false)//*
				 && (this._textPrefix.PrefixValidate() == 0)
				 && (_selectedLocation != null));

				return can;
			}
			else if (IsIturCode == true)
			{
				bool can = ((_iturs != null) && (_iturs.Count > 0) && (_selectedLocation != null));
				return can;
			}
			else if (IncludeAllIturs == true)
			{
				bool can = ((_itursSelectedList != null) && (_itursSelectedList.Count > 0) && (_selectedLocation != null));
				return can;
			}
			else
			{
				return false;
			}

		}
    }
}