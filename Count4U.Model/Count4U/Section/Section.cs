using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	[Serializable]
    public class Section 
	{
        public long ID { get; set; }
		public string SectionCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public string ParentSectionCode { get; set; }
		public string Tag { get; set; }
		public string TypeCode { get; set; }


		public Section()
		{
			SectionCode = DomainUnknownCode.UnknownSection ;
			Name = DomainUnknownName.UnknownSection; 
			Description = "";
			ParentSectionCode = "";
			Tag = "";
			TypeCode = TypeSectionEnum.S.ToString();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Section)) return false;
			return Equals((Section)obj);
		}

		public bool Equals(Section other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.SectionCode, this.SectionCode);
		}

		public override int GetHashCode()
		{
			return (SectionCode != null ? SectionCode.GetHashCode() : 0);
		}
	}
}
