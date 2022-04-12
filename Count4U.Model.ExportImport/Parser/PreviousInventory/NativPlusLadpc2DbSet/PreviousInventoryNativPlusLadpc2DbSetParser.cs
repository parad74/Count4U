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
	public class PreviousInventoryNativPlusLadpc2DbSetParser : IPreviousInventorySQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, PreviousInventory> _previousInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PreviousInventoryNativPlusLadpc2DbSetParser(
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

			bool updateSN = parms.GetBoolValueFromParm(ImportProviderParmEnum.UpdateSN);

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

			int k = 0;

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{
				k++;
				if (record == null)
				{
					continue;
				}

				if (record.Length < 15)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}


				//Field1: PropertyStr8
				//Field2: PropertyStr4
				//Field3: LocationCode
				//Field4: (NotInUse)
				//Field5: ItemCode
				//Field6: (NotInUse)
				//Field7: SerialNumberSupplier
				//Field8: SerialNumberLocal
				//Field9: Quantity
				//Field10: PropertyStr9
				//Field11: PropertyStr1
				//Field12: PropertyStr2
				//Field13: PropertyStr3
				//Field14: PropertyStr7
				//Field15: PropertyStr5
				
				//PropertyStr8,PropertyStr4,LocationCode,(NotInUse),ItemCode,(NotInUse),
				//SerialNumberSupplier,SerialNumberLocal,Quantity,
				//PropertyStr9,PropertyStr1,PropertyStr2,PropertyStr3,PropertyStr7,PropertyStr5
																											 //20
				String[] recordEmpty = {   "", "", "", "", "", "", "", "", "", ""
														,"", "", "", "", "", "", "", "", "", ""
														,"", "", "", "", "", "", "", "", "", "" 
														,"", "", "", "", "", "", "", "", "", "" 
														,"", "", "", "", "", "", "", "", "", "" 
														,"", "", "", "", "", "", "", "", "", ""};
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

				String[] propertyStr = new string[21]{ "", "", "", "", "", "", "", "", "", "",
																			"", "", "", "", "", "", "", "", "", "",  "",};

				//Field1: PropertyStr8							 0
				//Field2: PropertyStr4						 1
				//Field3: LocationCode							 2
				//Field5: ItemCode								 4
				//Field7: SerialNumberSupplier			 6
				//Field8: SerialNumberLocal				 7
			
				string itemCode = recordEmpty[4];
				string locationCode = recordEmpty[2]; 
				string serialNumberLocal = record[7].CutLength(49);
				string serialNumberSupplier = recordEmpty[6].CutLength(49);
				//copy data from SerialNumberSupplier to SerialNumberLocal
				if (updateSN == true) { serialNumberLocal = serialNumberSupplier; }
				propertyStr[8] = recordEmpty[0]; 
				propertyStr[4] = recordEmpty[1]; 
			
	
				//Field10: PropertyStr9						 9
				//Field11: PropertyStr1						  10
				//Field12: PropertyStr2						   11
				//Field13: PropertyStr3							12
				//Field14: PropertyStr7							 13
				//Field15: PropertyStr5							 14
				propertyStr[9] = recordEmpty[9]; 
				propertyStr[1] = recordEmpty[10]; 
				propertyStr[2] = recordEmpty[11]; 
				propertyStr[3] = recordEmpty[12];
				propertyStr[7] = recordEmpty[13];
				propertyStr[5] = recordEmpty[14];
	

				//Field9: Quantity								 8
				Double quantity = 0.0;	   
				bool ret = Double.TryParse(recordEmpty[8], out quantity);
	
				if (string.IsNullOrWhiteSpace(locationCode) == true) locationCode = "-1";

				//hashkey SerialNumberLocal|ItemCode|LocationCode
				//string[] Uids = new string[] { serialNumberLocal, itemCode, locationCode, propertyStr[8] };	   // в общем случае 4 составной ключ
				//Nativ+ четырехсоставной ключ
				// тот же что в    Nativ+
				string[] ids = new string[] { serialNumberLocal, itemCode, locationCode, serialNumberSupplier };	   // в общем случае 4 составной ключ, считается что с превиус инвентор приходит только SN у которого 4 составляющая пустая
				string uid = ids.JoinRecord("|");
				string ID = uid.CutLength(49);

				if (previousInventoryFromDBDictionary.ContainsKey(ID) == false)
				{
					PreviousInventory newPreviousInventory = new PreviousInventory();
					newPreviousInventory.Uid = ID;
					newPreviousInventory.ItemCode = itemCode;
					newPreviousInventory.LocationCode = locationCode;
					newPreviousInventory.SerialNumberLocal = serialNumberLocal;
					newPreviousInventory.Quantity = quantity.ToString();
					newPreviousInventory.SerialNumberSupplier = serialNumberSupplier;
					newPreviousInventory.DateCreated = "0";
					newPreviousInventory.DateModified = "0";
		 			newPreviousInventory.PropertyStr1 = propertyStr[1];
					newPreviousInventory.PropertyStr2 = propertyStr[2];
					newPreviousInventory.PropertyStr3 = propertyStr[3];
					newPreviousInventory.PropertyStr4 = propertyStr[4];
					newPreviousInventory.PropertyStr5 = propertyStr[5];
					newPreviousInventory.PropertyStr6 = propertyStr[6];
					newPreviousInventory.PropertyStr7 = propertyStr[7];
					newPreviousInventory.PropertyStr8 = propertyStr[8];
					newPreviousInventory.PropertyStr9 = propertyStr[9];
					newPreviousInventory.PropertyStr10 = propertyStr[10];
					newPreviousInventory.PropertyStr11 = propertyStr[11];
					newPreviousInventory.PropertyStr12 = propertyStr[12];
					newPreviousInventory.PropertyStr13 = propertyStr[13];
					newPreviousInventory.PropertyStr14 = propertyStr[14];
					newPreviousInventory.PropertyStr15 = propertyStr[15];
					newPreviousInventory.PropertyStr16 = propertyStr[16];
					newPreviousInventory.PropertyStr17 = propertyStr[17];
					newPreviousInventory.PropertyStr18 = propertyStr[18];
					newPreviousInventory.PropertyStr19 = propertyStr[19];
					newPreviousInventory.PropertyStr20 = propertyStr[20];

					previousInventoryFromDBDictionary[ID] = null;

					int f = k;
					yield return newPreviousInventory;
				}

			}
		}

	
	}
}
