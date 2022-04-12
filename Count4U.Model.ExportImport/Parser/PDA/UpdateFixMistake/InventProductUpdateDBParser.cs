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
	public class InventProductUpdateDBParser : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;

		public InventProductUpdateDBParser(
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

			InventProducts inventProductFromDB = this._inventProductRepository.GetInventProducts(dbPath);
			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
			importInventProductRepository.ClearInventProducts(dbPath);
//			 #1293

//			1. Go record by record from CurrentInventory
//2. Check if itemCode type = SN
//3. Check if SerialNumberSupplier is NULL or SerialNumberSupplier = PropertyStr8
//4. If 2 & 3 then do:
//Search for all available appearance on Merkava-Infrastructure.xlsx -> Sheet PreviousInventory
//Where
//CurrentInventory.ItemCode = PreviousInventory.ItemCode &
//CurrentInventory.SerialNumberLocal = PreviousInventory.SerialNumberLocal &
//CurrentInventory.PropertyStr1 = PreviousInventory.PropertyStr1 &
//5. Change current CurrentInventory.SerialNumberSupplier to PreviousInventory.SerialNumberSupplier
//6. if CurrentInventory.PropertyStr10 is empty
//Change current CurrentInventory.PropertyStr10 to PreviousInventory.PropertyStr10
//7. if CurrentInventory.PropertyStr11 is empty
//Change current CurrentInventory.PropertyStr11 to PreviousInventory.PropertyStr11
//In case there are more then one record on PreviousInventory - use the latest (last appearance on the excel sheet)

			int k = 0;
			foreach (var newInventProduct in inventProductFromDB)
			{
				newInventProduct.ShelfCode = "-1";
				string serialNumberLocal = newInventProduct.SerialNumber;
				string makat = newInventProduct.Makat;
				string propertyStr1 = newInventProduct.IPValueStr1;

				//Where
				//CurrentInventory.ItemCode = PreviousInventory.ItemCode &
				//CurrentInventory.SerialNumberLocal = PreviousInventory.SerialNumberLocal &
				//CurrentInventory.PropertyStr1 = PreviousInventory.PropertyStr1 
				string key = serialNumberLocal + "|" + makat + "|" + propertyStr1;

				if (productMakatDictionary.ContainsKey(makat) == true)
				{
					string unitTypeCode = productMakatDictionary[makat].UnitTypeCode;
					//2. Check if itemCode type = SN
					if (unitTypeCode == "SN")
					{
						if (dictionaryPreviousInventory.ContainsKey(key) == true)
						{
							PreviousInventory previousInventory = dictionaryPreviousInventory[key];	 //todo
							if (previousInventory != null)
							{	 
								//3. Check if SerialNumberSupplier is NULL or SerialNumberSupplier = PropertyStr8
								if (string.IsNullOrWhiteSpace(newInventProduct.SupplierCode) == true
									|| newInventProduct.SupplierCode == newInventProduct.IPValueStr8)
								{
									//dictionaryPreviousInventory[key]
									//key
									//CurrentInventory.ItemCode = PreviousInventory.ItemCode &
									//CurrentInventory.SerialNumberLocal = PreviousInventory.SerialNumberLocal &
									//CurrentInventory.PropertyStr1 = PreviousInventory.PropertyStr1 
									bool change = false;
									newInventProduct.StatusInventProductCode = previousInventory.SerialNumberSupplier;
									if (string.IsNullOrWhiteSpace(previousInventory.SerialNumberSupplier) == false)
									{
										if (string.IsNullOrWhiteSpace(newInventProduct.SupplierCode) == true)
										{
											newInventProduct.SupplierCode = previousInventory.SerialNumberSupplier.CutLength(249);				
											change = true;
										}
									}

									//6. if CurrentInventory.PropertyStr10 is empty
									//Change current CurrentInventory.PropertyStr10 to PreviousInventory.PropertyStr10
									if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr10) == true)
									{
										if (string.IsNullOrWhiteSpace(previousInventory.PropertyStr10) == false)
										{
											newInventProduct.IPValueStr10 = previousInventory.PropertyStr10.CutLength(49);
											change = true;
										}
									}

									//7. if CurrentInventory.PropertyStr11 is empty
									//Change current CurrentInventory.PropertyStr11 to PreviousInventory.PropertyStr11
									if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr11) == true)
									{
										if (string.IsNullOrWhiteSpace(previousInventory.PropertyStr11) == false)
										{
											newInventProduct.IPValueStr11 = previousInventory.PropertyStr11.CutLength(49);
											change = true;
										}
									}

									//7. if CurrentInventory.PropertyStr1 is empty
									//Change current CurrentInventory.PropertyStr1 to PreviousInventory.PropertyStr1
									if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr1) == true)
									{
										if (string.IsNullOrWhiteSpace(previousInventory.PropertyStr1) == false)
										{
											newInventProduct.IPValueStr1 = previousInventory.PropertyStr1.CutLength(49);
											change = true;
										}
									}

									if (change == true)
									{
										k++;
										newInventProduct.ShelfCode = k.ToString().PadLeft(4, '0');
										Log.Add(MessageTypeEnum.TraceParser, "Update InventProduct 1 :: Barcode = " + newInventProduct.Barcode
										+ ", SerialNumber = " + newInventProduct.SerialNumber
										+ ", SupplierCode = " + newInventProduct.SupplierCode
										+ ", propertyStr1 = " + newInventProduct.IPValueStr1
										+ ", propertyStr10 = " + newInventProduct.IPValueStr10
										+ ", propertyStr11 = " + newInventProduct.IPValueStr11);
									}
								}
							}
						}
					}
				}
				yield return newInventProduct;
			}
			Log.Add(MessageTypeEnum.TraceParser, "Update InventProduct [1] :: k = " + k);
		}
	}
}
