using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public enum TypeMakatEnum
	{
		B = 0,
		M = 1,
		W = 2
	}

	public enum TypeSectionEnum
	{
		S = 0,
		SS = 1,
		W = 2
	}

	public enum InputTypeCodeEnum
	{
		B = 0,
		K = 1
	}

	public enum FromCatalogTypeEnum
	{
		Unknown = 0,
		CatalogMakatOrBarcod = 1,
		CatalogBarcodeWithoutMakatParent = 2,
		InventProductWithoutMakat = 3,
		ProductMakatWithoutInventProduct = 4
	}

}
