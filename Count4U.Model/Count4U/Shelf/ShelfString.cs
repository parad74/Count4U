using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class ShelfString
	{
		public long ID { get; set; }
		public string ShelfPartCode { get; set; }
		public string IturCode { get; set; }
		public string SupplierCode { get; set; }
		public string ShelfCode { get; set; }
		public string SupplierName { get; set; }
		public string CreateDate { get; set; }
		public string CreateTime { get; set; }
		public string ShelfNum { get; set; }
		public string Width { get; set; }
		public string Height { get; set; }
		public string Area { get; set; }
		public string ShelfPartInItur { get; set; }


		public ShelfString()
		{
			ShelfPartCode = "";
			IturCode = "";
			SupplierCode = "";
			ShelfCode = "";
			SupplierName = "";
			CreateDate = "";
			CreateTime = "";
			ShelfNum = "1";
			Width = "0";
			Height = "0";
			Area = "0";
			ShelfPartInItur = "0";
		}
	}
}
