using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{
	public interface IMakatRepository
	{
		//List<string> GetProductMakatList(string pathDB, bool refill = false);
		//void ClearProductMakatList();

		//void ClearProductMakatDictionary();

		//Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB, bool refill = false);

		//void AddProductMakat(string makat, ProductMakat productMakat);

		//void RemoveProductMakat(string makat);

		//bool IsExistProductMakat(string makat);

		//ProductMakat GetProductByMakat(string makat);

		//string GetProductNameByMakat(string makat);

		//void FillProductMakatDictionary(string pathDB);

		Dictionary<string, ProductMakat> GetProductBarcodeDictionary(string pathDB, bool refill = false);
		void ClearProductBarcodeDictionary();
		void AddProductBarcode(string makat, ProductMakat productMakat);
		//void RemoveProductBarcode(string makat);
		bool IsExistProductBarcode(string makat, bool top = false);
		ProductMakat GetProductByBarcode(string makat, bool top = false);
		string GetProductNameByBarcode(string makat);
		void FillProductBarcodeDictionary(string pathDB);
		Dictionary<string, ProductSimple> FillProductBarcodeExcludeItursDictionary(List<string> iturCodes, string pathDB);
		List<ProductMakat> GetBarcodeProducts(string parentMakat, List<ProductMakat> products);
		List<ProductMakat> GetProductMakatList(string pathDB, bool refill = false);
		Dictionary<string, ProductSimple> GetMakatDictionaryFromInventProduct(string pathDB);
		Dictionary<string, string> GetProductMakatBarcodesDictionary(string pathDB, bool refill = false);
		bool ProductMakatDictionaryFill {get; set;}
		Dictionary<string, ProductMakat> ProductMakatDictionaryRefill(string pathDB, bool refill = false);
		Dictionary<string, ProductMakat> GetProductUnitTypeDictionary(string pathDB, bool refill = false);
	}

}
