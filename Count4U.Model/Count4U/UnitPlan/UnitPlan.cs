using System;

namespace Count4U.Model.Count4U
{
	[Serializable]
	public class UnitPlan
	{
		public long ID { get; set; }
		public string UnitPlanCode { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string LayerCode { get; set; }
		public string ObjectCode { get; set; }
		public bool? Container { get; set; }
		public bool? Visible { get; set; }
		public bool? Lock { get; set; }
		public string ParentCode { get; set; }
		public double StartX { get; set; }
		public double StartY { get; set; }
		public double Height { get; set; }
		public double Width { get; set; }
		public int Zoom { get; set; }
		public int Rotate { get; set; }
		public int ZIndex { get; set; }
		public string Tag { get; set; }
		public int StatusUnitPlanBit { get; set; }  // пока использую как сиквенс	   ?
		public int StatusGroupUnitPlanBit { get; set; }
		public string Picture { get; set; }
		public string LocationCode { get; set; }
		public int FontSize { get; set; }
		public string Color { get; set; }
		public string Font { get; set; }
		public string Value { get; set; }
		public string Tooltip { get; set; }
		public string Title { get; set; }

		// need Add  int UPNum

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

		public UnitPlan()
		{
			UnitPlanCode = "";
			Name = "";
			Description = "";
			LayerCode = "";
			ObjectCode = "";
			Container = false;
			Lock = false;
			Visible = true;
			ParentCode = "";
			StartX = 0;
			StartY = 0;
			Height = 0;
			Width = 0;
			Zoom = 100;
			Rotate = 0;
			ZIndex = 0;
			Tag = "";
			StatusUnitPlanBit = 0;
			StatusGroupUnitPlanBit = 0;
			Picture = "";
			LocationCode = "";
			FontSize = 8;
			Color = "";
			Font = "";
			Value = "";
			Tooltip = "";
			Title = "";
		}
	}
}
