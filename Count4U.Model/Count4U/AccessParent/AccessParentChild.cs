using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class AccessParentChild// : IAccessParentChild
	{
        public long ID { get; set; }
        public long? ParentProductID { get; set; }
        public long? ProductID { get; set; }
        public long? UnitTypeID { get; set; }

        public string InputType { get; set; }
        public string ParentProduct { get; set; }
        public string Product { get; set; }
        public double Unit { get; set; }
        public string UnitType { get; set; }
		public string InputTypeCode { get; set; }
		public string UnitTypeCode { get; set; }

		/*	public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof(AccessParentChild)) return false;
				return Equals((AccessParentChild)obj);
			}

			public bool Equals(AccessParentChild other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Equals(other.ID, this.ID);
			}

			public override int GetHashCode()
			{
				return (ID != null ? ID.GetHashCode() : 0);
			}
			*/

	}
}
