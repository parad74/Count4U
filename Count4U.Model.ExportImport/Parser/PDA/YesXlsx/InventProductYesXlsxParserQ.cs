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
	public class InventProductYesXlsxParserQ : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, DocumentHeader> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductYesXlsxParserQ(
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
			string newIturCode = "00010001";

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
			Dictionary<string, Itur> dictionaryIturCode = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCode = iturRepository.GetIturDictionary(dbPath);
			}
			catch { }

			//Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			//try
			//{
			//	// count4UDB
			//	dictionaryIturCodeERP = iturRepository.GetERPIturDictionary(dbPath);
			//}
			//catch { }

			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			Dictionary<string, InventProduct> dictionaryInventProductCode = new Dictionary<string, InventProduct>();
			try
			{
				// count4UDB
				dictionaryInventProductCode = inventProductRepository.GetDictionaryInventProductsUID(dbPath);
			}
			catch { }

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{

				if (record == null) continue;
				int countRecord = record.Length;
				if (countRecord < 6)
				{
						continue;
				}

				//	Q *
				//0 Location.Level1.Code
				//1 Location.Level2.Code
				//2 Location.Level2.Name
				//3
				//4 ItemCode
				//5 ItemName
				//Quantity
				//add Quantity default = 1

				string prfix = record[0].LeadingZero4();
				string suffix = record[1].LeadingZero4();
				newIturCode = prfix + suffix;

				if (dictionaryIturCode.ContainsKey(newIturCode) == false)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.IturCodeNotExistInDB, newIturCode));
					continue;
				}
				Itur currentItur = dictionaryIturCode[newIturCode];
				string erpIturCode = currentItur.ERPIturCode;
				string locationCode = currentItur.LocationCode;
				int currentIturInvStatus = currentItur.InvStatus;
				string serialNumber = "";
				string serialNumberSupplier = "";
				string makat = record[4].CutLength(299);
				string productName = record[5].CutLength(99);
				string barcode = makat;

				//Nativ + 4 составной ключ 
				string[] ids = new string[] { serialNumber, makat, erpIturCode, serialNumberSupplier };	// 4 - составной ключ для SN
				string ID = ids.JoinRecord("|");

				int IDDocumentHeader = 0;
				string currentDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, dbPath, newIturCode, currentIturInvStatus, newSessionCode);

				//			Q *
				//0 Location.Level1.Code
				//1 Location.Level2.Code
				//2 Location.Level2.Name
				//3
				//4 ItemCode
				//5 ItemName
				//Quantity
				//add Quantity default = 1
	
			
				InventProductSimpleString newInventProductString = new InventProductSimpleString();
				InventProduct newInventProduct = new InventProduct();
				newInventProductString.Makat = makat;
				newInventProductString.Barcode = ID.CutLength(299);
				newInventProductString.IturCode = newIturCode;
				newInventProductString.DocumentCode = currentDocumentCode;
				newInventProductString.SessionCode = newSessionCode;
				newInventProductString.WorkerID = newWorkerID;
				newInventProductString.ProductName = "NotExistInCatalog";
				newInventProductString.QuantityOriginal = "1";
				//newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();
				//newInventProductString.ImputTypeCodeFromPDA = record[3];
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
				newInventProduct.IturCode = newIturCode;
				if (erpIturCode != null) newInventProduct.ERPIturCode = erpIturCode;
				newInventProduct.DocumentCode = currentDocumentCode;
				newInventProduct.SessionCode = newSessionCode;
				newInventProduct.WorkerID = filename;
				newInventProduct.SectionNum = 0;
				
				
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
				dictionaryInventProductCode[ID] = newInventProduct;
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
