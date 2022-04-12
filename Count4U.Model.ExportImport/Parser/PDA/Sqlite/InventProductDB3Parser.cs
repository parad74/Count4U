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

namespace Count4U.Model.Count4U
{
	public class InventProductDB3Parser : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, string> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductDB3Parser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			this._iturInFileDictionary = new Dictionary<string, string>();
			this._rowDocumentHeader = new DocumentHeaderString();
			this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
		}

	
		public IEnumerable<InventProduct> GetInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
			Dictionary<string, ProductMakat> productMakatDictionary,
			Dictionary<string, Itur> iturFromDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			//bool firstStringH = false;
			//string newDocumentCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewDocumentCode);
			string newDocumentCode = Guid.NewGuid().ToString(); 

			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = ""; 
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newIturCode = "00010001";

			//Dictionary<string, Itur> erpIturDictionary = new Dictionary<string, Itur>();
			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//if (string.IsNullOrWhiteSpace(dbPath) == false)
			//{
			//	erpIturDictionary = iturRepository.GetERPIturDictionary(dbPath);
			//}

			string prefix = "2016";
			int number = iturRepository.GetMaxNumber(prefix, dbPath);

			//columnNameInRecordDictionary["IPValueStr1"] = "PropertyStr1";
			Dictionary<string, string> columnNameInRecordDictionary =	parms.GetIPAdvancedFieldNameDictionaryFromParm();			   //ToDO?

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
			currentInventoryColumnName = FillTableCurrentInventoryColumnNames( currentInventoryColumnName);
			Dictionary<string, int> dictionaryColumnNumbers = FillDictionryColumnNumbers(currentInventoryColumnName);

			int indexUid = -1;
			int indexSerialNumberLocal = -1;
			int indexItemCode = -1;
			int indexSerialNumberSupplier = -1;
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
						indexSerialNumberLocal = "SerialNumberLocal".GetIndexColumnByName(dictionaryColumnNumbers,  tableName, colCount, Log);
						indexItemCode = "ItemCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexSerialNumberSupplier = "SerialNumberSupplier".GetIndexColumnByName(dictionaryColumnNumbers,  tableName, colCount, Log);
						indexQuantity = "Quantity".GetIndexColumnByName(dictionaryColumnNumbers,tableName, colCount, Log);
						indexLocationCode = "LocationCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexDateModified = "DateModified".GetIndexColumnByName(dictionaryColumnNumbers,  tableName, colCount, Log);
						indexDateCreated = "DateCreated".GetIndexColumnByName(dictionaryColumnNumbers,tableName, colCount, Log);
						indexItemStatus = "ItemStatus".GetIndexColumnByName(dictionaryColumnNumbers,  tableName, colCount, Log);

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
					// создать новый документ в БД
					{
						newDocumentCode = Guid.NewGuid().ToString();
						this._rowDocumentHeader.Name = newDocumentCode;
						this._rowDocumentHeader.WorkerGUID = "00001";
						this._rowDocumentHeader.CreateDate = DateTime.Now.ToShortDateString();
						this._rowDocumentHeader.CreateTime = DateTime.Now.ToShortTimeString();
					}
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

					newIturCode = "0001001";
					int IDDocumentHeader = 0;
					bool isNewItur = false;

					string locationCodeFrom = record[indexLocationCode].Trim();	  //2060-10-
					string[] locationCodes = locationCodeFrom.Split('-');				   // 0		1
					string iturCodeERP = locationCodeFrom;
					if (locationCodes.Length >= 2)
					{
						string prefix1 = locationCodes[0].Trim();
						string suffix1 = locationCodes[1].Trim();
						iturCodeERP = prefix1 + suffix1;
						newIturCode = prefix1.PadLeft(4, '0') + suffix1.ToString().PadLeft(4, '0');
					}
					string locationCode=	DomainUnknownCode.UnknownLocation;
					if (locationCodes.Length >= 1)
					{
						locationCode = locationCodes[0].Trim();
					}
								

					/*   если не можем расчитывать на правильный IturCode
					if (erpIturDictionary.ContainsKey(iturCodeERP) == true)
					{
						newIturCode = erpIturDictionary[iturCodeERP].IturCode;
					}
					else   // нет в БД Itur с таким iturCodeERP => создаем новый IturCode, для того чтобы добавить в БД Itur 
					{
						number++;
						newIturCode = prefix.PadLeft(4, '0') + number.ToString().PadLeft(4, '0');
						Itur it = new Itur();
						it.IturCode = newIturCode;
						it.ERPIturCode = iturCodeERP;
						erpIturDictionary[iturCodeERP] = it;
						//это еще не все в БД нет
					}
					 */

					if (iturFromDBDictionary.IsDictionaryContainsKey(newIturCode) == false)
					{
						//======================================== Itur ==============================
						if (importType.Contains(ImportDomainEnum.ImportItur) == true)
						{
							Itur newItur = new Itur();
							IturString newIturString = new IturString();

							newIturString.IturCode = newIturCode;
							newIturString.ERPIturCode = iturCodeERP;
							newIturString.LocationCode = locationCode;
							newIturString.StatusIturBit = "0";

							int retBitItur = newItur.ValidateError(newIturString, this._dtfi);
							if (retBitItur != 0)  //Error
							{
								this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
							}
							else //	Error  retBit == 0 
							{
								retBitItur = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
								this._iturDictionary[newItur.IturCode] = newItur;  // словарь для добавления в БД в репозитории после импорта всего файла
								iturFromDBDictionary[newItur.IturCode] = null;     // словарь для того чтобы не дублировать Itur.IturCode в БД
								newIturCode = newItur.IturCode;
								isNewItur = true;
								if (retBitItur != 0)
								{
									this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
								}
							}
						}
					}// Itur

					//словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
					newDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, iturFromDBDictionary, importType, dbPath, newIturCode, newSessionCode, isNewItur);
					//}//if (firstStringH == false)
					//=================//end нет строки документа документа
					//[0]	"Uid"	
					//[1]	     "SerialNumberLocal"	
					//[2]	"ItemCode"	
					//[3]	"SerialNumberSupplier"	
					//[4]	"Quantity"	
					//[25]	"LocationCode"	
					//[26]	"DateModified"	
					//[27]	"DateCreated"	
					//[28]	"ItemStatus"	
					string makat = record[indexItemCode].Trim();
					string barcode = makat;
					InventProductSimpleString newInventProductString = new InventProductSimpleString();
					newInventProductString.IturCode = newIturCode;
					newInventProductString.DocumentCode = newDocumentCode;
					InventProduct newInventProduct = new InventProduct();

					int retBitInventProduct = newInventProduct.ValidateError(newInventProductString, this._dtfi);
					if (retBitInventProduct != 0)  //Error
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
						continue;
					} 

					newInventProductString.Makat = makat;
					newInventProductString.Barcode = barcode;
					string quantity = record[indexQuantity].Trim();
					if (string.IsNullOrWhiteSpace(quantity) == true) quantity = "1";
					newInventProductString.QuantityOriginal = record[indexQuantity].Trim();
				

					retBitInventProduct = newInventProduct.ValidateWarning(newInventProductString, this._dtfi);
					if (retBitInventProduct != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
					}

					newInventProduct.SerialNumber = record[indexSerialNumberLocal].Trim();
					newInventProduct.SupplierCode = record[indexSerialNumberSupplier].Trim();
					//TODO newInventProduct.CreateDate = 
					//TODO newInventProduct.ModifyDate = 
					//newInventProductString.CreateDate = record[indexDateCreated].Trim();
					//newInventProductString.CreateTime = record[indexDateCreated].Trim();
					//= record[indexItemStatus].Trim();
					newInventProduct.SessionCode = newSessionCode;
					newInventProduct.WorkerID = newWorkerID;
					newInventProduct.ProductName = "NotExistInCatalog";
					newInventProduct.InputTypeCode = InputTypeCodeEnum.B.ToString();

					newInventProduct.IturCode = newIturCode;
					newInventProduct.DocumentCode = newDocumentCode;
					newInventProduct.SessionCode = newSessionCode;
					newInventProduct.SectionNum = 0;
					newInventProduct.DocNum = Convert.ToInt32(IDDocumentHeader);

					if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
					{
						newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
						makat = productMakatDictionary.GetParentMakatFromMakatDictionary(barcode, Log);
						if (string.IsNullOrWhiteSpace(makat) == false)
						{
							if (makat == barcode) newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
							else newInventProduct.TypeMakat = TypeMakatEnum.B.ToString();
							newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
							newInventProduct.Makat = makat;
							newInventProduct.ProductName = productMakatDictionary[makat].Name;
						}
						else
						{	// TODO: проверить
							newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
							newInventProduct.Makat = barcode;
							newInventProduct.ProductName = "NotExistInCatalog";
							newInventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
							newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
						}
					}		//ExistMakat
					else   //Not ExistMakat
					{
						newInventProduct.ProductName = "NotCheckInCatalog";
					}
					if (importType.Contains(ImportDomainEnum.ImportParentProductAdvanced) == true)
					{
						//columnNameInRecordDictionary["IPValueStr1"] = "PropertyStr1";
						newInventProduct.SetAdvancedValueByName(record, tableName, dictionaryColumnNumbers,
							columnNameInRecordDictionary, Log, dbPath);		 

			//				public static void SetAdvancedValueByName(this InventProduct newInventProduct,
			//string[] record,
			//string tableName, 
			//Dictionary<string, int> dictionaryColumnNumbers,		   // ColumnName, ColumnNumber
			//Dictionary<string, string> columnNameInRecordDictionary,  //columnNameInRecordDictionary["IPValueStr1"] = "PropertyStr1";
			//ILog Log, string dbPath)
					}
					newInventProduct.Code = (newInventProduct.Makat + "^" + newInventProduct.Barcode).CutLength(299); 
					yield return newInventProduct;
				}

			} //foreach record from file

		
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
			Dictionary<string, Itur> iturFromDBDictionary,
			List<ImportDomainEnum> importType,
			string dbPath,
			string newIturCode,
			string newSessionCode,
			bool isNewItur)
		{
			string retDocumentCode = "";
			if (this._iturInFileDictionary.ContainsKey(newIturCode) == true) //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
			{
				retDocumentCode = this._iturInFileDictionary[newIturCode];
				DocumentHeader document =  base._documentHeaderRepository.GetDocumentHeaderByCode(retDocumentCode, dbPath)	;
				if (document != null) IDDocumentHeader = Convert.ToInt32(document.ID);

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
					if (isNewItur == true) newDocumentHeader.Approve = null;//false было  //first Document in Itur
					IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
					retDocumentCode = newDocumentCode;
					this._iturInFileDictionary[newIturCode] = newDocumentHeader.DocumentCode; //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader

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
