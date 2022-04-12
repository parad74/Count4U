using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr5
    {
        public string Uid { get; set; }
        public string PropStr5Code { get; set; }
        public string PropStr5Name { get; set; }


		public PropertyStr5()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr5Code = "";
			this.PropStr5Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr5)) return false;
			return Equals((PropertyStr5)obj);
		}

		public bool Equals(PropertyStr5 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
