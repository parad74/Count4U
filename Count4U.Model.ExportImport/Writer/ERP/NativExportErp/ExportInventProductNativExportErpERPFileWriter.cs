using System.IO;
using System.Collections.Generic;
using Count4U.Model.Interface;
using Count4U.Model.Common;
using System.Reflection;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductNativExportErpERPFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "",
			string separator = ",", object argument = null)
		{
		}
		public void AddRowInventProduct(StreamWriter sw, InventProduct inventProduct, long countRow,
			   string ERPNum = "", string INVDate = "", string separator = ",", object argument = null)
		{
	
			Dictionary<string, PropertyStr> dictionary = argument as Dictionary<string, PropertyStr>;
			string[] newRows = new string[100];
			if (countRow == 1)
			{
				string[] newRows1 = new string[100];
				foreach (KeyValuePair<string, PropertyStr> decorator in dictionary)
				{
					if (string.IsNullOrWhiteSpace(decorator.Key) == true) continue;
					//DomainObject //PropertyExportErpDecorator
					//include 						//Code								 0
					//index				 	  //TypeCode						1
					// PropertyName		 //PropertyStrCode 			2
					//Title 						// Name						3   //Option
					PropertyInfo objectProp = inventProduct.GetType().GetProperty(decorator.Key);
					if (objectProp != null)
					{
						if (string.IsNullOrWhiteSpace(decorator.Value.TypeCode) == true) continue;
						int index = -1;
						bool ret = int.TryParse(decorator.Value.TypeCode, out index);
						if (ret == false) continue;
						if (index < 1 || index > 99) continue;
						if (string.IsNullOrWhiteSpace(decorator.Value.Name) == false)
						{
							newRows1[index - 1] = decorator.Value.Name;
						}
						else
						{
							newRows1[index - 1] = decorator.Key;
						}
					}
				}
				string newRow1 = string.Join(separator, newRows1);
				sw.WriteLine(newRow1);
			}
		
		
			foreach (KeyValuePair<string, PropertyStr> decorator in dictionary)
			{
				if (string.IsNullOrWhiteSpace(decorator.Key) == true) continue;
				//DomainObject //PropertyExportErpDecorator
				//include 						//Code								 0
				//index				 	  //TypeCode						1
				// PropertyName		 //PropertyStrCode 			2
				//Title 						// Name						3   //Option
				PropertyInfo objectProp = inventProduct.GetType().GetProperty(decorator.Key);
				if (objectProp != null)
				{
					var parceData = objectProp.GetValue(inventProduct, null);
					if (string.IsNullOrWhiteSpace(decorator.Value.TypeCode) == true) continue;
					int index = -1;
					bool ret = int.TryParse(decorator.Value.TypeCode, out index);
					if (ret == false) continue;
					if (index < 1 || index > 99) continue;
					newRows[index-1] = parceData.ToString() ;
				}
			}

		
			//string uid = propertyStr.TypeCode + "|" + propertyStr.PropertyStrCode + "|" + propertyStr.Code;
			//DocumentSheet 	  //TypeCode						0
			// Row 					 //PropertyStrCode 			1
			//Col 						//Code								    2
			//Name 					//значение 							3

			//for (int i = 0; i < newRows.Count(); i++)
			//{
			//	//DocumentSheet, Row , 	  Col
			//	var futureKey = new Pair<string, string, string>("1", "1", (i + 1).ToString());
			//	if (dictionary.ContainsKey(futureKey) == true)
			//	{
			//		string title = dictionary[futureKey].Name;
			//		newRows[i] = title;
			//	}
			//}
			//newRows = new string[] { iturAnalyzes.Makat, iturAnalyzes.InputTypeCode,
			//		iturAnalyzes.QuantityEdit.ToString() , iturAnalyzes.IturCode };

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{

		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddFooter(StreamWriter sw, long countRow, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddFooterSum(StreamWriter sw, long countRow, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		
	}
}
//InventProduct inventProduct = new InventProduct();
//inventProduct.DocumentHeaderCode = iturAnalyzes.DocumentHeaderCode;
//inventProduct.DocumentCode = iturAnalyzes.DocumentCode;
//inventProduct.IturCode = iturAnalyzes.IturCode;
//inventProduct.Code = iturAnalyzes.Code;
//inventProduct.Makat = iturAnalyzes.Makat;
//inventProduct.InputTypeCode = iturAnalyzes.InputTypeCode;
//inventProduct.Barcode = iturAnalyzes.Barcode;
//inventProduct.CreateDate = iturAnalyzes.CreateDate;
//inventProduct.ModifyDate = iturAnalyzes.ModifyDate;
//inventProduct.PartialPackage = iturAnalyzes.PartialPackage;
//inventProduct.QuantityDifference = iturAnalyzes.QuantityDifference;
//inventProduct.QuantityEdit = iturAnalyzes.QuantityEdit;
//inventProduct.QuantityInPackEdit = iturAnalyzes.QuantityInPackEdit;
//inventProduct.QuantityOriginal = iturAnalyzes.QuantityOriginal;
//inventProduct.ShelfCode = iturAnalyzes.ShelfCode;
//inventProduct.ProductName = iturAnalyzes.ProductName;
//inventProduct.StatusInventProductCode = iturAnalyzes.StatusInventProductCode;
//inventProduct.StatusInventProductBit = iturAnalyzes.StatusInventProductBit;
//inventProduct.FromCatalogType = iturAnalyzes.FromCatalogType;
//inventProduct.SectionNum = iturAnalyzes.SectionNum;
//inventProduct.TypeMakat = iturAnalyzes.TypeMakat;
//inventProduct.IPNum = iturAnalyzes.IPNum;
//inventProduct.ImputTypeCodeFromPDA = iturAnalyzes.ImputTypeCodeFromPDA;
//inventProduct.DocNum = iturAnalyzes.DocNum;
//inventProduct.SessionNum = iturAnalyzes.SessionNum;
//inventProduct.SessionCode = iturAnalyzes.SessionCode;
//inventProduct.SectionCode = iturAnalyzes.SectionCode;
//inventProduct.SectionName = iturAnalyzes.SectionName;
//inventProduct.PriceBuy = iturAnalyzes.PriceBuy;
//inventProduct.PriceSale = iturAnalyzes.PriceSale;
//inventProduct.WorkerID = iturAnalyzes.WorkerID;
//inventProduct.SupplierName = iturAnalyzes.SupplierName;
//inventProduct.SupplierCode = iturAnalyzes.SupplierCode;
//inventProduct.SerialNumber = iturAnalyzes.SerialNumber;
//inventProduct.ItemStatus = iturAnalyzes.ItemStatus;
//inventProduct.IPValueStr1 = iturAnalyzes.IPValueStr1;
//inventProduct.IPValueStr2 = iturAnalyzes.IPValueStr2;
//inventProduct.IPValueStr3 = iturAnalyzes.IPValueStr3;
//inventProduct.IPValueStr4 = iturAnalyzes.IPValueStr4;
//inventProduct.IPValueStr5 = iturAnalyzes.IPValueStr5;
//inventProduct.IPValueStr6 = iturAnalyzes.IPValueStr6;
//inventProduct.IPValueStr7 = iturAnalyzes.IPValueStr7;
//inventProduct.IPValueStr8 = iturAnalyzes.IPValueStr8;
//inventProduct.IPValueStr9 = iturAnalyzes.IPValueStr9;
//inventProduct.IPValueStr10 = iturAnalyzes.IPValueStr10;
//inventProduct.IPValueStr11 = iturAnalyzes.IPValueStr11;
//inventProduct.IPValueStr12 = iturAnalyzes.IPValueStr12;
//inventProduct.IPValueStr13 = iturAnalyzes.IPValueStr13;
//inventProduct.IPValueStr14 = iturAnalyzes.IPValueStr14;
//inventProduct.IPValueStr15 = iturAnalyzes.IPValueStr15;
//inventProduct.IPValueStr16 = iturAnalyzes.IPValueStr16;
//inventProduct.IPValueStr17 = iturAnalyzes.IPValueStr17;
//inventProduct.IPValueStr18 = iturAnalyzes.IPValueStr18;
//inventProduct.IPValueStr19 = iturAnalyzes.IPValueStr19;
//inventProduct.IPValueStr20 = iturAnalyzes.IPValueStr20;
//inventProduct.IPValueFloat1 = iturAnalyzes.IPValueFloat1;
//inventProduct.IPValueFloat2 = iturAnalyzes.IPValueFloat2;
//inventProduct.IPValueFloat3 = iturAnalyzes.IPValueFloat3;
//inventProduct.IPValueFloat4 = iturAnalyzes.IPValueFloat4;
//inventProduct.IPValueFloat5 = iturAnalyzes.IPValueFloat5;
//inventProduct.IPValueInt1 = iturAnalyzes.IPValueInt1;
//inventProduct.IPValueInt2 = iturAnalyzes.IPValueInt2;
//inventProduct.IPValueInt3 = iturAnalyzes.IPValueInt3;
//inventProduct.IPValueInt4 = iturAnalyzes.IPValueInt4;
//inventProduct.IPValueInt5 = iturAnalyzes.IPValueInt5;
//inventProduct.IPValueBit1 = iturAnalyzes.IPValueBit1;
//inventProduct.IPValueBit2 = iturAnalyzes.IPValueBit2;
//inventProduct.IPValueBit3 = iturAnalyzes.IPValueBit3;
//inventProduct.IPValueBit4 = iturAnalyzes.IPValueBit4;
//inventProduct.IPValueBit5 = iturAnalyzes.IPValueBit5;
//inventProduct.ERPIturCode = iturAnalyzes.ERPIturCode;
//inventProduct.UnityCode = iturAnalyzes.UnityCode;
//inventProduct.Code = iturAnalyzes.LocationCode;
//inventProduct.Tag = iturAnalyzes.Tag;
//inventProduct.Tag1 = iturAnalyzes.Tag1;
//inventProduct.Tag2 = iturAnalyzes.Tag2;
//inventProduct.Tag3 = iturAnalyzes.Tag3;
//inventProduct.QuantityWithoutPackEdit = iturAnalyzes.QuantityWithoutPackEdit;
//inventProduct.ValueBuyDifference = iturAnalyzes.ValueBuyDifference;
//inventProduct.ValueBuyEdit = iturAnalyzes.ValueBuyEdit;
//inventProduct.ValueBuyQriginal = iturAnalyzes.ValueBuyQriginal;
//inventProduct.ValueBuyWithoutPackEdit = iturAnalyzes.ValueBuyWithoutPackEdit;
//inventProduct.ValueBuyInPackEdit = iturAnalyzes.ValueBuyInPackEdit;
//inventProduct.ValueSaleDifference = iturAnalyzes.ValueSaleDifference;
//inventProduct.ValueSaleEdit = iturAnalyzes.ValueSaleEdit;
//inventProduct.ValueSaleQriginal = iturAnalyzes.ValueSaleQriginal;
//inventProduct.ValueSaleWithoutPackEdit = iturAnalyzes.ValueSaleWithoutPackEdit;
//inventProduct.ValueSaleInPackEdit = iturAnalyzes.ValueSaleInPackEdit;