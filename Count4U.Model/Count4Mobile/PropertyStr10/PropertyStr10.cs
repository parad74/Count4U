using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr10
    {
        public string Uid { get; set; }
        public string PropStr10Code { get; set; }
        public string PropStr10Name { get; set; }


		public PropertyStr10()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr10Code = "";
			this.PropStr10Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr10)) return false;
			return Equals((PropertyStr10)obj);
		}

		public bool Equals(PropertyStr10 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
