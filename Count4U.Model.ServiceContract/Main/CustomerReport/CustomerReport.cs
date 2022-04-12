using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.ServiceContract;

namespace Count4U.Model.Main
{
    public class CustomerReport : ICustomerReport
    {
		public long ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ReportCode { get; set; }
		public string CustomerCode { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(CustomerReport)) return false;
			return Equals((CustomerReport)obj);
		}

		public bool Equals(CustomerReport other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ReportCode, this.ReportCode);
		}

		public override int GetHashCode()
		{
			return (this.ReportCode.GetHashCode());
		}

		public CustomerReport Clone()
		{
			return new CustomerReport()
			{
				//ID = this.ID,
				Description = this.Description,
				Name = this.Name
			};
		}
    }
}
