using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class InventProductUpdateCompare2SumByIturMakatFromDBParser1 : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;

		public InventProductUpdateCompare2SumByIturMakatFromDBParser1(
			IInventProductRepository inventProductRepository,
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			if (inventProductRepository == null) throw new ArgumentNullException("iturRepository");

			this._inventProductRepository = inventProductRepository;
		}

		/// <summary>
		/// Получение списка InventProduct  
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InventProduct> GetInventProducts(
		string fromPathFile,
		Encoding encoding, string[] separators,
		int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
		Dictionary<string, ProductMakat> productMakatDictionary,
		Dictionary<string, Itur> iturFromDBDictionary,
		List<ImportDomainEnum> importType,
		Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			// входит первый файл
			//  текущая БД. InventProduct - Чистая 
			// documentHeader - чистая
			// добавляем в каждый Itur один DocumentHeader

			//БЫЛО получить из входящего файла Count4UDb  [Itur][Makat] = Sum InventProduct by Makat
			//СТАЛО получить из входящего файла Count4UDb  [Itur][Barcode] = Sum InventProduct by Barcode
			//СТАЛО2 получить из входящего файла Count4UDb  [Itur][Code] = Sum InventProduct by Code
			// Очистить [InventProduct] & [DocumentHeader] table
			// Добавить 1 DocumentHeader в Itur и заполнить   Dictionry[Itur] = DocumentHeaderCode
			// Записать   "Sum InventProduct by Code" заменяя DocumentHeaderCode
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = "";

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string description1 = parms.GetStringValueFromParm(ImportProviderParmEnum.FileName4).CutLength(99);	  //descriptionInv1
			string description2 = parms.GetStringValueFromParm(ImportProviderParmEnum.FileName5).CutLength(99);	  //descriptionInv2


			//Dictionary<string, InventProduct> inventProductFromFileDictionary = 
			//	this._inventProductRepository.GetIPQuntetyByMakatsAndIturCode(null, fromPathFile);	   БЫЛО
			//Dictionary<string, InventProduct> inventProductFromFileDictionary =
			//	this._inventProductRepository.GetIPQuntetyBarcodeAndIturCode(null, fromPathFile);	   БЫЛО1

			//ToDictionary(k => k.Code + "|" + k.IturCode);
			Dictionary<string, InventProduct> inventProductFromFileDictionary =
				this._inventProductRepository.GetIPQuntetyCodeAndIturCode(null, fromPathFile);

			//providerInventProduct1.FromPathFile = item1.Path;
			//providerInventProduct1.Parms[ImportProviderParmEnum.FileName1] = item1.Path;
			//providerInventProduct1.Parms[ImportProviderParmEnum.FileName2] = item2.Path;

			string fileName_1 = parms.GetStringValueFromParm(ImportProviderParmEnum.FileName1);		   //item1.Path;
			string fileName_2 = parms.GetStringValueFromParm(ImportProviderParmEnum.FileName2);			 //item2.Path;

			string[] fileName1Array = fromPathFile.Split(@"\".ToCharArray());		//item1.Path;				//  Inventor\2017\9\24\<fileName>
			string[] fileName2array = fileName_2.Split(@"\".ToCharArray());		// item2.Path;		

			string fileName1 = "";
			string fileName2 = "";
			try
			{
				fileName1 = fileName1Array[4].CutLength(49);			   //item1.Path;
			}
			catch { }
			try
			{
				fileName2 = fileName2array[4].CutLength(49);		// item2.Path;
			}
			catch { }
		
			IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
			Dictionary<string, Product> productDBDictionary = productRepository.GetProductDictionary(dbPath);

			IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Iturs iturs = iturRepository.GetIturs(dbPath);
			Dictionary<string, Itur> iturDictionary = iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);

		//	List<string> iturDBCodes = this._inventProductRepository.GetIturCodeList(null, dbPath);	  //было все ItursCode from Iturs table

			List<string> iturDBCodes1 = this._inventProductRepository.GetIturCodeList(null, fileName_1);
			List<string> iturDBCodes2 = this._inventProductRepository.GetIturCodeList(null, fileName_2);
			List<string> iturDBCodes = iturDBCodes1;
			foreach(string iturCode in iturDBCodes2)
			{
				if (iturDBCodes1.Contains(iturCode) == false)
				{
					iturDBCodes.Add(iturCode);
				}
			}

			// добавляем в каждый Itur один DocumentHeader
			DocumentHeaders documentHeaders = new DocumentHeaders();
			Dictionary<string, string> IturDHDictionary = new Dictionary<string, string>();
			foreach (string iturCode in iturDBCodes)
			{
				string newDocumentCode = Guid.NewGuid().ToString();
				IturDHDictionary[iturCode] = newDocumentCode;
				DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
				DocumentHeader newDocumentHeader = new DocumentHeader();
				newDocumentHeaderString.DocumentCode = newDocumentCode;
				newDocumentHeaderString.SessionCode = newSessionCode;				//in
				newDocumentHeaderString.CreateDate = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
				newDocumentHeaderString.WorkerGUID = "00001";
				newDocumentHeaderString.IturCode = iturCode;
				newDocumentHeaderString.Name = "00001";

				int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
				if (retBitDocumentHeader != 0)  //Error
				{
					continue;
				}
				else //	Error  retBitSession == 0 
				{
					retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
					newDocumentHeader.Approve = null;//false было  //first Document in Itur
					documentHeaders.Add(newDocumentHeader);
					//ужас долго
					// int IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
				}
			}		 // Add DocumentHeader for Itur

			//base._documentHeaderRepository.Insert(documentHeaders, dbPath);
			IImportDocumentHeaderBlukRepository importDocumentHeaderBlukRepository = this._serviceLocator.GetInstance<IImportDocumentHeaderBlukRepository>();
			importDocumentHeaderBlukRepository.InsertDocumentHeaders(documentHeaders, dbPath);

			//очистить InventProduct   (должна быть и так чистая)
			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
			importInventProductRepository.ClearInventProducts(dbPath);

			// Form FILE1, если первый шаг 		   + работает
			foreach (KeyValuePair<string, InventProduct> keyValuePair in inventProductFromFileDictionary)
			{
				InventProduct newInventProduct = keyValuePair.Value;
				//Code	   - key
				//Barcode
				//Makat
				//IturCode
				//SerialNumber
				//QuentetyEdit
				//QuentetyOriginal
				string code = newInventProduct.Code;
				string makat = newInventProduct.Makat;
				string barcode = newInventProduct.Barcode;
				string serialNumber = newInventProduct.SerialNumber;
				string iturCode = newInventProduct.IturCode;

				string documentCode = Guid.NewGuid().ToString();
				if (IturDHDictionary.ContainsKey(iturCode) == true)
				{
					documentCode = IturDHDictionary[iturCode];
				}
				else
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.IturCodeNotExistInDB, iturCode));
					continue;
				}

				// Есть в newInventProduct
				//Code
				//Barcode
				//Makat
				//IturCode
				//SerialNumber
				//QuentetyEdit
				//QuentetyOriginal
				//newInventProduct.Makat = makat;

				newInventProduct.IPValueStr1 = fileName1;
				newInventProduct.IPValueStr4 = description1;
				newInventProduct.IPValueStr5 = description2;
				newInventProduct.IPValueStr11 = "Int1";
				newInventProduct.IPValueStr12 = "NotInt2";
				newInventProduct.IPValueStr13 = "Int1OrInt2"; 
				newInventProduct.IPValueInt1 = Convert.ToInt32(newInventProduct.QuantityEdit);
				newInventProduct.IPValueInt2 = 0;
				newInventProduct.QuantityOriginal = 0;
				
				newInventProduct.IPValueInt3 = newInventProduct.IPValueInt1 - newInventProduct.IPValueInt2;
				newInventProduct.IPValueStr3 = "";
				if (newInventProduct.IPValueInt3 != 0)		
				{
					//newInventProduct.QuantityEdit = 0;
					newInventProduct.IPValueStr3 = "4";
				}

				//newInventProduct.Barcode = makat;
				newInventProduct.DocumentCode = documentCode;
				newInventProduct.DocumentHeaderCode = documentCode;
				newInventProduct.SessionCode = newSessionCode;
				newInventProduct.WorkerID = "00001";
				newInventProduct.ProductName = "NotExistInCatalog";
				newInventProduct.InputTypeCode = InputTypeCodeEnum.B.ToString();
				newInventProduct.CreateDate = DateTime.Now;
				newInventProduct.StatusInventProductBit = 0;
				string erpcode = "";
				if (iturDictionary.ContainsKey(iturCode) == true)
				{
					erpcode = iturDictionary[iturCode].ERPIturCode;
					if (erpcode == null) erpcode = "";
				}
				newInventProduct.ERPIturCode = erpcode;
				Product newProduct = new Product();
				if (productDBDictionary.ContainsKey(makat) == true)
				{
					newProduct = productDBDictionary[makat];
					newInventProduct.ProductName = newProduct.Name;
					newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
					newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
					newInventProduct.ImputTypeCodeFromPDA = newProduct.UnitTypeCode;
					newInventProduct.SectionCode = newProduct.SectionCode;
					newInventProduct.SectionName = newProduct.SectionName;
					newInventProduct.SupplierName = newProduct.SupplierName;
					newInventProduct.SupplierCode = newProduct.SupplierCode;
					newInventProduct.SectionNum = 0;
				}
				else
				{
					newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
					newInventProduct.ProductName = "NotExistInCatalog";
					newInventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
					newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
				}


				yield return newInventProduct;
			}

		


			//InventProducts inventProductFromDB = this._inventProductRepository.GetInventProducts(dbPath);
			//string statusInventProductCode = fromPathFile.Replace("Inventor", "").CutLength(49);
			//foreach (var newInventProduct in inventProductFromDB)
			//{
			//	newInventProduct.StatusInventProductCode = statusInventProductCode;
			//	yield return newInventProduct;
			//}
		}
	}
}
