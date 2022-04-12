using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Threading.Tasks;
using System.Threading;

namespace Count4U.Model.Count4U
{
    public class TypeEFRepository : BaseEFRepository, ITypeRepository
    {
		public TypeEFRepository(IConnectionDB connection)
			: base(connection)
        {

        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
		     return objectSet.AsQueryable();
        }

        #endregion

        #region ITypeRepository Members

		public Types GetTypes(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.Types.ToList().Select(e => e.ToDomainObject());
                return Types.FromEnumerable(domainObjects);
            }
        }


        //public async void SomeAction()
        //{
        //    //Console.WriteLine("Start actions");
        //    //var task = new Task(() =>
        //    //{
        //    //    Console.WriteLine("DoLongWork entered");
        //    //    Thread.Sleep(2000);
        //    //    Console.WriteLine("DoLongWork ended");

        //    //});

        //    //task.Start();
        //    ////await task;
        //    //Console.WriteLine("End actions");
        //} 


        #endregion
    }
}
