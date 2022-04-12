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
	//не используется перенесено в  IturClalitXslx2SdfParser
	// большая входящая таблица, чтобы был один проход
	public class LocationClalitXslx2SdfParser : ILocationParser
	{
		private readonly ILog _log;
		private Dictionary<string, Location> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public LocationClalitXslx2SdfParser(IServiceLocator serviceLocator,
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
			bool fileXlsm = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsm);
			IFileParser fileParser;
			if (fileXlsm == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelMacrosFileParser.ToString()); }
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

				String[] recordEmpty = { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

				Location newLocation = new Location();
					LocationString newLocationString = new LocationString();
					int count = 14;
					if (record.Count() < 14)
					{
						count = record.Count();
					}
				 
					for (int i = 0; i < count; i++)
					{
						recordEmpty[i] = record[i].Trim();
					}

					string locationCodeFrom = recordEmpty[2].Trim();	  //2060-10-
					locationCodeFrom = locationCodeFrom.TrimEnd('-');
					locationCodeFrom = locationCodeFrom.TrimEnd('-');
					locationCodeFrom = locationCodeFrom.TrimEnd('-');

					string[] locationCodes = locationCodeFrom.Split('-');	  // 0		1
			
					if (locationCodes.Length != 1)						 //Только 1 уровень
					{
						continue;
					}
					string locationCode = DomainUnknownCode.UnknownLocation;
					locationCode = locationCodes[0].Trim();
					newLocationString.Code = locationCode.CutLength(49);

					//Field3: loc_func_location_id		2		Tag = loc_func_location_id			LocationCode = loc_func_location_id [0] //Split Code ‘-’
					//Field4: full_descr						3		Description = full_descr
					//Field5: loc_mosad_id				4		Level1 = loc_mosad_id
					//Field6: loc_mosad_descr			5		LocationName = loc_mosad_descr 	Name1 = loc_mosad_descr
  		
					string name = recordEmpty[5].CutLength(49);
					newLocationString.Name = name.ReverseDosHebrew(invertLetter, rt2lf);
					newLocationString.Description = recordEmpty[3].CutLength(499);//Full Desc;
					newLocationString.Level1 = recordEmpty[4].CutLength(49);
					newLocationString.Name1 = recordEmpty[5].CutLength(249);
					//newLocationString.Level2 = recordEmpty[4];
					//newLocationString.Name2 = recordEmpty[5];
					//newLocationString.Level3 = recordEmpty[6];
					//newLocationString.Name3 = recordEmpty[7];
					//newLocationString.Name4 = "";
					newLocationString.Tag = recordEmpty[2].CutLength(99); 
		

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
