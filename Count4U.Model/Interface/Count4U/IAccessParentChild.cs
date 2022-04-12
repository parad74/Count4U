using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфенйс объекта AccessParentChild, предназначенного для хранения информаци о товарах, хранимых партиями,
    /// как то мешок, блок, коробка, и которые можно продавать как партиями так и частями
    /// </summary>
    public interface IAccessParentChild
	{
        /// <summary>
        /// ID
        /// </summary>
		long ID { get; set; }

        /// <summary>
		///  Если этот продукт является составляющим другого продукта , 
		///  то это свойство  указывает  ID основного продукта.
		///  Например пачка сигарет и блок сигарет. Пачка входит в блок. 
        /// </summary>
        long? ParentProductID { get; set; }

        /// <summary>
        /// ID товара, который внутири, например кг сахара
        /// </summary>
        long? ProductID { get; set; }

        /// <summary>
        /// ID eдиниц хранения, как то кг, мешки 
        /// </summary>
        long? UnitTypeID { get; set; }

        /// <summary>
        /// Как вводился товар в приложение, вручную, автоматически 
        /// </summary>
		string InputType { get; set; }

        /// <summary>
        /// Название родительского товара
        /// </summary>
		string ParentProduct { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        string Product { get; set; }

        /// <summary>
        /// Количество ( считаемое в UnitType)  Product  в ParentProduct
        /// </summary>
		double Unit { get; set; }

        /// <summary>
        /// ID eдиниц хранения, как то кг, мешки 
        /// </summary>
        string UnitType { get; set; }

		string InputTypeCode { get; set; }
		string UnitTypeCode { get; set; }
	}
}
