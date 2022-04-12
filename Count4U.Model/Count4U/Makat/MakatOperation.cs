using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
	public static class MakatOperation
	{
		public static string GetParentMakatFromMakatDictionary(
			this Dictionary<string, ProductMakat> productMakatDictionary, string barcode, ILog log)
		{
			if (string.IsNullOrWhiteSpace(barcode) == true) return "";
			if (productMakatDictionary.ContainsKey(barcode) == false)
			{
				log.Add(MessageTypeEnum.WarningParser, ValidateErrorMessage.Warning + String.Format(ValidateErrorMessage.MakatAndBarcodeNotExistInDB, barcode));
				return "";
			}

			string makat = barcode;
			while (string.IsNullOrWhiteSpace(productMakatDictionary[makat].ParentMakat) == false)
			{
				makat = productMakatDictionary[makat].ParentMakat;
				if (productMakatDictionary.ContainsKey(makat) == false)
				{
					return "";
				}
			}
			return makat;
		}

		public static string GetProductNameFromMakatDictionary(
			this Dictionary<string, ProductMakat> productMakatDictionary, string barcode)
		{
			if (productMakatDictionary.ContainsKey(barcode) == false)
			{
				return "";
			}

			string makat = barcode;
			while (string.IsNullOrWhiteSpace(productMakatDictionary[makat].Name) == false)
			{
				makat = productMakatDictionary[makat].ParentMakat;
				if (productMakatDictionary.ContainsKey(makat) == false)
				{
					return "";
				}
			}
			return productMakatDictionary[makat].Name;
		}



	}
}
