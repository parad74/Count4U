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
	public class InventProductUpdateBarcodeFromDBParser : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;
		//private readonly ILog _log;
		//private Dictionary<string, Itur> _iturDictionary;
		//private List<BitAndRecord> _errorBitList;
		//public DateTimeFormatInfo _dtfi;

		public InventProductUpdateBarcodeFromDBParser(
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

			InventProducts inventProductFromDB = this._inventProductRepository.GetInventProducts(fromPathFile);

			IImportInventProductBlukRepository importInventProductRepository = _serviceLocator.GetInstance<IImportInventProductBlukRepository>();
			importInventProductRepository.ClearInventProducts(fromPathFile);

			 IIturRepository   iturRepository  = _serviceLocator.GetInstance<IIturRepository>();
			 Iturs iturs = iturRepository.GetIturs(fromPathFile);
			 Dictionary<string, Itur>  iturDictionary = iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);

			foreach (var newInventProduct in inventProductFromDB)
			{
				//Nativ + 4 составной ключ 
				string serialNumberSupplier = newInventProduct.SupplierCode;
				Itur itur = iturDictionary[newInventProduct.IturCode];
	
				if (string.IsNullOrWhiteSpace(serialNumberSupplier) == false)
				{
					string serialNumber = newInventProduct.SerialNumber;
					string makat = newInventProduct.Makat;
					string locationCodeFrom = itur.ERPIturCode;
					string[] ids = new string[] { serialNumber, makat, locationCodeFrom, serialNumberSupplier };	// 4 - составной ключ для SN
					//if (unitTypeCode == QuantityKey)
					//{
					//	ids = new string[] { serialNumber, makat, locationCodeFrom, propertyStrKey8 };	// 4 - составной ключ	  для Q
					//}
					string ID = ids.JoinRecord("|");
					string barcode = ID.CutLength(299);
					newInventProduct.Barcode = barcode;
					newInventProduct.Code = barcode;	//!!!
				}
				yield return newInventProduct;
			}
		}
	}
}
