using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
    public class  Catalog
    {
        public string Uid { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
		public string FamilyCode { get; set; }					
        public string FamilyName { get; set; }
        public string SectionCode { get; set; }				   
		public string SectionName { get; set; }					
        public string SubSectionCode { get; set; }
        public string SubSectionName { get; set; }
        public string PriceBuy { get; set; }
        public string PriceSell { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string UnitTypeCode { get; set; }
        public string Description { get; set; }

		public Catalog()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.ItemCode = "";
			this.ItemName = "";
			this.ItemType = "";
			this.FamilyCode = "";
			this.FamilyName = "";
			this.SectionCode = "";
			this.SectionName = "";
			this.SubSectionCode = "";
			this.SubSectionName = "";
			this.PriceBuy = "";
			this.PriceSell = "";
			this.SupplierCode = "";
			this.SupplierName = "";
			this.UnitTypeCode = "";
			this.Description = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Catalog)) return false;
			return Equals((Catalog)obj);
		}

		public bool Equals(Catalog other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
