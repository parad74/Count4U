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
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class InventProductUpdateDBParser3old : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;

		public InventProductUpdateDBParser3old(
			IInventProductRepository inventProductRepository,
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base(documentHeaderRepository, serviceLocator, log)
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
			IPreviousInventoryRepository _previousInventoryEFRepository = this._serviceLocator.GetInstance<IPreviousInventoryRepository>();
			Dictionary<string, PreviousInventory> dictionaryPreviousInventory = _previousInventoryEFRepository.GetDictionaryPreviousInventorys(fromPathFile);
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			//ProductMakat productMakat = productMakatDictionary[];

			//IEnumerable<Product> productFromDBSimples = this._productRepository.GetProducts(dbPath);
			////this._productRepository.DeleteAll(dbPath);
			//IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();
			//	IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();

			IPreviousInventorySQLiteParser previousInventorySQLiteParser =
				this._serviceLocator.GetInstance<IPreviousInventorySQLiteParser>(PreviousInventorySQLiteParserEnum.PreviousInventoryMerkavaXslx2DictiontyParser.ToString());
			IEnumerable<PreviousInventory> previousInventoryList = previousInventorySQLiteParser.GetPreviousInventory(fromPathFile,
			encoding, separators, countExcludeFirstString, new Dictionary<string, PreviousInventory>(), parms);
			var dictionaryPreviousInventoryFromFile = new Dictionary<string, PreviousInventory>();
			dictionaryPreviousInventoryFromFile = previousInventoryList.Select(e => e).Distinct().ToDictionary(x => x.Uid);
			//!!! todo UID - собран неполным LocationCode

			InventProducts inventProductFromDB = this._inventProductRepository.GetInventProducts(dbPath);
			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
			//importInventProductRepository.ClearInventProducts(dbPath);			   //!! Вернуть 

			int k = 0;

			foreach (var newInventProduct in inventProductFromDB)
			{
				if (newInventProduct.ImputTypeCodeFromPDA == "SN")
				{
					if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr10) == true
						|| string.IsNullOrWhiteSpace(newInventProduct.IPValueStr11) == true
						|| string.IsNullOrWhiteSpace(newInventProduct.IPValueStr1) == true)
					{
						PreviousInventory previousInventory = dictionaryPreviousInventoryFromFile[newInventProduct.Barcode];
						if (previousInventory != null)
						{
							string propertyStr10 = previousInventory.PropertyStr10;
							string propertyStr11 = previousInventory.PropertyStr11;
							string propertyStr1 = previousInventory.PropertyStr1;
							bool fix = false;

							if (string.IsNullOrWhiteSpace(propertyStr10) == false)
							{
								if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr10) == true)
								{
									newInventProduct.IPValueStr10 = propertyStr10;
									fix = true;
								}
							}

							if (string.IsNullOrWhiteSpace(propertyStr1) == false)
							{
								if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr1) == true)
								{
									newInventProduct.IPValueStr1 = propertyStr1;
									fix = true;
								}
							}

							if (string.IsNullOrWhiteSpace(propertyStr11) == false)
							{
								if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr11) == true)
								{
									newInventProduct.IPValueStr11 = propertyStr11;
									fix = true;
								}
							}

							if (fix == true)
							{
								k++;
								Log.Add(MessageTypeEnum.TraceParser, "Update InventProduct :: Barcode = " + newInventProduct.Barcode
							+ ", SerialNumber = " + newInventProduct.SerialNumber
							+ ", SupplierCode = " + newInventProduct.SupplierCode
							+ ", propertyStr1 = " + propertyStr1
							+ ", propertyStr10 = " + propertyStr10
							+ ", propertyStr11 = " + propertyStr11);
							}
						}
					}
				}

				yield return newInventProduct;
			}
			Log.Add(MessageTypeEnum.TraceParser, "Update SN Supplier k = " + k);
		}
	}
}
