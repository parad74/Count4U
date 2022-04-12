using System;

namespace Count4U.GenerationReport
{
	public class ContextReport
	{
		public string Code { get; set; }
		public string IturCode { get; set; }
		public string IturNameFull { get; set; }
		public string ERPIturCode { get; set; }
		public string IturPrefix { get; set; }
		public string AppNameReport { get; set; }
		public byte[] IturBarcode { get; set; }
		public byte[] LocationBarcode { get; set; }
		public string LocationCode { get; set; }
		public string DocumentHeaderCode { get; set; }
		public string DocumentCode { get; set; }
		public string SectionCode { get; set; }

		public string IturName { get; set; }
		public string LocationName { get; set; }
		public string DocumentHeaderName { get; set; }
		public string SectionName { get; set; }
		public string SectionNum { get; set; }

		public string CustomerAddress { get; set; }
		public string CustomerCode { get; set; }
		public string CustomerContactPerson { get; set; }
		public string CustomerDescription { get; set; }
		public string CustomerFax { get; set; }
		public string CustomerMail { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string CustomerDBPath { get; set; }
		public string CustomerLogoPath { get; set; }
		public string CustomerTag { get; set; }
		public string CustomerTag1 { get; set; }
		public string CustomerTag2 { get; set; }
		public string CustomerTag3 { get; set; }

		public string BranchAddress { get; set; }
		public string BranchCode { get; set; }
		public string BranchContactPerson { get; set; }
		public string BranchDescription { get; set; }
		public string BranchFax { get; set; }
		public string BranchLogoFile { get; set; }
		public string BranchMail { get; set; }
		public string BranchName { get; set; }
		public string BranchPhone { get; set; }
		public string BranchDBPath { get; set; }
		public string BranchCodeLocal { get; set; }
		public string BranchCodeERP { get; set; }
		public string BranchTag { get; set; }
		public string BranchTag1 { get; set; }
		public string BranchTag2 { get; set; }
		public string BranchTag3 { get; set; }

		public string InventorCode { get; set; }
		public string InventorName { get; set; }
		public string InventorDescription { get; set; }
		public string InventorDate { get; set; }
		public string InventorDDMM { get; set; }
		public string InventorYYYY { get; set; }
		public string InventorStatus { get; set; }
		public string InventorStatusCode { get; set; }
		public string InventorDBPath { get; set; }
		public string InventorTag { get; set; }
		public string InventorTag1 { get; set; }
		public string InventorTag2 { get; set; }
		public string InventorTag3 { get; set; }


		public string IturNumber { get; set; }
		public string IturStatusIturBit { get; set; }
		public string IturStatusIturGroupBit { get; set; }
		public string IturNumberPrefix { get; set; }
		public string IturNumberSufix { get; set; }
		public string IturStatusDocHeaderBit { get; set; }

		public string WorkerGUID { get; set; }
		public string DocStatusDocHeaderBit { get; set; }
		public string DocStatusInventProductBit { get; set; }
		public string DocStatusApproveBit { get; set; }
		
		public string Makat { get; set; }
		public string InputTypeCode { get; set; }
		public string Barcode { get; set; }
		public string ModifyDate { get; set; }
		public string CreateDate { get; set; }
		public string FromDate { get; set; }
		public string ToDate { get; set; }

		public string SerialNumber { get; set; }
		public string ShelfCode { get; set; }
		public string ProductName { get; set; }
		public string PDAStatusInventProductBit { get; set; }
		public string BarcodeOriginal { get; set; }
		public string MakatOriginal { get; set; }
		public string PriceString { get; set; }
		public string PriceBuy { get; set; }
		public string PriceSale { get; set; }
		public string PriceExtra { get; set; }
		public string PDANum { get; set; }
		public string DocNum { get; set; }
		public string CurrentCurrency { get; set; }
		public string TradeMark { get; set; }

		public string QuantityDifferenceOriginalERP { get; set; }
		public string ValueDifferenceOriginalERP { get; set; }
		public string QuantityDifference { get; set; }
		public string ValueBuyDifference { get; set; }
		public string QuantityEdit { get; set; }
		public string ValueBuyEdit { get; set; }
		public string SortByField { get; set; }
		public string FullFiterAndSort { get; set; }

		public double QuantityEditDouble { get; set; }
		public double QuantityDifferenceERPLess { get; set; }
		public double ValueDifferenceERPLess { get; set; }
		public double QuantityDifferenceERPMore { get; set; }
		public double ValueDifferenceERPMore { get; set; }

		public string Value1 { get; set; }
		public string Value2 { get; set; }
		public string Value3 { get; set; }
		public string Value4 { get; set; }
		public string Value5 { get; set; }
		public string Value6 { get; set; }
		public string Value7 { get; set; }
		public string Value8 { get; set; }
		public string Value9 { get; set; }
		public string Value10 { get; set; }			   //use only for BranchCode minus CustomerCode

		public string StartInventorDateTime { get; set; }
		public string EndInventorDateTime { get; set; }

		public string StartWorkDateTime { get; set; }
		public string EndWorkDateTime { get; set; }
  		public string StartEndWorkDuration { get; set; }
		public int StartEndWorkDurationSumMin { get; set; }
		public int StartEndWorkDurationSumSec { get; set; }
		public int StartEndInventorDurationSumMin { get; set; }
		public int StartEndInventorDurationSumSec { get; set; }
		public ContextReport()
		{
			Code = "";
			
			CustomerAddress = "";
			CustomerCode = "";
			CustomerContactPerson = "";
			CustomerDescription = "";
			CustomerFax = "";
			CustomerMail = "";
			CustomerName = "";
			CustomerPhone = "";
			CustomerDBPath = "";
			CustomerLogoPath = "";
			CustomerTag = "";
			CustomerTag1 = "";
			CustomerTag2 = "";
			CustomerTag3 = "";

			BranchAddress = "";
			BranchCode = "";
			BranchContactPerson = "";
			BranchDescription = "";
			BranchFax = "";
			BranchLogoFile = "";
			BranchMail = "";
			BranchName = "";
			BranchPhone = "";
			BranchDBPath = "";
			BranchCodeLocal = "";
			BranchCodeERP = "";
			BranchTag = "";
			BranchTag1 = "";
			BranchTag2 = "";
			BranchTag3 = "";

			InventorCode = "";
			InventorName = "";
			InventorDescription = "";
			InventorDate = "";
			InventorDDMM = "";
			InventorYYYY = "";
			InventorStatus = "";
			InventorStatusCode = "";
			InventorDBPath = "";
			InventorTag = "";
			InventorTag1 = "";
			InventorTag2 = "";
			InventorTag3 = "";

			IturCode = "";
			ERPIturCode = "";
			IturNameFull = "";
			IturPrefix = "ITUR";
			IturName = "";
			IturNumber = "";
			IturStatusIturBit = "";
			IturStatusIturGroupBit = "";
			IturNumberPrefix = "";
			IturNumberSufix = "";
			IturStatusDocHeaderBit = "";

			LocationCode = "";
			LocationName = "";

			DocumentHeaderCode = "";
			DocumentCode = "";
			DocumentHeaderName = "";
			WorkerGUID = "";
			DocStatusDocHeaderBit = "";
			DocStatusApproveBit = "";

			SectionCode = "";
			SectionName = "";
			SectionNum = "";

			Makat = "";
			InputTypeCode = "";
			Barcode = "";
 			SerialNumber = "";
			ShelfCode = "";
			ProductName = "";
			PDAStatusInventProductBit = "";
			BarcodeOriginal = "";
			MakatOriginal = "";
			PriceString = "";
			PriceBuy = "";
			PriceSale = "";
			PriceExtra = "";
			PDANum = "";
			DocNum = "";

			ModifyDate = "";
			CreateDate = "";
			FromDate = "";
			ToDate = "";

			QuantityDifferenceOriginalERP = "";
			ValueDifferenceOriginalERP = "";
			QuantityDifference = "";
			ValueBuyDifference  = "";
			QuantityEdit  = "";
			ValueBuyEdit = "";
 			SortByField = "";
			FullFiterAndSort = "";

			this.QuantityEditDouble = 0;
			QuantityDifferenceERPLess = -100;
			ValueDifferenceERPLess = -100;

			QuantityDifferenceERPMore = 100;
			ValueDifferenceERPMore = 100;

			Value1 = "";
			Value2 = "";
			Value3 = "";
			Value4 = "";
			Value5 = "";
			Value6 = "";
			Value7 = "";
			Value8 = "";
			Value9 = "";
			Value10 = "";

			StartInventorDateTime = "";
			EndInventorDateTime = "";
			StartWorkDateTime = "";
			EndWorkDateTime = "";

		}

		public void Clear()
		{
			this.Code = "";

			this.IturCode = "";
			this.IturNameFull = "";
			this.ERPIturCode = "";
			this.IturPrefix = "";
			this.AppNameReport = "";
			this.LocationCode = "";
			this.DocumentHeaderCode = "";
			this.DocumentCode = "";
			this.SectionCode = "";

			this.IturName = "";
			this.LocationName = "";
			this.DocumentHeaderName = "";
			this.SectionName = "";
			this.SectionNum = "";

			this.CustomerAddress = "";
			this.CustomerCode = "";
			this.CustomerContactPerson = "";
			this.CustomerDescription = "";
			this.CustomerFax = "";
			this.CustomerMail = "";
			this.CustomerName = "";
			this.CustomerPhone = "";
			this.CustomerDBPath = "";
			this.CustomerLogoPath = "";
			this.CustomerTag = "";
			this.CustomerTag1 = "";
			this.CustomerTag2 = "";
			this.CustomerTag3 = "";

			this.BranchAddress = "";
			this.BranchCode = "";
			this.BranchContactPerson = "";
			this.BranchDescription = "";
			this.BranchFax = "";
			this.BranchLogoFile = "";
			this.BranchMail = "";
			this.BranchName = "";
			this.BranchPhone = "";
			this.BranchDBPath = "";
			this.BranchCodeLocal = "";
			this.BranchCodeERP = "";
			this.BranchTag = "";
			this.BranchTag1 = "";
			this.BranchTag2 = "";
			this.BranchTag3 = "";

			this.InventorCode = "";
			this.InventorName = "";
			this.InventorDescription = "";
			this.InventorDate = "";
			this.InventorDDMM = "";
			this.InventorYYYY = "";
			this.InventorStatus = "";
			this.InventorStatusCode = "";
			this.InventorDBPath = "";
			this.InventorTag = "";
			this.InventorTag1 = "";
			this.InventorTag2 = "";
			this.InventorTag3 = "";

			this.IturNumber = "";
			this.IturStatusIturBit = "";
			this.IturStatusIturGroupBit = "";
			this.IturNumberPrefix = "";
			this.IturNumberSufix = "";
			this.IturStatusDocHeaderBit = "";

			this.WorkerGUID = "";
			this.DocStatusDocHeaderBit = "";
			this.DocStatusApproveBit = "";

			this.Makat = "";
			this.InputTypeCode = "";
			this.Barcode = "";
			this.ModifyDate = "";
			this.CreateDate = "";
			this.FromDate = "";
			this.ToDate = "";

			this.SerialNumber = "";
			this.ShelfCode = "";
			this.ProductName = "";
			this.PDAStatusInventProductBit = "";
			this.BarcodeOriginal = "";
			this.MakatOriginal = "";
			this.PriceString = "";
			this.PriceBuy = "";
			this.PriceSale = "";
			this.PriceExtra = "";
			this.PDANum = "";
			this.DocNum = "";

			this.QuantityDifferenceOriginalERP = "";
			this.ValueDifferenceOriginalERP = "";
			this.QuantityDifference = "";
			this.ValueBuyDifference = "";
			this.QuantityEdit = "";
			this.ValueBuyEdit = "";
			this.SortByField = "";
			this.FullFiterAndSort = "";

			this.QuantityEditDouble = 0;
			this.QuantityDifferenceERPLess = -100;
			this.ValueDifferenceERPLess = -100;

			this.QuantityDifferenceERPMore = 100;
			this.ValueDifferenceERPMore = 100;

			this.Value1 = "";
			this.Value2 = "";
			this.Value3 = "";
			this.Value4 = "";
			this.Value5 = "";
			this.Value6 = "";
			this.Value7 = "";
			this.Value8 = "";
			this.Value9 = "";
			this.Value10 = "";
		}

	
	}

	
}
