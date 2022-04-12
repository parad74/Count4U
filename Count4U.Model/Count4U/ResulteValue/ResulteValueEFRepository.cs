using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Count4U
{
    public class ResulteValueEFRepository : BaseEFRepository, IResulteValueRepository
    {
		private Dictionary<string, ResulteValue> _resulteValueDictionary;

		public ResulteValueEFRepository(IConnectionDB connection)
			: base(connection)
        {
			this._resulteValueDictionary = new Dictionary<string, ResulteValue>();
        }

        #region BaseEFRepository Members
	

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

		public ResulteValues GetResulteValues(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.ResulteValues.ToList().Select(e => e.ToDomainObject());
				return ResulteValues.FromEnumerable(domainObjects);
			}
		}


		public ResulteValues GetResulteValues(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetResulteValues(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.ResulteValues), db.ResulteValues.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = ResulteValues.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public string[] GetSectionCodes(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string[] codes = db.ResulteValues.ToList().Select(e => e.Code).ToArray();
				return codes;
			}
		}

		public ResulteValue GetResulteValueByCode(string code, string pathDB)
		{
			if (string.IsNullOrEmpty(code) == true) return null;

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public void Delete(ResulteValue resulteValue, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var sectionEntities = db.ResulteValues.Where(e => e.Code == resulteValue.Code).ToList();
				if (sectionEntities != null)
				{
					sectionEntities.ForEach(e => db.ResulteValues.DeleteObject(e));
				}

				db.SaveChanges();
			}
		}

		public void Insert(ResulteValue resulteValue, string pathDB)
		{
			if (resulteValue == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = resulteValue.ToEntity();
				db.ResulteValues.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(ResulteValues resulteValues, string pathDB)
		{
			if (resulteValues == null) return;
			Dictionary<string, ResulteValue> resulteValueFromDBDictionary = GetResulteValueDictionary(pathDB, true);
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var resulteValue in resulteValues)
				{
					if (resulteValueFromDBDictionary.ContainsKey(resulteValue.Code) == false)
					{
						var entity = resulteValue.ToEntity();
						db.ResulteValues.AddObject(entity);
					}
				}
				db.SaveChanges();
			}
		}

		//public void Update(ResulteValue section, string pathDB)
		//{
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        var entity = this.GetEntityByCode(db, section.SectionCode);
		//        if (entity == null) return;
		//        entity.ApplyChanges(section);
		//        db.SaveChanges();
		//    }
		//}

		//public void Update(ResulteValues sections, string pathDB)
		//{
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        foreach (Section section in sections)
		//        {
		//            var entity = this.GetEntityByCode(db, section.SectionCode);
		//            if (entity == null) return;
		//            entity.ApplyChanges(section);
		//        }
		//        db.SaveChanges();
		//    }
		//}

		public Dictionary<string, ResulteValue> GetResulteValueDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				this.FillResulteValueDictionary(pathDB);
			}
			return this._resulteValueDictionary;
		}

		public void ClearResulteValueDictionary()
		{
			this._resulteValueDictionary.Clear();
			GC.Collect();
		}

		public void FillResulteValueDictionary(string pathDB)
		{
			this.ClearResulteValueDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					ResulteValues resulteValues = this.GetResulteValues(pathDB);

					foreach (var resulteValue in resulteValues)
					{
						this._resulteValueDictionary[resulteValue.Code] = resulteValue;
					}

					// недавно изменила 
					//this._resulteValueDictionary = resulteValues.Select(e => e).Distinct().ToDictionary(k => k.Code);
				}
				catch { }
			}
		}
  
        #endregion

		private App_Data.ResulteValue GetEntityByCode(App_Data.Count4UDB db, string code)
		{
			var entity = AsQueryable(db.ResulteValues).FirstOrDefault(e => e.Code.CompareTo(code) == 0);
			return entity;
		}

    }
}
