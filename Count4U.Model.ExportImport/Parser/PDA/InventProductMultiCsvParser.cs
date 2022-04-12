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
	public class InventProductMultiCsvParser : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, DocumentHeader> documentHeaderFromDBDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductMultiCsvParser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			//this._iturInFileDictionary = new Dictionary<string, DocumentHeader>();
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

			string newDocumentCodePrefix = parms.GetStringValueFromParm(ImportProviderParmEnum.NewDocumentCode);

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);

			
			//string newDocumentCode = Guid.NewGuid().ToString(); 

			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = "MultiCSV";
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newIturCode = "00010001";

			string filename = System.IO.Path.GetFileName(fromPathFile);
			filename = filename.CutLength(49);

			//Dictionary<string, int> indexInRecordDictionary = parms.GetIPAdvancedFieldIndexDictionaryFromParm();

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();
			//this._iturInFileDictionary.Clear();

			string separator = "|";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();

			//this._iturInFileDictionary = _documentHeaderRepository.GetIturDocumentCodeDictionary(dbPath);

			documentHeaderFromDBDictionary =
					this._documentHeaderRepository.GetDocumentHeaderDictionary(dbPath, true);

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> dictionaryIturCode = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCode = iturRepository.GetIturDictionary(dbPath, true);
			}
			catch { }

			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = iturRepository.GetERPIturDictionary(dbPath);
			}
			catch { }

			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			//Dictionary<string, InventProduct> dictionaryInventProductCode = new Dictionary<string, InventProduct>();
			//try
			//{
			//	// count4UDB
			//	dictionaryInventProductCode = inventProductRepository.GetDictionaryInventProductsCode(dbPath);
			//}
			//catch { }

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{

				if (record == null) continue;
				int countRecord = record.Length;
				if (countRecord < 5)
				{
						continue;
				}

				//	Field 1 = Itur Code						 0
				//Field 2 = ERP Itur Code				 1
				//Field 3 = Barcode or ItemCode		 2
				//Field 4 = QuantityEdit				     3
				//Field 5 = PartialQuantity				 4

				newIturCode = record[0].Trim();
				
				if (dictionaryIturCode.ContainsKey(newIturCode) == false)
				{
					//нет  newIturCode
					string newErpIturCode = record[1].Trim();
					if (dictionaryIturCodeERP.ContainsKey(newErpIturCode) == false)
					{
						// нет 	 newIturCode и нет ErpIturCode
						newIturCode = "99999999";
					}
					else
					{
						newIturCode = dictionaryIturCodeERP[newErpIturCode].IturCode;
					}
				}
				

				if (dictionaryIturCode.ContainsKey(newIturCode) == false)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.IturCodeNotExistInDB, newIturCode));
					continue;
				}

				Itur currentItur = dictionaryIturCode[newIturCode];
				string erpIturCode = currentItur.ERPIturCode;
				string locationCode = currentItur.LocationCode;
				int currentIturInvStatus = currentItur.InvStatus;
				//string serialNumber = "";
				//string serialNumberSupplier = "";

				//Field 3 = Barcode or ItemCode		 2
				//Field 4 = QuantityEdit				     3
				//Field 5 = PartialQuantity				 4

				string makat = record[2].CutLength(299);
				
				if (makatApplyMask == true)
				{
					makat = maskPackage.MakatMaskTemplate.FormatString(
						makat, maskPackage.MakatMaskRecord.Value);
				}

				string barcode = makat;

				int IDDocumentHeader = 0;
				string currentDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, dbPath, newDocumentCodePrefix, newIturCode, currentIturInvStatus, newSessionCode);
				//string currentDocumentCode = newDocumentCodePrefix + "_" + newIturCode;

				InventProductSimpleString newInventProductString = new InventProductSimpleString();
				InventProduct newInventProduct = new InventProduct();
				newInventProductString.Makat = makat;
				newInventProductString.Barcode = barcode;
				newInventProductString.IturCode = newIturCode;
				newInventProductString.DocumentCode = currentDocumentCode;
				newInventProductString.SessionCode = newSessionCode;
				newInventProductString.WorkerID = newWorkerID;
				newInventProductString.ProductName = "NotExistInCatalog";
				newInventProductString.QuantityOriginal = "1";
				newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();
				newInventProductString.QuantityOriginal = record[3];
				newInventProductString.QuantityInPackEdit = record[4];
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

				newInventProduct.IturCode = newIturCode;
				if (erpIturCode != null) newInventProduct.ERPIturCode = erpIturCode;
				newInventProduct.DocumentCode = currentDocumentCode;
				newInventProduct.SessionCode = newSessionCode;
				newInventProduct.WorkerID = filename;
				newInventProduct.SectionNum = 0;
				newInventProduct.CreateDate = DateTime.Now;
				newInventProduct.ModifyDate = DateTime.Now;
				
				
				//newInventProduct.DocNum = Convert.ToInt32(IDDocumentHeader);

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
			
				newInventProduct.WorkerID = filename;
				newInventProduct.Code = (newInventProduct.Makat + "^" + newInventProduct.Barcode).CutLength(299); 
				yield return newInventProduct;
			} 

		}

			private string GetDocumentHeaderCodeByIturCode(
	ref int IDDocumentHeader,
	string dbPath,
	string newDocumentCodePrefix,
	string newIturCode,	 //ожидается IturCode
	int currentIturInvStatus,
	string newSessionCode)
		{
			string retDocumentCode = newDocumentCodePrefix + "_" + newIturCode; //+ "_99";
			if (this.documentHeaderFromDBDictionary.ContainsKey(retDocumentCode) == true) 
			{
				DocumentHeader document = this.documentHeaderFromDBDictionary[retDocumentCode];
				if (document != null) IDDocumentHeader = Convert.ToInt32(document.ID);
			
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
				DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
				DocumentHeader newDocumentHeader = new DocumentHeader();
				newDocumentHeaderString.DocumentCode = retDocumentCode;
				newDocumentHeaderString.SessionCode = newSessionCode;				//in
				newDocumentHeaderString.CreateDate = this._rowDocumentHeader.CreateDate;
				newDocumentHeaderString.WorkerGUID = "MultiCSV";
				newDocumentHeaderString.IturCode = newIturCode;
				newDocumentHeaderString.Name = "MultiCSV";//this._rowDocumentHeader.Name;
				//newDocumentHeaderString.WorkerGUID = this._rowDocumentHeader.WorkerGUID;

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
					this.documentHeaderFromDBDictionary[retDocumentCode] = newDocumentHeader; 

					if (retBitDocumentHeader != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.WarningParser });
					}
				}
				return retDocumentCode;
			}

		}


			protected static void GetApplyMasks(List<ImportDomainEnum> importType, MaskPackage maskPackage,
		out bool makatApplyMask, out bool barcodeApplyMask)
			{
				makatApplyMask = false;
				barcodeApplyMask = false;

				if (importType.Contains(ImportDomainEnum.MakatApplyMask) == true)
					if (maskPackage.MakatMaskTemplate != null
						&& maskPackage.MakatMaskRecord != null
						&& string.IsNullOrWhiteSpace(maskPackage.MakatMaskRecord.Value) == false)
					{
						makatApplyMask = true;
					}

				if (importType.Contains(ImportDomainEnum.BarcodeApplyMask) == true)
					if (maskPackage.BarcodeMaskTemplate != null
						&& maskPackage.BarcodeMaskRecord != null
						&& string.IsNullOrWhiteSpace(maskPackage.BarcodeMaskRecord.Value) == false)
					{
						barcodeApplyMask = true;
					}
			}
	}
}
