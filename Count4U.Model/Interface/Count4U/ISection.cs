using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс секций Section
    /// </summary>
    public interface ISection 
	{
		/// <summary>
		/// ID - идентификатор
		/// </summary>
		long ID { get; set; }

		/// <summary>
		/// Code - ключ
		/// </summary>
		string SectionCode { get; set; }

		/// <summary>
		/// Краткое название 
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Развернутое описание 
		/// </summary>
		string Description { get; set; }   

	}
}
