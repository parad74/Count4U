using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr9
    {
        public string Uid { get; set; }
        public string PropStr9Code { get; set; }
        public string PropStr9Name { get; set; }


		public PropertyStr9()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr9Code = "";
			this.PropStr9Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr9)) return false;
			return Equals((PropertyStr9)obj);
		}

		public bool Equals(PropertyStr9 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
