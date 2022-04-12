using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Common;
using System.Data.SqlServerCe;
using Count4U.Model.Count4Mobile;
using System.Data.Entity;
using System.IO;
using Count4U.Model.Count4Mobile.MappingEF;

namespace Count4U.Model.Count4U
{
	public class CurrentInventoryAdvancedEFRepository : BaseEFRepository, ICurrentInventoryAdvancedRepository
    {
		private readonly IServiceLocator _serviceLocator;
		private readonly IConnectionADO _connectionADO;
		private readonly IDBSettings _dbSettings;

		public CurrentInventoryAdvancedEFRepository(IConnectionDB connection,
			IConnectionADO connectionADO,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator)
            : base(connection)
        {
			this._serviceLocator = serviceLocator;
			this._connectionADO = connectionADO;
			this._dbSettings = dbSettings;
			//Database.SetInitializer<AnalyticDBContext>(new AnalyticDBContextInitializer());
        }

		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}

		
		

        #region ILocationRepository Members

		public CurrentInventoryAdvanceds GetCurrentInventoryAdvanceds(string pathDB)
        {
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var domainObjects = db.CurrentInventoryAdvanced.ToList().Select(e => e.ToDomainObject());
				return CurrentInventoryAdvanceds.FromEnumerable(domainObjects);
			}
        }




		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				db.TemplateInventory.ToList().ForEach(e => db.TemplateInventory.DeleteObject(e));
				db.SaveChanges();
			}
		}
	
		public void Insert(CurrentInventoryAdvanced currentInventoryAdvanced, string pathDB)
        {
			if (currentInventoryAdvanced == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entity = currentInventoryAdvanced.ToEntity();
				db.CurrentInventoryAdvanced.AddObject(entity);
				db.SaveChanges();
			}
        }

		public void Insert(CurrentInventoryAdvanceds currentInventoryAdvanceds, string pathDB)
		{
			if (currentInventoryAdvanceds == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				foreach (CurrentInventoryAdvanced сurrentInventoryAdvanced in currentInventoryAdvanceds)
				{
					if (сurrentInventoryAdvanced == null) { continue; }
					var entity = сurrentInventoryAdvanced.ToEntity();
					db.CurrentInventoryAdvanced.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		public IEnumerable<CurrentInventoryAdvanced> GetCurrentInventoryAdvancedEnumerable
			(string pathDB, bool refill = true)
		{
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
					this._serviceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();

			if (refill == true)
			{
				currentInventoryAdvancedSourceRepository.ClearCurrentInventoryAdvanced(pathDB);
				currentInventoryAdvancedSourceRepository.InsertCurrentInventoryAdvanced(pathDB, true);
			}//refill

			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var domainObjects = db.CurrentInventoryAdvanced.ToList().Select(e => e.ToDomainObject());
				return CurrentInventoryAdvanceds.FromEnumerable(domainObjects);
			}
		
		}

	
		public List<CurrentInventoryAdvanced> GetCurrentInventoryAdvancedList(string pathDB, 
			bool refill = true,
			bool refillCatalogDictionary = false,
			SelectParams selectParams = null,
			Dictionary<object, object> parmsIn = null,
			List<ImportDomainEnum> importType = null)
		{
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
					this._serviceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			 
			if (refill == true)
			{
				currentInventoryAdvancedSourceRepository.ClearCurrentInventoryAdvanced(pathDB);
				currentInventoryAdvancedSourceRepository.InsertCurrentInventoryAdvanced(pathDB, 
					refill, refillCatalogDictionary,selectParams, parmsIn, importType);
			}//refill

			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{

				if (selectParams != null)
				{
					long totalCount = 0;
					var entities = base.GetEntities(db, AsQueryable(db.CurrentInventoryAdvanced), 
						db.CurrentInventoryAdvanced.AsQueryable(),
						selectParams, out totalCount);
					return entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					return db.CurrentInventoryAdvanced.Select(e => e.ToDomainObject()).ToList();
				}
			}
		}

        #endregion

	}
}

