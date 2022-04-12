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
{     ////парсим и записываем Count4U.Itur по IturCode
	public class IturMultiCsv2SdfParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;		  //результат 
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private List<Location> _locationToDBList;
		private IIturRepository _iturRepository;

		public IturMultiCsv2SdfParser(
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

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);                    // start from 1
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
			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);    //Текущая БД

			//перенесено в update
			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = this._iturRepository.GetERPIturDictionary(toDBPath);
			}
			catch { }


			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{

				if (record == null) continue;
				int countRecord = record.Length;
				if (countRecord < 2)
				{
					continue;
				}

				if (string.IsNullOrWhiteSpace(record[0]) == true) continue;
				//	Field 1 = Itur Code						 0
				//Field 2 = ERP Itur Code				 1
				//Field 3 = Barcode or ItemCode		 2
				//Field 4 = QuantityEdit				     3
				//Field 5 = PartialQuantity				 4

				string newIturCode = record[0].Trim();
				if (iturFromDBDictionary.ContainsKey(newIturCode) == true)
				{
					continue;   //есть IturCode
				}

				Itur newItur = new Itur();
				IturString newIturString = new IturString();
				newIturString.IturCode = newIturCode;
				newIturString.ERPIturCode = record[1].Trim();
				
				newIturString.LocationCode = DomainUnknownCode.UnknownLocation;
				newIturString.StatusIturBit = "0";

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
						newItur.Disabled = false;
						this._iturDictionary[newItur.IturCode] = newItur;
						iturFromDBDictionary[newItur.IturCode] = null;
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

			if (iturFromDBDictionary.ContainsKey("99999999") == false)
			{
				Itur newItur = GetNewIturCode(toDBPath, "9999", DomainUnknownCode.UnknownLocation);

				this._iturDictionary[newItur.IturCode] = newItur;
			}


			//foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)	   // источник db3
			foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)      // источник db3
			{
				yield return keyValueItur.Value;
			}

		}

		private Itur GetNewIturCode(string toDBPath, string prefix, string locationCode)
		{
			Itur tempItur = new Itur();
			int maxNumber = this._iturRepository.GetMaxNumber(locationCode, toDBPath) + 1;
					
			tempItur.Number = maxNumber;
			tempItur.NumberPrefix = prefix.PadLeft(4, '0');
			tempItur.NumberSufix = maxNumber.ToString().PadLeft(4, '0');
			string newIturCode = tempItur.NumberPrefix + tempItur.NumberSufix;
			tempItur.IturCode = newIturCode;
			tempItur.ERPIturCode = newIturCode;
			tempItur.StatusIturBit = 0;
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
