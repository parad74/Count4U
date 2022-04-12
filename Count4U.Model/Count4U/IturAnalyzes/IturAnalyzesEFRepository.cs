using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Practices.ServiceLocation;
using System.Data;

namespace Count4U.Model.Count4U
{
    public class IturAnalyzesEFRepository : BaseEFRepository, IIturAnalyzesRepository
    {
		//protected readonly IIturAnalyzesADORepository _iturAnalyzesADORepository;
		protected readonly IServiceLocator _serviceLocator;
		protected readonly IIturRepository _iturRepository;
		protected readonly IDocumentHeaderRepository _documentHeaderRepository;
		protected readonly ILocationRepository _locationRepository;
		protected readonly IInventProductRepository _inventProductRepository;
		protected readonly IStatusIturRepository _statusIturRepository;
		protected readonly IStatusIturGroupRepository _statusIturGroupRepository;
		protected readonly IProductRepository _productRepository;
		protected readonly IMakatRepository _makatProductRepository;

		private Dictionary<string, Itur> _iturDictionary;
		private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		private Dictionary<string, Location> _locationDictionary;
		private Dictionary<int, IturStatusEnum> _statusIturDictionary;
		private Dictionary<int, IturStatusGroupEnum> _statusIturGroupDictionary;
		private Dictionary<string, ProductMakat> _productMakatDictionary;
		private Dictionary<string, ProductSimple> _productSimpleDictionary;

		private InventProducts _inventProductList;

		public IturAnalyzesEFRepository(ConnectionDB connection,
			IServiceLocator serviceLocator,
			//IIturAnalyzesADORepository iturAnalyzesADORepository,
			IIturRepository iturRepository,
		IDocumentHeaderRepository documentHeaderRepository,
		ILocationRepository locationRepository,
		IInventProductRepository inventProductRepository,
		IStatusIturRepository statusIturRepository,
		IStatusIturGroupRepository statusIturGroupRepository,
		IProductRepository productRepository,
		IMakatRepository makatProductRepository)
			: base(connection)
		{
			this._serviceLocator = serviceLocator;
			//this._iturAnalyzesADORepository = iturAnalyzesADORepository;
			this._iturRepository = iturRepository;
			this._documentHeaderRepository = documentHeaderRepository;
			this._locationRepository = locationRepository;
			this._inventProductRepository = inventProductRepository;
			this._statusIturRepository = statusIturRepository;
			this._statusIturGroupRepository = statusIturGroupRepository;
			this._productRepository = productRepository;
			this._makatProductRepository = makatProductRepository;

			this._iturDictionary = new Dictionary<string,Itur>();
			this._documentHeaderDictionary = new Dictionary<string,DocumentHeader>();
			this._locationDictionary = new Dictionary<string,Location>();
			this._inventProductList = new InventProducts();
			this._statusIturDictionary = new Dictionary<int, IturStatusEnum>();
			this._statusIturGroupDictionary = new Dictionary<int, IturStatusGroupEnum>();
			this._productMakatDictionary = new Dictionary<string, ProductMakat>();
			this._productSimpleDictionary = new Dictionary<string, ProductSimple>();
		}

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }
        
        #endregion

		#region IIturAnalyzesRepository Members

