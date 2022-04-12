using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Count4U.Model.Count4Mobile
{
	[Table("CurrentInventory")] 
	public class CurrentInventory
    {
  		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
        public string Uid { get; set; }
        public string SerialNumberLocal { get; set; }
        public string ItemCode { get; set; }
        public string SerialNumberSupplier { get; set; }
        public string Quantity { get; set; }
        public string PropertyStr1 { get; set; }
        public string PropertyStr2 { get; set; }
        public string PropertyStr3 { get; set; }
        public string PropertyStr4 { get; set; }
        public string PropertyStr5 { get; set; }
        public string PropertyStr6 { get; set; }
        public string PropertyStr7 { get; set; }
        public string PropertyStr8 { get; set; }
        public string PropertyStr9 { get; set; }
        public string PropertyStr10 { get; set; }
        public string PropertyStr11 { get; set; }
        public string PropertyStr12 { get; set; }
        public string PropertyStr13 { get; set; }
        public string PropertyStr14 { get; set; }
        public string PropertyStr15 { get; set; }
        public string PropertyStr16 { get; set; }
        public string PropertyStr17 { get; set; }
        public string PropertyStr18 { get; set; }
        public string PropertyStr19 { get; set; }
        public string PropertyStr20 { get; set; }
        public string LocationCode { get; set; }
        public string DateModified { get; set; }
        public string DateCreated { get; set; }
        public string ItemStatus { get; set; }

		public string ItemType { get; set; }
		public string UnitTypeCode { get; set; }
		


		public CurrentInventory()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.SerialNumberLocal = "";
			this.ItemCode = "";
			this.SerialNumberSupplier = "";
			this.Quantity = "";
			this.PropertyStr1 = "";
			this.PropertyStr2 = "";
			this.PropertyStr3 = "";
			this.PropertyStr4 = "";
			this.PropertyStr5 = "";
			this.PropertyStr6 = "";
			this.PropertyStr7 = "";
			this.PropertyStr8 = "";
			this.PropertyStr9 = "";
			this.PropertyStr10 = "";
			this.PropertyStr11 = "";
			this.PropertyStr12 = "";
			this.PropertyStr13 = "";
			this.PropertyStr14 = "";
			this.PropertyStr15 = "";
			this.PropertyStr16 = "";
			this.PropertyStr17 = "";
			this.PropertyStr18 = "";
			this.PropertyStr19 = "";
			this.PropertyStr20 = "";
			this.LocationCode = "";
			this.DateModified = "";
			this.DateCreated = "";
			this.ItemStatus = "";
			this.ItemType = "";
			this.UnitTypeCode = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(CurrentInventory)) return false;
			return Equals((CurrentInventory)obj);
		}

		public bool Equals(CurrentInventory other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}

		public override int GetHashCode()
		{
			return (Uid != null ? Uid.GetHashCode() : 0);
		}
    }
}
