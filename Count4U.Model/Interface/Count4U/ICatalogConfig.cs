using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
	/// <summary>
	/// Интерфейс для храннения текущего состояния аудита
	/// </summary>
	public interface ICatalogConfig 
	{
		long ID { get; set; }
		string InventorCode { get; set; }
		string BranchCode { get; set; }
		string CustomerCode { get; set; }
		string Description { get; set; }
		DateTime CreateDate { get; set; }
		DateTime? ModifyDate { get; set; }
		string Tag { get; set; }
	}
}
