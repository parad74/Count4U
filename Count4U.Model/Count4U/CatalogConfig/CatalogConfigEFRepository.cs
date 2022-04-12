using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface;
using Count4U.Model.Main;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Audit;

namespace Count4U.Model.Count4U
{
	public class CatalogConfigEFRepository : BaseEFRepository, ICatalogConfigRepository
    {
		public CatalogConfigEFRepository(IConnectionDB connection)
			: base(connection)
		{
		}

		#region BaseEFRepository Members
		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}
		#endregion

		public void ClearCatalogConfigs(string pathDB)
		{
			if (string.IsNullOrWhiteSpace(pathDB)) return;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.CatalogConfigs.ToList().ForEach(e => db.CatalogConfigs.DeleteObject(e));
				db.SaveChanges();
			}
		}

		public CatalogConfig GetCurrentCatalogConfig(string pathDB)
		{
			if (string.IsNullOrWhiteSpace(pathDB)) return null;
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entity = dc.CatalogConfigs.FirstOrDefault();
					return entity.ToDomainObject();
				}
				catch { return null; }
			}
		}

		public void SetCurrentCatalogConfig(CatalogConfig catalogConfig, string pathDB)
		{
			if (string.IsNullOrWhiteSpace(pathDB)) return;
			this.Insert(catalogConfig, pathDB);
		}

		public void SetCurrentCatalogConfig(AuditConfig auditConfig, string pathDB)
		{
			if (string.IsNullOrWhiteSpace(pathDB)) return;
			this.Insert(auditConfig, pathDB);
		}

	   //TODO: error on insert
		public void Insert(CatalogConfig catalogConfig, string pathDB)
		{
			if (catalogConfig == null) return;
			if (string.IsNullOrWhiteSpace(pathDB)) return;
			this.ClearCatalogConfigs(pathDB);
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = catalogConfig.ToEntity();
				db.CatalogConfigs.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(AuditConfig auditConfig, string pathDB)
		{
			if (string.IsNullOrWhiteSpace(pathDB)) return;
			CatalogConfig catalogConfig = auditConfig.ToCatalogConfig();
			if (catalogConfig != null)
			{
				this.Insert(catalogConfig, pathDB);
			}
		}

		public void Update(CatalogConfig catalogConfig, string pathDB)
		{
			if (catalogConfig == null) return;
			if (string.IsNullOrWhiteSpace(pathDB)) return;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByID(db, catalogConfig.ID);
				if (entity == null) return;
				entity.ApplyChanges(catalogConfig);
				db.SaveChanges();
			}
		}

	
		#region private

		private App_Data.CatalogConfig GetEntityByID(App_Data.Count4UDB db, long ID)
		{
			var entity = db.CatalogConfigs.FirstOrDefault(e => e.ID.CompareTo(ID) == 0);
			return entity;
		}

		#endregion


		
	}
}
