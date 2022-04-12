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
	public class IturUpdateNativSqlite2SdfParser : IIturParser
	{				  
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;


		public IturUpdateNativSqlite2SdfParser(IServiceLocator serviceLocator,
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
		public IEnumerable<Itur> GetItursEnumerable(string fromPathFile,
	Encoding encoding, string[] separators,
	int countExcludeFirstString,
	Dictionary<string, Itur> iturFromDBDictionary,
	Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД

			//IImportLocationSQLiteADORepository importLocationSQLiteADORepository = _serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			//Dictionary<string, LocationMobile> dictionaryLocationMobile = new Dictionary<string, LocationMobile>();
			//try
			//{
			//	// источник db3
			//	dictionaryLocationMobile = importLocationSQLiteADORepository.GetLocationMobileDictionary(encoding, fromPathFile);
			//}
			//catch { }

			IImportLocationSQLiteADORepository importLocationSQLiteADORepository = _serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			//Dictionary<string, LocationMobile> dictionaryLocationMobile = new Dictionary<string, LocationMobile>();
			Dictionary<string, LocationMobile> dictionaryLocationMobileWithStatus2And1 = new Dictionary<string, LocationMobile>();
			try
			{
				// источник db3	 WithInvStatus2	   только их надо проверить на подозрение для обновления
				dictionaryLocationMobileWithStatus2And1 = importLocationSQLiteADORepository.GetLocationMobileDictionaryWithStatus1And2(encoding, fromPathFile);
				//убрала 18/10/2018 вернула		  dictionaryLocationMobileWithStatus2And1
				//dictionaryLocationMobile = importLocationSQLiteADORepository.GetLocationMobileDictionary(encoding, fromPathFile);
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

			//убрала 18/10/2018		заменила  dictionaryLocationMobileWithStatus2And1
			//foreach (KeyValuePair<string, LocationMobile> keyValueLocation in dictionaryLocationMobile)	   // источник db3
			foreach (KeyValuePair<string, LocationMobile> keyValueLocation in dictionaryLocationMobileWithStatus2And1)	   // источник db3
			{
				
				string locationCodeFrom = keyValueLocation.Key;							//2060-10-

				//if (locationCodeFrom == "5060-5081-11")
				//{
				//	locationCodeFrom = locationCodeFrom;
				//}
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');

				if (string.IsNullOrWhiteSpace(locationCodeFrom) == true) continue;

				//string[] locationCodes = locationCodeFrom.Split('-');				   // 0	
				//if (locationCodes.Length == 0) continue;
				//string locationCode = DomainUnknownCode.UnknownLocation;

				//int deepNode = locationCodes.Length;
				//locationCode = locationCodes[0].Trim();
				//locationCode = locationCode.CutLength(49);

				//if (string.IsNullOrWhiteSpace(locationCode) == true) continue;
				//if (locationCode.Trim().ToLower() == "locationcode") continue;

				if (dictionaryIturCodeERP.ContainsKey(locationCodeFrom) == true)			//есть в текущей Count4UDB	- update itur in  Count4UDB
				{
					Itur itur = dictionaryIturCodeERP[locationCodeFrom];

					LocationMobile locationMobile = keyValueLocation.Value;
					//Update
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
				else	//нет в текущей Count4UDB	- itur in  Count4UDB
				{
					if (dictionaryTemporaryInsertLocations.ContainsKey(locationCodeFrom) == true)	   // new from db3
					{
						//Insert
						string iturCodeERP = this.GetIturCodeERP(locationCodeFrom);
						string[] locationCodes = iturCodeERP.Split('-');
						for (int i = 0; i < locationCodes.Length; i++)
						{
							if (string.IsNullOrWhiteSpace(locationCodes[i]) == true)
							{
								locationCodes[i] = "unknown";
							}
						}
						if (locationCodes.Length > 0)
						{
							int deepNode = locationCodes.Length;
							string locationCode = this.GetLocationCode(locationCodes);
							if (string.IsNullOrWhiteSpace(locationCode) == false)
							{
								if (string.IsNullOrWhiteSpace(iturCodeERP) == false)
								{
									//Insert
									//add new Itur in Count4UDB
									Itur tempItur = GetNewIturCode(toDBPath, dictionaryPrffixIndex,
									iturCodeERP, locationCode, deepNode);																  //base

									Itur newItur = new Itur();
									IturString newIturString = new IturString();
									newIturString.IturCode = tempItur.IturCode;
									newIturString.ERPIturCode = iturCodeERP;
									newIturString.LocationCode = locationCode;
									newIturString.StatusIturBit = "0";
									if (deepNode == 2) newIturString.Name = locationCodes[1];
									if (deepNode == 3) newIturString.Name = locationCodes[2];
									int retBit = newItur.ValidateError(newIturString, this._dtfi);
									if (retBit != 0)  //Error
									{
										this._errorBitList.Add(new BitAndRecord
										{
											Bit = retBit,
											Record = iturCodeERP,
											ErrorType = MessageTypeEnum.Error
										});
									}
									else //	Error  retBit == 0 
									{
										retBit = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
										newItur.LevelNum = deepNode;
										newItur.Disabled = false;
										if (deepNode == 1)		//root 
										{
										}
										if (deepNode == 2)			//root +1
										{
											newItur.ParentIturCode = locationCodes[0];// newItur.Level1;
										}
										if (deepNode == 3)		  //root +2
										{
											newItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1];//newItur.Level1 + "-" + newItur.Level2;
										}
										if (deepNode == 4)		  //root +3
										{
											newItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1] + "-" + locationCodes[2];//newItur.Level1 + "-" + newItur.Level2;
										}
										if (iturCodeERP.Contains("789-25_08_2020_19_19_31_47e5-25_08_2020_19_19_48_47e5") == true)
										{
											;
										}
										LocationMobile locationMobileByCode = importLocationSQLiteADORepository.GetLocationMobileByLocationCode(iturCodeERP, fromPathFile);
										if (locationMobileByCode != null)
										{
											int invSts = 0;
											bool ret = Int32.TryParse(locationMobileByCode.InvStatus, out invSts);
											newItur.InvStatus = invSts;
											newItur.Description = locationMobileByCode.Description != null ? locationMobileByCode.Description : "";
											newItur.Level1 = locationMobileByCode.Level1Code != null ? locationMobileByCode.Level1Code : "";
											newItur.Name1 = locationMobileByCode.Level1Name != null ? locationMobileByCode.Level1Name : "";
											newItur.Level2 = locationMobileByCode.Level2Code != null ? locationMobileByCode.Level2Code : "";
											newItur.Name2 = locationMobileByCode.Level2Name != null ? locationMobileByCode.Level2Name : "";
											newItur.Level3 = locationMobileByCode.Level3Code != null ? locationMobileByCode.Level3Code : "";
											newItur.Name3 = locationMobileByCode.Level3Name != null ? locationMobileByCode.Level3Name : "";
											int nodeType = 0;
											ret = Int32.TryParse(locationMobileByCode.NodeType, out nodeType);
											newItur.NodeType = nodeType;

											iturFromDBDictionary[newItur.IturCode] = newItur;
										}
										else
										{
											this._errorBitList.Add(new BitAndRecord
											{
												Bit = 64,
												Record = iturCodeERP,
												ErrorType = MessageTypeEnum.Error
											});
										}
									}
								}
							}
						}
					}
				}
			}

			//come to end
			IImportIturRepository importIturRepository = _serviceLocator.GetInstance<IImportIturRepository>();
			importIturRepository.ClearIturs(toDBPath);

			foreach (KeyValuePair<string, Itur> keyValueItur in iturFromDBDictionary)	   // источник db3
			{
				yield return keyValueItur.Value;
			}
		}

		private string GetLocationCode(string[] locationCodes)
		{
			string locationCode = "";
			locationCode = locationCodes[0].Trim();
			locationCode = locationCode.CutLength(49);
			if (locationCode.Trim().ToLower() == "locationcode") locationCode = "";
			return locationCode;
		}

		private string GetIturCodeERP(string locationCodeFrom)
		{
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');

			string iturCodeERP = locationCodeFrom.CutLength(249);
			return iturCodeERP;
		}

		private Itur GetNewIturCode(string toDBPath, Dictionary<string, int> dictionaryPrffixIndex, string iturCodeERP, string locationCode, int deepNode)
		{
			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Itur tempItur = new Itur();
			if (dictionaryPrffixIndex.ContainsKey(locationCode) == false)
			{
				dictionaryPrffixIndex[locationCode] = 1;	   //добавляем новый счетчик специально для локейшена (считаем намбер для суффикса)
				int maxNumber = iturRepository.GetMaxNumber(locationCode, toDBPath);
				if (maxNumber > 1) dictionaryPrffixIndex[locationCode] = maxNumber;
			}
			//================================
			string prefix = locationCode;
			string suffix = "";
			if (deepNode == 1)
			{
				suffix = "1";
			}
			else
			{
				int lastIndex = dictionaryPrffixIndex[locationCode];
				lastIndex++;
				suffix = lastIndex.ToString();
				dictionaryPrffixIndex[locationCode] = lastIndex;
			}


			int num = 0;
			bool ret = Int32.TryParse(suffix.TrimStart('0'), out num);
			tempItur.Number = num;
			tempItur.NumberPrefix = prefix.PadLeft(4, '0');
			tempItur.NumberSufix = suffix.ToString().PadLeft(4, '0');
			string newIturCode = tempItur.NumberPrefix + tempItur.NumberSufix;
			tempItur.IturCode = newIturCode;
			return tempItur;
		}

		#region IIturParser Members

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
