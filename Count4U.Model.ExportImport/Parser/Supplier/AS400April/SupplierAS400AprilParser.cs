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
	public class SupplierAS400AprilParser : ISupplierParser
	{
		private readonly ILog _log;
		private Dictionary<string, Supplier> _supplierDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public SupplierAS400AprilParser(IServiceLocator serviceLocator,
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

			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();

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

		
			//Field9: SupplierID (col 64-71)

			//SupplierCodeEnd = 71


			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point		//Field9: SupplierID (col 64-71)
			{
				Start = catalogParserPoints.SupplierCodeStart,
				End = catalogParserPoints.SupplierCodeEnd,
				Length = catalogParserPoints.SupplierCodeEnd - catalogParserPoints.SupplierCodeStart + 1
			});

			int count = startEndSubstring.Count;

			foreach (String rec in fileParser.GetRecords(fromPathFile,
			encoding,
			countExcludeFirstString))
			{

				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
				int lenRow = rec.Length;


				if (lenRow < catalogParserPoints.CatalogMinLengthIncomingRow)
				{
					continue;
				}

				String[] record = { "" };

				for (int i = 0; i < count; i++)
				{
					if (startEndSubstring[i].End < lenRow)
					{
						record[i] = rec.Substring(startEndSubstring[i].Start, startEndSubstring[i].Length);
					}
					else //startEndSubstring[i].End >= lenRow
					{
						if (startEndSubstring[i].Start < lenRow)
						{
							record[i] = rec.Substring(startEndSubstring[i].Start, lenRow - startEndSubstring[i].Start);
						}
					}
				}
			
				string supplierCode = record[0].Trim();
				//supplierCode = supplierCode.TrimStart('0');
				if (this._supplierDictionary.ContainsKey(supplierCode) == false)
				{
					//string supplierName = record[1].Trim();
					//string supplierName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
					this._supplierDictionary[supplierCode] = new Supplier { SupplierCode = supplierCode, Name = supplierCode, Description = "" };
				}
				 
			} //foreach
			//this._supplierDictionary = this._supplierRepository.GetSupplierDictionary(fromPathFile, true);
			return this._supplierDictionary;
		}																	    

	
	}
}
