using System;
//using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class InventorConfig// : IInventorConfig
	{
        public long ID { get; set; }
        public string Code { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
		public string InventorCode { get; set; }
		public string InventorName { get; set; }
		public string Description { get; set; }
        public DateTime CreateDate { get; set; }
		public DateTime InventorDate { get; set; }
		public DateTime? ModifyDate { get; set; }
		public bool? IsDirty { get; set; }
	   	public string StatusInventorConfigCode { get; set; }
		public string DBPath { get; set; }
		public string TypeObject { get; set; }
		

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
			return Equals(other.TypeObject, this.TypeObject);
		}

		public override int GetHashCode()
		{
			return (TypeObject != null ? TypeObject.GetHashCode() : 0);
		}

		public InventorConfig Clone()
		{
			return new InventorConfig()
			{
				//ID = this.ID,
				//Code = this.Code,
				//InventorDate = this.InventorDate,
				CreateDate = DateTime.Now,
				Description = this.Description,
				CustomerCode = this.CustomerCode,
				CustomerName = this.CustomerName,
				BranchCode = this.BranchCode,
				BranchName = this.BranchName,
				//IsDirty = this.IsDirty,
				//ModifyDate = this.ModifyDate,
				//StatusInventorConfigID = this.StatusInventorConfigID,
				//StatusInventorConfigCode = this.StatusInventorConfigCode,
				//DBPath = this.DBPath

			};
		}

	}
}
