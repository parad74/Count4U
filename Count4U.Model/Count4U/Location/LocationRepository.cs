using System;
using System.Linq;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.Mapping;
using System.Collections.Generic;

namespace Count4U.Model.Count4U
{
    public class LocationRepository : ILocationRepository
    {
 
        private Locations _locationsList;

        #region ILocationRepository Members

		public Locations GetLocations(string pathDB)
		{
			if (this._locationsList == null)
			{
				this._locationsList = new Locations {
		                   new Location {Name="Location1", Code = "LocationCode1", BackgroundColor="29, 201, 16"},
		                   new Location {Name="Location2", Code = "LocationCode2", BackgroundColor="49, 201, 255"},
		                   new Location {Name="Location3", Code = "LocationCode3", BackgroundColor="249, 20, 16"},
		                   new Location {Name="Location12", Code = "LocationCode12", BackgroundColor="29, 1, 255"},
   		                   new Location {Name="Location7", Code = "LocationCode7", BackgroundColor="24, 21, 109"},
		                   new Location {Name="Location6", Code = "LocationCode6", BackgroundColor="249, 201, 16"}
		               };
			}
			return this._locationsList;
		}

		public Locations GetLocations(SelectParams selectParams, string pathDB)
        {
            throw new NotImplementedException();
        }

		public void Delete(Location location, string pathDB)
        {
			var entity = this.GetEntityByName(location.Name, pathDB);
			if (entity == null) return;
            this.GetLocations(pathDB).Remove(entity);
        }

		public void DeleteAll(string pathDB)
        {
            this.GetLocations(pathDB).Clear();
        }

		public void Insert(Location location, string pathDB)
        {
			if (location == null) return;
            var entity = location.ToEntity();
            GetLocations(pathDB).Add(entity);
        }

		public void Update(Location location, string pathDB)
        {
			if (location == null) return;
			var entity = this.GetEntityByName(location.Name, pathDB);
			if (entity == null) return;
            entity.ApplyChanges(location);
        }

		public Location GetLocationByName(string name, string pathDB)
        {
			var entity = this.GetEntityByName(name, pathDB);
           // return entity.ToDomainObject();
		    return entity;
        }

		#endregion

        #region private

		private Location GetEntityByName(string name, string pathDB)
        {
            return GetLocations(pathDB).First(e => e.Name.CompareTo(name) == 0);
        }

        #endregion



		#region ILocationRepository Members


		public Location GetLocationByCode(string code, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public System.Collections.Generic.Dictionary<string, Location> GetLocationDictionary(string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public void ClearLocationDictionary()
		{
			throw new NotImplementedException();
		}

		public void AddLocationInDictionary(string code, Location location)
		{
			throw new NotImplementedException();
		}

		public void RemoveLocationFromDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public bool IsExistLocationInDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public Location GetLocationByCodeFromDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public void FillLocationDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public System.Collections.Generic.List<string> GetLocationCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public void RepairCodeFromDB(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion


		public string GetFistLocationCodeWithoutIturs(string pathDB)
		{
			throw new NotImplementedException();
		}


		public Locations GetLocations(int topCount, string pathDB)
		{
			throw new NotImplementedException();
		}

		#region ILocationRepository Members


		public void Update(Locations locations, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public System.Collections.Generic.List<string> GetLocationCodeListByTag(string pathDB, string tag)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public Locations GetLocationListByTag(string pathDB, string tag)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public System.Collections.Generic.List<string> GetLocationCodeListIncludedTag(string pathDB, string tag)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public string GetFistLocationCodeWithoutIturs(Locations locations, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILocationRepository Members


		public void Insert(List<Location> locations, string pathDB)
		{
			throw new NotImplementedException();
		}

		public string GetFistFromAllLocationCodeWithoutIturs(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
