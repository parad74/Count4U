using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr6
    {
        public string Uid { get; set; }
        public string PropStr6Code { get; set; }
        public string PropStr6Name { get; set; }


		public PropertyStr6()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr6Code = "";
			this.PropStr6Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr6)) return false;
			return Equals((PropertyStr6)obj);
		}

		public bool Equals(PropertyStr6 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
