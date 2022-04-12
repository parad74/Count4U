using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    public interface IItur
	{
        /// <summary>
        /// ID
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// LocationID - локайшин
        /// </summary>
        long? LocationID { get; set; }

        /// <summary>
        /// StatusIturID  - статус
        /// </summary>
        long? StatusIturID { get; set; }

		/// <summary>
		///  Утверждено - не утверждено
		/// </summary>
		bool? Approve { get; set; }

		/// <summary>
		/// Код
		/// </summary>
		string Code { get; set; }

		/// <summary>
		/// Описание
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Количество различных типов продуктов , которые должны быть в этом итуре
		/// </summary>
		double? InitialQuantityMakatExpected { get; set; }

		/// <summary>
		/// Локейшин
		/// </summary>
		string Location { get; set; }

		/// <summary>
		/// Код локейшин
		/// </summary>
		string LocationCode { get; set; }

		/// <summary>
		/// Отображаемое имя
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Порядковый номер итура, получает его автоматически от системы
		/// </summary>
		int Number { get; set; }

		/// <summary>
		/// Статус 
		/// </summary>
		string StatusItur { get; set; }

		DateTime CreateDate { get; set; }
		DateTime? ModifyDate { get; set; }
		bool? Publishe { get; set; }
		string StatusIturCode { get; set; }
		int StatusIturBit { get; set; }
	}
}
