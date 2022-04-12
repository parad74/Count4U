using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class InventProductSimple
	{
		//public long ID { get; set; }
		public string Makat { get; set; }
		public string Barcode { get; set; }
        //public string DocumentHeaderCode { get; set; }
		public string DocumentCode { get; set; }
		public string IturCode{ get; set; }
		//public string Code { get; set; }
		public double QuantityOriginal { get; set; }
		public int QuantityInPackEdit { get; set; }
		//public double ValueBuyQriginal { get; set; }
		public string InputTypeCode { get; set; }
		public string ImputTypeCodeFromPDA { get; set; }
      
        public DateTime CreateDate { get; set; }
		
		//public DateTime? ModifyDate { get; set; }
		//public string DocumentHeader { get; set; }
		//public bool? PartialPackage { get; set; }
		//public string QuantityDifference { get; set; }
		//public double? QuantityEdit { get; set; }

		//public string SerialNumber { get; set; }
		//public string ShelfCode { get; set; }
		////public string StatusInventProduct { get; set; }
		public string ProductName { get; set; }
		public string StatusInventProductCode { get; set; }
		public int StatusInventProductBit { get; set; }

		//public long Num { get; set; }
		public int FromCatalogType { get; set; }
		public int SectionNum { get; set; }
		public string TypeMakat { get; set; }
		public long IPNum { get; set; }
		public int DocNum { get; set; }
	}
}
