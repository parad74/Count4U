using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{	//парсим ImportInfrastructure.Xslx и записываем Count4U.Location
	public class LocationNativXslx2SdfParser : ILocationParser
	{
		private readonly ILog _log;
		private Dictionary<string, Location> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public LocationNativXslx2SdfParser(IServiceLocator serviceLocator,
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
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);		

			this._locationDictionary.Clear();
			this._errorBitList.Clear();

			Random rnd = new Random();
			string separator = " ";
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


			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{
				if (record == null)	continue;

				if (record.Length < 4)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				String[] recordEmpty = { "", "", "", "", "", ""};

				Location newLocation = new Location();
					LocationString newLocationString = new LocationString();
					int count = 6;
					if (record.Count() < 6)
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

					string locationCodeFrom = recordEmpty[0].Trim();	  //2060-10-
					locationCodeFrom = locationCodeFrom.TrimEnd('-');
					locationCodeFrom = locationCodeFrom.TrimEnd('-');
					locationCodeFrom = locationCodeFrom.TrimEnd('-');
					if (string.IsNullOrWhiteSpace(locationCodeFrom) == true) continue;
					//if (locationFromDBDictionary.ContainsKey() == true) continue;

					string[] locationCodes = locationCodeFrom.Split('-');				   // 0		1

					if (locationCodes.Length < 1)	continue;

					string locationCode = DomainUnknownCode.UnknownLocation;
					locationCode = locationCodes[0].Trim();

					newLocationString.Code = locationCode.CutLength(49);
					string name = recordEmpty[3].CutLength(49);
					newLocationString.Description = recordEmpty[1].CutLength(499);//Full Desc;
					newLocationString.Name = name.ReverseDosHebrew(invertLetter, rt2lf);
					newLocationString.Level1 = recordEmpty[2].CutLength(49);
					newLocationString.Name1 = recordEmpty[3].CutLength(249);
					//newLocationString.Level2 = recordEmpty[4];
					//newLocationString.Name2 = recordEmpty[5];
					//newLocationString.Level3 = recordEmpty[6];
					//newLocationString.Name3 = recordEmpty[7];
					//newLocationString.Name4 = "";
					newLocationString.Tag = recordEmpty[0].CutLength(99); ;

					//LocationCode 0
					//LocationName 1
					//Level1Code 2
					//Level1Name 3
					//Level2Code 4
					//Level2Name 5
				
					//if (record.Length > 4)
					//{
					//	newLocationString.Tag = recordEmpty[4];
					//}

					int retBit = newLocation.ValidateError(newLocationString, rnd);
					if (retBit != 0)  //Error
					{
						//this.IsDictionaryContainsKey(newItur.Code, iturFromDBDictionary, record.JoinRecord(separator));	   //?
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
						retBit = newLocation.ValidateWarning(newLocationString, rnd); //Warning
						 //все это гипотетически. Потому как все дублируется в Itur, и из Itur экспортируется назад
						newLocation.LevelNum = 1;			//первый уровень от корня (счет с 1)
						newLocation.NodeType = 2;			//Not Terminal node & Container (for InventProduct)// (Юваль - не конечные)
						newLocation.InvStatus = 0;
						newLocation.Disabled = true;  
						//newLocation.DateModified =		???
					
						if (locationFromDBDictionary.ContainsKey(newLocation.Code) == false)
						{
							this._locationDictionary[newLocation.Code] = newLocation;
							locationFromDBDictionary[newLocation.Code] = null;
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
			return this._locationDictionary;
		}

	}
}
