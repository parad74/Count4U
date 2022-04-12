using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.ProcessC4U
{
	public class Process
	{
		public long ID { get; set; }
		public string ProcessCode { get; set; }
		public string Code { get; set; }
		public string SyncCode { get; set; }
		public string DBPath { get; set; }
		public DateTime? CreateDate { get; set; }
		public string Manager { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string StatusCode { get; set; }

		public string Tag { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public string Tag3 { get; set; }


		public Process()
		{
			ProcessCode = "";
			Code = "";
			SyncCode = "";
			DBPath = "";
			Manager = "";
			Name = "";
			Title = "";
			Description = "";
			StatusCode = "";
			Tag = "";
			Tag1 = "";
			Tag2 = "";
			Tag3 = "";
			CreateDate = DateTime.Now;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Process)) return false;
			return Equals((Process)obj);
		}

		public bool Equals(Process other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ProcessCode, this.ProcessCode);
		}

		public override int GetHashCode()
		{
			return (ProcessCode != null ? ProcessCode.GetHashCode() : 0);
		}
	}

}
