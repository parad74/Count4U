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
	public class InventProductUpdate2MakatAndSNFromDBParser : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;
	
		public InventProductUpdate2MakatAndSNFromDBParser(
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
		
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			InventProducts inventProducts =  inventProductRepository.GetInventProducts(dbPath);

			//очистить InventProduct
			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
			importInventProductRepository.ClearInventProducts(dbPath);

			foreach (InventProduct inventProduct in inventProducts)
			{
				InventProduct newInventProduct = inventProduct;
				string makat = newInventProduct.Makat;
				string serialNumber = newInventProduct.SerialNumber;
				string newMakat = makat;
				//if (string.IsNullOrWhiteSpace(serialNumber) == false)
				//{
					newMakat = makat + "|" + serialNumber;
				//}
				//newInventProduct.Barcode = newMakat.CutLength(49); 
				newInventProduct.Code = newMakat.CutLength(299); 
				yield return newInventProduct;
			}


		}
	}
}
