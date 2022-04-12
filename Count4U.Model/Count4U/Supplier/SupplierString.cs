using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class SupplierString
	{
		public long ID { get; set; }
		public string SupplierCode { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }


		public SupplierString()
		{
			SupplierCode = "";
			Name = "";
			Description = "";
		}
	}
}
