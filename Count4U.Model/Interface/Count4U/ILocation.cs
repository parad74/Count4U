using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    public interface ILocation
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
        /// Публикуемое имя
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
		string Description { get; set; }

        /// <summary>
        /// Отображаемый цвет
        /// </summary>
        string BackgroundColor { get; set; }
	}
}
