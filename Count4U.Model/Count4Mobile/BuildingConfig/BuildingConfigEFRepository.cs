using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class BuildingConfigEFRepository : BaseEFRepository, IBuildingConfigRepository
    {
		public BuildingConfigEFRepository(IConnectionDB connection)
			: base(connection)
        {

        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IUnitTypeRepository Members

		public BuildingConfigs GetBuildingConfigs(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				//var domainObjects = db.BuildingConfig.ToList().Select(e => e.ToDomainObject());
				//return UnitTypes.FromEnumerable(domainObjects);		 
				return null;
            }
		}

	
		public Dictionary<string, BuildingConfig> GetBuildingConfigDictionary(string pathDB, bool refill = true)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//var domainObjects = db.BuildingConfig.ToList().Select(e => e.ToDomainObject());
				//return UnitTypes.FromEnumerable(domainObjects);		 
				return null;
			}
		}

		#endregion
    }
}
