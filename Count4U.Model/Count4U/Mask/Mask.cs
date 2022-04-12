using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
	public class Mask
	{
		public long ID { get; set; }
		public string Code { get; set; }
		public string AdapterCode { get; set; }
		public string FileCode { get; set; }
		public string BarcodeMask { get; set; }
		public string MakatMask { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Mask)) return false;
			return Equals((Mask)obj);
		}

		public bool Equals(Mask other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Code, this.Code) 
				&& Equals(other.AdapterCode, this.AdapterCode) 
				&& Equals(other.FileCode, this.FileCode));
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}

  
	}
}
