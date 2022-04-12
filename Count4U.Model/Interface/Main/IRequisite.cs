using System;
namespace Count4U.Model.Interface.Main
{
    /// <summary>
    /// Интерфейс репозитория для доступа к контактонй информации Requisite свойствам других объектов
    /// </summary>
    public interface IRequisite
    {
        /// <summary>
        /// ID
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// Адресс
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// Контактное лицо
        /// </summary>
        string ContactPerson { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Факс
        /// </summary>
        string Fax { get; set; }

        /// <summary>
        /// Лого файл
        /// </summary>
        string LogoFile { get; set; }

        /// <summary>
        /// Почта
        /// </summary>
        string Mail { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        string Phone { get; set; }

		System.Byte[] Logo { get; set; }
    }
}
