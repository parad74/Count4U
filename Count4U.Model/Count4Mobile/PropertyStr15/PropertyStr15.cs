using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr15
    {
        public string Uid { get; set; }
        public string PropStr15Code { get; set; }
        public string PropStr15Name { get; set; }


		public PropertyStr15()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr15Code = "";
			this.PropStr15Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr15)) return false;
			return Equals((PropertyStr15)obj);
		}

		public bool Equals(PropertyStr15 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
