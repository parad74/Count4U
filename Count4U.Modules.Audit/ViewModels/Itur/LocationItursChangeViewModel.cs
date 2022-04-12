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
using System.Windows.Media;
using System.ComponentModel;

namespace Count4U.Modules.Audit.ViewModels
{
	public class LocationItursChangeViewModel : CBIContextBaseViewModel, IDataErrorInfo
	{
		private static readonly Random _rnd = new Random();

		private readonly ILocationRepository _locationRepository;
		private readonly IIturRepository _iturRepository;
		private readonly IEventAggregator _eventAggregator;
		private readonly IImportIturBlukRepository _importIturBlukRepository;

		private readonly DelegateCommand _cancelCommand;
		private readonly DelegateCommand _okCommand;
		private Iturs _iturs;

		private Location _selectedLocation;
		//private Locations _locationsTo;
		private Locations _locations;

		private string _code;
		private string _name;
		private string _description;
		private string _tag;
		private Color _color;
		private Location _location;

		private string _filterLocation;

		private string _iturList = String.Empty;

		private Iturs _itursSelectedList;
		private Locations _locationsAll;

	
		private string _textValuePlusPrefix;
		private string _plusPrefix;
		private bool _isOldLocation;
		private bool _isNewLocation;


		public LocationItursChangeViewModel(IContextCBIRepository repository,
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
			this._isOldLocation = false;
			this._isNewLocation = true;
			this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
			this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
			this._locationsAll = new Locations();
		}

