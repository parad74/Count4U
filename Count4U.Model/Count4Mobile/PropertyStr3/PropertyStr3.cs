using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr3
    {
        public string Uid { get; set; }
        public string PropStr3Code { get; set; }
        public string PropStr3Name { get; set; }


		public PropertyStr3()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr3Code = "";
			this.PropStr3Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr3)) return false;
			return Equals((PropertyStr3)obj);
		}

		public bool Equals(PropertyStr3 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
