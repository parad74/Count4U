using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;

namespace Count4U.Model.Count4U
{	   //парсим *.db3 и записываем Count4U.Location
	// похоже что не используем, потому что Локейшен виртуальный, а на андроиде не добавляются новые локейшены
	public class LocationUpdateMerkavaSqlite2SdfParser : ILocationParser
	{
		private readonly ILog _log;
		private Dictionary<string, Location> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public LocationUpdateMerkavaSqlite2SdfParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
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
			Random rnd = new Random();

			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			IImportLocationSQLiteADORepository importLocationSQLiteADORepository = _serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			Dictionary<string, LocationMobile> dictionaryLocationMobile = new Dictionary<string, LocationMobile>();
			try
			{
				// источник db3
				dictionaryLocationMobile = importLocationSQLiteADORepository.GetLocationMobileDictionary(encoding, fromPathFile);
			}
			catch { }

			ITemporaryInventoryRepository temporaryInventoryRepository = this._serviceLocator.GetInstance<ITemporaryInventoryRepository>();
			Dictionary<string, TemporaryInventory> dictionaryTemporaryInsertLocations
			= temporaryInventoryRepository.GetDictionaryTemporaryInventorys(toDBPath, "Location", "INSERT");

			foreach (KeyValuePair<string, LocationMobile> keyValueLocation in dictionaryLocationMobile)	   // источник db3
			{
				string locationCodeFrom = keyValueLocation.Key;
				if (locationCodeFrom.ToLower() == "locationcode") continue;
				locationCodeFrom = locationCodeFrom.TrimEnd('-');		   //2060-10-
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');

				string[] locationCodes = locationCodeFrom.Split('-');	   // 0		1

				if (locationCodes.Length != 1)
				{
					continue;		  // одинарный код только у локейшена. двойной у итура
				}

				string locationCode = DomainUnknownCode.UnknownLocation;
				LocationMobile locationMobile = keyValueLocation.Value;
				locationCode = locationCodes[0].Trim();

				//Update
				if (locationFromDBDictionary.ContainsKey(locationCode) == true)			//есть в текущей Count4UDB	- update location in  Count4UDB
				{
					Location location = locationFromDBDictionary[locationCode];

					string androidDateModified = locationMobile.DateModified;
					var longAndroidDateModified = androidDateModified.GetNullableValue<long>().GetValueOrDefault();
					DateTime modifyDateMobile1 = new DateTime(longAndroidDateModified);
					DateTime modifyDateMobile = modifyDateMobile1.ConvertFromAndroidTime();			   //получаем c андроид тикис (от  1970)
					if (location.DateModified < modifyDateMobile)
					{
						location.DateModified = modifyDateMobile;
						int invStatus = 0;
						bool ret = Int32.TryParse(locationMobile.InvStatus, out invStatus);
						location.InvStatus = invStatus;
					}

					this._locationDictionary[locationCode] = location;
				}
				//Insert
				else  	//нет в текущей Count4UDB	- add new Location in Count4UDB
				{
					//if (dictionaryTemporaryInsertLocations.ContainsKey(locationCode) == true)						!!!
					//{
						Location newLocation = new Location();
						LocationString newLocationString = new LocationString();
						//Code
						//Desc 
						//Level1Code 
						//Level1Name 
						newLocationString.Code = locationCode;
						newLocationString.Name = locationMobile.Level1Name.CutLength(249);
						newLocationString.Description = locationMobile.Description.CutLength(499);
						newLocationString.Level1 = locationMobile.Level1Code.CutLength(49);
						newLocationString.Name1 = locationMobile.Level1Name.CutLength(249);
						newLocationString.Tag = locationCodeFrom.CutLength(249); ;


						int retBit = newLocation.ValidateError(newLocationString, rnd);
						if (retBit != 0)  //Error
						{
							this._errorBitList.Add(new BitAndRecord
							{
								Bit = retBit,
								Record = locationCodeFrom,
								ErrorType = MessageTypeEnum.Error
							});
							continue;
						}
						else //	Error  retBit == 0 
						{
							retBit = newLocation.ValidateWarning(newLocationString, rnd); //Warning
							if (retBit != 0)
							{
								this._errorBitList.Add(new BitAndRecord
								{
									Bit = retBit,
									Record = newLocation.Code,
									ErrorType = MessageTypeEnum.WarningParser
								});
							}
							//все это гипотетически. Потому как все дублируется в Itur, и из Itur экспортируется назад
							newLocation.LevelNum = 1;			//первый уровень от корня (счет с 1)
							newLocation.NodeType = 2;			//Not Terminal node & Container (for InventProduct)// (Юваль - не конечные)
							int invStatus = 0;
							bool ret = Int32.TryParse(locationMobile.InvStatus, out invStatus);
							newLocation.InvStatus = invStatus;
							newLocation.Disabled = true;

							string androidDateModified = locationMobile.DateModified;
							if (string.IsNullOrWhiteSpace(androidDateModified) == false)
							{
								var longAndroidDateModified = androidDateModified.GetNullableValue<long>().GetValueOrDefault();
								DateTime modifyDateMobile1 = new DateTime(longAndroidDateModified);
								DateTime modifyDateMobile = modifyDateMobile1.ConvertFromAndroidTime();			   //получаем c андроид тикис (от  1970)
								newLocation.DateModified = modifyDateMobile;
							}
							//string androidDateModified = locationMobile.DateModified;
							//DateTime modifyDateMobile = new DateTime(androidDateModified.GetNullableValue<long>().GetValueOrDefault())
							//	.ConvertFromAndroidTime();
							//newLocation.DateModified = modifyDateMobile;
						}
						this._locationDictionary[locationCode] = newLocation;
					}
				//}				 !!!
			}

			if (locationFromDBDictionary.ContainsKey(DomainUnknownCode.UnknownLocation) == false
				&& this._locationDictionary.ContainsKey(DomainUnknownCode.UnknownLocation) == false)
			{
				Location newLocation = new Location();
				newLocation.Code = DomainUnknownCode.UnknownLocation;
				newLocation.Name = DomainUnknownName.UnknownLocation;		//TODO Localization
				newLocation.Description = DomainUnknownCode.UnknownLocation;
				newLocation.BackgroundColor = "255, 12, 255";
				this._locationDictionary[newLocation.Code] = newLocation;
			}

			//come to end
			IImportLocationRepository importIturRepository = _serviceLocator.GetInstance<IImportLocationRepository>();
			importIturRepository.ClearLocations(toDBPath);

			return this._locationDictionary;
		}

	}
}
