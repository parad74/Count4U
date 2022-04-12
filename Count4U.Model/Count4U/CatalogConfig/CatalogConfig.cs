using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class CatalogConfig : ICatalogConfig
	{
        public long ID { get; set; }
        public string InventorCode { get; set; }
        public string BranchCode { get; set; }
        public string CustomerCode { get; set; }
 		public string Description { get; set; }
        public DateTime CreateDate { get; set; }
		public DateTime? ModifyDate { get; set; }
		public string Tag { get; set; }


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(InventorConfig)) return false;
			return Equals((InventorConfig)obj);
		}

		public bool Equals(InventorConfig other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ID, this.ID);
		}

		public override int GetHashCode()
		{
			return (this.ID.GetHashCode());
		}

	}
}
