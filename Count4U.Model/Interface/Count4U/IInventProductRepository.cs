using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Codeplex.Reactive;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к InventProduct объектам 
    /// ! изменения могут быть описки - проверять
    /// </summary>
	public interface IInventProductRepository
	{
		/// <summary>
		/// Получить список всех InventProduct
		/// </summary>
		/// <returns></returns>
		InventProducts GetInventProducts(string pathDB);

		/// <summary>
		/// Получить список InventProduct по параметрам выборки
		/// </summary>
		/// <param name="selectParams"></param>
		/// <returns></returns>
		InventProducts GetInventProducts(SelectParams selectParams, string pathDB);
		InventProducts GetInventProductsExtended(SelectParams selectParams, string code,  string pathDB);
		InventProducts GetInventProductNotExistInCatalog(string pathDB);
		
		InventProducts GetInventProductTotal(string pathDB, SelectParams selectParams = null);
		Dictionary<Tuple<string, string, string>, InventProduct> GetQuantitySerialInventProducts(string pathDB);
		Dictionary<string, InventProduct> GetDictionaryInventProductsUID(string pathDB);
		/// <summary>
		/// Получить InventProduct по Code
		/// </summary>
		/// <param name="inventProductCode"></param>
		/// <returns></returns>
		InventProduct GetInventProductByCode(string inventProductCode, string pathDB);
		int GetMaxNumForDocumentCode(string documentCode, string pathDB);

		InventProduct GetInventProductByID(long ID, string pathDB);

		InventProduct GetInventProductByBarcode(string barcode, string pathDB);

		InventProduct GetInventProductByMakat(string makat, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyByMakatsAndIturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyBarcodeAndIturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyCodeAndIturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyEditIturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyByMakatsAndDocAndIturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyByMakatAndSNAndIturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyByBarcodeAndSNAndIturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyByMakatAndSNAndProp10IturCode(SelectParams selectParams, string pathDB);
		Dictionary<string, InventProduct> GetIPQuntetyByBarcodeAndSNAndProp10IturCode(SelectParams selectParams, string pathDB);

		IQueryable<InventProduct> GetInventProductMakatAndSumQuantityEdit(string pathDB);
		double GetSumQuantityEditByMakat(string makat, string pathDB);
		double GetSumQuantityEditByDocumentCode(string documentCode, string pathDB);
		double GetSumQuantityOriginalByDocumentCode(string documentCode, string pathDB);
		double GetSumQuantityDifferenceByDocumentCode(string documentCode, string pathDB);
		double GetSumQuantityEditByIturCode(string iturCode, string pathDB);
		double GetSumQuantityDifferenceByIturCode(string iturCode, string pathDB);
		double GetSumQuantityOriginalByIturCode(string iturCode, string pathDB);
		IEnumerable<Itur> GetIturSumQuantityEdit(SelectParams selectParams,	string pathDB); //?? перепроверить, что гарантированное есть IturCode
		List<string> GetIturCodeList(SelectParams selectParams, string pathDB);

		ReactiveProperty<string> ReturnStringSumQuantityEditByDocumentCode(string documentCode, string pathDB);
		ReactiveProperty<string> ReturnStringSumQuantityEditByIturCode(string iturCode, string pathDB);

		int GetCountItemByDocumentCode(string documentCode, string pathDB);
		string GetCountMakatTotal(AnalezeValueTypeEnum resulteCode, string pathDB);
		List<string> GetDocumentCodeList(string pathDB);		
		long CountInventProduct(string pathDB);

		InventProducts GetInventProductsByStatusCode(string statusCode, string pathDB);
		InventProducts GetInventProductsByDocumentHeader(DocumentHeader documentHeader, string pathDB);
		InventProducts GetInventProductsByDocumentCode(string documentCode, string pathDB);
		InventProducts GetInventProductsByInputTypeCode(string inputTypeName, string pathDB);
		Dictionary<string, ProductMakat> GetIPCountByMakatsAndIturCode(SelectParams selectParams, string pathDB);
		//InventProduct Clone(InventProduct inventProduct);
		void Delete(InventProduct inventProduct, string pathDB);
		void DeleteAllByDocumentHeaderCode(string documentCode, string pathDB);
		List<string> DeleteAllNotExistInCatalog(string pathDB);
		void Insert(InventProduct inventProduct, DocumentHeader documentHeader, string pathDB);
		void Insert(InventProduct inventProduct, string pathDB);
		void Insert(InventProducts inventProducts, string pathDB);
		void InsertClone(InventProducts inventProducts, string pathDB);
		
		void Update(InventProduct inventProduct, string pathDB);
		void InsertOrUpdate(InventProducts newInventProducts, string toDocumentHeaderCode, string pathDB);

		void ClearStatusBit(string pathDB);
		void ClearStatusBit(string documentCode, string pathDB);
		void SetStatusBitByID(long ID, int bit, string pathDB);
		int GetStatusBitByID(long ID, string pathDB);
		int[] GetStatusBitArrayIntByDocumentCode(string documentCode, string pathDB);
		List<BitArray> GetStatusBitArrayListByDocumentCode(string documentCode, string pathDB);
		BitArray GetResultStatusBitOrByDocumentCode(string documentCode, string pathDB);

		//void FillInventProductDictionary(string pathDB);
		//void ClearInventProductDictionary();

        bool IsAnyInventProductInDb(string pathDB);
		bool IsAnyInventProductInIturCode(string iturCode, string pathDB);
	}
}
