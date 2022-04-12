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
	public class FamilyEFRepository : BaseEFRepository, IFamilyRepository
    {
		private Dictionary<string, Family> _familyDictionary;
		private readonly IServiceLocator _serviceLocator;

		public FamilyEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
			: base(connection)												   
        {
			this._familyDictionary = new Dictionary<string, Family>();
			this._serviceLocator = serviceLocator;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

		#region IFamilyRepository Members

		public Familys GetFamilys(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.Family.ToList().Select(e => e.ToDomainObject());
				return Familys.FromEnumerable(domainObjects);
            }
        }

		public Familys GetFamilys(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetFamilys(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Family), db.Family.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = Familys.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(Family family, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, family.FamilyCode);
				if (entity == null) return;
				db.Family.DeleteObject(entity);
				db.SaveChanges();
			}
		}

		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.Family.ToList().ForEach(e => db.Family.DeleteObject(e));
				db.SaveChanges();
			}
		}

		public void Insert(Family family, string pathDB)
		{
			if (family == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = family.ToEntity();
				db.Family.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(Dictionary<string, Family> dictionaryFamily, string pathDB)
		{
			this.GetFamilyDictionary(pathDB, true);

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (KeyValuePair<string, Family> keyValuePair in dictionaryFamily)
				{
					if (this._familyDictionary.ContainsKey(keyValuePair.Key) == false)
					{
						if (keyValuePair.Value != null)
						{
							var entity = keyValuePair.Value.ToEntity();
							db.Family.AddObject(entity);
						}
					}
				}
				db.SaveChanges();
			}
		}

		public void Update(Family family, string pathDB)
		{
			if (family == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, family.FamilyCode);
				if (entity == null) return;
				entity.ApplyChanges(family);
				db.SaveChanges();
			}
		}

		public Family GetFamilyByName(string name, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByName(db, name);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public Family GetFamilyByCode(string code, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

        public int GetFamilysTotal(string pathDB)
        {
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				return db.Family.Count();
            }
        }

		public List<string> GetFamilysCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Family.Select(e => e.FamilyCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetFamilysCodeList", exp);
				}
			}
			return ret;
		}

		public void RepairCodeFromDB(string pathDB)
		{
			IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
			List<string> familyCodeListFromProduct = productRepository.GetFamilyCodeList(pathDB);			//из
			List<string> familyCodeListFromFamily = this.GetFamilysCodeList(pathDB); //в
			Dictionary<string, string> difference = new Dictionary<string, string>();

			foreach (var familyCodeFromProduct in familyCodeListFromProduct)			   //из
			{
				if (familyCodeListFromFamily.Contains(familyCodeFromProduct) == false)		 //в
				{
					difference[familyCodeFromProduct] = familyCodeFromProduct;
				}
			}

			foreach (KeyValuePair<string, string> keyValuePair in difference)
			{
				if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
				{
					Model.Count4U.Family newFamily = new Model.Count4U.Family();
					newFamily.FamilyCode = keyValuePair.Value;
					newFamily.Name = keyValuePair.Value;
					newFamily.Description = "Repair from Product";
					this.Insert(newFamily, pathDB);
				}
			}
		}

        #endregion

		#region Dictionary

		public Dictionary<string, Family> GetFamilyDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearFamilyDictionary();
				this.FillFamilyDictionary(pathDB);
			}
			return this._familyDictionary;
		}

		public Dictionary<string, Family> GetFamilyDictionary_DescriptionKey(string pathDB)
		{
			Dictionary<string, Family> familyDictionary = new Dictionary<string, Family>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Familys familys = this.GetFamilys(pathDB);

					familyDictionary = familys.Select(e => e).Distinct().ToDictionary(k => k.Description);
				}
				catch { }
			}

			return familyDictionary;
		}

		public void ClearFamilyDictionary()
		{
			this._familyDictionary.Clear();
			GC.Collect();
		}

		public void AddFamilyInDictionary(string code, Family location)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._familyDictionary.ContainsKey(code) == false)
			{
				this._familyDictionary.Add(code, location);
			}
		}

		public void RemoveFamilyFromDictionary(string code)
		{
			try
			{
				this._familyDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistFamilyInDictionary(string code)
		{
			if (this._familyDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public Family GetFamilyByCodeFromDictionary(string code)
		{
			if (this._familyDictionary.ContainsKey(code) == true)
			{
				return this._familyDictionary[code];
			}
			return null;
		}

		public void FillFamilyDictionary(string pathDB)
		{
			this.ClearFamilyDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Familys familys = this.GetFamilys(pathDB);
					this._familyDictionary = familys.Select(e => e).Distinct().ToDictionary(k => k.FamilyCode); 
					
				}
				catch { }
			}
		}
         #endregion

		#region private

		private App_Data.Family GetEntityByCode(App_Data.Count4UDB db, string familyCode)
		{
			var entity = db.Family.FirstOrDefault(e => e.FamilyCode.CompareTo(familyCode) == 0);
			return entity;
		}


		private App_Data.Family GetEntityByName(App_Data.Count4UDB db, string name)
		{
			var entity = db.Family.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
			return entity;
		}

		#endregion

        public bool IsAnyInDb(string pathDB)
        {
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                try
                {
					long n = db.Family.LongCount();
					if (n > 0) return true;
					else return false;
                }
                catch { }
            }

            return false;
        }
	}
}
