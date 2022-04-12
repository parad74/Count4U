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
    public class UnitPlanValueEFRepository : BaseEFRepository, IUnitPlanValueRepository
    {
		private Dictionary<string, UnitPlanValue> _unitPlanValueDictionary;
		private readonly IServiceLocator _serviceLocator;

		public UnitPlanValueEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
            : base(connection)
        {
			this._unitPlanValueDictionary = new Dictionary<string, UnitPlanValue>();
			this._serviceLocator = serviceLocator;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

		#region IUnitPlanValueRepository Members

		public UnitPlanValues GetUnitPlanValues(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.UnitPlanValue.ToList().Select(e => e.ToDomainObject());
				return UnitPlanValues.FromEnumerable(domainObjects);
            }
        }

		public UnitPlanValues GetUnitPlanValues(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetUnitPlanValues(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.UnitPlanValue), db.UnitPlanValue.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = UnitPlanValues.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void FillUnitPlanValues(string pathDB, SelectParams selectParams = null)
		{

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			//IIturAnalyzesRepository iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();

			UnitPlanValues unitPlanValues = null;
			//IEnumerable<IturAnalyzesSimple> iturAnalyzesList = iturAnalyzesRepository.GetIturSumQuantityEditByIturCode(selectParams, pathDB, true);
			
			if (selectParams == null)
			{
				unitPlanValues = this.GetUnitPlanValues(pathDB);
			}
			else
			{
				unitPlanValues = this.GetUnitPlanValues(selectParams, pathDB);
			}
			if (unitPlanValues == null) return;

			foreach (UnitPlanValue unitPlanValue in unitPlanValues)
			{
				SelectParams selectParms = new SelectParams();
				List<string> iturCodes = iturRepository.GetIturCodesUnitPlanCode(unitPlanValue.UnitPlanCode, pathDB);
				selectParms.FilterStringListParams.Add("IturCode", new FilterStringListParam() { Values = iturCodes });
						
				unitPlanValue.Done = (int)iturRepository.GetIturTotalDone(selectParms, pathDB);
				unitPlanValue.TotalItur = iturCodes.Count();
			}
			this.Update(unitPlanValues, pathDB);
		}
			
		


		public void Delete(UnitPlanValue unitPlanValue, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, unitPlanValue.UnitPlanCode);
				if (entity == null) return;
				db.UnitPlanValue.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void DeleteAll(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				db.UnitPlanValue.ToList().ForEach(e => db.UnitPlanValue.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public void Insert(UnitPlanValue unitPlanValue, string pathDB)
        {
			if (unitPlanValue == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = unitPlanValue.ToEntity();
				db.UnitPlanValue.AddObject(entity);
                db.SaveChanges();
            }
        }

		public void Update(UnitPlanValue unitPlanValue, string pathDB)
        {
			if (unitPlanValue == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, unitPlanValue.UnitPlanCode);
				if (entity == null) return;
				entity.ApplyChanges(unitPlanValue);
                db.SaveChanges();
            }
        }

		public void Update(UnitPlanValues unitPlanValues, string pathDB)
		{
			if (unitPlanValues == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (UnitPlanValue unitPlanValue in unitPlanValues)
				{
					if (unitPlanValue == null) continue;
					var entity = this.GetEntityByCode(db, unitPlanValue.UnitPlanCode);
					if (entity == null) continue;
					entity.ApplyChanges(unitPlanValue);
				}
				db.SaveChanges();
			}
		}

		public UnitPlanValue GetUnitPlanValueByUnitPlanCode(string unitPlanCode, string pathDB)
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

		public Dictionary<string, UnitPlanValue> GetUnitPlanValueDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearUnitPlanValueDictionary();
				this.FillUnitPlanValueDictionary(pathDB);
			}
			return this._unitPlanValueDictionary;
		}

		public void ClearUnitPlanValueDictionary()
		{
			this._unitPlanValueDictionary.Clear();
			GC.Collect();
		}

		public void AddUnitPlanValueInDictionary(string unitPlanCode, UnitPlanValue unitPlanValue)
		{
			if (string.IsNullOrWhiteSpace(unitPlanCode)) return;
			if (this._unitPlanValueDictionary.ContainsKey(unitPlanCode) == false)
			{
				this._unitPlanValueDictionary.Add(unitPlanCode, unitPlanValue);
			}
		}

		public void RemoveUnitPlanValueFromDictionary(string unitPlanCode)
		{
			try
			{
				this._unitPlanValueDictionary.Remove(unitPlanCode);
			}
			catch { }
		}

		public bool IsExistUnitPlanValueInDictionary(string code)
		{
			if (this._unitPlanValueDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public UnitPlanValue GetUnitPlanValueByCodeFromDictionary(string code)
		{
			if (this._unitPlanValueDictionary.ContainsKey(code) == true)
			{
				return this._unitPlanValueDictionary[code];
			}
			return null;
		}

		public void FillUnitPlanValueDictionary(string pathDB)
		{
			this.ClearUnitPlanValueDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					UnitPlanValues unitPlanValues = this.GetUnitPlanValues(pathDB);
					//this._locationDictionary = db.Locations.Select(e => new Location
					//{
					//    Code = e.Code,
					//    Name = e.Name,
					//    BackgroundColor = e.BackgroundColor,
					//    Description = e.Description
					//}).Distinct().ToDictionary(k => k.Code);

					this._unitPlanValueDictionary = unitPlanValues.Select(e => e).Distinct().ToDictionary(k => k.UnitPlanCode); 
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

		private App_Data.UnitPlanValue GetEntityByCode(App_Data.Count4UDB db, string unitPlanCode)
		{
			var entity = db.UnitPlanValue.FirstOrDefault(e => e.UnitPlanCode.CompareTo(unitPlanCode) == 0);
			return entity;
		}


		public List<string> GetUnitPlanValueCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.UnitPlanValue.Select(e => e.UnitPlanCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetUnitPlanCodeList", exp);
				}
			}
			return ret;
		}

		public void RepairUnitPlanValueCodeFromDB(string pathDB)
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
