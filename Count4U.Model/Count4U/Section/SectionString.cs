using System;
namespace Count4U.Model.Count4U
{
	public class SectionString
	{
		public string SectionCode { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ParentSectionCode { get; set; }
		public string Tag { get; set; }
		public string TypeCode { get; set; }


		public SectionString()
		{
			SectionCode = "";
			Name = "";
			Description = "";
			ParentSectionCode = "";
			Tag = "";
			TypeCode = "";
		}

	}
}

	

