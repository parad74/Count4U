using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class LocationFromDBParser : ILocationParser
	{
		private readonly ILocationRepository _locationRepository;
		private readonly ILog _log;
		private Dictionary<string, Location> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public LocationFromDBParser(ILocationRepository locationRepository,
			ILog log)
		{
			if (locationRepository == null) throw new ArgumentNullException("locationRepository");

			this._locationRepository = locationRepository;
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
		public Dictionary<string, Location> GetLocations(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Location> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._locationDictionary.Clear();
			this._errorBitList.Clear();

			Locations locationsFromDB = this._locationRepository.GetLocations(fromPathFile);
			foreach (var locationFromDB in locationsFromDB)
			{
				if (locationFromDBDictionary.ContainsKey(locationFromDB.Code) == false)
				{
					//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
					locationFromDB.Restore = fromPathFile.CutLength(99);
					this._locationDictionary[locationFromDB.Code] = locationFromDB;
					locationFromDBDictionary[locationFromDB.Code] = null;
				}
			}

			//this._locationDictionary = this._locationRepository.GetLocationDictionary(fromPathFile, true);
			return this._locationDictionary;
		}
	}
}
