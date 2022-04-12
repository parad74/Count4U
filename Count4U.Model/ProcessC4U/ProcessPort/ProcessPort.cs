using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.ProcessC4U
{
	public class ProcessPort
	{
		public long ID { get; set; }
		public string ProcessPortCode { get; set; }
		public string ProcessCode { get; set; }
		public string PortCode { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string StatusCode { get; set; }
		public string Tag { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public string Tag3 { get; set; }


		public ProcessPort()
		{
			ProcessPortCode = "";
			ProcessCode = "";
			PortCode = "";
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
			if (obj.GetType() != typeof(ProcessPort)) return false;
			return Equals((ProcessPort)obj);
		}

		public bool Equals(ProcessPort other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ProcessPortCode, this.ProcessPortCode);
		}

		public override int GetHashCode()
		{
			return (ProcessPortCode != null ? ProcessPortCode.GetHashCode() : 0);
		}
	}

}
