using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.ProcessC4U
{
	public class Port
	{
		public long ID { get; set; }
		public string PortCode { get; set; }
		public string Code { get; set; }
		public string IP { get; set; }
		public string Address { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string StatusCode { get; set; }
		public string Tag { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public string Tag3 { get; set; }


		public Port()
		{
			PortCode = "";
			Code = "";
			IP = "";
			Address = "";
			Name = "";
			Description = "";
			StatusCode = "";
			Tag = "";
			Tag1 = "";
			Tag2 = "";
			Tag3 = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Port)) return false;
			return Equals((Port)obj);
		}

		public bool Equals(Port other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.PortCode, this.PortCode);
		}

		public override int GetHashCode()
		{
			return (PortCode != null ? PortCode.GetHashCode() : 0);
		}
	}

}
