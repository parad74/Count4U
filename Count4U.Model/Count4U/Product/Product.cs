using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    [Serializable]
	public class Product : BaseProduct
	{
        public long ID { get; set; }
		public string Code { get; set; }
		public string Barcode { get; set; }
		public string Makat { get; set; }
		public string MakatERP { get; set; }
   		//public int TypeMakatCode{ get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double BalanceQuantityERP { get; set; }
        public int? BalanceQuantityPartialERP { get; set; }
        public string Tag { get; set; }
        public long? CountMax { get; set; }
        public long? CountMin { get; set; }
        public string Family { get; set; }
	    public string FamilyCode { get; set; }
        public long? Importance { get; set; }

        public double PriceBuy { get; set; }
        public double PriceExtra { get; set; }
        public double PriceSale { get; set; }
        public string PriceString { get; set; }

		//public string Section { get; set; }
		//public string Supplier { get; set; }
		public string SectionCode { get; set; }
		public string SupplierCode { get; set; }
		public string TypeCode { get; set; }
		public string UnitTypeCode { get; set; }
		public string InputTypeCode { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime? ModifyDate { get; set; }
		//public string ParentCode { get; set; }
		//public string ParentBarcode { get; set; }
		public string ParentMakat { get; set; }
		public int? CountInParentPack { get; set; }
		public string ParserBag { get; set; }
		public string MakatOriginal { get; set; }
		public int FromCatalogType { get; set; }
		public bool IsUpdateERP { get; set; }
		public string IturCodeExpected { get; set; }

		//TO ADD in COUNT4U
		public string SectionName { get; set; }
		public string SubSectionCode { get; set; }
		public string SubSectionName { get; set; }
		public string SupplierName { get; set; }
		public string ItemType { get; set; }

		//#1061
		//Name: 100
		//Makat 300
		//Barcode 300
		//ParentMakat 300
		//MakatOriginal 300
		//Code: 300
	
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Product)) return false;
			return Equals((Product)obj);
		}

		public bool Equals(Product other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Makat, this.Makat);			  // TODO: принять решение
		}

		public override int GetHashCode()
		{
			return (Makat != null ? Makat.GetHashCode() : 0);
		}

		public Product()
		{
			Code = "";
			Barcode = "";
			Makat = "";
			MakatERP = "";
			// TypeMakatCode= 0;
			Name = "";
			Description = "";
			BalanceQuantityERP = 0;
			BalanceQuantityPartialERP = 0;
			Tag = "";
			CountMax = 0;
			CountMin = 0;
			Family = "";
			FamilyCode = "";
			Importance = 0;

			PriceBuy = 0;
			PriceExtra = 0;
			PriceSale = 0;
			PriceString = "";

			//Section = "";
			//Supplier = "";
			SectionCode = "";
			SupplierCode = "";
			TypeCode = "";
			UnitTypeCode = "";
			InputTypeCode = "";
			CreateDate = DateTime.Now;
			ModifyDate = DateTime.Now;
			//ParentCode = "";
			//ParentBarcode = "";
			ParentMakat = "";
			CountInParentPack = 1;
			ParserBag = "";
			MakatOriginal = "";
			FromCatalogType = 0;
			IsUpdateERP = false;
			IturCodeExpected = "";
			SectionName = "";
			SubSectionCode = "";
			SubSectionName = "";
			SupplierName = "";
			ItemType = "";
		}


		protected override BaseProduct Create()
		{
			return new Product();
		}
	}
}