		public string Code
		{
			get { return this._code; }
			set
			{
				this._code = value;
				RaisePropertyChanged(() => Code);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public string Name
		{
			get { return this._name; }
			set
			{
				this._name = value;
				RaisePropertyChanged(() => Name);
				this._okCommand.RaiseCanExecuteChanged();
			}
		}

	
		public string Error
		{
			get { return String.Empty; }
		}

		bool IsLocationCodeUnique()
		{
			if (this._isNewLocation)
				return !this._locations.Any(r => r.Code.ToUpper() == this.Code.ToUpper());

			return true;
		}

		public bool IsReadNewLocation //*	  IsReadOnly
		{
			get
			{
				if (IsNewLocation == true)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		public bool IsReadOldLocation //*	  IsReadOnly
		{
			get
			{
				if (IsOldLocation == true)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}


		public bool EnabledLocationsComboBox //*
		{
			get
			{
				if (IsOldLocation == true)
				{
					return true;
				}
				else
				{
					return false;
				}

			}
		}

		public bool IsNewLocation
		{
			get { return _isNewLocation; }
			set
			{
				this._isNewLocation = value;
				if (value == true)                    //!!!
				{
				this._isOldLocation = !value;

					this._iturList = "";
				//	this._textValue = this._inTextValue;
					//this._textPrefix = "";
					//this._plusPrefix = "";
					//this._textValuePlusPrefix = "";
					this._filterLocation = "";

					Location location = new Location();
					location.Name = String.Empty;
					location.Code = GetNextLocationCode();//String.Empty;       //add next number codeLocation
					location.BackgroundColor = String.Format("{0}, {1}, {2}", _rnd.Next(0, 255), _rnd.Next(0, 255), _rnd.Next(0, 255));
					location.Description = String.Empty;
					this._location = location;

					this._name = this._location.Name;
					this._code = this._location.Code;

					this.RaisePropertyChanged(() => this.IsNewLocation);
					this.RaisePropertyChanged(() => this.IsOldLocation);

					//	this.RaisePropertyChanged(() => this.TextValue);
					//	this.RaisePropertyChanged(() => this.TextPrefix);
					//this.RaisePropertyChanged(() => this.PlusPrefix);
					//this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
					//this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);
					this.RaisePropertyChanged(() => this.FilterLocation);
					this.RaisePropertyChanged(() => this.Code);
					this.RaisePropertyChanged(() => this.Name);

					//		this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					//	this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsReadNewLocation);
					this.RaisePropertyChanged(() => this.IsReadOldLocation);
					this.RaisePropertyChanged(() => this.EnabledLocationsComboBox);
					
				//	this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
				//	this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);
				//this.RaisePropertyChanged(() => this.EnabledIncludeAllIturs);
				//this._okCommand.RaiseCanExecuteChanged();
				}

			}
		}


		public bool IsOldLocation
		{
			get { return _isOldLocation; }
			set
			{
				this._isOldLocation = value;
				if (value == true)                    //!!!
				{
					this._isNewLocation = !value;
					this._iturList = "";
					this._code = "";
					this._name = "";
					//this._textValue = "";
					//this._textPrefix = "";
					//	this._plusPrefix = this._inTextPrefix;
					//	this._textValuePlusPrefix = this._inTextValue;
					this.RaisePropertyChanged(() => this.IsNewLocation);
					this.RaisePropertyChanged(() => this.IsOldLocation);
					//this.RaisePropertyChanged(() => this.TextValue);
					//this.RaisePropertyChanged(() => this.TextPrefix);
					//this.RaisePropertyChanged(() => this.PlusPrefix);
					//this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
				//	this.RaisePropertyChanged(() => this.IturList);
					this.RaisePropertyChanged(() => this.TotalIturs);
					this.RaisePropertyChanged(() => this.Code);
					this.RaisePropertyChanged(() => this.Name);

					//	this.RaisePropertyChanged(() => this.IsReadLocationAndNumber);
					//this.RaisePropertyChanged(() => this.IsReadIncludeAllItursWithPrefix);
					this.RaisePropertyChanged(() => this.IsReadPrefixAndNumber);
					this.RaisePropertyChanged(() => this.IsReadNewLocation);
					this.RaisePropertyChanged(() => this.IsReadOldLocation);
					this.RaisePropertyChanged(() => this.EnabledLocationsComboBox);
					this.RaisePropertyChanged(() => FilterLocation);
					//        this.RaisePropertyChanged(() => this.IsEnabledLocationAndNumber);
					//  this.RaisePropertyChanged(() => this.IsEnabledIncludeAllItursFromLocation);
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

		public Locations Locations
		{
			get { return _locations; }
			set
			{
				_locations = value;
				RaisePropertyChanged(() => Locations);
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

		//private Location _selectedAllFromLocation;
		//public Location SelectedAllFromLocation
		//{
		//	get { return _selectedAllFromLocation; }
		//	set
		//	{
		//		_selectedAllFromLocation = value;
		//		this.RaisePropertyChanged(() => SelectedAllFromLocation);
		//	//	this.RaisePropertyChanged(() => this.IturList);
		//		this.RaisePropertyChanged(() => this.TotalIturs);

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}




		//private Location _selectedLocationAndNumber;
		//public Location SelectedLocationAndNumber
		//{
		//	get { return _selectedLocationAndNumber; }
		//	set
		//	{
		//		this._selectedLocationAndNumber = value;
		//		this.RaisePropertyChanged(() => SelectedLocationAndNumber);
		//	//	this.RaisePropertyChanged(() => this.IturList);
		//		this.RaisePropertyChanged(() => this.TotalIturs);

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}


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
					RaisePropertyChanged(() => Locations);
					//RaisePropertyChanged(() => SelectedLocation);
				}
			}
		}

		private void BuildLocationCodeFullList()
		{
			//this._locationsAll.Clear();
			this._locationsAll = this._locationRepository.GetLocations(base.GetDbPath);
			var locs = this._locationsAll.Take(20).ToList().Select(x => x);
			this._locations = Locations.FromEnumerable(locs);
			//if (_locationsAll == null) return;

			string LocationCode = this._locationRepository.GetFistFromAllLocationCodeWithoutIturs(base.GetDbPath);

			if (string.IsNullOrWhiteSpace(LocationCode) == false)
			{
				this._selectedLocation = this._locationRepository.GetLocationByCode(LocationCode, base.GetDbPath);
			}
			else
			{
				this._selectedLocation = this._locationsAll.FirstOrDefault();
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
			this._locations = Locations.FromEnumerable(list);
			if (this._locations == null) return;

			string LocationCode = this._locationRepository.GetFistLocationCodeWithoutIturs(this._locations, base.GetDbPath);

			if (string.IsNullOrWhiteSpace(LocationCode) == false)
			{
				this.SelectedLocation = this._locationRepository.GetLocationByCode(LocationCode, base.GetDbPath);
			}
			else
			{
				if (this._locations.Count() > 0)
				{
					this._selectedLocation = this._locations.FirstOrDefault();
				}
				else
				{
					this._selectedLocation = null;
				}
			}

			

		}

		
		

		public string PlusPrefix//*										   //TODO
		{
			get { return this._plusPrefix; }
			set
			{
				//if (IsOldLocation == true)
				//{
					this._plusPrefix = value;
					//this._inPlusPrefix = value;
					this._iturList = "";
				//}
				//else
				//{
				//	this._plusPrefix = "";
				//}

				this.RaisePropertyChanged(() => this.PlusPrefix);
					this.RaisePropertyChanged(() => this.TotalIturs);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public string TextValuePlusPrefix//*
		{
			get { return this._textValuePlusPrefix; }
			set
			{
				//if (IsOldLocation == true)
				//{
					this._textValuePlusPrefix = value;
					//this._inTextValue = value;
					//this._textValue = "";
					this._iturList = "";
				//}
				//else
				//{
				//	this._textValuePlusPrefix = "";
				//}

				this.RaisePropertyChanged(() => this.TextValuePlusPrefix);
				//this.RaisePropertyChanged(() => this.TextValue);

			//	this.RaisePropertyChanged(() => this.IturList);
				this.RaisePropertyChanged(() => this.TotalIturs);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		private void RefeshItursSelectedList()
		{
			this._itursSelectedList = null;
			this._itursSelectedList = this.ItursWithPrefixAndNumber(this._plusPrefix, this._textValuePlusPrefix);

			//if (IsNewLocation == true)
			//{
			//	//if (this._selectedLocationAndNumber != null)
			//	//	this._itursSelectedList = this.ItursWithLocationAndNumber(this._selectedLocationAndNumber.Code, this._textValue);
			//	this._itursSelectedList = this.ItursWithPrefixAndNumber(this._plusPrefix, this._textValuePlusPrefix);
			//}
			////else if (IncludeAllIturs == true)
			////{
			////	this._itursSelectedList = this.AllIturs();
			////}
			////else if (IncludeAllItursFromLocation == true)
			////{
			////	if (this._selectedAllFromLocation != null)
			////		this._itursSelectedList = ItursWithLocationCode(this._selectedAllFromLocation.Code);
			////}
			////else if (IncludeAllItursWithPrefix == true)              //TODO
			////{
			////	this._itursSelectedList = ItursWithPrefix(this._textPrefix);
			////}
			//else if (IsOldLocation == true)                         //TODO
			//{
			//	this._itursSelectedList = this.ItursWithPrefixAndNumber(this._plusPrefix, this._textValuePlusPrefix);
			//}
			//else
			//{
			//	this._itursSelectedList = _iturs;
			//}
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

		//private bool IsTextValid() //*
		//{
		//	return CommaDashStringParser.IsValid(this._textValue);
		//}

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
					case "Code":
						{
							if (IsNewLocation == true)
							{
								string validation = LocationValidate.CodeValidate(this._code);
								if (!String.IsNullOrEmpty(validation))
									return validation;

								if (!this.IsLocationCodeUnique())
									return Model.ValidateMessage.Location.CodeExist;
							}
							else return "";
							break;
						}
					case "Name":
						{
						if (IsNewLocation == true)
						{
								string validation = LocationValidate.NameValidate(this._name);

							if (!String.IsNullOrEmpty(validation))
								return validation;
							}
							else return "";
							break;
						}
					//case "TextValue":
					//	{
					//		if (this.IsTextValid() == false)
					//		{
					//			return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
					//		}
					//	}

					//	break;
					case "TextValuePlusPrefix":
						{
							if (this.IsTextValuePlusPrefixValid() == false)
							{
								return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
							}
						}

						break;
					//case "TextPrefix":
					//	{
					//		int bit = this._textPrefix.PrefixValidate();
					//		if (bit != 0)
					//		{
					//			return IturValidate.Bit2PrefixErrorMessageWitoutNewLine(bit);
					//		}
					//	}
						//if (IsPrefixValid() == false)
						//{
						//	return "Prefix is not empty numeric string 4 characters maximum";
						//}

					//	break;

					case "PlusPrefix":
						{
							int bit = this._plusPrefix.PrefixValidate();
							if (bit != 0)
							{
								return IturValidate.Bit2PrefixErrorMessageWitoutNewLine(bit);
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
			if (String.IsNullOrEmpty(this._plusPrefix))
				return true;
			return this._plusPrefix.Length <= 4 &&
				   Regex.IsMatch(this._plusPrefix, @"^[0-9]+$");
		}

		
		public bool IsReadPrefixAndNumber //*	  IsReadOnly
		{
			get
			{
			return false;
			}
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
					//string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
					//this._inIturCodes = commaSeparatedNumbers;
					//this._iturList = _inIturCodes;


					//string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();
					this._isNewLocation = true;
					this._isOldLocation = false;

					List<int> itursNumbers = new List<int>();
					Dictionary<string, Itur> iturDictionary = this._iturRepository.GetIturDictionary(base.GetDbPath, true);
					Dictionary<string, Location> locationDictionary = this._locationRepository.GetLocationDictionary(base.GetDbPath, true);

					this._location = new Location();
					this._location.Name = String.Empty;
					this._location.Code = String.Empty;
					this._location.BackgroundColor = String.Format("{0}, {1}, {2}", _rnd.Next(0, 255), _rnd.Next(0, 255), _rnd.Next(0, 255));
					this._location.Description = String.Empty;

					this._iturs = new Iturs();

					if (iturDictionary.Count > 0)
					{
						try
						{
							string prefix = iturDictionary.Values.First().NumberPrefix;
							this._plusPrefix = prefix;
						}
						catch { }
					}
				
					//foreach (string iturCode in split)
					//{
					//	Itur itur = null;
					//	iturDictionary.TryGetValue(iturCode, out itur);
					//	if (itur != null)
					//	{
					//		this._inTextPrefix = itur.NumberPrefix;
					//		//this._inPlusPrefix = itur.NumberPrefix;

					//		//this._inLocationCode = itur.LocationCode;
					//		//Location location = this._locationRepository.GetLocationByCode(itur.LocationCode, base.GetDbPath);
					//		Location location = null;
					//		locationDictionary.TryGetValue(itur.LocationCode, out location);
					//		if (location != null)
					//		{
					//			if (this._selectedAllFromLocation == null)
					//				this._selectedAllFromLocation = location;
					//			if (this._selectedLocationAndNumber == null)
					//				this._selectedLocationAndNumber = location;
					//		}
					//		itursNumbers.Add(itur.Number);
					//		this._iturs.Add(itur);
					//	}
					//}

					//	this._inTextValue = CommaDashStringParser.Reverse(itursNumbers); //*

					BuildLocationCodeFullList();

					if (this._isNewLocation == true) 
					{
						Location location = new Location();
						location.Name = String.Empty;
						location.Code = GetNextLocationCode();//String.Empty;       //add next number codeLocation

						location.BackgroundColor = String.Format("{0}, {1}, {2}", _rnd.Next(0, 255), _rnd.Next(0, 255), _rnd.Next(0, 255));
						location.Description = String.Empty;

						this._location = location;
					}

					this._name = this._location.Name;
					this._code = this._location.Code;
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


		private string GetNextLocationCode()
		{
			if (_locationsAll == null) return "";
			for (int i = 1; i < 999; i++)
			{
				string newLocationCode = i.ToString().PadLeft(3, '0');
				Location location = _locationsAll.FirstOrDefault(e => e.Code.CompareTo(newLocationCode) == 0);
				if (location == null)
					return newLocationCode;
				else
					continue;
			}

			return "";
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

			this._location.Name = this._name;

			this._location.Description = this._description;
			this._location.Tag = this._tag;
			this._location.BackgroundColor = ColorParser.ColorToString(this._color);

			Location toLocation = new Location();
			if (this.IsNewLocation)
			{
				this._location.Code = this.Code;
				this._location.Name = this.Name;
				this._locationRepository.Insert(this._location, base.GetDbPath);
				toLocation = this._location;
				this._eventAggregator.GetEvent<LocationAddedEvent>().Publish(this._location);
			}
			else
			{
				toLocation = this.SelectedLocation;
			}

			this.MoveIturs(toLocation);

			if (this.IsNewLocation)
			{
				this._eventAggregator.GetEvent<LocationAddedEvent>().Publish(this._location);
			}

			this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

		}
		private void MoveIturs(Location toLocation)
		{
			if (toLocation == null) return;
			Iturs itursAll = new Iturs();
			try
			{
				using (new CursorWait())
				{
	
					itursAll = this._itursSelectedList;

					Dictionary<string, Itur> iturDictionary = this._iturRepository.GetIturDictionary(base.GetDbPath, true);

					if (itursAll != null)
					{
						foreach (Itur itur in itursAll)
						{
							if (itur == null) continue;
							itur.LocationCode = toLocation.Code;
							itur.Name1 = toLocation.Name;
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
						new IturLocationChangedEventPayload { Iturs = itursAll, Location = toLocation/*, AllChange = this.IncludeAllItursFromLocation*/ }
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
			//return !String.IsNullOrWhiteSpace(_code) &&
			// this.IsLocationCodeUnique() &&
			// !String.IsNullOrWhiteSpace(_name);
			if (IsNewLocation == true)
			{
				bool can = !String.IsNullOrWhiteSpace(_code)
				 && this.IsLocationCodeUnique()
				 && !String.IsNullOrWhiteSpace(_name)
				&& (string.IsNullOrWhiteSpace(this._plusPrefix) == false)
				&& (string.IsNullOrWhiteSpace(this._textValuePlusPrefix) == false)
				&& (this.IsTextValuePlusPrefixValid() == true)
				&& (this._plusPrefix.PrefixValidate() == 0);
				return can; 
			}
			else if (IsOldLocation == true)
			{
				bool can = (_selectedLocation != null)
				 && (string.IsNullOrWhiteSpace(this._plusPrefix) == false)
				&& (string.IsNullOrWhiteSpace(this._textValuePlusPrefix) == false)
				&& (this.IsTextValuePlusPrefixValid() == true)
				 && (this._plusPrefix.PrefixValidate() == 0);
				return can; 
			}
			else
			{
				return false;
			}

		}
    }
}