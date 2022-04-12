using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
	
	public class MaskTemplate
	{
		public MaskTemplateEnum MaskTemplateType { get; set; }
 		public string Code { get; set; }
		public string Name { get; set; }
		public Func<string, string, string> FormatString; 

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(MaskTemplate)) return false;
			return Equals((MaskTemplate)obj);
		}

		public bool Equals(MaskTemplate other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Code, this.Code) ;
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}

  
	}
}
