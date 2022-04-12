using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class Barcode : IBarcode
	{
        public long ID { get; set; }
        public long? ProductID { get; set; }

        public string Value { get; set; }
		public string ProductName { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Barcode)) return false;
			return Equals((Barcode)obj);
		}

		public bool Equals(Barcode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Value, this.Value);
		}

		public override int GetHashCode()
		{
			return (this.Value != null ? this.Value.GetHashCode() : 0);
		}

        public Barcode Clone()
        {
            return new Barcode()
            {
				//ID = this.ID,
                ProductID = this.ProductID,
				ProductName = this.ProductName,
                Value = this.Value
            };
        }
	}
}
