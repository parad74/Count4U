using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class PreviousInventoryNativPlusYesXlsxDbSetParserSN : IPreviousInventorySQLiteParser
	{
		//protected Dictionary<string, DocumentHeader> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		//protected DocumentHeaderString _rowDocumentHeader;
		private readonly ILog _log;
		private Dictionary<string, PreviousInventory> _previousInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;


		public PreviousInventoryNativPlusYesXlsxDbSetParserSN(
			IServiceLocator serviceLocator,
			ILog log) 
	
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._previousInventoryDictionary = new Dictionary<string, PreviousInventory>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, PreviousInventory> PreviousInventoryDictionary
		{
			get { return this._previousInventoryDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}


		public IEnumerable<PreviousInventory> GetPreviousInventory(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, PreviousInventory> previousInventoryFromDBDictionary,
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

			this._previousInventoryDictionary.Clear();
			this._errorBitList.Clear();

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			string separator = "|";

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


			// TO DO with previoseInventor
			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			//Dictionary<string, InventProduct> dictionaryInventProductCode = new Dictionary<string, InventProduct>();
			//try
			//{
			//	// count4UDB
			//	dictionaryInventProductCode = inventProductRepository.GetDictionaryInventProductsCode(dbPath);
			//}
			//catch { }

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{

				if (record == null) continue;
				int countRecord = record.Length;
				if (countRecord < 6)
				{
					continue;
				}
				String[] recordEmpty = { "", "", "", "", "", "" };
				int count = recordEmpty.Length;
				if (record.Count() < count)
				{
					count = record.Count();
				}

				for (int i = 0; i < count; i++)
				{
					if (record[i].Length > 49)
					{
						record[i] = record[i].Substring(0, 49);
					}
					recordEmpty[i] = record[i].Trim();
				}

				//	Q *
				//0 Location.Level1.Code
				//1 Location.Level2.Code
				//2 Location.Level2.Name
				//3
				//4 ItemCode
				//5 ItemName
				//Quantity
				//add Quantity default = 1


				string makat = recordEmpty[4].CutLength(49); 
				if (String.IsNullOrWhiteSpace(makat) == true)
				{
					String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator));
					continue;
				}
				string prfix = recordEmpty[0].LeadingZero4();
				string suffix = recordEmpty[1].LeadingZero4();
				string locationCode = prfix + "-" + suffix;

				if (String.IsNullOrWhiteSpace(recordEmpty[0]) == true
					&& String.IsNullOrWhiteSpace(recordEmpty[1]) == true)
				{
					String.Format(ParserFileErrorMessage.LocationCodeIsEmpty, record.JoinRecord(separator));
					locationCode = "-1";
				}

				string serialNumber = "";
				string serialNumberSupplier = "";

				Double quantity = 1.0;

				//Nativ + 4 составной ключ 
				string[] ids = new string[] { serialNumber, makat, locationCode, serialNumberSupplier };	// 4 - составной ключ для SN
				string ID = ids.JoinRecord("|");
				ID = ID.CutLength(49);

				//			Q *
				//0 Location.Level1.Code
				//1 Location.Level2.Code
				//2 Location.Level2.Name
				//3
				//4 ItemCode
				//5 ItemName
				//Quantity
				//add Quantity default = 1

				PreviousInventory newPreviousInventory = new PreviousInventory();
				newPreviousInventory.Uid = ID;
				newPreviousInventory.ItemCode = makat;
				newPreviousInventory.LocationCode = locationCode;
				newPreviousInventory.SerialNumberLocal = serialNumber;
				newPreviousInventory.Quantity = quantity.ToString();
				newPreviousInventory.SerialNumberSupplier = serialNumberSupplier;
				newPreviousInventory.DateCreated = "0";
				newPreviousInventory.DateModified = "0";

				if (previousInventoryFromDBDictionary.ContainsKey(ID) == false)
				{
					yield return newPreviousInventory;
				}
			}

	
		}

	}
}
