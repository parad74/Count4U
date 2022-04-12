using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using NLog;
using System.Data.Entity;
//using System.Data.Entity;

namespace Count4U.Model
{
    public abstract class BaseEFDbSetRepository
    {
		protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		//private string _connectionString = PropertiesSettings.Count4UDBConnectionString;
		//private string _mainDBConnectionString;
		//private string _auditConnectionString;
		//private string _count4UConnectionString;
		protected IConnectionDB _connection;

		//public BaseEFRepository(string connectionString)
		public BaseEFDbSetRepository(IConnectionDB connection)
		{            
			//if (String.IsNullOrWhiteSpace(connectionString) == false)
			this._connection = connection;
			//this._auditConnectionString = connection.AuditConnectionString;
			//this.Count4UConnectionString = connection.Count4UConnectionString;
			//this._mainDBConnectionString = connection.MainDBConnectionString;
		}

		//public IConnectionDB Connection
		//{
		//    get { return this._connection; }
		//    set { this._connection = value; }
		//}

		public string ProductMakatBarcodesDictionaryCapacity
		{
			get
			{
				return _connection.ProductMakatBarcodesDictionaryCapacity;
			}
		}

		//public string MainDBConnectionString
		//{
		//	get { return this._mainDBConnectionString; }
		//	set { this._mainDBConnectionString = value; }
		//}

		//public string AuditConnectionString
		//{
		//	get { return this._auditConnectionString; }
		//	set { this._auditConnectionString = value; }
		//}

	
		//public string BuildCount4UConnectionString(string subFolder)
		//{
		//	return this._connection.BuildCount4UConnectionString(subFolder);
		//}

		public string BuildAnalyticDBConnectionString(string subFolder)
		{
			return this._connection.BuildAnalyticDBConnectionString(subFolder);
		}

		//public abstract IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet) where TEntity : class;

		public abstract IQueryable<TEntity> AsQueryable<TEntity>(DbSet<TEntity> objectSet) where TEntity : class; 
		
      
//		public List<TEntity> GetEntities<TEntity>(ObjectContext db, IQueryable<TEntity> query, 
//			IQueryable<TEntity> counterQuery, SelectParams selectParams, out long totalCount)
//		{
//			//db.Configuration.AutoDetectChangesEnabled = false;
//			//db.Configuration.ValidateOnSaveEnabled = false; 

//			totalCount = 0;
//			counterQuery = selectParams.ApplyFilterToQuery(counterQuery);
//			totalCount = counterQuery.LongCount();

////			query.AsNoTracking()
//			query = selectParams.ApplyFilterToQuery(query);
//			query = selectParams.ApplySortAndPagingToQuery(query);

//			_logger.Trace(((System.Data.Objects.ObjectQuery)query).ToTraceString());

//			if (query == null) return new List<TEntity>();
//			var entities = query.ToList();
//			return entities;
//		}


		public List<TEntity> GetEntities<TEntity>(DbContext db, IQueryable<TEntity> query, 
            IQueryable<TEntity> counterQuery, SelectParams selectParams, out long totalCount)
        {
			//db.Configuration.AutoDetectChangesEnabled = false;
			//db.Configuration.ValidateOnSaveEnabled = false; 

            totalCount = 0;
            counterQuery = selectParams.ApplyFilterToQuery(counterQuery);
			totalCount = counterQuery.LongCount();

//			query.AsNoTracking()
			query = selectParams.ApplyFilterToQuery(query);
            query = selectParams.ApplySortAndPagingToQuery(query);

            //_logger.Trace(((System.Data.Objects.ObjectQuery)query).ToTraceString());

			if (query == null) return new List<TEntity>();
			var entities = query.ToList();
            return entities;
        }

    }
}
