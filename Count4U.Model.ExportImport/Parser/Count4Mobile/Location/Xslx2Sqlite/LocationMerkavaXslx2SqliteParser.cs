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

namespace Count4U.Model.Count4Mobile
{
	public class LocationMerkavaXslx2SqliteParser : ILocationSQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, LocationMobile> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public LocationMerkavaXslx2SqliteParser(IServiceLocator serviceLocator,
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
				

				String[] recordEmpty = { "", "", "", "", "", "","",""};

				LocationMobile newLocation = new LocationMobile();
				int count = 8;
					if (record.Count() < 8)
					{
						count = record.Count();
					}
				 
					for (int i = 0; i < count; i++)
					{
						recordEmpty[i] = record[i].Trim();
					}
					//	Level1Code		  0
					//Level1Name		 1
					//Level2Code		 2
					//Level2Name		3
					//Level3Code		4
					//Level3Name		5


					newLocation.Level1Name = recordEmpty[3].ReverseDosHebrew(invertLetter, rt2lf);
					newLocation.Level2Name = recordEmpty[5].ReverseDosHebrew(invertLetter, rt2lf);
					newLocation.Level3Name = recordEmpty[7].ReverseDosHebrew(invertLetter, rt2lf);
					string[] names = new string[] { newLocation.Level1Name, newLocation.Level2Name, newLocation.Level3Name };

					string description = names.JoinRecord("-", true);
					newLocation.Description = description;

					newLocation.Level1Code = recordEmpty[2];
					newLocation.Level2Code = recordEmpty[4];
					newLocation.Level3Code = recordEmpty[6];
					string[] codes = new string[] { newLocation.Level1Code, newLocation.Level2Code, newLocation.Level3Code };
					string locationCode = codes.JoinRecord("-", true);
					newLocation.LocationCode = locationCode;
					newLocation.Uid = Guid.NewGuid().ToString();
					newLocation.InvStatus = "0";
					//Code 0	
					//Full Desc 1
					//Level1Code 2
					//Level1Name 3
					//Level2Code 4
					//Level2Name 5
					//Level3Code 6
					//Level3Name 7
					if (locationFromDBDictionary.ContainsKey(locationCode) == false)
					{
						//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
						this._locationDictionary[locationCode] = newLocation;
						locationFromDBDictionary[locationCode] = null;
					}				
			}
	
			return this._locationDictionary;
		}

	}
}
