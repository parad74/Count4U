using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr20
    {
        public string Uid { get; set; }
        public string PropStr20Code { get; set; }
        public string PropStr20Name { get; set; }


		public PropertyStr20()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr20Code = "";
			this.PropStr20Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr20)) return false;
			return Equals((PropertyStr20)obj);
		}

		public bool Equals(PropertyStr20 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
