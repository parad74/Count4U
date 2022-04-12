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
	public class InventProductNativPlusMISSqlite2SdfParser : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, string> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;
		protected IIturRepository _iturRepository;
		protected ILocationRepository _locationRepository;
		private const string SerialKey = "SN";			  ////
		private const string QuantityKey = "Q";			////

		public InventProductNativPlusMISSqlite2SdfParser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILocationRepository locationRepository ,
			IIturRepository iturRepository,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			this._iturInFileDictionary = new Dictionary<string, string>();
			this._rowDocumentHeader = new DocumentHeaderString();
			this._iturRepository = iturRepository;
			this._locationRepository = locationRepository;
			this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());		 ////
		}


		public IEnumerable<InventProduct> GetInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
			Dictionary<string, ProductMakat> productMakatDictionary,
			Dictionary<string, Itur> iturFromDBDictionary,	   // пустой
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			//bool firstStringH = false;
			//string newDocumentCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewDocumentCode);
			string newDocumentCode = Guid.NewGuid().ToString();

			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = "00000001";
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newIturCode = "00010001";
			string deviceName = "";
			string localUserName = "";
			string filename = System.IO.Path.GetFileName(fromPathFile);	 ////
			filename = filename.Replace(".db3", "");
			filename = filename.CutLength(49);					  ////


			Dictionary<string, string> columnNameInRecordDictionary = parms.GetIPAdvancedFieldNameDictionaryFromParm();			   //ToDO?

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();
			this._iturInFileDictionary.Clear();

			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();

			////IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
			////this._iturInFileDictionary = documentHeaderRepository.GetIturDocumentCodeDictionary(dbPath);

			string separator = "|";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = _iturRepository.GetERPIturDictionary(dbPath);
			}
			catch { }

			List<string> locations = _locationRepository.GetLocationCodeList(dbPath);
			Dictionary<string, int> dictionaryPrffixIndex = new Dictionary<string, int>();
			if (locations.Contains(DomainUnknownCode.UnknownLocation) == false) locations.Add(DomainUnknownCode.UnknownLocation);
			foreach (string code in locations)
			{
				dictionaryPrffixIndex[code] = 1;
				int maxNumber = this._iturRepository.GetMaxNumber(code, dbPath);
				if (maxNumber > 1) dictionaryPrffixIndex[code] = maxNumber;

			}

			////IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			//////Dictionary<Tuple<string, string, string>, InventProduct> dictionaryMakat_SerialNumber_IturCode  = new Dictionary<Tuple<string,string,string>,InventProduct>();
			////Dictionary< string, InventProduct> dictionaryInventProductCode = new Dictionary<string,  InventProduct>();		
			////try
			////{
			////	// count4UDB
			////	dictionaryInventProductCode = inventProductRepository.GetDictionaryInventProductsCode(dbPath);
			////}
			////catch { }

			////ITemporaryInventoryRepository temporaryInventoryRepository = this._serviceLocator.GetInstance<ITemporaryInventoryRepository>();
			////Dictionary<string, TemporaryInventory> dictionaryTemporaryDeleteInventorys
			////	= temporaryInventoryRepository.GetDictionaryTemporaryInventorys(dbPath, "InventProduct", "DELETE");

			string tableName = "CurrentInventory";
			int rowCount = 0;
			int colCount = 35;
			string[] currentInventoryColumnName = new string[colCount];
			currentInventoryColumnName = FillTableCurrentInventoryColumnNames(currentInventoryColumnName);
			Dictionary<string, int> dictionaryColumnNumbers = FillDictionryColumnNumbers(currentInventoryColumnName);

			IPropertyStrRepository propertyStrRepository = this._serviceLocator.GetInstance<IPropertyStrRepository>();
			//Name =						//XPath								0		"UIDKey";
			//TypeCode = 				//Index 								1
			//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
			//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
			//DomainObject = "Profile";
			Dictionary<string, string> keyPropertyDictionary =
				propertyStrRepository.GetDictionaryProfileProperty(DomainObjectTypeEnum.Profile.ToString(), "UIDKey", dbPath);
			//PropertyStrs propertyStrs = propertyStrRepository.GetPropertyStrs(DomainObjectTypeEnum.Profile.ToString(), dbPath);
			int LengthOfKey = 4;
			if (keyPropertyDictionary.ContainsKey("5") == true) LengthOfKey = 5;
			if (keyPropertyDictionary.ContainsKey("6") == true) LengthOfKey = 6;


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
			int indexIturCode	= -1;
			int indexDeviceName = -1;
			int indexLocalUserName = -1;
			//[0]	"Uid"	
			//[1]	     "SerialNumberLocal"	
			//[2]	"ItemCode"	
			//[3]	"SerialNumberSupplier"	
			//[4]	"Quantity"	
			//[25]	"LocationCode"	
			//[26]	"DateModified"	
			//[27]	"DateCreated"	
			//[28]	"ItemStatus"	
			//[29]	"IturCode"

			////int modifided = 0;
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
						indexPropertyStr13 = "PropertyStr13".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexPropertyStr8 = "PropertyStr8".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexQuantity = "Quantity".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexLocationCode = "LocationCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexDateModified = "DateModified".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexDateCreated = "DateCreated".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexItemStatus = "ItemStatus".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexIturCode = "IturCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexDeviceName = "DeviceName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexLocalUserName = "LocalUserName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);


						//row[0]	"Uid"	
						//row[1]	"SerialNumberLocal"	
						//row[2]	"ItemCode"	
						//row[3]	"SerialNumberSupplier"	
						//row[4]	"Quantity"	
						//row[25]	"LocationCode"	
						//row[26]	"DateModified"	
						//row[27]	"DateCreated"	
						//row[28]	"ItemStatus"	

						//if (objectType == "H")
						//{
						//46d10cfdd5cbccc4_07-11-2017-18-10.db3

						string idPDA = filename;
						this._rowDocumentHeader.Name = newWorkerID;
						this._rowDocumentHeader.WorkerGUID = newWorkerID;
						DateTime dt = DateTime.Now;
						string createDateDoc  = dt.ToShortDateString();
						string createTimeDoc = dt.ToShortTimeString();
						try
						{
							//					0						1						2							3
							//44b9b47e6a9fbea8^2018-166~9^2018_06_18_15_10_32^
							string[] filesPart = new string[] { "", "","","" };
							string[] files = filename.Split('^');
							int let = 4;
							if (let > files.Length) let = files.Length;
							for (int i = 0; i < let; i++)
							{
								filesPart[i] = files[i];
								if (i == 2)
								{
									try
									{
										string[] dateTime = filesPart[i].Split('_');
										if (dateTime.Length >= 3)
										{
											createDateDoc = dateTime[2] + "/" + dateTime[1] + "/" + dateTime[0];
										}
										if (dateTime.Length >= 6)
										{
											createTimeDoc = dateTime[3] + ":" + dateTime[4] + ":" + dateTime[5];
										}
									}
									catch { }
								}
							}

							if (let > 0)
							{
								newWorkerID = filesPart[0];
								idPDA = filesPart[0];
							}
						}
						catch { }

						this._rowDocumentHeader.WorkerGUID = newWorkerID;// idPDA.PadLeft(9, '0');
						this._rowDocumentHeader.CreateDate = createDateDoc;
						this._rowDocumentHeader.CreateTime = createTimeDoc;
						this._rowDocumentHeader.Name = idPDA;
						//} // "H"
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
					//[29]	"IturCode"

					//===========================InventProduct==============================
					string makat = record[indexItemCode].CutLength(299);
					if (string.IsNullOrWhiteSpace(makat) == true)
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
						continue;
					}

					//string unitTypeCode = productMakatDictionary[makat].UnitTypeCode.Trim().ToUpper();
					//if (unitTypeCode != QuantityKey) continue;

					//newIturCode = "0001001"; ???
					//string locationCodeFrom = record[indexLocationCode].Trim();	  //2060-10-
					if (indexIturCode != -1)
					{
						newIturCode = record[indexIturCode].Trim();
					}

					if (indexDeviceName != -1)
					{
						deviceName = record[indexDeviceName].Trim();
					}

					if (indexLocalUserName != -1)
					{
						localUserName = record[indexLocalUserName].Trim();
					}

					string erpIturCode = record[indexLocationCode].Trim(); 
   					int IDDocumentHeader = 0;
					bool isNewItur = false;

					if (newIturCode.IsNotNone() == true)	  //IturCode есть в таблице
					{
						if (iturFromDBDictionary.IsDictionaryContainsKey(newIturCode) == false)
						{
							//========================================add new Itur with newIturCode ==============================
							if (importType.Contains(ImportDomainEnum.ImportItur) == true)
							{
								Itur newItur = new Itur();
								IturString newIturString = new IturString();

								newIturString.IturCode = newIturCode;
								newIturString.ERPIturCode = erpIturCode;
								newIturString.LocationCode = DomainUnknownCode.UnknownLocation;
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
									iturFromDBDictionary[newItur.IturCode] = newItur;     // словарь для того чтобы не дублировать Itur.IturCode в БД
									newIturCode = newItur.IturCode;
									isNewItur = true;
									if (retBitItur != 0)
									{
										this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
									}
								}
							}
							// должно быть continue; ?
						}
					}
					else		// IturCode is None	   - работать с ERPIturCode
					{
						if (dictionaryIturCodeERP.ContainsKey(erpIturCode) == true) //и  Есть в текущей БД значит все ok 
						{
							Itur itur = dictionaryIturCodeERP[erpIturCode];
							newIturCode = itur.IturCode;
						}
						else
						{	//if (importType.Contains(ImportDomainEnum.ImportItur) == false)		//и  erpIturCode нет db
							//Insert
							//нет в текущей Count4UDB	- add new Itur in Count4UDB	
							//создаем новый IturCode в другой location strange 
							string locationCode = "ProblemCode";
							Itur tempItur = GetNewIturCode(dbPath, dictionaryPrffixIndex,
								erpIturCode, locationCode, 2); //DeepNode

							Itur newItur = new Itur();
							IturString newIturString = new IturString();

							newIturString.IturCode = tempItur.IturCode;
							newIturString.ERPIturCode = erpIturCode;
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
								//iturFromDBDictionary[newItur.IturCode] = newItur;     // словарь для того чтобы не дублировать Itur.IturCode в БД
								dictionaryIturCodeERP[newItur.ERPIturCode] = newItur;     // словарь для того чтобы не дублировать Itur.IturCode в БД
								//newIturCode = newItur.IturCode;
								isNewItur = true;
								if (retBitItur != 0)
								{
									this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
								}
							}
						}
					}		 // new Itur By ERPIturCode
					
					//TODO
					//Itur currentItur = dictionaryIturCodeERP[erpIturCode];
					//newIturCode = currentItur.IturCode;

					newDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, /* iturFromDBDictionary, importType */
						dbPath, newIturCode, newSessionCode, isNewItur, deviceName, localUserName);

					//============= start test

					//string serialNumberSupplier1 = record[indexSerialNumberSupplier].CutLength(49);
					//string propertyStrKey8 = record[indexPropertyStr8].CutLength(99);
					//InventProduct inventProduct = new InventProduct();
					//inventProduct.SupplierCode = "serialNumberSupplier1_Value";
					//inventProduct.IPValueStr8 = "propertyStrKey8_VAlue";

					//if (LengthOfKey >= 4)
					//{
					//	string propertyName4 = keyPropertyDictionary["4"];
					//	string part1 = inventProduct.GetPropertyStringValueByPropertyName(propertyName4);					   //Test
					//}
					//if (LengthOfKey >= 5)
					//{
					//	string propertyName5 = keyPropertyDictionary["5"];
					//	string part1 = inventProduct.GetPropertyStringValueByPropertyName(propertyName5);					   //Test
					//}

					//if (LengthOfKey == 4) { }//собираем ключ из 4 элемнтов
					//if (LengthOfKey == 5) { }//собираем ключ из 5 элемнтов

					//string part1 = inventProduct.GetPropertyStringValueByPropertyName("SupplierCode");					   //Test
					//string part2 = inventProduct.GetPropertyStringValueByPropertyName("IPValueStr8");
					//============= end test

					//string locationCode = DomainUnknownCode.UnknownLocation;
					//locationCode = currentItur.LocationCode;
					//newIturCode = currentItur.IturCode;
					//int currentIturInvStatus = currentItur.InvStatus;

					//}//if (firstStringH == false)
					//=================//end нет строки документа документа

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

					string serialNumber = record[indexSerialNumberLocal].CutLength(49);
				//	string makat = record[indexItemCode].CutLength(49);
					string serialNumberSupplier = record[indexSerialNumberSupplier].CutLength(49);
				//	string key4thPart = record[indexPropertyStr13].CutLength(99);
	

					//============= start test
					//string propertyStrKey8 = record[indexPropertyStr8].CutLength(99);
					//InventProduct inventProduct = new InventProduct();
					//inventProduct.SupplierCode = serialNumberSupplier;
					//inventProduct.IPValueStr8 = propertyStrKey8;
					//string part1 = inventProduct.GetPropertyValue("SupplierCode");					   //Test
					//string part2 = inventProduct.GetPropertyValue("IPValueStr8"); 
					//============= end test

					// static key SerialNumberLocal | Item Code | LocationCode 
					// считаем с 1,2,3		  Sheet17 - Profile
					// XPath			Index		Property_Count4U		Property_mINV
					//UIDKey			4				SupplierCode				SerialNumberSupplier
					//UIDKey			5				IPValueStr1					PropertyStr8


					//Nativ + 4 составной ключ 
				//	string[] ids = new string[] { serialNumber, makat, locationCodeFrom, serialNumberSupplier };	// 4 - составной ключ для SN
					//if (unitTypeCode == QuantityKey)
					//{
					//	ids = new string[] { serialNumber, makat, locationCodeFrom, propertyStrKey8 };	// 4 - составной ключ	  для Q
					//}
					//string ID = ids.JoinRecord("|");
					//if(ID.Trim() == "|11020040|1001-14-19||")
					//{
					//	ID = ids.JoinRecord("|");
					//}

					InventProductSimpleString newInventProductString = new InventProductSimpleString();
					InventProduct newInventProduct = new InventProduct();
					newInventProductString.IturCode = newIturCode;
					newInventProductString.DocumentCode = newDocumentCode;
					string barcode = makat;

					////	string barcode = ID.CutLength(49);
					newInventProductString.Makat = makat;
					newInventProductString.Barcode = barcode;
					newInventProductString.SessionCode = newSessionCode;
					newInventProductString.WorkerID = newWorkerID;

					newInventProductString.ProductName = "NotExistInCatalog";
					string quantity = record[indexQuantity].Trim();
					if (string.IsNullOrWhiteSpace(quantity) == true) quantity = "1";
					newInventProductString.QuantityOriginal = quantity;
					newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();

					int retBitInventProduct = newInventProduct.ValidateError(newInventProductString, this._dtfi);
					if (retBitInventProduct != 0)  //Error
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
						continue;
					}

					retBitInventProduct = newInventProduct.ValidateDb3Warning(newInventProductString);
					if (retBitInventProduct != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
					}

					newInventProduct.Tag3 = record[indexUid].CutLength(99);
						newInventProduct.SerialNumber = serialNumber;// record[indexSerialNumberLocal].Trim();
					newInventProduct.SupplierCode = record[indexSerialNumberSupplier].CutLength(49);
					newInventProduct.SessionCode = newSessionCode;
					newInventProduct.WorkerID = localUserName;
					newInventProduct.ProductName = "NotExistInCatalog";
					newInventProduct.InputTypeCode = InputTypeCodeEnum.B.ToString();

					//newInventProduct.IturCode = newIturCode;
					newInventProduct.ERPIturCode = erpIturCode.CutLength(249);
					//newInventProduct.DocumentCode = newDocumentCode;
					newInventProduct.SessionCode = newSessionCode;
					newInventProduct.SectionNum = 0;
					newInventProduct.DocNum = Convert.ToInt32(IDDocumentHeader);

					newInventProduct.ModifyDate = newInventProduct.CreateDate = AndroidUtils.UnixEpoch;
					string androidDateModified = record[indexDateModified].Trim();
					string androidDateCreated = record[indexDateCreated].Trim();
					if (string.IsNullOrWhiteSpace(androidDateModified) == false)
					{
						newInventProduct.ModifyDate = new DateTime(androidDateModified.GetNullableValue<long>().GetValueOrDefault())
							.ConvertFromAndroidTime();
					}
					if (string.IsNullOrWhiteSpace(androidDateCreated) == false)
					{
						newInventProduct.CreateDate = new DateTime(androidDateCreated.GetNullableValue<long>().GetValueOrDefault())
							.ConvertFromAndroidTime();
					}

					if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
					{
						newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
						makat = productMakatDictionary.GetParentMakatFromMakatDictionary(barcode, Log);

						if (string.IsNullOrWhiteSpace(makat) == false)
						{
							if (makat == barcode) newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
							else newInventProduct.TypeMakat = TypeMakatEnum.B.ToString();
							newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
							newInventProduct.Makat = makat;
							newInventProduct.ImputTypeCodeFromPDA = productMakatDictionary[makat].UnitTypeCode.CutLength(49);
							newInventProduct.ProductName = productMakatDictionary[makat].Name.CutLength(99);
							newInventProduct.SectionCode = productMakatDictionary[makat].SectionCode;
						}
						else
						{	// TODO: проверить
							newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
							newInventProduct.Makat = barcode;
							newInventProduct.ProductName = "NotExistInCatalog";
							newInventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
							newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
							newInventProduct.ImputTypeCodeFromPDA = QuantityKey;
						}
					}		//ExistMakat
					else   //Not ExistMakat
					{
						newInventProduct.ProductName = "NotCheckInCatalog";
					}
					if (importType.Contains(ImportDomainEnum.ImportParentProductAdvanced) == true)
					{
						newInventProduct.SetAdvancedValueByName(record, tableName, dictionaryColumnNumbers,
							columnNameInRecordDictionary, Log, dbPath);
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

		//private string GetDocumentHeaderCodeByIturCode(
		//	ref int IDDocumentHeader,
		//	string dbPath,
		//	string newIturCode,	 //ожидается IturCode
		//	int currentIturInvStatus,
		//	string newSessionCode	)
		//{
		//	string retDocumentCode = "";
		//	//if (iturFromDBDictionary.ContainsKey(newIturCode) == false) return retDocumentCode;
		//	// есть   DocumentHeader в Itur
		//	if (this._iturInFileDictionary.ContainsKey(newIturCode) == true) //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
		//	{
		//		DocumentHeader document = this._iturInFileDictionary[newIturCode];
		//		if (document != null) IDDocumentHeader = Convert.ToInt32(document.ID);
		//		retDocumentCode = document.DocumentCode;

		//		if (currentIturInvStatus == 1 && document.Approve != false)			//if (currentIturInvStatus == 1) newDocumentHeader.Approve = false;			
		//		{
		//			document.Approve = false;
		//			base._documentHeaderRepository.Update(document, dbPath);
		//		}
		//		else if 	(currentIturInvStatus == 2 && document.Approve != true)	//else if (currentIturInvStatus == 2) newDocumentHeader.Approve = true;		
		//		{
		//			document.Approve = true;
		//			base._documentHeaderRepository.Update(document, dbPath);
		//		}
		//		return retDocumentCode;
		//	}
		//	//========================================DocumentHeader==================
		//	else // create new DocumentHeader
		//	{
		//		string newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле

		//		DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
		//		DocumentHeader newDocumentHeader = new DocumentHeader();
		//		newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
		//		newDocumentHeaderString.DocumentCode = newDocumentCode;
		//		newDocumentHeaderString.SessionCode = newSessionCode;				//in
		//		newDocumentHeaderString.CreateDate = this._rowDocumentHeader.CreateDate;
		//		newDocumentHeaderString.WorkerGUID = "UnknownWorker";
		//		newDocumentHeaderString.IturCode = newIturCode;
		//		newDocumentHeaderString.Name = this._rowDocumentHeader.Name;
		//		newDocumentHeaderString.WorkerGUID = this._rowDocumentHeader.WorkerGUID;

		//		int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
		//		if (retBitDocumentHeader != 0)  //Error
		//		{
		//			this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.Error });
		//		}
		//		else //	Error  retBitSession == 0 
		//		{
		//			retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
		//			newDocumentHeader.Approve = null;
		//			if (currentIturInvStatus == 1) newDocumentHeader.Approve = false; //first Document in Itur		  //currentIturInvStatus == 1
		//			else if (currentIturInvStatus == 2) newDocumentHeader.Approve = true;
		//			IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
		//			newDocumentHeader.ID = IDDocumentHeader;
		//			retDocumentCode = newDocumentCode;
		//			this._iturInFileDictionary[newIturCode] = newDocumentHeader; //словарь IturCode -> DocumentHeader. Создаем для каждого Itur только один DocumentHeader

		//			if (retBitDocumentHeader != 0)
		//			{
		//				this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.WarningParser });
		//			}
		//		}
		//		return retDocumentCode;
		//	}

		//}



		private string GetDocumentHeaderCodeByIturCode(
		ref int IDDocumentHeader,
//		Dictionary<string, Itur> iturERPCodeFromDBDictionary,	  
		//List<ImportDomainEnum> importType,
		string dbPath,
		string newIturCode,
		string newSessionCode,
		bool isNewItur	,
		string deviceName,
		string localUserName)
		{
			string retDocumentCode = "";
			if (this._iturInFileDictionary.ContainsKey(newIturCode) == true) //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
			{
				retDocumentCode = this._iturInFileDictionary[newIturCode];
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
				newDocumentHeaderString.CreateDate = this._rowDocumentHeader.CreateDate + " " + this._rowDocumentHeader.CreateTime;
				newDocumentHeaderString.WorkerGUID = localUserName;
				newDocumentHeaderString.IturCode = newIturCode;
				newDocumentHeaderString.Name = deviceName;
				//newDocumentHeaderString.WorkerGUID = this._rowDocumentHeader.WorkerGUID;

				int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
				if (retBitDocumentHeader != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.Error });
				}
				else //	Error  retBitSession == 0 
				{
					retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
					//newDocumentHeader.WorkerGUID = this._rowDocumentHeader.WorkerGUID;
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


		private Itur GetNewIturCode(string toDBPath, Dictionary<string, int> dictionaryPrffixIndex, string iturCodeERP, string locationCode, int deepNode)
		{
			Itur tempItur = new Itur();
			if (dictionaryPrffixIndex.ContainsKey(locationCode) == false)
			{
				dictionaryPrffixIndex[locationCode] = 1;	   //добавляем новый счетчик специально для локейшена (считаем намбер для суффикса)
				int maxNumber = this._iturRepository.GetMaxNumber(locationCode, toDBPath);
				if (maxNumber > 1) dictionaryPrffixIndex[locationCode] = maxNumber;
			}
			//================================
			string prefix = locationCode;
			string suffix = "";
			if (deepNode == 1)
			{
				suffix = "1";
			}
			else
			{
				int lastIndex = dictionaryPrffixIndex[locationCode];
				lastIndex++;
				suffix = lastIndex.ToString();
				dictionaryPrffixIndex[locationCode] = lastIndex;
			}

			int num = 0;
			bool ret = Int32.TryParse(suffix.TrimStart('0'), out num);
			tempItur.Number = num;
			tempItur.NumberPrefix = prefix.PadLeft(4, '0');
			tempItur.NumberSufix = suffix.ToString().PadLeft(4, '0');
			string newIturCode = tempItur.NumberPrefix + tempItur.NumberSufix;
			tempItur.IturCode = newIturCode;
			return tempItur;
		}


		private string GetLocationCode(string[] locationCodes)
		{
			string locationCode = "";
			locationCode = locationCodes[0].Trim();
			locationCode = locationCode.CutLength(249);
			if (locationCode.Trim().ToLower() == "locationcode") locationCode = "";
			return locationCode;
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
