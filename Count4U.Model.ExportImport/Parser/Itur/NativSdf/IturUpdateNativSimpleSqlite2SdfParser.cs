using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;

namespace Count4U.Model.Count4U
{
	//парсим *.db3 и записываем Count4U.Itur
	// не добавляем Location только статус надо изменить 
	// Пока только для Clalit и Nativ+ такое правило
	public class IturUpdateNativSimpleSqlite2SdfParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;


		public IturUpdateNativSimpleSqlite2SdfParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._iturDictionary = new Dictionary<string, Itur>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// Получение списка Product 
		/// </summary>
		/// <returns></returns>
		
		//public Dictionary<string, Itur> GetIturs(string fromPathFile,
			public IEnumerable<Itur> GetItursEnumerable(string fromPathFile, 
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Itur> iturFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД

			IImportLocationSQLiteADORepository importLocationSQLiteADORepository = _serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			Dictionary<string, LocationMobile> dictionaryLocationMobileWithStatus2And1 = new Dictionary<string, LocationMobile>();
			try
			{
				// источник db3	 WithInvStatus2	   только их надо проверить на подозрение для обновления
				dictionaryLocationMobileWithStatus2And1 = importLocationSQLiteADORepository.GetLocationMobileDictionaryWithStatus1And2(encoding, fromPathFile);
			}
			catch { }

			ITemporaryInventoryRepository temporaryInventoryRepository = this._serviceLocator.GetInstance<ITemporaryInventoryRepository>();
			Dictionary<string, TemporaryInventory> dictionaryTemporaryInsertLocations
			= temporaryInventoryRepository.GetDictionaryTemporaryInventorys(toDBPath, "Location", "INSERT");

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = iturRepository.GetERPIturDictionary(toDBPath);
			}
			catch { }

			ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
			List<string> locations = locationRepository.GetLocationCodeList(toDBPath);
			Dictionary<string, int> dictionaryPrffixIndex = new Dictionary<string, int>();
			foreach (string code in locations)
			{
				dictionaryPrffixIndex[code] = 1;
				int maxNumber = iturRepository.GetMaxNumber(code, toDBPath) ;
				if (maxNumber > 1) dictionaryPrffixIndex[code] = maxNumber;
			}

			foreach (KeyValuePair<string, LocationMobile> keyValueLocation in dictionaryLocationMobileWithStatus2And1)	   // источник db3
			{
				string locationCodeFrom = keyValueLocation.Key;							//2060-10-
				//if (locationCodeFrom.Contains("2600-8210") == true)
				//{
				//	locationCodeFrom = locationCodeFrom.Trim();
				//}
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				//if (locationCodeFrom == "26-030")
				//{
				//	;
				//}

				if (string.IsNullOrWhiteSpace(locationCodeFrom) == true) continue;


				if (dictionaryIturCodeERP.ContainsKey(locationCodeFrom) == true)			//есть в текущей Count4UDB	- update location in  Count4UDB
				{
					Itur itur = dictionaryIturCodeERP[locationCodeFrom];

					//if (itur.InvStatus == 2) continue;	  //уже статус установлен	  - обновлять не надо

					//Update
		
					LocationMobile locationMobile = keyValueLocation.Value;

					string androidDateModified = locationMobile.DateModified;
					DateTime modifyDateMobile = new DateTime(androidDateModified.GetNullableValue<long>().GetValueOrDefault())
						.ConvertFromAndroidTime();
					if (itur.ModifyDate == null) itur.ModifyDate = ModelUtils.GetMinDateTime();
					if (itur.ModifyDate < modifyDateMobile)
					{
						itur.ModifyDate = modifyDateMobile;
						int invStatus = 0;
						bool ret = Int32.TryParse(locationMobile.InvStatus, out invStatus);
						itur.InvStatus = invStatus;
  					}
					iturFromDBDictionary[itur.IturCode] = itur;
				}
			}
			

			//come to end
			IImportIturRepository importIturRepository = _serviceLocator.GetInstance<IImportIturRepository>();
			importIturRepository.ClearIturs(toDBPath);

			//foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)	   // источник db3
			foreach (KeyValuePair<string, Itur> keyValueItur in iturFromDBDictionary)	   // источник db3
			{
				yield return keyValueItur.Value;
			}
		}

		#region IIturParser Members


		//public IEnumerable<Itur> GetItursEnumerable(string fromPathFile,
 			public Dictionary<string, Itur> GetIturs(string fromPathFile,
			Encoding encoding, string[] separators, int countExcludeFirstString, Dictionary<string, Itur> IturFromDBDictionary, Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
		}


		public List<Location> LocationToDBList
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
