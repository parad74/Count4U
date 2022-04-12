using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr8
    {
        public string Uid { get; set; }
        public string PropStr8Code { get; set; }
        public string PropStr8Name { get; set; }


		public PropertyStr8()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr8Code = "";
			this.PropStr8Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr8)) return false;
			return Equals((PropertyStr8)obj);
		}

		public bool Equals(PropertyStr8 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
