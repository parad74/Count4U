using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr1
    {
        public string Uid { get; set; }
        public string PropStr1Code { get; set; }
        public string PropStr1Name { get; set; }

		public PropertyStr1()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr1Code = "";
			this.PropStr1Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr1)) return false;
			return Equals((PropertyStr1)obj);
		}

		public bool Equals(PropertyStr1 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}

		public override int GetHashCode()
		{
			return (Uid != null ? Uid.GetHashCode() : 0);
		}
    }
}
