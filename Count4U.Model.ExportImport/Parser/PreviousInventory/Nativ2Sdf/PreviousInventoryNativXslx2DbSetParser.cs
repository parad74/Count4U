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
using Count4U.Model.Common;


namespace Count4U.Model.Count4Mobile
{
	public class PreviousInventoryNativXslx2DbSetParser : IPreviousInventorySQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, PreviousInventory> _previousInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PreviousInventoryNativXslx2DbSetParser(IServiceLocator serviceLocator,
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PreviousInventory> GetPreviousInventory(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, PreviousInventory> previousInventoryFromDBDictionary,
			//Dictionary<string, string> catalogFromDBDictionary,
			//Dictionary<string, string> locationFromDBDictionary,
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
				if (record == null) continue;

				if (record.Length < 5)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}


				//ItemCode 			 0
				//LocationCode	 1
				// SNLocal			 2
				//SNSupplier		 3
				//Quantity			 4

				String[] recordEmpty = { "", "", "", "", "" };

														//"", "", "", "", "", "", "", "", "", "", 
														//"", "", "", "", "", "", "", "", "", "", 
														//"", "", "", "", "", "", "", "", "", "", 
														//"", "", "", "", "", "", "", "", "", "", 
														//"", "", "", "", "", "", "", "", "", "",};
				int count = recordEmpty.Length;
				//String[] recordEmpty = new string[count];
				//for (int i = 0; i < count; i++)
				//{
				//	recordEmpty[i] = "";
				//}

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

				//ItemCode 			 0
				//LocationCode	 1
				//SNLocal			 2
				//SNSupplier		 3
				//Quantity			 4
				string itemCode = recordEmpty[0] ;
				string locationCode = recordEmpty[1];  
				string serialNumberLocal = record[2].CutLength(249); 
				string serialNumberSupplier = recordEmpty[3];  
				Double quantity = 0.0;	   // AW
				bool ret = Double.TryParse(recordEmpty[4], out quantity);
	
				if (string.IsNullOrWhiteSpace(locationCode) == true) locationCode = "-1";

				//hashkey SerialNumberLocal|ItemCode|LocationCode
				//string[] Uids = new string[] { serialNumberLocal, itemCode, locationCode, propertyStr[8] };	   // в общем случае 4 составной ключ
				string[] Uids = new string[] { serialNumberLocal, itemCode, locationCode, "" };	   // в общем случае 4 составной ключ, считается что с превиус инвентор приходит только SN у которого 4 составляющая пустая
				string uid = Uids.JoinRecord("|");
				uid = uid.CutLength(49);
		

				//if (previousInventoryFromDBDictionary.ContainsKey(uid) == false)
				//{
					PreviousInventory newPreviousInventory = new PreviousInventory();
					newPreviousInventory.Uid = uid;
					newPreviousInventory.ItemCode = itemCode;
					newPreviousInventory.LocationCode = locationCode;
					newPreviousInventory.SerialNumberLocal = serialNumberLocal;
					newPreviousInventory.Quantity = quantity.ToString();
					newPreviousInventory.SerialNumberSupplier = serialNumberSupplier;
					newPreviousInventory.DateCreated = "0";
					newPreviousInventory.DateModified = "0";

					//previousInventoryFromDBDictionary[uid] = null;

					yield return newPreviousInventory;
				//}

			}
		}

	
	}
}
