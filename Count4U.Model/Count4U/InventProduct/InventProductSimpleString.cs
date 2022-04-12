using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class InventProductSimpleString
	{
		public string Makat { get; set; }
		public string Barcode { get; set; }
		//public string DocumentHeaderCode { get; set; }
		public string DocumentCode { get; set; }
		public string IturCode { get; set; }
		//public string Code { get; set; }
		public string QuantityOriginal { get; set; }
		public string QuantityInPackEdit { get; set; }
		public string InputTypeCode { get; set; }
		public string ImputTypeCodeFromPDA { get; set; }
		public string CreateDate { get; set; }
		public string CreateTime { get; set; }

		//public string ModifyDate { get; set; }
		//public string DocumentHeader { get; set; }
		//public string PartialPackage { get; set; }
		//public string QuantityDifference { get; set; }
		//public string QuantityEdit { get; set; }
		public string SerialNumber { get; set; }
		//public string ShelfCode { get; set; }
		//public string StatusInventProduct { get; set; }
		public string ProductName { get; set; }
		//public string StatusInventProductCode { get; set; }   //При смене БД заменить на TypeCode - B, M 
		public string TypeMakat { get; set; }
		public string SessionCode { get; set; }
		public string WorkerID { get; set; }

		//public string StatusInventProductBit { get; set; }

		public InventProductSimpleString()
		{
			Makat = "";
			Barcode = "";
			DocumentCode = "";
			IturCode = "";
			//DocumentHeaderCode = "";
			//Code  = "";
			QuantityOriginal = "0.0";
			QuantityInPackEdit = "0";
			InputTypeCode = "";
			ImputTypeCodeFromPDA = "";
			CreateDate = "";
			CreateTime = "";
			//ModifyDate  = "";
			//DocumentHeader  = "";
			//PartialPackage  = "";
			//QuantityDifference   = "";
			//QuantityEdit   = "";
			SerialNumber = "";
			//ShelfCode   = "";
			//StatusInventProduct  = "";
			ProductName = "";
			TypeMakat = "";
			//StatusInventProductCode   = "";
			//StatusInventProductBit  = "";
			WorkerID = "";
		}

		
	}
}
