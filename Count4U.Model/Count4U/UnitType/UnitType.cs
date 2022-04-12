using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class UnitType 
	{
        public long ID { get; set; }
		public string UnitTypeCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

		public UnitType()
		{
			UnitTypeCode = DomainUnknownCode.UnknownUnitType;
			Name = DomainUnknownName.UnknownUnitType; 
			Description = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(UnitType)) return false;
			return Equals((UnitType)obj);
		}

		public bool Equals(UnitType other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.UnitTypeCode, this.UnitTypeCode);
		}

		public override int GetHashCode()
		{
			return (UnitTypeCode != null ? UnitTypeCode.GetHashCode() : 0);
		}
	}
}
