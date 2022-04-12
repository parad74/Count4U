using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{   
    /// <summary>
    /// Интерфейс репозитория для доступа к Product объектам
    /// </summary>
    public interface IProductRepository
	{ 
        /// <summary>
        /// Получить все продукты
        /// </summary>
        /// <returns></returns>
		Products GetProducts(string pathDB);
		Products GetProductsMakatOnly(string pathDB);
		Products GetProductsBarcodeOnly(string pathDB);
		List<App_Data.Product> GetApp_DataProducts(string pathDB);

		IEnumerable<ProductSimple> GetProductSimples(string pathDB);
		Dictionary<string, Product> GetProductDictionary(string pathDB);

        /// <summary>
        /// Получить список продуктов по параметрам выборки
        /// </summary>
        /// <param name="selectParams"></param>
        /// <returns></returns>
		Products GetProducts(SelectParams selectParams, string pathDB);

        /// <summary>
        /// Получить продукты определенных способом получения информации
        /// </summary>
        /// <param name="inputTypeName"></param>
        /// <returns></returns>
		Products GetProductsByInputTypeCode(string inputTypeCode, string pathDB);

        /// <summary>
        /// Получить продукты по баркоду
        /// </summary>
        /// <param name="barcodeValue"></param>
        /// <returns></returns>
		Product GetProductByBarcode(string barcode, string pathDB);

   
        /// <summary>
		/// Получить продукт по makat
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
		Product GetProductByMakat(string makat, string pathDB);

        /// <summary>
        /// Получить продукты по Supplier
        /// </summary>
        /// <param name="supplierName"></param>
        /// <returns></returns>
		Products GetProductsBySupplierCode(string supplierCode, string pathDB);

        /// <summary>
        /// Получить продукты связанный с секцией
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
		Products GetProductsBySectionCode(string sectionCode, string pathDB);

        /// <summary>
        /// Получить продукты по типу
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		Products GetProductsByTypeCode(string typeCode, string pathDB);

        /// <summary>
        /// Получить продукты по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		Products GetProductsByName(string name, string pathDB);
		DateTime GetMaxModifyDate(string pathDB);

		//void ClearProductMakats();

		//void AddProductMakat(string makat);

		//void RemoveProductMakat(string makat);

		//bool IsExistProductMakat(string makat);

		//void ProductMakatsDistinct();

		//void FillProductMakatList(string pathDB);

        /// <summary>
        /// Клонировать продукт с публичной информацией
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
		Product Clone(Product product);

		///// <summary>
		///// Удалить
		///// </summary>
		///// <param name="product"></param>
		//void Delete(Product product, string pathDB);

		/// <summary>
		/// Удалить
		/// </summary>
		/// <param name="product"></param>
		void Delete(string makat, string pathDB);

        /// <summary>
        /// Вставить
        /// </summary>
        /// <param name="product"></param>
		void Insert(Product product, string pathDB);
		void Copy(List<App_Data.Product> products, string toPathDB);

           /// <summary>
        /// Удалить продукты
        /// </summary>
        /// <param name="products"></param>
		void DeleteAll(Products products, string pathDB);
		void DeleteAll(string pathDB);
		void DeleteAllIsUpdated(string pathDB, bool isUpdate = true);
		void DeleteAllMakat(string pathDB);
	
        /// <summary>
        /// Вставить продукты
        /// </summary>
        /// <param name="products"></param>
		void Insert(Products products, string pathDB);

        /// <summary>
        /// Изменить продукты
        /// </summary>
        /// <param name="product"></param>
		void Update(Product product, string pathDB);
		Dictionary<string, ProductSimple> GetProductQuantityERPDictionary(string pathDB);
		Dictionary<string, ProductSimple> GetSumProductQuantityERPByMakatDictionary(string pathDB);
   
		/// <summary>
		/// Количество продуктов в катологе
		/// </summary>
		/// <returns></returns>
		long CountProduct(string pathDB);
		long CountMakat(string pathDB);
		long CountBarcode(string pathDB);
		Dictionary<string, ProductSimple> GetProductSimpleDictionary(string pathDB, bool refill = false, string typeMakat = "M");
		void FillProductSimpleDictionary(string pathDB, string typeMakat, out Dictionary<string, ProductSimple> productSimpleDictionary);
		Dictionary<string, Product> GetProduct_IturCodeErpDictionary(string pathDB);
		Dictionary<string, ProductTagSimple> GetProductTagDictionary(string pathDB);
		bool IsAnyProductInDb(string pathDB);
		List<string> GetSectionCodeList(string pathDB);
		List<string> GetSupplierCodeList(string pathDB);
		List<string> GetFamilyCodeList(string pathDB);


		void SetLastUpdatedCatalog(string pathDB);
		//Dictionary<string, ProductSimple> GetProductSimpleUpdateOnlyDictionary(string pathDB,  bool refill = false);
	}
}
