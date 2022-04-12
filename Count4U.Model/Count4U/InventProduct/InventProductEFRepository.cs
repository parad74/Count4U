using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using System.Collections;
using Codeplex.Reactive;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
    public class InventProductEFRepository : BaseEFRepository, IInventProductRepository
    {
        public InventProductEFRepository(IConnectionDB connection)
            : base(connection)
        {
		
        }

        #region BaseEFRepository Members


        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IInventProductRepository Members

        public InventProducts GetInventProducts(string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entities = db.InventProducts.ToList();
				if (entities == null)
				{
					InventProducts res = new InventProducts();
					res.TotalCount = 0;
					res.TotalItur = 0;
					res.SumQuantityEdit = 0;
					return res;
				}
				//var domainObjects = db.InventProducts.ToList().Select(e => e.ToDomainObject());
				//return InventProducts.FromEnumerable(domainObjects);
				var result = InventProducts.FromEntityList(entities);
				result.TotalCount = entities.LongCount();
				//result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				//var iturs = entities.Select(e => e.IturCode).Distinct().ToArray();
				//result.TotalItur = iturs.LongCount();
				return result;
            }
        }

        public InventProducts GetInventProducts(SelectParams selectParams, string pathDB)
        {
            if (selectParams == null)
                return GetInventProducts(pathDB);

            long totalCount = 0;
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entities = GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
				if (entities == null)
				{
					InventProducts res = new InventProducts();
					res.TotalCount = 0;
					res.TotalItur = 0;
					res.SumQuantityEdit = 0;
					return res;
				}
				//var domainObjects = entities.Select(e => e.ToDomainObject());
				//var result = InventProducts.FromEnumerable(domainObjects);
				//result.TotalCount = totalCount;
				//return result;
				var result = InventProducts.FromEntityList(entities);
				result.TotalCount = totalCount;

				//result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				//var iturs = entities.Select(e => e.IturCode).Distinct().ToArray();
				//result.TotalItur = iturs.LongCount();


				return result;
            }
        }

		public InventProducts GetInventProductsExtended(SelectParams selectParams, string code, string pathDB)
		{
			InventProducts resultInventProducts = this.GetInventProducts(selectParams, pathDB);
	

			// =======================	  Extended ==================

			Dictionary<string, App_Data.Itur> iturDictionary = new Dictionary<string, App_Data.Itur>();
			if (code == "[Rep-IP1-05]")
			{
				using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					iturDictionary = db.Iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);
				}
			}


			if (code == "[Rep-IP1-05]")
			{
				foreach (InventProduct ip in resultInventProducts)
				{
					if (iturDictionary.ContainsKey(ip.IturCode) == true)
					{
						ip.IPValueStr11 = ip.IturCode;
						ip.IPValueStr12 = ip.ERPIturCode;
					}
					else
					{
						ip.IPValueStr11 = "code not exist";
						ip.IPValueStr12 = "code not exist";
					}
				}
			}

			return resultInventProducts;
		}

		public InventProducts GetInventProductTotal(string pathDB, SelectParams selectParams = null)
		{
			InventProducts result = new InventProducts();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams == null)
				{
					long longCount =  db.InventProducts.LongCount();
					result.TotalCount = longCount;
					if (longCount == 0)
					{
						result.SumQuantityEdit = 0;
						result.TotalItur = 0;
					}
					else
					{
						result.SumQuantityEdit = db.InventProducts.Sum(e => e.QuantityEdit);
						var iturs = db.InventProducts.Select(e => e.IturCode).Distinct().ToArray();
						result.TotalItur = iturs.LongCount();
					}
				}
				else  // selectParams != 0 
				{
					IQueryable<App_Data.InventProduct> inventProductQueryable = db.InventProducts.AsQueryable();
					inventProductQueryable = selectParams.ApplyFilterToQuery(inventProductQueryable);
					long longCount = inventProductQueryable.LongCount();
					if (longCount == 0)
					{
						result.SumQuantityEdit = 0;
						result.TotalItur = 0;
					}
					else
					{
						result.SumQuantityEdit = inventProductQueryable.Sum(e => e.QuantityEdit);
						var iturs = inventProductQueryable.Select(e => e.IturCode).Distinct().ToArray();
						result.TotalItur = iturs.LongCount();
					}
					//selectParams.IsEnablePaging = false;
					//var entities = GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(),
					//selectParams, out totalCount);
					//InventProducts result = new InventProducts();
					//if (entities == null)
					//{
					//    result.TotalCount = 0;
					//    result.TotalItur = 0;
					//    result.SumQuantityEdit = 0;
					//}
					//else
					//{
					//    result.TotalCount = totalCount;
					//    result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
					//    var iturs =  entities.Select(e => e.IturCode).Distinct().ToArray();
					//    result.TotalItur = iturs.LongCount();
						
					//}
					
				}
			}
			return result;
		}


		public InventProducts GetInventProductNotExistInCatalog(string pathDB)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				int fromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
				var entities = db.InventProducts.Where(e => e.FromCatalogType == fromCatalogType).ToList();
				InventProducts result = InventProducts.FromEntityList(entities);
				return result;
			}
		}


        public InventProduct GetInventProductByCode(string inventProductCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByCode(db, inventProductCode);
                if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

        public InventProduct GetInventProductByID(long ID, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByID(db, ID);
                if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

        public InventProduct GetInventProductByBarcode(string barcode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByBarcode(db, barcode);
                if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

        public InventProducts GetInventProductsByStatusCode(string statusCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entities = 	db.InventProducts.Where(e => e.StatusInventProductCode == statusCode).ToList();
				if (entities == null) 
				{
					InventProducts res = new InventProducts();
					res.TotalCount = 0;
					res.TotalItur = 0;
					res.SumQuantityEdit = 0;
					return res;
				}
				//var domainObjects = entities.Select(e => e.ToDomainObject());
				//return InventProducts.FromEnumerable(domainObjects);
				var result = InventProducts.FromEntityList(entities);
				result.TotalCount = entities.LongCount();
				//result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				//var iturs = entities.Select(e => e.IturCode).Distinct().ToArray();
				//result.TotalItur = iturs.LongCount();
				return result;
            }
        }

        public InventProducts GetInventProductsByDocumentHeader(DocumentHeader documentHeader, string pathDB)
        {
            return this.GetInventProductsByDocumentCode(documentHeader.DocumentCode, pathDB);
        }

        public InventProducts GetInventProductsByDocumentCode(string documentCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entities = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0).ToList();
				if (entities == null)
				{
					InventProducts res = new InventProducts();
					res.TotalCount = 0;
					res.TotalItur = 0;
					res.SumQuantityEdit = 0;
					return res;
				}
				//var domainObjects = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0)
				//    .ToList().Select(e => e.ToDomainObject());
				//return InventProducts.FromEnumerable(domainObjects);
				var result = InventProducts.FromEntityList(entities);
				result.TotalCount = entities.LongCount();
				//result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				//var iturs = entities.Select(e => e.IturCode).Distinct().ToArray();
				//result.TotalItur = iturs.LongCount();
				return result;
            }
        }

        public int GetMaxNumForDocumentCode(string documentCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0);//ToDomainObject();
				if (entity == null) return 0;
				if (entity.Count() == 0) return 0;
  				var ipNum = entity.Max(e => e.IPNum);
                return Convert.ToInt32(ipNum);
            }
        }


		public double GetSumQuantityEditByMakat(string makat, string pathDB)
		{
			//string typeMakat = TypeMakatEnum.M.ToString();
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var ents = db.InventProducts.Where(e => e.Makat.CompareTo(makat) == 0
					//&&  e.TypeMakat == typeMakat
					&& e.QuantityEdit != null);
				if (ents != null && ents.Count() > 0)
				{
					var sum = ents.Sum(x => x.QuantityEdit);
					return sum;
				}
				return 0;
			}
		}


		public Dictionary<Tuple<string, string, string>, InventProduct> GetQuantitySerialInventProducts(string pathDB)
			//List<tblCurrentInventory> tempCurrentInventories)
		{
			//var dictionary = new Dictionary<Pair<string, string, string>, InventProduct>();
			Dictionary<Tuple<string, string, string>, InventProduct> dictionary = new Dictionary<Tuple<string, string, string>, InventProduct>();
			InventProducts inventProducts = this.GetInventProducts(pathDB);
			 //dictionary = inventProducts.ToDictionary(e => Tuple.Create(e.Makat, e.SerialNumber, e.IturCode));
			//InventProduct inventProduct   =  dictionary1[  Tuple.Create("makat1","sn1", "26002600")	 ;
			foreach (var inventProduct in inventProducts)
			{
				dictionary[Tuple.Create(inventProduct.Makat, inventProduct.SerialNumber, inventProduct.IturCode)] = inventProduct;
			}
			 return dictionary;
		}

		public Dictionary<string, InventProduct> GetDictionaryInventProductsUID(string pathDB)
		{
			Dictionary<string, InventProduct> dictionary = new Dictionary<string, InventProduct>();
			InventProducts inventProducts = this.GetInventProducts(pathDB);
			//dictionary = inventProducts.ToDictionary(e => Tuple.Create(e.Makat, e.SerialNumber, e.IturCode));
			//InventProduct inventProduct   =  dictionary1[  Tuple.Create("makat1","sn1", "26002600")	 ;
			foreach (var inventProduct in inventProducts)
			{
				if (string.IsNullOrWhiteSpace(inventProduct.Barcode) == false)
				{
					dictionary[inventProduct.Barcode] = inventProduct;
				}
			}
			return dictionary;
		}



        //не работает
        public IQueryable<InventProduct> GetInventProductMakatAndSumQuantityEdit(string pathDB)
        {
            string makat = TypeMakatEnum.M.ToString();
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = from ip in db.InventProducts
                                    where ip.TypeMakat == makat
                                    group ip by ip.Makat
                                    into q
                                    select new InventProduct
                                        {
                                            Makat = q.Key,
                                            QuantityEdit = q.Sum(x => x.QuantityEdit)
                                        };
                return domainObjects;
                //var domainObjects = db.InventProducts.ToList().Select(e => e.ToDomainObject()).Sum(x => x.QuantityEdit);
                //return domainObjects;
            }
        }


        public double GetSumQuantityEditByDocumentCode(string documentCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var ents = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0 
					&& e.QuantityEdit !=null) ;
				if (ents != null && ents.Count() > 0)
				{
					var sum = ents.Sum(x => x.QuantityEdit);
					return sum;
				}
	           return 0;
            }
        }

		public ReactiveProperty<string> ReturnStringSumQuantityEditByDocumentCode(string documentCode, string pathDB)
		{
			ReactiveProperty<string> ret = new ReactiveProperty<string>();
			double sumQuantityEdit = GetSumQuantityEditByDocumentCode(documentCode, pathDB);
			if (sumQuantityEdit > 0)
			{
				ret.Value = String.Format("{0:0.##}", sumQuantityEdit);
				return ret;
			}
			ret.Value = "0";
			return ret;
	  	 }

		private string GetStringSumQuantityEditByDocumentCode(string documentCode, string pathDB)
		{
			double sumQuantityEdit = GetSumQuantityEditByDocumentCode(documentCode, pathDB);
			if (sumQuantityEdit > 0)
			{
				return String.Format("{0:0.##}", sumQuantityEdit);
			}
			return "0";
		}

		//public ReactiveProperty<string> ReturnSumQuantityEditByDocumentCode(string documentCode, string pathDB)
		//{
			//это не работает
			//using (ReactiveProperty<string> ResultsSum = new ReactiveProperty<string>(/*Scheduler.NewThread*/))
			//{
			//	var results = ResultsSum
			//		.Select(_ =>
			//		{
			//			return GetStringSumQuantityEditByDocumentCode(documentCode, pathDB);
			//                 }).ToReactiveProperty();
			//	return results;
			//	}
			//Это работает
			//double sumQuantityEdit = GetSumQuantityEditByDocumentCode(documentCode, pathDB);
			//ValidationAttr.Value = String.Format("{0:0.##}", sumQuantityEdit);
			//return ValidationAttr;



			//    GetSumQuantityEditByDocumentCode(documentCode, pathDB));
			//return Observable.Create<double>(o =>
			//{
			//    o.OnNext(123);
			//    o.OnCompleted();
			//    return Disposable.Empty;
			//});

			//return Observable.Create<double>(
			//        (IObserver<double> observer) =>
			//        {
			//            observer.OnNext(123.123);
			//            observer.OnCompleted();
			//            return Disposable.Empty;
			//        });

			//List<string> Items = new List<string>();
			//LocationItems.Add("12");
			//IObservable<string> val = LocationItems.ToObservable();
			////var val = LocationItems as IEnumerable<string>;
			//_sumQuantityEditByDocumentCode = val.ToReactiveProperty();
			////var sum = val.Select(x => x).ToReactiveProperty();

			//return _sumQuantityEditByDocumentCode;
		//}
		
		public double GetSumQuantityOriginalByDocumentCode(string documentCode, string pathDB)
        {
 			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var ents = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0
					&& e.QuantityOriginal != null);
				if (ents != null && ents.Count() > 0)
				{
					var sum = ents.Sum(x => x.QuantityOriginal);
					return sum;
				}
				return 0;
			}
        }

        public double GetSumQuantityDifferenceByDocumentCode(string documentCode, string pathDB)
        {
      		using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var ents = db.InventProducts.Where(e => e.DocumentCode.CompareTo(documentCode) == 0
					&& e.QuantityDifference != null);
				if (ents != null && ents.Count() > 0)
				{
					var sum = ents.Sum(x => x.QuantityDifference);
					return sum;
				}
				return 0;
			}
        }

        public double GetSumQuantityEditByIturCode(string iturCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				double sum = 0;
				var inventProducts = db.InventProducts.Where(e => e.IturCode.CompareTo(iturCode) == 0).ToList();
				if (inventProducts == null) return sum;
				try
				{
					sum = (double)inventProducts.Sum(x => x.QuantityEdit);
				}
				catch { }
				//double sum = 0;
				//var docs = db.DocumentHeaders.Where(e => e.IturCode.CompareTo(iturCode) == 0).ToList();
				//if (docs == null) return sum;
				//try
				//{
				//	sum = (double)docs.Sum(x => x.QuantityEdit);
				//}
				//catch { }
				return sum;
				//foreach (var doc in docs)
				//{	 
				//	double sum1 = 0;
				//	var entetis = db.InventProducts.Where(e => e.DocumentCode.CompareTo(doc.DocumentCode) == 0
				//		&& e.QuantityEdit !=null) ;
				//	if (entetis != null && entetis.Count() > 0)
				//	{
				//		sum1 = entetis.Sum(x => x.QuantityEdit);
				//		}
				//	sum = sum + sum1;
				//}
                //return sum;
            }
        }

		//old не используется
		public IEnumerable<Itur> GetIturSumQuantityEdit(SelectParams selectParams, 	string pathDB)
		{
			long totalCount = 0;
	
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode
										   group e by e.IturCode into g
										   select new Itur
										   {
											   IturCode = g.Key,
											   SumQuantityEdit = g.Sum(x => x.QuantityEdit),
										   };
					foreach (var itur in domainObjectsSum)
					{

					}
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode
										   group e by e.IturCode into g
										   select new Itur
										   {
											   IturCode = g.Key,
											   SumQuantityEdit = g.Sum(x => x.QuantityEdit),
										   };
					return domainObjectsSum;
				}
			}
		}


		public List<string> GetIturCodeList(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(),
						selectParams, out totalCount);
					List<string> codeList = entities.Select(e => e.IturCode).Distinct().ToList();
					return codeList;
				}
				else
				{
					var entities = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
					List<string> codeList = entities.Select(e => e.IturCode).Distinct().ToList();
					return codeList;
				}
			}
		}

		public ReactiveProperty<string> ReturnStringSumQuantityEditByIturCode(string iturCode, string pathDB)
		{
			ReactiveProperty<string> ret = new ReactiveProperty<string>();
			double sumQuantityEdit = this.GetSumQuantityEditByIturCode(iturCode, pathDB);
			if (sumQuantityEdit > 0)
			{
				ret.Value = String.Format("{0:0.##}", sumQuantityEdit);
				return ret;
			}
			ret.Value = "";
			return ret;
		}

		private string GetStringSumQuantityEditByIturCode(string iturCode, string pathDB)
		{
			double sumQuantityEdit = this.GetSumQuantityEditByIturCode(iturCode, pathDB);
			if (sumQuantityEdit > 0)
			{
				return String.Format("{0:0.##}", sumQuantityEdit);
			}
			return "0";
		}

		//public ReactiveProperty<string> ReturnSumQuantityEditByIturCode(string iturCode, string pathDB)
		//{
		//	using (ReactiveProperty<string> ResultsSum = new ReactiveProperty<string>(/*Scheduler.NewThread*/))
		//	{
		//		var results = ResultsSum
		//			.Select(_ =>
		//			{
		//				return this.GetStringSumQuantityEditByIturCode(iturCode, pathDB);
		//			}).ToReactiveProperty();
		//		return results;
		//	}

		//	//using (ReactiveProperty<string> ValidationAttr = new ReactiveProperty<string>(Scheduler.NewThread))
		//	//{
		//	//    double  sumQuantityEdit= this.GetSumQuantityEditByIturCode(iturCode, pathDB);
		//	//    ValidationAttr.Value = String.Format(Localization.Resources.ViewModel_InventProductListDetails_TotalItems + " {0:0.##}", sumQuantityEdit);
		//	//    return ValidationAttr;
		//	//}
		//}

		// не используется
        public double GetSumQuantityDifferenceByIturCode(string iturCode, string pathDB)
        {
 			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				double sum = 0;
				var docs = db.DocumentHeaders.Where(e => e.IturCode.CompareTo(iturCode) == 0).ToList();
				if (docs == null) return sum;

				foreach (var doc in docs)
				{
					double sum1 = 0;
					var entetis = db.InventProducts.Where(e => e.DocumentCode.CompareTo(doc.DocumentCode) == 0
						&& e.QuantityDifference != null);
					if (entetis != null && entetis.Count() > 0)
					{
						sum1 = entetis.Sum(x => x.QuantityDifference);
					}
					sum = sum + sum1;
				}
				return sum;
			}
        }

		// не используется
        public double GetSumQuantityOriginalByIturCode(string iturCode, string pathDB)
        {
      		using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				double sum = 0;
				var docs = db.DocumentHeaders.Where(e => e.IturCode.CompareTo(iturCode) == 0).ToList();
				if (docs == null) return sum;

				foreach (var doc in docs)
				{
					double sum1 = 0;
					var entetis = db.InventProducts.Where(e => e.DocumentCode.CompareTo(doc.DocumentCode) == 0
						&& e.QuantityOriginal != null);
					if (entetis != null && entetis.Count() > 0)
					{
						sum1 = entetis.Sum(x => x.QuantityOriginal);
					}
					sum = sum + sum1;
				}
				return sum;
			}

        }

		public int GetCountItemByDocumentCode(string documentCode, string pathDB)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				int sum = 0;
				var doc = db.DocumentHeaders.Where(e => e.IturCode.CompareTo(documentCode) == 0).FirstOrDefault();
				if (doc == null) return sum;
				return Convert.ToInt32(doc.Total);
			}
		}

		//================ IPSumQuantityEdit		IPSumQuantityMakatAndIturCode
		//-----------------------------------------------ByMakats                            IturCode 
		//=================from  IturAnalyzes
		//-----------------------------------------------by 
		//ExportInventProductNibitFileWriter
		public Dictionary<string, ProductMakat> GetIPCountByMakatsAndIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, ProductMakat> productDictionary = new Dictionary<string, ProductMakat>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Makat
										 group e by new
										 {
											 e.IturCode,
											 e.Makat
										 } into g
										 select new InventProduct
											{
												Makat = g.Key.Makat,
												IturCode = g.Key.IturCode				 //IturCode		
											};

				var iturCodeCountByMakat = from e in InventProductGroup
										   orderby e.Makat
										   group e by e.Makat into g
										   select new InventProduct
									  {
										  Makat = g.Key,
										  IPNum = g.Count()   // by IturCode		
									  };

				var countByMakatMoreThenOne = iturCodeCountByMakat.Where(x => x.IPNum > 1).Select(x => x).ToList();

				var productMakats = from e in countByMakatMoreThenOne
									select new ProductMakat
								   {
									   Makat = e.Makat
								   };

				productDictionary = productMakats.Select(e => e).ToDictionary(k => k.Makat);
				return productDictionary;
			}
		}

		//==================
		public Dictionary<string, InventProduct> GetIPQuntetyBarcodeAndIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Barcode
										 group e by new
										 {
											 e.IturCode,
											 e.Barcode
										 } into g
										 select new InventProduct
										 {
											 Barcode = g.Key.Barcode,
											 IturCode = g.Key.IturCode,				 //IturCode		
											 Makat = g.Max(x => x.Makat),
											 SerialNumber = g.Max(x => x.SerialNumber),
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Barcode + "|" + k.IturCode);
				return productDictionary;
			}
		}
		//==================
		public Dictionary<string, InventProduct> GetIPQuntetyCodeAndIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Code
										 group e by new
										 {
											 e.IturCode,
											 e.Code
										 } into g
										 select new InventProduct
										 {
											 Code = g.Key.Code,
											 IturCode = g.Key.IturCode,				 //IturCode		
											 Makat = g.Max(x => x.Makat),
											 Barcode = g.Max(x => x.Barcode),
											 SerialNumber = g.Max(x => x.SerialNumber),
											 IPValueStr10 = g.Max(x => x.IPValueStr10), 
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Code + "|" + k.IturCode);
				return productDictionary;
			}
		}
		//==================

		//==================

		public Dictionary<string, InventProduct> GetIPQuntetyEditIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode
										 group e by new
										 {
											 e.IturCode,
										 } into g
										 select new InventProduct
										 {
											 IturCode = g.Key.IturCode,				 //IturCode		
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.IturCode);
				return productDictionary;
			}
		}
	//==================
		public Dictionary<string, InventProduct> GetIPQuntetyByMakatsAndIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Makat
										 group e by new
										 {
											 e.IturCode,
											 e.Makat
										 } into g
										 select new InventProduct
										 {
											 Makat = g.Key.Makat,
											 IturCode = g.Key.IturCode,				 //IturCode		
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),	
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Makat + "|" + k.IturCode);
				return productDictionary;
			}
		}

		//==================	   GetIPQuntetyByMakatAndSNAndProp10IturCode
		public Dictionary<string, InventProduct> GetIPQuntetyByMakatAndSNAndProp10IturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Makat
										 group e by new
										 {
											 e.IturCode,
											 e.Makat,
											 e.SerialNumber,
											 e.IPValueStr10
										 } into g
										 select new InventProduct
										 {
											 IturCode = g.Key.IturCode,				 //IturCode		
											 Makat = g.Key.Makat,
											 SerialNumber = g.Key.SerialNumber,
											 IPValueStr10 = g.Key.IPValueStr10,
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Makat + "|" + k.SerialNumber + "|" + k.IPValueStr10 + "|" + k.IturCode);
				return productDictionary;
			}
		}
		//==================

		public Dictionary<string, InventProduct> GetIPQuntetyByMakatAndSNAndIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Makat
										 group e by new
										 {
											 e.IturCode,
											 e.Makat,
											 e.SerialNumber
										 } into g
										 select new InventProduct
										 {
											 IturCode = g.Key.IturCode,				 //IturCode		
											 Makat = g.Key.Makat,
											 SerialNumber = g.Key.SerialNumber,
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Makat + "|" + k.SerialNumber + "|" + k.IturCode);
				return productDictionary;
			}
		}
		//==================

		//==================
		public Dictionary<string, InventProduct> GetIPQuntetyByBarcodeAndSNAndIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Barcode
										 group e by new
										 {
											 e.IturCode,
											 e.Barcode,
											 e.SerialNumber
										 } into g
										 select new InventProduct
										 {
											 IturCode = g.Key.IturCode,				 //IturCode		
											 Barcode = g.Key.Barcode,
											 SerialNumber = g.Key.SerialNumber,
											 Makat = g.Max(x => x.Makat),
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Barcode + "|" + k.SerialNumber + "|" + k.IturCode);
				return productDictionary;
			}
		}

		//==================
		public Dictionary<string, InventProduct> GetIPQuntetyByBarcodeAndSNAndProp10IturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Barcode
										 group e by new
										 {
											 e.IturCode,
											 e.Barcode,
											 e.SerialNumber,
											 e.IPValueStr10
										 } into g
										 select new InventProduct
										 {
											 IturCode = g.Key.IturCode,				 //IturCode		
											 Barcode = g.Key.Barcode,
											 SerialNumber = g.Key.SerialNumber,
											 IPValueStr10 = g.Key.IPValueStr10,
											 Makat = g.Max(x => x.Makat),
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Barcode + "|" + k.SerialNumber + "|" + k.IPValueStr10 + "|" + k.IturCode);
				return productDictionary;
			}
		}



		public Dictionary<string, InventProduct> GetIPQuntetyByMakatsAndDocAndIturCode(SelectParams selectParams, string pathDB)
		{
			long totalCount = 0;
			Dictionary<string, InventProduct> productDictionary = new Dictionary<string, InventProduct>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<InventProduct> domainObjects = new List<InventProduct>();
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(), selectParams, out totalCount);
					domainObjects = entities.Select(e => e.ToDomainObject()).ToList();
				}
				else
				{
					domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
				}

				var InventProductGroup = from e in domainObjects
										 orderby e.IturCode, e.Makat
										 group e by new
										 {
											 e.IturCode,
											 e.DocumentCode,
											 e.Makat
										 } into g
										 select new InventProduct
										 {
											 Makat = g.Key.Makat,
											 DocumentCode = g.Key.DocumentCode,
											 IturCode = g.Key.IturCode,				 //IturCode		
											 QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											 QuantityEdit = g.Sum(x => x.QuantityEdit),
										 };

				productDictionary = InventProductGroup.Select(e => e).ToDictionary(k => k.Makat + "|" + k.IturCode + "|" + k.DocumentCode);
				return productDictionary;
			}
		}
		//============================

        public InventProducts GetInventProductsByInputTypeCode(string inputTypeCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entities = db.InventProducts.Where(e => e.InputTypeCode.CompareTo(inputTypeCode) == 0).ToList();
				if (entities == null)
				{
					InventProducts res = new InventProducts();
					res.TotalCount = 0;
					res.TotalItur = 0;
					res.SumQuantityEdit = 0;
					return res;
				}
				//var domainObjects = db.InventProducts.Where(e => e.InputTypeCode.CompareTo(inputTypeCode) == 0)
				//    .ToList().Select(e => e.ToDomainObject());
				//return InventProducts.FromEnumerable(domainObjects);

				var result = InventProducts.FromEntityList(entities);
				result.TotalCount = entities.LongCount();
				//result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				//var iturs = entities.Select(e => e.IturCode).Distinct().ToArray();
				//result.TotalItur = iturs.LongCount();
				return result;
            }
        }

	     public void Delete(InventProduct inventProduct, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByID(db, inventProduct.ID);
                if (entity == null) return;
                db.InventProducts.DeleteObject(entity);
                db.SaveChanges();
            }
        }

        public void DeleteAllByDocumentHeaderCode(string documentCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.InventProducts.Where(e => e.DocumentCode == documentCode).ToList();
                if (entities == null) return;
                entities.ForEach(e => db.InventProducts.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public List<string> DeleteAllNotExistInCatalog(string pathDB)
		{
			List<string> documentCodeList = new List<string>();
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				int fromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
				var entities = db.InventProducts.Where(e => e.FromCatalogType == fromCatalogType).ToList();
				if (entities == null) return null;
				documentCodeList = entities.Select(x => x.DocumentHeaderCode).Distinct().ToList();
				entities.ForEach(e => db.InventProducts.DeleteObject(e));
				db.SaveChanges();
			}
			return documentCodeList; 
		}

        public void Insert(InventProduct inventProduct, DocumentHeader documentHeader, string pathDB)
        {
			if (inventProduct == null) return;
			if (documentHeader == null) return;
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                inventProduct.CreateDate = DateTime.Now;
                var entity = inventProduct.ToEntity();
                entity.DocumentCode = documentHeader.DocumentCode;
                db.InventProducts.AddObject(entity);
                db.SaveChanges();
            }
        }

        public void Insert(InventProduct inventProduct, string pathDB)
        {
			if (inventProduct == null) return;
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                inventProduct.CreateDate = DateTime.Now;
                var entity = inventProduct.ToEntity();
                db.InventProducts.AddObject(entity);
                db.SaveChanges();
            }
        }



		public void Insert(InventProducts inventProducts, string pathDB)
		{
			if (inventProducts == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
			
				foreach (var inventProduct in inventProducts)
				{
					inventProduct.CreateDate = DateTime.Now;
					var entity = inventProduct.ToEntity();
					db.InventProducts.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		public void InsertClone(InventProducts inventProducts, string pathDB)
		{
			if (inventProducts == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{

				foreach (var inventProduct in inventProducts)
				{
					var entity = inventProduct.ToEntity();
					db.InventProducts.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		/// <summary>
		/// вставить список в документ, без дублирования кодов, старые IP заменить на новые с тем же кодом
		/// </summary>
		/// <param name="newInventProducts"></param>
		/// <param name="toDocumentHeaderCode"></param>
		/// <param name="pathDB"></param>
		public void InsertOrUpdate(InventProducts newInventProducts, string toDocumentHeaderCode, string pathDB)
		{								
			if (newInventProducts == null) return;
			Dictionary<string, InventProduct> dictionaryOldInvent = new Dictionary<string, InventProduct>();

			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var oldEntities = db.InventProducts.Where(e => e.DocumentCode.CompareTo(toDocumentHeaderCode) == 0).ToList();
				if (oldEntities != null)
				{
					foreach (var entitie in oldEntities) 
					{
						dictionaryOldInvent[entitie.Barcode] = entitie.ToDomainObject();
					}
				}
	
				foreach (var inventProduct in newInventProducts)		 //надо зараннее заполнить все поля и DocumentCode, IturCode, ERPIturCode
				{
					if (dictionaryOldInvent.ContainsKey(inventProduct.Barcode) == true)
					{
						inventProduct.ModifyDate = DateTime.Now;
					}
					dictionaryOldInvent[inventProduct.Barcode] = inventProduct;
				}

				this.DeleteAllByDocumentHeaderCode(toDocumentHeaderCode, pathDB);

				foreach (KeyValuePair<string, InventProduct> keyValuePair in dictionaryOldInvent)
				{
					InventProduct ip = keyValuePair.Value;
					var entity = ip.ToEntity();
					db.InventProducts.AddObject(entity);
				}

				db.SaveChanges();
			}
		}

        public void Update(InventProduct inventProduct, string pathDB)
        {
			if (inventProduct == null) return;
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByID(db, inventProduct.ID);
                if (entity == null) return;
                entity.ApplyChanges(inventProduct);
                db.SaveChanges();
            }
        }

        public long CountInventProduct(string pathDB)
        {
            long count = 0;
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                try
                {
                    count = db.InventProducts.LongCount();
                }
                catch
                {
                }
                return count;
            }
        }

        public InventProduct GetInventProductByMakat(string makat, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByMakat(db, makat);
                if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

        //public Dictionary<string, InventProduct> GetInventProductDictionary(string pathDB, bool refill = false)
        //{
        //    if (refill == true)
        //    {
        //        this.FillInventProductDictionary(pathDB);
        //    }
        //    return this._inventProductList;
        //}

        //public void FillInventProductList(string pathDB)
        //{
        //    this.ClearInventProductDictionary();
        //    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
        //    {
        //        try
        //        {
        //            InventProducts inventProducts = this.GetInventProducts(pathDB);

        //            //this._inventProductDictionary = inventProducts.Select(e => e).Distinct().ToDictionary(k => k.Makat);
        //        }
        //        catch { }
        //    }
        //}

        //public void ClearInventProductDictionary()
        //{
        //    this._inventProductList.Clear();
        //    GC.Collect();
        //}

        #endregion

        #region statusBit

        public void ClearStatusBit(string pathDB)
        {
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.InventProducts.Select(e => e);
                if (entities == null) return;
                foreach (var entity in entities)
                {
                    entity.StatusInventProductBit = 0;
                }
                db.SaveChanges();
            }
        }

        public void ClearStatusBit(string documentCode, string pathDB)
        {
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.InventProducts.Select(e => e).Where(e => e.DocumentCode == documentCode);
                if (entities == null) return;
                foreach (var entity in entities)
                {
                    entity.StatusInventProductBit = 0;
                }
                db.SaveChanges();
            }
        }

        public void SetStatusBitByID(long ID, int bit, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByID(db, ID);
                if (entity == null) return;
                entity.StatusInventProductBit = entity.StatusInventProductBit + bit;
                db.SaveChanges();
            }
        }

        public int GetStatusBitByID(long ID, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByID(db, ID);
                if (entity == null) return 0;
                return entity.StatusInventProductBit;
            }
        }


        public int[] GetStatusBitArrayIntByDocumentCode(string documentCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.InventProducts.Where(e => e.DocumentCode == documentCode).
                    ToList().Select(e => e.StatusInventProductBit).ToArray();
                if (entities == null) return null;
                return entities;
            }
        }

        public List<BitArray> GetStatusBitArrayListByDocumentCode(string documentCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.InventProducts.Where(e => e.DocumentCode == documentCode).
                    ToList().Select(e => e.StatusInventProductBit).ToArray();
                if (entities == null) return null;
                return BitStatus.GetBitList(entities);
            }
        }


        public BitArray GetResultStatusBitOrByDocumentCode(string documentCode, string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.InventProducts.Where(e => e.DocumentCode == documentCode).
                    ToList().Select(e => e.StatusInventProductBit).ToArray();
                if (entities == null) return null;
                return BitStatus.GetResultBitArrayOr(entities);
            }
        }

        public string GetCountMakatTotal(AnalezeValueTypeEnum resulteCode, string pathDB)
        {
            string ret = "";
            string B = InputTypeCodeEnum.B.ToString();
            string K = InputTypeCodeEnum.K.ToString();
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                if (resulteCode == AnalezeValueTypeEnum.CountItems_Total)
                {
                    //var entities = db.InventProducts.Select(e => e.Makat).ToArray();
                    var count = db.InventProducts.LongCount();
                    ret = count.ToString();
                }
                else if (resulteCode == AnalezeValueTypeEnum.CountItems_InsertManually)
                {
                    var entities = db.InventProducts.Where(e => e.InputTypeCode == K).LongCount();
					
                    //    ToList().Select(e => e.Makat).ToArray();
                    //var count = entities.LongCount();
                    //ret = count.ToString();
					try
					{
						ret = entities.ToString();
					}
					catch { ret = "0"; }
                }
                else if (resulteCode == AnalezeValueTypeEnum.CountItems_InsertFromBarcode)
                {
                    var entities = db.InventProducts.Where(e => e.InputTypeCode == B).LongCount();
                    //ToList().Select(e => e.Makat).ToArray();
                    //var count = entities.LongCount();
                    //ret = count.ToString();
					try
					{
                    ret = entities.ToString();
					}
					catch { ret = "0"; }
                }
                if (resulteCode == AnalezeValueTypeEnum.CountERPMakats_Total)
                {
                    string typeMakat = TypeMakatEnum.M.ToString();
                    var entities = db.Products.Where(e => e.BalanceQuantityERP != 0 && e.TypeCode.CompareTo(typeMakat) == 0)
                        .Select(e => e.Makat).ToArray();
                    var count = entities.Distinct().LongCount();
                    ret = count.ToString();
                }
                if (resulteCode == AnalezeValueTypeEnum.CountPDAMakats_Total)
                {
                    var entities = db.InventProducts.Where(e => e.QuantityEdit != 0).Select(e => e.Makat).ToArray();
                    var count = entities.Distinct().LongCount();
                    ret = count.ToString();
                }
				//if (resulteCode == AnalezeValueTypeEnum.SumDifferenceValueGrate0_Total)
				//{
				//    var entities = db.IturAnalyzes.Where(e => e.QuantityEdit != 0).Select(e => e.Makat).ToArray();
				//    var count = entities.Distinct().LongCount();
				//    ret = count.ToString();
				//}

            }
            return ret;
        }

		public List<string> GetDocumentCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.InventProducts.Select(e => e.DocumentCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetDocumentCodeList", exp);
				}
			}
			return ret;
		}

        #endregion

        #region private

        private App_Data.InventProduct GetEntityByCode(App_Data.Count4UDB db, string inventProductCode)
        {
            var entity = db.InventProducts.FirstOrDefault(e => e.Code == inventProductCode);
            return entity;
        }

        private App_Data.InventProduct GetEntityByID(App_Data.Count4UDB db, long ID)
        {
            var entity = db.InventProducts.FirstOrDefault(e => e.ID == ID);
            return entity;
        }

        private App_Data.InventProduct GetEntityByMakat(App_Data.Count4UDB db, string makat)
        {
            var entity = db.InventProducts.FirstOrDefault(e => e.Makat == makat);
            return entity;
        }

        private App_Data.InventProduct GetEntityByBarcode(App_Data.Count4UDB db, string barcode)
        {
            var entity = db.InventProducts.FirstOrDefault(e => e.Barcode == barcode);
            return entity;
        }

        #endregion


        public bool IsAnyInventProductInDb(string pathDB)
        {
            using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                return db.InventProducts.Any();
            }
        }

		public bool IsAnyInventProductInIturCode(string iturCode, string pathDB)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				bool contance = db.InventProducts.Where(x=>x.IturCode == iturCode).Any();
				return contance;
			}
		}
	}
}
