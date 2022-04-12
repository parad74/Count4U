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
	public class PreviousInventoryNativPlusXslx2DbSetParser : IPreviousInventorySQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, PreviousInventory> _previousInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PreviousInventoryNativPlusXslx2DbSetParser(
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

				//ItemCode	0
				//LocationCode	1
				//SerialNumberLocal	2
				//SerialNumberSupplier	3
				//Quantity	4
				//PropertyStr1	5
				//PropertyStr2	6
				//PropertyStr3	7
				//PropertyStr4	8
				//PropertyStr5	9
				//PropertyStr6	10
				//PropertyStr7	11
				//PropertyStr8	12
				//PropertyStr9	13

				//PropertyStr10	14
				//PropertyStr11	15
				//PropertyStr12	16
				//PropertyStr13	17
				//PropertyStr14	18
				//PropertyStr15	19


				//PropertyStr16	20
				//PropertyStr17	21
				//PropertyStr18	22
				//PropertyStr19	23
				//PropertyStr20	24
				

				string itemCode = recordEmpty[0];
				string locationCode = recordEmpty[1];
				string serialNumberLocal = record[2].CutLength(249);
				string serialNumberSupplier = recordEmpty[3];
				propertyStr[1] = recordEmpty[5]; // F
				propertyStr[2] = recordEmpty[6]; // G
				propertyStr[3] = recordEmpty[7]; // H
				propertyStr[4] = recordEmpty[8]; // I
				propertyStr[5] = recordEmpty[9]; // J
				propertyStr[6] = recordEmpty[10]; //	K
				propertyStr[7] = recordEmpty[11]; //L
				propertyStr[8] = recordEmpty[12]; //M
				propertyStr[9] = recordEmpty[13]; // N

				//PropertyStr10	14
				//PropertyStr11	15
				//PropertyStr12	16
				//PropertyStr13	17
				//PropertyStr14	18
				//PropertyStr15	19
				propertyStr[10] = recordEmpty[14]; 
				propertyStr[11] = recordEmpty[15]; 
				propertyStr[12] = recordEmpty[16]; 
				propertyStr[13] = recordEmpty[17];
				propertyStr[14] = recordEmpty[18];
				propertyStr[15] = recordEmpty[19];

				//PropertyStr16	20
				//PropertyStr17	21
				//PropertyStr18	22
				//PropertyStr19	23
				//PropertyStr20	24
				propertyStr[16] = recordEmpty[20]; 
				propertyStr[17] = recordEmpty[21]; 
				propertyStr[18] = recordEmpty[22]; 
				propertyStr[19] = recordEmpty[23]; 
				propertyStr[20] = recordEmpty[24]; 


				Double quantity = 0.0;	   // AW
				bool ret = Double.TryParse(recordEmpty[4], out quantity);
	
				if (string.IsNullOrWhiteSpace(locationCode) == true) locationCode = "-1";

				//hashkey SerialNumberLocal|ItemCode|LocationCode
				//string[] Uids = new string[] { serialNumberLocal, itemCode, locationCode, propertyStr[8] };	   // в общем случае 4 составной ключ
				//Nativ+ четырехсоставной ключ
				string[] ids = new string[] { serialNumberLocal, itemCode, locationCode, serialNumberSupplier };	   // в общем случае 4 составной ключ, считается что с превиус инвентор приходит только SN у которого 4 составляющая пустая
				string ID = ids.JoinRecord("|");
				ID = ID.CutLength(49);
		

				//if (previousInventoryFromDBDictionary.ContainsKey(uid) == false)
				//{
					PreviousInventory newPreviousInventory = new PreviousInventory();
					newPreviousInventory.Uid = ID;
					newPreviousInventory.ItemCode = itemCode;
					newPreviousInventory.LocationCode = locationCode;
					newPreviousInventory.SerialNumberLocal = serialNumberLocal;
					newPreviousInventory.Quantity = quantity.ToString();
					newPreviousInventory.SerialNumberSupplier = serialNumberSupplier;
					newPreviousInventory.DateCreated = "0";
					newPreviousInventory.DateModified = "0";

					//	PropertyStr1	5	F
					//PropertyStr2	6	G
					//PropertyStr3	7	H
					//PropertyStr4	8	I
					//PropertyStr5	9	J
					//PropertyStr6	10	K
					//PropertyStr7	11	L
					//PropertyStr8	12	M
					//PropertyStr9	13	N

					newPreviousInventory.PropertyStr1 = propertyStr[1];
					newPreviousInventory.PropertyStr2 = propertyStr[2];
					newPreviousInventory.PropertyStr3 = propertyStr[3];
					newPreviousInventory.PropertyStr4 = propertyStr[4];
					newPreviousInventory.PropertyStr5 = propertyStr[5];
					newPreviousInventory.PropertyStr6 = propertyStr[6];
					newPreviousInventory.PropertyStr7 = propertyStr[7];
					newPreviousInventory.PropertyStr8 = propertyStr[8];
					newPreviousInventory.PropertyStr9 = propertyStr[9];
					//PropertyStr10	14
					//PropertyStr11	15
					//PropertyStr12	16
					//PropertyStr13	17
					//PropertyStr14	18
					//PropertyStr15	19

					newPreviousInventory.PropertyStr10 = propertyStr[10];
					newPreviousInventory.PropertyStr11 = propertyStr[11];
					newPreviousInventory.PropertyStr12 = propertyStr[12];
					newPreviousInventory.PropertyStr13 = propertyStr[13];
					newPreviousInventory.PropertyStr14 = propertyStr[14];
					newPreviousInventory.PropertyStr15 = propertyStr[15];

					//PropertyStr16	20
					//PropertyStr17	21
					//PropertyStr18	22
					//PropertyStr19	23
					//PropertyStr20	24
					newPreviousInventory.PropertyStr16 = propertyStr[16];
					newPreviousInventory.PropertyStr17 = propertyStr[17];
					newPreviousInventory.PropertyStr18 = propertyStr[18];
					newPreviousInventory.PropertyStr19 = propertyStr[19];
					newPreviousInventory.PropertyStr20 = propertyStr[20];

					//previousInventoryFromDBDictionary[uid] = null;

					int f = k;
					yield return newPreviousInventory;
				//}

			}
		}

	
	}
}
