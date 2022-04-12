using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class FamilyString
	{
		public long ID { get; set; }
		public string FamilyCode { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string Size { get; set; }
		public string Extra1 { get; set; }
		public string Extra2 { get; set; }


		public FamilyString()
		{
			FamilyCode = "";
			Name = "";
			Description = "";
			Type = "";
			Size = "";
			Extra1 = "";
			Extra2 = "";

		}
	}
}
