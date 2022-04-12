﻿using System;
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
	public class PreviousInventoryMerkavaXslx2DbSetParser : IPreviousInventorySQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, PreviousInventory> _previousInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PreviousInventoryMerkavaXslx2DbSetParser(IServiceLocator serviceLocator,
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

				if (record.Length < 8)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				//Guid defGuid = default(Guid);

				String[] recordEmpty = { "", "", "", "", "", "", "", "", "", "",
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "",};
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

				//String[] propertyStr = new string[21];
				//for (int i = 1; i < 21; i++)
				//{
				//	propertyStr[i] = "";
				//}
				String[] propertyStr = new string[21]{ "", "", "", "", "", "", "", "", "", "",
																			"", "", "", "", "", "", "", "", "", "",  "",};
			   //все плохо !!! надо изначально сделать файл == InventProduct
				propertyStr[1] = recordEmpty[0]; // B
				propertyStr[2] = recordEmpty[1]; // C
				string itemCode = recordEmpty[4]; // F
				if (string.IsNullOrWhiteSpace(itemCode) == true) itemCode = "-1";
				if (itemCode == "0") itemCode = "-1";
				string itemName = recordEmpty[5]; // G
				string serialNumberLocal = record[6].CutLength(249);// H
				propertyStr[3] = recordEmpty[15]; // Q
				propertyStr[4] = recordEmpty[17]; //S
				string locationLevel3 = recordEmpty[18]; // T
				string locationName3 = recordEmpty[19]; // U
				propertyStr[5] = recordEmpty[20]; // V
				string locationLevel1 = recordEmpty[21]; // W
				string locationLevel2 = recordEmpty[22]; // X
				string locationName2 = recordEmpty[23]; // Y

				propertyStr[6] = recordEmpty[33]; // AI
				propertyStr[7] = recordEmpty[35]; // AK

				string propertyStr6FromPropertyStrList6Code = recordEmpty[33]; // AI
				string propertyStr7NameNotRelevant = recordEmpty[34]; // AJ
				string propertyStr7FromPropertyStrList6Code = recordEmpty[35]; // AK
				string propertyStr7NameNotRelevant2 = recordEmpty[36]; // AL
				string firstNameNotRelevant = recordEmpty[37]; // AM
				Double quantity = 0.0;	   // AW
				bool ret = Double.TryParse(recordEmpty[47], out quantity);
				string serialNumberSupplier = recordEmpty[51]; // BA
				propertyStr[8] = recordEmpty[52]; // BB
				propertyStr[9] = recordEmpty[53]; // BC
				propertyStr[10] = recordEmpty[54]; // BD
				propertyStr[11] = recordEmpty[55]; // BE
				string sourceSystemNotRelevant = recordEmpty[56]; // BF
				propertyStr[12] = recordEmpty[57]; // BG		 	Marina
				string locationCodeNotRelevant = recordEmpty[58]; // BH	Marina

				string[] codes = new string[] { locationLevel1, locationLevel2, locationLevel3 };
				string locationCode = codes.JoinRecord("-", true);
				if (string.IsNullOrWhiteSpace(locationCode) == true) locationCode = "-1";

				//hashkey SerialNumberLocal|ItemCode|LocationCode
				//string[] Uids = new string[] { serialNumberLocal, itemCode, locationCode, propertyStr[8] };	   // в общем случае 4 составной ключ
				string[] Uids = new string[] { serialNumberLocal, itemCode, locationCode, "" };	   // в общем случае 4 составной ключ, считается что с превиус инвентор приходит только SN у которого 4 составляющая пустая
				string uid = Uids.JoinRecord("|");
				uid = uid.CutLength(49);
				if (recordEmpty[0] == "1000425817" || propertyStr[1] == "1000425817")
				{
					uid = uid.CutLength(49);
				}

				//if (previousInventoryFromDBDictionary.ContainsKey(uid) == false)
				//{
					PreviousInventory newPreviousInventory = new PreviousInventory();
					newPreviousInventory.Uid = uid;
					newPreviousInventory.ItemCode = itemCode;
					newPreviousInventory.LocationCode = locationCode;
					newPreviousInventory.SerialNumberLocal = serialNumberLocal;
					newPreviousInventory.Quantity = quantity.ToString();
					newPreviousInventory.SerialNumberSupplier = serialNumberSupplier;

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
					newPreviousInventory.DateCreated = "0";
					newPreviousInventory.DateModified = "0";

				
					
					//previousInventoryFromDBDictionary[uid] = null;
					string key = serialNumberLocal + "|" + itemCode + "|" + propertyStr[1];			//	 | serialNumberSupplier
					//if (newPreviousInventory.PropertyStr1 == "10421418" || serialNumberLocal == "4C33030011285")
					//{
					//	string test = key;
					//}
					if (_previousInventoryDictionary.ContainsKey(key) == true)
					{
						PreviousInventory oldPreviousInventory = _previousInventoryDictionary[key]; 
						//если существует такой не заменять в случае если в нем есть SN Supplier, а у вновь пришедщего нет SN Supplier
						if (string.IsNullOrWhiteSpace(serialNumberSupplier) == false)	 // есть   new serialNumberSupplier
						{
							//NEWserialNumberSupplier == "AAA" 
							_previousInventoryDictionary[key] = newPreviousInventory;		  // значит меняем но новое значение
						}
						else	// Если   new serialNumberSupplier is empty				  //!!
						{
							 if (string.IsNullOrWhiteSpace(oldPreviousInventory.SerialNumberSupplier) == true)	// old serialNumberSupplier is empty
							  {
								  //NEWserialNumberSupplier == ""  &&  OLDserialNumberSupplier == "" 
								  if (string.IsNullOrWhiteSpace(serialNumberLocal) == false)	 // newserialNumberLocal is not empty
								  {
									  // (SerialNumberLocal) doesn't start with "33" or "26"
									  if (serialNumberLocal.StartsWith("33") == false)
										 if (serialNumberLocal.StartsWith("26") == false)
									  {
										  newPreviousInventory.SerialNumberSupplier = serialNumberLocal;
									  }
									}
								  _previousInventoryDictionary[key] = newPreviousInventory;		  // значит меняем но новое значение
							  }
							  else
							  {
								  //NEWserialNumberSupplier == ""  &&  OLDserialNumberSupplier == "BBB" 
								  _previousInventoryDictionary[key] = oldPreviousInventory;	
							  }
						}
					}

					else	 //_previousInventoryDictionary.ContainsKey(key) == false . Не заменяем, а новое значение добавляем
					{
						if (string.IsNullOrWhiteSpace(newPreviousInventory.SerialNumberSupplier) == true)	// old serialNumberSupplier is empty
						{
							if (string.IsNullOrWhiteSpace(serialNumberLocal) == false)	 // newserialNumberLocal is not empty
							{
								// (SerialNumberLocal) doesn't start with "33" or "26"
								if (serialNumberLocal.StartsWith("33") == false)
									if (serialNumberLocal.StartsWith("26") == false)
									{
										newPreviousInventory.SerialNumberSupplier = serialNumberLocal;
									}
							}
							_previousInventoryDictionary[key] = newPreviousInventory;
						}
						else
						{
							//NEWserialNumberSupplier == ""  &&  OLDserialNumberSupplier == "BBB" 
							_previousInventoryDictionary[key] = newPreviousInventory;
						}
						
					}
					//yield return newPreviousInventory;
				//}

			}

			foreach (KeyValuePair<string, PreviousInventory> keyValuePreviousInventory in this._previousInventoryDictionary)	   // источник db3
			{
				yield return keyValuePreviousInventory.Value;
			}
		}

	
	}
}
