using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface;

namespace Count4U.Model.Audit
{
    public class StatusAuditConfig : IStatusAuditConfig
	{
        public long ID { get; set; }
		public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public int Bit { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(StatusAuditConfig)) return false;
			return Equals((StatusAuditConfig)obj);
		}

		public bool Equals(StatusAuditConfig other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Code, this.Code);
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);
		}

		public StatusAuditConfig Clone()
		{
			return new StatusAuditConfig()
			{
				//ID = this.ID,
				//Code = this.Code,
				Description = this.Description,
				Name = this.Name
			};
		}

	
	}
}
