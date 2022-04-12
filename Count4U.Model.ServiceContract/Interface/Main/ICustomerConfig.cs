using System;
namespace Count4U.Model.Interface.Main
{
    /// <summary>
    /// Интерфейс настроек пользователя CustomerConfig
    /// </summary>
    public interface ICustomerConfig
    {
        /// <summary>
        /// ID
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Публичное имя
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Код пользователя //?? TO DO в БД нет и политика пока не определена, что такое настройки
        /// </summary>
        string CustomerСode { get; set; }
    }
}
