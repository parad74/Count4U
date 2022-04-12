using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr19
    {
        public string Uid { get; set; }
        public string PropStr19Code { get; set; }
        public string PropStr19Name { get; set; }


		public PropertyStr19()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr19Code = "";
			this.PropStr19Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr19)) return false;
			return Equals((PropertyStr19)obj);
		}

		public bool Equals(PropertyStr19 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
