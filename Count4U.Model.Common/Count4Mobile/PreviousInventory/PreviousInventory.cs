using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Count4U.Model.Count4Mobile
{
	[Table("PreviousInventory")] 
	public class PreviousInventory
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
        public string Uid { get; set; }
		//[Column("SerialNumber", TypeName="string")]  так можно задавать имя для генерации DBContext
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

		public string PropExtenstion1 { get; set; }
		public string PropExtenstion2 { get; set; }
		public string PropExtenstion3 { get; set; }
		public string PropExtenstion4 { get; set; }
		public string PropExtenstion5 { get; set; }
		public string PropExtenstion6 { get; set; }
		public string PropExtenstion7 { get; set; }
		public string PropExtenstion8 { get; set; }
		public string PropExtenstion9 { get; set; }
		public string PropExtenstion10 { get; set; }
		public string PropExtenstion11 { get; set; }
		public string PropExtenstion12 { get; set; }
		public string PropExtenstion13 { get; set; }
		public string PropExtenstion14 { get; set; }
		public string PropExtenstion15 { get; set; }
		public string PropExtenstion16 { get; set; }
		public string PropExtenstion17 { get; set; }
		public string PropExtenstion18 { get; set; }
		public string PropExtenstion19 { get; set; }
		public string PropExtenstion20 { get; set; }
		public string PropExtenstion21 { get; set; }
		public string PropExtenstion22 { get; set; }

		public PreviousInventory()
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
			this.PropExtenstion1 = "";
			this.PropExtenstion2 = "";
			this.PropExtenstion3 = "";
			this.PropExtenstion4 = "";
			this.PropExtenstion5 = "";
			this.PropExtenstion6 = "";
			this.PropExtenstion7 = "";
			this.PropExtenstion8 = "";
			this.PropExtenstion9 = "";
			this.PropExtenstion10 = "";
			this.PropExtenstion11 = "";
			this.PropExtenstion12 = "";
			this.PropExtenstion13 = "";
			this.PropExtenstion14 = "";
			this.PropExtenstion15 = "";
			this.PropExtenstion16 = "";
			this.PropExtenstion17 = "";
			this.PropExtenstion18 = "";
			this.PropExtenstion19 = "";
			this.PropExtenstion20 = "";
			this.PropExtenstion21 = "";
			this.PropExtenstion22 = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PreviousInventory)) return false;
			return Equals((PreviousInventory)obj);
		}

		public bool Equals(PreviousInventory other)
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
