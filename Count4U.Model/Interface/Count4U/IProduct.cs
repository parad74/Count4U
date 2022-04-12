using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    public interface IProduct
	{
        /// <summary>
        /// ID
        /// </summary>
		long ID { get; set; }

        /// <summary>
        /// Впособ ввода ID
        /// </summary>
        long? InputTypeID { get; set; }

        /// <summary>
		/// ID кода секции ID
        /// </summary>
        long? SectionID { get; set; }

        /// <summary>
		/// ID кода поставщика
        /// </summary>
        long? SupplierID { get; set; }

        /// <summary>
		/// Тип makata (обычный или имеющий серийный номер)
		/// Пример продуктов  имеющих серийный номер- мобильные телефоны  
        /// </summary>
		long? TypeMakataID { get; set; }

        /// <summary>
        /// Единицы измерения товара ID 
        /// </summary>
        long? UnitTypeID { get; set; }

        /// <summary>
        /// Публикуемое имя
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Маркат 
        /// </summary>
        string Makat { get; set; }

        /// <summary>
		/// Макат в системе ERP клиента
        /// </summary>
        string MakatERP { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Значение баркода
        /// </summary>
        string Barcode { get; set; }

		/// <summary>
		/// Баланс который записан в системе ERP клиента (весь остаток), 
		/// для расчета разницы после проверки остатков в магазине
		/// </summary>
        double? BalanceQuantityERP { get; set; }

		/// <summary>
		/// Частичный баланс, который записан в системе ERP клиента,
		/// для расчета разницы после проверки остатков в магазине . 
		/// Он рассчитывается как  [разница целых остатков]/[] .
		/// </summary>
		double? BalanceQuantityPartialERP { get; set; }

		/// <summary>
		/// Для расширения
		/// </summary>
		string ColumnXML { get; set; }

		/// <summary>
		/// 
		/// </summary>
		long? CountMax { get; set; }

		/// <summary>
		/// 
		/// </summary>
		long? CountMin { get; set; }

		/// <summary>
		/// Специфичные данные, обычно для одежды
		/// Это свойство указывает например на цвет ткани и составляет часть маката
		/// </summary>
		string Family { get; set; }

		/// <summary>
		/// Приоритет (используется для показа очередности вывода продуктов, 
		/// по умолчанию  подукты выводятся  по имени А..Я
		/// </summary>
		long? Importance { get; set; }

		/// <summary>
		/// Способ получения информации 
		/// 1.Через каталог клиента	 
		/// 2.Вручную , если продукт отсутствует в каталоге, но есть в магазине	
		/// 3.После получения информации от мобильных устройств считывания
		/// информации (масафон)
		/// </summary>
        string InputType { get; set; }

		/// <summary>
		/// Цена покупки
		/// </summary>
		double? PriceBuy { get; set; }

		/// <summary>
		/// Дополнительная цена
		/// </summary>
		double? PriceExtra { get; set; }

		/// <summary>
		/// Цена продажи
		/// </summary>
		double? PriceSale { get; set; }

		/// <summary>
		/// 
		/// </summary>
		string PriceString { get; set; }

		/// <summary>
		/// Код секции
		/// </summary>
        string Section { get; set; }

		/// <summary>
		/// Код поставщика
		/// </summary>
        string Supplier { get; set; }

		/// <summary>
		/// Тип makata (обычный или имеющий серийный номер)
		/// Пример продуктов  имеющих серийный номер- мобильные телефоны  
		/// </summary>
		string TypeMakata { get; set; }

		/// <summary>
		/// Код типа единицы продукта (например: килограммы, пакет , граммы и т.д)
		/// </summary>
        string UnitType { get; set; }


		string SectionCode { get; set; }
		string SupplierCode { get; set; }
		string TypeCode { get; set; }
		string UnitTypeCode { get; set; }
		string InputTypeCode { get; set; }
	}
}
