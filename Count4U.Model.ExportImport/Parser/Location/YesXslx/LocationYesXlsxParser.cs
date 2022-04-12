using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;


namespace Count4U.Model.Count4U
{
	public class LocationYesXlsxParser : ILocationParser
	{
		private readonly ILog _log;
		private Dictionary<string, Location> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public LocationYesXlsxParser(IServiceLocator serviceLocator,
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
				countExcludeFirstString))
			{
				if (record == null)	continue;

				if (record.Length < 1)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				String[] recordEmpty = { ""};

				//string objectType = Convert.ToString(record[0]).Trim(" ".ToCharArray());

				//if (objectType == "L")
				//{
					Location newLocation = new Location();
					LocationString newLocationString = new LocationString();
					int count = 1;
					//if (record.Count() < 1)
					//{
					//	count = record.Count();
					//}
				 
					for (int i = 0; i < count; i++)
					{
						recordEmpty[i] = record[i].Trim();
					}

					newLocationString.Code = recordEmpty[0];
					newLocationString.Name = recordEmpty[0];

					//if (record.Length > 1)
					//{
					//	newLocationString.Name = recordEmpty[1].ReverseDosHebrew(invertLetter, rt2lf);
					//}
					//if (record.Length > 2)
					//{
					//	newLocationString.Description = recordEmpty[2];
					//}
					//if (record.Length > 3)
					//{
					//	newLocationString.BackgroundColor = recordEmpty[3].Replace(',',':');
					//}
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
						//if (locationFromDBDictionary.IsDictionaryContainsKey(newLocation.Code, record.JoinRecord(separator), Log) == false)
						if (locationFromDBDictionary.ContainsKey(newLocation.Code) == false)
						{
							//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
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
				//}  //L
				//else
				//{
				//	Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, record.JoinRecord(separator)));
				//}
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
