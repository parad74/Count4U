using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public class ProductSimpleString
	{
		//public string ID { get; set; }
		//public string Code { get; set; }
		//public string Barcode { get; set; }
		public string Makat { get; set; }
		//public string MakatERP { get; set; }
		//public string TypeMakatCode { get; set; }
		public string Name { get; set; }
		//public string Description { get; set; }

		//public string Tag { get; set; }
		//public string CountMax { get; set; }
		//public string CountMin { get; set; }
		public string Family { get; set; }
		public string FamilyCode { get; set; }
		//public string Importance { get; set; }

		public string PriceBuy { get; set; }
		public string PriceExtra { get; set; }
		public string PriceSale { get; set; }
		public string PriceString { get; set; }

		//public string Section { get; set; }
		//public string Supplier { get; set; }
		//public string SectionCode { get; set; }
		public string SupplierCode { get; set; }
		public string TypeCode { get; set; }
		//public string UnitTypeCode { get; set; }
		//public string InputTypeCode { get; set; }
		//public string CreateDate { get; set; }
		//public string ModifyDate { get; set; }
		//public string ParentCode { get; set; }
		//public string ParentBarcode { get; set; }
		public string ParentMakat { get; set; }
		public string CountInParentPack { get; set; }
		//public string ParserBag { get; set; }
		public string MakatOriginal { get; set; }
		public string SectionCode { get; set; }
		public string UnitTypeCode { get; set; }
		public string BalanceQuantityERP { get; set; }
		public string BalanceQuantityPartialERP { get; set; }

		public ProductSimpleString()
		{
			//Code = "";
			//Barcode = "";
			Makat = "";
			ParentMakat = "";
			//MakatERP = "";
			//TypeMakatCode  = "";
			Name = "";
			//Description  = "";

			//BalanceQuantityERP = "";
			//BalanceQuantityPartialERP  = "";
			//Tag = "";
			//CountMax  = "";
			//CountMin = "";
			Family  = "";
			FamilyCode = "";
			//Importance  = "";

			PriceBuy = "0";
			PriceExtra = "0";
			PriceSale = "0";
			PriceString = "0";

			//Section = "";
			//Supplier  = "";
			//SectionCode = "";
			SupplierCode = "";
			TypeCode = TypeMakatEnum.M.ToString();
			//UnitTypeCode  = "";
			//InputTypeCode  = "";
			//CreateDate = "";
			//ModifyDate  = "";
			//ParentCode  = "";
			//ParentBarcode = "";
			//ParentMakat  = "";
			CountInParentPack = "1";
			//ParserBag  = "";
			MakatOriginal = "";
			SectionCode = "";
			UnitTypeCode = "";
			BalanceQuantityERP = "0";
			BalanceQuantityPartialERP = "0";
			
		}

		
	}
}
