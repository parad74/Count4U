using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Count4U.Model.Count4Mobile
{
	[Table("TemplateInventory")] 
	public class TemplateInventory
    {
  		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		public string Uid { get; set; }	
		public string Level1Code { get; set; }
		public string Level2Code { get; set; }
		public string Level3Code { get; set; }
		public string Level4Code { get; set; }
		public string ItemCode { get; set; }
		public string QuantityExpected { get; set; }
		public string Tag { get; set; }	
		public string Domain { get; set; }

		public TemplateInventory()
		{
			this.Uid = "";
			this.Level1Code = "";
			this.Level2Code = "";
			this.Level3Code = "";
			this.Level4Code = "";
			this.ItemCode = "";
			this.QuantityExpected = "";
			this.Tag = "";
			this.Domain	 = "";		
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(TemplateInventory)) return false;
			return Equals((TemplateInventory)obj);
		}

		public bool Equals(TemplateInventory other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Id, this.Id));
		}

		public override int GetHashCode()
		{
			return (Id != null ? Id.GetHashCode() : 0);
		}
    }
}
