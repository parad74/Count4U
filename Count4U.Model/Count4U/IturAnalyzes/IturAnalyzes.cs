using System;

namespace Count4U.Model.Count4U
{
	public class IturAnalyzes
	{
		public long ID { get; set; }
		public bool? Itur_Disabled { get; set; }
		public string Code { get; set; }
		public string LocationCode { get; set; }
		public string Itur_LocationCode { get; set; }
		public int Itur_Number { get; set; }
		public bool? Itur_Publishe { get; set; }
		public int Itur_StatusIturBit { get; set; }
		public int Itur_StatusIturGroupBit { get; set; }
		public string Itur_NumberPrefix { get; set; }
		public string Itur_NumberSufix { get; set; }
		public int Itur_StatusDocHeaderBit { get; set; }
		public string Doc_Name { get; set; }				   //StatusInventProductCode
		public bool? Doc_Approve { get; set; }
		public string Doc_WorkerGUID { get; set; }
		public int Doc_StatusDocHeaderBit { get; set; }
		public int Doc_StatusInventProductBit { get; set; }
		public int Doc_StatusApproveBit { get; set; }
		public string DocumentHeaderCode { get; set; }
		public string DocumentCode { get; set; }
		public string IturCode { get; set; }
		public string ERPIturCode { get; set; }
		public string Makat { get; set; }
		public string InputTypeCode { get; set; }
		public string Barcode { get; set; }
		public DateTime? ModifyDate { get; set; }
	//	public bool? PartialPackage { get; set; }
		public double QuantityDifference { get; set; }
		public double QuantityEdit { get; set; }
		public double QuantityOriginal { get; set; }
		public double ValueBuyDifference { get; set; }
		public double ValueBuyEdit { get; set; }
		public double ValueBuyQriginal { get; set; }
		public string SerialNumber { get; set; }
		public string ShelfCode { get; set; }
		public string ProductName { get; set; }
		public int PDA_StatusInventProductBit { get; set; }
		public string BarcodeOriginal { get; set; }
		public string MakatOriginal { get; set; }
		public string PriceString { get; set; }
		public double Price { get; set; }
		public double PriceBuy { get; set; }
		public double PriceSale { get; set; }
		public double PriceExtra { get; set; }
		public int IPNum { get; set; }
		public int DocNum { get; set; }
		public int FromCatalogType { get; set; }
		public int SectionNum { get; set; }
		public string TypeCode { get; set; }
		public string TypeMakat { get; set; }
		public int Count { get; set; }
		public string ValueChar { get; set; }  //TODO Family
		//public string Family { get; set; }
		public int ValueInt { get; set; }
		public double ValueFloat { get; set; }
		public string SectionCode { get; set; }

		public string SectionName { get; set; }

		public int ERPType { get; set; }
		public long PDA_ID { get; set; }

		public bool? IsResulte { get; set; }
		public string ResultCode { get; set; }
		public string ResulteDescription { get; set; }
		public bool? IsUpdateERP { get; set; }
		public string ImputTypeCodeFromPDA { get; set; }
		public string ResulteValue { get; set; }

		//public string PDA_Makat { get; set; }

		public double QuantityOriginalERP { get; set; }
		public double ValueOriginalERP { get; set; }
		public double QuantityDifferenceOriginalERP { get; set; }
		public double ValueDifferenceOriginalERP { get; set; }

		public int BalanceQuantityPartialERP { get; set; }
		public int QuantityInPackEdit { get; set; }
		public int CountInParentPack { get; set; }

		//public double QuantityOriginalERPAndPartial { get; set; }			 //		 + BalanceQuantityPartialERP/CountInParentPack
		//public double ValueOriginalERPAndPartial { get; set; }				  
		//public double QuantityDifferenceOriginalERPAndPartial { get; set; }
		//public double ValueDifferenceOriginalERPAndPartial { get; set; }

		//public double QuantityEditAndPartial { get; set; }						 //			 + QuantityInPackEdit/CountInParentPack
		//public double ValueEditAndPartial { get; set; }
		//public double QuantityDifferenceEditAndPartial { get; set; }
		//public double ValueDifferenceEditAndPartial { get; set; }	

