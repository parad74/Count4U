using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface ICustomerReport
	{
		[Key]
		long ID { get; set; }
		[DataMember(Name = "Name", Order = 1, IsRequired = true)]
		string Name { get; set; }
		[DataMember(Name = "Description", Order = 2, IsRequired = true)]
		string Description { get; set; }
		[DataMember(Name = "ReportCode", Order = 3, IsRequired = true)]
		string ReportCode { get; set; }
		[DataMember(Name = "CustomerCode", Order = 4, IsRequired = true)]
		string CustomerCode { get; set; }
	}
}
