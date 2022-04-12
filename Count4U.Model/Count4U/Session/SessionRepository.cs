using System;
using System.Collections.Generic;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class SessionRepository : ISessionRepository
    {
        private Sessions _list;

        #region ISessionRepository Members

		public Sessions GetSession(string pathDB)
        {
            if (this._list == null)
            {
                this._list = new Sessions {
                    new Session() { ID = 1, SessionCode = "Session1", CreateDate = DateTime.Now.Date },
                };
            }
            return this._list;
        }

        #endregion

		#region ISessionRepository Members

		public Sessions GetSessions(string pathDB)
		{
			throw new NotImplementedException();
		}

		public Session GetSessionByID(long ID, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Session GetSessionByCode(string sessionCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISessionRepository Members


		public List<string> Insert(List<string> sessionCodeList, string pathDB)
		{
			return null;
		}

		#endregion

		#region ISessionRepository Members


		public Sessions GetSessions(SelectionParams.SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Delete(Session session, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Insert(Session session, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Update(Session session, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Update(Sessions sessions, string pathDB)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.Dictionary<string, Session> GetSessionDictionary(string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public void ClearSessionDictionary()
		{
			throw new NotImplementedException();
		}

		public void FillSessionDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISessionRepository Members


		public Session GetSessionWithMaxDateCreated(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion


		public List<string> GetDocumentHeaderCodeList(List<string> sessionCodeList, string pathDB)
		{
			throw new NotImplementedException();
		}
	}
}
