using System;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Extensions;

namespace Count4U.Model.Count4U
{
    [Serializable]
    public class InventProduct
    {
        public long ID { get; set; }
        public string DocumentHeaderCode { get; set; }
        public string DocumentCode { get; set; }
        public string IturCode { get; set; }
        public string Code { get; set; }
        public string Makat { get; set; }
        public string InputTypeCode { get; set; }

        public string Barcode { get; set; }		
        public DateTime CreateDate { get; set; }
		public DateTime? ModifyDate { get; set; }
        //public string DocumentHeader { get; set; }
        public bool? PartialPackage { get; set; }
        public double QuantityDifference { get; set; }
        public double QuantityEdit { get; set; }
        [PropertyLink] public int QuantityInPackEdit { get; set; }
        public double QuantityOriginal { get; set; }

        public string ShelfCode { get; set; }
        //public string StatusInventProduct { get; set; }
        public string ProductName { get; set; }
        public string StatusInventProductCode { get; set; }
        public int StatusInventProductBit { get; set; }
        //public long Num { get; set; }
        public int FromCatalogType { get; set; }
        public int SectionNum { get; set; }
        public string TypeMakat { get; set; }
        public int IPNum { get; set; }
        public string ImputTypeCodeFromPDA { get; set; }
        public int DocNum { get; set; }
        public int SessionNum { get; set; }
        public string SessionCode { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public double PriceBuy { get; set; }
        public double PriceSale { get; set; }	
		 public string WorkerID { get; set; }
		public string SupplierName { get; set; }

		[PropertyLink]
		public string SupplierCode { get; set; }
		[PropertyLink]
		public string SerialNumber { get; set; }
		[PropertyLink]
		public string ItemStatus { get; set; }
        [PropertyLink]
        public string IPValueStr1 { get; set; }
        [PropertyLink]
        public string IPValueStr2 { get; set; }
        [PropertyLink]
        public string IPValueStr3 { get; set; }
        [PropertyLink]
        public string IPValueStr4 { get; set; }
        [PropertyLink]
        public string IPValueStr5 { get; set; }
        [PropertyLink]
        public string IPValueStr6 { get; set; }
        [PropertyLink]
        public string IPValueStr7 { get; set; }
        [PropertyLink]
        public string IPValueStr8 { get; set; }
        [PropertyLink]
        public string IPValueStr9 { get; set; }
        [PropertyLink]
        public string IPValueStr10 { get; set; }
		[PropertyLink]
		public string IPValueStr11 { get; set; }
		[PropertyLink]
		public string IPValueStr12 { get; set; }
		[PropertyLink]
		public string IPValueStr13 { get; set; }
		[PropertyLink]
		public string IPValueStr14 { get; set; }
		[PropertyLink]
		public string IPValueStr15 { get; set; }
		[PropertyLink]
		public string IPValueStr16 { get; set; }
		[PropertyLink]
		public string IPValueStr17 { get; set; }
		[PropertyLink]
		public string IPValueStr18 { get; set; }
		[PropertyLink]
		public string IPValueStr19 { get; set; }
		[PropertyLink]
		public string IPValueStr20 { get; set; }
        [PropertyLink]
        public double IPValueFloat1 { get; set; }
        [PropertyLink]
        public double IPValueFloat2 { get; set; }
        [PropertyLink]
        public double IPValueFloat3 { get; set; }
        [PropertyLink]
        public double IPValueFloat4 { get; set; }
        [PropertyLink]
        public double IPValueFloat5 { get; set; }
        [PropertyLink]
        public int IPValueInt1 { get; set; }
        [PropertyLink]
        public int IPValueInt2 { get; set; }
        [PropertyLink]
        public int IPValueInt3 { get; set; }
        [PropertyLink]
        public int IPValueInt4 { get; set; }
        [PropertyLink]
        public int IPValueInt5 { get; set; }
        [PropertyLink]
        public bool IPValueBit1 { get; set; }
        [PropertyLink]
        public bool IPValueBit2 { get; set; }
        [PropertyLink]
        public bool IPValueBit3 { get; set; }
        [PropertyLink]
        public bool IPValueBit4 { get; set; }
        [PropertyLink]
        public bool IPValueBit5 { get; set; }

		public string ERPIturCode { get; set; }


		//#1601
		//	Makat 300
		//Barcode: 300
		//ProductName: 100
		//Code: 300
		//ERPIturCode: 250

		//TO ADD in COUNT4U
		public string UnityCode { get; set; }
		public string LocationCode { get; set; }	 
		public string Tag { get; set; }	   
		public string Tag1 { get; set; }
		public string Tag2  { get; set; }
		public string Tag3 { get; set; }
		public double QuantityWithoutPackEdit { get; set; }
		public double ValueBuyDifference { get; set; }
		public double ValueBuyEdit { get; set; }
		public double ValueBuyQriginal { get; set; }
		public double ValueBuyWithoutPackEdit { get; set; }
		public double ValueBuyInPackEdit { get; set; }
		public double ValueSaleDifference { get; set; }
		public double ValueSaleEdit { get; set; }
		public double ValueSaleQriginal { get; set; }
		public double ValueSaleWithoutPackEdit { get; set; }
		public double ValueSaleInPackEdit { get; set; }
		//проверить все в catalog
	

		//Barcode need increece length

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(InventProduct)) return false;
            return Equals((InventProduct)obj);
        }

