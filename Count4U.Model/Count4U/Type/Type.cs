using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class Type : IType
	{
        public long ID { get; set; }
		public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Type)) return false;
			return Equals((Type)obj);
		}

		public bool Equals(Type other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Code, this.Code);
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}
	}
}
