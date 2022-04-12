using System;
using Count4U.Model.Audit;

namespace Count4U.Model.Interface.Audit
{
	/// <summary>
	/// Инвентаризация - аудит 
	/// </summary>
    public interface IInventor
	{
        /// <summary>
        /// ID
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// Код клиента
        /// </summary>
        string CustomerCode { get; set; }

        /// <summary>
        /// Код магазина
        /// </summary>
        string BranchCode { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Дата создания аудита
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        /// Дата проведения аудита
        /// </summary>
        DateTime InventorDate { get; set; }

        /// <summary>
        /// Сохранен ли в общем хранилище
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Фиктивный код
        /// </summary>
        string DirtyCode { get; set; }

		 /// <summary>
		 /// Статус инвентризации
		 /// </summary>
		string Status { get; set; }
	   
		/// <summary>
		/// Статус инвентаризации
		/// </summary>
		string StatusCode { get; set; }

		/// <summary>
		/// Каталог БД
		/// </summary>
		string DBPath { get; set; }
	}
}
