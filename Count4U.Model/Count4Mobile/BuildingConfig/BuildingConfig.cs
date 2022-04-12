using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
    public class BuildingConfig
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public int? Ord { get; set; }
		public string NameEn { get; set; }
		public string NameHe { get; set; }


		public BuildingConfig()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.Name = "";
			this.Ord = 0;
			this.NameEn = "";
			this.NameHe = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(BuildingConfig)) return false;
			return Equals((BuildingConfig)obj);
		}

		public bool Equals(BuildingConfig other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}
    }

	
}
