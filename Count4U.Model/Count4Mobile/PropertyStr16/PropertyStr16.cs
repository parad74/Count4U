using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr16
    {
        public string Uid { get; set; }
        public string PropStr16Code { get; set; }
        public string PropStr16Name { get; set; }


		public PropertyStr16()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr16Code = "";
			this.PropStr16Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr16)) return false;
			return Equals((PropertyStr16)obj);
		}

		public bool Equals(PropertyStr16 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
