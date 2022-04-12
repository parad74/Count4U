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
using Count4U.Model.Common;

namespace Count4U.Model.Count4Mobile
{
	public class LocationMerkavaSdf2SqliteParser : ILocationSQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, LocationMobile> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		public IIturRepository _iturRepository;
		public ILocationRepository _locationRepository;

		public LocationMerkavaSdf2SqliteParser(IServiceLocator serviceLocator,
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
			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			string fromDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД
			Dictionary<string, DocumentHeader> iturInFileDictionary;
			IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
			iturInFileDictionary = documentHeaderRepository.GetIturDocumentCodeDictionary(fromPathFile);
			this._locationDictionary.Clear();
			this._errorBitList.Clear();

			bool includeCurrentInventor = parms.GetBoolValueFromParm(ImportProviderParmEnum.IncludeCurrentInventor);
	
			this._iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			foreach (Itur itur in this._iturRepository.GetIturs(fromPathFile))
			{
				if (itur == null) continue;
//[Location].Code = [Itur].ERPIturCode
//[Location].FullDesc = [Itur].Description
//[Location].Level1Code  = [Itur].Level1
//[Location].Level1Name  = [Itur].Name1
//[Location].Level2Code  = [Itur]. Level2
//[Location].Level2Name  = [Itur].Name2
//[Location].Level3Code  = [Itur].Level3
//[Location].Level3Name  = [Itur].Name3
//[Location].InvStatus= [Itur].InvStatus
//[Location].NodeType= [Itur].NodeType
//[Location].NodeType= [Itur].NodeType
//[Location].LevelNum= [Itur]. LevelNum


				LocationMobile newLocation = new LocationMobile();
				string locationCode = itur.ERPIturCode;
				newLocation.IturCode = itur.IturCode;
				newLocation.LocationCode = itur.ERPIturCode;
				try
				{
					newLocation.Description = itur.Description;
					newLocation.Level1Code = itur.Level1;
					newLocation.Level1Name = itur.Name1;
					newLocation.Level2Code = itur.Level2;
					newLocation.Level2Name = itur.Name2;
					newLocation.Level3Code = itur.Level3;
					newLocation.Level3Name = itur.Name3;
					newLocation.LevelNum = itur.LevelNum.ToString();
					newLocation.NodeType = itur.NodeType.ToString();

					newLocation.Total = "0";
					newLocation.InvStatus = "0" ;
					newLocation.DateModified = "";
					if (includeCurrentInventor == true)
					{
						newLocation.InvStatus = itur.InvStatus == null ? "0" : itur.InvStatus.ToString();

						if (iturInFileDictionary.ContainsKey(itur.IturCode) == true)
						{
							DocumentHeader doc = iturInFileDictionary[itur.IturCode];
							if (doc.Approve == false) newLocation.InvStatus = "1";
							if (doc.Approve == true) newLocation.InvStatus = "2";
							double totalQuantity = inventProductRepository.GetSumQuantityEditByIturCode(itur.IturCode, fromPathFile);
							newLocation.Total = ((int)totalQuantity).ToString();
						}

						string dateModified = itur.ModifyDate == null ? "" : (Convert.ToDateTime(itur.ModifyDate)).ConvertDateTimeToAndroid();
						newLocation.DateModified = dateModified;				// отправляем на андроид тикетсы (текущей дата - 1970) // получаем c андроид тикис (от  1970)  => надо прибавить
 					}
				
				}
				catch 	(Exception ex)
				{
					newLocation.Description = "#ERROR parsing Location#" + ex.Message;
				}

				newLocation.Uid = Guid.NewGuid().ToString();
				if (locationFromDBDictionary.ContainsKey(locationCode) == false)
				{
					this._locationDictionary[locationCode] = newLocation;
					locationFromDBDictionary[locationCode] = null;
				}				
			}

			if (locationFromDBDictionary.ContainsKey("-1") == false)
			{
				this._locationDictionary["-1"] = new LocationMobile { LocationCode = "-1", Description = "NoLocation", Level1Code = "-1", Level1Name = "NoLocation" , NodeType = "0"};
				locationFromDBDictionary["-1"] = null;
			}				
			return this._locationDictionary;
		}

	}
}
