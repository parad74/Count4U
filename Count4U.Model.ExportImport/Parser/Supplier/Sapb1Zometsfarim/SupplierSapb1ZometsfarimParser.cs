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
	public class SupplierSapb1ZometsfarimParser : ISupplierParser
	{
		private readonly ILog _log;
		private Dictionary<string, Supplier> _supplierDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public SupplierSapb1ZometsfarimParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._supplierDictionary = new Dictionary<string, Supplier>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Supplier> SupplierDictionary
		{
			get { return this._supplierDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		public Dictionary<string, Supplier> GetSuppliers(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Supplier> supplierFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._supplierDictionary.Clear();
			this._errorBitList.Clear();

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

			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			int i = 0;
			foreach (String row in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				i++;
				if (string.IsNullOrWhiteSpace(row) == true) { continue; }
				
				string temp = row.Trim('"');

				temp = temp.Replace('"', '%');
				temp = temp.Replace("%,%", "~");
				string[] recordSplit = temp.Split('~');
				if (recordSplit == null) continue;

				String[] record = { "", "", "", "", "", "", "", "", "", "", "", "", };

				int count = 12;
				if (recordSplit.Count() < 12)
				{
					count = recordSplit.Count();
				}

				for (int j = 0; j < count; j++)
				{
					string trim = recordSplit[j].Trim();
					trim = trim.Replace('%', '"');
					record[j] = trim;
				}

				if (record.Length < 5)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
				//Field5: SupplierCode 						4
				//Field6: SupplierName 						5				

				string supplierCode = record[4].Trim();
				supplierCode = supplierCode.Trim('"');
				supplierCode = supplierCode.Trim();
				
				if (this._supplierDictionary.ContainsKey(supplierCode) == false)
				{
					string supplierName = record[5].ReverseDosHebrew(invertLetter, rt2lf);
					supplierName = supplierName.Trim('"');
					supplierName = supplierName.Trim();
					this._supplierDictionary[supplierCode] = new Supplier { SupplierCode = supplierCode, Name = supplierName, Description = "" };
				}
			} //foreach
			//this._supplierDictionary = this._supplierRepository.GetSupplierDictionary(fromPathFile, true);
			return this._supplierDictionary;
		}

	
	}
}
