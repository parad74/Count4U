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
	public class PreviousInventoryEFRepository : BaseEFRepository, IPreviousInventoryRepository
    {
		private readonly IServiceLocator _serviceLocator;
		private readonly IConnectionADO _connectionADO;
		private readonly IDBSettings _dbSettings;

		public PreviousInventoryEFRepository(IConnectionDB connection,
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

		public PreviousInventorys GetPreviousInventorys(string pathDB)
        {
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var domainObjects = db.PreviousInventory.ToList().Select(e => e.ToDomainObject());
				return PreviousInventorys.FromEnumerable(domainObjects);
			}
        }


		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				db.PreviousInventory.ToList().ForEach(e => db.PreviousInventory.DeleteObject(e));
				db.SaveChanges();
			}

		}


		public void Insert(PreviousInventory previousInventory, string pathDB)
		{
			if (previousInventory == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entity = previousInventory.ToEntity();
				db.PreviousInventory.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(PreviousInventorys previousInventorys, string pathDB)
		{
			if (previousInventorys == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				foreach (PreviousInventory previousInventory in previousInventorys)
				{
					if (previousInventory == null) { continue; }
					var entity = previousInventory.ToEntity();
					db.PreviousInventory.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		//string key = serialNumberLocal + "|" + makat + "|" + propertyStr1;
		public Dictionary<string, PreviousInventory> GetDictionaryPreviousInventorys(string pathDB)
		{
			Dictionary<string, PreviousInventory> dictionary = new Dictionary<string, PreviousInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{

				try
				{
					dictionary = db.PreviousInventory.Select(e => e.ToDomainObject()).Distinct().ToDictionary(k => k.SerialNumberLocal + "|" + k.ItemCode + "|" + k.PropertyStr1);
				}
				catch { }
			}
			if (dictionary == null) return new Dictionary<string, PreviousInventory>();

			return dictionary;
		}

		//Uid
		public Dictionary<string, PreviousInventory> GetDictionaryPreviousInventorysByUid(string pathDB)
		{
			Dictionary<string, PreviousInventory> dictionary = new Dictionary<string, PreviousInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{

				try
				{
					dictionary = db.PreviousInventory.Select(e => e.ToDomainObject()).Distinct().ToDictionary(k => k.Uid);
				}
				catch { }
			}
			if (dictionary == null) return new Dictionary<string, PreviousInventory>();

			return dictionary;
		}


		public List<PreviousInventory> GetListBySerialNumberLocal(string serialNumberLocal, string pathDB)
		{
			List<PreviousInventory> list = new List<PreviousInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				try
				{
					list = db.PreviousInventory.Where(k => k.SerialNumberLocal == serialNumberLocal).Select(e => e.ToDomainObject()).ToList();
				}
				catch { }
			}
			if (list == null) return new List<PreviousInventory>();

			return list;
		}

		public List<PreviousInventory> GetListBySerialNumberSupplier(string serialNumberSupplier, string pathDB)
		{
			List<PreviousInventory> list = new List<PreviousInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				try
				{
					list = db.PreviousInventory.Where(k => k.SerialNumberSupplier == serialNumberSupplier).Select(e => e.ToDomainObject()).ToList();
				}
				catch { }
			}
			if (list == null) return new List<PreviousInventory>();

			return list;
		}


		public List<PreviousInventory> GetListBySerialNumberSupplierOrSerialNumberLocal(string serialNumberSupplier, string serialNumberLocal, string pathDB)
		{
			List<PreviousInventory> list = new List<PreviousInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{

				try
				{
					list = db.PreviousInventory.Where(k => k.SerialNumberSupplier == serialNumberSupplier || k.SerialNumberLocal == serialNumberLocal).Select(e => e.ToDomainObject()).ToList();
				}
				catch { }
			}
			if (list == null) return new List<PreviousInventory>();

			return list;
		}
	

		public List<PreviousInventory> GetListByItemCode(string itemCode, string pathDB)
		{
			List<PreviousInventory> list = new List<PreviousInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				try
				{
					list = db.PreviousInventory.Where(k => k.ItemCode == itemCode).Select(e => e.ToDomainObject()).ToList();
				}
				catch { }
			}
			if (list == null) return new List<PreviousInventory>();

			return list;
		}

        #endregion

	}
}

