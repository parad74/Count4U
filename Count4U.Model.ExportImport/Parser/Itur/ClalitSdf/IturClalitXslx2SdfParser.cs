using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{	  //парсим ImportInfrastructure.Xslx и записываем Count4U.Itur 
	public class IturClalitXslx2SdfParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private List<Location> _locationToDBList;
		private IIturRepository _iturRepository;


		public IturClalitXslx2SdfParser(
			IIturRepository iturRepository,
			IServiceLocator serviceLocator,
			ILog log)
		{
			this._iturRepository = iturRepository;
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._iturDictionary = new Dictionary<string, Itur>();
			this._locationToDBList = new List<Location>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
		}

		public List<Location> LocationToDBList
		{
			get { return this._locationToDBList; }
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
			Dictionary<string, Itur> iturFromDBDictionary,	   //ожидается с ключом IturCodeERP
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsm = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsm);
			IFileParser fileParser;
			if (fileXlsm == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelMacrosFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);

			this._iturDictionary.Clear();
			this._locationToDBList.Clear();
			this._errorBitList.Clear();

			string separator = ",";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			bool invertLetter = false;
			bool rt2lf = false;
			if (parms != null)
			{
				if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
				{
					invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
					rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
				}
			}
			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД

			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = this._iturRepository.GetERPIturDictionary(toDBPath);
			}
			catch { }

			ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();

			List<string> locations = locationRepository.GetLocationCodeList(toDBPath);
			Dictionary<string, int> dictionaryPrffixIndex = new Dictionary<string, int>();
			foreach (string code in locations)
			{
				dictionaryPrffixIndex[code] = 1;
				int maxNumber = this._iturRepository.GetMaxNumber(code, toDBPath);
				if (maxNumber > 1) dictionaryPrffixIndex[code] = maxNumber;
			}

			Dictionary<string, Itur> unsureIturErpCode = new Dictionary<string, Itur>();

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


				String[] recordEmpty = { "", "", "", "", ""
														, "", "", "", "", ""
														, "", "", "", "" };
				int count = 8;
				if (record.Count() < 8)
				{
					count = record.Count();
				}
				for (int i = 0; i < count; i++)
				{
					recordEmpty[i] = record[i].Trim();
				}

				//LocationCode = Codes[0] //Split Code ‘-’
				//Prefix = LocationCode
				//suffix suffix = sequence step 1 from 2 
				//if Level2Code is empty suffix = 1
				//IturName = Level#Name
				//Field3: loc_func_location_id			2		 IturCodeERP = loc_func_location_id
				//Field4: full_descr							3		Description
				//Field5: loc_mosad_id					4		1. District(zone)	Level1 
				//Field6: loc_mosad_descr			    5									Name1
				//Field7: loc_building_id				    6		  2. Building	Level2
				//Field8: loc_building_descr		    7							Name2


				//Field11: loc_floor_id					   10		  3. Floor		  Level3
				//Field12: loc_floor_descr			   11							  Name3
				//Field13: loc_room_id			       12		 4. Room		   Level4
				//Field14: loc_room_descr			   13							  Name4

				//1. District(zone) 2. Building 3. Floor 4. Room
				//LevelNum = level from root (counted from 1)
				//NodeType = 0|2 look at table;

				//Field3: loc_func_location_id			2		 IturCodeERP = loc_func_location_id
				string locationCodeFrom = recordEmpty[2].Trim();	  //2060-10-
				string iturCodeERP = this.GetIturCodeERP(locationCodeFrom);				  //base

				if (string.IsNullOrWhiteSpace(locationCodeFrom) == true) continue;

				string[] locationCodes = locationCodeFrom.Split('-');				   // 0		1
				for (int i = 0; i < locationCodes.Length; i++)
				{
					if (string.IsNullOrWhiteSpace(locationCodes[i]) == true)
					{
						locationCodes[i] = "unknown";
					}
				}

				if (locationCodes.Length == 0) continue;

				int deepNode = locationCodes.Length;

				string locationCode = this.GetLocationCode(locationCodes);								//base
				if (string.IsNullOrWhiteSpace(locationCode) == true) continue;

				//Insert
				//нет в текущей Count4UDB	- add new Itur in Count4UDB

				Itur tempItur = this.GetNewIturCode(toDBPath, dictionaryPrffixIndex,
					iturCodeERP, locationCode, deepNode, recordEmpty );				

				//if (dictionaryPrffixIndex.ContainsKey(locationCode) == false)
				//{
				//	dictionaryPrffixIndex[locationCode] = 1;	   //добавляем новый счетчик специально для локейшена (считаем намбер для суффикса)
				//	int maxNumber = iturRepository.GetMaxNumber(locationCode, toDBPath);
				//	if (maxNumber > 1) dictionaryPrffixIndex[locationCode] = maxNumber;

				//	//============= ADD NEW LOCATION
				//	//Также добавляем в словарь новй локейшен
				//	{
				//		Location newLocation = new Location();
				//		newLocation.Code = locationCode.CutLength(49);

				//		//Field3: loc_func_location_id		2		Tag = loc_func_location_id			LocationCode = loc_func_location_id [0] //Split Code ‘-’
				//		//Field4: full_descr						3		Description = full_descr
				//		//Field5: loc_mosad_id				4		Level1 = loc_mosad_id
				//		//Field6: loc_mosad_descr			5		LocationName = loc_mosad_descr 	Name1 = loc_mosad_descr

				//		newLocation.Name = recordEmpty[5].CutLength(49);
				//		newLocation.Description = recordEmpty[3].CutLength(499);//Full Desc;
				//		newLocation.Level1 = recordEmpty[4].CutLength(49);
				//		newLocation.Name1 = recordEmpty[5].CutLength(449);
				//		newLocation.Tag = recordEmpty[2].CutLength(99);
				//		this._locationToDBList.Add(newLocation);
				//	}
				//}

				//===============================
				//string prefix = locationCode;
				//string suffix = "";
				//if (deepNode == 1)
				//{
				//	suffix = "1";
				//}
				//else
				//{
				//	int lastIndex = dictionaryPrffixIndex[locationCode];
				//	lastIndex++;
				//	suffix = lastIndex.ToString();
				//	dictionaryPrffixIndex[locationCode] = lastIndex;
				//}

				//string iturCodeERP = locationCodeFrom.CutLength(249);
				//string newIturCode = iturCodeERP;
				//newIturCode = prefix.PadLeft(4, '0') + suffix.ToString().PadLeft(4, '0');
				//============
			
				Itur newItur = new Itur();
				IturString newIturString = new IturString();
				newIturString.IturCode = tempItur.IturCode;
				newIturString.ERPIturCode = iturCodeERP;
				newIturString.LocationCode = locationCode;
				newIturString.StatusIturBit = "0";
				newIturString.Name = recordEmpty[5].CutLength(49);

				//IturName = Level#Name
				//Field3: loc_func_location_id			2		 IturCodeERP = loc_func_location_id
				//Field4: full_descr							3		Description
				//Field5: loc_mosad_id					4		1. District(zone)	Level1 
				//Field6: loc_mosad_descr			    5									Name1
				//Field7: loc_building_id				    6		  2. Building	Level2
				//Field8: loc_building_descr		    7							Name2


				//Field11: loc_floor_id					   10		  3. Floor		  Level3
				//Field12: loc_floor_descr			   11							  Name3
				//Field13: loc_room_id			       12		 4. Room		   Level4
				//Field14: loc_room_descr			   13							  Name4
				string name = recordEmpty[3].CutLength(49); //	 full_descr
				if (deepNode == 1) name = recordEmpty[5].CutLength(49); 	 //Level1Name
				if (deepNode == 2) name = recordEmpty[7].CutLength(49);	  //Level2Name
				if (deepNode == 3) name = recordEmpty[10].CutLength(49); 	 //Level3Name
				if (deepNode == 4) name = recordEmpty[13].CutLength(49);	  //Level4Name
				if (deepNode > 4) name = "unknown" + deepNode.ToString();	  //Level3Name

				newIturString.Name = name;


				int retBit = newItur.ValidateError(newIturString, this._dtfi);
				if (retBit != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord
					{
						Bit = retBit,
						Record = record.JoinRecord(separator),
						ErrorType = MessageTypeEnum.Error
					});
					continue;
				}
				else //	Error  retBit == 0 
				{
					retBit = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
					if (iturFromDBDictionary.IsDictionaryContainsKey(newItur.ERPIturCode) == false)
					{
						//Field3: loc_func_location_id			2		 IturCodeERP = loc_func_location_id
						//Field4: full_descr							3		Description
						//Field5: loc_mosad_id					4		1. District(zone)	Level1 
						//Field6: loc_mosad_descr			    5									Name1
						//Field7: loc_building_id				    6		  2. Building	Level2
						//Field8: loc_building_descr		    7							Name2


						//Field11: loc_floor_id					   10		  3. Floor		  Level3
						//Field12: loc_floor_descr			   11							  Name3
						//Field13: loc_room_id			       12		 4. Room		   Level4
						//Field14: loc_room_descr			   13							  Name4

						newItur.InvStatus = 0;
						newItur.LevelNum = deepNode;
						newItur.Disabled = false;
						newItur.Description = recordEmpty[3].CutLength(499);
						newItur.Level1 = recordEmpty[4].CutLength(49);//Level1Code
						newItur.Name1 = recordEmpty[5].CutLength(249);//Level1Name
						newItur.Level2 = recordEmpty[6].CutLength(49);//Level2Code
						newItur.Name2 = recordEmpty[7].CutLength(249);//Level2Name
						newItur.Level3 = recordEmpty[10].CutLength(49);//Level3Code
						newItur.Name3 = recordEmpty[11].CutLength(249);//Level3Name
						newItur.Level4 = recordEmpty[12].CutLength(49);//Level4Code
						newItur.Name4 = recordEmpty[13].CutLength(249);//Level4Name
						newItur.Disabled = false;//Contaner изначально предполагаем что все  контейнеры
						newItur.NodeType = 0;  //0 = Terminal Node  - изначально предполагаем что все терминальный
						if (deepNode == 1)		//root 
						{
						}
						else if (deepNode == 2)	 //root +1
						{
							newItur.ParentIturCode = locationCodes[0];// newItur.Level1;
						}
						else if (deepNode == 3)			 //root +2
						{
							newItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1];//newItur.Level1 + "-" + newItur.Level2;
						}
						else  if (deepNode == 4)			//root +3	 
						{
							newItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1] + "-" + locationCodes[2]; //newItur.Level1 + "-" + newItur.Level2 + "-" + newItur.Level3;
						}
						else if (deepNode > 4)			// more 4
						{
							unsureIturErpCode[locationCodeFrom] = newItur;
							string parentIturCode = "";
							for (int i = 0; i < deepNode - 1; i++)
							{
								parentIturCode = parentIturCode + locationCodes[i] + "-";
							}
							parentIturCode = parentIturCode.TrimEnd('-');
							newItur.ParentIturCode = parentIturCode;
						}

						if (deepNode < 4)
						{
							this._iturDictionary[newItur.IturCode] = newItur;
							iturFromDBDictionary[newItur.ERPIturCode] = newItur;
						}
						//this._iturDictionary.AddToDictionary(newItur.Code, newItur, record.JoinRecord(separator), Log);
					}
					if (retBit != 0)
					{
						this._errorBitList.Add(new BitAndRecord
						{
							Bit = retBit,
							Record = record.JoinRecord(separator),
							ErrorType = MessageTypeEnum.WarningParser
						});
					}
				}

			}

			int countUnsure = unsureIturErpCode.Count;

			foreach (KeyValuePair<string, Itur> keyValueItur in iturFromDBDictionary)	   // источник db3
			{
				Itur itur = keyValueItur.Value;
				if (itur == null) continue;

				if (string.IsNullOrWhiteSpace(itur.ParentIturCode) == false)
				{
					if (iturFromDBDictionary.ContainsKey(itur.ParentIturCode) == true)
					{
						Itur parentItur = iturFromDBDictionary[itur.ParentIturCode];			 //Вытаскиваем родительский и eго меняем
						if (parentItur != null) parentItur.NodeType = 2;		//2 = Not Terminal node & Container (for InventProduct)

						this._iturDictionary[parentItur.IturCode] = parentItur; 				   //Записываем в коллекцию другую, которая будет ухожить на запись
					}
					else
					{	//ops Parent не найден надо добавить 
						if (this._iturDictionary.ContainsKey(itur.ParentIturCode) == false)
						{ // Eсли еще не добавили как новый на предыдущих шагах
							Itur newParenItur = new Itur();
							string erpIturCode = itur.ParentIturCode;
							string[] locationCodes = erpIturCode.Split('-');
							int deepNode = locationCodes.Length;
							string locationCode = this.GetLocationCode(locationCodes);
							Itur tempItur = this.GetNewIturCode(toDBPath, dictionaryPrffixIndex,
								erpIturCode, locationCode, deepNode);
							if (deepNode > 0)
							{
								newParenItur.IturCode = tempItur.IturCode;
								newParenItur.Number = tempItur.Number;
								newParenItur.NumberPrefix = tempItur.NumberPrefix;
								newParenItur.NumberSufix = tempItur.NumberSufix;
								newParenItur.LocationCode = locationCode;
								newParenItur.ERPIturCode = itur.ParentIturCode;
								newParenItur.StatusIturBit = 0;
								newParenItur.Name = "Added automatic";
								newParenItur.Disabled = false;
								newParenItur.LevelNum = deepNode;
								if (deepNode == 1)		  //root 	Not Terminal 
								{
									newParenItur.Level1 = locationCodes[0];
									newParenItur.Name1 = "Added automatic";
									newParenItur.NodeType = 2; //Not terminal Contaner
								}
								if (deepNode == 2)			//root + 1 , Not Terminal  
								{
									newParenItur.Level1 = locationCodes[0];
									newParenItur.Level2 = locationCodes[1];
									newParenItur.Name1 = "Added automatic";
									newParenItur.Name2 = "Added automatic";
									newParenItur.ParentIturCode = locationCodes[0];		 //root
									newParenItur.NodeType = 2; 	 //2; 	 //?? менять здесь если добавится 3уровень Terminal Contaner Может быть 2 если станет нетерминальной
								}
								if (deepNode == 3)			//root + 2 Not Terminal  
								{
									newParenItur.Level1 = locationCodes[0];
									newParenItur.Level2 = locationCodes[1];
									newParenItur.Level3 = locationCodes[2];
									newParenItur.Name1 = "Added automatic";
									newParenItur.Name2 = "Added automatic";
									newParenItur.Name3 = "Added automatic";
									newParenItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1]; 	 //root +1
									newParenItur.NodeType = 2; 	 //2; 	 //Not Terminal Contaner
								}

								this._iturDictionary[itur.ParentIturCode] = newParenItur;
							}
						}
					}//ops Parent не найден надо добавить 
				}
			}

			//come to end
			IImportIturRepository importIturRepository = _serviceLocator.GetInstance<IImportIturRepository>();
			importIturRepository.ClearIturs(toDBPath);

			//foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)	   // источник db3
			foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)	   // источник db3
			{
				yield return keyValueItur.Value;
			}
			//yield return newItur;

			locationRepository.Insert(this._locationToDBList, toDBPath);
			//return this._iturDictionary;
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

		private Itur GetNewIturCode(string toDBPath, Dictionary<string, int> dictionaryPrffixIndex,
			string iturCodeERP, string locationCode, int deepNode, String[] recordEmpty = null)
		{
			Itur tempItur = new Itur();
			if (dictionaryPrffixIndex.ContainsKey(locationCode) == false)
			{
				dictionaryPrffixIndex[locationCode] = 1;	   //добавляем новый счетчик специально для локейшена (считаем намбер для суффикса)
				int maxNumber = this._iturRepository.GetMaxNumber(locationCode, toDBPath);
				if (maxNumber > 1) dictionaryPrffixIndex[locationCode] = maxNumber;

				//============= ADD NEW LOCATION
				//Также добавляем в словарь новй локейшен
				if (recordEmpty != null)
				{
					Location newLocation = new Location();
					newLocation.Code = locationCode.CutLength(49);

					//Field3: loc_func_location_id		2		Tag = loc_func_location_id			LocationCode = loc_func_location_id [0] //Split Code ‘-’
					//Field4: full_descr						3		Description = full_descr
					//Field5: loc_mosad_id				4		Level1 = loc_mosad_id
					//Field6: loc_mosad_descr			5		LocationName = loc_mosad_descr 	Name1 = loc_mosad_descr

					newLocation.Name = recordEmpty[5].CutLength(49);
					newLocation.Description = recordEmpty[3].CutLength(499);//Full Desc;
					newLocation.Level1 = recordEmpty[4].CutLength(49);
					newLocation.Name1 = recordEmpty[5].CutLength(449);
					newLocation.Tag = recordEmpty[2].CutLength(99);
					this._locationToDBList.Add(newLocation);
				}
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


		public Dictionary<string, Itur> GetIturs(string fromPathFile, Encoding encoding, string[] separators, int countExcludeFirstString, Dictionary<string, Itur> IturFromDBDictionary, Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
		}

		#endregion
	}


}
