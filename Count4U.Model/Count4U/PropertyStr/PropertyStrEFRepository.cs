using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class PropertyStrEFRepository : BaseEFRepository, IPropertyStrRepository
    {
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private readonly IServiceLocator _serviceLocator;

		public PropertyStrEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
            : base(connection)
        {
			this._propertyStrDictionary = new Dictionary<string, PropertyStr>();
			this._serviceLocator = serviceLocator;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

		#region IPropertyStrRepository Members

		public PropertyStrs GetPropertyStrs(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.PropertyStrs.ToList().Select(e => e.ToDomainObject());
				return PropertyStrs.FromEnumerable(domainObjects);
            }
        }

		public PropertyStrs GetPropertyStrs(string domainObjectType, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = AsQueryable(db.PropertyStrs).Where(
					e => e.DomainObject.CompareTo(domainObjectType) == 0)
                    .ToList().Select(e => e.ToDomainObject());
				return PropertyStrs.FromEnumerable(domainObjects);
            }
        }
		

		public PropertyStrs GetPropertyStrs(int topCount, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.PropertyStrs.Take(topCount).ToList().Select(e => e.ToDomainObject());
				return PropertyStrs.FromEnumerable(domainObjects);
			}
		}

		public PropertyStrs GetPropertyStrs(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetPropertyStrs(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.PropertyStrs), db.PropertyStrs.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = PropertyStrs.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(PropertyStr propertyStr, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, propertyStr.PropertyStrCode);
				if (entity == null) return;
				db.PropertyStrs.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void DeleteAll(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				db.PropertyStrs.ToList().ForEach(e => db.PropertyStrs.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public void DeleteAllByDomainObjectType(string domainObjectType, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
					db.PropertyStrs.Where(e => e.DomainObject.CompareTo(domainObjectType) == 0)
						.ToList().ForEach(e => db.PropertyStrs.DeleteObject(e));
					db.SaveChanges();
			}
		}

		public void Insert(PropertyStr propertyStr, string pathDB)
        {
			if (propertyStr == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = propertyStr.ToEntity();
				db.PropertyStrs.AddObject(entity);
                db.SaveChanges();
            }
        }

		public void Update(PropertyStr propertyStr, string pathDB)
        {
			if (propertyStr == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, propertyStr.PropertyStrCode);
				if (entity == null) return;
				entity.ApplyChanges(propertyStr);
                db.SaveChanges();
            }
        }

		public PropertyStr GetPropertyStrByName(string name, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByName(db, name);
				if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

		public PropertyStr GetPropertyStrByCode(string code, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		//Name =						//XPath								0		"UIDKey";
		//TypeCode = 				//Index 								1
		//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
		//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
		//DomainObject = "Profile";
		public Dictionary<string, string> GetDictionaryProfileProperty(string domainObjectType, string XPath, string pathDB)
		{
			PropertyStrs collection = GetPropertyStrs(domainObjectType, pathDB);
			var dictionary = new Dictionary<string, string>();
			string xpath = XPath.ToLower().Trim();
			foreach (PropertyStr propertyStr in collection)
			{
				if (string.IsNullOrWhiteSpace(propertyStr.TypeCode) == true) continue;
				if (propertyStr.Name.ToLower().Trim() == xpath)
				{
					int index = 4;
					bool ret = Int32.TryParse(propertyStr.TypeCode, out index);
					if (ret == true) dictionary[index.ToString()] = propertyStr.Code;
				}
			}
			return dictionary;
		}

		//DomainObject //PropertyDecorator

		//DocumentSheet 	  //TypeCode						0
		// Row 					 //PropertyStrCode 			1
		//Col 						//Code								    2
		//Name 					//значение 							3
		//string uid = propertyStr.TypeCode + "|" + propertyStr.PropertyStrCode + "|" + propertyStr.Code;
		public Dictionary<Pair<string, string, string>, PropertyStr> GetDictionaryDecoratorProperty(string domainObjectType, string pathDB)
		{
			PropertyStrs collection = GetPropertyStrs(domainObjectType, pathDB);
			var dictionary = new Dictionary<Pair<string, string, string>, PropertyStr>();
			foreach (PropertyStr propertyStr in collection)
			{																									  //futureLocationCode, futureItemCode, serialNumber
				var futureKey = new Pair<string, string, string>(propertyStr.TypeCode, propertyStr.PropertyStrCode, propertyStr.Code);
				dictionary[futureKey] = propertyStr;
			}
			return dictionary;
			//foreach (tblCurrentInventory currentIventory in tempCurrentInventories.Where(currentIventory => currentIventory.tblCatalog.ItemType == QuantityKey))
			//{
			//	var futureKey = new Pair<string, string, string>(currentIventory.tblLocation.LocationCode, currentIventory.tblCatalog.ItemCode, currentIventory.PropertyStr8);
			//	if (!dictionary3.ContainsKey(futureKey))
			//	{
			//		dictionary3.Add(futureKey, currentIventory);
			//	}
			//}
			return dictionary;
		}

		public Dictionary<string, PropertyStr> GetPropertyExportErpDecoratorProperty(string domainObjectType, string pathDB)
		{
			//DomainObject //PropertyExportErpDecorator
			//include 						//Code								 0
			//index				 	  //TypeCode						1
			// PropertyName		 //PropertyStrCode 			2
			//Title 						// Name						3   //Option
			PropertyStrs collection = GetPropertyStrs(domainObjectType, pathDB);
			var dictionary = new Dictionary<string, PropertyStr>();
			foreach (PropertyStr propertyStr in collection)
			{
				if (string.IsNullOrWhiteSpace(propertyStr.PropertyStrCode) == true) continue;
				dictionary[propertyStr.PropertyStrCode] = propertyStr;
			}
			return dictionary;
			//foreach (tblCurrentInventory currentIventory in tempCurrentInventories.Where(currentIventory => currentIventory.tblCatalog.ItemType == QuantityKey))
			//{
			//	var futureKey = new Pair<string, string, string>(currentIventory.tblLocation.LocationCode, currentIventory.tblCatalog.ItemCode, currentIventory.PropertyStr8);
			//	if (!dictionary3.ContainsKey(futureKey))
			//	{
			//		dictionary3.Add(futureKey, currentIventory);
			//	}
			//}
			return dictionary;
		}


		//Name =						//XPath								0		"UIDKey";
		//TypeCode = 				//Index 								1
		//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
		//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
		//DomainObject = "Profile";
		public string GetUIDKey_mINV(string pathDB)//string domainObjectType, string XPath, string pathDB)
		{
			//DomainObjectTypeEnum.Profile.ToString(), "UIDKey", dbPath
			string domainObjectType = "Profile";
			string XPath = "UIDKey";
			PropertyStrs collection = GetPropertyStrs(domainObjectType, pathDB);
			var dictionary = new Dictionary<int, string>();
			string xpath = XPath.ToLower().Trim();
			foreach (PropertyStr propertyStr in collection)
			{
				if (string.IsNullOrWhiteSpace(propertyStr.TypeCode) == true) continue;
				if (propertyStr.Name.ToLower().Trim() == xpath)
				{
					int index = 4;
					bool ret = Int32.TryParse(propertyStr.TypeCode, out index);			  	//TypeCode = 				//Index 		
					if (ret == true) dictionary[index] = propertyStr.PropertyStrCode;		 //PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
				}
			}

			string keyUIDstring = "";
			for (int i = 0; i < 20; i++)
			{
				if (dictionary.ContainsKey(i))
				{
					keyUIDstring = keyUIDstring + dictionary[i] + "|"; 		//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
				}
				else
				{
					break;
				}
			}
			return keyUIDstring;
		}

		//Name =						//XPath								0		"UIDKey";
		//TypeCode = 				//Index 								1
		//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
		//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
		//DomainObject = "Profile";
		public string GetUIDKey_Count4U(string pathDB)//string domainObjectType, string XPath, string pathDB)
		{
			//DomainObjectTypeEnum.Profile.ToString(), "UIDKey", dbPath
			string domainObjectType = "Profile";
			string XPath = "UIDKey";
			PropertyStrs collection = GetPropertyStrs(domainObjectType, pathDB);
			var dictionary = new Dictionary<int, string>();
			string xpath = XPath.ToLower().Trim();
			foreach (PropertyStr propertyStr in collection)
			{
				if (string.IsNullOrWhiteSpace(propertyStr.TypeCode) == true) continue;
				if (propertyStr.Name.ToLower().Trim() == xpath)
				{
					int index = 4;
					bool ret = Int32.TryParse(propertyStr.TypeCode, out index);			  	//TypeCode = 				//Index 		
					if (ret == true) dictionary[index] = propertyStr.Code;		//Code =	 //Property_Count4U			2		"Makat";   - fromCount4U
				}
			}

			string keyUIDstring = "";
			for (int i = 0; i < 20; i++)
			{
				if (dictionary.ContainsKey(i))
				{
					keyUIDstring = keyUIDstring + dictionary[i] + "|"; 		//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
				}
				else
				{
					break;
				}
			}
			return keyUIDstring;
		}



        #endregion

		#region Dictionary


		public Dictionary<string, PropertyStr> GetPropertyStrDictionary(string domainObjectType, string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearPropertyStrDictionary();
				this.FillPropertyStrDictionary(domainObjectType, pathDB);
			}
			return this._propertyStrDictionary;
		}

		public void ClearPropertyStrDictionary()
		{
			this._propertyStrDictionary.Clear();
			GC.Collect();
		}
																   
		public Dictionary<string, string> GetNamePropertyStrDictionaryByDomainAndCodeAsKey(string pathDB)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					PropertyStrs propertyStrs = this.GetPropertyStrs(pathDB);
					foreach (PropertyStr propertyStr in propertyStrs)
					{
						if (string.IsNullOrWhiteSpace(propertyStr.DomainObject) == true) continue;
						if (string.IsNullOrWhiteSpace(propertyStr.PropertyStrCode) == true) continue;
						if (string.IsNullOrWhiteSpace(propertyStr.Name) == true) continue;
						string key = propertyStr.DomainObject + "|" + propertyStr.PropertyStrCode;
						dictionary[key] = propertyStr.Name;
					}
				}
				catch { }
			}
			return dictionary;
		}

		public Dictionary<string, PropertyStr> GetPropertyStrDictionaryByDomainAndCodeAsKey(string pathDB)
		{
			Dictionary<string, PropertyStr> dictionary = new Dictionary<string, PropertyStr>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					PropertyStrs propertyStrs = this.GetPropertyStrs(pathDB);
					foreach (PropertyStr propertyStr in propertyStrs)
					{
						if (string.IsNullOrWhiteSpace(propertyStr.DomainObject) == true) continue;
						if (string.IsNullOrWhiteSpace(propertyStr.PropertyStrCode) == true) continue;
						if (string.IsNullOrWhiteSpace(propertyStr.Name) == true) continue;
						string key = propertyStr.DomainObject + "|" + propertyStr.PropertyStrCode;
						dictionary[key] = propertyStr;
					}
				}
				catch { }
			}
			return dictionary;
		}

		public Dictionary<string, string> GetPropertyStrDictionaryByCodeAsKey(string domainObjectType, string pathDB)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					PropertyStrs propertyStrs = this.GetPropertyStrs(domainObjectType, pathDB);
					foreach (PropertyStr propertyStr in propertyStrs)
					{
						dictionary[propertyStr.PropertyStrCode] = propertyStr.Name;
					}
				}
				catch { }
			}
			return dictionary;
		}
		

		public void AddPropertyStrInDictionary(string code, PropertyStr propertyStr)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._propertyStrDictionary.ContainsKey(code) == false)
			{
				this._propertyStrDictionary.Add(code, propertyStr);
			}
		}

		public void RemovePropertyStrFromDictionary(string code)
		{
			try
			{
				this._propertyStrDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistPropertyStrInDictionary(string code)
		{
			if (this._propertyStrDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public PropertyStr GetPropertyStrByCodeFromDictionary(string code)
		{
			if (this._propertyStrDictionary.ContainsKey(code) == true)
			{
				return this._propertyStrDictionary[code];
			}
			return null;
		}

		public void FillPropertyStrDictionary(string domainObjectType, string pathDB)
		{
			this.ClearPropertyStrDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					PropertyStrs propertyStrs = this.GetPropertyStrs(domainObjectType, pathDB);
					//this._locationDictionary = db.Locations.Select(e => new Location
					//{
					//    Code = e.Code,
					//    Name = e.Name,
					//    BackgroundColor = e.BackgroundColor,
					//    Description = e.Description
					//}).Distinct().ToDictionary(k => k.Code);

					this._propertyStrDictionary = propertyStrs.Select(e => e).Distinct().ToDictionary(k => k.PropertyStrCode); 
					//foreach (var i in this._locationDictionary)
					//{
					//    var f = i.Key;
					//    var f1 = i.Value;
					//}
				}
				catch { }
			}
		}


		public Dictionary<string, PropertyStr> GetFullPropertyStrDictionary( string pathDB)
		{
			Dictionary<string, PropertyStr> strDictionary = new Dictionary<string, PropertyStr>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					PropertyStrs propertyStrs = this.GetPropertyStrs(pathDB);
					strDictionary = propertyStrs.Select(e => e).Distinct().ToDictionary(k => k.DomainObject + "|" + k.PropertyStrCode);
					//foreach (var i in this._locationDictionary)
					//{
					//    var f = i.Key;
					//    var f1 = i.Value;
					//}
				}
				catch { }
			}
			return strDictionary;
		}

		#endregion

        #region private

		private App_Data.PropertyStr GetEntityByCode(App_Data.Count4UDB db, string code)
		{
			var entity = db.PropertyStrs.FirstOrDefault(e => e.PropertyStrCode.CompareTo(code) == 0);
			return entity;
		}


		private App_Data.PropertyStr GetEntityByName(App_Data.Count4UDB db, string name)
        {
			var entity = db.PropertyStrs.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
            return entity;
        }

		public List<string> GetPropertyStrsCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.PropertyStrs.Select(e => e.PropertyStrCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetPropertyStrCodeList", exp);
				}
			}
			return ret;
		}

	    #endregion

	
	}
}
