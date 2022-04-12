using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
    public class StatusDocHeaderEFRepository : BaseEFRepository, IStatusDocHeaderRepository
    {
		public StatusDocHeaderEFRepository(IConnectionDB connection)
			: base(connection)
        {

        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IStatusDocHeaderRepository Members

		public StatusDocHeaders GetStatuses(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.StatusDocHeaders.ToList().Select(e => e.ToDomainObject());
                return StatusDocHeaders.FromEnumerable(domainObjects);
            }
        }

        #endregion
    }
}
