using System;
using System.Data.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections.Generic;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyDecoratorEFRepository : BaseEFRepository, IPropertyDecoratorRepository
    {
		public PropertyDecoratorEFRepository(ConnectionDB connection)
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

		public PropertyDecorators GetPropertyDecorators(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				return null;
            }
		}


		public Dictionary<string, PropertyDecorator> GetPropertyDecoratorDictionary(string pathDB, bool refill = true)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				return null;
			}
		}

		#endregion
    }
}
