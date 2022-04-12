using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr2
    {
        public string Uid { get; set; }
        public string PropStr2Code { get; set; }
        public string PropStr2Name { get; set; }


		public PropertyStr2()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr2Code = "";
			this.PropStr2Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr2)) return false;
			return Equals((PropertyStr2)obj);
		}

		public bool Equals(PropertyStr2 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
