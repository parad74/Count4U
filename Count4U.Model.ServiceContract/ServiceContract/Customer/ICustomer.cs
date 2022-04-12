using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface ICustomer
	{
		[Key]
		long ID { get; set; }
		[DataMember(Name = "Name", Order = 1, IsRequired = true)]
		string Name { get; set; }
		[DataMember(Name = "Description", Order = 2, IsRequired = true)]
		string Description { get; set; }
		[DataMember(Name = "Code", Order = 3, IsRequired = true)]
		string Code { get; set; }
		[DataMember(Name = "DBPath", Order = 4, IsRequired = true)]
		string DBPath { get; set; }
	}


}
