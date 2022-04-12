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
	public class InventProductMerkavaXlsxParserSN : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, DocumentHeader> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductMerkavaXlsxParserSN(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			this._iturInFileDictionary = new Dictionary<string, DocumentHeader>();
			this._rowDocumentHeader = new DocumentHeaderString();
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
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);

			//string newDocumentCode = Guid.NewGuid().ToString(); 

			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = "";
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string currentIturCode = "00010001";

			string filename = System.IO.Path.GetFileName(fromPathFile);
			filename = filename.CutLength(49);

			Dictionary<string, int> indexInRecordDictionary = parms.GetIPAdvancedFieldIndexDictionaryFromParm();

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();
			this._iturInFileDictionary.Clear();

			string separator = "|";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();

			this._iturInFileDictionary = _documentHeaderRepository.GetIturDocumentCodeDictionary(dbPath);

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//Dictionary<string, Itur> dictionaryIturCode = new Dictionary<string, Itur>();
			//try
			//{
			//	// count4UDB
			//	dictionaryIturCode = iturRepository.GetIturDictionary(dbPath);
			//}
			//catch { }

			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = iturRepository.GetERPIturDictionary(dbPath);
			}
			catch { }

			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			Dictionary<string, InventProduct> dictionaryInventProductCode = new Dictionary<string, InventProduct>();
			try
			{
				// count4UDB
				dictionaryInventProductCode = inventProductRepository.GetDictionaryInventProductsUID(dbPath);
			}
			catch { }

			int inIP = dictionaryInventProductCode.Count;

			int k = 0;
			int j = 4; //счетчик строк 
			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{
				j++;
				if (record == null) continue;
				int countRecord = record.Length;
				if (countRecord < 25)
				{
					Log.Add(MessageTypeEnum.Error, "row = [" + j + "]  " + String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
				//60
				String[] recordEmpty = { "", "", "", "", "", "", "", "", "", "",
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "", 
														"", "", "", "", "", "", "", "", "", "",};
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

				String[] propertyStr = new string[21]{ "", "", "", "", "", "", "", "", "", "",
																			"", "", "", "", "", "", "", "", "", "",  "",};

				//record[0] = "		PropertyStr1	";
				//record[1] = "		PropertyStr2	";
				//record[4] = "		ItemCode	";
				//record[5] = "		ItemName	";
				//record[6] = "		SerialNumberLocal	";
				propertyStr[1] = recordEmpty[0]; 
				propertyStr[2] = recordEmpty[1];
				string makat = recordEmpty[4];
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, "row = [" + j + "]  " + String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				string itemName = recordEmpty[5]; // G
				string serialNumber = record[6].CutLength(249);// H

				//record[15] = "		PropertyStr3	";
				//record[17] = "		PropertyStr4	";
				//record[18] = "		Location.Level3	";
				//record[19] = "		Location.Name3	";
				//record[20] = "		PropertyStr5	";
				//record[21] = "		Location.Level1	";
				//record[22] = "		Location.Level2	";
				//record[23] = "		Location.Name2	";


				propertyStr[3] = recordEmpty[15]; 
				propertyStr[4] = recordEmpty[17]; 
				string locationLevel3 = recordEmpty[18]; 
				string locationName3 = recordEmpty[19];  //not use
				propertyStr[5] = recordEmpty[20]; 
				string locationLevel1 = recordEmpty[21]; 
				string locationLevel2 = recordEmpty[22]; 
				string locationName2 = recordEmpty[23];	 //not use

				//record[33] = "		PropertyStr6	";
				//record[35] = "		PropertyStr7	";
				propertyStr[6] = recordEmpty[33]; 
				propertyStr[7] = recordEmpty[35];

				//record[	47]="Quantity ";
				//record[	51	]="SerialNumberSupplier";
				Double quantity = 0.0;	   // AW
				bool ret = Double.TryParse(recordEmpty[47], out quantity);
				string serialNumberSupplier = recordEmpty[51]; 

				//record[52] = "		PropertyStr8	";
				//record[53] = "		PropertyStr9	";
				//record[54] = "		PropertyStr10	";
				//record[55] = "		PropertyStr11	";
				//record[57] = "		PropertyStr12	";

				propertyStr[8] = recordEmpty[52]; 
				propertyStr[9] = recordEmpty[53]; 
				propertyStr[10] = recordEmpty[54];
				propertyStr[11] = recordEmpty[55]; 
				propertyStr[12] = recordEmpty[57];
				//record[58] = "		LocationCode	";		 // не используем

				string[] codes = new string[] { locationLevel1, locationLevel2, locationLevel3 };
				string locationCodeFrom = codes.JoinRecord("-", true);
				if (string.IsNullOrWhiteSpace(locationCodeFrom) == true)
				{
					Log.Add(MessageTypeEnum.Error, "row = [" + j + "]  " + String.Format(ParserFileErrorMessage.IturCodeIsEmpty, record.JoinRecord(separator)));
					continue;
				}

						
				string erpIturCode = locationCodeFrom.CutLength(249);
				if (dictionaryIturCodeERP.ContainsKey(erpIturCode) == false)
				{
					Log.Add(MessageTypeEnum.Error, "row = [" + j + "]  " + String.Format(ParserFileErrorMessage.IturCodeNotExistInDB, erpIturCode));
					continue;
				}

				string unitTypeCode = "Q";
				if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
				{
					if (productMakatDictionary.ContainsKey(makat) == true)
					{
						unitTypeCode = productMakatDictionary[makat].UnitTypeCode.CutLength(49);

						if (unitTypeCode == "Q")
						{
							Log.Add(MessageTypeEnum.Error, "row = [" + j + "]  " + String.Format("Type of Product in Catalog is Q  : {0}", makat));
							continue;
						}
					}
					else
					{
						Log.Add(MessageTypeEnum.Error, "row = [" + j + "]  " + String.Format(ParserFileErrorMessage.MakatNotExistInDB, makat));
						continue;
					}
				}

				Itur currentItur = dictionaryIturCodeERP[erpIturCode];
				currentIturCode = currentItur.IturCode;
				string locationCode = currentItur.LocationCode;
				int currentIturInvStatus = currentItur.InvStatus;

				
				//	 В Меркаве - есть 2 типа айтемов - SN и Q. Так вот только для SN данные храняться в PreviuseInventory. =>
				//Для SN четвертая составляющая ключа везде пустая строка. И в PreviuseInventory и в currentInventory
				//Для айтемов типа "Q". Их нет в PreviuseInventory. Добавляя их в currentInventory четвертая строка в ключе = propertyStrKey8
				//serialNumber, makat, locationCode, ""
				string[] Uids = new string[] { serialNumber, makat, erpIturCode, "" };	// 4 - составной ключ для SN
				//string[] Uids = new string[] { serialNumber, makat, erpIturCode, serialNumber };	// 4 - составной ключ для SN
				string ID = Uids.JoinRecord("|");
				string uid = ID.CutLength(299);

				int IDDocumentHeader = 0;
				string currentDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, dbPath, currentIturCode, currentIturInvStatus, newSessionCode);

					

				InventProductSimpleString newInventProductString = new InventProductSimpleString();
				InventProduct newInventProduct = new InventProduct();
				newInventProductString.Makat = makat;
				newInventProductString.Barcode = uid;
				newInventProductString.IturCode = currentIturCode;
				newInventProductString.DocumentCode = currentDocumentCode;
				newInventProductString.SessionCode = newSessionCode;
				newInventProductString.ProductName = "NotExistInCatalog";
				newInventProductString.QuantityOriginal = "1";
				newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();
				//newInventProductString.CreateDate = record[6];
				//newInventProductString.CreateTime = record[7];


				int retBitInventProduct = newInventProduct.ValidateError(newInventProductString, this._dtfi);
				if (retBitInventProduct != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
					continue;
				}

				retBitInventProduct = newInventProduct.ValidateWarning(newInventProductString, this._dtfi);
				if (retBitInventProduct != 0)
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}

				newInventProduct.Code = ID.CutLength(299);
				newInventProduct.IturCode = currentIturCode;
				if (erpIturCode != null) newInventProduct.ERPIturCode = erpIturCode;
				newInventProduct.DocumentCode = currentDocumentCode;
				newInventProduct.SessionCode = newSessionCode;
				newInventProduct.SerialNumber = serialNumber;
				newInventProduct.WorkerID = filename;
				newInventProduct.ImputTypeCodeFromPDA = unitTypeCode; 
				newInventProduct.SectionNum = 0;
				DateTime dt = DateTime.Now;
				newInventProduct.ModifyDate = dt;
				newInventProduct.CreateDate = dt;

				newInventProduct.DocNum = Convert.ToInt32(IDDocumentHeader);

				if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
				{
					newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
					if (productMakatDictionary.ContainsKey(makat) == true)
					{
						newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
						newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
						newInventProduct.Makat = makat;
						newInventProduct.ProductName = productMakatDictionary[makat].Name;
						//newInventProduct.SectionCode = productMakatDictionary[makat].SectionCode;
					}
					else
					{	// TODO: проверить
						newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
						newInventProduct.ProductName = "NotExistInCatalog";
						//newInventProduct.SectionCode = DomainUnknownCode.UnknownSection;
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
					newInventProduct.IPValueStr1 = propertyStr[1];
					newInventProduct.IPValueStr2 = propertyStr[2];
					newInventProduct.IPValueStr3 = propertyStr[3];
					newInventProduct.IPValueStr4 = propertyStr[4];
					newInventProduct.IPValueStr5 = propertyStr[5];
					newInventProduct.IPValueStr6 = propertyStr[6];
					newInventProduct.IPValueStr7 = propertyStr[7];
					newInventProduct.IPValueStr8 = serialNumber;		//??? странное требование 21/08/2018
					newInventProduct.IPValueStr9 = propertyStr[9];
					newInventProduct.IPValueStr10 = propertyStr[10];
					newInventProduct.IPValueStr11 = propertyStr[11];
					newInventProduct.IPValueStr12 = propertyStr[12];
					newInventProduct.IPValueStr13 = "InsertedFromAdapterMerkavaExcel";// propertyStr[13];
					newInventProduct.IPValueStr14 = propertyStr[14];
					newInventProduct.IPValueStr15 = propertyStr[15];
					newInventProduct.IPValueStr16 = propertyStr[16];
					newInventProduct.IPValueStr17 = propertyStr[17];
					newInventProduct.IPValueStr18 = propertyStr[18];
					newInventProduct.IPValueStr19 = propertyStr[19];
					newInventProduct.IPValueStr20 = propertyStr[20];

					newInventProduct.SetAdvancedValue(record, indexInRecordDictionary, dbPath);
				}
				//UPDATE in   InventProduct
				//if (dictionaryInventProductCode.ContainsKey(ID) == true)
				//{
				//	dictionaryInventProductCode[ID] = null;
				//	//InventProduct oldInventProduct = dictionaryInventProductCode[IDQ];
				//	//dictionaryInventProductCode[ID] = oldInventProduct;
				//}
				newInventProduct.WorkerID = filename;
				if (dictionaryInventProductCode.ContainsKey(uid) == true)
				{
					Log.Add(MessageTypeEnum.Warning, "row = [" + j + "]  " + String.Format(ParserFileErrorMessage.BarcodeExistInDB, uid));
				}
				dictionaryInventProductCode[uid] = newInventProduct;		   // Всегда заменям , считаем что из файла приходит важнее
				k++;
			} //foreach record from file

			// DELETE  then UPDATE 
			importInventProductRepository.ClearInventProducts(dbPath);
			//then UPDATE 
			var inventProducts = dictionaryInventProductCode.Values.Select(x => x).AsEnumerable();

			foreach (InventProduct inventProduct in inventProducts)
			{
				if (inventProduct == null) continue;
				yield return inventProduct;
			}

			int outIP= inventProducts.Count();
			Log.Add(MessageTypeEnum.TraceParser, String.Format("Before update was in DB  [{0}] Items ", inIP));
			Log.Add(MessageTypeEnum.TraceParser, String.Format("Read from file [{0}] Items ", j));
			Log.Add(MessageTypeEnum.TraceParser, String.Format("Updated without error [{0}] Items ", k));
			Log.Add(MessageTypeEnum.TraceParser, String.Format("After update are in DB  [{0}] Items ", outIP));
		}

			private string GetDocumentHeaderCodeByIturCode(
	ref int IDDocumentHeader,
	string dbPath,
	string newIturCode,	 //ожидается IturCode
	int currentIturInvStatus,
	string newSessionCode)
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
				else if (currentIturInvStatus == 2 && document.Approve != true)	//else if (currentIturInvStatus == 2) newDocumentHeader.Approve = true;		
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
}
