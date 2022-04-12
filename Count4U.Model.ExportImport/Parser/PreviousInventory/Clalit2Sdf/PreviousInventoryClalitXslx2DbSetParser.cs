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
	public class PreviousInventoryClalitXslx2DbSetParser : IPreviousInventorySQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, PreviousInventory> _previousInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PreviousInventoryClalitXslx2DbSetParser(IServiceLocator serviceLocator,
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
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsm = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsm);
			IFileParser fileParser;
			if (fileXlsm == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelMacrosFileParser.ToString()); }
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
					recordEmpty[i] = record[i].CutLength(49);
				}

				String[] propertyStr = new string[21]{ "", "", "", "", "", "", "", "", "", "",
																			"", "", "", "", "", "", "", "", "", "",  "",};
				String[] propExtenstion = new string[23]{ "", "", "", "", "", "", "", "", "", "",
																				 "", "", "", "", "", "", "", "", "", "", 
																				"", "",  "",};
				//for (int i = 1; i < 21; i++)
				//{
				//	propertyStr[i] = "";
				//}

				string itemCode = "111";
				string serialNumberLocal = record[1].CutLength(249); // B :		1
				if (serialNumberLocal.ToUpper() == "C15017116")
				{
					;
				}
				
				string iturCodeERP = recordEmpty[2]; // C :								2
				propertyStr[1] = recordEmpty[4]; // E : 									   4
				string serialNumberSupplier = recordEmpty[5]; // F :				   5
				propertyStr[2] = recordEmpty[6]; // G :									   6
				propertyStr[3] = recordEmpty[7]; // H :									   7
				propertyStr[4] = recordEmpty[8]; // I  :									   8
				propertyStr[5] = recordEmpty[9]; // J :								      9
				propertyStr[6] = recordEmpty[10]; // K :								  10
				string propertyStr7Bool = recordEmpty[11];	  // L :				  11
				if (propertyStr7Bool.ToUpper() == "N") propertyStr7Bool = "false";
				else if (propertyStr7Bool.ToUpper() == "Y") propertyStr7Bool = "true";
				else if (string.IsNullOrWhiteSpace(propertyStr7Bool) == true) propertyStr7Bool = "false";
				propertyStr[7] = propertyStr7Bool;
				propertyStr[11] = recordEmpty[34]; // AI								34
				propertyStr[12] = recordEmpty[35]; // AJ : 							35
				propertyStr[13] = recordEmpty[36]; // AK :							36
				propertyStr[14] = recordEmpty[37]; // AL :							37

				propExtenstion[1] = recordEmpty[12]; // M :							12
				propExtenstion[2] = recordEmpty[13]; // N :							13
				propExtenstion[3] = recordEmpty[14]; // O :							14
				propExtenstion[4] = recordEmpty[15]; // P :							15
				propExtenstion[5] = recordEmpty[16]; // Q :							16
				propExtenstion[6] = recordEmpty[17]; // R :							17
				propExtenstion[7] = recordEmpty[18]; // S :							18
				propExtenstion[8] = recordEmpty[19]; // T :							19
				propExtenstion[9] = recordEmpty[20]; // U :							20
				propExtenstion[10] = recordEmpty[21]; // V :							21
				propExtenstion[11] = recordEmpty[22]; // W :							22
				propExtenstion[12] = recordEmpty[23]; // X :							23

				propertyStr[8] = recordEmpty[24]; // Y : 								 24
				string longTsxt = recordEmpty[25];
				if (longTsxt.Contains("\n") == true)
				{
					longTsxt = longTsxt.Replace("\r\n", " ");
					longTsxt = longTsxt.Replace("\n", " ");
				}
				propertyStr[9] = longTsxt;					// Z :								 25			  long text
				propertyStr[10] = recordEmpty[26]; // AA :							 26


				propExtenstion[13] = recordEmpty[28]; // AC :							28
				propExtenstion[14] = recordEmpty[29]; // AD :							29
				propExtenstion[15] = recordEmpty[30]; // AE :							30
				propExtenstion[16] = recordEmpty[31]; // AF :							31
				propExtenstion[17] = recordEmpty[32]; // AG :							32
				propExtenstion[18] = recordEmpty[33]; // AH :							33



				//hashkey SerialNumberLocal|ItemCode|LocationCode
				string[] Uids = new string[] { serialNumberLocal, itemCode, iturCodeERP, propertyStr[13] };	   // в общем случае 4 составной ключ
				string uid = Uids.JoinRecord("|");
				uid = uid.CutLength(49);

				//if (previousInventoryFromDBDictionary.ContainsKey(uid) == false)
				//{
				PreviousInventory newPreviousInventory = new PreviousInventory();
				newPreviousInventory.Uid = uid;
				newPreviousInventory.ItemCode = itemCode;
				newPreviousInventory.LocationCode = iturCodeERP;
				newPreviousInventory.SerialNumberLocal = serialNumberLocal;
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

				newPreviousInventory.PropExtenstion1 = propExtenstion[1];
				newPreviousInventory.PropExtenstion2 = propExtenstion[2];
				newPreviousInventory.PropExtenstion3 = propExtenstion[3];
				newPreviousInventory.PropExtenstion4 = propExtenstion[4];
				newPreviousInventory.PropExtenstion5 = propExtenstion[5];
				newPreviousInventory.PropExtenstion6 = propExtenstion[6];
				newPreviousInventory.PropExtenstion7 = propExtenstion[7];
				newPreviousInventory.PropExtenstion8 = propExtenstion[8];
				newPreviousInventory.PropExtenstion9 = propExtenstion[9];
				newPreviousInventory.PropExtenstion10 = propExtenstion[10];
				newPreviousInventory.PropExtenstion11 = propExtenstion[11];
				newPreviousInventory.PropExtenstion12 = propExtenstion[12];
				newPreviousInventory.PropExtenstion13 = propExtenstion[13];
				newPreviousInventory.PropExtenstion14 = propExtenstion[14];
				newPreviousInventory.PropExtenstion15 = propExtenstion[15];
				newPreviousInventory.PropExtenstion16 = propExtenstion[16];
				newPreviousInventory.PropExtenstion17 = propExtenstion[17];
				newPreviousInventory.PropExtenstion18 = propExtenstion[18];
				newPreviousInventory.PropExtenstion19 = propExtenstion[19];
				newPreviousInventory.PropExtenstion20 = propExtenstion[20];
				newPreviousInventory.PropExtenstion21 = propExtenstion[21];
				newPreviousInventory.PropExtenstion22 = propExtenstion[22];

				newPreviousInventory.DateCreated = "0";
				newPreviousInventory.DateModified = "0";

				//previousInventoryFromDBDictionary[uid] = null;

				yield return newPreviousInventory;
				//}

			}
		}


	}
}
