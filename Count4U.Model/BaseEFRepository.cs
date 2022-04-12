using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using NLog;
//using System.Data.Entity;

namespace Count4U.Model
{
    public abstract class BaseEFRepository
    {
		protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		//private string _connectionString = PropertiesSettings.Count4UDBConnectionString;
		private string _mainDBConnectionString;
		private string _auditConnectionString;
		private string _processDBConnectionString;
		//private string _count4UConnectionString;
		protected IConnectionDB _connection;

		//public BaseEFRepository(string connectionString)
		public BaseEFRepository(IConnectionDB connection)
		{            
			//if (String.IsNullOrWhiteSpace(connectionString) == false)
			this._connection = connection;
			this._auditConnectionString = connection.AuditConnectionString;			 //_configurationAuditDBConnectionString
			//this.Count4UConnectionString = connection.Count4UConnectionString;
			this._mainDBConnectionString = connection.MainDBConnectionString;
			this._processDBConnectionString = connection.ProcessDBConnectionString;

		}

		//public void RefreshBaseEFRepositoryConnectionDB()	 //??? только для теста
		//{
		//	this._connection._configurationProcessDBConnectionString = this._connection.DBSettings.BuildProcessDbConnectionString();
		//	//string subProcess = @"Process\" + processRepository.GetProcessCode_InProcess();
		//	//получить текущий процесс и конектить БД из него
		//	//string subProcess = @"Process\ProcessCode1";
		//	string subProcess = @"";
		//	if (string.IsNullOrWhiteSpace(this._connection.DBSettings.SettingsRepository.ProcessCode) == false)
		//		subProcess = @"Process\" + this._connection.DBSettings.SettingsRepository.ProcessCode;
		//	//string subProcess = @"";
		//	this._connection._configurationMainDBConnectionString = this._connection.DBSettings.BuildMainDBConnectionString(subProcess);
		//	this._connection._configurationAuditDBConnectionString = this._connection.DBSettings.BuildAuditDbConnectionString(subProcess);

		//	this._connection._configurationAuditDBConnectionString = this._connection.DBSettings.BuildAuditDbConnectionString(subProcess);
		//	// this._configurationAuditDBConnectionString == connection.AuditConnectionString;	 == _auditConnectionString
		//}

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

		public string MainDBConnectionString()
		{
			return this._mainDBConnectionString; 
			//get { return this._mainDBConnectionString; }
			//set { this._mainDBConnectionString = value; }
		}

		public string AuditConnectionString()
		{
			return this._auditConnectionString;
			//get { return this._auditConnectionString; }
			//set { this._auditConnectionString = value; }
		}


		public string ProcessDBConnectionString()
		{
			return this._processDBConnectionString;
			//get { return this._processDBConnectionString; }
			//set { this._processDBConnectionString = value; }
		}
	
		public string BuildCount4UConnectionString(string subFolder)
		{
			return this._connection.BuildCount4UConnectionString(subFolder);
		}

		public string BuildAnalyticDBConnectionString(string subFolder)
		{
			return this._connection.BuildAnalyticDBConnectionString(subFolder);
		}

        public abstract IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet) where TEntity : class;

	//	public abstract IQueryable<TEntity> AsQueryable<TEntity>(DbSet<TEntity> objectSet) where TEntity : class; 
		
      
		public List<TEntity> GetEntities<TEntity>(ObjectContext db, IQueryable<TEntity> query, 
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

			_logger.Trace(((System.Data.Entity.Core.Objects.ObjectQuery)query).ToTraceString());

			if (query == null) return new List<TEntity>();
            var entities = query.ToList();
            return entities;
        }


//		public List<TEntity> GetEntities<TEntity>(DbContext db, IQueryable<TEntity> query, 
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

    }
}
