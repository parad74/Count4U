using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class InventProductString
	{
	    public string DocumentHeaderCode { get; set; }
		public string DocumentCode { get; set; }
		public string IturCode { get; set; }
		public string Code { get; set; }
		public string Makat { get; set; }
		public string InputTypeCode { get; set; }
        public string Barcode { get; set; }
		public string CreateDate { get; set; }
		public string ModifyDate { get; set; }
   		//public string PartialPackage { get; set; }
		//public string QuantityDifference { get; set; }
		//public string QuantityEdit { get; set; }
		public string QuantityOriginal { get; set; }
        public string SerialNumber { get; set; }
        //public string ShelfCode { get; set; }
    	public string ProductName { get; set; }
		public string WorkerID { get; set; }
		//public string StatusInventProductBit { get; set; }
		//public string SupplierCode { get; set; }
		//public string SupplierName { get; set; }

		public  InventProductString()
		{
			DocumentHeaderCode = "";
			DocumentCode = "";
			IturCode = "";
			Code = "";
			Makat = "";
			InputTypeCode = "";
			Barcode  = "";
			CreateDate  = "";
			ModifyDate = "";
			//PartialPackage  = "";
			//QuantityDifference  = "";
			//QuantityEdit  = "";
			QuantityOriginal = "";
			//SerialNumber = "";
			//ShelfCode = "";
			ProductName = "";
			//StatusInventProductBit = "";
			WorkerID = "";
			//SupplierCode = "";
			//SupplierName = "";
		}
	}
}
