using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	[Serializable]
	public class ResulteValue 
	{
        public long ID { get; set; }
		public string Code { get; set; }
        public string Name { get; set; }
		public string ValueTypeCode { get; set; }
		public string ColGroupCode { get; set; }
		public string RowGroupCode { get; set; }
		public int ColIndex { get; set; }
		public int RowIndex { get; set; }
		public string ColCode { get; set; }
		public string RowCode { get; set; }
		public string Value { get; set; }
		public int ValueInt { get; set; }
		public string ValueStr { get; set; }
		public double ValueFloat { get; set; }
		public bool ValueBit { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ResulteValue)) return false;
			return Equals((ResulteValue)obj);
		}

		public bool Equals(ResulteValue other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Code, this.Code);
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}
	}
}
