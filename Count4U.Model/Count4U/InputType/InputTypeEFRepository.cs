using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
    public class InputTypeEFRepository : BaseEFRepository, IInputTypeRepository
    {
		public InputTypeEFRepository(IConnectionDB connection)
			: base(connection)
        {

        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IInputTypeRepository Members

		public InputTypes GetInputTypes(string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.InputTypes.ToList().Select(e => e.ToDomainObject());
                return InputTypes.FromEnumerable(domainObjects);
            }
        }

        #endregion
    }
}
