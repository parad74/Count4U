using System;

namespace Count4U.Model.Count4U
{
	public class LocationString
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string BackgroundColor { get; set; }

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
		public string NodeType { get; set; }
		public string LevelNum { get; set; }
		public string Total { get; set; }
		public string Tag { get; set; }

		public LocationString()
		{
			Code = "";
			Description = "";
			Name = "";
			BackgroundColor = "";
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
			NodeType = "1";
			LevelNum = "1";
			Total = "0";
			Tag = "";
		}
	}
}
