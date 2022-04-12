using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class Family
	{
		public long ID { get; set; }
		public string FamilyCode { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string Size { get; set; }
		public string Extra1 { get; set; }
		public string Extra2 { get; set; }

		public Family()
		{
			this.FamilyCode = DomainUnknownCode.UnknownFamily;
			this.Name = DomainUnknownName.UnknownFamily; 
			this.Description = "";
			this.Type = "";
			this.Size = "";
			this.Extra1 = "";
			this.Extra2 = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Family)) return false;
			return Equals((Family)obj);
		}

		public bool Equals(Family other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.FamilyCode, this.FamilyCode);
		}

		public override int GetHashCode()
		{
			return (FamilyCode != null ? FamilyCode.GetHashCode() : 0);
		}
	}

}
