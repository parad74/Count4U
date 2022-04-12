using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr4
    {
        public string Uid { get; set; }
        public string PropStr4Code { get; set; }
        public string PropStr4Name { get; set; }


		public PropertyStr4()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.PropStr4Code = "";
			this.PropStr4Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr4)) return false;
			return Equals((PropertyStr4)obj);
		}

		public bool Equals(PropertyStr4 other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }
}
