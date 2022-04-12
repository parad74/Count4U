using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс для описания объектов DocumentHeader,
    /// Документ - это единица аудита. 
	/// Каждая Сессия может содержать несколько Документов
	/// Каждый Документ принадлежит одной Сессии
	/// Документ содержит длин Итур или или несколько целых Итуров и несколько частей Итуров
	/// Итур может принадлежать одному или нескольким Документам
    /// </summary>
    public interface IDocumentHeader
	{
        /// <summary>
        /// ID
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// IturID - Итур
        /// </summary>
        long? IturID { get; set; }
        
        /// <summary>
        /// SessionID - Секция
        /// </summary>
        long? SessionID { get; set; }

        /// <summary>
        /// StatusDocHeaderID - Документ
        /// </summary>
        long? StatusDocHeaderID { get; set; }

        /// <summary>
        /// Имя документа, для отбражения в списках
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Утвежденный
        /// </summary>
        bool? Approve { get; set; }

        /// <summary>
		/// Код документа полученный от считывающего устройства	
        /// </summary>
		string Code { get; set; }

        /// <summary>
		/// Код документа полученный от считывающего устройства	
        /// </summary>
		string DocumentCode { get; set; }

        /// <summary>
        /// Итур
        /// </summary>
        string Itur { get; set; }

        /// <summary>
        /// Секция
        /// </summary>
        string Session { get; set; }

        /// <summary>
        /// Статус документа
        /// </summary>
        string StatusDocHeader { get; set; }

		/// <summary>
		/// 
		/// </summary>
		DateTime CreateDate { get; set; }

		/// <summary>
		/// 
		/// </summary>
		DateTime? ModifyDate { get; set; }

		/// <summary>
		/// IturCode - Итур
		/// </summary>
		string IturCode { get; set; }

		/// <summary>
		/// StatusDocHeaderCode - Документ
		/// </summary>
		string StatusDocHeaderCode { get; set; }

		int StatusDocHeaderBit { get; set; }
 
		string WorkerGUID { get; set; }
	}
}
