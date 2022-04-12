using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class Shelf
	{
		public long ID { get; set; }
		public string ShelfCode { get; set; }
		public string ShelfPartCode { get; set; }
		public string IturCode { get; set; }
		public string SupplierCode { get; set; }
		public string SupplierName { get; set; }
		public DateTime CreateDataTime { get; set; }
		public int ShelfNum { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public double Area { get; set; }

		public Shelf()
		{
			ShelfCode = "";
			ShelfPartCode = "";
			IturCode = "";
			SupplierCode = DomainUnknownCode.UnknownSupplier;
			SupplierName = "";
			CreateDataTime = DateTime.Now;
			ShelfNum = 0;
			Width = 0;
			Height = 0;
			Area = 0;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Shelf)) return false;
			return Equals((Shelf)obj);
		}

		public bool Equals(Shelf other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ShelfPartCode, this.ShelfPartCode);
		}

		public override int GetHashCode()
		{
			return (ShelfPartCode != null ? ShelfPartCode.GetHashCode() : 0);
		}
	}

}
