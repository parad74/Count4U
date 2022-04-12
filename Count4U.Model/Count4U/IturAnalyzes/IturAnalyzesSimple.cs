using System;

namespace Count4U.Model.Count4U
{
    [Serializable]
	public class IturAnalyzesSimple
	{
		public string IturCode { get; set; }
		public string Makat { get; set; }
		public long MakatLong { get; set; }
		public string Barcode { get; set; }
		public double QuantityOriginal { get; set; }
		public double QuantityOriginalERP { get; set; } //new
		public double? QuantityEdit { get; set; }
		public int QuantityInPackEdit { get; set; }
		public double IPValueFloat5 { get; set; }
		public int CountInParentPack { get; set; }	   //new
		public string ProductName { get; set; }
		public string MakatOriginal { get; set; }
		public double PriceBuy { get; set; }	  //new
		public double PriceSale { get; set; }	  //new
		public double Price { get; set; } //new
		public int FromCatalogType { get; set; }
		public string TypeMakat { get; set; }
		public int Count { get; set; }
		public string SectionCode { get; set; }
		public string SubSectionCode { get; set; }
		public string UnitTypeCode { get; set; }
        public string FamilyCode { get; set; }
		public string Tag { get; set; }
		public string SupplierCode { get; set; }
		
        
        


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(IturAnalyzesSimple)) return false;
			return Equals((IturAnalyzesSimple)obj);
		}

		public bool Equals(IturAnalyzesSimple other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Makat, this.Makat);
		}

		public override int GetHashCode()
		{
			return (Makat != null ? Makat.GetHashCode() : 0);
		}



		
	}
}