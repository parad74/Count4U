using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ
    public class UploadUnit 
	{
		public string UploadUnitCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(UploadUnit)) return false;
			return Equals((UploadUnit)obj);
		}

		public bool Equals(UploadUnit other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.UploadUnitCode, this.UploadUnitCode);
		}

		public override int GetHashCode()
		{
			return (UploadUnitCode != null ? UploadUnitCode.GetHashCode() : 0);
		}
	}

}
