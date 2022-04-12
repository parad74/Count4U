using System;

namespace Count4U.Model.Count4U
{
  
	public class UnitPlanString
	{
  //      public long ID { get; set; }
		public string UnitPlanCode { get; set; }
		//public string Name { get; set; }
		public string Description { get; set; }
		public string LayerCode { get; set; }
		public string ObjectCode { get; set; }
		//public string Container { get; set; }
		//public string Visible { get; set; }
		//public string ParentCode { get; set; }
		public string StartX { get; set; }
		public string StartY { get; set; }
		public string Height { get; set; }
		public string Width { get; set; }
		//public string Zoom { get; set; }
		public string Rotate { get; set; }
		//public string Tag { get; set; }
		//public string StatusUnitPlanBit { get; set; }
		//public string StatusGroupUnitPlanBit { get; set; }

	
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(UnitPlan)) return false;
			return Equals((UnitPlan)obj);
		}

		public bool Equals(UnitPlan other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.UnitPlanCode, this.UnitPlanCode);
		}

		public override int GetHashCode()
		{
			return (UnitPlanCode != null ? UnitPlanCode.GetHashCode() : 0);
		}

		public UnitPlanString()
		{
			UnitPlanCode = "";
			//Name = "";
			Description = "";
			LayerCode = "";
			ObjectCode = "";
			//Container = "0";
			//Visible = "1";
			//ParentCode = "";
			StartX = "0";
			StartY = "0";
			Height = "0";
			Width = "0";
			//Zoom = "100";
			Rotate = "0";
			//Tag = "";
			//StatusUnitPlanBit = "0";
			//StatusGroupUnitPlanBit = "0";
		}
	}
}
