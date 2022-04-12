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
	public class IturMerkavaXslx2SdfParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private List<Location> _locationToDBList;
		private IIturRepository _iturRepository;

		public IturMerkavaXslx2SdfParser(
			IIturRepository iturRepository,
			IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._iturRepository = iturRepository;
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
		//public Dictionary<string, Itur> GetIturs(string fromPathFile,
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
			this._iturDictionary = iturFromDBDictionary;

			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = this._iturRepository.GetERPIturDictionary(toDBPath);
			}	catch{}

			ILocationRepository  locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
			//Dictionary<string, Location> dictionaryLocation = locationRepositorym.GetLocationDictionary(toDBPath, true);
			List<string> locations = locationRepository.GetLocationCodeList(toDBPath);
			Dictionary<string, int> dictionaryPrffixIndex = new Dictionary<string, int>();
			foreach(string code in locations)
			{
				dictionaryPrffixIndex[code] = 1;
				int maxNumber = this._iturRepository.GetMaxNumber(code, toDBPath);
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

				String[] recordEmpty = { "", "", "", "", "", "", "", "" };
				int count = 8;
				if (record.Count() < 8)
				{
					count = record.Count();
				}
				for (int i = 0; i < count; i++)
				{
					recordEmpty[i] = record[i].Trim();
				}
				//Code 0
				//Full Desc 1
				//Level1Code 2
				//Level1Name 3
				//Level2Code 4
				//Level2Name 5
				//Level3Code 6
				//Level3Name 7

				string locationCodeFrom = recordEmpty[0].Trim();	  //2060-10-
				string iturCodeERP = this.GetIturCodeERP(locationCodeFrom);				  //base

				if (string.IsNullOrWhiteSpace(iturCodeERP) == true) continue;
				if (dictionaryIturCodeERP.ContainsKey(iturCodeERP) == true)
				{
					Itur oldItur = dictionaryIturCodeERP[iturCodeERP];
					this._iturDictionary[oldItur.IturCode] = oldItur;
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.IturCodeExistInDB, iturCodeERP));
					continue;
				}

				string[] locationCodes = iturCodeERP.Split('-');
				if (locationCodes == null) continue;
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

				Itur tempItur = GetNewIturCode(toDBPath, dictionaryPrffixIndex,
					iturCodeERP, locationCode, deepNode);																  //base
				if (tempItur == null)
				{
					continue;				//Error
				}

				Itur newItur = new Itur();
				IturString newIturString = new IturString();

				newIturString.IturCode = tempItur.IturCode;
				newIturString.ERPIturCode = iturCodeERP;
				newIturString.LocationCode = locationCode;
				newIturString.StatusIturBit = "0";
				newIturString.Name = recordEmpty[5].CutLength(49);

				//Level1Name 3
				//Level2Name 5
				//Level3Name 7
				string name = recordEmpty[5].CutLength(49); //deepNode == 2	 Level2Name
				if (deepNode == 1) name = recordEmpty[3].CutLength(49); 	 //Level1Name
				if (deepNode == 3) name = recordEmpty[7].CutLength(49);	  //Level3Name
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
					newItur.InvStatus = 0;
					newItur.LevelNum = deepNode;
					newItur.Description = recordEmpty[1].CutLength(499);
					newItur.Level1 = recordEmpty[2].CutLength(49);//Level1Code
					newItur.Name1 = recordEmpty[3].CutLength(249);//Level1Name
					newItur.Level2 = recordEmpty[4].CutLength(49);//Level2Code
					newItur.Name2 = recordEmpty[5].CutLength(249);//Level2Name
					newItur.Level3 = recordEmpty[6].CutLength(49);//Level3Code
					newItur.Name3 = recordEmpty[7].CutLength(249);//Level3Name

					//0 = Terminal Node & Container (for InventProduct)
					//1 = Not Terminal Node & Not Container (for InventProduct)
					//2 = Not Terminal node & Container (for InventProduct)
					newItur.Disabled = false;//Contaner изначально предполагаем что все  контейнеры
					newItur.NodeType = 0;  //0 = Terminal Node  - изначально предполагаем что все терминальный

					this._iturDictionary[newItur.IturCode] = newItur;
					dictionaryIturCodeERP[iturCodeERP] = newItur;
					//this._iturDictionary.AddToDictionary(newItur.Code, newItur, record.JoinRecord(separator), Log);
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
	

			//come to end
			IImportIturRepository importIturRepository = _serviceLocator.GetInstance<IImportIturRepository>();
			importIturRepository.ClearIturs(toDBPath);

	
			foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)	   // источник db3
			{
				yield return keyValueItur.Value;
			}
			
			//return this._iturDictionary;
		}

		private string GetLocationCode(string[] locationCodes)
		{
			string locationCode = "";
			locationCode = locationCodes[0].Trim();
			locationCode = locationCode.CutLength(249);
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
			Itur tempItur = new Itur();
			if (dictionaryPrffixIndex.ContainsKey(locationCode) == false)
			{
				dictionaryPrffixIndex[locationCode] = 1;	   //добавляем новый счетчик специально для локейшена (считаем намбер для суффикса)
				int maxNumber = this._iturRepository.GetMaxNumber(locationCode, toDBPath);
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

		
			int num  =  0;
			bool ret = Int32.TryParse (suffix.TrimStart('0'), out num);
			tempItur.Number = num;
			tempItur.NumberPrefix = prefix.PadLeft(4, '0');
			tempItur.NumberSufix = suffix.ToString().PadLeft(4, '0');
			string newIturCode = tempItur.NumberPrefix + tempItur.NumberSufix;
			tempItur.IturCode = newIturCode;
			return tempItur;
		}

		#region IIturParser Members


		//public IEnumerable<Itur> GetItursEnumerable(string fromPathFile, 
		public Dictionary<string, Itur> GetIturs(string fromPathFile,
			Encoding encoding, string[] separators, int countExcludeFirstString, Dictionary<string, Itur> IturFromDBDictionary, Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
		}

		#endregion

	}


}
