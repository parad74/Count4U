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
	public class SupplierGeneralXLSXParser : ISupplierParser
	{
		private readonly ILog _log;
		private Dictionary<string, Supplier> _supplierDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public SupplierGeneralXLSXParser(IServiceLocator serviceLocator,
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

		/// <summary>
		/// Получение списка Product 
		/// </summary>
		/// <returns></returns>
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

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);		

			this._supplierDictionary.Clear();
			string unknownSupplierCode = DomainUnknownCode.UnknownSupplier;
			string unknownSupplierName = DomainUnknownName.UnknownSupplier;
			this._supplierDictionary[unknownSupplierCode] = new Supplier { SupplierCode = unknownSupplierCode, Name = unknownSupplierName, Description = "" };

			this._errorBitList.Clear();

			string separator = ",";
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 
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
				if (record == null) continue;
				if (record.Length < 9) 
				{
					continue;
				}

				//Field8: Supplier Code (optional) //7	
				//Field9: Supplier Name (optional) //8
				string supplierCode = DomainUnknownCode.UnknownSection;
				if (string.IsNullOrWhiteSpace(record[7].Trim()) == false)
				{
					supplierCode = record[7].Trim();
				}

				string supplierName = DomainUnknownName.UnknownSection;
				if (string.IsNullOrWhiteSpace(record[8].Trim()) == false)
				{
					supplierName = record[8].ReverseDosHebrew(invertLetter, rt2lf);
				}
			
				//if (string.IsNullOrWhiteSpace(supplierCode) == true)
				//{
				//	supplierCode = DomainUnknownCode.UnknownSupplier;
				//	if (this._supplierDictionary.ContainsKey(supplierCode) == false)
				//	{
				//		string supplierName = DomainUnknownName.UnknownSupplier;
				//		this._supplierDictionary[supplierCode] = new Supplier { SupplierCode = supplierCode, Name = supplierName, Description = "" };
				//	}
				//}
				//else
				if (this._supplierDictionary.ContainsKey(supplierCode) == false)
				{
					this._supplierDictionary[supplierCode] = new Supplier { SupplierCode = supplierCode, Name = supplierName, Description = "" };
				}


			}//foreach
			return this._supplierDictionary;
		}

	}


}
