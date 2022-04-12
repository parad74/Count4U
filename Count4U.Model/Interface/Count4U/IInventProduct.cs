using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
	/// <summary>
	/// Интерфейс данных инвентаризации - одно снятое значение инвентаризации
	/// </summary>
    public interface IInventProduct
	{
        /// <summary>
        /// ID
        /// </summary>
        long ID { get; set; }

        /// <summary>
		/// DocumentHeaderID - получаем от масофона
        /// </summary>
        long? DocumentHeaderID { get; set; }

        /// <summary>
		/// InputTypeID - способ ввода  данных автоматический/вручную 
		/// (получаем от масофона)  
        /// </summary>
		long? InputTypeID { get; set; }

        /// <summary>
        /// StatusInventProductID
        /// </summary>
        long? StatusInventProductID { get; set; }

		/// <summary>
		///  Баркод  - получаем от масофона
		/// </summary>
		string Barcode { get; set; }

		/// <summary>
		///  Дата и время считывания данных  (получаем от масофона)  
		/// </summary>
		DateTime CreateDate { get; set; }

		/// <summary>
		/// DocumentHeader,  получаем от масофона
		/// </summary>
		string DocumentHeader { get; set; }

		/// <summary>
		/// Способ ввода элемент инвентаризации
		/// </summary>
        string InputType { get; set; }

		/// <summary>
		///  On/off   бит  или количество было полное  или поштучное 
		/// </summary>
		bool? PartialPackage { get; set; }

		/// <summary>
		///  Разность между QuantityOriginal- QuantityEdit (вычисляется автоматически)
		/// </summary>
		string QuantityDifference { get; set; }

		/// <summary>
		///  Количество продукта  исправленное
		/// </summary>
		double? QuantityEdit { get; set; }

		/// <summary>
		/// Количество продукта, фактически при инвентаризации   
		/// </summary>
		double? QuantityOriginal { get; set; }

		/// <summary>
		/// Серийный номер
		/// </summary>
		string SerialNumber { get; set; }

		/// <summary>
		/// Код полки откуда были считаны данные  (получаем от масофона)  
		/// </summary>
		string ShelfCode { get; set; }

		/// <summary>
		/// Статус инвентаризации
		/// </summary>
        string StatusInventProduct { get; set; }

		string StatusInventProductCode { get; set; }

		int StatusInventProductBit { get; set; }
		
	}
}
