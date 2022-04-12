using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;

namespace Count4U.Model.Main
{
	public class ImportAdapter
	{
		public long ID { get; set; }
		public string Code { get; set; }
		public string AdapterCode { get; set; }
		public string DomainType { get; set; }
		public string Description { get; set; }

		//Add 
		//Tag
		//Tag1
		//Tag2
		//Tag3

		//public bool Equals(ImportAdapter other)
		//{
		//    if (ReferenceEquals(null, other)) return false;
		//    if (ReferenceEquals(this, other)) return true;
		//    return Equals(other.AdapterCode, this.AdapterCode);
		//}

		//public override int GetHashCode()
		//{
		//    return (this.AdapterCode != null ? this.AdapterCode.GetHashCode() : 0);
		//}
	}
}

