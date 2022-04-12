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
	public class InventProductMinusByMakatFromDBParser : InventProductParserBase, IInventProductSimpleParser
	{
		private readonly IInventProductRepository _inventProductRepository;
		//private readonly ILog _log;
		//private Dictionary<string, Itur> _iturDictionary;
		//private List<BitAndRecord> _errorBitList;
		//public DateTimeFormatInfo _dtfi;

		public InventProductMinusByMakatFromDBParser(
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

			string toDbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			Dictionary<string, ProductSimple> productBarcodeExcludeDictionary = new Dictionary<string, ProductSimple>();
			IMakatRepository makatRepository = this._serviceLocator.GetInstance<IMakatRepository>();
			productBarcodeExcludeDictionary = makatRepository.GetMakatDictionaryFromInventProduct(fromPathFile);

			InventProducts inventProductFromDB = this._inventProductRepository.GetInventProducts(toDbPath);

			IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
			importInventProductRepository.ClearInventProducts(toDbPath);


			foreach (var newInventProduct in inventProductFromDB)
			{
				if (productBarcodeExcludeDictionary.ContainsKey(newInventProduct.Makat) == false)
				{
					yield return newInventProduct;
				}
			}
		}
	}
}
