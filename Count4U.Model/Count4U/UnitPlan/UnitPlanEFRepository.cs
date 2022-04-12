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
    public class UnitPlanEFRepository : BaseEFRepository, IUnitPlanRepository
    {
		private Dictionary<string, UnitPlan> _unitPlanDictionary;
		private readonly IServiceLocator _serviceLocator;

		public UnitPlanEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
            : base(connection)
        {
			this._unitPlanDictionary = new Dictionary<string, UnitPlan>();
			this._serviceLocator = serviceLocator;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

		#region IUnitPlanRepository Members

		public UnitPlans GetUnitPlans(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.UnitPlan.ToList().Select(e => e.ToDomainObject());
				return UnitPlans.FromEnumerable(domainObjects);
            }
        }

		public UnitPlans GetUnitPlans(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetUnitPlans(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.UnitPlan), db.UnitPlan.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = UnitPlans.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(UnitPlan unitPlan, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, unitPlan.UnitPlanCode);
				if (entity == null) return;
				db.UnitPlan.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void DeleteAll(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				db.UnitPlan.ToList().ForEach(e => db.UnitPlan.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public void Insert(UnitPlan unitPlan, string pathDB)
        {
			if (unitPlan == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = unitPlan.ToEntity();
				db.UnitPlan.AddObject(entity);
                db.SaveChanges();
            }
        }

		public void Insert(Dictionary<string, UnitPlan> unitPlanToDBDictionary, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (KeyValuePair<string, UnitPlan> keyValuePair in unitPlanToDBDictionary)
				{
					//string key = keyValuePair.Key;
					UnitPlan unitPlan = keyValuePair.Value;
					if (unitPlan != null)
					{
						var entity = unitPlan.ToEntity();
						db.UnitPlan.AddObject(entity);
					}
				}
				db.SaveChanges();
			}
		}

		public void Update(UnitPlan unitPlan, string pathDB)
        {
			if (unitPlan == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, unitPlan.UnitPlanCode);
				if (entity == null) return;
				entity.ApplyChanges(unitPlan);
                db.SaveChanges();
            }
		
       }

		public long GetMaxUPCode(string pathDB)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = db.UnitPlan;
				if (entity == null) return 0;
				if (entity.Count() == 0) return 0;
				long? upNum = 0;
				upNum = db.UnitPlan.Max(e => e.StatusUnitPlanBit); // потом заменю на UPCode
				return Convert.ToInt64(upNum);
			}
		}

		public UnitPlan GetUnitPlanByUnitPlanCode(string unitPlanCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, unitPlanCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

        #endregion

		#region Dictionary

		public Dictionary<string, UnitPlan> GetUnitPlanDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearUnitPlanDictionary();
				this.FillUnitPlanDictionary(pathDB);
			}
			return this._unitPlanDictionary;
		}

		public void ClearUnitPlanDictionary()
		{
			this._unitPlanDictionary.Clear();
			GC.Collect();
		}

		public void AddUnitPlanInDictionary(string unitPlanCode, UnitPlan unitPlan)
		{
			if (string.IsNullOrWhiteSpace(unitPlanCode)) return;
			if (this._unitPlanDictionary.ContainsKey(unitPlanCode) == false)
			{
				this._unitPlanDictionary.Add(unitPlanCode, unitPlan);
			}
		}

		public void RemoveUnitPlanFromDictionary(string code)
		{
			try
			{
				this._unitPlanDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistUnitPlanInDictionary(string code)
		{
			if (this._unitPlanDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public UnitPlan GetUnitPlanByCodeFromDictionary(string code)
		{
			if (this._unitPlanDictionary.ContainsKey(code) == true)
			{
				return this._unitPlanDictionary[code];
			}
			return null;
		}


		public Dictionary<string, UnitPlan> FillUnitPlanDictionary(string pathDB)
		{
			this.ClearUnitPlanDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					UnitPlans unitPlans = this.GetUnitPlans(pathDB);
	
					this._unitPlanDictionary = unitPlans.Select(e => e).Distinct().ToDictionary(k => k.UnitPlanCode); 
				}
				catch { }
			}
			return this._unitPlanDictionary;
		}

		#endregion

        #region private

		private App_Data.UnitPlan GetEntityByCode(App_Data.Count4UDB db, string unitPlanCode)
		{
			var entity = db.UnitPlan.FirstOrDefault(e => e.UnitPlanCode.CompareTo(unitPlanCode) == 0);
			return entity;
		}


		private App_Data.UnitPlan GetEntityByName(App_Data.Count4UDB db, string name)
        {
			var entity = db.UnitPlan.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
            return entity;
        }

		public List<string> GetUnitPlanCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.UnitPlan.Select(e => e.UnitPlanCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetUnitPlanCodeList", exp);
				}
			}
			return ret;
		}

		public void RepairUnitPlanCodeFromDB(string pathDB)
		{
			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//List<string> locationCodeListFromItur = iturRepository.GetLocationCodeList(pathDB);			//из
			//List<string> locationCodeListFromLocation = this.GetLocationCodeList(pathDB); //в
			//Dictionary<string, string> difference = new Dictionary<string, string>();

			//foreach (var locationCodeFromItur in locationCodeListFromItur)			   //из
			//{
			//    if (locationCodeListFromLocation.Contains(locationCodeFromItur) == false)		 //в
			//    {
			//        difference[locationCodeFromItur] = locationCodeFromItur;
			//    }
			//}

			//foreach (KeyValuePair<string, string> keyValuePair in difference)
			//{
			//    if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
			//    {
			//        Location locationNew = new Location();
			//        locationNew.Code = keyValuePair.Value;
			//        locationNew.Name = keyValuePair.Value;
			//        //if (locationNew.Code == DomainUnknownCode.UnknownLocation)
			//        //{
			//        //    locationNew.Name = DomainUnknownName.UnknownLocation;
			//        //}
			//        locationNew.RestoreBit = true;
			//        locationNew.Description = "Repair from Itur";
			//        locationNew.Restore = DateTime.Now.ToString();
			//        this.Insert(locationNew, pathDB);
			//    }

			//}

			//Location unknownLocation = this.GetLocationByCode(DomainUnknownCode.UnknownLocation, pathDB);
			//if (unknownLocation == null)
			//{
			//    Location locationNew = new Location();
			//    locationNew.Code = DomainUnknownCode.UnknownLocation;
			//    locationNew.Name = DomainUnknownName.UnknownLocation;
			//    locationNew.Description = "Repair";
			//    this.Insert(locationNew, pathDB);
			//}
			//else
			//{
			//    unknownLocation.Name = DomainUnknownName.UnknownLocation;
			//    this.Update(unknownLocation, pathDB);
			//}

		}
        #endregion
    }
}
