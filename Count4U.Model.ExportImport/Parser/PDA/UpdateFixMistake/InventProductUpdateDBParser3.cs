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
	public class InventProductUpdateDBParser3 : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;

		public InventProductUpdateDBParser3(
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

			int k = 0;

			foreach (var newInventProduct in inventProductFromDB)
			{
				if (newInventProduct.ImputTypeCodeFromPDA == "SN")
				{
					if (newInventProduct.IPValueStr10.Trim() == "0") newInventProduct.IPValueStr10 = "";
					if (newInventProduct.IPValueStr11.Trim() == "0") newInventProduct.IPValueStr11 = "";

					if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr10) == true
						&& string.IsNullOrWhiteSpace(newInventProduct.IPValueStr11) == true)
					{
						//List<PreviousInventory> previousInventorys = 
						//	_previousInventoryEFRepository.GetListByItemCode(newInventProduct.Makat, fromPathFile);
							
						List<PreviousInventory> previousInventorys = null;
						if (string.IsNullOrWhiteSpace(newInventProduct.SupplierCode) == false
							&& string.IsNullOrWhiteSpace(newInventProduct.SerialNumber) == false)
						{
							previousInventorys =
								_previousInventoryEFRepository.GetListBySerialNumberSupplierOrSerialNumberLocal(newInventProduct.SupplierCode, newInventProduct.SerialNumber, dbPath);
						}

						if (string.IsNullOrWhiteSpace(newInventProduct.SupplierCode) == true
						&& string.IsNullOrWhiteSpace(newInventProduct.SerialNumber) == false)		   //есть
						{
							previousInventorys =
								_previousInventoryEFRepository.GetListBySerialNumberSupplier(newInventProduct.SupplierCode, dbPath);
						}

						if (string.IsNullOrWhiteSpace(newInventProduct.SupplierCode) == false			 //есть
						&& string.IsNullOrWhiteSpace(newInventProduct.SerialNumber) == true)
						{
							previousInventorys =
								_previousInventoryEFRepository.GetListBySerialNumberLocal( newInventProduct.SerialNumber, dbPath);
						}

						if (previousInventorys != null)
						{
							if (previousInventorys.Count > 0)
							{
								string propertyStr10 = "";
								string propertyStr11 = "";
								string propertyStr1 = "";
								foreach (var pInventor in previousInventorys)
								{
									if (pInventor.ItemCode == newInventProduct.Makat) continue;
									if (pInventor != null)
									{
										bool isDoFixSerialNumberLocal = false;
										if (string.IsNullOrWhiteSpace(pInventor.SerialNumberLocal) == false)
										{
											isDoFixSerialNumberLocal = pInventor.SerialNumberLocal.Trim().ToLower() == newInventProduct.SerialNumber.Trim().ToLower();
										}

										bool isDoFixSerialNumberSupplier = false;
										if (string.IsNullOrWhiteSpace(pInventor.SerialNumberSupplier) == false)
										{
											isDoFixSerialNumberSupplier = pInventor.SerialNumberSupplier.Trim().ToLower() == newInventProduct.SupplierCode.Trim().ToLower();
										}

										if (isDoFixSerialNumberLocal || isDoFixSerialNumberSupplier)
										{
											if (string.IsNullOrWhiteSpace(pInventor.PropertyStr10) == false
											|| string.IsNullOrWhiteSpace(pInventor.PropertyStr11) == false
											|| string.IsNullOrWhiteSpace(pInventor.PropertyStr1) == false)
											{
												//6. Change current CurrentInventory.PropertyStr10 to PreviousInventory.PropertyStr10
												//7. Change current CurrentInventory.PropertyStr11 to PreviousInventory.PropertyStr11
												//7. Change current CurrentInventory.PropertyStr1 to PreviousInventory.PropertyStr1
												if (string.IsNullOrWhiteSpace(pInventor.PropertyStr10) == false)
												{
													propertyStr10 = pInventor.PropertyStr10;
												}
												if (string.IsNullOrWhiteSpace(pInventor.PropertyStr11) == false)
												{
													propertyStr11 = pInventor.PropertyStr11;
												}
												if (string.IsNullOrWhiteSpace(pInventor.PropertyStr1) == false)
												{
													propertyStr1 = pInventor.PropertyStr1;
												}
											}
										}
									}
								}

								bool fix = false;
								if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr10) == true)
								{
									if (string.IsNullOrWhiteSpace(propertyStr10) == false)
									{
										newInventProduct.IPValueStr10 = propertyStr10;
										fix = true;
									}
								}
								if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr11) == true)
								{
									if (string.IsNullOrWhiteSpace(propertyStr11) == false)
									{
										newInventProduct.IPValueStr11 = propertyStr11;
										fix = true;
									}
								}
								if (string.IsNullOrWhiteSpace(newInventProduct.IPValueStr1) == true)
								{
									if (string.IsNullOrWhiteSpace(propertyStr1) == false)
									{
										newInventProduct.IPValueStr1 = propertyStr1;
										fix = true;
									}
								}

								if (fix == true)
								{
									k++;
									Log.Add(MessageTypeEnum.TraceParser, "Update InventProduct 3 :: Barcode = " + newInventProduct.Barcode
								+ ", SerialNumber = " + newInventProduct.SerialNumber
								+ ", SupplierCode = " + newInventProduct.SupplierCode
								+ ", propertyStr1 = " + propertyStr1
								+ ", propertyStr10 = " + propertyStr10
								+ ", propertyStr11 = " + propertyStr11);
								}

							}
						}
					}
				}

				yield return newInventProduct;
			}
			Log.Add(MessageTypeEnum.TraceParser, "Update InventProduct [3] :: k = " + k);
		}
	}
}
