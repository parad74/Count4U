using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr7
    {
        public string Uid { get; set; }
        public string PropStr7Code { get; set; }
        public string PropStr7Name { get; set; }


		public PropertyStr7()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr7Code = "";
			this.PropStr7Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr7)) return false;
			return Equals((PropertyStr7)obj);
		}

		public bool Equals(PropertyStr7 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
