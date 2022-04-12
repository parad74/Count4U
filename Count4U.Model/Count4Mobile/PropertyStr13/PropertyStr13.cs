using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr13
    {
        public string Uid { get; set; }
        public string PropStr13Code { get; set; }
        public string PropStr13Name { get; set; }


		public PropertyStr13()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr13Code = "";
			this.PropStr13Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr13)) return false;
			return Equals((PropertyStr13)obj);
		}

		public bool Equals(PropertyStr13 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
