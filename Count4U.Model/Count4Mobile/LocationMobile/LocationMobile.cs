using System;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class LocationMobile
    {
        public string Uid { get; set; }
        public string LocationCode { get; set; }
		public string IturCode { get; set; }
        public string Description { get; set; }
        public string Level1Code { get; set; }
        public string Level1Name { get; set; }
        public string Level2Code { get; set; }
        public string Level2Name { get; set; }
        public string Level3Code { get; set; }
        public string Level3Name { get; set; }
        public string Level4Code { get; set; }
        public string Level4Name { get; set; }
        public string InvStatus { get; set; }
        public string NodeType { get; set; }
        public string LevelNum { get; set; }
        public string Total { get; set; }
        public string DateModified { get; set; }

		public LocationMobile()
		{
			this.Uid = Guid.NewGuid().ToString();

			this.LocationCode = "";
			this.IturCode = "";
			this.Description = "";
			this.Level1Code = "";
			this.Level1Name = "";
			this.Level2Code = "";
			this.Level2Name = "";
			this.Level3Code = "";
			this.Level3Name = "";
			this.Level4Code = "";
			this.Level4Name = "";
			this.InvStatus = "0";
			this.NodeType = "";
			this.LevelNum = "1";
			this.Total = "0";
			this.DateModified = "";
		}


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(LocationMobile)) return false;
			return Equals((LocationMobile)obj);
		}

		public bool Equals(LocationMobile other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}

    }
}
