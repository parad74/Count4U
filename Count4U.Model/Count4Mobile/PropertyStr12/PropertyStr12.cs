using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr12
    {
        public string Uid { get; set; }
        public string PropStr12Code { get; set; }
        public string PropStr12Name { get; set; }


		public PropertyStr12()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr12Code = "";
			this.PropStr12Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr12)) return false;
			return Equals((PropertyStr12)obj);
		}

		public bool Equals(PropertyStr12 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
