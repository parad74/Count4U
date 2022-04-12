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
	public class ShelfEFRepository : BaseEFRepository, IShelfRepository
    {
		private Dictionary<string, Shelf> _shelfDictionary;
		private readonly IServiceLocator _serviceLocator;

		public ShelfEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
			: base(connection)												   
        {
			this._shelfDictionary = new Dictionary<string, Shelf>();
			this._serviceLocator = serviceLocator;
        }

		//#region BaseEFRepository Members

		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}

		public Shelfs GetShelves(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.Shelf.ToList().Select(e => e.ToDomainObject());
				return Shelfs.FromEnumerable(domainObjects);
			}
		}

		public Shelfs GetShelvesByIturCode(string iturCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = AsQueryable(db.Shelf).Where(
					e => e.IturCode.CompareTo(iturCode) == 0)
					.ToList().Select(e => e.ToDomainObject());
				return Shelfs.FromEnumerable(domainObjects);
			}
		}

		public Shelfs GetShelves(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetShelves(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Shelf), db.Shelf.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = Shelfs.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(Shelf shelf, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, shelf.ShelfCode);
				if (entity == null) return;
				db.Shelf.DeleteObject(entity);
				db.SaveChanges();
			}
		}

		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.Shelf.ToList().ForEach(e => db.Shelf.DeleteObject(e));
				db.SaveChanges();
			}
		}

		public void Insert(Shelf shelf, string pathDB)
		{
			if (shelf == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = shelf.ToEntity();
				db.Shelf.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(Dictionary<string, Shelf> dictionaryShelf, string pathDB)
		{
			this.GetShelfDictionary(pathDB, true);

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (KeyValuePair<string, Shelf> keyValuePair in dictionaryShelf)
				{
					if (this._shelfDictionary.ContainsKey(keyValuePair.Key) == false)
					{
						if (keyValuePair.Value != null)
						{
							var entity = keyValuePair.Value.ToEntity();
							db.Shelf.AddObject(entity);
						}
					}
				}
				db.SaveChanges();
			}
		}

		public void Update(Shelf shelf, string pathDB)
		{
			if (shelf == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, shelf.ShelfCode);
				if (entity == null) return;
				//shelf.Area = shelf.Height * shelf.Width;
				shelf.CreateDataTime = DateTime.Now;
				entity.ApplyChanges(shelf);
				db.SaveChanges();
			}
		}

		public Shelf GetShelfByName(int num, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByShelfNum(db, num);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public Shelf GetShelfByCode(string code, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public int GetShelfsTotal(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				return db.Shelf.Count();
			}
		}

		public List<string> GetShelfsCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Shelf.Select(e => e.ShelfCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetShelfCodeList", exp);
				}
			}
			return ret;
		}

		//public void RepairCodeFromDB(string pathDB)
		//{
		//	IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
		//	List<string> familyCodeListFromProduct = productRepository.GetFamilyCodeList(pathDB);			//из
		//	List<string> familyCodeListFromFamily = this.GetFamilysCodeList(pathDB); //в
		//	Dictionary<string, string> difference = new Dictionary<string, string>();

		//	foreach (var familyCodeFromProduct in familyCodeListFromProduct)			   //из
		//	{
		//		if (familyCodeListFromFamily.Contains(familyCodeFromProduct) == false)		 //в
		//		{
		//			difference[familyCodeFromProduct] = familyCodeFromProduct;
		//		}
		//	}

		//	foreach (KeyValuePair<string, string> keyValuePair in difference)
		//	{
		//		if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
		//		{
		//			Model.Count4U.Family newFamily = new Model.Count4U.Family();
		//			newFamily.FamilyCode = keyValuePair.Value;
		//			newFamily.Name = keyValuePair.Value;
		//			newFamily.Description = "Repair from Product";
		//			this.Insert(newFamily, pathDB);
		//		}
		//	}
		//}

		//#endregion

		#region Dictionary

		public Dictionary<string, Shelf> GetShelfDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearShelfDictionary();
				this.FillShelfDictionary(pathDB);
			}
			return this._shelfDictionary;
		}

		public void ClearShelfDictionary()
		{
			this._shelfDictionary.Clear();
			GC.Collect();
		}

		public void AddShelfInDictionary(string code, Shelf shelf)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._shelfDictionary.ContainsKey(code) == false)
			{
				this._shelfDictionary.Add(code, shelf);
			}
		}

		public void RemoveShelfFromDictionary(string code)
		{
			try
			{
				this._shelfDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistShelfInDictionary(string code)
		{
			if (this._shelfDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public Shelf GetShelfByCodeFromDictionary(string code)
		{
			if (this._shelfDictionary.ContainsKey(code) == true)
			{
				return this._shelfDictionary[code];
			}
			return null;
		}

		public void FillShelfDictionary(string pathDB)
		{
			this.ClearShelfDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Shelfs shelfs = this.GetShelves(pathDB);
					this._shelfDictionary = shelfs.Select(e => e).Distinct().ToDictionary(k => k.ShelfCode);

				}
				catch { }
			}
		}
		#endregion

		#region private

		private App_Data.Shelf GetEntityByCode(App_Data.Count4UDB db, string shelfCode)
		{
			var entity = db.Shelf.FirstOrDefault(e => e.ShelfCode.CompareTo(shelfCode) == 0);
			return entity;
		}


		private App_Data.Shelf GetEntityByShelfNum(App_Data.Count4UDB db, int shelfNum)
		{
			var entity = db.Shelf.FirstOrDefault(e => e.ShelfNum == shelfNum);
			return entity;
		}

		#endregion

		public bool IsAnyInDb(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					long n = db.Shelf.LongCount();
					if (n > 0) return true;
					else return false;
				}
				catch { }
			}

			return false;
		}
	}
}
