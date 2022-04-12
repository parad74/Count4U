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
	public class InventProductUpdateCompare2SumByIturMakatFromDBParser2 : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;
		// входит второй файл
		//  текущая БД заполнена из первого файла данными 
		// documentHeader  заполнена по всем Itur на первом шаге,

		public InventProductUpdateCompare2SumByIturMakatFromDBParser2(
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
			//БЫЛО получить из БД [Itur][Makat] = Sum InventProduct by Makat
			//СТАЛО получить из входящего файла Count4UDb  [Itur][Barcode] = Sum InventProduct by Barcode
			//СТАЛО2 получить из входящего файла Count4UDb  [Itur][Code] = Sum InventProduct by Code
			// Очистить [InventProduct] & [DocumentHeader] table
			// Добавить 1 DocumentHeader в Itur и заполнить   Dictionry[Itur] = DocumentHeaderCode
			// Записать   "Sum InventProduct by Code" заменяя DocumentHeaderCode
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = "";

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			//Dictionary<string, InventProduct> inventProductFromFileDictionary = 
			//	this._inventProductRepository.GetIPQuntetyByMakatsAndIturCode(null, fromPathFile);			БЫЛО

			//Dictionary<string, InventProduct> inventProductFromFileDictionary =							БЫЛО1
			//this._inventProductRepository.GetIPQuntetyBarcodeAndIturCode(null, fromPathFile);

			Dictionary<string, InventProduct> inventProductFromFileDictionary =											  //СТАЛО
					this._inventProductRepository.GetIPQuntetyCodeAndIturCode(null, fromPathFile);


			//providerInventProduct2.Parms[ImportProviderParmEnum.FileName1] = item1.Path;
			//providerInventProduct2.Parms[ImportProviderParmEnum.FileName2] = item2.Path;
			//providerInventProduct2.FromPathFile = item2.Path;
			string fileName_1 = parms.GetStringValueFromParm(ImportProviderParmEnum.FileName1);
			string[] fileName2array = fromPathFile.Split(@"\".ToCharArray());		// item2.Path;				//  Inventor\2017\9\24\<fileName>
			string[] fileName1array = fileName_1.Split(@"\".ToCharArray());		  // item1.Path;
			string description1 = parms.GetStringValueFromParm(ImportProviderParmEnum.FileName4).CutLength(99); ;	  //descriptionInv1
			string description2 = parms.GetStringValueFromParm(ImportProviderParmEnum.FileName5).CutLength(99); ;	  //descriptionInv2


			string fileName1 = "";
			string fileName2 = "";
			try
			{
				fileName1 = fileName1array[4].CutLength(49);			//item1.Path;
			}
			catch { }
			try
			{
				fileName2 = fileName2array[4].CutLength(49);		// item2.Path;
			}
			catch { }

		


			//Dictionary<string, InventProduct> inventProductCurrentDBDictionary =
			//	this._inventProductRepository.GetIPQuntetyByMakatsAndIturCode(null, dbPath);		 БЫЛО

				//Dictionary<string, InventProduct> inventProductCurrentDBDictionary =						  БЫЛО1
				//this._inventProductRepository.GetIPQuntetyBarcodeAndIturCode(null, dbPath);


			Dictionary<string, InventProduct> inventProductCurrentDBDictionary =						  //СТАЛО
				this._inventProductRepository.GetIPQuntetyCodeAndIturCode(null, dbPath);

			IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
			Dictionary<string, Product> productDictionary = productRepository.GetProductDictionary(dbPath);

			IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();

			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//List<string> iturCodes = iturRepository.GetIturCodeList(dbPath);

			Dictionary<string, string> IturDHDictionary = new Dictionary<string, string>();
			DocumentHeaders documents = documentHeaderRepository.GetDocumentHeaders(dbPath);
			foreach (DocumentHeader document in documents)
			{
				IturDHDictionary[document.IturCode] = document.DocumentCode;
			}

	
			//очистить InventProduct
			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
			importInventProductRepository.ClearInventProducts(dbPath);

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Iturs iturs = iturRepository.GetIturs(dbPath);
			Dictionary<string, Itur> iturDictionary = iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);

			//From DB (form FILE1)
			foreach (KeyValuePair<string, InventProduct> keyValuePair in inventProductCurrentDBDictionary)
			{
				InventProduct newInventProduct = keyValuePair.Value;
				//вносим из второго файла
				//Code	   - key
				//Barcode
				//Makat
				//IturCode
				//SerialNumber
				//QuentetyEdit
				//QuentetyOriginal
				string code = newInventProduct.Code;
				string makat = newInventProduct.Makat;
				string iturCode = newInventProduct.IturCode;
				string barcode = newInventProduct.Barcode;
				string serialNumber = newInventProduct.SerialNumber;

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


				newInventProduct.IPValueInt1 = Convert.ToInt32(newInventProduct.QuantityEdit);
				newInventProduct.IPValueStr1 = fileName1;
				newInventProduct.IPValueStr4 = description1;
				newInventProduct.IPValueStr5 = description2;
				newInventProduct.IPValueStr11 = "Int1";
				newInventProduct.IPValueStr12 = "NotInt2";
				newInventProduct.IPValueStr13 = "Int1OrInt2"; 
				newInventProduct.IPValueInt2 = 0;
				newInventProduct.QuantityOriginal = 0;
				newInventProduct.IPValueInt3 = newInventProduct.IPValueInt1 - newInventProduct.IPValueInt2;
				newInventProduct.IPValueStr3 = "";
				if (newInventProduct.IPValueInt3 != 0)
				{
					//newInventProduct.QuantityEdit = 0;
					newInventProduct.IPValueStr3 = "4";
				}

				//добаляем данные для сравнение из FILE2 в 
				InventProduct fromSecondInventProduct = null;
				//string key = makat + "|" + iturCode;   БЫЛO
				//string key = barcode + "|" + iturCode;	   БЫЛO	1
				string key = code + "|" + iturCode;	   //СТАЛО
				if (inventProductFromFileDictionary.ContainsKey(key) == true)
				{
					fromSecondInventProduct = inventProductFromFileDictionary[key];		  //во 2 file с тем же ключем
					if (fromSecondInventProduct != null)
					{
						newInventProduct.IPValueInt2 = Convert.ToInt32(fromSecondInventProduct.QuantityEdit);
						newInventProduct.QuantityOriginal = fromSecondInventProduct.QuantityEdit;
						newInventProduct.IPValueInt3 = newInventProduct.IPValueInt1 - newInventProduct.IPValueInt2;
						newInventProduct.IPValueStr2 = fileName2;
						newInventProduct.IPValueStr4 = description1;
						newInventProduct.IPValueStr5 = description2;
						newInventProduct.IPValueStr12 = "Int2";
						newInventProduct.IPValueStr13 = "Int1OrInt2"; 

						newInventProduct.IPValueStr3 = "";
						if (newInventProduct.IPValueInt3 != 0) 
						{
							//newInventProduct.QuantityEdit = 0;
							//newInventProduct.QuantityOriginal = 0;
							newInventProduct.IPValueStr3 = "8";
						}
						else
						{
							newInventProduct.IPValueInt4 = newInventProduct.IPValueInt2;  
						}
						inventProductFromFileDictionary[key] = null;		 // метка что учли из второго смеджили с первым, вотрой раз не надо записвать  в БД
					}
					else
					{
						Log.Add(MessageTypeEnum.Error, "Parser ERROR 1 " + key);
					}

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
				newInventProduct.IPValueStr1 = fileName1;
				newInventProduct.IPValueStr4 = description1;
				newInventProduct.IPValueStr5 = description2;
				string erpcode = "";
				if (iturDictionary.ContainsKey(iturCode) == true)
				{
					erpcode = iturDictionary[iturCode].ERPIturCode;
					if (erpcode == null) erpcode = "";
				}
				newInventProduct.ERPIturCode = erpcode;

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

				if (newInventProduct.IPValueInt3 != 0)
				{
					newInventProduct.QuantityEdit = 0;
					newInventProduct.QuantityOriginal = 0;
				}
				yield return newInventProduct;
			}

		
			//на втором шаге from FILE2
			foreach (KeyValuePair<string, InventProduct> keyValuePair in inventProductFromFileDictionary)
			{
				InventProduct newInventProduct = keyValuePair.Value;
				
				if (newInventProduct == null)
					continue;  //значит данные уже внесены	в БД  на первом шаге

				// прошли дальше, значит не внесены в БД на первом шаге - отсутствует
				//вносим из второго файла
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

				newInventProduct.IPValueStr2 = fileName2;
				newInventProduct.IPValueStr4 = description1;
				newInventProduct.IPValueStr5 = description2;
				newInventProduct.IPValueStr12 = "Int2";
				newInventProduct.IPValueStr11 = "NotInt1";
				newInventProduct.IPValueStr13 = "Int1OrInt2"; 
				newInventProduct.IPValueInt2 = Convert.ToInt32(newInventProduct.QuantityEdit);
				newInventProduct.IPValueInt1 = 0;
				newInventProduct.QuantityOriginal = 0.0;
				newInventProduct.IPValueInt3 = newInventProduct.IPValueInt1 - newInventProduct.IPValueInt2;
				

				newInventProduct.IPValueStr3 = "";
				if (newInventProduct.IPValueInt3 != 0)
				{
					//newInventProduct.QuantityEdit = 0;
					//newInventProduct.QuantityOriginal = 0;
					newInventProduct.IPValueStr3 = "16";
				}

				//Есть в  newInventProduct
				//Code	   - key
				//Barcode
				//Makat
				//IturCode
				//SerialNumber

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

				if (newInventProduct.IPValueInt3 != 0)
				{
					newInventProduct.QuantityEdit = 0;
					newInventProduct.QuantityOriginal = 0;
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
