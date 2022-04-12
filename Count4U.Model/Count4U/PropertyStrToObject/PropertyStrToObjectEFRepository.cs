using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class PropertyStrToObjectEFRepository : BaseEFRepository, IPropertyStrToObjectRepository
    {
		private Dictionary<string, PropertyStrToObject> _propertyStrToObjectDictionary;
		private readonly IServiceLocator _serviceLocator;

		public PropertyStrToObjectEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
            : base(connection)
        {
			this._propertyStrToObjectDictionary = new Dictionary<string, PropertyStrToObject>();
			this._serviceLocator = serviceLocator;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

		#region IPropertyStrToObjectRepository Members

		public PropertyStrToObjects GetPropertyStrToObjects(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.PropertyStrToObjects.ToList().Select(e => e.ToDomainObject());
				return PropertyStrToObjects.FromEnumerable(domainObjects);
            }
        }

		public PropertyStrToObjects GetPropertyStrToObjects(int topCount, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.PropertyStrToObjects.Take(topCount).ToList().Select(e => e.ToDomainObject());
				return PropertyStrToObjects.FromEnumerable(domainObjects);
			}
		}

		public PropertyStrToObjects GetPropertyStrToObjects(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetPropertyStrToObjects(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.PropertyStrToObjects), db.PropertyStrToObjects.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = PropertyStrToObjects.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(PropertyStrToObject propertyStrToObject, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, propertyStrToObject.PropertyStrCode);
				if (entity == null) return;
				db.PropertyStrToObjects.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void DeleteAll(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				db.PropertyStrToObjects.ToList().ForEach(e => db.PropertyStrToObjects.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public void Insert(PropertyStrToObject propertyStrToObject, string pathDB)
        {
			if (propertyStrToObject == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = propertyStrToObject.ToEntity();
				db.PropertyStrToObjects.AddObject(entity);
                db.SaveChanges();
            }
        }

		public void Update(PropertyStrToObject propertyStrToObject, string pathDB)
        {
			if (propertyStrToObject == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, propertyStrToObject.PropertyStrCode);
				if (entity == null) return;
				entity.ApplyChanges(propertyStrToObject);
                db.SaveChanges();
            }
        }


		public PropertyStrToObject GetPropertyStrToObjectByCode(string code, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}




        #endregion

		#region Dictionary

		public Dictionary<string, PropertyStrToObject> GetPropertyStrToObjectDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearPropertyStrToObjectDictionary();
				this.FillPropertyStrToObjectDictionary(pathDB);
			}
			return this._propertyStrToObjectDictionary;
		}

		public void ClearPropertyStrToObjectDictionary()
		{
			this._propertyStrToObjectDictionary.Clear();
			GC.Collect();
		}

		public void AddPropertyStrToObjectInDictionary(string code, PropertyStrToObject propertyStrToObject)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._propertyStrToObjectDictionary.ContainsKey(code) == false)
			{
				this._propertyStrToObjectDictionary.Add(code, propertyStrToObject);
			}
		}

		public void RemovePropertyStrToObjectFromDictionary(string code)
		{
			try
			{
				this._propertyStrToObjectDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistPropertyStrToObjectInDictionary(string code)
		{
			if (this._propertyStrToObjectDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public PropertyStrToObject GetPropertyStrToObjectByCodeFromDictionary(string code)
		{
			if (this._propertyStrToObjectDictionary.ContainsKey(code) == true)
			{
				return this._propertyStrToObjectDictionary[code];
			}
			return null;
		}

		public void FillPropertyStrToObjectDictionary(string pathDB)
		{
			this.ClearPropertyStrToObjectDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					PropertyStrToObjects propertyStrToObjects = this.GetPropertyStrToObjects(pathDB);
					//this._locationDictionary = db.Locations.Select(e => new Location
					//{
					//    Code = e.Code,
					//    Name = e.Name,
					//    BackgroundColor = e.BackgroundColor,
					//    Description = e.Description
					//}).Distinct().ToDictionary(k => k.Code);

					this._propertyStrToObjectDictionary = propertyStrToObjects.Select(e => e).Distinct().ToDictionary(k => k.PropertyStrCode); 
					//foreach (var i in this._locationDictionary)
					//{
					//    var f = i.Key;
					//    var f1 = i.Value;
					//}
				}
				catch { }
			}
		}

		public List<string> GetPropertyStrToObjectsCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.PropertyStrToObjects.Select(e => e.PropertyStrCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetPropertyStrToObjectCodeList", exp);
				}
			}
			return ret;
		}

		#endregion

        #region private

		private App_Data.PropertyStrToObject GetEntityByCode(App_Data.Count4UDB db, string code)
		{
			var entity = db.PropertyStrToObjects.FirstOrDefault(e => e.PropertyStrCode.CompareTo(code) == 0);
			return entity;
		}


		//private App_Data.PropertyStrToObject GetEntityByName(App_Data.Count4UDB db, string name)
		//{
		//	var entity = db.PropertyStrToObjects.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
		//	return entity;
		//}

	

	    #endregion
    }
}
