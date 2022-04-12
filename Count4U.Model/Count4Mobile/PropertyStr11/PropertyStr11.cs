using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr11
    {
        public string Uid { get; set; }
        public string PropStr11Code { get; set; }
        public string PropStr11Name { get; set; }


		public PropertyStr11()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr11Code = "";
			this.PropStr11Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr11)) return false;
			return Equals((PropertyStr11)obj);
		}

		public bool Equals(PropertyStr11 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
