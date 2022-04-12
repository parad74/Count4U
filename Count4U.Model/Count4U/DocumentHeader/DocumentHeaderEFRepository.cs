using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections;
using System.Collections.Generic;
using NLog;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
    public class DocumentHeaderEFRepository : BaseEFRepository, IDocumentHeaderRepository
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		private IServiceLocator _serviceLocator;
		public DocumentHeaderEFRepository(IConnectionDB connection, IServiceLocator serviceLocator)
			: base(connection)
        {
			this._serviceLocator = serviceLocator;
			this._documentHeaderDictionary = new Dictionary<string, DocumentHeader>();
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IDocumentHeaderRepository Members

		public DocumentHeaders GetDocumentHeaders(string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.DocumentHeaders.ToList().Select(e => e.ToDomainObject());
                return DocumentHeaders.FromEnumerable(domainObjects);
            }
        }

		public DocumentHeaders GetDocumentHeaders(SelectParams selectParams, string pathDB)
        {
            if (selectParams == null)
                return GetDocumentHeaders(pathDB);

            long totalCount = 0;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.DocumentHeaders),
				 db.DocumentHeaders.AsQueryable(), selectParams, out totalCount);

				var domainObjects = entities.Select(e => e.ToDomainObject());

				var result = DocumentHeaders.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
        }



		public List<string> GetDocumentCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.DocumentHeaders.Select(e => e.DocumentCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetDocumentCodeList", exp);
				}
			}
			return ret;
		}

		public void SetNullToApproveDocuments(List<string> documentCodes, string pathDB)
		{
			SelectParams selectParams = new SelectParams();
			if (documentCodes.Count == 0) return;
			selectParams.FilterStringListParams.Add("DocumentCode", new FilterStringListParam()
			{
				Values = documentCodes
			});

			DocumentHeaders documentHeaders = GetDocumentHeaders(selectParams, pathDB);
			//foreach (var documentHeader in documentHeaders)
			//{
			//	documentHeader.Approve = null;


			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var documentHeader in documentHeaders)
				{
					var entity = this.GetEntityByDocumentCode(db, documentHeader.DocumentCode);
					if (entity == null) continue;
					entity.Approve = null;
					//entity.ApplyChanges(documentHeader);
				}
				db.SaveChanges();
			}
		}

		//OLD
		//public int GetCountDocumentWithError(List<string> documentCodes, string pathDB)
		//{
		//	SelectParams selectParams = new SelectParams();
		//	if (documentCodes.Count == 0) return 0;
		//	if (documentCodes.Count > 30) return 0;
		//	selectParams.FilterStringListParams.Add("DocumentCode", new FilterStringListParam()
		//	{
		//		Values = documentCodes
		//	});

		//	DocumentHeaders documentHeaders = GetDocumentHeaders(selectParams, pathDB);
		//	int count = 0;
		//	foreach (var documentHeader in documentHeaders)
		//	{
		//		if (documentHeader.StatusInventProductBit != 0
		//			|| documentHeader.StatusDocHeaderBit != 0)
		//		{
		//			count++;
		//		}
		//	}
		//	return count;
		//}
		public long GetCountDocumentWithError(List<string> sessionCodeList, string pathDB)
		{
			SelectParams selectParams = new SelectParams();
			if (sessionCodeList.Count == 0) return 0;

			selectParams.FilterStringListParams.Add("SessionCode", new FilterStringListParam()
			{
				Values = sessionCodeList
			});

			DocumentHeaders documentHeaders = GetDocumentHeaders(selectParams, pathDB);

			long count = documentHeaders.Where(x => x.StatusInventProductBit != 0 && x.StatusDocHeaderBit != 0).LongCount();

			//int count = 0;
			//foreach (var documentHeader in documentHeaders)
			//{
			//	if (documentHeader.StatusInventProductBit != 0
			//		|| documentHeader.StatusDocHeaderBit != 0)
			//	{
			//		count++;
			//	}
			//}
			return count;
		}

		//OLD
		//public int GetCountDocumentWithoutError(List<string> documentCodes, string pathDB)
		//{
		//	SelectParams selectParams = new SelectParams();
		//	if (documentCodes.Count == 0) return 0;
		//	if (documentCodes.Count > 30) return 0;
		//	selectParams.FilterStringListParams.Add("DocumentCode", new FilterStringListParam()
		//	{
		//		Values = documentCodes
		//	});

		//	DocumentHeaders documentHeaders = GetDocumentHeaders(selectParams, pathDB);
		//	int count = 0;
		//	foreach (var documentHeader in documentHeaders)
		//	{
		//		if (documentHeader.StatusInventProductBit == 0
		//			&& documentHeader.StatusDocHeaderBit == 0)
		//		{
		//			count++;
		//		}
		//	}
		//	return count;
		//}

		public long GetCountDocumentWithoutError(List<string> sessionCodeList, string pathDB)
		{
			SelectParams selectParams = new SelectParams();
			if (sessionCodeList.Count == 0) return 0;

			selectParams.FilterStringListParams.Add("SessionCode", new FilterStringListParam()
			{
				Values = sessionCodeList
			});

			DocumentHeaders documentHeaders = GetDocumentHeaders(selectParams, pathDB);
			long count = documentHeaders.Where(x => x.StatusInventProductBit == 0 && x.StatusDocHeaderBit == 0).LongCount();
			//int count = 0;
			//foreach (var documentHeader in documentHeaders)
			//{
			//	if (documentHeader.StatusInventProductBit == 0
			//		&& documentHeader.StatusDocHeaderBit == 0)
			//	{
			//		count++;
			//	}
			//}
			return count;
		}

		public DocumentHeaders GetDocumentHeadersByItur(Itur itur, string pathDB)
        {
            return GetDocumentHeadersByIturCode(itur.IturCode, pathDB);
        }

		public DocumentHeaders GetDocumentHeadersByIturCode(string iturCode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.DocumentHeaders.Where(e => e.IturCode == iturCode)
												 .ToList().Select(e => e.ToDomainObject());
				return DocumentHeaders.FromEnumerable(domainObjects);

            }
        }

		public void DeleteAllDocumentsWithoutAnyInventProduct(string pathDB)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var documentHeaders = db.DocumentHeaders.Distinct().ToList();
				var documentHeadersToDelete = new List<App_Data.DocumentHeader>();
				List<string> IPdocumentHeaderCodes = db.InventProducts.Select(x => x.DocumentHeaderCode).Distinct().ToList();
				if (documentHeaders.LongCount() == IPdocumentHeaderCodes.LongCount()) return;

				if (documentHeaders != null)
				{
					foreach (var dh in documentHeaders)
					{
						if (dh != null)
						{
							if (IPdocumentHeaderCodes.Contains(dh.DocumentCode) == false)
							{
								documentHeadersToDelete.Add(dh);
								//db.DocumentHeaders.DeleteObject(dh);
							}
							//bool isAnyInventProducts = db.InventProducts.Any(z => z.DocumentCode == dh.DocumentCode);

							//if(isAnyInventProducts == false) 
							//{
							//	db.DocumentHeaders.DeleteObject(dh);
							//}
						}
					}
				}


				//var entities = db.Products.Where(e => selectedMakats.Contains(e.Makat)).ToList();
				if (documentHeadersToDelete == null) return;
				if (documentHeadersToDelete.Count > 0)
				{
					documentHeadersToDelete.ForEach(e => db.DocumentHeaders.DeleteObject(e));
					db.SaveChanges();
				}
			}
		}

		public DocumentHeaders GetDocumentHeadersBySession(Session session, string pathDB)
        {
			return this.GetDocumentHeadersBySessionCode(session.SessionCode, pathDB);
        }

		public DocumentHeaders GetDocumentHeadersBySessionCode(string sessionCode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.DocumentHeaders.Where(e => e.SessionCode == sessionCode)
                                                     .ToList().Select(e => e.ToDomainObject());
                return DocumentHeaders.FromEnumerable(domainObjects);
            }
        }

		public DocumentHeaders GetDocumentHeadersByStatusCode(string statusCode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.DocumentHeaders.Where(e => e.StatusDocHeaderCode.CompareTo(statusCode) == 0)
                                                     .ToList().Select(e => e.ToDomainObject());
                return DocumentHeaders.FromEnumerable(domainObjects);
            }
        }

		public Dictionary<string, DocumentHeader> GetDocumentHeaderDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				this.FillDocumentHeaderDictionary(pathDB);
			}
			return this._documentHeaderDictionary;
		}

		public void FillDocumentHeaderDictionary(string pathDB)
		{
			this.ClearDocumentHeaderDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					DocumentHeaders documentHeaders = this.GetDocumentHeaders(pathDB);

					this._documentHeaderDictionary = documentHeaders.Select(e => e).Distinct().ToDictionary(k => k.DocumentCode);
				}
				catch (Exception exp)
				{
					_logger.ErrorException("FillDocumentHeaderDictionary", exp);
				}
			}
		}

		public void ClearDocumentHeaderDictionary()
		{
			this._documentHeaderDictionary.Clear();
			GC.Collect();
		}

		//работает
		public void RefillDocumentStatisticBySession(List<string> sessionCodeList, string pathDB)
		{
			//string inputTypeCodeB = InputTypeCodeEnum.B.ToString();
			List<string> documentCodeSessionList = new List<string>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//Dictionary<string, App_Data.DocumentHeader> documentHeaderEntityDictionary =
				//	db.DocumentHeaders.Select(e => e).Distinct().ToDictionary(k => k.DocumentCode);

				foreach (var sessionCode in sessionCodeList)
				{
					//Parallel.ForEach(docCodes, docCode =>
					//{
					if (string.IsNullOrEmpty(sessionCode) != true)
					{
						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => x.SessionCode == sessionCode).ToList().Select(e => e);
						Dictionary<string, App_Data.DocumentHeader> documentHeaderForSessionDictionary = documentHeaders.Select(e => e).ToDictionary(k => k.DocumentCode);
						documentCodeSessionList = documentHeaders.Select(s => s.DocumentCode).ToList();

						var inventProducts = AsQueryable(db.InventProducts).ToList().Select(x => x);  //&& x.InputTypeCode == inputTypeCodeB
						var inventProductsSumByDocumentCode = from e in inventProducts
															  orderby e.DocumentCode
															  group e by e.DocumentCode into g
															  select new DocumentHeader
															  {
																  DocumentCode = g.Key,
																  QuantityEdit = g.Sum(x => x.QuantityEdit),
																  FromTime = g.Min(x => (DateTime)x.CreateDate),
																  ToTime = g.Max(x => (DateTime)x.CreateDate),
																  Total = g.LongCount()
															  };

						Dictionary<string, DocumentHeader> documentHeaderSumIPDictionary = inventProductsSumByDocumentCode.Select(e => e).ToDictionary(k => k.DocumentCode );
						if (documentHeaderSumIPDictionary == null) return;
						inventProducts = null;

						foreach (var docCode in documentCodeSessionList)
						{
							//App_Data.DocumentHeader entity = new App_Data.DocumentHeader();
							//bool retCan = documentHeaderEntityDictionary.TryGetValue(docCode, out entity);
							App_Data.DocumentHeader entity = null;
							DocumentHeader documentHeaderSum = null;
							try
							{
								entity = documentHeaderForSessionDictionary[docCode];
								documentHeaderSum = documentHeaderSumIPDictionary[docCode];
							}
							catch { }

							if (entity != null)
							{
								var documentHeader = entity.ToDomainObject();
								DateTime fromTime = DateTime.Now;
								DateTime toTime = fromTime;
								long ticksTimeSpan = 0;
								string periodFromTo = "00:00:00";
								int periodFromToDays = 0;
								int periodFromToH = 0;
								int periodFromToMin = 0;
								int periodFromToSec = 0;
								long total = 0;
								double quantityEdit = 0;

								if (documentHeaderSum != null)
								{
									try
									{
										fromTime = documentHeaderSum.FromTime;
										toTime = documentHeaderSum.ToTime;
									}
									catch { }
									TimeSpan fromTo = (TimeSpan)(toTime - fromTime);

									try
									{
										ticksTimeSpan = fromTo.Ticks;
										periodFromToDays = fromTo.Days;
										periodFromToH = fromTo.Hours;
										periodFromToMin = fromTo.Minutes;
										periodFromToSec = fromTo.Seconds;
										periodFromTo = (periodFromToDays * 24 + periodFromToH).ToString().PadLeft(2, '0') + ":" +
																				periodFromToMin.ToString().PadLeft(2, '0') + ":" +
																				periodFromToSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss

										//periodFromTo = fromTo.ToString(@"dd\:hh\:mm");
										//if (periodFromTo == "00:00:00") periodFromTo = "00:00:01";
										total = documentHeaderSum.Total;
										quantityEdit = documentHeaderSum.QuantityEdit;
									}
									catch { }
								}
								documentHeader.QuantityEdit = quantityEdit;
								documentHeader.Total = total;
								documentHeader.FromTime = fromTime;
								documentHeader.ToTime = toTime;
								documentHeader.TicksTimeSpan = ticksTimeSpan;
								documentHeader.PeriodFromTo = periodFromTo;

								entity.ApplyChanges(documentHeader);
							}
						}

						documentHeaders = null;
					}
					//db.SaveChanges();
					//}); //Parallel.ForEach docCodes
				}//foreach
				//documentHeaderEntityDictionary.Clear();
				//documentHeaderEntityDictionary = null;
				db.SaveChanges();
				
			}		//db
		}

		//работает
		public void RefillIturStatistic(List<string> sessionCodeList, List<string> iturCodes, List<string> docCodes, string pathDB)
		{
			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				Dictionary<string, App_Data.Itur> iturEntityDictionary = db.Iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);
				var inventProducts = AsQueryable(db.InventProducts).ToList().Select(x => x);

				var inventProductsSumByIturCode = from e in inventProducts
													  orderby e.IturCode
													  group e by e.IturCode into g
													  select new Itur
													  {
														  IturCode = g.Key,
														  SumQuantityEdit = g.Sum(x => x.QuantityEdit),
														  TotalItem = g.LongCount()
													  };

				Dictionary<string, Itur> inventProductsSumIPDictionary = inventProductsSumByIturCode.Select(e => e).ToDictionary(k => k.IturCode);

				inventProducts = null;
				if (inventProductsSumIPDictionary == null) return;
				if (iturCodes == null) return;

				foreach (var code in iturCodes)
				{
					if (string.IsNullOrEmpty(code) != true)
					{
						App_Data.Itur iturEntity = new App_Data.Itur();
						bool retCan = iturEntityDictionary.TryGetValue(code, out iturEntity);
						if (retCan == true)
						{
							//var iturDomain = iturEntity.ToDomainObject();
							//double quantityEdit = 0;
							//long total = 0;

							//var inventProductEntities = db.InventProducts.Where(e => e.IturCode == code).ToList().Select(e => e.ToDomainObject());  //&& e.InputTypeCode == inputTypeCodeB
							//if (inventProductEntities != null && inventProductEntities.Count() > 0)
							//{

							//	try { quantityEdit = inventProductEntities.Sum(x => x.QuantityEdit); }
							//	catch { }

							//	List<string> ipCode = inventProductEntities.Select(x => x.Makat).Distinct().ToList();
							//	total = ipCode.LongCount();
							//	}


							//iturDomain.SumQuantityEdit = quantityEdit;
							//iturDomain.TotalItem = total; 
							//iturEntity.ApplyChanges(iturDomain);

							//New
							Itur itur = inventProductsSumIPDictionary[code];
							iturEntity.SumQuantityEdit = itur.SumQuantityEdit;
							iturEntity.TotalItem = itur.TotalItem;
						}
					}
				}//foreach iturcode

				db.SaveChanges();
				iturEntityDictionary = null;
			}
		
		}		//db
	


		public void Delete(DocumentHeader documentHeader, string pathDB)
        {
            this.Delete(documentHeader.DocumentCode, pathDB);
        }

		public void Delete(string documentCode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByDocumentCode(db, documentCode);
				if (entity != null)
				{
					db.DocumentHeaders.DeleteObject(entity);
				}
				var entities = db.InventProducts.Where(e => e.DocumentCode == documentCode).ToList();
				if (entities != null)
				{
					entities.ForEach(e => db.InventProducts.DeleteObject(e));
				}
				db.SaveChanges();
            }
        }

		public void DeleteAllByItur(Itur itur, string pathDB)
        {
			this.DeleteAllByIturCode(itur.IturCode, pathDB);
        }

		public void DeleteAllByIturCode(string iturCode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var documentHeaders = db.DocumentHeaders.Where(e => e.IturCode == iturCode).ToList();
				if (documentHeaders != null)
				{
					foreach (var dh in documentHeaders)
					{
						var inventProductEntities = db.InventProducts.Where(e => e.DocumentCode == dh.DocumentCode).ToList();
						if (inventProductEntities != null)
						{
							inventProductEntities.ForEach(e => db.InventProducts.DeleteObject(e));
						}
					}

					documentHeaders.ForEach(e => db.DocumentHeaders.DeleteObject(e));
				}
                db.SaveChanges();
            }
        }

		public void Insert(Itur itur, DocumentHeader documentHeader, string pathDB)
        {
			if (itur == null) return;
			if (documentHeader == null) return;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				documentHeader.CreateDate = DateTime.Now;
                var entity = documentHeader.ToEntity();
                entity.IturCode = itur.IturCode;
                db.DocumentHeaders.AddObject(entity);
                db.SaveChanges();
            }
        }

		public long Insert(DocumentHeader documentHeader, string pathDB)
		{
			if (documentHeader == null) return -1;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				documentHeader.CreateDate = DateTime.Now;
				var entity = documentHeader.ToEntity();
				db.DocumentHeaders.AddObject(entity);
				db.SaveChanges();

				 if (string.IsNullOrEmpty(documentHeader.DocumentCode) == true) return -1;
				 var doc = this.GetEntityByDocumentCode(db, documentHeader.DocumentCode);	  //newInventProduct.DocNum
				 if (doc == null) return -1;
				 return entity.ID ;
			}
		}

		public void Insert(DocumentHeaders documentHeaders, string pathDB)
		{
			if (documentHeaders == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var documentHeader in documentHeaders)
				{
					documentHeader.CreateDate = DateTime.Now;
					var entity = documentHeader.ToEntity();
					db.DocumentHeaders.AddObject(entity);
				}
				db.SaveChanges();
			}
		}


		public DocumentHeader GetDocumentHeaderByCode(string documentCode, string pathDB)
		{
			if (string.IsNullOrEmpty(documentCode) == true) return null;

			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByDocumentCode(db, documentCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public void Update(DocumentHeader documentHeader, string pathDB)
        {
			if (documentHeader == null) return;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByDocumentCode(db, documentHeader.DocumentCode);
				if (entity == null) return;
                entity.ApplyChanges(documentHeader);
                db.SaveChanges();
            }
        }


		public void UpdateWorkerName(string deviceName, string oldWorkerName, string newWorkerName, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entitys = db.DocumentHeaders.Select(e => e).Where(x => x.Name == deviceName && x.WorkerGUID == oldWorkerName).ToList();
				if (entitys == null) return;
				foreach (var entity in entitys)
				{
					entity.WorkerGUID = newWorkerName;
					entity.StatusDocHeaderCode = deviceName + "|" + newWorkerName;
				}
				db.SaveChanges();
			}
		}

		public void Insert(Itur itur, DocumentHeaders documentHeaders, string pathDB)
        {
			documentHeaders.ToList().ForEach(e => Insert(itur, e, pathDB));
        }

		#endregion



		//not use
		public BitArray GetResultInventProductStatusBitOrByDocumentCode(string documentCode, string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					var entities = db.InventProducts.Where(e => e.DocumentCode == documentCode).
						ToList().Select(e => e.StatusInventProductBit).ToArray();
					if (entities == null) return null;
					BitArray bitArray = BitStatus.GetResultBitArrayOr(entities);
					return bitArray;
				}
			}
			else
			{
				using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					var entity = this.GetEntityByDocumentCode(db, documentCode);
					if (entity == null) return BitStatus.GetBitArray(0);
					return BitStatus.GetBitArray(entity.StatusDocHeaderBit);
				}
			}
		}


		//not use
		public int GetResultInventProductStatusIntOrByDocumentCode(string documentCode, string pathDB, 
			bool refill = false)
		{
			BitArray bitArray = this.GetResultInventProductStatusBitOrByDocumentCode(documentCode, pathDB, refill);
			int[] array = new int[1];
			bitArray.CopyTo(array, 0);
			return array[0];
		}

		//not use
		public int RefillDocHeaderStatusBitByDocumenCode(string documentCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByDocumentCode(db, documentCode);
				if (entity == null) return  0;
				int status = this.GetResultInventProductStatusIntOrByDocumentCode(entity.DocumentCode, pathDB, true);
				//TODO:	 test
				entity.StatusDocHeaderBit = entity.StatusDocHeaderBit.Or(status);
				db.SaveChanges();
				return status;
			}
		}

		//not use
		public void RefillStatusBit(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = db.DocumentHeaders.Select(e => e);
				if (entities == null) return;
				foreach (var entity in entities)
				{
					int status = this.GetResultInventProductStatusIntOrByDocumentCode(entity.DocumentCode, pathDB, true);
					//TODO: or bit
					entity.StatusDocHeaderBit = entity.StatusDocHeaderBit.Or(status);
				}
				db.SaveChanges();
			}
		}

		//not use
		public void ClearStatusBit(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = db.DocumentHeaders.Select(e => e);
				if (entities == null) return;
				foreach (var entity in entities)
				{
					entity.StatusDocHeaderBit = 0;
				}
				db.SaveChanges();
			}
		}

	

	
		//public List<string> GetDocumentCodeList(string pathDB)
		//{
		//    List<string> ret = new List<string>();
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        try
		//        {
		//            var entitys = db.InventProducts.Select(e => e.DocumentCode).Distinct().ToList();
		//            return entitys;
		//        }
		//        catch (Exception exp)
		//        {
		//            _logger.ErrorException("GetDocumentCodeList", exp);
		//        }
		//    }
		//    return ret;
		//}

		public List<string> GetDocumentHeaderCodeList(IEnumerable<App_Data.DocumentHeader>  documentHeaders)
		{
			var entitys = documentHeaders.Select(e => e.DocumentCode).Distinct().ToList();
			return entitys;
		}

		//public List<string> GetIturCodeList(ObjectSet<App_Data.DocumentHeader> documentHeaders)
		//{
		//    var docs = documentHeaders.AsEnumerable();
		//    foreach (var doc in docs) { }
		//    var entitys = documentHeaders.Select(e => e.DocumentCode).Distinct().ToList();
		//    return entitys;
		//}

		public List<string> GetIturCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.DocumentHeaders.Select(e => e.IturCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturCodeList", exp);
				}
			}
			return ret;
		}

		//public List<string> GetIturCodeList(SelectParams selectParams, string pathDB)
		//{
		//	long totalCount = 0;

		//	using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//	{
		//		if (selectParams != null)
		//		{
		//			var entities = this.GetEntities(db, AsQueryable(db.InventProducts), db.InventProducts.AsQueryable(),
		//				selectParams, out totalCount);
		//			List<string> codeList = entities.Select(e => e.IturCode).Distinct().ToList();
		//			return codeList;
		//		}
		//		else
		//		{
		//			var entities = AsQueryable(db.InventProducts).ToList().Select(e => e.ToDomainObject());
		//			List<string> codeList = entities.Select(e => e.IturCode).Distinct().ToList();
		//			return codeList;
		//		}
		//	}
		//}


		public Dictionary<string, DocumentHeader> GetIturDocumentCodeDictionary(string pathDB)
		{
			Dictionary<string, DocumentHeader> dictionary = new Dictionary<string, DocumentHeader>();
			DocumentHeaders documentHeaders = this.GetDocumentHeaders(pathDB);

			foreach (DocumentHeader doc in documentHeaders)
			{
				dictionary[doc.IturCode] = doc;
			}
				return dictionary;
		}
																		
		public Dictionary<string, DocumentHeader> GetIturDictionaryMaxDateTime(string pathDB)
		{
			List<string> ret = new List<string>();
			Dictionary<string, DocumentHeader> dictionary = new Dictionary<string, DocumentHeader>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					//var entitys = db.DocumentHeaders.Select(e => e.IturCode).Distinct().ToList();
					//return entitys;

					var domainObjects = AsQueryable(db.DocumentHeaders).ToList().Select(e => e.ToDomainObject());
					var domainObjectsSum = from e in domainObjects
										   orderby e.IturCode
										   group e by e.IturCode into g
										   select new DocumentHeader
										   {
											   IturCode = g.Key,
											   CreateDate = g.Max(x => x.CreateDate)
										   };

					dictionary = domainObjectsSum.Select(e => e).ToDictionary(k => k.IturCode);
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturDictionaryMaxDateTime", exp);
				}
				return dictionary;
			}
		}

		private App_Data.DocumentHeader GetEntityByDocumentCode(App_Data.Count4UDB db, string documentCode)
        {
			var entity = db.DocumentHeaders.FirstOrDefault(e => e.DocumentCode.CompareTo(documentCode) == 0);
            return entity;
        }

 
    }
}
