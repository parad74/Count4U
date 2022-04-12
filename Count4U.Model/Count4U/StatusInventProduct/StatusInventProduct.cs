using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class StatusInventProduct : IStatusInventProduct
	{
        public long ID { get; set; }
		public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public int Bit { get; set; }

		public StatusInventProduct()
		{
			Code = "";
			Name = "";
			Description = "";
			Bit = 0;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(StatusInventProduct)) return false;
			return Equals((StatusInventProduct)obj);
		}

		public bool Equals(StatusInventProduct other)
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
