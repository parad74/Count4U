using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr17
    {
        public string Uid { get; set; }
        public string PropStr17Code { get; set; }
        public string PropStr17Name { get; set; }


		public PropertyStr17()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr17Code = "";
			this.PropStr17Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr17)) return false;
			return Equals((PropertyStr17)obj);
		}

		public bool Equals(PropertyStr17 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
