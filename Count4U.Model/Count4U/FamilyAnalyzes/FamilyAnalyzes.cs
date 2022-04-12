using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class FamilyAnalyzes
	{
		public long ID { get; set; }
		public string FamilyCode { get; set; }
		public string FamilyName { get; set; }
		public string Description { get; set; }
		public string FamilyType { get; set; }
		public string FamilySize { get; set; }
		public string FamilyExtra1 { get; set; }
		public string FamilyExtra2 { get; set; }

		public string Makat { get; set; }
		public string LocationCode { get; set; }
		public string LocationName { get; set; }
		public string IturCode { get; set; }
		public string IturName { get; set; }
		public byte[] IturBarcode { get; set; }
		public byte[] LocationBarcode { get; set; }

		public FamilyAnalyzes()
		{
			FamilyCode = "";
			FamilyName = "";
			Description = "";
			FamilyType = "";
			FamilySize = "";
			FamilyExtra1 = "";
			FamilyExtra2 = "";

			Makat = "";
			LocationCode = "";
			LocationName = "";
			IturCode = "";
			IturName = "";
			IturBarcode = null;
			LocationBarcode = null;

		}


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(FamilyAnalyzes)) return false;
			return Equals((FamilyAnalyzes)obj);
		}

		public bool Equals(FamilyAnalyzes other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.FamilyCode, this.FamilyCode);
		}

		public override int GetHashCode()
		{
			return (FamilyCode != null ? FamilyCode.GetHashCode() : 0);
		}
	}

}
