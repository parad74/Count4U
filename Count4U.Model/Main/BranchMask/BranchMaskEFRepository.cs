using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Main.MappingEF;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Main
{
    public class BranchMaskEFRepository : BaseEFRepository, IMaskRepository
    {

		public BranchMaskEFRepository(IConnectionDB connection)
			: base(connection)
		{

		}

		public IConnectionDB Connection
		{
			get { return this._connection; }
			set { this._connection = value; }
		}

		#region BaseEFRepository Members
		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}
		#endregion

		#region IMaskRepository Members

		public Masks GetMasks()
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.BranchMask.ToList().Select(e => e.ToBranchDomainObject());
				return Masks.FromEnumerable(domainObjects);
			}
		}

		public Masks GetMasks(SelectParams selectParams)
		{
		    if (selectParams == null)
		        return GetMasks();

		    long totalCount = 0;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
		    {
				 //Получение сущностей и общего количества из БД.
				 //Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.BranchMask), dc.BranchMask.AsQueryable(), selectParams, out totalCount);

				 //Преобразование сущностей в объекты предметной области.
				 //Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToBranchDomainObject());

				 //Возврат результата.
				 //Returning result.
		        Masks result = Masks.FromEnumerable(domainObjects);
		        result.TotalCount = totalCount;
		        return result;
		    }
		}


		public void Delete(string code, string adapterCode, string fileCode, string pathDB)
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(db, code,  adapterCode, fileCode);
				if (entity == null) return;
				db.BranchMask.DeleteObject(entity);
				db.SaveChanges();
			}
		}

		public void Delete(string code, string adapterCode, string pathDB)
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entities = this.GetEntitiesByCode(db, code, adapterCode);
				if (entities == null) return;
				foreach (var entity in entities)
				{
					db.BranchMask.DeleteObject(entity);
				}
				db.SaveChanges();
			}
		}

		public void Delete(string code, string pathDB)
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entities = this.GetEntitiesByCode(db, code);
				if (entities == null) return;
				foreach (var entity in entities)
				{
					db.BranchMask.DeleteObject(entity);
				}
				db.SaveChanges();
			}
		}

		public void Insert(Mask mask, string pathDB)
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = mask.ToBranchEntity();
				db.BranchMask.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Update(Mask mask, string pathDB)
		{
			if (mask == null) return;
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(db, mask.Code, mask.AdapterCode, mask.FileCode);
				if (entity == null) return;
				entity.ApplyBranchChanges(mask);
				db.SaveChanges();
			}
		}
		#endregion

		#region private

		private App_Data.BranchMask GetEntityByCode(App_Data.MainDB db, string code,
			string adapterCode, string fileCode)
		{
			var entity = db.BranchMask.FirstOrDefault(e => (e.Code.CompareTo(code) == 0) 
				&& (e.AdapterCode.CompareTo(adapterCode) == 0) && (e.FileCode.CompareTo(fileCode) == 0));
			return entity;
		}

		private List<App_Data.BranchMask> GetEntitiesByCode(App_Data.MainDB db, string code,
			string adapterCode)
		{
			var entitis = db.BranchMask.Where(e => (e.Code == code && e.AdapterCode == adapterCode)).ToList();
			return entitis;
		}

		private List<App_Data.BranchMask> GetEntitiesByCode(App_Data.MainDB db, string code)
		{
			var entitis = db.BranchMask.Where(e => (e.Code == code)).ToList();
			return entitis;
		}
		#endregion


		
	}
}