		public IturAnalyzesCollection GetIturAnalyzesCollection(string pathDB, bool refill = true, bool refillCatalogDictionary = false,
			Dictionary<object, object> parms = null)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				if (refill == true)
				{
					IIturAnalyzesSourceRepository iturAnalyzesSourceRepository  =
						 this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
					iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
					iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, null, parms);
				}
				//var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToDomainObject());
				//return IturAnalyzesCollection.FromEnumerable(domainObjects);

				var entities = AsQueryable(db.IturAnalyzes).ToList();
				var result = IturAnalyzesCollection.FromEntityList(entities);
				//var domainObjects = entities.Select(e => e.ToDomainObject());
				//var result = IturAnalyzesCollection.FromEnumerable(domainObjects);
				result.TotalCount = entities.LongCount();
				result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				return result;
            }
		}

		public IturAnalyzesCollection GetIturAnalyzesSumCollection(
		SelectParams selectParams,
		string pathDB, bool refill = true,
		 bool refillCatalogDictionary = false, 
		Dictionary<object, object> parms = null)
		{
			if (refill == true)
			{
				IIturAnalyzesSourceRepository iturAnalyzesSourceRepository = this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzesSumSimple(pathDB, refill, refillCatalogDictionary, null, parms);
			}//refill

			//=================	 GetEntities =====================================================
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					long totalCount = 0;
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var result = IturAnalyzesCollection.FromEntityList(entities);
					result.TotalCount = totalCount;
		
					if (parms != null)
					{
						if (parms.ContainsKey("SelectParams") == true)
						{
							object sp = parms["SelectParams"];
							if (sp != null)
							{
								if (sp is SelectParams)
								{
									SelectParams sp1 = (SelectParams)sp;
									var entitiesWithoutPage = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(), sp1, out totalCount);
									result.SumQuantityEdit = entitiesWithoutPage.Sum(e => e.QuantityEdit);
								}
							}
						}
					}

					return result;
				}
				else
				{
					var entities = AsQueryable(db.IturAnalyzes).ToList();
					var result = IturAnalyzesCollection.FromEntityList(entities);
					//var domainObjects = entities.Select(e => e.ToDomainObject());
					//var result = IturAnalyzesCollection.FromEnumerable(domainObjects);
					result.TotalCount = entities.LongCount();
					result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit); 
					return result;
				}
			}
		}


		public IEnumerable<IturAnalyzesSimple> GetIturAnalyzesSumEnumerable(
			SelectParams selectParams,
			string pathDB, bool refill = true,
			bool refillCatalogDictionary = false,
			Dictionary<object, object> parms = null,
			bool addResult = true,
			List<ImportDomainEnum> importType = null)
		{
			if (refill == true)
			{
				IIturAnalyzesSourceRepository iturAnalyzesSourceRepository = this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzesSumSimple(pathDB, refill, refillCatalogDictionary, null, parms, addResult, importType);
			}//refill

			//=================	 GetEntities =====================================================
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					long totalCount = 0;
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(), selectParams, out totalCount);

					//var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatOriginalDomainObject());

					var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
					return domainObjects;
				}
				else
				{
					var entities = AsQueryable(db.IturAnalyzes).ToList();
					var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
					return domainObjects;
				}
			}
		}


		//public IEnumerable<IturAnalyzesSimple> GetIturAnalyzesSumMakatOriginalEnumerable(
		//SelectParams selectParams,
		//string pathDB, bool refill = true,
		//bool refillCatalogDictionary = false,
		//Dictionary<object, object> parms = null)
		//{
		//	if (refill == true)
		//	{
		//		IIturAnalyzesSourceRepository iturAnalyzesSourceRepository = this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
		//		iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
		//		iturAnalyzesSourceRepository.InsertIturAnalyzesSumSimple(pathDB, refill, refillCatalogDictionary, null, parms);
		//	}//refill

		//	//=================	 GetEntities =====================================================
		//	using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//	{
		//		if (selectParams != null)
		//		{
		//			long totalCount = 0;
		//			var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(), selectParams, out totalCount);

		//			//var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatOriginalDomainObject());

		//			var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
		//			return domainObjects;
		//		}
		//		else
		//		{
		//			var entities = AsQueryable(db.IturAnalyzes).ToList();
		//			var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
		//			return domainObjects;
		//		}
		//	}
		//}

		/// <summary>
		/// Вызывается из созадания отчетов - только?!
		/// </summary>
		/// <param name="levelInAnalyzes"></param>
		/// <param name="selectParams"></param>
		/// <param name="pathDB"></param>
		/// <param name="refill"></param>
		/// <param name="simpleListOrSum"></param>
		/// <returns></returns>
		public IturAnalyzesCollection GetIturAnalyzesCollection(
			LevelInAnalyzesEnum levelInAnalyzes,
			SelectParams selectParams,
			string pathDB, bool refill = true,
			bool refillCatalogDictionary = false,
			IturAnalyzeTypeEnum simpleListOrSum = IturAnalyzeTypeEnum.Full,
			Dictionary<object, object> parms = null)
		{
			if (selectParams == null)
				return this.GetIturAnalyzesCollection(pathDB, refill, refillCatalogDictionary);

			if (refill == true)
			{
				this.ReBuildSelectParamByLavel(levelInAnalyzes, ref selectParams, pathDB);

				IIturAnalyzesSourceRepository iturAnalyzesSourceRepository = this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);

				//=========================	 simpleListOrSum  ======================

				if (simpleListOrSum == IturAnalyzeTypeEnum.Simple)
				{
					iturAnalyzesSourceRepository.InsertIturAnalyzesSimple(pathDB, true, refillCatalogDictionary, selectParams, parms);
				}
				else if (simpleListOrSum == IturAnalyzeTypeEnum.Full)
				{
					iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, selectParams, parms);
				}
				else if (simpleListOrSum == IturAnalyzeTypeEnum.SimpleSum)				 //суммы без деления на Iturs and Locations \ by Makat
				{
					if (levelInAnalyzes == LevelInAnalyzesEnum.Iturs)
					{
						// ???? selectParams = null;	надо закометарить когда исправлю	 
						//selectParams = null;		//???	 убрала недавно	при рефакторинге для поиска	перестало искаться
						// проблемы если в SelectParam содержит параметры из InventProduct но вдруг не заполнены 
						// (это может быть из формы печати) LocationCode, familyCode, suppliearCode, sectionCode и т.д
 						// поэтому сначала все просумиросать и заполнить, а затем (ниже в функции) уже отбирать  по SelectParam
						iturAnalyzesSourceRepository.InsertIturAnalyzesSumSimple(pathDB, true, refillCatalogDictionary, /*selectParams*/null, parms);
					}
					else
					{
						iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, selectParams, parms);
					}
				}
				else if (simpleListOrSum == IturAnalyzeTypeEnum.FullFamilySortLocationIturMakat)
				{
					iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, selectParams, parms);
				}
				else
				{
					iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, selectParams, parms);
				}

			}//refill

			//=================	 GetEntities =====================================================
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (simpleListOrSum != IturAnalyzeTypeEnum.FullFamilySortLocationIturMakat)
				{
					if (selectParams != null)
					{
						long totalCount = 0;
						db.ContextOptions.ProxyCreationEnabled = false;
						db.IturAnalyzes.MergeOption = MergeOption.NoTracking;
						var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
							selectParams, out totalCount);
						var entitiesList = entities.ToList();
						var result = IturAnalyzesCollection.FromEntityList(entitiesList);
						//var domainObjects = entities.Select(e => e.ToDomainObject());
						//var result = IturAnalyzesCollection.FromEnumerable(domainObjects);
						result.TotalCount = totalCount;
						result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
						return result;
					}
					else
					{
						db.ContextOptions.ProxyCreationEnabled = false;
						db.IturAnalyzes.MergeOption = MergeOption.NoTracking;
						var entities = AsQueryable(db.IturAnalyzes).ToList();
						var result = IturAnalyzesCollection.FromEntityList(entities);
						//var domainObjects = entities.Select(e => e.ToDomainObject());
						//var result = IturAnalyzesCollection.FromEnumerable(domainObjects);
						result.TotalCount = entities.LongCount();
						result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
						return result;
					}
				}

				// ------------------ FullFamilySortLocationIturMakat
				else 
				{
					// var filteredUsers = Users.Where(u => u.Active && u.AllowLogin && !u.Loggedln) .OrderBy( u => u.Name).ThenBy(u => u.Location);

					//					var sts = new List<Student> { new Student { Name = "Vasya", Fak = "U", Mark = 5 } };
					//var res =sts.Where(x=>x.Mark>3).GroupBy(x => x.Fak).Select(x => new
					//{
					//	Fak = x.Key,
					//	Students = x.GroupBy(y => y.Mark).Select(y =>new
					//	{
					//		Mark = y.Key,
					//		Count = y.Count()
					//	}).ToList()
					//}).ToList();
					//res.ForEach(x=> x.Students.ForEach(y=> Console.WriteLine("{0} - {1} - {2}", x.Fak, y.Mark, y.Count)));
					//-----------------

					List<App_Data.IturAnalyzes> entities = new List<App_Data.IturAnalyzes>();
					if (selectParams != null)
					{
						long totalCount = 0;
						entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
							selectParams, out totalCount);
					}
					else
					{
						entities = AsQueryable(db.IturAnalyzes).ToList();
					}

					var entitiesGroup = entities.GroupBy(x => x.LocationCode).Select(x => new
					{
						_locationCode = x.Key,
						_locationName = x.Max(x1 => x1.LocationName),
						Iturs = x.GroupBy(y => y.IturCode).Select(y => new
						{
							_iturCode = y.Key,
							Makats = y.GroupBy(z => z.Makat).Select(z => new
							{
								_makat = z.Key,
								_familyCode = z.Max(s => s.FamilyCode),
								_familyName = z.Max(s => s.FamilyName),
								_familySize = z.Max(s => s.FamilySize),
								_familyType = z.Max(s => s.FamilyType),
								_sumQuantityEdit = z.Sum(s => s.QuantityEdit)
							}).ToList()
						}).ToList()
					}).ToList();

					IturAnalyzesCollection res = new IturAnalyzesCollection();
					entitiesGroup.ForEach(x => x.Iturs.ForEach(
						y => y.Makats.ForEach(z =>
							res.Add(new IturAnalyzes
							{
								LocationCode = x._locationCode,
								LocationName = x._locationName,
								IturCode = y._iturCode,
								Makat = z._makat,
								FamilyCode = z._familyCode,
								FamilyName = z._familyName,
								FamilyType = z._familyType,
								FamilySize = z._familySize,
								QuantityEdit = z._sumQuantityEdit,
							}))));


					IEnumerable<IturAnalyzes> sortedList = res.OrderBy(x => x.LocationCode).ThenBy(x => x.IturCode).ThenBy(x => x.Makat).AsEnumerable<IturAnalyzes>();
					//IEnumerable<IturAnalyzes> sortedList = res.OrderBy(x => x.Makat).AsEnumerable<IturAnalyzes>();
					IturAnalyzesCollection result = IturAnalyzesCollection.FromEnumerable(sortedList);

					result.TotalCount = entities.LongCount();
					result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
					return result;
					//}
					//else
					//{
					//	var entities = AsQueryable(db.IturAnalyzes).ToList();
					//	var result = IturAnalyzesCollection.FromEntityList(entities);
					//	result.TotalCount = entities.LongCount();
					//	result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
					//	return result;
					//}
				}
			}//using
		}
		


		private void ReBuildSelectParamByLavel(LevelInAnalyzesEnum levelInAnalyzes, 
			ref SelectParams selectParams, string pathDB)
		{

			//!!!! TODO - 74p - error - to much Iturs
			//if (levelInAnalyzes == LevelInAnalyzesEnum.Iturs ||
			 if(levelInAnalyzes == LevelInAnalyzesEnum.Itur)
			{
				using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					try
					{
						long totalCount = 0;
						var entities = this.GetEntities(db, AsQueryable(db.Iturs), db.Iturs.AsQueryable(),
							selectParams, out totalCount);
						List<string> searchItur = entities.Select(r => r.IturCode).Distinct().ToList();
						SelectParams sp = new SelectParams();
						if (searchItur.Count != 0)
						{
							sp.FilterStringListParams.Add("IturCode", new FilterStringListParam()
							{
								Values = searchItur
							});
						}
						selectParams = sp;
					}
					catch { }
				}
			}
			//else if (viewContext == ReportDomainContextEnum.Itur)
			//{
			//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			//    {
			//        try
			//        {
			//            var entities = this.GetEntities(db, AsQueryable(db.Iturs), db.Iturs.AsQueryable(),
			//                selectParams, out totalCount);
			//            List<string> searchItur = entities.Select(r => r.IturCode).Distinct().ToList();
			//            SelectParams sp = new SelectParams();
			//            if (searchItur.Count != 0)
			//            {
			//                sp.FilterStringListParams.Add("IturCode", new FilterStringListParam()
			//                {
			//                    Values = searchItur
			//                });
			//            }
			//            selectParams = sp;
			//        }
			//        catch { }
			//    }
			//}
			else 
			if (levelInAnalyzes == LevelInAnalyzesEnum.InventProduct)
			{
				using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					try
					{
						long totalCount = 0;
						var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(),
							selectParams, out totalCount);
						List<string> searchDoc = entities.Select(r => r.DocumentCode).Distinct().ToList();		 //up from InventProduct to Doc

						SelectParams spDoc = new SelectParams();
						if (searchDoc.Count != 0)
						{
							spDoc.FilterStringListParams.Add("DocumentCode", new FilterStringListParam()
							{
								Values = searchDoc
							});

							//=============== add InventProductNums to selectParms

							List<int> searchInventProductNums = entities.Select(r => r.IPNum).Distinct().ToList();
							List<int> searchInventProduct = new List<int>();
							foreach (var searchInventProductNum in searchInventProductNums)
							{
								searchInventProduct.Add(Convert.ToInt32(searchInventProductNum));
							}

							if (searchDoc.Count != 0)
							{
								spDoc.FilterIntListParams.Add("IPNum", new FilterIntListParam()
								{
									Values = searchInventProduct
								});
							}
						}
						selectParams = spDoc;
					}
					catch { }
				}
			}
			//else if (viewContext == ReportDomainContextEnum.Doc)
			//{
			//using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			//{
			//    try
			//    {
			//        var entities = this.GetEntities(db, AsQueryable(db.DocumentHeaders), db.DocumentHeaders.AsQueryable(),
			//            selectParams, out totalCount);
			//        List<string> searchItur = entities.Select(r => r.IturCode).Distinct().ToList();

			//        SelectParams sp = new SelectParams();
			//        if (searchItur.Count != 0)
			//        {
			//            sp.FilterStringListParams.Add("IturCode", new FilterStringListParam()
			//            {
			//                Values = searchItur
			//            });
			//        }
			//        selectParams = sp;
			//    }
			//    catch { }
			//}
			//}
		}


		public IturAnalyzesCollection GetIturAnalyzesTotal(string pathDB, SelectParams selectParams = null)
		{
			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams == null)
				{
					IturAnalyzesCollection result = new IturAnalyzesCollection();
					result.TotalCount = db.IturAnalyzes.LongCount();
					result.SumQuantityEdit = db.IturAnalyzes.Sum(e => e.QuantityEdit);
					return result;
				}
				else
				{
					var entities = GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
                    selectParams, out totalCount);
					IturAnalyzesCollection result = new IturAnalyzesCollection();
					result.TotalCount = totalCount;
					result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				    return result;
  				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="selectParams"></param>
		/// <param name="pathDB"></param>
		/// <param name="refill"></param>
		/// <returns></returns>
		public IturAnalyzesCollection GetIturAnalyzesCollection(SelectParams selectParams,
			string pathDB, bool refill = true, bool refillCatalogDictionary = false, Dictionary<object, object> parms = null)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
		
			long totalCount = 0;
			if (refill == true)
			{
					iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
					iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, selectParams, parms);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{

				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),selectParams, out totalCount);
					var result = IturAnalyzesCollection.FromEntityList(entities);
					result.TotalCount = totalCount;
					
					if (parms != null){
						if (parms.ContainsKey("SelectParams") == true)
						{
							object sp = parms["SelectParams"];
							if (sp != null)
							{
								if (sp is SelectParams)
								{
									SelectParams sp1 = (SelectParams)sp;
									var entitiesWithoutPage = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(), sp1, out totalCount);
									result.SumQuantityEdit = entitiesWithoutPage.Sum(e => e.QuantityEdit);
								}
							}
						}
					}
				return result;
				}
				else
				{
					var entities = AsQueryable(db.IturAnalyzes).ToList();
					var result = IturAnalyzesCollection.FromEntityList(entities);
					result.TotalCount = entities.LongCount();
					result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
					return result;
				}
			}
		}

		//================ IAFull
		//-----------------------------------------------By
		//=================from  IturAnalyzes 
		//-----------------------------------------------by InsertIturAnalyzes(pathDB, true, selectParams);
		//WriteToFile - WriteRow - All - iturAnalyzes
		public IturAnalyzesCollection GetIACollection(SelectParams selectParams,  string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository = this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{

				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var result = IturAnalyzesCollection.FromEntityList(entities);
					//var domainObjects = entities.Select(e => e.ToDomainObject());
					//var result = IturAnalyzesCollection.FromEnumerable(domainObjects);
					result.TotalCount = totalCount;
					result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
					return result;
				}
				else
				{
					var entities = AsQueryable(db.IturAnalyzes).ToList();
					var result = IturAnalyzesCollection.FromEntityList(entities);
					//var domainObjects = entities.Select(e => e.ToDomainObject());
					//var result = IturAnalyzesCollection.FromEnumerable(domainObjects);
					result.TotalCount = entities.LongCount();
					result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
					return result;
				}
			}
		}
		//================ IPSumQuantityEdit
		//-----------------------------------------------ByMakatsOriginal
		//=================from  IturAnalyzes 
		//-----------------------------------------------by InsertIturAnalyzes(pathDB, true, selectParams);
		//WriteToFile
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsOriginal(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> param = null)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp, param);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{

				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatOriginalDomainObject());
					var domainObjectsSum = from e in domainObjects
																	orderby e.Makat
																	group e by e.Makat into g
																	select new IturAnalyzesSimple
																	{
																		Makat = g.Key,													  // хранит 	MakatOriginal
																		MakatOriginal = g.Max(x => x.MakatOriginal),				// хранит 	Makat
																		QuantityEdit = g.Sum(x => x.QuantityEdit),
																		Count = g.Count()
																	};
					//var result = IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					//result.TotalCount = totalCount;
					//return result;
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatOriginalDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   MakatOriginal = g.Max(x => x.MakatOriginal),
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					return domainObjectsSum;
				}
			}
		}

		//================ IPSumQuantityEdit
		//-----------------------------------------------ByMakats
		//=================from  IturAnalyzes 
		//-----------------------------------------------by InsertIturAnalyzesSimple(pathDB, true, selectParams);
		//WriteToFile
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakats(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				//iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams); test
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true,refillCatalogDictionary, sp, parms);			//!!
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby  e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit), 
											   Count = g.Count()
										   };

					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					return domainObjectsSum;
				}
			}
		}


		//================ IPSumQuantityEdit
		//-----------------------------------------------ByMakats and Barcode
		//=================from  IturAnalyzes 
		//-----------------------------------------------by InsertIturAnalyzesSimple(pathDB, true, selectParams);
		//WriteToFile
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsBarcode(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				//iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams); test
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp, parms);			//!!
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatBarcodeDomainObject());
					
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   Barcode = g.Max(x => x.Barcode),
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//var result = IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					//result.TotalCount = totalCount;
					//return result;
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatBarcodeDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   Barcode = g.Max(x => x.Barcode),
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					return domainObjectsSum;
				}
			}
		}//??


		//================ IPSumQuantityEdit
		//-----------------------------------------------ByMakatsOriginal and Barcode
		//=================from  IturAnalyzes 
		//-----------------------------------------------by InsertIturAnalyzesSimple(pathDB, true, selectParams);
		//WriteToFile
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsOriginalBarcode(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();

			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				//iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams); test
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp, parms);			//!!
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
					selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatOriginalBarcodeDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   Barcode = g.Max(x => x.Barcode),
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//var result = IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					//result.TotalCount = totalCount;
					//return result;
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatOriginalBarcodeDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   Barcode = g.Max(x => x.Barcode),
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					return domainObjectsSum;
				}
			}
		}//??


		//================ IPSumQuantityEdit    IPSumQuantityOriginal
		//-----------------------------------------------ByMakats
		//=================from  InventProduct 
		//-----------------------------------------------by 
		//InsertIturAnalyzesSumSimple
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditDifferenceByMakats(SelectParams selectParams, 
			string pathDB)
		{
			//IIturAnalyzesADORepository iturAnalyzesADORepository =
			//        this._serviceLocator.GetInstance<IIturAnalyzesADORepository>();

			long totalCount = 0;
			//if (refill == true)
			//{
			//    iturAnalyzesADORepository.ClearIturAnalyzes(pathDB);
			//    iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams);
			//}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					//var result = IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					//result.TotalCount = totalCount;
					//return result;
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.InventProducts).ToList().Select(e => e.ToSimpleDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityOriginal = g.Sum(x => x.QuantityOriginal),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					//return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					return domainObjectsSum;
				}
			}
		}

		//================ IPSumQuantityEdit		IPSumQuantityInPackEdit	 
		//-----------------------------------------------ByMakats
		//=================from  IturAnalyzes// было InventProduct 
		//-----------------------------------------------by 
		//ExportInventProductYarpaERPFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakats(SelectParams selectParams, string pathDB)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refill = true;
			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
						return domainObjectsSum;
				}
			}
		}


	
		//================ IPSumQuantityEdit		IPSumQuantityInPackEdit	 
		//-----------------------------------------------ByMakatsOriginal
		//=================from  IturAnalyzes 
		//-----------------------------------------------by 
		//WriteToFile
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsOriginal(SelectParams selectParams,
			string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();

			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
					selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatOriginalDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatOriginalDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
			}
		}
		

		//--------
		//================ IPSumQuantityEdit		IPSumQuantityInPackEdit	LikeNumber 
		//-----------------------------------------------ByMakats                             LikeNumber
		//=================from  IturAnalyzes// было InventProduct 
		//-----------------------------------------------by 
		//ExportInventProductMaccaibiPaharmFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsLikeNumber(SelectParams selectParams, string pathDB)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refill = true;
			bool refillCatalogDictionary = true;
			long totalCount = 0;

			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatNumberDomainObject()); //  ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.MakatLong
										   group e by e.MakatLong into g
										   select new IturAnalyzesSimple
										   {
											   MakatLong = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatNumberDomainObject());//ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.MakatLong
										   group e by e.MakatLong into g
										   select new IturAnalyzesSimple
										   {
											   MakatLong = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit), 
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
			}
		}


		//================ IPSumQuantityEdit		IPSumQuantityInPackEdit	with  (QuantityEdit  without add QuantityInPackEdit) 
		//-----------------------------------------------ByMakats
		//=================from ExportInventProductAS400LeumitERPFileWriter
		//-----------------------------------------------by 
		//ExportInventProductYarpaERPFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsWithoutAddQuantityInPackEdit(SelectParams selectParams, string pathDB)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refill = true;
			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.IPValueFloat5),		//QuantityEdit without QuantityInPackEdit
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.IPValueFloat5),		//QuantityEdit without QuantityInPackEdit
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
			}
		}

		//================ IPSumQuantityEdit		IPSumQuantityInPackEdit	with  (QuantityEdit  without QuantityInPackEdit)
		//-----------------------------------------------ByMakatsOriginal
		//=================from  ExportInventProductAS400LeumitERPFileWriter 
		//-----------------------------------------------by 
		//WriteToFile
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsOriginalWithoutAddQuantityInPackEdit(SelectParams selectParams,
			string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();

			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
					selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatOriginalDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.IPValueFloat5),		//QuantityEdit without QuantityInPackEdit
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatOriginalDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.IPValueFloat5),		//QuantityEdit without QuantityInPackEdit
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
			}
		}

		//--------
		//================ IPSumQuantityEdit		IPSumQuantityMakatAndExpiredDate	ExpiredDate
		//-----------------------------------------------ByMakats                            Expired Date
		//=================from  IturAnalyzes
		//-----------------------------------------------by 
		//ExportInventProductMade4NetFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsAndExpiredDate(SelectParams selectParams, string pathDB)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refill = true;
			bool refillCatalogDictionary = true;
			long totalCount = 0;

			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject()).ToList(); //  ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.ERPIturCode, e.Makat
										   group e by new{
											e.ERPIturCode,
										   e.Makat,
										   e.IPValueStr2
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key.Makat, 
											   Barcode = g.Key.IPValueStr2,								//ExpireDate string
											   IturCode = g.Key.ERPIturCode,							//ERPIturCode
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.IPValueInt1) //Quentetity in Box
										   };
					return domainObjectsSum;
				}

				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject());//ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.ERPIturCode, e.Makat
										   group e by new{
											e.ERPIturCode,
										   e.Makat,
										   e.IPValueStr2
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key.Makat, 
											   Barcode = g.Key.IPValueStr2,								//ExpireDate string
											   IturCode = g.Key.ERPIturCode,							//ERPIturCode
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.IPValueInt1)  //Quentetity in Box
										   };
					return domainObjectsSum;
				}
			}
		}


		//================ IPSumQuantityEdit		IPSumQuantityMakatAndIturCode
		//-----------------------------------------------ByMakats                            IturCode 
		//=================from  IturAnalyzes
		//-----------------------------------------------by 
		//ExportInventProductNibitFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsAndIturCode(SelectParams selectParams, string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = refill;
			long totalCount = 0;

			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, refill, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject()).ToList(); //  ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode, e.Makat
										   group e by new
										   {
											   e.IturCode,
											   e.Makat
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key.Makat,
											   MakatOriginal = g.Max(x => x.MakatOriginal),	
											   IturCode = g.Key.IturCode,							//IturCode
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}

				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject());//ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode, e.Makat
										   group e by new
										   {
											   e.IturCode,
											   e.Makat
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key.Makat,
											   MakatOriginal = g.Max(x => x.MakatOriginal),	
											   IturCode = g.Key.IturCode,							//IturCode
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
			}
		}

		//================ IPSumQuantityEdit		IPSumQuantityMakatAndIturCode
		//-----------------------------------------------ByMakats                            ERPIturCode 
		//=================from  IturAnalyzes
		//-----------------------------------------------by 
		//ExportInventProductNibitFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsAndERPIturCode(SelectParams selectParams, string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = refill;
			long totalCount = 0;

			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, refill, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject()).ToList(); //  ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.ERPIturCode, e.Makat
										   group e by new
										   {
											   e.ERPIturCode,
											   e.Makat
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key.Makat,
											   MakatOriginal = g.Max(x => x.MakatOriginal),
											   IturCode = g.Key.ERPIturCode,							//IturCode
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}

				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject());//ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.ERPIturCode, e.Makat
										   group e by new
										   {
											   e.ERPIturCode,
											   e.Makat
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key.Makat,
											   MakatOriginal = g.Max(x => x.MakatOriginal),
											   IturCode = g.Key.ERPIturCode,							//IturCode
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count(), 
										   };
					return domainObjectsSum;
				}
			}
		}

		//================ IPSumQuantityEdit		IPSumQuantityMakatAndIturCode
		//-----------------------------------------------ByMakats                            IturCode 
		//=================from  IturAnalyzes
		//-----------------------------------------------by 
		//ExportInventProductNibitFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByBakcodesAndIturCode(SelectParams selectParams, string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = refill;
			long totalCount = 0;

			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, refill, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject()).ToList(); //  ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode, e.Barcode
										   group e by new
										   {
											   e.IturCode,
											   e.Barcode					//barcode from PDA
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Barcode = g.Key.Barcode,
											   IturCode = g.Key.IturCode,							//IturCode
											   Makat = g.Max(x => x.Makat),
											   MakatOriginal = g.Max(x => x.MakatOriginal),	
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}

				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject());//ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode, e.Barcode
										   group e by new
										   {
											   e.IturCode,
											   e.Barcode				//barcode from PDA
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Barcode = g.Key.Barcode,
											   IturCode = g.Key.IturCode,							//IturCode
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Makat = g.Max(x => x.Makat),
											   MakatOriginal = g.Max(x => x.MakatOriginal),	
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
			}
		}

		///////
		//================ IPSumQuantityEdit		IPSumQuantityMakatAndIturCode
		//-----------------------------------------------ByMakats                            Bakcodes 
		//=================from  IturAnalyzes
		//-----------------------------------------------by 
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatAndBakcodes(SelectParams selectParams, string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
			this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = refill;
			long totalCount = 0;

			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, refill, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject()).ToList(); //  ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat, e.Barcode
										   group e by new
										   {
											   e.Makat,
											   e.Barcode					//barcode from PDA
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Barcode = g.Key.Barcode,
											   Makat = g.Key.Makat,							//Makat
											   MakatOriginal = g.Key.Makat,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   ProductName = g.Max(x => x.ProductName)
										   };
					return domainObjectsSum;
				}

				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatsAndExpiredDateDomainObject());//ToSimpleMakatDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat, e.Barcode
										   group e by new
										   {
											   e.Makat,
											   e.Barcode				//barcode from PDA
										   } into g
										   select new IturAnalyzesSimple
										   {
											   Barcode = g.Key.Barcode,
											   Makat = g.Key.Makat,							//IturCode
											   MakatOriginal = g.Key.Makat,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   ProductName = g.Max(x => x.ProductName)
										   };
					return domainObjectsSum;
				}
			}
		}


		//================ IPSumQuantityEdit		GetIPSumQuantityEditByMakatsByIturCode
		//----------------------------------------------- ByIturCode 
		//=================from  IturAnalyzes
		//-----------------------------------------------by 
		//ExportInventProductNibitFileWriter
		public IturAnalyzesCollection GetIPSumQuantityEditByIturCode(SelectParams selectParams, string pathDB, bool refillIturStatistic = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =	this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();

			//bool refill = true;
			//bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refillIturStatistic == true)
			{
				iturRepository.RefillIturStatistic(pathDB);
			}

			//if (refill == true)
			//{
			//	SelectParams sp = new SelectParams();
			//	iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
			//	iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			//}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//List<Itur> iturDomainObjects = null;
			//	Dictionary<string, App_Data.DocumentHeader> documentHeaderEntityDictionary = db.DocumentHeaders.Select(e => e).Distinct().ToDictionary(k => k.DocumentCode);
				var documentHeaderEntitis = AsQueryable(db.DocumentHeaders).ToList().Select(e => e).ToList();
				//List<App_Data.DocumentHeader> documentHeaderEntityDictionary = db.DocumentHeaders.Select(e => e).ToList();
				//var entities = GetEntities(db, AsQueryable(db.DocumentHeaders), db.DocumentHeaders.AsQueryable(),selectParams, out totalCount); // TODo if need selectParams

				//if (selectParams != null)
				//{
				//	var iturEntities = this.GetEntities(db, AsQueryable(db.Iturs), db.Iturs.AsQueryable(),
				//		selectParams, out totalCount);
				//	iturDomainObjects = iturEntities.Select(e => e.ToDomainObject()).ToList(); 
				//}
				//else
				//{
				List<Itur> iturDomainObjects = AsQueryable(db.Iturs).ToList().Select(e => e.ToDomainObject()).ToList();
				//}

				List<IturAnalyzes> IturAnalyzesList = new List<IturAnalyzes>();
				foreach (var itur in iturDomainObjects)
				{
					IturAnalyzes newIturAnalyzes = new IturAnalyzes { IturCode = itur.IturCode, Total = (long)itur.TotalItem, QuantityEdit = itur.SumQuantityEdit };
					var docsFromTime =  documentHeaderEntitis.Where(x=>x.IturCode == itur.IturCode).OrderBy(x => x.FromTime).Select(x => x).ToList();
					var docsToTime = documentHeaderEntitis.Where(x => x.IturCode == itur.IturCode).OrderBy(x => x.ToTime).Select(x => x).ToList();
					if (docsToTime != null)
					{
						if (docsToTime.Count != 0)
						{
								var docLast = docsToTime.LastOrDefault();
								newIturAnalyzes.ToTime = Convert.ToDateTime(docLast.ToTime);
								newIturAnalyzes.WorkerID = docLast.WorkerGUID;
						}
						if (docsFromTime != null)
						{
							if (docsFromTime.Count != 0)
							{
								var docFirst = docsFromTime.FirstOrDefault();
								newIturAnalyzes.FromTime = Convert.ToDateTime(docFirst.FromTime);
							}
						}
					}
					IturAnalyzesList.Add(newIturAnalyzes); 
				}

				return IturAnalyzesCollection.FromEnumerable(IturAnalyzesList);
			}
		}

		//================ IPSumQuantityEdit		IPSumQuantityInPackEdit	 	LikeNumber 
		//-----------------------------------------------ByMakatsOriginal						LikeNumber 
		//=================from  IturAnalyzes 
		//-----------------------------------------------by 
		//ExportInventProductMaccaibiPaharmFileWriter
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditInPackByMakatsOriginalLikeNumber(SelectParams selectParams,
			string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();

			long totalCount = 0;
			bool refillCatalogDictionary = true;
			if (refill == true)
			{
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp);
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
					selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatOriginalNumberDomainObject()); // ToSimpleMakatOriginalDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.MakatLong
										   group e by e.MakatLong into g
										   select new IturAnalyzesSimple
										   {
											   MakatLong = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatOriginalNumberDomainObject());//  ToSimpleMakatOriginalDomainObject
					var domainObjectsSum = from e in domainObjects
										   orderby e.MakatLong
										   group e by e.MakatLong into g
										   select new IturAnalyzesSimple
										   {
											   MakatLong = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   QuantityInPackEdit = g.Sum(x => x.QuantityInPackEdit),
											   Count = g.Count()
										   };
					return domainObjectsSum;
				}
			}
		}
		//--------

		public IEnumerable<IturAnalyzesSimple> GetPSumQuantityOriginalERPByMakats(SelectParams selectParams,  string pathDB)
		{
			//IIturAnalyzesADORepository iturAnalyzesADORepository =
			//        this._serviceLocator.GetInstance<IIturAnalyzesADORepository>();

			long totalCount = 0;
			//if (refill == true)
			//{
			//    iturAnalyzesADORepository.ClearIturAnalyzes(pathDB);
			//    //iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams); test
			//    iturAnalyzesADORepository.InsertIturAnalyzesSimple(pathDB, true, selectParams);
			//}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.Products), db.Products.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.BalanceQuantityERP) 
										   };
					//var result = IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					//result.TotalCount = totalCount;
					//return result;
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.Products).ToList().Select(e => e.ToSimpleDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.Makat
										   group e by e.Makat into g
										   select new IturAnalyzesSimple
										   {
											   Makat = g.Key,
											   QuantityEdit = g.Sum(x => x.BalanceQuantityERP)
										   };
					//return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					return domainObjectsSum;
				}
			}
		}


		////================ IPSumQuantityEdit
		////-----------------------------------------------ByMakats
		////=================from  IturAnalyzes 
		////-----------------------------------------------by InsertIturAnalyzesSimple(pathDB, true, selectParams);
		////WriteToFile
		//public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakats(SelectParams selectParams,
		//    string pathDB, bool refill = true, Dictionary<object, object> parms = null)
		//{
		//    IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
		//            this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();

		//    long totalCount = 0;
		//    if (refill == true)
		//    {
		//        iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
		//        //iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams); test
		//        SelectParams sp = new SelectParams();
		//        iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, sp, parms);			//!!
		//    }//refill

		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        if (selectParams != null)
		//        {
		//            var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
		//                selectParams, out totalCount);
		//            var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
		//            var domainObjectsSum = from e in domainObjects
		//                                   orderby e.Makat
		//                                   group e by e.Makat into g
		//                                   select new IturAnalyzesSimple
		//                                   {
		//                                       Makat = g.Key,
		//                                       QuantityEdit = g.Sum(x => x.QuantityEdit),
		//                                       Count = g.Count()
		//                                   };
		//            //var result = IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
		//            //result.TotalCount = totalCount;
		//            //return result;
		//            return domainObjectsSum;
		//        }
		//        else
		//        {
		//            var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatDomainObject());
		//            var domainObjectsSum = from e in domainObjects
		//                                   orderby e.Makat
		//                                   group e by e.Makat into g
		//                                   select new IturAnalyzesSimple
		//                                   {
		//                                       Makat = g.Key,
		//                                       QuantityEdit = g.Sum(x => x.QuantityEdit),
		//                                       Count = g.Count()
		//                                   };
		//            //return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
		//            return domainObjectsSum;
		//        }
		//    }
		//}


		//================ IturSumQuantityEdit
		//-----------------------------------------------ByMakats
		//=================from  IturAnalyzes 
		//-----------------------------------------------by InsertIturAnalyzesSimple(pathDB, true, selectParams);
		//WriteToUnitPlan
		public IEnumerable<IturAnalyzesSimple> GetIturSumQuantityEditByIturCode(SelectParams selectParams,
			string pathDB, bool refill = true, Dictionary<object, object> parms = null)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();

			long totalCount = 0;
			bool refillCatalogDictionary = true;
			if (refill == true)
			{
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				//iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams); test
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.InsertIturAnalyzesSimple(pathDB, true, refillCatalogDictionary, sp, parms);			//!!
			}//refill

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					var entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
					var domainObjects = entities.Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode
										   group e by e.IturCode into g
										   select new IturAnalyzesSimple
										   {
											   IturCode = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//var result = IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					//result.TotalCount = totalCount;
					//return result;
					return domainObjectsSum;
				}
				else
				{
					var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode
										   group e by e.IturCode into g
										   select new IturAnalyzesSimple
										   {
											   IturCode = g.Key,
											   QuantityEdit = g.Sum(x => x.QuantityEdit),
											   Count = g.Count()
										   };
					//return IturAnalyzesSimpleCollection.FromEnumerable(domainObjectsSum);
					return domainObjectsSum;
				}
			}
		}

		//================ IPSumQuantityEdit
		//-----------------------------------------------ByMakats + Code
		//=================from  IturAnalyzes 
		//-----------------------------------------------by InsertIturAnalyzesSumSimple(pathDB, true, selectParams);
		//WriteToFile
		public IEnumerable<IturAnalyzesSimple> GetIPSumQuantityEditByMakatsPlusCode(SelectParams selectParams,
			string pathDB, bool refill = true)
		{
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository =
					this._serviceLocator.GetInstance<IIturAnalyzesSourceRepository>();
			bool refillCatalogDictionary = true;
			long totalCount = 0;
			if (refill == true)
			{
				iturAnalyzesSourceRepository.ClearIturAnalyzes(pathDB);
				//iturAnalyzesADORepository.InsertIturAnalyzes(pathDB, true, selectParams); test
				SelectParams sp = new SelectParams();
				iturAnalyzesSourceRepository.InsertIturAnalyzes(pathDB, true, refillCatalogDictionary, sp, null);			//!!
			}//refill
			List<App_Data.IturAnalyzes> entities = new List<App_Data.IturAnalyzes>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (selectParams != null)
				{
					entities = this.GetEntities(db, AsQueryable(db.IturAnalyzes), db.IturAnalyzes.AsQueryable(),
						selectParams, out totalCount);
				}
				else
				{
					//var domainObjects = AsQueryable(db.IturAnalyzes).ToList().Select(e => e.ToSimpleMakatDomainObject());
					entities = AsQueryable(db.IturAnalyzes).Select(e => e).ToList();
				}
				var domainObjectsSum = from e in entities
									   orderby e.Makat
									   group e by e.Makat into g
									   select new IturAnalyzesSimple
									   {
										   Makat = g.Key,
										   QuantityEdit = g.Sum(x => x.QuantityEdit),
										   MakatOriginal = g.Max(x => x.Code),				// хранит 	Code
										   Count = g.Count()
									   };
					return domainObjectsSum;
			}
		}


		public void FillAllDictionary(string pathDB, bool refill = true)
		{
			this._iturDictionary = this._iturRepository.GetIturDictionary(pathDB, refill);
			this._documentHeaderDictionary = this._documentHeaderRepository.GetDocumentHeaderDictionary(pathDB, refill);
			this._locationDictionary = this._locationRepository.GetLocationDictionary(pathDB, refill);
			this._statusIturDictionary = this._statusIturRepository.BitStatusIturEnumDictionary;
			this._statusIturGroupDictionary = this._statusIturGroupRepository.BitStatusIturGroupEnumDictionary;
			this._productMakatDictionary = this._makatProductRepository.GetProductBarcodeDictionary(pathDB, refill);
		}

		public void ClearDictionarys()
		{
			this._iturDictionary.Clear();
			this._documentHeaderDictionary.Clear();
			this._locationDictionary.Clear();
			//this._inventProductList.Clear();
			this._statusIturDictionary.Clear();
			this._statusIturGroupDictionary.Clear();
			this._productMakatDictionary.Clear();
		}

		public  Dictionary<string,Itur> GetIturDictionary(string pathDB, bool refill = true)
		{
			if (refill == true)
			{
				this._iturDictionary.Clear();
				this._iturDictionary = this._iturRepository.GetIturDictionary(pathDB, true);
			}
			return this._iturDictionary;
		}

		public Dictionary<string, DocumentHeader> GetDocumentHeaderDictionary(string pathDB, 
			bool refill = true)
		{
			if (refill == true)
			{
				this._documentHeaderDictionary.Clear();
				this._documentHeaderDictionary =
					this._documentHeaderRepository.GetDocumentHeaderDictionary(pathDB, refill);
			}
			return this._documentHeaderDictionary;
		}

		public Dictionary<string, Location> GetLocationDictionary(string pathDB, bool refill = true) 
		{
			if (refill == true)
			{
				this._locationDictionary.Clear();
				this._locationDictionary = this._locationRepository.GetLocationDictionary(pathDB, true);
			}
			return this._locationDictionary;
		}

		public InventProducts GetInventProductList(SelectParams selectParms, string pathDB)
		{
			this._inventProductList.Clear();
			this._inventProductList =
				this._inventProductRepository.GetInventProducts(selectParms, pathDB);
			return this._inventProductList;
		}

		public Dictionary<int, IturStatusEnum> GetStatusIturDictionary()
		{
			this._statusIturDictionary.Clear();
			this._statusIturDictionary = this._statusIturRepository.BitStatusIturEnumDictionary;
			return this._statusIturDictionary;
		}

		public void ClearProductDictionary()
		{
			this._productMakatDictionary.Clear();
			this._productSimpleDictionary.Clear();
		}

		public Dictionary<int, IturStatusGroupEnum> GetStatusIturGroupDictionary()
		{
			this._statusIturGroupDictionary.Clear();
			this._statusIturGroupDictionary = 
				this._statusIturGroupRepository.BitStatusIturGroupEnumDictionary;
			return this._statusIturGroupDictionary;
		}

		public Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB, bool refill = true)
		{
			if (refill == true || this._productMakatDictionary.Count() < 1)
			{
				this._productMakatDictionary.Clear();
				this._productMakatDictionary =
					this._makatProductRepository.GetProductBarcodeDictionary(pathDB, true);
			}
			return this._productMakatDictionary;
		}

		public Dictionary<string, ProductSimple> GetProductSimpleDictionary(string pathDB, bool refill = true, string typeMakat = "M")
		{
			if (typeMakat == "M")
			{
				if (refill == true || this._productSimpleDictionary.Count() < 1)
				{
					this._productSimpleDictionary.Clear();
					this._productSimpleDictionary =
						this._productRepository.GetProductSimpleDictionary(pathDB, true);
				}
				return this._productSimpleDictionary;
			}

			if (typeMakat == TypeMakatEnum.B.ToString())
			{
				Dictionary<string, ProductSimple> productSimpleDictionary =
					this._productRepository.GetProductSimpleDictionary(pathDB, true, typeMakat);
				return productSimpleDictionary;
			}

			else return this._productSimpleDictionary;
		}

		//public Dictionary<string, ProductSimple> GetProductSimpleUpdateOnlyDictionary(string pathDB, bool refill = true)
		//{
		//    if (refill == true)
		//    {
		//        this._productSimpleDictionary.Clear();
		//        this._productSimpleDictionary =
		//            this._productRepository.GetProductSimpleUpdateOnlyDictionary(pathDB, refill);
		//    }
		//    return this._productSimpleDictionary;
		//}

		public void SetResulteValue(AnalezeValueTypeEnum resulteCode, string resulteValue, string resulteDescription, string pathDB)
		{
			IturAnalyzes iturAnalyzes = GetIturAnalyzesEmpty();
			//IturAnalyzes iturAnalyzes = new IturAnalyzes();
			iturAnalyzes.ResultCode = resulteCode.ToString();
			iturAnalyzes.ResulteValue = resulteValue;
			iturAnalyzes.ResulteDescription = resulteDescription;
			iturAnalyzes.IsResulte = true;

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = iturAnalyzes.ToEntity();
				db.IturAnalyzes.AddObject(entity);
				db.SaveChanges();
			}
			
		}

		public IturAnalyzes SetIturAnalyzesResulteValue(AnalezeValueTypeEnum resulteCode, string resulteValue, string resulteDescription, string pathDB, string inventorCode="none", string inventorName = "none")
		{
			IturAnalyzes iturAnalyzes = GetIturAnalyzesEmpty();
			//IturAnalyzes iturAnalyzes = new IturAnalyzes();
			iturAnalyzes.ResultCode = resulteCode.ToString();
			iturAnalyzes.ResulteValue = resulteValue;
			iturAnalyzes.ResulteDescription = resulteDescription;
			iturAnalyzes.IsResulte = true;
			iturAnalyzes.InventorCode = inventorCode;
			iturAnalyzes.InventorName = inventorName;
			return iturAnalyzes;
		}


		public string GetResulteValue(AnalezeValueTypeEnum resulteCode, string pathDB)
		{	
			string ret = "";
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string _resulteCode = resulteCode.ToString();
				var entity = AsQueryable(db.IturAnalyzes).FirstOrDefault(e => e.ResultCode.CompareTo(_resulteCode) == 0);
				if (entity != null)
				{
					ret = String.IsNullOrWhiteSpace(entity.ResulteValue) == false ? entity.ResulteValue : string.Empty;
				}
				//IturAnalyzes iturAnalyzes = entity.ToDomainObject();
			}
			return ret;
		}


		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.IturAnalyzes.ToList().ForEach(e => db.IturAnalyzes.DeleteObject(e));
				db.SaveChanges();

				List<long> ids = db.IturAnalyzes.Select(e => e.ID).Distinct().ToList();   //Какая-то ошибка где не понятно ? пока заплатка

				//foreach (long id in ids)
				//{
				//	var entity = db.IturAnalyzes.Single(x => x.ID == id);
				//	if (entity != null)
				//	{
				//		db.IturAnalyzes.DeleteObject(entity);
						
				//	}
				//	db.SaveChanges();
				//}

			
				//List<long> ids1 = db.IturAnalyzes.Select(e => e.ID).Distinct().ToList(); 
		
		
			}

			IturAnalyzesCollection ret = GetIturAnalyzesCollection(pathDB);
			var test = ret.ToList();
			
			//"SELECT ID  FROM     IturAnalyzes"
		}


		public IturAnalyzesCollection GetIturAnalyzesCollection(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = AsQueryable(db.IturAnalyzes).ToList();
				var result = IturAnalyzesCollection.FromEntityList(entities);
				result.TotalCount = entities.LongCount();
				result.SumQuantityEdit = entities.Sum(e => e.QuantityEdit);
				return result;
			}
		}



		#endregion

		#region Private methods
		private IturAnalyzes GetIturAnalyzesEmpty()
		{
			IturAnalyzes iturAnalyzes = new IturAnalyzes();
			//iturAnalyzes.IPValueStr1 = "";
			//iturAnalyzes.IPValueStr2 = "";
			//iturAnalyzes.IPValueStr3 = "";
			//iturAnalyzes.IPValueStr4 = "";
			//iturAnalyzes.IPValueStr5 = "";
			//iturAnalyzes.IPValueStr6 = "";
			//iturAnalyzes.IPValueStr7 = "";
			//iturAnalyzes.IPValueStr8 = "";
			//iturAnalyzes.IPValueStr9 = "";
			//iturAnalyzes.IPValueStr10 = "";
			//iturAnalyzes.IPValueFloat1 = 0;
			//iturAnalyzes.IPValueFloat2 = 0;
			//iturAnalyzes.IPValueFloat3 = 0;
			//iturAnalyzes.IPValueFloat4 = 0;
			//iturAnalyzes.IPValueFloat5 = 0;
			//iturAnalyzes.IPValueInt1 = 0;
			//iturAnalyzes.IPValueInt2 = 0;
			//iturAnalyzes.IPValueInt3 = 0;
			//iturAnalyzes.IPValueInt4 = 0;
			//iturAnalyzes.IPValueInt5 = 0;
			//iturAnalyzes.IPValueBit1 = false;
			//iturAnalyzes.IPValueBit2 = false;
			//iturAnalyzes.IPValueBit3 = false;
			//iturAnalyzes.IPValueBit4 = false;
			//iturAnalyzes.IPValueBit5 = false;
			//iturAnalyzes.QuantityInPackEdit = 0;
			//iturAnalyzes.CountInParentPack = 1;
			//iturAnalyzes.BalanceQuantityPartialERP = 0;

			return iturAnalyzes;
		}

		private App_Data.IturAnalyzes GetEntityByCode(App_Data.Count4UDB db, string iturCode)
        {
			var entity = AsQueryable(db.IturAnalyzes).FirstOrDefault(e => e.IturCode.CompareTo(iturCode) == 0);
            return entity;
        }

		

        #endregion
	}

	public enum LevelInAnalyzesEnum		   //? уровень с которого запускаем, ? только для IA 	 Level
	{
		Itur = 0,
		Doc = 1,
		Location = 2,
		Product = 3,
		InventProduct = 4,
		PDA = 5,
		Iturs = 6,
		Customer = 7,
		Branch = 8,
		Inventor = 9,
		AuditConfig = 10,
		None = 20
	}

	public enum IturAnalyzeTypeEnum
	{
		Simple = 0,
		SimpleSum = 1,
		Full = 20,
		FullFamilySortLocationIturMakat = 21
	}

	public enum AnalezeValueTypeEnum
	{
		CountItems_InsertManually = 1,
		CountItems_InsertFromBarcode= 2,
		CountItems_Total = 3,
		CountPDAMakats_Total = 4,
		CountERPMakats_Total = 5,
		SumDifferenceValueLess0_Total = 6,
		SumDifferenceValueGrate0_Total = 7
	}
	
}