		public string SupplierCode { get; set; }
		public string SupplierName { get; set; }

		public string IturName { get; set; }
		public string LocationName { get; set; }
		public string SessionCode { get; set; }
		public int SessionNum { get; set; }

		public string WorkerID { get; set; }
		public string WorkerName { get; set; }
		public long Total { get; set; }
		public DateTime FromTime { get; set; }
		public DateTime ToTime { get; set; } //ToTime
		 
		public long TicksTimeSpan { get; set; }
		public string PeriodFromTo { get; set; }

		public string FamilyCode { get; set; }
		public string FamilyName { get; set; }
		public string FamilyType { get; set; }
		public string FamilySize { get; set; }
		public string FamilyExtra1 { get; set; }
		public string FamilyExtra2 { get; set; }

		public string UnitTypeCode { get; set; }
		public string InventorCode { get; set; }
		public string InventorName { get; set; }
		public string BranchCode { get; set; }
		public string BranchName { get; set; }
		public string BranchERPCode { get; set; }
		public DateTime InventorDate { get; set; }

		public string IPValueStr1 { get; set; }
		public string IPValueStr2 { get; set; }
		public string IPValueStr3 { get; set; }
		public string IPValueStr4 { get; set; }
		public string IPValueStr5 { get; set; }
		public string IPValueStr6 { get; set; }
		public string IPValueStr7 { get; set; }
		public string IPValueStr8 { get; set; }
		public string IPValueStr9 { get; set; }
		public string IPValueStr10 { get; set; }
		public string IPValueStr11 { get; set; }
		public string IPValueStr12 { get; set; }
		public string IPValueStr13 { get; set; }
		public string IPValueStr14 { get; set; }
		public string IPValueStr15 { get; set; }
		public string IPValueStr16 { get; set; }
		public string IPValueStr17 { get; set; }
		public string IPValueStr18 { get; set; }
		public string IPValueStr19 { get; set; }
		public string IPValueStr20 { get; set; }
		public double IPValueFloat1 { get; set; }
		public double IPValueFloat2 { get; set; }
		public double IPValueFloat3 { get; set; }
		public double IPValueFloat4 { get; set; }
		public double IPValueFloat5 { get; set; }
		public int IPValueInt1 { get; set; }
		public int IPValueInt2 { get; set; }
		public int IPValueInt3 { get; set; }
		public int IPValueInt4 { get; set; }
		public int IPValueInt5 { get; set; }
		public bool IPValueBit1 { get; set; }
		public bool IPValueBit2 { get; set; }
		public bool IPValueBit3 { get; set; }
		public bool IPValueBit4 { get; set; }
		public bool IPValueBit5 { get; set; }
		public string IturCodeExpected { get; set; }
		public bool IturCodeDiffer { get; set; }
		//TO ADD in COUNT4U
		 //StatusInventProductCode

		public string SubSessionCode { get; set; } //надо для отчета	имела ввиду 	 SubSectionCode		 
		public string SessionName { get; set; }			 //имела ввиду 	 SectionName		 
		public string SubSessionName { get; set; }		//имела ввиду 	 SubSectionName	  :)

		//#1601
		//ERPIturCode: 250

