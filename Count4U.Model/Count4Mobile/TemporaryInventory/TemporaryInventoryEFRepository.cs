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
using Count4U.Model.Common;
using Count4U.Model.Count4Mobile.MappingEF;


namespace Count4U.Model.Count4U
{
	public class TemporaryInventoryEFRepository : BaseEFRepository, ITemporaryInventoryRepository
    {
		private readonly IServiceLocator _serviceLocator;
		private readonly IConnectionADO _connectionADO;
		private readonly IDBSettings _dbSettings;

		public TemporaryInventoryEFRepository(IConnectionDB connection,
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

		public TemporaryInventorys GetTemporaryInventorys(string pathDB)
        {
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var domainObjects = db.TemporaryInventory.ToList().Select(e => e.ToDomainObject());
				return TemporaryInventorys.FromEnumerable(domainObjects);
			}
        }

	
		

		public void DeleteAll(string pathDB)
        {
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				db.TemporaryInventory.ToList().ForEach(e => db.TemporaryInventory.DeleteObject(e));
				db.SaveChanges();
			}
        }


		public void Insert(TemporaryInventory temporaryInventory, string pathDB)
		{
			if (temporaryInventory == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entity = temporaryInventory.ToEntity();
				db.TemporaryInventory.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(TemporaryInventorys temporaryInventorys, string pathDB)
		{
			if (temporaryInventorys == null) return;
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				foreach (TemporaryInventory temporaryInventory in temporaryInventorys)
				{
					if (temporaryInventory == null) continue;
					var entity = temporaryInventory.ToEntity();
					db.TemporaryInventory.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		public void Update(TemporaryInventorys temporaryInventorys, string pathDB)
		{
			if (temporaryInventorys == null) return;

			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				foreach (TemporaryInventory temporaryInventory in temporaryInventorys)
				{
					if (temporaryInventory == null) continue;
					var entity = this.GetEntityByID(db, temporaryInventory.Id);
					if (entity == null) continue;
					entity.ApplyChanges(temporaryInventory);
				}
				db.SaveChanges();
			}
		}


		public void Update(TemporaryInventory temporaryInventory, string pathDB)
		{
			if (temporaryInventory == null) return;

			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{
				var entity = this.GetEntityByID(db, temporaryInventory.Id);
					if (entity == null) return;
					entity.ApplyChanges(temporaryInventory);
				db.SaveChanges();
			}
		}


		public Dictionary<string, TemporaryInventory> GetDictionaryTemporaryInventorys(string pathDB, string domain = "InventProduct", 
			string operation = "DELETE")
		{
			Dictionary<string, TemporaryInventory> dictionary = new Dictionary<string, TemporaryInventory>();
			string connectionstring =  this.BuildAnalyticDBConnectionString(pathDB);
			using (var db = new App_Data.AnalyticDB(connectionstring) )
			{
				try
				{
					var temporaryInventorys = db.TemporaryInventory.
					Where(x => x.Domain.Contains(domain) == true && x.Operation == operation).ToList().Select(e => e.ToDomainObject()); 

									
					foreach (TemporaryInventory temporaryInventory in temporaryInventorys.OrderBy(x=>x.DateModified))
					{
						if (temporaryInventory != null)
						{
							if (operation == "DELETE")
							{
								if (string.IsNullOrWhiteSpace(temporaryInventory.OldUid) == false)
								{
									dictionary[temporaryInventory.OldUid] = temporaryInventory;
								}
							}
							else if (operation == "INSERT")
							{
								if (string.IsNullOrWhiteSpace(temporaryInventory.NewUid) == false)
								{
									dictionary[temporaryInventory.NewUid] = temporaryInventory;
								}
							}
						}
					}
				}
				catch { }
			}
			if (dictionary == null) return new Dictionary<string, TemporaryInventory>();

			return dictionary;
		}


		public Dictionary<string, TemporaryInventory> GetDictionaryDeletedByNewUidTemporaryInventorys(string pathDB, 
			string domain = "InventProduct")
		{
			Dictionary<string, TemporaryInventory> dictionary = new Dictionary<string, TemporaryInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{

				try
				{
					var temporaryInventorys = db.TemporaryInventory.
					Where(x => x.Domain.Contains(domain) == true && x.Operation == "DELETE").ToList().Select(e => e.ToDomainObject()); 

					foreach (TemporaryInventory temporaryInventory in temporaryInventorys)
					{
						if (temporaryInventory != null)
						{
							if (string.IsNullOrWhiteSpace(temporaryInventory.NewUid) == false)
							{
								//serialNumber, makat, locationCodeFrom, propertyStrKey8
								string newUid = temporaryInventory.NewUid;
								string[] keys = SplitUID(newUid);
								temporaryInventory.NewSerialNumber = keys[0];
								temporaryInventory.NewItemCode = keys[1];
								temporaryInventory.NewLocationCode = keys[2];
								temporaryInventory.NewKey = keys[3];

								string oldUid = temporaryInventory.OldUid;
								string[] keys1 = SplitUID(oldUid);
								temporaryInventory.OldSerialNumber = keys1[0];
								temporaryInventory.OldItemCode = keys1[1];
								temporaryInventory.OldLocationCode = keys1[2];
								temporaryInventory.OldKey = keys1[3];

								if (string.IsNullOrWhiteSpace(temporaryInventory.DateModified) == false)
								{
									DateTime dt = new DateTime(temporaryInventory.DateModified.GetNullableValue<long>().GetValueOrDefault())
										.ConvertFromAndroidTime();
									temporaryInventory.Device = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_" + dt.ToString("HH") + "_" + dt.ToString("mm") + "_" + dt.ToString("ss");
								}

								dictionary[temporaryInventory.NewUid] = temporaryInventory;
							}
						}
					}
				}
				catch { }
			}

			if (dictionary == null) return new Dictionary<string, TemporaryInventory>();

			return dictionary;
		}


		public void FillKeysNewUidTemporaryInventorys(string pathDB, string domain = "InventProduct")
		{
			TemporaryInventorys temporaryInventorys = new TemporaryInventorys();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{

				try
				{
					var tempInventorys = db.TemporaryInventory.
					Where(x => x.Domain.Contains(domain) == true && x.Operation == "DELETE").ToList().Select(e => e.ToDomainObject());

					foreach (TemporaryInventory temporaryInventory in tempInventorys)
					{
						if (temporaryInventory != null)
						{
							if (string.IsNullOrWhiteSpace(temporaryInventory.NewUid) == false)
							{
								// Nativ +
								//SerialNumberLocal|ItemCode|LocationCode|PropertyStr13
								string newUid = temporaryInventory.NewUid;
								string[] keys = SplitUID(newUid);
								temporaryInventory.NewSerialNumber = keys[0];
								temporaryInventory.NewItemCode = keys[1];
								temporaryInventory.NewLocationCode = keys[2];
								temporaryInventory.NewKey = keys[3];
								temporaryInventory.NewProductCode = ""; //будем хранить ключ восстановления


								string oldUid = temporaryInventory.OldUid;
								string[] keys1 = SplitUID(oldUid);
								temporaryInventory.OldSerialNumber = keys1[0];
								temporaryInventory.OldItemCode = keys1[1];
								temporaryInventory.OldLocationCode = keys1[2];
								temporaryInventory.OldKey = keys1[3];

								if (string.IsNullOrWhiteSpace(temporaryInventory.DateModified) == false)
								{
									DateTime dt = new DateTime(temporaryInventory.DateModified.GetNullableValue<long>().GetValueOrDefault())
										.ConvertFromAndroidTime();
									temporaryInventory.Device = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_" + dt.ToString("HH") + "_" + dt.ToString("mm") + "_" + dt.ToString("ss");
								}
								temporaryInventorys.Add(temporaryInventory);
							}
						}
					}
				}
				catch { }
			}

			Update(temporaryInventorys, pathDB);

		
		}


		public List<TemporaryInventory> GetTemporaryInventorysInventProduct(string pathDB, string domain = "InventProduct", string operation = "DELETE")
		{
			List<TemporaryInventory> temporaryInventorys = new List<TemporaryInventory>();
			using (var db = new App_Data.AnalyticDB(this.BuildAnalyticDBConnectionString(pathDB)))
			{

				try
				{
					var tempInventorys = db.TemporaryInventory.
					Where(x => x.Domain.Contains(domain) == true && x.Operation == operation).ToList().Select(e => e.ToDomainObject());

					foreach (TemporaryInventory temporaryInventory in tempInventorys)
					{
						if (temporaryInventory != null)
						{
							if (string.IsNullOrWhiteSpace(temporaryInventory.NewUid) == false)
							{
								// Nativ +
								//SerialNumberLocal|ItemCode|LocationCode|PropertyStr13
								string newUid = temporaryInventory.NewUid;
								if (string.IsNullOrWhiteSpace(newUid) == false)
								{
									string[] keys = SplitUID(newUid);
									temporaryInventory.NewSerialNumber = keys[0];
									temporaryInventory.NewItemCode = keys[1];
									temporaryInventory.NewLocationCode = keys[2];
									temporaryInventory.NewKey = keys[3];
									temporaryInventory.NewProductCode = ""; //будем хранить ключ восстановления
									temporaryInventory.Tag = "";
								}

								string oldUid = temporaryInventory.OldUid;
								string[] keys1 = SplitUID(oldUid);
								temporaryInventory.OldSerialNumber = keys1[0];
								temporaryInventory.OldItemCode = keys1[1];
								temporaryInventory.OldLocationCode = keys1[2];
								temporaryInventory.OldKey = keys1[3];

								if (string.IsNullOrWhiteSpace(temporaryInventory.DateModified) == false)
								{
									DateTime dt = new DateTime(temporaryInventory.DateModified.GetNullableValue<long>().GetValueOrDefault())
										.ConvertFromAndroidTime();
									temporaryInventory.Device = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_" + dt.ToString("HH") + "_" + dt.ToString("mm") + "_" + dt.ToString("ss");
									string key = temporaryInventory.NewUid + "-" + temporaryInventory.OldUid + "-" + temporaryInventory.Device;
									temporaryInventory.Description = key;
								}

								if (string.IsNullOrWhiteSpace(newUid) == false)
								{
									temporaryInventorys.Add(temporaryInventory);
								}
							}
						}
					}

					var domainObjects = from e in temporaryInventorys
										orderby e.Device descending
										//	group e by e.Description into g			//1Variant
										group e by e.OldUid into g				//2Variant
										select new TemporaryInventory
										{
											//	Description = g.Key,			 //1Variant
											OldUid = g.Key,						 //2Variant
											Device = g.Max(x => x.Device),	 //Datemodify
											Domain = g.Max(x => x.Domain),
											NewItemCode = g.Max(x => x.NewItemCode),
											NewKey = g.Max(x => x.NewKey),
											NewLocationCode = g.Max(x => x.NewLocationCode),
											NewProductCode = g.Max(x => x.NewProductCode),
											NewSerialNumber = g.Max(x => x.NewSerialNumber),
											NewUid = g.Max(x => x.NewUid),
											OldItemCode = g.Max(x => x.OldItemCode),
											OldKey = g.Max(x => x.OldKey),
											OldLocationCode = g.Max(x => x.OldLocationCode),
											OldProductCode = g.Max(x => x.OldProductCode),
											OldSerialNumber = g.Max(x => x.OldSerialNumber),
											//OldUid = g.Max(x => x.OldUid),
											Operation = g.Max(x => x.Operation),
											Tag = g.Max(x => x.Tag)
											//DbFileName = g.Count().ToString()
										};
					temporaryInventorys = domainObjects.ToList();
				}
				catch { }

			}
			return temporaryInventorys;


		}
		//взвращает отсортированный список по GUI и по дате модификации
		//берем старшую дату 
		//которая должна оказаться на 1 месте
		public List<TemporaryInventory> GetListDeletedByNewUidTemporaryInventorys(string pathDB, string newID, List<TemporaryInventory> temporaryInventorysFromDB)
		{
			List<TemporaryInventory> temporaryInventorList = new List<TemporaryInventory>();
			//string analyticDBFile = _dbSettings.AnalyticDBFile;
			//string connectionString = _connectionADO.GetADOConnectionStringBySubFolder(pathDB, analyticDBFile);
			//SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			//AnalyticDBContext db = new AnalyticDBContext(sqlCeConnection);

			try
			{
				//List<TemporaryInventory> temporaryInventorys = db.TemporaryInventoryDatas.
				//Where(x => x.Domain.Contains("InventProduct") == true && x.Operation == "DELETE"
				//	&& x.NewUid == newID && x.Tag != "Recover").ToList();	   //еще не поучаствовали в восстановлении

				List<TemporaryInventory> temporaryInventorys = temporaryInventorysFromDB.
				Where(x => x.NewUid == newID && x.Tag != "Recover").ToList();	   //еще не поучаствовали в восстановлении

				Dictionary<string, TemporaryInventory> dictionary = new Dictionary<string, TemporaryInventory>();
			

				TemporaryInventorys listDuble = new TemporaryInventorys();
				List<TemporaryInventory> temporaryInventoryDistinct = new List<TemporaryInventory>();
				foreach (var ti in temporaryInventorys)
				{
					string key = ti.NewUid + "-" + ti.OldUid + "-" + ti.Device;
					if (dictionary.ContainsKey(key) == false)
					{
						dictionary[key] = ti;
						temporaryInventoryDistinct.Add(ti);
					}
					//else
					//{
					//	ti.NewProductCode = "Duplicate ";// +key;
					//	ti.Tag = "Recover";
					//	listDuble.Add(ti);
					//}
				}
				//Update(listDuble, pathDB); 
				temporaryInventorList = temporaryInventoryDistinct.OrderByDescending(x => x.Device).ToList();
			}
			catch (Exception ex)
			{
				string massage = ex.Message;
			}
			if (temporaryInventorList == null) return new List<TemporaryInventory>();


			return temporaryInventorList;
		}

		private string[] SplitUID(string newUid)
		{
			string[] keys = newUid.Split('|');
			string[] keysEmpty = { "", "", "", "" };
			int count = Math.Min(keys.Length, 4);
			for (var k = 0; k < count; k++)
			{
				keysEmpty[k] = keys[k];
			}
			return keysEmpty;
		}

        #endregion

		private App_Data.TemporaryInventory GetEntityByID(App_Data.AnalyticDB dc, long id)
		{
			var entity = dc.TemporaryInventory.FirstOrDefault(e => e.Id.CompareTo(id) == 0);
			return entity;
		}
	}
}
