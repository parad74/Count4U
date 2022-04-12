using System;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Interface.Count4U;
using Count4U.Model;

namespace Count4U.Model.Count4U
{
    [Serializable]
    public class Location //: ILocation
	{
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BackgroundColor { get; set; }
		public bool? RestoreBit { get; set; }
		public string Restore { get; set; }

		public string ParentLocationCode { get; set; }
		public string TypeCode { get; set; }
		public string Level1 { get; set; }
		public string Level2 { get; set; }
		public string Level3 { get; set; }
		public string Level4 { get; set; }
		public string Name1 { get; set; }
		public string Name2 { get; set; }
		public string Name3 { get; set; }
		public string Name4 { get; set; }
		public int NodeType { get; set; }
		public int LevelNum { get; set; }
		public int Total { get; set; }
		public DateTime DateModified { get; set; }
		public string Tag { get; set; }

		//TO ADD in COUNT4U
		public int InvStatus { get; set; }
		public bool Disabled { get; set; }

		public Location()
		{
			Code = DomainUnknownCode.UnknownLocation;
			Name = DomainUnknownName.UnknownLocation;
			Description = "";
			BackgroundColor = "";
			Restore = "";
			ParentLocationCode = "";
			TypeCode = "";
			Level1 = "";
			Level2 = "";
			Level3 = "";
			Level4 = "";
			Name1 = "";
			Name2 = "";
			Name3 = "";
			Name4 = "";
			NodeType = 1;
			LevelNum = 1;
			Total = 0;
			Tag = "";
			InvStatus = 0;
			Disabled = false;
			DateModified = ModelUtils.GetMinDateTime();
			
		}

	
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Location)) return false;
			return Equals((Location)obj);
		}

		public bool Equals(Location other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Code, this.Code);
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}

		public Location Clone()
		{
			return new Location()
			{
				//ID = this.ID,
				BackgroundColor = this.BackgroundColor,
				//Code = this.Code,
				Description = this.Description,
				Name = this.Name,
				ParentLocationCode = this.Name,
				TypeCode = this.TypeCode,
				Level1 = this.Level1,
				Level2 = this.Level2,
				Level3 = this.Level3,
				Level4 = this.Level4,
				Name1 = this.Name1,
				Name2 = this.Name2,
				Name3 = this.Name3,
				Name4 = this.Name4,
				NodeType = this.NodeType,
				LevelNum = this.LevelNum,
				Total = this.Total,
				InvStatus =this.InvStatus,
				Disabled = this.Disabled,
				Tag = this.Tag,
				DateModified = ModelUtils.GetMinDateTime()
				


			};

			
		}
	}
}
