using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class Session 
    {
        public long ID { get; set; }
        public string PDAID { get; set; }
        public string SessionCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PDADate { get; set; }
        public string WorkerGUID { get; set; }
		public int CountItem { get; set; }
		public int CountDocument { get; set; }
		public int CountItur { get; set; }
		public double SumQuantityEdit { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Session)) return false;
			return Equals((Session)obj);
		}

		public bool Equals(Session other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.SessionCode, this.SessionCode);						  //TODO: 
		}

		public override int GetHashCode()
		{
			return (SessionCode != null ? SessionCode.GetHashCode() : 0);
		}
	}
}
