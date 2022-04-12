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
	public class InventProductUpdate2SumByIturDocMakatFromDBParser : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;
		//private readonly ILog _log;
		//private Dictionary<string, Itur> _iturDictionary;
		//private List<BitAndRecord> _errorBitList;
		//public DateTimeFormatInfo _dtfi;

		public InventProductUpdate2SumByIturDocMakatFromDBParser(
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
			//получить из БД [IturCode|DocumentCode][Makat] = Sum InventProduct by Makat
			// Очистить [InventProduct] 
			// HET  Очистить [DocumentHeader] table
			 // HET Добавить 1 DocumentHeader в Itur и 
			// HET заполнить   Dictionry[Itur] = DocumentHeaderCode
			// Записать   "Sum InventProduct by Makat" HE заменяя DocumentHeaderCode
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = "";

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			//получить из БД [IturCode|DocumentCode][Makat] = Sum InventProduct by Makat
			Dictionary<string, InventProduct>  inventProductDictionary = this._inventProductRepository.GetIPQuntetyByMakatsAndDocAndIturCode(null, dbPath);

			// Delete DocumentHeaders and  Add new DocumentHeaders for Itur
			IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
			documentHeaderRepository.DeleteAllDocumentsWithoutAnyInventProduct(dbPath);

			IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
			Dictionary<string, Product> productDictionary = productRepository.GetProductDictionary(dbPath);
		
			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//List<string> iturCodes = iturRepository.GetIturCodeList(dbPath);

			//Dictionary<string, string> IturDHDictionary = new Dictionary<string, string>();
			//foreach (string iturCode in iturCodes)
			//{
			//	string newDocumentCode = Guid.NewGuid().ToString();
			//	IturDHDictionary[iturCode] = newDocumentCode;
			//	DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
			//	DocumentHeader newDocumentHeader = new DocumentHeader();
			//	newDocumentHeaderString.DocumentCode = newDocumentCode;
			//	newDocumentHeaderString.SessionCode = newSessionCode;				//in
			//	newDocumentHeaderString.CreateDate = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
			//	newDocumentHeaderString.WorkerGUID = "00001";
			//	newDocumentHeaderString.IturCode = iturCode;
			//	newDocumentHeaderString.Name = "00001";

			//	int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
			//	if (retBitDocumentHeader != 0)  //Error
			//	{
			//		continue;
			//	}
			//	else //	Error  retBitSession == 0 
			//	{
			//		retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
			//		newDocumentHeader.Approve = null;//false было  //first Document in Itur
			//		int IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
			//	}
			//}		 // Add DocumentHeader for Itur

			//очистить InventProduct
			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
			importInventProductRepository.ClearInventProducts(dbPath);

			foreach (KeyValuePair<string, InventProduct> keyValuePair in inventProductDictionary)
			{
				InventProduct newInventProduct = keyValuePair.Value;
				string makat = newInventProduct.Makat;
				string iturCode = newInventProduct.IturCode;
				string documentCode = newInventProduct.DocumentCode;
				//string documentCode = Guid.NewGuid().ToString();
				//if (IturDHDictionary.ContainsKey(iturCode) == true)
				//{
				//	documentCode = IturDHDictionary[iturCode];
				//}
	
				// Есть в newInventProduct
				//makat
				//iturCode
				//QuentetyEdit
				//QuentetyOriginal
				//newInventProduct.Makat = makat;
				newInventProduct.Barcode = makat;
				//newInventProduct.DocumentCode = documentCode;
				newInventProduct.DocumentHeaderCode = documentCode;
				newInventProduct.SessionCode = newSessionCode;
				newInventProduct.WorkerID = "00001";
				newInventProduct.ProductName = "NotExistInCatalog";
				newInventProduct.InputTypeCode = InputTypeCodeEnum.B.ToString();
				newInventProduct.CreateDate = DateTime.Now;
				newInventProduct.StatusInventProductBit = 0;

				Product newProduct = new Product();
				if (productDictionary.ContainsKey(makat) == true)
				{
				   newProduct = productDictionary[makat]; 
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
