using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
    public class StatusIturEFRepository : BaseEFRepository //, IStatusIturRepository
    {
		public StatusIturEFRepository(IConnectionDB connection)
			: base(connection)
        {

        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IStatusIturRepository Members

		public StatusIturs GetStatuses(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.StatusIturs.ToList().Select(e => e.ToDomainObject());
                return StatusIturs.FromEnumerable(domainObjects);
            }
        }

		//public StatusIturs GetStatuses(string pathDB)
		//{
			
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        var domainObjects = db.StatusIturs.ToList().Select(e => e.ToDomainObject());
		//        return StatusIturs.FromEnumerable(domainObjects);
		//    }
		//}
//        DocumentStatusEnum
//        "No One Document"	 "NoOneDoc"	"NoOneDoc"	0
//"None"	"None"	"None"	1
//"One Document Is Not Approve"	"OneDocIsNotApprove"	"OneDocIsNotApprove"	2
//"One Document Is Approve"	"OneDocIsApprove"	"OneDocIsApprove" 	3
//"Any of Documents Is Not Approve"	"AnyDocIsNotApprove"	"AnyDocIsNotApprove"	4
//"All Documents Is Approve"	"AllDocIsApprove"	"AllDocIsApprove"	5


		public StatusIturs GetStatuses(SelectParams selectParams, string pathDB)
        {
            if (selectParams == null)
                return GetStatuses(pathDB);

            long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                // Получение сущностей и общего количества из БД.
                // Getting entities and total count from database.
                var entities = GetEntities(db, AsQueryable(db.StatusIturs), db.StatusIturs.AsQueryable(), 
					selectParams, out totalCount);

                // Преобразование сущностей в объекты предметной области.
                // Converting entites to domain objects.
                var domainObjects = entities.Select(e => e.ToDomainObject());

                // Возврат результата.
                // Returning result.
                var result = StatusIturs.FromEnumerable(domainObjects);
                result.TotalCount = totalCount;
                return result;
            }
        }

        #endregion
    }
}
