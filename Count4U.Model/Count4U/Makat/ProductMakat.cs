using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public class ProductMakat : IEquatable<ProductMakat>
	{
		//public long ID { get; set; }
		//public string Code { get; set; }
		//public string Barcode { get; set; }
		public string Makat { get; set; }
		public string MakatOriginal { get; set; }
		//public string MakatERP { get; set; }
		//public int TypeMakatCode { get; set; }
		public string Name { get; set; }
		//public string Description { get; set; }

		public double? BalanceQuantityERP { get; set; }
		public bool? IsUpdateERP { get; set; }
		//public double? BalanceQuantityPartialERP { get; set; }
		//public string Tag { get; set; }
		//public long? CountMax { get; set; }
		//public long? CountMin { get; set; }
		public string FamilyCode { get; set; }
		//public long? Importance { get; set; }

		//public double? PriceBuy { get; set; }
		//public double? PriceExtra { get; set; }
		//public double PriceSale { get; set; }
		//public string PriceString { get; set; }

		//public string Section { get; set; }
		//public string Supplier { get; set; }
		public string SectionCode { get; set; }
		public string SubSectionCode { get; set; }
		public string SupplierCode { get; set; }
		public string TypeCode { get; set; }
		public string UnitTypeCode { get; set; }
		//public string InputTypeCode { get; set; }
		//public DateTime CreateDate { get; set; }
		//public DateTime? ModifyDate { get; set; }
		//public string ParentCode { get; set; }
		//public string ParentBarcode { get; set; }
		public string ParentMakat { get; set; }
		//public double? CountInParentPack { get; set; }
		//public string ParserBag { get; set; }


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ProductMakat)) return false;
			return Equals((ProductMakat)obj);
		}

		public bool Equals(ProductMakat other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Makat, this.Makat);
		}

		public override int GetHashCode()
		{
			return (Makat != null ? Makat.GetHashCode() : 0);
		}
	}


}
