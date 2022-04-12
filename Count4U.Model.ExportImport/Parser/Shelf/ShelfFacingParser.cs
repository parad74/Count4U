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
	public class ShelfFacingParser : IShelfParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private Dictionary<string, Supplier> _supplierDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public ShelfFacingParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._iturDictionary = new Dictionary<string, Itur>();
			this._supplierDictionary = new Dictionary<string, Supplier>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
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
		//public Dictionary<string, Itur> GetIturs(string fromPathFile,
		//	Encoding encoding, string[] separators,
		//	int countExcludeFirstString,
		//	Dictionary<string, Itur> iturFromDBDictionary,
		//	Dictionary<ImportProviderParmEnum, object> parms = null)
		public IEnumerable<Shelf> GetShelfs(string fromPathFile, 
			Encoding encoding, string[] separators, 
			int countExcludeFirstString, 
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null,
			string iturCodeIn = "", string shelfCodeIn = "")

		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._iturDictionary.Clear();
			this._supplierDictionary.Clear();
			this._errorBitList.Clear();
			//string iturCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewIturCode);
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

			string newDocumentCode = Guid.NewGuid().ToString(); 
			
			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{
				if (record == null) continue;
				if (record.Length < 9)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				//Field1: Itur Code				0 в Itur
				//Field2: Width					1 в Itur
				//Field3: Height					2 в Itur
				//Field4: Total Shelves		3 количество - хранится в Itur
				//Field5: Shelf Code			4  номер полки, если итур разбит (Total Shelves)
				//Field6: Supplier Code		5
				//Field7: Supplier Width		6
				//Field8: Date						7
				//Field9: Time						8


				//string objectType = Convert.ToString(record[0]).Trim(" \t".ToCharArray());

				Shelf newShelf = new Shelf();
				ShelfString newShelfString = new ShelfString();
				string iturCode = record[0].Trim();
				iturCode = iturCode.TrimStart('0');
				if (iturCode.Length == 0) newShelfString.IturCode = "20159999";
				else if (iturCode.Length <= 4) 	newShelfString.IturCode = "2015" + iturCode.PadLeft(4, '0');
				else newShelfString.IturCode = iturCode;
			
				newShelfString.ShelfNum = record[4];
				newShelfString.SupplierCode = record[5];
				newShelfString.CreateDate = record[7];
				newShelfString.CreateTime = record[8];
				newShelfString.ShelfPartInItur = record[3];
				newShelfString.Width = record[6];
				newShelfString.Height = record[2]; //надо поделить на newShelfString.Width
				newShelfString.ShelfPartCode = newDocumentCode;
				//newShelfString.StatusIturBit = "0";

				int retBit = newShelf.ValidateError(newShelfString, this._dtfi);
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
					retBit = newShelf.ValidateWarning(newShelfString, this._dtfi); //Warning
				}
				yield return newShelf;
			}
		}

		private int GetInt(string strValue)
		{
			int ret = 0;
			bool yes1 = Int32.TryParse(strValue, out ret);
			if (yes1 == true) return ret;
			else return 0;
		
		}


			

	
	}

	
}
