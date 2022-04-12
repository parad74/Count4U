using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using System.IO;
using System.Security.AccessControl;
using Count4U.Model.Count4U;
using NLog;
using System.Text;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Count4Mobile.MappingEF;

namespace Count4U.Model.AnalyticDB
{
	public class CurrentInventoryEFRepository : BaseEFRepository, ICurrentInventoryRepository
	{

		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public CurrentInventoryEFRepository(IConnectionDB connection)
			: base(connection)
		{
		}

		public IConnectionDB Connection
		{
			get { return this._connection; }
			set { this._connection = value; }
		}

		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}


		public Count4Mobile.CurrentInventorys GetCurrentInventorys(string pathDB)
		{
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var domainObjects = db.CurrentInventory.ToList().Select(e => e.ToDomainObject());
				return CurrentInventorys.FromEnumerable(domainObjects);
			}
		}

		public Count4Mobile.CurrentInventorys GetCurrentInventorys(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return this.GetCurrentInventorys(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entities = this.GetEntities(db, AsQueryable(db.CurrentInventory), db.CurrentInventory.AsQueryable(), selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				CurrentInventorys result = CurrentInventorys.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

   		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				db.CurrentInventory.ToList().ForEach(e => db.CurrentInventory.DeleteObject(e));
				db.SaveChanges();
			}

		}



		public void Insert(Count4Mobile.CurrentInventorys currentInventorys, string pathDB)
		{
			if (currentInventorys == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				foreach (CurrentInventory currentInventory in currentInventorys)
				{
					if (currentInventory == null) { continue; }
					var entity = currentInventory.ToEntity();
					db.CurrentInventory.AddObject(entity);
 				}
				db.SaveChanges();
			}
		}



		public void Update(Count4Mobile.CurrentInventory currentInventory, string pathDB)
		{
			if (currentInventory == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entity = this.GetEntityByID(db, currentInventory.Id);
				if (entity == null) return;
				entity.ApplyChanges(currentInventory);
				db.SaveChanges();
			}
		}


		private App_Data.CurrentInventory GetEntityByID(App_Data.AnalyticDB dc, long id)
		{
			var entity = dc.CurrentInventory.FirstOrDefault(e => e.Id.CompareTo(id) == 0);
			return entity;
		}



	
	}
}