		public IturAnalyzes()
		{
			this.Itur_Disabled = false;
			this.Code = "";
			this.LocationCode = "";
			this.Itur_LocationCode = "";
			this.Itur_Number = 0;
			this.Itur_Publishe = false;
			this.Itur_StatusIturBit = 0;
			this.Itur_StatusIturGroupBit = 0;
			this.Itur_NumberPrefix = "";
			this.Itur_NumberSufix = "";
			this.Itur_StatusDocHeaderBit = 0;
			this.Doc_Name = "";
			this.Doc_Approve = false;
			this.Doc_WorkerGUID = "";
			this.Doc_StatusDocHeaderBit = 0;
			this.Doc_StatusInventProductBit = 0;
			this.Doc_StatusApproveBit = 0;
			this.DocumentHeaderCode = "";
			this.DocumentCode = "";
			this.IturCode = "";
			this.ERPIturCode = "";
			this.Makat = "";
			this.InputTypeCode = "";
			this.Barcode = "";
			this.ModifyDate = DateTime.Now;
		//	this.PartialPackage = false;
			this.QuantityDifference = 0.0;
			this.QuantityEdit = 0.0;
			this.QuantityOriginal = 0.0;
			this.ValueBuyDifference = 0.0;
			this.ValueBuyEdit = 0.0;
			this.ValueBuyQriginal = 0.0;
			this.SerialNumber = "";
			this.ShelfCode = "";
			this.ProductName = "NotExistInCatalog";
			this.PDA_StatusInventProductBit = 0;
			this.BarcodeOriginal = "";
			this.MakatOriginal = "";
			this.PriceString = "";
			this.Price = 0.0;
			this.PriceBuy = 0.0;
			this.PriceSale = 0.0;
			this.PriceExtra = 0.0;
			this.IPNum = 0;
			this.DocNum = 0;
			this.FromCatalogType = 0;
			this.SectionNum = 0;
			this.TypeCode = "";
			this.TypeMakat = "";
			this.Count = 0;
			this.ValueChar = "";
			this.ValueInt = 0;
			this.ValueFloat = 0.0;
			this.SectionCode = "";

			this.SectionName = "";

			this.ERPType = 0;
			this.PDA_ID = 0;

			this.IsResulte = false;
			this.ResultCode = "XXX";
			this.ResulteDescription = "";
			this.IsUpdateERP = false;
			this.ImputTypeCodeFromPDA = "";
			this.ResulteValue = "";

			this.QuantityOriginalERP = 0.0;
			this.ValueOriginalERP = 0.0;
			this.QuantityDifferenceOriginalERP = 0.0;
			this.ValueDifferenceOriginalERP = 0.0;

			this.BalanceQuantityPartialERP = 0;
			this.QuantityInPackEdit = 0;
			this.CountInParentPack = 1;

			this.SupplierCode = "";
			this.SupplierName = "";

			this.IturName = "";
			this.LocationName = "";
			this.SessionCode = "";
			this.SessionNum = 0;

			this.WorkerID = "";
			this.WorkerName = "";
			this.Total = 0;
			this.FromTime = DateTime.Now;
			this.ToTime = DateTime.Now;//
			this.TicksTimeSpan = 0;
			this.PeriodFromTo = "";

			this.FamilyCode = "";
			this.FamilyName = "";
			this.FamilyType = "";
			this.FamilySize = "";
			this.FamilyExtra1 = "";
			this.FamilyExtra2 = "";

			this.UnitTypeCode = "";
			this.InventorCode = "";
			this.InventorName = "";
			this.BranchCode = "";
			this.BranchName = "";
			this.BranchERPCode = "";
			this.InventorDate = DateTime.Now;
			this.IturCodeExpected  = "";
			this.IturCodeDiffer = false;

			this.SubSessionCode   = "";
			this.SessionName   = "";
			this.SubSessionName = "";

			this.IPValueStr1 = "";
			this.IPValueStr2 = "";
			this.IPValueStr3 = "";
			this.IPValueStr4 = "";
			this.IPValueStr5 = "";
			this.IPValueStr6 = "";
			this.IPValueStr7 = "";
			this.IPValueStr8 = "";
			this.IPValueStr9 = "";
			this.IPValueStr10 = "";
			this.IPValueStr11 = "";
			this.IPValueStr12 = "";
			this.IPValueStr13 = "";
			this.IPValueStr14 = "";
			this.IPValueStr15 = "";
			this.IPValueStr16 = "";
			this.IPValueStr17 = "";
			this.IPValueStr18 = "";
			this.IPValueStr19 = "";
			this.IPValueStr20 = "";
			this.IPValueFloat1 = 0.0;
			this.IPValueFloat2 = 0.0;
			this.IPValueFloat3 = 0.0;
			this.IPValueFloat4 = 0.0;
			this.IPValueFloat5 = 0.0;
			this.IPValueInt1 = 0;
			this.IPValueInt2 = 0;
			this.IPValueInt3 = 0;
			this.IPValueInt4 = 0;
			this.IPValueInt5 = 0;
			this.IPValueBit1 = false;
			this.IPValueBit2 = false;
			this.IPValueBit3 = false;
			this.IPValueBit4 = false;
			this.IPValueBit5 = false;
		}
	}
}