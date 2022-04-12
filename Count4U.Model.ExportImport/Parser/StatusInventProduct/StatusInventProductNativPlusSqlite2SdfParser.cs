using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Common;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class StatusInventProductNativPlusSqlite2SdfParser : InventProductParserBase, IStatusInventProductSimpleParser
	{
		protected Dictionary<string, DocumentHeader> _iturInFileDictionary; //key IturCode, DocumentHeader 
		protected DocumentHeaderString _rowDocumentHeader;
		private const string SerialKey = "SN";
		private const string QuantityKey = "Q";

		public StatusInventProductNativPlusSqlite2SdfParser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			this._iturInFileDictionary = new Dictionary<string, DocumentHeader>();
			this._rowDocumentHeader = new DocumentHeaderString();
			this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
		}


		public IEnumerable<StatusInventProduct> GetStatusInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null
		)
		{
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string filename = System.IO.Path.GetFileName(fromPathFile);
			filename = filename.CutLength(49);

			Dictionary<string, string> columnNameInRecordDictionary = parms.GetIPAdvancedFieldNameDictionaryFromParm();			   //ToDO?

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();
			this._iturInFileDictionary.Clear();

			string separator = "|";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}


			string tableName = "CurrentInventory";
			int rowCount = 0;
			int colCount = 35;
			string[] currentInventoryColumnName = new string[colCount];
			currentInventoryColumnName = FillTableCurrentInventoryColumnNames(currentInventoryColumnName);
			Dictionary<string, int> dictionaryColumnNumbers = FillDictionryColumnNumbers(currentInventoryColumnName);


			int indexUid = -1;
			int indexSerialNumberLocal = -1;
			int indexItemCode = -1;
			int indexSerialNumberSupplier = -1;
			int indexPropertyStr8 = -1;//Входит в составной ключ
			int indexPropertyStr13 = -1;//Входит в составной ключ
			
			int indexQuantity = -1;
			int indexLocationCode = -1;
			int indexDateModified = -1;
			int indexDateCreated = -1;
			int indexItemStatus = -1;
			//[0]	"Uid"	
			//[1]	     "SerialNumberLocal"	
			//[2]	"ItemCode"	
			//[3]	"SerialNumberSupplier"	
			//[4]	"Quantity"	
			//[25]	"LocationCode"	
			//[26]	"DateModified"	
			//[27]	"DateCreated"	
			//[28]	"ItemStatus"	

			int modifided = 0;
			foreach (object[] objects in this._fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString, tableName))
			{
				if (objects == null) continue;

				rowCount++;
				string[] record = new string[] { "rowCount = " + rowCount };

				try
				{
					record = objects as string[];
				}
				catch
				{
					Log.Add(MessageTypeEnum.Error,
					String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
				}

				if (record == null) continue;

				if (rowCount == 1) // header of Table
				{
					colCount = CheckTableColumnNames(currentInventoryColumnName, record, tableName);
					// 	dictionaryColumnNumbers - TODO получить индексы нужных полей, не найденых == -1
					{
						indexUid = "Uid".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexSerialNumberLocal = "SerialNumberLocal".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexItemCode = "ItemCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexSerialNumberSupplier = "SerialNumberSupplier".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexPropertyStr8 = "PropertyStr8".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexPropertyStr13 = "PropertyStr8".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexQuantity = "Quantity".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexLocationCode = "LocationCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexDateModified = "DateModified".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexDateCreated = "DateCreated".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexItemStatus = "ItemStatus".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);

						//row[0]	"Uid"	
						//row[1]	"SerialNumberLocal"	
						//row[2]	"ItemCode"	
						//row[3]	"SerialNumberSupplier"	
						//row[4]	"Quantity"	
						//row[25]	"LocationCode"	
						//row[26]	"DateModified"	
						//row[27]	"DateCreated"	
						//row[28]	"ItemStatus"	
					}
					// создать новый документ в БД		 - !!! только один документ в Itur может быть
				}
				else		   //row from table
				{
					int countRecord = record.Length;
					if (countRecord < colCount)
					{
						Log.Add(MessageTypeEnum.Error,
							String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
						continue;
					}

					//[0]	"Uid"	
					//[1]	     "SerialNumberLocal"	
					//[2]	"ItemCode"	
					//[3]	"SerialNumberSupplier"	
					//[4]	"Quantity"	
					//[5]	"PropertyStr1"	
					//[6]	"PropertyStr2"	
					//[7]	"PropertyStr3"	
					//[8]	"PropertyStr4"
					//[9]	"PropertyStr5"	
					//[10]	"PropertyStr6"	
					//[11]	"PropertyStr7"	
					//[12]	"PropertyStr8"	
					//[13]	"PropertyStr9"	
					//[14]	"PropertyStr10"	
					//[15]	"PropertyStr11"	
					//[16]	"PropertyStr12"	
					//[17]	"PropertyStr13"	
					//[18]	"PropertyStr14"	
					//[19]	"PropertyStr15"	
					//[20]	"PropertyStr16"	
					//[21]	"PropertyStr17"	
					//[22]	"PropertyStr18"	
					//[23]	"PropertyStr19"	
					//[24]	"PropertyStr20"	
					//[25]	"LocationCode"	
					//[26]	"DateModified"	
					//[27]	"DateCreated"	
					//[28]	"ItemStatus"	

					//===========================InventProduct==============================
					//	 В Меркаве - есть 2 типа айтемов - SN и Q. Так вот только для SN данные храняться в PreviuseInventory. =>
					//Для SN четвертая составляющая ключа везде пустая строка. И в PreviuseInventory и в currentInventory
					//Для айтемов типа "Q". Их нет в PreviuseInventory. Добавляя их в currentInventory четвертая строка в ключе = propertyStrKey8
					//KEY = Makat +		SerialNumber +			IturCodeERP +	IPValueStr8	  
					//KEY = ItemCode + SerialNumberLocal + LocationCode	 + PropertyStr8
					//[0]	"Uid"	
					//[1]	     "SerialNumberLocal"	
					//[2]	"ItemCode"	
					//[3]	"SerialNumberSupplier"	
					//[4]	"Quantity"	
					//[25]	"LocationCode"	
					//[26]	"DateModified"	
					//[27]	"DateCreated"	
					//[28]	"ItemStatus"	

					string locationCodeFrom = record[indexLocationCode].Trim();	  //2060-10-
					string serialNumber = record[indexSerialNumberLocal].CutLength(49);
					string makat = record[indexItemCode].CutLength(49);
					string serialNumberSupplier = record[indexSerialNumberSupplier].CutLength(49);
					string propertyStrKey13 = record[indexPropertyStr13].CutLength(99);
					//string propertyStrKey8 = record[indexPropertyStr8].CutLength(99);

					//string unitTypeCode = QuantityKey;
					//if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
					//{
					//	if (productMakatDictionary.ContainsKey(makat) == true)
					//	{
					//		unitTypeCode = productMakatDictionary[makat].UnitTypeCode.CutLength(49); 
					//	}
					//}
					//Nativ + 4 составной ключ 
					string[] ids = new string[] { serialNumber, makat, locationCodeFrom, serialNumberSupplier };	// 4 - составной ключ для SN
	
					//if (unitTypeCode == QuantityKey)
					//{
					//	ids = new string[] { serialNumber, makat, locationCodeFrom, propertyStrKey8 };	// 4 - составной ключ	  для Q
					//}
					string ID = ids.JoinRecord("|");
					StatusInventProduct newStatusInventProduct = new StatusInventProduct();
					string barcode = ID.CutLength(49);
					string quantity = record[indexQuantity].Trim();
					if (string.IsNullOrWhiteSpace(quantity) == true) quantity = "1";
					newStatusInventProduct.Code = barcode;
					newStatusInventProduct.Bit = 1; //?? считаем что всегда 1
					newStatusInventProduct.Name = filename;

					string androidDateModified = record[indexDateModified].Trim();
					string androidDateCreated = record[indexDateCreated].Trim();
					if (string.IsNullOrWhiteSpace(androidDateModified) == false)
					{
						DateTime dt = new DateTime(androidDateModified.GetNullableValue<long>().GetValueOrDefault())
							.ConvertFromAndroidTime();
						newStatusInventProduct.Description = dt.ToShortDateString() + " " + dt.ToShortTimeString();
					}


					yield return newStatusInventProduct;
				}
			}
		}

		

		private string[] FillTableCurrentInventoryColumnNames(string[] currentInventoryColumnName)
		{
			currentInventoryColumnName[0] = "Uid";
			currentInventoryColumnName[1] = "SerialNumberLocal";
			currentInventoryColumnName[2] = "ItemCode";
			currentInventoryColumnName[3] = "SerialNumberSupplier";
			currentInventoryColumnName[4] = "Quantity";
			currentInventoryColumnName[5] = "PropertyStr1";
			currentInventoryColumnName[6] = "PropertyStr2";
			currentInventoryColumnName[7] = "PropertyStr3";
			currentInventoryColumnName[8] = "PropertyStr4";
			currentInventoryColumnName[9] = "PropertyStr5";
			currentInventoryColumnName[10] = "PropertyStr6";
			currentInventoryColumnName[11] = "PropertyStr7";
			currentInventoryColumnName[12] = "PropertyStr8";
			currentInventoryColumnName[13] = "PropertyStr9";
			currentInventoryColumnName[14] = "PropertyStr10";
			currentInventoryColumnName[15] = "PropertyStr11";
			currentInventoryColumnName[16] = "PropertyStr12";
			currentInventoryColumnName[17] = "PropertyStr13";
			currentInventoryColumnName[18] = "PropertyStr14";
			currentInventoryColumnName[19] = "PropertyStr15";
			currentInventoryColumnName[20] = "PropertyStr16";
			currentInventoryColumnName[21] = "PropertyStr17";
			currentInventoryColumnName[22] = "PropertyStr18";
			currentInventoryColumnName[23] = "PropertyStr19";
			currentInventoryColumnName[24] = "PropertyStr20";
			currentInventoryColumnName[25] = "LocationCode";
			currentInventoryColumnName[26] = "DateModified";
			currentInventoryColumnName[27] = "DateCreated";
			currentInventoryColumnName[28] = "ItemStatus";
			currentInventoryColumnName[29] = "IturCode";
			currentInventoryColumnName[30] = "DeviceId";
			currentInventoryColumnName[31] = "DeviceName";
			currentInventoryColumnName[32] = "LoginUserName";
			currentInventoryColumnName[33] = "LocalUserName";
			currentInventoryColumnName[34] = "QuantityInPackEdit";
			return currentInventoryColumnName;
		}

		private Dictionary<string, int> FillDictionryColumnNumbers(string[] columnNames)
		{
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string,int>();
			if (columnNames == null) return dictionaryColumnNumbers;
			int columCount= columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}
			return dictionaryColumnNumbers;
		}

		private int CheckTableColumnNames(string[] columnNames, string[] record, string fromTable)
		{
			if (columnNames == null) return -1;
			if (record == null) return -1;

			int columCount = columnNames.Length;
			int columCount1 = record.Length;
			if (columCount1 != columCount)
			{
				Log.Add(MessageTypeEnum.Error, String.Format("It is Expected  Different Number of Columns in {0} Table {1} : {2}", fromTable, columCount, columCount1));
			}
			int columCountMin = Math.Min(columCount, columCount1);
			for (int i = 0; i < columCountMin; i++)
			{
				string columnName = columnNames[i].ToLower().Trim();
				string columnName1 = record[i].ToLower().Trim();
				if (columnName != columnName1)
				{
					Log.Add(MessageTypeEnum.Error, String.Format("Column {0}  Different from Column {1} in {2} Table ", columnName, columnName1, fromTable));
				}
			}
			return columCountMin;
		}

		//private int GetIndexColumnByName(Dictionary<string, int> dictionaryColumnNumbers, string name, string fromTable)
		//{
		//	int indexUid = -1;
		//	bool ret = dictionaryColumnNumbers.TryGetValue(name, out indexUid);
		//	if (ret == false)
		//	{
		//		Log.Add(MessageTypeEnum.Error, String.Format("Column {0} not find in {1} Table ", name, fromTable));
		//	}
		//	return indexUid;
		//}

		private string GetDocumentHeaderCodeByIturCode(
			ref int IDDocumentHeader,
			string dbPath,
			string newIturCode,	 //ожидается IturCode
			int currentIturInvStatus,
			string newSessionCode	)
		{
			string retDocumentCode = "";
			//if (iturFromDBDictionary.ContainsKey(newIturCode) == false) return retDocumentCode;
			// есть   DocumentHeader в Itur
			if (this._iturInFileDictionary.ContainsKey(newIturCode) == true) //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
			{
				DocumentHeader document = this._iturInFileDictionary[newIturCode];
				if (document != null) IDDocumentHeader = Convert.ToInt32(document.ID);
				retDocumentCode = document.DocumentCode;

				if (currentIturInvStatus == 1 && document.Approve != false)			//if (currentIturInvStatus == 1) newDocumentHeader.Approve = false;			
				{
					document.Approve = false;
					base._documentHeaderRepository.Update(document, dbPath);
				}
				else if 	(currentIturInvStatus == 2 && document.Approve != true)	//else if (currentIturInvStatus == 2) newDocumentHeader.Approve = true;		
				{
					document.Approve = true;
					base._documentHeaderRepository.Update(document, dbPath);
				}
				return retDocumentCode;
			}
			//========================================DocumentHeader==================
			else // create new DocumentHeader
			{
				string newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле

				DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
				DocumentHeader newDocumentHeader = new DocumentHeader();
				newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
				newDocumentHeaderString.DocumentCode = newDocumentCode;
				newDocumentHeaderString.SessionCode = newSessionCode;				//in
				newDocumentHeaderString.CreateDate = this._rowDocumentHeader.CreateDate;
				newDocumentHeaderString.WorkerGUID = "UnknownWorker";
				newDocumentHeaderString.IturCode = newIturCode;
				newDocumentHeaderString.Name = this._rowDocumentHeader.Name;
				newDocumentHeaderString.WorkerGUID = this._rowDocumentHeader.WorkerGUID;

				int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
				if (retBitDocumentHeader != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.Error });
				}
				else //	Error  retBitSession == 0 
				{
					retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
					newDocumentHeader.Approve = null;
					if (currentIturInvStatus == 1) newDocumentHeader.Approve = false; //first Document in Itur		  //currentIturInvStatus == 1
					else if (currentIturInvStatus == 2) newDocumentHeader.Approve = true;
					IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
					newDocumentHeader.ID = IDDocumentHeader;
					retDocumentCode = newDocumentCode;
					this._iturInFileDictionary[newIturCode] = newDocumentHeader; //словарь IturCode -> DocumentHeader. Создаем для каждого Itur только один DocumentHeader

					if (retBitDocumentHeader != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.WarningParser });
					}
				}
				return retDocumentCode;
			}

		}

		
	}


	//	<Property1>ציוד</Property1>	  оборудование
	//<Property2>תאור ציוד</Property2>	  осветительного оборудования
	//<Property3>ערך נכס מופחת</Property3> Снижение стоимости имущества
	//<Property4>שדה מיון</Property4>		  поле Категория
	//<Property5>חדר</Property5>	   комната
	//<Property8>UID-RFID</Property8>		
	//<Property9>הערות</Property9>	  Комментарии
	//<Property10>תאריך רכישה</Property10>	Дата покупки 
  //<Property11>ערך רכישה</Property11> покупная стоимость
	//<Property12>נכס</Property12>	  свойство
//תאור ציוד
//ערך נכס מופחת
//שדה מיון
//חדר
	//UID-RFID	 (PropertyStr8)
//הערות
//תאריך רכישה
//ערך רכישה
//נכס

//оборудование
//осветительного оборудования
//Снижение стоимости имущества
//поле Категория
//комната
	//UID-RFID	 (PropertyStr8)
//Комментарии
//Дата покупки
//покупная стоимость
//свойство
}
