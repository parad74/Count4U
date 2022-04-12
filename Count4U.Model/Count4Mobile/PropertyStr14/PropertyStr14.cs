using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr14
    {
        public string Uid { get; set; }
        public string PropStr14Code { get; set; }
        public string PropStr14Name { get; set; }

		
		public PropertyStr14()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr14Code = "";
			this.PropStr14Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr14)) return false;
			return Equals((PropertyStr14)obj);
		}

		public bool Equals(PropertyStr14 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
