using System;
namespace Count4U.Model.Interface
{
    /// <summary>
    /// Интерфейс предназначен для всех классификаторов - выпадающих списков в интерфейсе пользователя
    /// </summary>
    public interface IClassificator
    {
        /// <summary>
        /// ID - идентификатор
        /// </summary>
        long ID { get; set; }

		/// <summary>
		/// Code - ключ
		/// </summary>
		string Code { get; set; }

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
