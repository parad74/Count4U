using System;
using Count4U.Model.Count4U;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к Session объектам
    /// </summary>
    public interface ISessionRepository 
    {
        /// <summary>
        /// Получить все сессии
        /// </summary>
        /// <returns></returns>
		Sessions GetSessions(string pathDB);
		Sessions GetSessions(SelectParams selectParams, string pathDB);

		Session GetSessionByID(long ID, string pathDB);

		Session GetSessionByCode(string sessionCode, string pathDB);

		void Delete(Session session, string pathDB);
		void Insert(Session session, string pathDB);
		void Update(Session session, string pathDB) ;
		void Update(Sessions sessions, string pathDB);
		List<string> Insert(List<string> sessionCodeList, string pathDB);
		Dictionary<string, Session> GetSessionDictionary(string pathDB, bool refill = false)	;
		void ClearSessionDictionary();
		void FillSessionDictionary(string pathDB);
		Session GetSessionWithMaxDateCreated(string pathDB);
		List<string> GetDocumentHeaderCodeList(List<string> sessionCodeList, string pathDB);
	}
}
