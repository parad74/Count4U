using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface IItur
	{
		[Key]
		 long ID { get; set; }
		[DataMember(Name = "IturCode", Order = 1, IsRequired = true)]
		string IturCode { get; set; }
		[DataMember(Name = "ERPIturCode", Order = 2, IsRequired = true)]
		string ERPIturCode { get; set; }
		[DataMember(Name = "Description", Order = 3, IsRequired = true)]
		string Description { get; set; }
		[DataMember(Name = "LocationCode", Order = 4, IsRequired = true)]
		string LocationCode { get; set; }
		[DataMember(Name = "Name", Order = 5, IsRequired = true)]
		string Name { get; set; }
		[DataMember(Name = "Number", Order = 6, IsRequired = true)]
		int Number { get; set; }
		[DataMember(Name = "NumberPrefix", Order = 7, IsRequired = true)]
		string NumberPrefix { get; set; }
		[DataMember(Name = "NumberSufix", Order = 8, IsRequired = true)]
		string NumberSufix { get; set; }
		[DataMember(Name = "Level1", Order = 9, IsRequired = true)]
		string Level1 { get; set; }
		[DataMember(Name = "Level2", Order = 10, IsRequired = true)]
		string Level2 { get; set; }
		[DataMember(Name = "Level3", Order = 11, IsRequired = true)]
		string Level3 { get; set; }
		[DataMember(Name = "Level4", Order = 12, IsRequired = true)]
		string Level4 { get; set; }
		[DataMember(Name = "Name1", Order = 13, IsRequired = true)]
		string Name1 { get; set; }
		[DataMember(Name = "Name2", Order = 14, IsRequired = true)]
		string Name2 { get; set; }
		[DataMember(Name = "Name3", Order = 15, IsRequired = true)]
		string Name3 { get; set; }
		[DataMember(Name = "Name4", Order = 16, IsRequired = true)]
		string Name4 { get; set; }
		[DataMember(Name = "NodeType", Order = 17, IsRequired = true)]
		int NodeType { get; set; }
		[DataMember(Name = "LevelNum", Order = 18, IsRequired = true)]
		int LevelNum { get; set; }
		[DataMember(Name = "Total", Order = 19, IsRequired = true)]
		int Total { get; set; }
		[DataMember(Name = "Tag", Order = 20, IsRequired = true)]
		string Tag { get; set; }
		[DataMember(Name = "InvStatus", Order = 21, IsRequired = true)]
		int InvStatus { get; set; }
		[DataMember(Name = "ParentIturCode", Order = 22, IsRequired = true)]
		string ParentIturCode { get; set; }
		[DataMember(Name = "TypeCode", Order = 23, IsRequired = true)]
		string TypeCode { get; set; }

	}
}
