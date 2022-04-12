using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    public interface ISession 
    {
        /// <summary>
        /// ID
        /// </summary>
		long ID { get; set; }

		/// <summary>
		/// Код масафона  (получаем от масофона)  
		/// </summary>
        string PDAID { get; set; }

		/// <summary>
		/// Код
		/// </summary>
        string Code { get; set; }

		/// <summary>
		/// Дата и время создания сессии в база данных
		/// </summary>
		DateTime CreateDate { get; set; }

		/// <summary>
		/// Дата и время создания данных на масафоне   
		/// </summary>
		DateTime? PDADate { get; set; }

		/// <summary>
		/// Код оператора 
		/// </summary>
		string WorkerGUID { get; set; }
	}
}