        public bool Equals(InventProduct other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ID, this.ID);					  // TODO: вопрос как сравнивать
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

		public InventProduct()
		{
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
			this.IPValueFloat1 = 0;
			this.IPValueFloat2 = 0;
			this.IPValueFloat3 = 0;
			this.IPValueFloat4 = 0;
			this.IPValueFloat5 = 0;
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
			this.QuantityInPackEdit = 0;
			this.DocumentHeaderCode = "";
			this.DocumentCode = "";
			this.IturCode = "";
			this.Code = "";
			this.Makat = "";
			this.InputTypeCode = "";
			this.Barcode = "";
			this.CreateDate = DateTime.Now;
			this.ModifyDate = ModelUtils.GetMinDateTime();
			//this.DocumentHeader = "";
			this.PartialPackage = false;
			this.QuantityDifference = 0;
			this.QuantityEdit = 0;
			this.QuantityOriginal = 0;
			this.SerialNumber = "";
			this.ShelfCode = "";
			this.ProductName = "";
			this.StatusInventProductCode = "";
			this.StatusInventProductBit = 0;
			this.FromCatalogType = 0;
			this.SectionNum = 0;
			this.TypeMakat = "";
			this.IPNum = 0;
			this.ImputTypeCodeFromPDA = "";
			this.DocNum = 0;
			this.SessionNum = 0;
			this.SessionCode = "";
			this.SectionCode = "";
			this.SectionName = "";
			this.PriceBuy = 0;
			this.PriceSale = 0;
			this.WorkerID = "";
			this.SupplierCode = "";
			this.SupplierName = "";
			this.ItemStatus = "";
			this.ERPIturCode = "";
			this.UnityCode = "";
			this.LocationCode = "";
			this.Tag = "";
			this.Tag1 = "";
			this.Tag2 = "";
			this.Tag3 = "";
			this.QuantityWithoutPackEdit = 0;
			this.ValueBuyDifference = 0;
			this.ValueBuyEdit = 0;
			this.ValueBuyQriginal = 0;
			this.ValueBuyWithoutPackEdit = 0;
			this.ValueBuyInPackEdit = 0;
			this.ValueSaleDifference = 0;
			this.ValueSaleEdit = 0;
			this.ValueSaleQriginal = 0;
			this.ValueSaleWithoutPackEdit = 0;
			this.ValueSaleInPackEdit = 0;
		}

