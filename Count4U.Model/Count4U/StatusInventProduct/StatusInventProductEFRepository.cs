using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Count4U.Model.Count4U
{
    public class StatusInventProductEFRepository : BaseEFRepository, IStatusInventProductRepository
    {
		public StatusInventProductEFRepository(IConnectionDB connection)
			: base(connection)
        {

        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
			//BitArray bit = new BitArray(new int[] {4});

			//BitArray[] bitarray = new BitArray[] {
			//    new BitArray(new int[] {4}), 
			//    new BitArray(new int[] {4})};

			//List<BitArray> ba = new List<BitArray>();
			//ba.Add(new BitArray(new int[] { 4 }));

            return objectSet.AsQueryable();
        }

        #endregion

        #region IStatusInventProductRepository Members

		public StatusInventProducts GetStatuses(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.StatusInventProducts.ToList().Select(e => e.ToDomainObject());
                return StatusInventProducts.FromEnumerable(domainObjects);
            }
        }

		public Dictionary<string, StatusInventProduct> GetStatusInventProductSumBitDictionary(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.StatusInventProducts.ToList().Select(e => e.ToDomainObject());
					var domainObjectsSum = from e in domainObjects
									   orderby e.Code
									   group e by new
									   {
										   e.Code,
										   //e.Makat
									   } into g
										   select new StatusInventProduct
									   {
										   Code = g.Key.Code,
										   Bit = g.Sum(x => x.Bit)
									   };

					Dictionary<string, StatusInventProduct> dictionary = domainObjectsSum.Select(e => e).Distinct().ToDictionary(k => k.Code);

					return dictionary;
			}
		}

	
		
		//public List<BitArray> GetStatusBitArrayList(string pathDB)
		//{
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        var intArray = db.StatusInventProducts.Select(e => e.Bit).ToArray();
		//        List<BitArray> bitArrayList = BitStatus.GetBitList(intArray);
		//        return bitArrayList;
		//    }
		//}

		//public BitArray GetStatusBitArray(string pathDB)
		//{
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        var intArray = db.StatusInventProducts.Select(e => e.Bit).ToArray();
		//        BitArray bitArrayAdd = BitStatus.GetBitArrayAnd(intArray);
		//        return bitArrayAdd;
		//    }
		//}
        #endregion
    }
}
