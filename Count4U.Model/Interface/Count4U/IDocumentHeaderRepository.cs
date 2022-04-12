using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

namespace Count4U.Model.Interface.Count4U
{
	/// <summary>
	/// Интерфейс репозитория для доступа к DocumentHeader объектам
	/// </summary>
	public interface IDocumentHeaderRepository
	{
		/// <summary>
		/// Получить весь список документов
		/// </summary>
		/// <returns></returns>
		DocumentHeaders GetDocumentHeaders(string pathDB);

		/// <summary>
		/// Получить список документов по параметрам выборки
		/// </summary>
		/// <param name="selectParams"></param>
		/// <returns></returns>
		DocumentHeaders GetDocumentHeaders(SelectParams selectParams, string pathDB);

		/// <summary>
		/// Получить документ по коду
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		DocumentHeader GetDocumentHeaderByCode(string code, string pathDB);

		/// <summary>
		/// Получить документы, к которым привязан Итур
		/// </summary>
		/// <param name="itur"></param>
		/// <returns></returns>
		DocumentHeaders GetDocumentHeadersByItur(Itur itur, string pathDB);
		void SetNullToApproveDocuments(List<string> documentCodes, string pathDB);

		/// <summary>
		///  Получить документы, к которым привязан Итур
		/// </summary>
		/// <param name="iturID"></param>
		/// <returns></returns>
		DocumentHeaders GetDocumentHeadersByIturCode(string iturCode, string pathDB);

		/// <summary>
		///  Получить документы, которые созданы в сессию
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		DocumentHeaders GetDocumentHeadersBySession(Session session, string pathDB);

		/// <summary>
		///  Получить документы, которые созданы в сессию
		/// </summary>
		/// <param name="sessionID"></param>
		/// <returns></returns>
		DocumentHeaders GetDocumentHeadersBySessionCode(string sessionCode, string pathDB);

		/// <summary>
		///   Получить документы с заданным статусом 
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		DocumentHeaders GetDocumentHeadersByStatusCode(string statusCode, string pathDB);

		/// <summary>
		/// Удалить документ
		/// </summary>
		/// <param name="documentHeader"></param>
		void Delete(DocumentHeader documentHeader, string pathDB);

		/// <summary>
		/// Удалить документ по коду
		/// </summary>
		/// <param name="documentCode"></param>
		void Delete(string code, string pathDB);

		/// <summary>
		/// Удалить все документы к которым привязан Итур
		/// </summary>
		/// <param name="itur"></param>
		void DeleteAllByItur(Itur itur, string pathDB);

		/// <summary>
		///  Удалить все документы к которым привязан итур
		/// </summary>
		/// <param name="iturID"></param>
		void DeleteAllByIturCode(string iturCode, string pathDB);

		void DeleteAllDocumentsWithoutAnyInventProduct(string pathDB);

		/// <summary>
		/// Вставить итур и привязать к документу
		/// </summary>
		/// <param name="itur"></param>
		/// <param name="documentHeader"></param>
		void Insert(Itur itur, DocumentHeader documentHeader, string pathDB);

		/// <summary>
		/// Вставить итур и привязать ко всем документам в списке
		/// </summary>
		/// <param name="itur"></param>
		/// <param name="documentHeaders"></param>
		void Insert(Itur itur, DocumentHeaders documentHeaders, string pathDB);
		long Insert(DocumentHeader documentHeader, string pathDB);
		void Insert(DocumentHeaders documentHeaders, string pathDB);

		/// <summary>
		/// Изменить 
		/// </summary>
		/// <param name="branch"></param>
		void Update(DocumentHeader documentHeader, string pathDB);

		BitArray GetResultInventProductStatusBitOrByDocumentCode(string documentCode, string pathDB,
			bool refill = false);

		int GetResultInventProductStatusIntOrByDocumentCode(string documentCode, string pathDB,
			bool refill = false);

		int RefillDocHeaderStatusBitByDocumenCode(string documentCode, string pathDB);
		void RefillIturStatistic(List<string> sessionCodeList, List<string> iturCodes, List<string> docCodes, string pathDB);
		void RefillDocumentStatisticBySession(List<string> sessionCodeList, string pathDB);

		void RefillStatusBit(string pathDB);

		void ClearStatusBit(string pathDB);

		Dictionary<string, DocumentHeader> GetDocumentHeaderDictionary(string pathDB, bool refill = false);
		long GetCountDocumentWithError(List<string> sessionCodeList, string pathDB);
		List<string> GetDocumentCodeList(string pathDB);
		long GetCountDocumentWithoutError(List<string> sessionCodeList, string pathDB);
		void FillDocumentHeaderDictionary(string pathDB);
		void ClearDocumentHeaderDictionary();
		List<string> GetIturCodeList(string pathDB);
		List<string> GetDocumentHeaderCodeList(IEnumerable<App_Data.DocumentHeader> documentHeaders);
		Dictionary<string, DocumentHeader> GetIturDictionaryMaxDateTime(string pathDB);
		Dictionary<string, DocumentHeader> GetIturDocumentCodeDictionary(string pathDB);

		void UpdateWorkerName(string deviceName, string oldWorkerName, string newWorkerName, string pathDB);



	}
}
