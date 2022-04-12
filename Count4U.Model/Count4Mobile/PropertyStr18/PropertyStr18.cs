using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr18
    {
        public string Uid { get; set; }
        public string PropStr18Code { get; set; }
        public string PropStr18Name { get; set; }


		public PropertyStr18()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr18Code = "";
			this.PropStr18Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr18)) return false;
			return Equals((PropertyStr18)obj);
		}

		public bool Equals(PropertyStr18 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
