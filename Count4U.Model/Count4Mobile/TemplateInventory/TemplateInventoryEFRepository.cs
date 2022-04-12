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
	public class TemplateInventoryEFRepository : BaseEFRepository, ITemplateInventoryRepository
	{

		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public TemplateInventoryEFRepository(IConnectionDB connection)
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

		
		public Count4Mobile.TemplateInventorys GetTemplateInventorys(string pathDB)
		{
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var domainObjects = db.TemplateInventory.ToList().Select(e => e.ToDomainObject());
				return TemplateInventorys.FromEnumerable(domainObjects);
			}
		}

		public Count4Mobile.TemplateInventorys GetTemplateInventorys(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return this.GetTemplateInventorys(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entities = this.GetEntities(db, AsQueryable(db.TemplateInventory), db.TemplateInventory.AsQueryable(), selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				TemplateInventorys result = TemplateInventorys.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
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

	

		public void Insert(Count4Mobile.TemplateInventorys templateInventorys, string pathDB)
		{
			if (templateInventorys == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				foreach (TemplateInventory templateInventory in templateInventorys)
				{
					if (templateInventory == null) { continue; }
					App_Data.TemplateInventory entity = GetEntityByID(db, templateInventory.Id);
					// если нет объекта с таким кодом, только тогда добавляем
					if (entity == null)
					{
						entity = templateInventory.ToEntity();
						db.TemplateInventory.AddObject(entity);
					}
				}
				db.SaveChanges();
			}
		}



		public void Update(Count4Mobile.TemplateInventory templateInventory, string pathDB)
		{
			if (templateInventory == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entity = this.GetEntityByID(db, templateInventory.Id);
				if (entity == null) return;
				entity.ApplyChanges(templateInventory);
				db.SaveChanges();
			}
		}


		private App_Data.TemplateInventory GetEntityByID(App_Data.AnalyticDB dc, long id)
		{
			var entity = dc.TemplateInventory.FirstOrDefault(e => e.Id.CompareTo(id) == 0);
			return entity;
		}
	}
}


