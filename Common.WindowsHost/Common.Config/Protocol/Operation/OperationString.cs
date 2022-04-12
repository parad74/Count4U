using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdSip2.Common.Protocol
{

	public class OperationString
	{
		public string code { get; set; }
		public string name { get; set; }
		public string type { get; set; }
		public string back { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(OperationString)) return false;
			return Equals((OperationString)obj);
		}

		public bool Equals(OperationString other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.code, this.code);
		}

		public override int GetHashCode()
		{
			return (this.code != null ? this.code.GetHashCode() : 0);
		}

		public OperationString()
		{
			this.code = "";
			this.name = "";
			this.type = "";
			this.back = "";
		}

	}
}

