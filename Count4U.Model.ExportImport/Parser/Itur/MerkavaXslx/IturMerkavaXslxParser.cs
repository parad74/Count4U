using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class IturMerkavaXslxParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;


		public IturMerkavaXslxParser(IServiceLocator serviceLocator,
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
		public Dictionary<string, Itur> GetIturs(string fromPathFile,
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
			ILocationRepository  locationRepositorym = this._serviceLocator.GetInstance<ILocationRepository>();
			Dictionary<string, Location> dictionaryLocation =locationRepositorym.GetLocationDictionary(toDBPath, true);
			//string filePath = Properties.Settings.Default.FilePath;
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

				string locationCodeFrom = record[0].Trim();							//2060-10-
				string[] locationCodes = locationCodeFrom.Split('-');				   // 0		1
				//string prefix = locationCodes[0].Trim();
				//string suffix = locationCodes[1].Trim();
				string prefix =  record[2].Trim();
				string suffix = record[4].Trim();

				if (locationCodes.Length < 2) continue;
				if (string.IsNullOrWhiteSpace(suffix) == true) continue;
				if (string.IsNullOrWhiteSpace(prefix) == true) continue;

				string iturCodeERP = locationCodeFrom.TrimEnd('-');
				string newIturCode = locationCodeFrom.TrimEnd('-');
				//iturCodeERP = prefix + suffix;
				newIturCode = prefix.PadLeft(4, '0') + suffix.ToString().PadLeft(4, '0');

				string locationCode = DomainUnknownCode.UnknownLocation;
				locationCode = prefix;

				Itur newItur = new Itur();
				IturString newIturString = new IturString();
				newIturString.IturCode = newIturCode;
				newIturString.ERPIturCode = iturCodeERP;
				newIturString.LocationCode = locationCode;
				newIturString.StatusIturBit = "0";
				newIturString.Name = record[5];
		
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
					if (iturFromDBDictionary.IsDictionaryContainsKey(newItur.IturCode) == false)
					{
						this._iturDictionary[newItur.IturCode] = newItur;
						iturFromDBDictionary[newItur.IturCode] = null;
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
			return this._iturDictionary;
		}
	}


}
