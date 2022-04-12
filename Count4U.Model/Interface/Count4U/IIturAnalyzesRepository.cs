using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using System.Collections;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к Itur объектам
    /// </summary>
	public interface IIturAnalyzesRepository
	{
        /// <summary>
        /// Получить весь список итуров
        /// </summary>
        /// <returns></returns>
		IturAnalyzesCollection GetIturAnalyzesCollection(string pathDB, bool refill = true, bool refillCatalogDictionary = false, Dictionary<object, object> parms = null);
		IturAnalyzesCollection GetIturAnalyzesCollection(SelectParams selectParams, string pathDB, bool refill = true, bool refillCatalogDictionary = false, 
			Dictionary<object, object> parms = null);
		IturAnalyzesCollection GetIturAnalyzesCollection(LevelInAnalyzesEnum viewContext, SelectParams selectParams,
			string pathDB, bool refill = true, bool refillCatalogDictionary = false, IturAnalyzeTypeEnum simple = IturAnalyzeTypeEnum.Full,
			Dictionary<object, object> parms = null);
		IturAnalyzesCollection GetIturAnalyzesSumCollection(SelectParams selectParams,
		string pathDB, bool refill = true, bool refillCatalogDictionary = false, Dictionary<object, object> parms = null);
		void FillAllDictionary(string pathDB, bool refill = true);
		void ClearDictionarys();
		void ClearProductDictionary();
		IEnumerable<IturAnalyzesSimple> GetIturAnalyzesSumEnumerable(
			SelectParams selectParams,
			string pathDB, bool refill = true,
			bool refillCatalogDictionary = false,
			Dictionary<object, object> parms = null,
			bool addResult = true,
			List<ImportDomainEnum> importType = null
			);

		Dictionary<string, Itur> GetIturDictionary(string pathDB, bool refill = true);
		Dictionary<string, DocumentHeader> GetDocumentHeaderDictionary(string pathDB, bool refill = true);
		Dictionary<string, Location> GetLocationDictionary(string pathDB, bool refill = true);
		InventProducts GetInventProductList(SelectParams selectParms, string pathDB);
		Dictionary<int, IturStatusEnum> GetStatusIturDictionary();
		Dictionary<int, IturStatusGroupEnum> GetStatusIturGroupDictionary();
		Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB, bool refill = true);
		Dictionary<string, ProductSimple> GetProductSimpleDictionary(string pathDB, bool refill = true, string typeMakat = "M");
		IturAnalyzesCollection GetIACollection(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsOriginal(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> param = null);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakats(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditDifferenceByMakats(SelectParams selectParams, string pathDB);
		IEnumerable<IturAnalyzesSimple> GetPSumQuantityOriginalERPByMakats(SelectParams selectParams, string pathDB);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakats(SelectParams selectParams, string pathDB);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsOriginal(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsLikeNumber(SelectParams selectParams, string pathDB);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsWithoutAddQuantityInPackEdit(SelectParams selectParams, string pathDB);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsOriginalWithoutAddQuantityInPackEdit(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsAndExpiredDate(SelectParams selectParams, string pathDB);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsAndIturCode(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsAndERPIturCode(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByBakcodesAndIturCode(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatAndBakcodes(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsOriginalLikeNumber(SelectParams selectParams, string pathDB, bool refill = true);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsBarcode(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsOriginalBarcode(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null);
		IEnumerable<IturAnalyzesSimple> GetIturSumQuantityEditByIturCode(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null);
		void SetResulteValue(AnalezeValueTypeEnum resulteCode, string resulteValue, string resulteDescription, string pathDB);
		IturAnalyzes SetIturAnalyzesResulteValue(AnalezeValueTypeEnum resulteCode, string resulteValue, string resulteDescription, string pathDB, string inventorCode="none", string inventorName = "none");
		string GetResulteValue(AnalezeValueTypeEnum resulteCode, string pathDB);
		IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsPlusCode(SelectParams selectParams, string pathDB, bool refill = true);
		IturAnalyzesCollection GetIturAnalyzesTotal(string pathDB, SelectParams selectParams = null);
		//Dictionary<string, ProductSimple> GetProductSimpleUpdateOnlyDictionary(string pathDB, bool refill = true);
		IturAnalyzesCollection GetIPSumQuantityEditByIturCode(SelectParams selectParams, string pathDB, bool refillIturStatistic = true);
		void DeleteAll(string pathDB);
		IturAnalyzesCollection GetIturAnalyzesCollection(string pathDB);
		
	}
}
