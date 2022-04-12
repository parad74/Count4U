using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
    public class SessionEFRepository : BaseEFRepository, ISessionRepository
    {
		private readonly IServiceLocator _serviceLocator;
		private Dictionary<string, Session> _sessionDictionary;
		public SessionEFRepository(IConnectionDB connection, IServiceLocator serviceLocator)
			: base(connection)
        {
			this._serviceLocator = serviceLocator;
        }

		protected IServiceLocator ServiceLocator
		{
			get { return _serviceLocator; }
		}

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region ISessionRepository Members

		public Sessions GetSessions(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.Sessions.ToList().Select(e => e.ToDomainObject());
                return Sessions.FromEnumerable(domainObjects);
            }
        }

		public Sessions GetSessions(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetSessions(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Sessions), db.Sessions.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = Sessions.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}
		
		public Session GetSessionByID(long ID, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByID(db, ID);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}


		public Session GetSessionByCode(string sessionCode, string pathDB)
		{
		using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, sessionCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public Session GetSessionWithMaxDateCreated( string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityWithMaxDateCreated(db);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public void Delete(Session session, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var sectionEntities = db.Sessions.Where(e => e.SessionCode == session.SessionCode).ToList();
				if (sectionEntities != null)
				{
					sectionEntities.ForEach(e => db.Sessions.DeleteObject(e));
				}

				db.SaveChanges();
			}
		}

		public void Insert(Session session, string pathDB)
		{
			if (session == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = session.ToEntity();
				db.Sessions.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Update(Session session, string pathDB)
		{
			if (session == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, session.SessionCode);
				if (entity == null) return;
				entity.ApplyChanges(session);
				db.SaveChanges();
			}
		}

		public void Update(Sessions sessions, string pathDB)
		{
			if (sessions == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (Session session in sessions)
				{
					if (session == null) continue;
					var entity = this.GetEntityByCode(db, session.SessionCode);
					if (entity == null) continue;
					entity.ApplyChanges(session);
				}
				db.SaveChanges();
			}
		}



		public List<string> Insert(List<string> sessionCodeList, string pathDB)
		{
			List<string> documentCodeFullList = new List<string>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var sessionCode in sessionCodeList)
				{
					if (string.IsNullOrEmpty(sessionCode) != true)
					{
						//	if (string.IsNullOrWhiteSpace(sessionCode) == true) continue;
						//	List<string> documentCodeList = db.DocumentHeaders.Where(e => e.SessionCode == sessionCode).Select(e => e.DocumentCode).Distinct().ToList();

						//	foreach (var docCode in documentCodeList)
						//	{
						//		if (documentCodeFullList.Contains(docCode) == false)
						//		{
						//			documentCodeFullList.Add(docCode);
						//		}
						//	}

						//IInventProductRepository inventProductRepository = ServiceLocator.GetInstance<IInventProductRepository>();
						//int countItem = 0;
						//double sumQuantityEdit = 0;

						//foreach (var documentCode in documentCodeList)
						//{
						//	if (string.IsNullOrWhiteSpace(documentCode) == true) continue;
						//	//int count = inventProductRepository.GetCountItemByDocumentCode(documentCode, pathDB);
						//	var entety = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0);
						//	if (entety == null) continue;
						//	var count = entety.Count();
						//	countItem = countItem + Convert.ToInt32(count);
						//	double sum = 0;
						//	try
						//	{
						//		sum = entety.Sum(x => x.QuantityEdit);
						//	}
						//	catch { }
						//	sumQuantityEdit = sumQuantityEdit + Convert.ToDouble(sum);
						//}

						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => x.SessionCode == sessionCode).ToList().Select(e => e);
						documentCodeFullList = documentHeaders.Select(s => s.DocumentCode).ToList();
						//var documentSum = from e in documentHeaders
						//				  orderby e.DocumentCode
						//				  group e by e.DocumentCode into g
						//				  select new DocumentHeader
						//				  {
						//					  DocumentCode = g.Key//, 
						//					  //Total = g.LongCount()
						//				  };


						var inventProducts = AsQueryable(db.InventProducts).Where(x => x.SessionCode == sessionCode).ToList().Select(e => e);

						var inventProductsSum = from e in inventProducts
												orderby e.SessionCode
												group e by e.SessionCode into g
												select new InventProduct
												{
													SectionCode = g.Key,
													QuantityEdit = g.Sum(x => x.QuantityEdit),
													IPValueInt5 = g.Count()
												};
						//var temp = domainObjectsSum.ToList();
						//var temp1 = domainObjectsSum1.ToList();

						Session session = new Session();
						session.SessionCode = sessionCode;
						session.CreateDate = DateTime.Now;
						//session.CountDocument = documentCodeList.Count;

						if (documentHeaders != null)
						{
							//					DocumentHeader documentHeader = documentSum.First();
							session.CountDocument = documentHeaders.Count();
						}
						//session.CountItem = countItem;
						//session.SumQuantityEdit = sumQuantityEdit;

						if (inventProductsSum != null && inventProductsSum.Count() > 0)
						{
							InventProduct inventProduct = inventProductsSum.First();
							session.CountItem = inventProduct.IPValueInt5;
							session.SumQuantityEdit = inventProduct.QuantityEdit;
						}

						//session.CountItem = countItem;
						//session.SumQuantityEdit = sumQuantityEdit;


						var entity = session.ToEntity();
						db.Sessions.AddObject(entity);
					}//sessionCode
				} //sessionCode

				db.SaveChanges();
			}//using
			return documentCodeFullList;
		}


		public List<string> GetDocumentHeaderCodeList(List<string> sessionCodeList, string pathDB)
		{
			List<string> documentCodeFullList = new List<string>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var sessionCode in sessionCodeList)
				{
					if (string.IsNullOrWhiteSpace(sessionCode) == true) continue;
					List<string> documentCodeList = db.DocumentHeaders.Where(e => e.SessionCode == sessionCode).Select(e => e.DocumentCode).Distinct().ToList();

					foreach (var docCode in documentCodeList)
					{
						if (documentCodeFullList.Contains(docCode) == false)
						{
							documentCodeFullList.Add(docCode);
						}
					}

					//int countItem = 0;
					//double sumQuantityEdit = 0;

					//foreach (var documentCode in documentCodeFullList)
					//{
					//	if (string.IsNullOrWhiteSpace(documentCode) == true) continue;
					//	var entety = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0);
					//	if (entety == null) continue;
					//	var count = entety.Count();
					//	countItem = countItem + Convert.ToInt32(count);
					//	double sum = 0;
					//	try
					//	{
					//		sum = entety.Sum(x => x.QuantityEdit);
					//	}
					//	catch { }
					//	sumQuantityEdit = sumQuantityEdit + Convert.ToDouble(sum);
					//}
				} //sessionCode

			}//using
			return documentCodeFullList;
		}

		public Dictionary<string, Session> GetSessionDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				this.FillSessionDictionary(pathDB);
			}
			return this._sessionDictionary;
		}

		public void ClearSessionDictionary()
		{
			this._sessionDictionary.Clear();
			GC.Collect();
		}

		public void FillSessionDictionary(string pathDB)
		{
			this.ClearSessionDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Sessions sessions = this.GetSessions(pathDB);

					this._sessionDictionary = sessions.Select(e => e).Distinct().ToDictionary(k => k.SessionCode);
				}
				catch { }
			}
		}
			

        #endregion

		#region private

		private App_Data.Session GetEntityByID(App_Data.Count4UDB db, long ID)
		{
			var entity = db.Sessions.FirstOrDefault(e => e.ID.CompareTo(ID) == 0);
			return entity;
		}

		private App_Data.Session GetEntityByCode(App_Data.Count4UDB db, string sessionCode)
		{
			var entity = db.Sessions.FirstOrDefault(e => e.SessionCode.CompareTo(sessionCode) == 0);
			return entity;
		}


		private App_Data.Session GetEntityWithMaxDateCreated(App_Data.Count4UDB db)
		{
			var entity = db.Sessions.OrderByDescending(t => t.CreateDate).
				FirstOrDefault();
			return entity;
		}

		//private double GetSumQuantityEditByCode(App_Data.Count4UDB db, string sessionCode)
		//{
		//    var entity = db.Sessions.OrderByDescending(t => t.CreateDate).
		//        FirstOrDefault();
		//    return entity;
		//}
	
		#endregion

		
	}
}
