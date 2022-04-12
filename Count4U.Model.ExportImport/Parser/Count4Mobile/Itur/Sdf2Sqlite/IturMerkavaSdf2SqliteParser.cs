using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4Mobile
{
	public class IturMerkavaSdf2SqliteParser : ILocationSQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, LocationMobile> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		public IIturRepository _iturRepository;
		//public ILocationRepository _locationRepository;

		public IturMerkavaSdf2SqliteParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._locationDictionary = new Dictionary<string, LocationMobile>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, LocationMobile> LocationDictionary
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
		public Dictionary<string, LocationMobile> GetLocationMobiles(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
//			Dictionary<string, Itur> IturCount4uDictionary = this._iturRepository.GetIturs(fromPathFile, true);


			foreach (Itur itur in this._iturRepository.GetIturs(fromPathFile))
			{
				if (itur == null) continue;
				//Field1: Code									Field1: Code
				//Field2: FullDesc							 Field2: Description
				//Field3: Level1Code						 Field3: Code
				//Field4: Level1Name						 Field4: Name
				//Field5: Level2Code
				//Field6: Level2Name
				//Field7: Level3Code
				//Field8: Level3Name
				//LevelNum = "1";
				//NodeType = "2";

				LocationMobile newLocation = new LocationMobile();
				string locationCode = itur.ERPIturCode;
				//newLocation.LocationCode = itur.Code;
				//newLocation.Description = itur.Description;
				//newLocation.Level1Code = itur.Code;
				//newLocation.Level1Name = itur.Name;
				newLocation.LevelNum = "1";
				newLocation.NodeType = "2";
			//	newLocation.InvStatus = string.IsNullOrWhiteSpace(itur.Restore) == true ? "0" : itur.Restore; // "0";
				newLocation.Uid = Guid.NewGuid().ToString();
				//if (locationFromDBDictionary.ContainsKey(locationCode) == false)
				//{
				//	this._locationDictionary[locationCode] = newLocation;
				//	locationFromDBDictionary[locationCode] = null;
				//}	
				if (_locationDictionary.ContainsKey(locationCode) == false)
				{
					this._locationDictionary[locationCode] = newLocation;
				}	
			}

			this._iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			foreach (Itur itur in this._iturRepository.GetIturs(fromPathFile))
			{
				if (itur == null) continue;
//Field1: Code										   Field1: IturCodeERP
//Field2: FullDesc								   Field2: Itur Name
//Field3: Level1Code							   Field3: prefix, locationCode
//Field4: Level1Name							  Field4: 
//Field5: Level2Code							  Field5: suffix
//Field6: Level2Name							Field6: Name
//Field7: Level3Code							Field7: 
//Field8: Level4Name							 Field8:
//IturCode = prefix + suffix \\ lenthg =4+4


				LocationMobile newLocation = new LocationMobile();
				string locationCode = itur.ERPIturCode;
				newLocation.LocationCode = locationCode;
				newLocation.Description = itur.NumberPrefix + "-" + itur.Name;
				newLocation.Level1Code = itur.NumberPrefix;
				newLocation.Level1Name = itur.NumberPrefix;
				newLocation.Level2Code = itur.NumberSufix.TrimStart('0');
				newLocation.Level2Name = itur.Name;
				newLocation.LevelNum = "2";
				newLocation.NodeType = "0";
				newLocation.InvStatus = string.IsNullOrWhiteSpace(itur.Restore) == true ? "0" : itur.Restore; // "0";
				newLocation.Uid = Guid.NewGuid().ToString();
				if (locationFromDBDictionary.ContainsKey(locationCode) == false)
				{
					//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
					this._locationDictionary[locationCode] = newLocation;
					locationFromDBDictionary[locationCode] = null;
				}				
			}
	
			return this._locationDictionary;
		}

	}
}
