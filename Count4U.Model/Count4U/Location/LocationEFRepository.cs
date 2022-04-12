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
    public class LocationEFRepository : BaseEFRepository, ILocationRepository
    {
		private Dictionary<string, Location> _locationDictionary;
 
		public LocationEFRepository(IConnectionDB connection)
            : base(connection)
        {
			this._locationDictionary = new Dictionary<string, Location>();
		}

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region ILocationRepository Members

        public Locations GetLocations(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.Locations.ToList().Select(e => e.ToDomainObject());
                return Locations.FromEnumerable(domainObjects);
            }
        }

		public Locations GetLocations(int topCount, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.Locations.Take(topCount).ToList().Select(e => e.ToDomainObject());
				return Locations.FromEnumerable(domainObjects);
			}
		}

		public Locations GetLocations(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetLocations(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Locations), db.Locations.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = Locations.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(Location location, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, location.Code);
				if (entity == null) return;
                db.Locations.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void DeleteAll(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                db.Locations.ToList().ForEach(e => db.Locations.DeleteObject(e));
                db.SaveChanges();
            }
        }

        public void Insert(Location location, string pathDB)
        {
			if (location == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = location.ToEntity();
                db.Locations.AddObject(entity);
                db.SaveChanges();
            }
        }

		public void Insert(List<Location> locations, string pathDB)
		{
			if (locations == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (Location location in locations)
				{
					var entity = location.ToEntity();
					db.Locations.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		public void Update(Location location, string pathDB)
        {
			if (location == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, location.Code);
				if (entity == null) return;
                entity.ApplyChanges(location);
                db.SaveChanges();
            }
        }

		public void Update(Locations locations, string pathDB)
		{
			if (locations == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (Location location in locations)
				{
					if (location == null) continue;
					var entity = this.GetEntityByCode(db, location.Code);
					if (entity == null) continue;
					entity.ApplyChanges(location);
				}
				db.SaveChanges();
			}
		}

        public Location GetLocationByName(string name, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByName(db, name);
				if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

		public Location GetLocationByCode(string code, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public string  GetFistLocationCodeWithoutIturs(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<App_Data.Location> entities = db.Locations.OrderBy(e => e.Code).Select(e => e).ToList();
				foreach (var entety in entities)
				{
					int countItursInLocation = db.Iturs.Where(x => x.LocationCode.CompareTo(entety.Code) == 0).Count();
					if (countItursInLocation == 0) return entety.Code;
				}
				var entety1 = entities.FirstOrDefault();
				if (entety1 == null) return "";
				return entety1.Code;
				
			}
		}


		public string GetFistFromAllLocationCodeWithoutIturs(string pathDB)
		{
			Dictionary<string, int> dictionary = GetIturTotal0GroupByAllLocationCode(pathDB);
			foreach (var pair in dictionary)
			{
				if (pair.Value == 0) return pair.Key;
			}

			if (dictionary.Count() == 0)
			{
				return "";
			}
			else
			{
				foreach (var pair in dictionary)
				{
					return pair.Key;
				}
			}
			return "";
		}

		public string GetFistLocationCodeWithoutIturs(Locations locations, string pathDB)
		{
			Dictionary<string, int> dictionary = GetIturTotal0GroupByLocationCode(locations, pathDB);
			foreach (var pair in dictionary)
			{
				if (pair.Value == 0) return pair.Key;
			}
			var entety = locations.FirstOrDefault();
			if (entety == null) return "";
			return entety.Code;

		}

	


		private Dictionary<string, int> GetIturTotal0GroupByAllLocationCode(string pathDB)			
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					List<string> locationCodeListFromItur = db.Iturs.Select(x => x.LocationCode).Distinct().ToList();
					List<string> locationCodeList = db.Locations.Select(x => x.Code).Distinct().ToList();
					foreach (var locationCode in locationCodeList)
					{
						if (locationCodeListFromItur.Contains(locationCode)) continue;
  						result[locationCode] = 0;
					}

				}
				catch { }
			}

			return result;
		}


		private Dictionary<string, int> GetIturTotal0GroupByLocationCode(Locations locations,string pathDB)
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					List<string> locationCodeListFromItur = db.Iturs.Select(x => x.LocationCode).Distinct().ToList();
					List<string> locationCodeList = locations.Select(x => x.Code).Distinct().ToList();
					foreach (var locationCode in locationCodeList)
					{
						if (locationCodeListFromItur.Contains(locationCode)) continue;
						result[locationCode] = 0;
					}
				}
				catch { }
			}

			return result;
		}

		#endregion

		#region Dictionary

		public Dictionary<string, Location> GetLocationDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearLocationDictionary();
				this.FillLocationDictionary(pathDB);
			}
			return this._locationDictionary;
		}

		public void ClearLocationDictionary()
		{
			this._locationDictionary.Clear();
			GC.Collect();
		}

		public void AddLocationInDictionary(string code, Location location)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._locationDictionary.ContainsKey(code) == false)
			{
				this._locationDictionary.Add(code, location);
			}
		}

		public void RemoveLocationFromDictionary(string code)
		{
			try
			{
				this._locationDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistLocationInDictionary(string code)
		{
			if (this._locationDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public Location GetLocationByCodeFromDictionary(string code)
		{
			if (this._locationDictionary.ContainsKey(code) == true)
			{
				return this._locationDictionary[code];
			}
			return null;
		}

		public void FillLocationDictionary(string pathDB)
		{
			this.ClearLocationDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Locations locations = this.GetLocations(pathDB);
					//this._locationDictionary = db.Locations.Select(e => new Location
					//{
					//    Code = e.Code,
					//    Name = e.Name,
					//    BackgroundColor = e.BackgroundColor,
					//    Description = e.Description
					//}).Distinct().ToDictionary(k => k.Code);

					this._locationDictionary = locations.Select(e => e).Distinct().ToDictionary(k => k.Code); 
					//foreach (var i in this._locationDictionary)
					//{
					//    var f = i.Key;
					//    var f1 = i.Value;
					//}
				}
				catch { }
			}
		}

		#endregion

        #region private

		private App_Data.Location GetEntityByCode(App_Data.Count4UDB db, string code)
		{
			var entity = db.Locations.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
			return entity;
		}


        private App_Data.Location GetEntityByName(App_Data.Count4UDB db, string name)
        {
            var entity = db.Locations.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
            return entity;
        }

		public List<string> GetLocationCodeListByTag(string pathDB, string tag)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var locations = db.Locations.Where(x => x.Tag.CompareTo(tag) == 0).ToList();
					ret = locations.Select(e => e.Code).Distinct().ToList();
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetLocationCodeListByTag", exp);
				}
			}
			return ret;
		}


		public List<string> GetLocationCodeListIncludedTag(string pathDB, string tag)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var locations = db.Locations.Where(x => x.Tag.Contains(tag) == true).ToList();
					ret = locations.Select(e => e.Code).Distinct().ToList();
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetLocationCodeListByTag", exp);
				}
			}
			return ret;
		}


		public List<string> GetLocationCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Locations.Select(e => e.Code).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetLocationCodeList", exp);
				}
			}
			return ret;
		}

		public Locations GetLocationListByTag(string pathDB, string tag)
		{
			Locations ret = new Locations();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entities = db.Locations.Where(x => x.Tag.CompareTo(tag) == 0).ToList();
					var domainObjects = entities.Select(e => e.ToDomainObject());
					ret = Locations.FromEnumerable(domainObjects);
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetLocationCodeListByTag", exp);
				}
			}
			return ret;
		}

		public void RepairCodeFromDB(string pathDB)
		{
			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//List<string> locationCodeListFromItur = iturRepository.GetLocationCodeList(pathDB);			//из
			//List<string> locationCodeListFromLocation = this.GetLocationCodeList(pathDB); //в
			//Dictionary<string, string> difference = new Dictionary<string, string>();

			//foreach (var locationCodeFromItur in locationCodeListFromItur)			   //из
			//{
			//	if (locationCodeListFromLocation.Contains(locationCodeFromItur) == false)		 //в
			//	{
			//		difference[locationCodeFromItur] = locationCodeFromItur;
			//	}
			//}

			//foreach (KeyValuePair<string, string> keyValuePair in difference)
			//{
			//	if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
			//	{
			//		Location locationNew = new Location();
			//		locationNew.Code = keyValuePair.Value;
			//		locationNew.Name = keyValuePair.Value;
			//		//if (locationNew.Code == DomainUnknownCode.UnknownLocation)
			//		//{
			//		//    locationNew.Name = DomainUnknownName.UnknownLocation;
			//		//}
			//		locationNew.RestoreBit = true;
			//		locationNew.Description = "Repair from Itur";
			//		locationNew.Restore = DateTime.Now.ToString();
			//		this.Insert(locationNew, pathDB);
			//	}

			//}

			//Location unknownLocation = this.GetLocationByCode(DomainUnknownCode.UnknownLocation, pathDB);
			//if (unknownLocation == null)
			//{
			//	Location locationNew = new Location();
			//	locationNew.Code = DomainUnknownCode.UnknownLocation;
			//	locationNew.Name = DomainUnknownName.UnknownLocation;
			//	locationNew.Description = "Repair";
			//	this.Insert(locationNew, pathDB);
			//}
			//else
			//{
			//	unknownLocation.Name = DomainUnknownName.UnknownLocation;
			//	this.Update(unknownLocation, pathDB);
			//}

		}
        #endregion

		
	}
}
