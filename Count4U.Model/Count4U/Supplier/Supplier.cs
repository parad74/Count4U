using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class Supplier 
	{
        public long ID { get; set; }
		public string SupplierCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public int PlaceCount { get; set; }
        public double Value { get; set; }
		public int IturCount { get; set; }
		public int SupplierLevel { get; set; }
        public double Area { get; set; }
        public double PercentArea { get; set; }


		public Supplier()
		{
			this.SupplierCode = DomainUnknownCode.UnknownSupplier; 
			this.Name = DomainUnknownName.UnknownSupplier; 
			this.Description = "";
			this.PlaceCount = 0;
			this.Value = 0;
			this.IturCount = 0;
			this.SupplierLevel = 0;
			this.Area = 0;
			this.PercentArea = 0;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Supplier)) return false;
			return Equals((Supplier)obj);
		}

		public bool Equals(Supplier other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.SupplierCode, this.SupplierCode);
		}

		public override int GetHashCode()
		{
			return (SupplierCode != null ? SupplierCode.GetHashCode() : 0);
		}
	}

}
