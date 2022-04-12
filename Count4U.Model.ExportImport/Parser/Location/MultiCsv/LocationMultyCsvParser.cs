using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Count4U.Model.Common;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class LocationMultyCsvParser : ILocationParser
	{
		private readonly ILocationRepository _locationRepository;
		private readonly IIturRepository _iturRepository;
		private readonly ILog _log;
		private Dictionary<string, Location> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public LocationMultyCsvParser(ILocationRepository locationRepository
			, IIturRepository iturRepository
			,ILog log)
		{
			if (locationRepository == null) throw new ArgumentNullException("locationRepository");
			this._locationRepository = locationRepository;
			this._iturRepository = iturRepository;
			this._log = log;
			this._locationDictionary = new Dictionary<string, Location>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, Location> LocationDictionary
		{
			get { return this._locationDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, Location> GetLocations(string fromPathFile,		   //current Inventor
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Location> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._locationDictionary.Clear();
			this._errorBitList.Clear();
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			//Locations locationsFromDB = this._locationRepository.GetLocations(fromPathFile);

			//============== Location ===============
			Location newLocation = new Location();
			newLocation.Code = DomainUnknownCode.UnknownLocation;
			newLocation.Name = DomainUnknownName.UnknownLocation;
			if (locationFromDBDictionary.ContainsKey(DomainUnknownCode.UnknownLocation) == false)
			{
				this._locationRepository.Insert(newLocation, dbPath);
				//this._locationDictionary[addedFromCode] = newLocation;
				//locationFromDBDictionary[addedFromCode] = null;
			}
			//============== Itur ===============
			Itur newItur = new Itur();
			IturString newIturString = new IturString();
			string addedIturCode = "99999999";
			newIturString.IturCode = addedIturCode;
			newIturString.LocationCode = DomainUnknownCode.UnknownLocation;
			newIturString.StatusIturBit = "0";

			int retBitItur = newItur.ValidateError(newIturString, this._dtfi);
			if (retBitItur == 0)  //Error
			{
				retBitItur = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
			}

			Itur itur = this._iturRepository.GetIturByCode(addedIturCode, dbPath);
			 if (itur == null)
			{
				this._iturRepository.Insert(newItur, dbPath);
			}
			return this._locationDictionary;
		}
	}
}
