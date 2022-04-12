using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
	/// <summary>
	/// Интерфейс для храннения текущего состояния аудита
	/// </summary>
	public interface IInventorConfig 
	{
		long ID { get; set; }
		string Code { get; set; }
		string BranchCode { get; set; }
		string BranchName { get; set; }
		string CustomerCode { get; set; }
		string CustomerName { get; set; }
		string InventorName { get; set; }
		string Description { get; set; }
		DateTime CreateDate { get; set; }
		DateTime InventorDate { get; set; }
		DateTime? ModifyDate { get; set; }
		bool? IsDirty { get; set; }
		string StatusInventorConfigCode { get; set; }
		string DBPath { get; set; }
	}
}
