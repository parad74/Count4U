using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс объекта Barcode, предназначенного идентификации продуктов по штрихкоду
    /// </summary>
    public interface IBarcode
	{
        /// <summary>
        /// ID
        /// </summary>
		long ID { get; set; }

        /// <summary>
        /// ProductID - продукт
        /// </summary>
		long? ProductID { get; set; }

        /// <summary>
        /// Значение баркода
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Название продукта
        /// </summary>
        string ProductName { get; set; }
	}
}
