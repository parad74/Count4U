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
	//пока не используется
	//парсим ImportInfrastructure.Xslx и записываем Count4U.Itur
	public class IturUpdateNativXslx2SdfParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private List<Location> _locationToDBList;
		private IIturRepository _iturRepository;


		public IturUpdateNativXslx2SdfParser(IServiceLocator serviceLocator,
				IIturRepository iturRepository,
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

		public List<Location> LocationToDBList
		{
			get { return this._locationToDBList; }
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
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);		

			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			bool invertLetter = false;
			bool rt2lf = false;

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
			Dictionary<string, LocationMobile> dictionaryLocationMobile = new Dictionary<string, LocationMobile>();
			//Dictionary<string, LocationMobile> dictionaryLocationMobileWithStatus2And1 = new Dictionary<string, LocationMobile>();
			try
			{
				// источник db3	 WithInvStatus2	   только их надо проверить на подозрение для обновления
				//dictionaryLocationMobileWithStatus2And1 = importLocationSQLiteADORepository.GetLocationMobileDictionaryWithStatus1And2(encoding, fromPathFile);
				dictionaryLocationMobile = importLocationSQLiteADORepository.GetLocationMobileDictionary(encoding, fromPathFile);
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

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
		encoding, separators,
		countExcludeFirstString,
		sheetNameXlsx,
		sheetNumberXlsx))
			{
				if (record == null) continue;
				if (record.Length < 6)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				String[] recordEmpty = { "", "", "", "", "", "", "", "", "", "" };
				int count = 10;
				if (record.Count() < 10)
				{
					count = record.Count();
				}
				for (int i = 0; i < count; i++)
				{
					recordEmpty[i] = record[i].Trim();
				}
				//LocationCode 0
				//LocationName 1
				//Level1Code 2
				//Level1Name 3
				//Level2Code 4
				//Level2Name 5
				//Level3Code 6
				//Level3Name 7
				//Level4Code 8
				//Level4Name 9

				string locationCodeFrom = recordEmpty[0].Trim();	  //2060-10-
				string iturCodeERP = this.GetIturCodeERP(locationCodeFrom);			//base
		
				if (string.IsNullOrWhiteSpace(iturCodeERP) == true) continue;
				//////////////////////////////////
			//	string locationCodeFrom = keyValueLocation.Key;							//2060-10-

				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');
				locationCodeFrom = locationCodeFrom.TrimEnd('-');

				if (string.IsNullOrWhiteSpace(locationCodeFrom) == true) continue;

				if (dictionaryIturCodeERP.ContainsKey(locationCodeFrom) == true)			//есть в текущей Count4UDB	- update itur in  Count4UDB
				{
					Itur itur = dictionaryIturCodeERP[locationCodeFrom];

					//LocationMobile locationMobile = keyValueLocation.Value;
					////Update
					//string androidDateModified = locationMobile.DateModified;
					//DateTime modifyDateMobile = new DateTime(androidDateModified.GetNullableValue<long>().GetValueOrDefault())
					//	.ConvertFromAndroidTime();
					//if (itur.ModifyDate == null) itur.ModifyDate = ModelUtils.GetMinDateTime();
					//if (itur.ModifyDate < modifyDateMobile)
					//{
					//	itur.ModifyDate = modifyDateMobile;
					//	int invStatus = 0;
					//	bool ret = Int32.TryParse(locationMobile.InvStatus, out invStatus);
					//	itur.InvStatus = invStatus;
					//}
					//iturFromDBDictionary[itur.IturCode] = itur;
				}
				else	//нет в текущей Count4UDB	- itur in  Count4UDB
				{
					if (dictionaryTemporaryInsertLocations.ContainsKey(locationCodeFrom) == true)	   // new from db3
					{
						//Insert
						//string iturCodeERP = this.GetIturCodeERP(locationCodeFrom);
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
									//Level1Name 3
									//Level2Name 5
									//Level3Name 7
									//Level4Name 9

									string name = recordEmpty[5].CutLength(49); //deepNode == 2	 Level2Name
									if (deepNode == 1) name = recordEmpty[3].CutLength(49); 	 //Level1Name
									if (deepNode == 3) name = recordEmpty[7].CutLength(49);	  //Level3Name
									if (deepNode == 4) name = recordEmpty[9].CutLength(49);	  //Level4Name
									newIturString.Name = name;

									//if (deepNode == 2) newIturString.Name = locationCodes[1];
									//if (deepNode == 3) newIturString.Name = locationCodes[2];
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
										newItur.InvStatus = 0;
										newItur.LevelNum = deepNode;

										newItur.Disabled = false;//Contaner изначально предполагаем что все  контейнеры
										newItur.NodeType = 0;  //0 = Terminal Node  - изначально предполагаем что все терминальный
	
										newItur.Description = recordEmpty[1].CutLength(499);
										newItur.Level1 = recordEmpty[2].CutLength(49);//Level1Code
										newItur.Name1 = recordEmpty[3].CutLength(249);//Level1Name
										newItur.Level2 = recordEmpty[4].CutLength(49);//Level2Code
										newItur.Name2 = recordEmpty[5].CutLength(249);//Level2Name
										newItur.Level3 = recordEmpty[6].CutLength(49);//Level3Code
										newItur.Name3 = recordEmpty[7].CutLength(249);//Level3Name
										newItur.Level4 = recordEmpty[8].CutLength(49);//Level4Code
										newItur.Name4 = recordEmpty[9].CutLength(249);//Level4Name

										//0 = Terminal Node & Container (for InventProduct)
										//1 = Not Terminal Node & Not Container (for InventProduct)
										//2 = Not Terminal node & Container (for InventProduct)
		
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

										//newItur.NodeType = nodeType;
				
										iturFromDBDictionary[newItur.IturCode] = newItur;
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
		

		#endregion
	}
}