		//public InventProduct Clone()
		//{
		public InventProduct(InventProduct source)
		{
			if (source == null) return;

			this.IPValueStr1 = source.IPValueStr1;
			this.IPValueStr2 = source.IPValueStr2;
			this.IPValueStr3 = source.IPValueStr3;
			this.IPValueStr4 = source.IPValueStr4;
			this.IPValueStr5 = source.IPValueStr5;
			this.IPValueStr6 = source.IPValueStr6;
			this.IPValueStr7 = source.IPValueStr7;
			this.IPValueStr8 = source.IPValueStr8;
			this.IPValueStr9 = source.IPValueStr9;
			this.IPValueStr10 = source.IPValueStr10;
			this.IPValueStr11 = source.IPValueStr11;
			this.IPValueStr12 = source.IPValueStr12;
			this.IPValueStr13 = source.IPValueStr13;
			this.IPValueStr14 = source.IPValueStr14;
			this.IPValueStr15 = source.IPValueStr15;
			this.IPValueStr16 = source.IPValueStr16;
			this.IPValueStr17 = source.IPValueStr17;
			this.IPValueStr18 = source.IPValueStr18;
			this.IPValueStr19 = source.IPValueStr19;
			this.IPValueStr20 = source.IPValueStr20;
			this.IPValueFloat1 = source.IPValueFloat1;
			this.IPValueFloat2 = source.IPValueFloat2;
			this.IPValueFloat3 = source.IPValueFloat3;
			this.IPValueFloat4 = source.IPValueFloat4;
			this.IPValueFloat5 = source.IPValueFloat5;
			this.IPValueInt1 = source.IPValueInt1;
			this.IPValueInt2 = source.IPValueInt2;
			this.IPValueInt3 = source.IPValueInt3;
			this.IPValueInt4 = source.IPValueInt4;
			this.IPValueInt5 = source.IPValueInt5;
			this.IPValueBit1 = source.IPValueBit1;
			this.IPValueBit2 = source.IPValueBit2;
			this.IPValueBit3 = source.IPValueBit3;
			this.IPValueBit4 = source.IPValueBit4;
			this.IPValueBit5 = source.IPValueBit5;
			this.QuantityInPackEdit = source.QuantityInPackEdit;
			this.DocumentHeaderCode = source.DocumentHeaderCode;
			this.DocumentCode = source.DocumentCode;
			this.IturCode = source.IturCode;
			this.Code = source.Code;
			this.Makat = source.Makat;
			this.InputTypeCode = source.InputTypeCode;
			this.Barcode = source.Barcode;
			this.CreateDate = source.CreateDate;
			this.ModifyDate = source.ModifyDate;
			this.PartialPackage = source.PartialPackage;
			this.QuantityDifference = source.QuantityDifference;
			this.QuantityEdit = source.QuantityEdit;
			this.QuantityOriginal = source.QuantityOriginal;
			this.SerialNumber = source.SerialNumber;
			this.ShelfCode = source.ShelfCode;
			this.ProductName = source.ProductName;
			this.StatusInventProductCode = source.StatusInventProductCode;
			this.StatusInventProductBit = source.StatusInventProductBit;
			this.FromCatalogType = source.FromCatalogType;
			this.SectionNum = source.SectionNum;
			this.TypeMakat = source.TypeMakat;
			this.IPNum = source.IPNum;
			this.ImputTypeCodeFromPDA = source.ImputTypeCodeFromPDA;
			this.DocNum = source.DocNum;
			this.SessionNum = source.SessionNum;
			this.SessionCode = source.SessionCode;
			this.SectionCode = source.SectionCode;
			this.SectionName = source.SectionName;
			this.PriceBuy = source.PriceBuy;
			this.PriceSale = source.PriceSale;
			this.WorkerID = source.WorkerID;
			this.SupplierCode = source.SupplierCode;
			this.SupplierName = source.SupplierName;
			this.ItemStatus =  source.ItemStatus;
			this.ERPIturCode =  source.ERPIturCode;
			this.UnityCode = UnityCode;
			this.LocationCode = LocationCode;
			this.Tag = Tag;
			this.Tag1 = Tag1;
			this.Tag2 = Tag2;
			this.Tag3 = Tag3;
			this.QuantityWithoutPackEdit = QuantityWithoutPackEdit;
			this.ValueBuyDifference = ValueBuyDifference;
			this.ValueBuyEdit = ValueBuyEdit;
			this.ValueBuyQriginal = ValueBuyQriginal;
			this.ValueBuyWithoutPackEdit = ValueBuyWithoutPackEdit;
			this.ValueBuyInPackEdit = ValueBuyInPackEdit;
			this.ValueSaleDifference = ValueSaleDifference;
			this.ValueSaleEdit = ValueSaleEdit;
			this.ValueSaleQriginal = ValueSaleQriginal;
			this.ValueSaleWithoutPackEdit = ValueSaleWithoutPackEdit;
			this.ValueSaleInPackEdit = ValueSaleInPackEdit;
		}






    }
}
