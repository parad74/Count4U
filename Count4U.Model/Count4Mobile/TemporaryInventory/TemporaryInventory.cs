using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Count4U.Model.Count4Mobile
{
	[Table("TemporaryInventory")] 
	public class TemporaryInventory
    {
  		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		public string Operation { get; set; }
		public string Domain { get; set; }				   //add

		public string OldUid { get; set; }				  //parent
		public string NewUid { get; set; }				//	chalde

        public string OldSerialNumber { get; set; }
		public string OldItemCode { get; set; }
		public string OldLocationCode { get; set; }
		public string OldProductCode { get; set; }				 //add
		public string OldKey { get; set; }

		public string NewSerialNumber { get; set; }
		public string NewItemCode { get; set; }
		public string NewLocationCode { get; set; }
		public string NewProductCode { get; set; }		   //add
		public string NewKey { get; set; }

		public string DateModified { get; set; }
		public string Device { get; set; }
		public string DbFileName { get; set; }
		public string Tag { get; set; }			 //add
		public string Description { get; set; }	  //add

		public TemporaryInventory()
		{
			this.OldUid = "";			  //parent
			this.NewUid = "";			//	chalde
			this.Domain	 = "";		

			this.OldSerialNumber = "";
			this.OldItemCode = "";
			this.OldLocationCode = "";
			this.OldProductCode	 = "";
			this.OldKey = "";

			this.NewSerialNumber = "";
			this.NewItemCode = "";
			this.NewLocationCode = "";
			this.NewProductCode = "";
			this.NewKey = "";

			this.DateModified = "";
			this.Operation = "";
			this.Device = "";
			this.DbFileName = "";
			this.Tag  = "";
			this.Description = "";

		}


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(TemporaryInventory)) return false;
			return Equals((TemporaryInventory)obj);
		}

		public bool Equals(TemporaryInventory other)
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
