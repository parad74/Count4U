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
using NLog;
using System.Threading.Tasks;
using System.Globalization;
using Count4U.Model.Common;
using System.Text;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class IturEFRepository : BaseEFRepository, IIturRepository
	{
		private Dictionary<string, Itur> _iturDictionary;
		private readonly IStatusIturGroupRepository _statusIturGroupRepository;
		private readonly IStatusIturRepository _statusIturRepository;
		private readonly IInventProductRepository _inventProductRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IDocumentHeaderRepository _documentHeaderRepository;
	//	private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
	//	private readonly IServiceLocator _serviceLocator;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private static bool _refillApproveStatus;


		public IturEFRepository(IConnectionDB connection,
			IStatusIturGroupRepository statusIturGroupRepository,
			IStatusIturRepository statusIturRepository,
			IInventProductRepository inventProductRepository,
			IDocumentHeaderRepository documentHeaderRepository,
			ILocationRepository locationRepositor)
			: base(connection)
		{
			this._iturDictionary = new Dictionary<string, Itur>();
			this._statusIturGroupRepository = statusIturGroupRepository;
			this._statusIturRepository = statusIturRepository;
			this._locationRepository = locationRepositor;
			//this._iturAnalyzesRepository = iturAnalyzesRepository;
			this._inventProductRepository = inventProductRepository;
			this._documentHeaderRepository = documentHeaderRepository;
			_refillApproveStatus = false;
		}

		#region BaseEFRepository Members

		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}

		#endregion

		#region IIturRepository Members

		public bool RefillApproveStatus
		{
			get { return IturEFRepository._refillApproveStatus; }
			set { IturEFRepository._refillApproveStatus = value; }
		}

		public Iturs GetIturs(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (String.IsNullOrWhiteSpace(pathDB) == true) return new Iturs();
				var domainObjects = AsQueryable(db.Iturs).ToList().Select(e => e.ToDomainObject());
				return Iturs.FromEnumerable(domainObjects);
			}
		}

		public Iturs GetItursAndLocationName(string pathDB)
		{
			Dictionary<string, Location> locationDictionary = this._locationRepository.GetLocationDictionary(pathDB, true);
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (String.IsNullOrWhiteSpace(pathDB) == true) return new Iturs();
				var domainObjects = AsQueryable(db.Iturs).ToList().Select(e => e.ToDomainObject(locationDictionary));

				return Iturs.FromEnumerable(domainObjects);
			}
		}

		public Iturs GetItursAndLocationName(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
			{
				Iturs result = GetItursAndLocationName(pathDB);
				//Iturs result1 = Iturs.FillBarcode(result);
				return result;
			}

			long totalCount = 0;
			Dictionary<string, Location> locationDictionary = this._locationRepository.GetLocationDictionary(pathDB, true);

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Iturs), db.Iturs.AsQueryable(),
					selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject(locationDictionary));

				Iturs result = Iturs.FromEnumerable(domainObjects);
				//	Iturs result1 = Iturs.FillBarcode(result);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public Locations GetLocationList(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> locationCodeList = db.Iturs.Select(e => e.LocationCode).Distinct().ToList();
				Locations ret = new Locations();
				foreach (string locationCode in locationCodeList)
				{
					Location newLocation = new Location();
					if (string.IsNullOrWhiteSpace(locationCode) == false)
					{
						newLocation.Code = locationCode;
						newLocation.Name = locationCode;
					}
					ret.Add(newLocation);
				}
				//var location = Locations.FromEnumerable(db.Iturs.Select(e =>
				//new Location { Code = e.LocationCode != null ? e.LocationCode : DomainUnknownCode.UnknownLocation }).
				//	Distinct());
				return ret;
			}
		}

		public List<string> GetTagList(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> tags = db.Iturs.Select(e => e.Tag).Distinct().ToList();
				return tags;
			}
		}

		public int GetItursTotal(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				return db.Iturs.Count();
			}
		}

		public Iturs GetIturs(SelectParams selectParams, string pathDB)
		{
			_logger.Trace("GetIturs");

			if (selectParams == null)
				return GetIturs(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				// Получение сущностей и общего количества из БД.
				// Getting entities and total count from database.
				var entities = GetEntities(db, AsQueryable(db.Iturs), db.Iturs.AsQueryable(),
					selectParams, out totalCount);

				// Преобразование сущностей в объекты предметной области.
				// Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToDomainObject());

				// Возврат результата.
				// Returning result.
				var result = Iturs.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public double GetIturTotalDone(Iturs iturs, string pathDB)
		{
			int disableAndNoOneDocBit = (int)IturStatusGroupEnum.DisableAndNoOneDoc;
			int emptyBit = (int)IturStatusGroupEnum.Empty;

			var disabledAndNoOneDoc = iturs.Where(x => x.StatusIturGroupBit == disableAndNoOneDocBit).Select(x => x);
			var empty = iturs.Where(r => r.StatusIturGroupBit == emptyBit).Select(x => x);

			int disabledAndNoOneDocCount = disabledAndNoOneDoc == null ? 0 : disabledAndNoOneDoc.Count();
			int emptyCount = empty == null ? 0 : empty.Count();
			int totalIturs = iturs == null ? 0 : iturs.Count();

			double totalDone = 0;
			double A = totalIturs - disabledAndNoOneDocCount;
			double B = emptyCount;
			if (A > 0)
			{
				totalDone = ((A - B) / A) * 100;
			}

			return totalDone;
		}

		public double GetIturTotalDone(SelectParams selectParams, string pathDB)
		{
			Iturs iturs = GetIturs(selectParams, pathDB);
			return GetIturTotalDone(iturs, pathDB);
		}

		public Iturs GetItursByLocation(Location location, string pathDB)
		{
			return GetItursByLocationCode(location.Code, pathDB);
		}

		public Iturs GetItursByLocationCode(string locationCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = AsQueryable(db.Iturs).Where(
					e => e.LocationCode.CompareTo(locationCode) == 0)
					.ToList().Select(e => e.ToDomainObject());
				return Iturs.FromEnumerable(domainObjects);
			}
		}

		public Iturs GetItursByUnitPlanCode(string unitPlanCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = AsQueryable(db.Iturs).Where(
					e => e.UnitPlanCode.CompareTo(unitPlanCode) == 0)
					.ToList().Select(e => e.ToDomainObject());
				return Iturs.FromEnumerable(domainObjects);
			}
		}

		public List<string> GetIturCodesUnitPlanCode(string unitPlanCode, string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Iturs.Where(e => e.UnitPlanCode.CompareTo(unitPlanCode) == 0).
						Select(e => e.IturCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturCodesUnitPlanCode", exp);
				}
			}
			return ret;
		}


		public List<string> GetIturCodesForLocationCode(string locationCode, string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Iturs.Where(e => e.LocationCode.CompareTo(locationCode) == 0).
						Select(e => e.IturCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturCodesForLocationCode", exp);
				}
			}
			return ret;
		}



		public List<string> GetIturCodesForLocationCodes(string[] locationCodes, string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					foreach (var locationCode in locationCodes)
					{
						var entitys = db.Iturs.Where(e => e.LocationCode.CompareTo(locationCode) == 0).
							Select(e => e.IturCode).Distinct().ToList();
						ret.AddRange(entitys);
					}
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturCodesForLocationCode", exp);
				}
			}
			return ret;
		}
		public string[] GetIturCodes(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string[] codes = db.Iturs.ToList().Select(e => e.IturCode).Distinct().ToArray();
				return codes;
			}
		}

		public List<string> GetIturCodesWithInventProduct(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> codes = db.Iturs.Where(e => e.TotalItem > 0).ToList().Select(e => e.IturCode).ToList();
				return codes;
			}
		}

		public int[] GetIturNumbers(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				int[] numbers = db.Iturs.ToList().Select(e => e.Number).ToArray();
				return numbers;
			}
		}

		public Itur GetIturByDocumentCode(string documentCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				App_Data.DocumentHeader documentHeader = db.DocumentHeaders.
					FirstOrDefault(e => e.DocumentCode.CompareTo(documentCode) == 0);
				if (documentHeader != null)
				{
					var entity = db.Iturs.FirstOrDefault(e => e.IturCode.CompareTo(documentHeader.IturCode) == 0);
					return entity.ToDomainObject();
				}
				else return null;
			}
		}

		public Iturs GetItursByStatusCode(string statusCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = AsQueryable(db.Iturs).Where(e => e.StatusIturCode.CompareTo(statusCode) == 0)
										   .ToList().Select(e => e.ToDomainObject());
				return Iturs.FromEnumerable(domainObjects);
			}
		}


		public void DeleteHierarchical(Itur itur, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var documentHeaderEntities = db.DocumentHeaders.Where(e => e.IturCode == itur.IturCode).ToList();
				if (documentHeaderEntities != null)
				{
					foreach (var dh in documentHeaderEntities)
					{
						var inventProductEntities = db.InventProducts.Where(e => e.DocumentCode == dh.DocumentCode).ToList();
						if (inventProductEntities != null)
						{
							inventProductEntities.ForEach(e => db.InventProducts.DeleteObject(e));
						}
					}

					documentHeaderEntities.ForEach(e => db.DocumentHeaders.DeleteObject(e));
				}

				var iturEntety = this.GetEntityByCode(db, itur.IturCode);
				if (iturEntety != null)
				{
					db.Iturs.DeleteObject(iturEntety);
				}

				db.SaveChanges();
			}
		}



		public void ClearIturHierarchical(Itur itur, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var documentHeaderEntities = db.DocumentHeaders.Where(e => e.IturCode == itur.IturCode).ToList();
				if (documentHeaderEntities != null)
				{
					foreach (var dh in documentHeaderEntities)
					{
						var inventProductEntities = db.InventProducts.Where(e => e.DocumentCode == dh.DocumentCode).ToList();
						if (inventProductEntities != null)
						{
							inventProductEntities.ForEach(e => db.InventProducts.DeleteObject(e));
						}
					}

					documentHeaderEntities.ForEach(e => db.DocumentHeaders.DeleteObject(e));
				}
				db.SaveChanges();

				var iturEntety = this.GetEntityByCode(db, itur.IturCode);
				if (iturEntety != null)
				{
					this.RefillApproveStatusBitByIturCode(itur.IturCode, pathDB);
				}
			}


		}

		public void Delete(Itur itur, string pathDB)
		{
			if (itur == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//var documentHeaderEntities = db.DocumentHeaders.Where(e => e.IturCode == itur.IturCode).ToList();
				//if (documentHeaderEntities != null)
				//{
				//    foreach (var dh in documentHeaderEntities)
				//    {
				//        var inventProductEntities = db.InventProducts.Where(e => e.DocumentCode == dh.DocumentCode).ToList();
				//        if (inventProductEntities != null)
				//        {
				//            inventProductEntities.ForEach(e => db.InventProducts.DeleteObject(e));
				//        }
				//    }

				//    documentHeaderEntities.ForEach(e => db.DocumentHeaders.DeleteObject(e));
				//}

				var iturEntety = this.GetEntityByCode(db, itur.IturCode);
				if (iturEntety != null)
				{
					db.Iturs.DeleteObject(iturEntety);
				}

				db.SaveChanges();
			}
		}

		public void Delete(string iturCode, string pathDB)
		{
			if (string.IsNullOrWhiteSpace(iturCode) == true) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var iturEntety = this.GetEntityByCode(db, iturCode);
				if (iturEntety != null)
				{
					db.Iturs.DeleteObject(iturEntety);
				}

				db.SaveChanges();
			}
		}

		public void DeleteOnlyEmpty(Iturs iturs, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var itur in iturs)
				{
					var iturEntety = this.GetEntityByCode(db, itur.IturCode);
					if (iturEntety != null)
					{
						if (iturEntety.StatusIturGroupBit == (int)IturStatusGroupEnum.Empty)
						{
							db.Iturs.DeleteObject(iturEntety);
						}
					}
				}
				db.SaveChanges();
			}
		}



		public void DeleteAllByLocationCode(string locationCode, string pathDB)
		{
			//using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			//{
			//    var entities = db.Iturs.Where(e => e.LocationCode == locationCode).ToList();
			//    if (entities == null) return;
			//    entities.ForEach(e => db.Iturs.DeleteObject(e));
			//    db.SaveChanges();
			//}

			//var iturs1 = this._selectedItems.Select(z => z.Itur);
			//Iturs iturs = Iturs.FromEnumerable(iturs1);
			//this._iturRepository.DeleteOnlyEmpty(iturs, base.GetDbPath);


		}


		public void Insert(Itur itur, DocumentHeader documentHeader, string pathDB)
		{
			if (itur == null) return;
			if (documentHeader == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				itur.CreateDate = DateTime.Now;
				var entity = itur.ToEntity();
				db.Iturs.AddObject(entity);
				db.SaveChanges();

				var entityDocumentHeader = db.DocumentHeaders.FirstOrDefault(e => e.DocumentCode == documentHeader.DocumentCode);
				entityDocumentHeader.IturCode = entity.IturCode;
				db.SaveChanges();
			}
		}

		public void Insert(Itur itur, string pathDB)
		{
			if (itur == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				itur.CreateDate = DateTime.Now;
				var entity = itur.ToEntity();
				db.Iturs.AddObject(entity);
				db.SaveChanges();
			}
		}



		public void Insert(Iturs iturs, string pathDB)
		{
			if (iturs == null) return;
			Dictionary<string, Itur> iturFromDBDictionary = GetIturDictionary(pathDB, true);
			//Dictionary<string, Itur> iturToDBDictionary = new Dictionary<string,Itur>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var itur in iturs)
				{
					if (iturFromDBDictionary.ContainsKey(itur.IturCode) == false)
					{
						//iturToDBDictionary[itur.IturCode] = itur;
						var entity = itur.ToEntity();
						db.Iturs.AddObject(entity);
					}
				}
				// IImportIturRepository iturADORepository = this._serviceLocator.GetInstance<IImportIturRepository>();
				db.SaveChanges();
			}
		}



		public void Insert(Itur itur, Location location, string pathDB)
		{
			this.Insert(itur, location.Code, pathDB);
		}

		public void Insert(Itur itur, string locationCode, string pathDB)
		{
			if (itur == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				itur.CreateDate = DateTime.Now;
				var entity = itur.ToEntity();
				entity.LocationCode = locationCode;
				db.Iturs.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Update(Itur itur, string pathDB)
		{
			if (itur == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, itur.IturCode);
				if (entity == null) return;
				entity.ApplyChanges(itur);
				db.SaveChanges();
			}
		}

		public void UpdateIturCode(Iturs iturs, string pathDB)
		{
			if (iturs == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (Itur itur in iturs)
				{
					if (itur == null) continue;
					var entity = this.GetEntityByCode(db, itur.IturCode);
					string newIturCode = itur.Description;
					if (string.IsNullOrWhiteSpace(newIturCode) == false)
					{
						entity.IturCode = newIturCode;
						entity.Description = "";
					}
					//if (entity == null) continue;
					//entity.ApplyChanges(itur);
				}
				db.SaveChanges();
			}
		}

		public void UpdateIturCode(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = AsQueryable(db.Iturs).ToList().Select(e => e);
				foreach (var entity in entities)
				{
					string newIturCode = entity.Description;
					if (string.IsNullOrWhiteSpace(newIturCode) == false)
					{
						entity.IturCode = newIturCode;
						entity.Description = "";
					}
				}
				db.SaveChanges();
			}
		}

		public void DeleteAllIturs(string pathDB)
		{

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
		
				var entities1 = db.Iturs.ToList();
				if (entities1 == null) return;
				entities1.ForEach(e => db.Iturs.DeleteObject(e));
				db.SaveChanges();
		
			}
		}
		public void Update(Iturs iturs, string pathDB)
		{
			if (iturs == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (Itur itur in iturs)
				{
					if (itur == null) continue;
					var entity = this.GetEntityByCode(db, itur.IturCode);
					if (entity == null) continue;
					entity.ApplyChanges(itur);
				}
				db.SaveChanges();
			}
		}

		public void UpdatePrefix(string prefixNew, string pathDB)
		{
			if (string.IsNullOrWhiteSpace(prefixNew) == true) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = AsQueryable(db.Iturs).ToList().Select(e => e);
				foreach (var entity in entities)
				{
					entity.NumberPrefix = prefixNew;
					entity.Description = prefixNew + entity.NumberSufix;
				}
				db.SaveChanges();
			}
		}

		public Iturs GetItursByNumber(string number, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = AsQueryable(db.Iturs).Where(e => e.Number.CompareTo(number) == 0)
									  .ToList().Select(e => e.ToDomainObject());
				return Iturs.FromEnumerable(entities);
			}
		}

		public Iturs GetItursByPrefix(string prefix, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = AsQueryable(db.Iturs).Where(e => e.NumberPrefix.CompareTo(prefix) == 0)
									  .ToList().Select(e => e.ToDomainObject());
				return Iturs.FromEnumerable(entities);
			}
		}
		public Iturs GetItursIturCodeList(List<string> iturCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Itur> GetItursByNumber(int number, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<Itur> iturs = AsQueryable(db.Iturs).Where(e => e.Number.CompareTo(number) == 0).ToList().Select(x => x.ToDomainObject()).AsEnumerable<Itur>();
				return iturs;
			}
		}

		public IEnumerable<Itur> GetItursByNumberInLocation(int number, string locationCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				IEnumerable<Itur> iturs = AsQueryable(db.Iturs).Where(e => e.Number.CompareTo(number) == 0 && e.LocationCode.CompareTo(locationCode) == 0).ToList().Select(x => x.ToDomainObject()).AsEnumerable<Itur>();
				return iturs;
			}
		}

		public Iturs GetItursByDate(DateTime createDate, string pathDB)  //TODO:
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = from i in db.Iturs
									join dh in db.DocumentHeaders on i.IturCode equals dh.IturCode
									join s in db.Sessions on dh.SessionCode equals s.SessionCode
									where s.CreateDate == createDate
									select i.ToDomainObject();
				return Iturs.FromEnumerable(domainObjects);
			}
		}

		public Itur GetIturByCode(string iturCode, string pathDB)
		{
			if (string.IsNullOrEmpty(iturCode) == true) return null;

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, iturCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}


		public Itur GetIturByErpIturCode(string erpIturCode, string pathDB, bool nativ = false)
		{
			if (string.IsNullOrEmpty(erpIturCode) == true) return null;

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = AsQueryable(db.Iturs).FirstOrDefault(e => e.ERPIturCode.CompareTo(erpIturCode) == 0);
				if (entity != null)
				{
					return entity.ToDomainObject();
				}
				else //иначе создаем !!! для Nativ only
				{
					if (nativ == true)
					{
						Itur itur = GetOrCreateNativItur(erpIturCode, pathDB);
						itur.CreateDate = DateTime.Now;
						var entityItur = itur.ToEntity();
						db.Iturs.AddObject(entityItur);
						db.SaveChanges();

						var entityNew = AsQueryable(db.Iturs).FirstOrDefault(e => e.ERPIturCode.CompareTo(erpIturCode) == 0);
						if (entityNew != null)
						{
							return entityNew.ToDomainObject();
						}
					}
				}
			}
			return null;
		}


		public List<string> GetIturCodeListByTag(string pathDB, string tag)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var iturs = db.Iturs.Where(x => x.Tag.CompareTo(tag) == 0).ToList();
					ret = iturs.Select(e => e.IturCode).Distinct().ToList();
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturCodeListByTag", exp);
				}
			}
			return ret;
		}


		public List<string> GetIturCodeListIncludedTag(string pathDB, string tag)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var iturs = db.Iturs.Where(x => x.Tag.Contains(tag) == true).ToList();
					ret = iturs.Select(e => e.IturCode).Distinct().ToList();
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturCodeListIncludedTag", exp);
				}
			}
			return ret;
		}


		public void RefillIturStatistic(string pathDB)
		{
			//string inputTypeCodeB = InputTypeCodeEnum.B.ToString();
			//Dictionary<string, DocumentHeader> documentHeaderDictionary = this.GetDocumentHeaderDictionary(pathDB, true);

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = AsQueryable(db.Iturs).ToList().Select(e => e);
				foreach (var entity in entities)
				{

					//List<string> iturCodes = db.Iturs.Select(x => x.IturCode).Distinct().ToList();
					////fo Iturs
					//Dictionary<string, App_Data.Itur> iturEntityDictionary = db.Iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);
					//foreach (var code in iturCodes)
					//{
					//	if (string.IsNullOrEmpty(code) != true)
					//	{
					//App_Data.Itur iturEntity = new App_Data.Itur();
					//bool retCan = iturEntityDictionary.TryGetValue(code, out iturEntity);
					////bool retCan = documentHeaderDictionary.TryGetValue(docCode, out documentHeader);
					//if (retCan == true)
					//{
					var iturDomain = entity.ToDomainObject();
					double quantityEdit = 0;
					long total = 0;
					//DateTime fromTime = DateTime.Now;
					//DateTime toTime = fromTime;
					//long ticksTimeSpan = 0;
					//string periodFromTo = "00:00:00";

					var inventProductEntities = db.InventProducts.Where(e => e.IturCode == entity.IturCode)
							//&& e.InputTypeCode == inputTypeCodeB).ToList().Select(e => e.ToDomainObject());
							.ToList().Select(e => e.ToDomainObject());
					if (inventProductEntities != null && inventProductEntities.Count() > 0)
					{

						try { quantityEdit = inventProductEntities.Sum(x => x.QuantityEdit); }
						catch { }

						//total = inventProductEntities.LongCount();
						List<string> ipCode = inventProductEntities.Select(x => x.Makat).Distinct().ToList();
						total = ipCode.LongCount();

						//try
						//{
						//	fromTime = inventProductEntities.Min(x => x.CreateDate);
						//	toTime = inventProductEntities.Max(x => x.CreateDate);
						//}
						//catch { }
						//TimeSpan fromTo = (TimeSpan)(toTime - fromTime);

						//try
						//{
						//	ticksTimeSpan = fromTo.Ticks;
						//	periodFromTo = fromTo.ToString(@"hh\:mm\:ss");
						//}
						//catch { }
					}

					iturDomain.SumQuantityEdit = quantityEdit;
					iturDomain.TotalItem = total; // Distinct Makat
												  //need add DistinctMakat
												  //iturDomain.FromTime = fromTime;
												  //iturDomain.ToTime = toTime;
												  //iturDomain.TicksTimeSpan = ticksTimeSpan;
												  //iturDomain.PeriodFromTo = periodFromTo;
					entity.ApplyChanges(iturDomain);

					//}
					//}
				}//foreach iturcode

				db.SaveChanges();
			}
			//documentHeaderEntityDictionary.Clear();
			//documentHeaderEntityDictionary = null;
		}


		public int GetMaxNumber(string prefix, string pathDB)
		{
			prefix = prefix.PadLeft(4, '0');
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = db.Iturs.Where(e => e.NumberPrefix == prefix && e.Number != 9999);
				if (entity == null) return 0;
				if (entity.Count() == 0) return 0;
				var number = entity.Max(e => e.Number);
				return Convert.ToInt32(number);
			}
		}


		#endregion

	

		#region statusBitnotuse
		//not use
		public StatusIturs GetStatusIturList(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var statusIturs = StatusIturs.FromEnumerable(db.Iturs.Select(e =>
				new StatusItur { Code = e.StatusIturCode != null ? e.StatusIturCode : DomainUnknownCode.UnknownStatus }).
					Distinct());
				return statusIturs;
			}
		}

		//not use
		public BitArray GetResultStatusBitOrByIturCode(string code, string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					var entities = db.DocumentHeaders.Where(e => e.IturCode == code).
						ToList().Select(e => e.StatusDocHeaderBit).ToArray();
					if (entities == null) return null;
					BitArray bitArray = BitStatus.GetResultBitArrayOr(entities);
					//int ret = Convert.ToInt32(bitArray);
					return bitArray;
				}
			}
			else
			{
				using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					var entity = this.GetEntityByCode(db, code);
					if (entity == null) return BitStatus.GetBitArray(0);
					return BitStatus.GetBitArray(entity.StatusIturBit);
				}
			}
		}

		//not use
		public int GetResultStatusIntOrByIturCode(string code, string pathDB, bool refill = false)
		{
			BitArray bitArray = this.GetResultStatusBitOrByIturCode(code, pathDB, refill);
			return Convert.ToInt32(bitArray);
		}
		#endregion

		#region statusBit
		// =======================  main  RefillApproveStatusBitByIturCode !!! ==============================================
		public int RefillApproveStatusBitByIturCode(string iturCode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//документы Itur	- устанавливаем документам статусы аппрув по бизнес- логике
				IEnumerable<App_Data.DocumentHeader> documents = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
					ToList().OrderBy(e => e.ID).Select(e => e);
				if (documents == null) return 0;
				int k = 0;
				foreach (var doc in documents)
				{
					k++;
					int statusApproveBit = 0;
					doc.StatusApproveBit = 0;
					doc.StatusInventProductBit = 0;

					//OLD замена
					//int statusInventProductBit = BitStatus.GetInt32(this._inventProductRepository.
					//	GetResultStatusBitOrByDocumentCode(doc.DocumentCode, pathDB));

					//doc.StatusInventProductBit = doc.StatusInventProductBit.Or(statusInventProductBit);

					int[] entitiesStatusInventProductBit = db.InventProducts.Where(e => e.DocumentCode == doc.DocumentCode).Select(e => e.StatusInventProductBit).Distinct().ToArray();

					int statusInventProductBit = 0;
					if (entitiesStatusInventProductBit != null)
					{
						if (entitiesStatusInventProductBit.Length == 1)
						{
							statusInventProductBit = entitiesStatusInventProductBit[0];
						}
						else if (entitiesStatusInventProductBit.Length == 0)
						{
							statusInventProductBit = 0;
						}
						else
						{
							statusInventProductBit = BitStatus.GetInt32(BitStatus.GetResultBitArrayOr(entitiesStatusInventProductBit));
						}
					}

					if (statusInventProductBit != 0)
					{
						doc.StatusInventProductBit = doc.StatusInventProductBit.Or(statusInventProductBit);
					}
					//end OLD замена

					if (doc.StatusInventProductBit != 0 || doc.StatusDocHeaderBit != 0)
					{
						doc.Approve = false;
					}
					//1 вариант
					//if (doc.Approve == null) doc.Approve = false;
					//doc.StatusApproveBit = doc.Approve == true ? 1 : 0;
					//если документ один
					if (documents.Count() == 1)
					{
						//и если его св-во еще не трогали после импорта	 || уже принято
						if (doc.Approve == null || doc.Approve == true)
						{
							doc.Approve = true;
							statusApproveBit = (int)CodeIturStatusEnum.Approve
									+ (int)CodeIturStatusEnum.OneDoc; //DocumentStatusEnum.OneDocIsApprove; //1 + 2;
						}
						else    //если уже не принято
						{
							statusApproveBit = (int)CodeIturStatusEnum.NotApprove
							+ (int)CodeIturStatusEnum.OneDoc; //DocumentStatusEnum.OneDocIsNotApprove; //0 + 2;
						}
					}
					else
					{   // если несколько документов в Itur
						if (k == 1)       //если первый документ в списке
						{
							//и если его св-во еще не трогали после импорта	 || уже принято
							if (doc.Approve == null || doc.Approve == true)
							{
								doc.Approve = true;
								statusApproveBit = (int)CodeIturStatusEnum.Approve
									+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsApprove; //1 + 4;
							}
							else    //если уже не принято
							{
								statusApproveBit = (int)CodeIturStatusEnum.NotApprove
									+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsNotApprove; //0 + 4;
							}
						}
						else            // если не первый документ в списке
						{
							//и если его св-во еще не трогали после импорта	 || уже принято
							if (doc.Approve == null || doc.Approve == false)
							{
								doc.Approve = false;
								statusApproveBit = (int)CodeIturStatusEnum.NotApprove
									+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsNotApprove; //0 + 4;
							}
							else    //если уже принято
							{
								statusApproveBit = (int)CodeIturStatusEnum.Approve
								+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsApprove; //1 + 4;
							}
						}
					}
					if (statusApproveBit != 0)
					{
						doc.StatusApproveBit = doc.StatusApproveBit.Or(statusApproveBit);
					}
				}        //fareach doc

				db.SaveChanges();

				//List<string> docCodes = this._documentHeaderRepository.GetDocumentCodeList(documents);
				//this._documentHeaderRepository.RefillDocumentStatistic(docCodes, pathDB);
				//	}


				// Пересчитываем статусы самого Itur 
				//using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				//{
				App_Data.Itur itur = db.Iturs.FirstOrDefault(e => e.IturCode.CompareTo(iturCode) == 0);
				//var itur = this.GetEntityByCode(db, iturCode);
				if (itur == null) return 0;
				//reset Bit
				itur.StatusIturBit = 0;
				itur.StatusDocHeaderBit = 0;
				//BitArray statusIturBitArrayOr = new BitArray(new int[] { 0 });

				documents = db.DocumentHeaders.Where(e => e.IturCode == iturCode).ToList().Select(e => e);

				// Все документы итура и их статус валидации при парсинге документа
				//int[] bitDocHeaders = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
				//	ToList().Select(e => e.StatusDocHeaderBit).ToArray();
				int[] bitDocHeaders = documents.Select(e => e.StatusDocHeaderBit).ToArray();
				BitArray bitArrayOrDocHeaders = BitStatus.GetResultBitArrayOr(bitDocHeaders);
				int bitStatusDocHeaders = BitStatus.GetInt32(bitArrayOrDocHeaders);
				//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrDocHeaders);

				// Все документы итура и их статус валидации при парсинге инвент продакт в них входящих
				//int[] bitInventProductDocs = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
				//	ToList().Select(e => e.StatusInventProductBit).ToArray();
				int[] bitInventProductDocs = documents.Select(e => e.StatusInventProductBit).ToArray();
				BitArray bitArrayOrInventProductDocs = BitStatus.GetResultBitArrayOr(bitInventProductDocs);
				int bitStatusInventProductDocs = BitStatus.GetInt32(bitArrayOrInventProductDocs);
				//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrInventProductDocs);

				// Все документы итура и их статус Approve и CountDoc
				//int[] bitApproveDocs = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
				//	ToList().Select(e => e.StatusApproveBit).ToArray();
				int[] bitApproveDocs = documents.Select(e => e.StatusApproveBit).ToArray();

				//Вариант1-  хотя бы один документ Approve
				/*BitArray bitArrayOrApproveDocs  = BitStatus.GetResultBitArrayOr(bitApproveDocs);
				int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayOrApproveDocs);*/

				//Вариант2-  все документы Approve
				BitArray bitArrayAndApproveDocs = BitStatus.GetResultBitArrayAnd(bitApproveDocs);
				int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayAndApproveDocs);


				//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrApproveDocs);

				//BitArray bitArray = BitStatus.GetResultBitArrayAnd(bits);
				//int[] array = new int[1];
				//bitArray.CopyTo(array, 0);

				//itur.StatusDocHeaderBit = BitStatus.GetResultBitIntArrayAnd(bitDocHeaders);

				//itur.StatusIturBit = BitStatus.GetResultBitIntArrayAnd(bitDocHeaders);
				//int statusIturBit = BitStatus.GetInt32(statusIturBitArrayOr);

				// WarningConvert DocHeaders  InventProductDocs
				if (bitStatusDocHeaders != 0)
				{
					itur.StatusDocHeaderBit = itur.StatusDocHeaderBit.Or(bitStatusDocHeaders);
				}

				if (bitStatusInventProductDocs != 0)
				{
					itur.StatusDocHeaderBit = itur.StatusDocHeaderBit.Or(bitStatusInventProductDocs);
				}

				if (bitStatusApproveDocs != 0)
				{
					itur.StatusIturBit = itur.StatusIturBit.Or(bitStatusApproveDocs);
				}


				if (itur.StatusDocHeaderBit != 0)
				{
					//itur.StatusIturBit = itur.StatusIturBit.Or((int)IturStatusEnum.WarningConvert);  //TODO:
					//itur.StatusIturBit =(int)IturStatusEnum.WarningConvert;
					itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
						this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.WarningConvert]);
				}
				else
				{
					//тут проверяется статус самого Itur
					// Disabled
					if (itur.Disabled == true)
					{
						itur.StatusIturBit = itur.StatusIturBit.Or((int)CodeIturStatusEnum.DisableItur);
						if (bitStatusApproveDocs == (int)IturStatusEnum.NoOneDoc) //? test	NoOneDoc и disable
						{
							itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
							this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndNoOneDoc]);
						}
						else
						{
							itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
							this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndSomeDocIsNotApprove]);
						}
					}
					else
					{
						itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
							  this._statusIturRepository.BitStatusIturEnumDictionary[itur.StatusIturBit]);
					}
				}



				//if (itur.StatusDocHeaderBit != 0)
				//{
				//    itur.StatusIturBit = itur.StatusIturBit.Or((int)IturStatusEnum.WarningConvert);  //TODO:
				//    //itur.StatusIturBit =(int)IturStatusEnum.WarningConvert;
				//    itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
				//        this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.WarningConvert]);
				//}
				//else
				//{
				//    itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
				//            this._statusIturRepository.BitStatusIturEnumDictionary[itur.StatusIturBit]);
				//}

				db.SaveChanges();
				return itur.StatusIturBit;
			}
		}

		//===========================

		public List<string> RefillApproveStatusBitByStep1Docs(List<string> sessionCodeList, string pathDB)
		{
			List<string> iturCodes = new List<string>();
			Dictionary<string, Dictionary<string, App_Data.DocumentHeader>> iturDocsDictionary = new Dictionary<string, Dictionary<string, App_Data.DocumentHeader>>();
		
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//документы Itur	- устанавливаем документам статусы аппрув по бизнес- логике
				if (sessionCodeList == null) return iturCodes;
				if (sessionCodeList.Count == 0) return iturCodes;

				List<App_Data.DocumentHeader> documentsAll = db.DocumentHeaders.Select(e => e).ToList();

				foreach (var sessionCode in sessionCodeList)
				{
					var entetyInventProducts = db.InventProducts.ToList().Select(e => e);
					IEnumerable<App_Data.DocumentHeader> documentsSession = db.DocumentHeaders.Where(e => e.SessionCode == sessionCode).
								ToList().OrderBy(e => e.ID).Select(e => e);

					//Dictionary<string, App_Data.DocumentHeader> _documentDictionary = documents.Select(e => e.StatusInventProductBit).Distinct().ToDictionary(k => k.DocumentCode);
					if (documentsSession == null) continue;

					//int k = 0;
					foreach (var doc in documentsSession)
					{
						if (iturCodes.Contains(doc.IturCode) == false)
						{
							iturCodes.Add(doc.IturCode);
							iturDocsDictionary[doc.IturCode] = new Dictionary<string, App_Data.DocumentHeader>();
							Dictionary<string, App_Data.DocumentHeader> docDictionary = documentsAll.Where(e => e.IturCode == doc.IturCode).Distinct().ToDictionary(k => k.DocumentCode);
							iturDocsDictionary[doc.IturCode] = docDictionary;
						}
					}

					foreach (var doc in documentsSession)
					{
						doc.StatusApproveBit = 0;
						doc.StatusInventProductBit = 0;

						int[] entitiesStatusInventProductBit = entetyInventProducts.Where(e => e.DocumentCode == doc.DocumentCode).Select(e => e.StatusInventProductBit).Distinct().ToArray();


						int statusInventProductBit = 0;
						if (entitiesStatusInventProductBit != null)
						{
							if (entitiesStatusInventProductBit.Length == 1)
							{
								statusInventProductBit = entitiesStatusInventProductBit[0];
							}
							else if (entitiesStatusInventProductBit.Length == 0)
							{
								statusInventProductBit = 0;
							}
							else
							{
								statusInventProductBit = BitStatus.GetInt32(BitStatus.GetResultBitArrayOr(entitiesStatusInventProductBit));
							}
						}

						if (statusInventProductBit != 0)
						{
							doc.StatusInventProductBit = doc.StatusInventProductBit.Or(statusInventProductBit);
						}

						if (doc.StatusInventProductBit != 0 || doc.StatusDocHeaderBit != 0)
						{
							doc.Approve = false;
						}

						iturDocsDictionary[doc.IturCode][doc.DocumentCode] = doc;
					} //fareach documents

					entetyInventProducts = null;
					documentsSession = null;

					//1 вариант
					//if (doc.Approve == null) doc.Approve = false;
					//doc.StatusApproveBit = doc.Approve == true ? 1 : 0;
					foreach (var docs in iturDocsDictionary)	   //по IturCode
					{
						int k = 0;
						var documentList = docs.Value.Values.OrderBy(e => e.ID).ToList();
						foreach (var doc in documentList)			//по Документам в Itur
						{
							//если документ один
							int statusApproveBit = 0;
							k++;
							if (documentList.Count() == 1)
							{
								
								//и если его св-во еще не трогали после импорта	 || уже принято
								if (doc.Approve == null || doc.Approve == true)
								{
									doc.Approve = true;
									statusApproveBit = (int)CodeIturStatusEnum.Approve
											+ (int)CodeIturStatusEnum.OneDoc; //DocumentStatusEnum.OneDocIsApprove; //1 + 2;
								}
								else    //если уже не принято
								{
									statusApproveBit = (int)CodeIturStatusEnum.NotApprove
									+ (int)CodeIturStatusEnum.OneDoc; //DocumentStatusEnum.OneDocIsNotApprove; //0 + 2;
								}
							}
							else
							{   // если несколько документов в Itur
								if (k == 1)       //если первый документ в списке
								{
									//и если его св-во еще не трогали после импорта	 || уже принято
									if (doc.Approve == null || doc.Approve == true)
									{
										doc.Approve = true;
										statusApproveBit = (int)CodeIturStatusEnum.Approve
											+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsApprove; //1 + 4;
									}
									else    //если уже не принято
									{
										statusApproveBit = (int)CodeIturStatusEnum.NotApprove
											+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsNotApprove; //0 + 4;
									}
								}
								else            // если не первый документ в списке
								{
									//и если его св-во еще не трогали после импорта	 || уже принято
									if (doc.Approve == null || doc.Approve == false)
									{
										doc.Approve = false;
										statusApproveBit = (int)CodeIturStatusEnum.NotApprove
											+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsNotApprove; //0 + 4;
									}
									else    //если уже принято
									{
										statusApproveBit = (int)CodeIturStatusEnum.Approve
										+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsApprove; //1 + 4;
									}
								}
							}

							if (statusApproveBit != 0)
							{
								doc.StatusApproveBit = doc.StatusApproveBit.Or(statusApproveBit);
							}
						}      //	 fareach docs для одного Itur
					}        //fareach iturDocsDictionary

				}
				db.SaveChanges();
			}

			return iturCodes;
		}

		public void RefillApproveStatusBitByStep2Iturs(List<string> iturCodes, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entetyDocumentHeaders = db.DocumentHeaders.ToList().Select(e => e);
				var entetyIturs = db.Iturs.ToList().Select(e => e);
				foreach (string iturCode in iturCodes)
				{
					App_Data.Itur itur = entetyIturs.FirstOrDefault(e => e.IturCode.CompareTo(iturCode) == 0);
					//var itur = this.GetEntityByCode(db, iturCode);
					if (itur == null) return;
					//reset Bit
					itur.StatusIturBit = 0;
					itur.StatusDocHeaderBit = 0;
					//BitArray statusIturBitArrayOr = new BitArray(new int[] { 0 });

					IEnumerable<App_Data.DocumentHeader> documents = entetyDocumentHeaders.Where(e => e.IturCode == iturCode).ToList();

					// Все документы итура и их статус валидации при парсинге документа
					//int[] bitDocHeaders = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
					//	ToList().Select(e => e.StatusDocHeaderBit).ToArray();
					int[] bitDocHeaders = documents.Select(e => e.StatusDocHeaderBit).ToArray();
					BitArray bitArrayOrDocHeaders = BitStatus.GetResultBitArrayOr(bitDocHeaders);
					int bitStatusDocHeaders = BitStatus.GetInt32(bitArrayOrDocHeaders);
					//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrDocHeaders);

					// Все документы итура и их статус валидации при парсинге инвент продакт в них входящих
					//int[] bitInventProductDocs = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
					//	ToList().Select(e => e.StatusInventProductBit).ToArray();
					int[] bitInventProductDocs = documents.Select(e => e.StatusInventProductBit).ToArray();
					BitArray bitArrayOrInventProductDocs = BitStatus.GetResultBitArrayOr(bitInventProductDocs);
					int bitStatusInventProductDocs = BitStatus.GetInt32(bitArrayOrInventProductDocs);
					//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrInventProductDocs);

					// Все документы итура и их статус Approve и CountDoc
					//int[] bitApproveDocs = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
					//	ToList().Select(e => e.StatusApproveBit).ToArray();
					int[] bitApproveDocs = documents.Select(e => e.StatusApproveBit).ToArray();

					//Вариант1-  хотя бы один документ Approve
					/*BitArray bitArrayOrApproveDocs  = BitStatus.GetResultBitArrayOr(bitApproveDocs);
					int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayOrApproveDocs);*/

					//Вариант2-  все документы Approve
					BitArray bitArrayAndApproveDocs = BitStatus.GetResultBitArrayAnd(bitApproveDocs);
					int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayAndApproveDocs);


					//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrApproveDocs);

					//BitArray bitArray = BitStatus.GetResultBitArrayAnd(bits);
					//int[] array = new int[1];
					//bitArray.CopyTo(array, 0);

					//itur.StatusDocHeaderBit = BitStatus.GetResultBitIntArrayAnd(bitDocHeaders);

					//itur.StatusIturBit = BitStatus.GetResultBitIntArrayAnd(bitDocHeaders);
					//int statusIturBit = BitStatus.GetInt32(statusIturBitArrayOr);

					// WarningConvert DocHeaders  InventProductDocs
					if (bitStatusDocHeaders != 0)
					{
						itur.StatusDocHeaderBit = itur.StatusDocHeaderBit.Or(bitStatusDocHeaders);
					}

					if (bitStatusInventProductDocs != 0)
					{
						itur.StatusDocHeaderBit = itur.StatusDocHeaderBit.Or(bitStatusInventProductDocs);
					}

					if (bitStatusApproveDocs != 0)
					{
						itur.StatusIturBit = itur.StatusIturBit.Or(bitStatusApproveDocs);
					}


					if (itur.StatusDocHeaderBit != 0)
					{
						//itur.StatusIturBit = itur.StatusIturBit.Or((int)IturStatusEnum.WarningConvert);  //TODO:
						//itur.StatusIturBit =(int)IturStatusEnum.WarningConvert;
						itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
							this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.WarningConvert]);
					}
					else
					{
						//тут проверяется статус самого Itur
						// Disabled
						if (itur.Disabled == true)
						{
							itur.StatusIturBit = itur.StatusIturBit.Or((int)CodeIturStatusEnum.DisableItur);
							if (bitStatusApproveDocs == (int)IturStatusEnum.NoOneDoc) //? test	NoOneDoc и disable
							{
								itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndNoOneDoc]);
							}
							else
							{
								itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndSomeDocIsNotApprove]);
							}
						}
						else
						{
							itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								  this._statusIturRepository.BitStatusIturEnumDictionary[itur.StatusIturBit]);
						}
					}

				}
				db.SaveChanges();
				entetyDocumentHeaders = null;
				entetyIturs = null;
			}
		}

		//public void RefillApproveStatusBitByIturCodeTest(List<string> iturCodes, string pathDB)
		//{
		//	using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//	{
		//		foreach (string iturCode in iturCodes)
		//		{
		//			if (string.IsNullOrWhiteSpace(iturCode) == true) continue;
		//			//документы Itur	- устанавливаем документам статусы аппрув по бизнес- логике
		//			IEnumerable<App_Data.DocumentHeader> documents = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
		//				ToList().OrderBy(e => e.ID).Select(e => e);
		//			if (documents == null) continue;
		//			int k = 0;
		//			foreach (var doc in documents)
		//			{
		//				k++;
		//				int statusApproveBit = 0;
		//				doc.StatusApproveBit = 0;
		//				doc.StatusInventProductBit = 0;

		//				int statusInventProductBit = BitStatus.GetInt32(this._inventProductRepository.
		//					GetResultStatusBitOrByDocumentCode(doc.DocumentCode, pathDB));
		//				doc.StatusInventProductBit = doc.StatusInventProductBit.Or(statusInventProductBit);
		//				if (doc.StatusInventProductBit != 0 || doc.StatusDocHeaderBit != 0)
		//				{
		//					doc.Approve = false;
		//				}
		//				//1 вариант
		//				//if (doc.Approve == null) doc.Approve = false;
		//				//doc.StatusApproveBit = doc.Approve == true ? 1 : 0;
		//				//если документ один
		//				if (documents.Count() == 1)
		//				{
		//					//и если его св-во еще не трогали после импорта	 || уже принято
		//					if (doc.Approve == null || doc.Approve == true)
		//					{
		//						doc.Approve = true;
		//						statusApproveBit = (int)CodeIturStatusEnum.Approve
		//								+ (int)CodeIturStatusEnum.OneDoc; //DocumentStatusEnum.OneDocIsApprove; //1 + 2;
		//					}
		//					else	//если уже не принято
		//					{
		//						statusApproveBit = (int)CodeIturStatusEnum.NotApprove
		//						+ (int)CodeIturStatusEnum.OneDoc; //DocumentStatusEnum.OneDocIsNotApprove; //0 + 2;
		//					}
		//				}
		//				else
		//				{	// если несколько документов в Itur
		//					if (k == 1)		  //если первый документ в списке
		//					{
		//						//и если его св-во еще не трогали после импорта	 || уже принято
		//						if (doc.Approve == null || doc.Approve == true)
		//						{
		//							doc.Approve = true;
		//							statusApproveBit = (int)CodeIturStatusEnum.Approve
		//								+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsApprove; //1 + 4;
		//						}
		//						else	//если уже не принято
		//						{
		//							statusApproveBit = (int)CodeIturStatusEnum.NotApprove
		//								+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsNotApprove; //0 + 4;
		//						}
		//					}
		//					else			// если не первый документ в списке
		//					{
		//						//и если его св-во еще не трогали после импорта	 || уже принято
		//						if (doc.Approve == null || doc.Approve == false)
		//						{
		//							doc.Approve = false;
		//							statusApproveBit = (int)CodeIturStatusEnum.NotApprove
		//								+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsNotApprove; //0 + 4;
		//						}
		//						else	//если уже принято
		//						{
		//							statusApproveBit = (int)CodeIturStatusEnum.Approve
		//							+ (int)CodeIturStatusEnum.SomeDocs; //DocumentStatusEnum.SomeDocsIsApprove; //1 + 4;
		//						}
		//					}
		//				}
		//				doc.StatusApproveBit = doc.StatusApproveBit.Or(statusApproveBit);
		//			}		 //fareach doc
		//		}  //fareach iturs
		//		db.SaveChanges();
		//		//List<string> docCodes = this._documentHeaderRepository.GetDocumentCodeList(documents);
		//		//this._documentHeaderRepository.RefillDocumentStatistic(docCodes, pathDB);
		//	}


		//	// Пересчитываем статусы самого Itur 
		//	using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//	{
		//		foreach (string iturCode in iturCodes)
		//		{
		//			if (string.IsNullOrWhiteSpace(iturCode) == true) continue;
		//			var itur = this.GetEntityByCode(db, iturCode);
		//			if (itur == null) continue;
		//			//reset Bit
		//			itur.StatusIturBit = 0;
		//			itur.StatusDocHeaderBit = 0;
		//			BitArray statusIturBitArrayOr = new BitArray(new int[] { 0 });

		//			// Все документы итура и их статус валидации при парсинге документа
		//			int[] bitDocHeaders = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
		//				ToList().Select(e => e.StatusDocHeaderBit).ToArray();
		//			BitArray bitArrayOrDocHeaders = BitStatus.GetResultBitArrayOr(bitDocHeaders);
		//			int bitStatusDocHeaders = BitStatus.GetInt32(bitArrayOrDocHeaders);
		//			//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrDocHeaders);

		//			// Все документы итура и их статус валидации при парсинге инвент продакт в них входящих
		//			int[] bitInventProductDocs = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
		//				ToList().Select(e => e.StatusInventProductBit).ToArray();
		//			BitArray bitArrayOrInventProductDocs = BitStatus.GetResultBitArrayOr(bitInventProductDocs);
		//			int bitStatusInventProductDocs = BitStatus.GetInt32(bitArrayOrInventProductDocs);
		//			//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrInventProductDocs);

		//			// Все документы итура и их статус Approve и CountDoc
		//			int[] bitApproveDocs = db.DocumentHeaders.Where(e => e.IturCode == iturCode).
		//				ToList().Select(e => e.StatusApproveBit).ToArray();

		//			//Вариант1-  хотя бы один документ Approve
		//			/*BitArray bitArrayOrApproveDocs  = BitStatus.GetResultBitArrayOr(bitApproveDocs);
		//			int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayOrApproveDocs);*/

		//			//Вариант2-  все документы Approve
		//			BitArray bitArrayAndApproveDocs = BitStatus.GetResultBitArrayAnd(bitApproveDocs);
		//			int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayAndApproveDocs);


		//			//statusIturBitArrayOr = statusIturBitArrayOr.Or(bitArrayOrApproveDocs);

		//			//BitArray bitArray = BitStatus.GetResultBitArrayAnd(bits);
		//			//int[] array = new int[1];
		//			//bitArray.CopyTo(array, 0);

		//			//itur.StatusDocHeaderBit = BitStatus.GetResultBitIntArrayAnd(bitDocHeaders);

		//			//itur.StatusIturBit = BitStatus.GetResultBitIntArrayAnd(bitDocHeaders);
		//			//int statusIturBit = BitStatus.GetInt32(statusIturBitArrayOr);

		//			// WarningConvert DocHeaders  InventProductDocs
		//			itur.StatusDocHeaderBit = itur.StatusDocHeaderBit.Or(bitStatusDocHeaders);
		//			itur.StatusDocHeaderBit = itur.StatusDocHeaderBit.Or(bitStatusInventProductDocs);
		//			itur.StatusIturBit = itur.StatusIturBit.Or(bitStatusApproveDocs);
		//			if (itur.StatusDocHeaderBit != 0)
		//			{
		//				//itur.StatusIturBit = itur.StatusIturBit.Or((int)IturStatusEnum.WarningConvert);  //TODO:
		//				//itur.StatusIturBit =(int)IturStatusEnum.WarningConvert;
		//				itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
		//					this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.WarningConvert]);
		//			}
		//			else
		//			{
		//				//тут проверяется статус самого Itur
		//				// Disabled
		//				if (itur.Disabled == true)
		//				{
		//					itur.StatusIturBit = itur.StatusIturBit.Or((int)CodeIturStatusEnum.DisableItur);
		//					if (bitStatusApproveDocs == (int)IturStatusEnum.NoOneDoc) //? test	NoOneDoc и disable
		//					{
		//						itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
		//						this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndNoOneDoc]);
		//					}
		//					else
		//					{
		//						itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
		//						this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndSomeDocIsNotApprove]);
		//					}
		//				}
		//				else
		//				{
		//					itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
		//						  this._statusIturRepository.BitStatusIturEnumDictionary[itur.StatusIturBit]);
		//				}
		//			}



		//			//if (itur.StatusDocHeaderBit != 0)
		//			//{
		//			//    itur.StatusIturBit = itur.StatusIturBit.Or((int)IturStatusEnum.WarningConvert);  //TODO:
		//			//    //itur.StatusIturBit =(int)IturStatusEnum.WarningConvert;
		//			//    itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
		//			//        this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.WarningConvert]);
		//			//}
		//			//else
		//			//{
		//			//    itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
		//			//            this._statusIturRepository.BitStatusIturEnumDictionary[itur.StatusIturBit]);
		//			//}
		//		} 	//foreach iturs

		//		db.SaveChanges();
		//		//return itur.StatusIturBit;
		//	}
		//}
		// =======================end   main  RefillApproveStatusBitByIturCode !!! ==============================================
		//пересчет статусов Iturs в который входят докумнеты с кодами из списка
		//вызов после импорта документов   OLD?
		public void RefillApproveStatusBit(List<string> docCodes, List<string> sessionCodeList, string pathDB)
		{
			//_refillApproveStatus = false;
			Dictionary<string, DocumentHeader> documentHeaderEntityDictionary = this._documentHeaderRepository.GetDocumentHeaderDictionary(pathDB, true);
			List<string> iturCodes = new List<string>();
			string docCodeInfo = "";
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					foreach (var docCode in docCodes)
					{
						docCodeInfo = docCode;
						iturCodes.Add(documentHeaderEntityDictionary[docCode].IturCode);
					}
				}
				catch (Exception exp)
				{
					_logger.ErrorException("RefillApproveStatusBit : DocCode " + docCodeInfo, exp);
				}

				List<string> iturCodesDistinct = iturCodes.Distinct().Select(e => e).ToList();

				List<string> itursCodeAll = db.Iturs.Select(e => e.IturCode).ToList();
				if (itursCodeAll == null) return;
				try
				{
					foreach (var iturCode in itursCodeAll) // вернуть  
					{
						if (iturCodesDistinct.Contains(iturCode) == false) continue;
						this.RefillApproveStatusBitByIturCode(iturCode, pathDB);
					}
				}
				catch (Exception exp)
				{
					_logger.ErrorException("RefillApproveStatusBit : RefillApproveStatusBitByIturCode " + docCodeInfo, exp);
				}

				//this.RefillApproveStatusBitByIturCodeTest(iturCodesDistinct, pathDB); //тест

				this._documentHeaderRepository.RefillIturStatistic(sessionCodeList, iturCodes, docCodes, pathDB); //работает
				try
				{
					this._documentHeaderRepository.RefillDocumentStatisticBySession(sessionCodeList, pathDB);        //работает
				}
				catch (Exception exp)
				{
					_logger.ErrorException("RefillApproveStatusBit : RefillDocumentStatisticBySession " + docCodeInfo, exp);
				}
			}
			//_refillApproveStatus = true;
		}



		public void RefillApproveStatusBitByStep(List<string> docCodes, List<string> sessionCodeList, string pathDB)     //второй вариант через 	  sessionCode и другим циклам
		{
			//Step1 - by Docs
			//List<string> iturCodes = new List<string>();
			List<string> iturCodes = this.RefillApproveStatusBitByStep1Docs(sessionCodeList, pathDB);
			if (iturCodes == null) return;
			if (iturCodes.Count == 0) return;
			//Step2 - by Iturss
			this.RefillApproveStatusBitByStep2Iturs(iturCodes, pathDB) ;

			//Вернуть - убрано для теста
			this._documentHeaderRepository.RefillIturStatistic(sessionCodeList, iturCodes, docCodes, pathDB); //работает
			try
			{
				this._documentHeaderRepository.RefillDocumentStatisticBySession(sessionCodeList, pathDB);        //работает
			}
			catch (Exception exp)
			{
				_logger.ErrorException("RefillApproveStatusBit : RefillDocumentStatisticBySession ", exp);
			}

		}

		// Пересчит всех статусов Itur  из Main формы
		public void RefillApproveStatusBit(string pathDB)
		{
			//_refillApproveStatus = false;
			//IIturAnalyzesRepository iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			//_iturAnalyzesRepository.ClearProductDictionary();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> itursCodeAll = db.Iturs.Select(e => e.IturCode).ToList();
				if (itursCodeAll == null) return;

				try
				{
					foreach (var iturCode in itursCodeAll)
					{
						this.RefillApproveStatusBitByIturCode(iturCode, pathDB);
					}
				}
				catch (Exception exp)
				{
					_logger.ErrorException("RefillApproveStatusBit :  ", exp);
				}
				//Parallel.ForEach(entities, entity =>
				//{
				//	RefillApproveStatusBitByIturCode(entity.IturCode, pathDB);
				//});

			}

			//!! работает перенесено в другое место
			//List<string> docCodes = this._documentHeaderRepository.GetDocumentCodeList(pathDB);
			//this._documentHeaderRepository.RefillDocumentStatistic(docCodes, pathDB);

			//_refillApproveStatus = true;
		}

		//Пересчет статусов итура из списка DocumentHeaders
		public void RefillApproveStatusBit(string iturCode, List<string> docCodes, string pathDB)
		{
			//_refillApproveStatus = false;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (string.IsNullOrWhiteSpace(iturCode) == true) return;
				try
				{
					this.RefillApproveStatusBitByIturCode(iturCode, pathDB);
					//работет
					//if (docCodes != null)
					//{
					//	this._documentHeaderRepository.RefillDocumentStatistic(docCodes, pathDB);
					//}
				}
				catch (Exception exp)
				{
					_logger.ErrorException("RefillApproveStatusBit :  ", exp);
				}
			}
			//_refillApproveStatus = true;
		}

		public void RefillApproveStatusBit(Iturs iturs, string pathDB)
		{
			//_refillApproveStatus = false;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (iturs == null) return;
				foreach (var itur in iturs)
				{
					try
					{
						this.RefillApproveStatusBitByIturCode(itur.IturCode, pathDB);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("RefillApproveStatusBit :  ", exp);
					}
				}

			}
			//_refillApproveStatus = true;
		}

		//public void RefillStatusBit(string pathDB)
		//{
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        var entities = db.Iturs.Select(e => e);
		//        if (entities == null) return;
		//        foreach (var entity in entities)
		//        {
		//            int status = this.GetResultStatusIntOrByIturCode(entity.IturCode, pathDB);
		//            entity.StatusIturBit = status;
		//            entity.StatusIturGroupBit = (int)this._statusIturGroupRepository.
		//                BitStatusIturGroupEnumDictionary[entity.StatusIturBit];
		//        }
		//        db.SaveChanges();
		//    }
		//}

		//============RefillDisabled
		public void SetDisabledStatusBitByIturCode(Iturs iturs, bool disabled, string pathDB)
		{
			// Пересчитываем статусы самого Itur 
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				if (iturs == null) return;
				foreach (var it in iturs)
				{
					string code = it.IturCode;
					var itur = this.GetEntityByCode(db, code);
					if (itur == null) continue;
					itur.Disabled = disabled;
					//сбросили бит
					itur.StatusIturBit = itur.StatusIturBit.ClearBit((int)CodeIturStatusEnum.DisableItur);

					//// Все документы итура и их статус Approve и CountDoc
					int[] bitApproveDocs = db.DocumentHeaders.Where(e => e.IturCode == code).
						ToList().Select(e => e.StatusApproveBit).ToArray();

					////Вариант1-  хотя бы один документ Approve
					///*BitArray bitArrayOrApproveDocs  = BitStatus.GetResultBitArrayOr(bitApproveDocs);
					//int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayOrApproveDocs);*/

					////Вариант2-  все документы Approve
					BitArray bitArrayAndApproveDocs = BitStatus.GetResultBitArrayAnd(bitApproveDocs);
					int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayAndApproveDocs);

					if (itur.StatusDocHeaderBit != 0)
					{
						itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
						this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.WarningConvert]);
					}
					else
					{
						//тут проверяется статус самого Itur
						// Disabled
						if (itur.Disabled == true)
						{
							itur.StatusIturBit = itur.StatusIturBit.Or((int)CodeIturStatusEnum.DisableItur);
							if (bitStatusApproveDocs == (int)IturStatusEnum.NoOneDoc)
							{
								itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndNoOneDoc]);
							}
							else
							{
								itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndSomeDocIsNotApprove]);
							}
						}
						else
						{
							itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								  this._statusIturRepository.BitStatusIturEnumDictionary[itur.StatusIturBit]);
						}
					}
				}
				db.SaveChanges();
				//return itur.StatusIturBit;
			}
		}
		//==============

		public void ClearStatusBit(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = db.Iturs.Where(e => e.StatusIturBit != 0 || e.StatusIturGroupBit != 0).Select(e => e);
				if (entities == null) return;
				foreach (var entity in entities)
				{
					entity.StatusIturBit = 0;
					entity.StatusIturGroupBit = 0;
				}
				db.SaveChanges();
			}
		}


		//============RefillDisabled
		public Iturs SwitchDisabledStatusBitByIturCode(Iturs iturs, bool disabled, string pathDB)
		{
			Iturs iturList = new Iturs();
			// Пересчитываем статусы самого Itur 
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entetyDocumentHeaders = db.DocumentHeaders.Select(x => x).ToList();
				Dictionary<string, Itur> iturDictionary = this.GetIturDictionary(pathDB, true);			  
				if (iturs == null) return iturs;
				foreach (var it in iturs)
				{
					string iturCode = it.IturCode;
					if (iturDictionary.ContainsKey(iturCode) == false) continue;
					Itur itur = iturDictionary[iturCode];
					//var itur = this.GetEntityByCode(db, code);
					if (itur == null) continue;
					itur.Disabled = disabled;
					//сбросили бит
					itur.StatusIturBit = itur.StatusIturBit.ClearBit((int)CodeIturStatusEnum.DisableItur);

					//// Все документы итура и их статус Approve и CountDoc
					int[] bitApproveDocs = entetyDocumentHeaders.Where(e => e.IturCode == itur.IturCode).
						ToList().Select(e => e.StatusApproveBit).ToArray();

					////Вариант1-  хотя бы один документ Approve
					///*BitArray bitArrayOrApproveDocs  = BitStatus.GetResultBitArrayOr(bitApproveDocs);
					//int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayOrApproveDocs);*/

					////Вариант2-  все документы Approve
					BitArray bitArrayAndApproveDocs = BitStatus.GetResultBitArrayAnd(bitApproveDocs);
					int bitStatusApproveDocs = BitStatus.GetInt32(bitArrayAndApproveDocs);

					if (itur.StatusDocHeaderBit != 0)
					{
						itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
						this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.WarningConvert]);
					}
					else
					{
						//тут проверяется статус самого Itur
						// Disabled
						if (itur.Disabled == true)
						{
							itur.StatusIturBit = itur.StatusIturBit.Or((int)CodeIturStatusEnum.DisableItur);
							if (bitStatusApproveDocs == (int)IturStatusEnum.NoOneDoc)
							{
								itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndNoOneDoc]);
							}
							else
							{
								itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								this._statusIturRepository.BitStatusIturEnumDictionary[(int)IturStatusEnum.DisableAndSomeDocIsNotApprove]);
							}
						}
						else
						{
							itur.StatusIturGroupBit = (int)this._statusIturGroupRepository.GetIturStatusGroup(
								  this._statusIturRepository.BitStatusIturEnumDictionary[itur.StatusIturBit]);
						}
					}
					iturDictionary[iturCode] = itur;
				}
	
				foreach (KeyValuePair<string, Itur> keyValuePair in iturDictionary)
				{
					if (keyValuePair.Value == null) continue;
					iturList.Add(keyValuePair.Value);
				}
				//db.SaveChanges();
				//return itur.StatusIturBit;
			}
			return iturList;
		}
		//==============
		#endregion


		#region Dictionary
		public Dictionary<string, Itur> GetIturDictionary(string pathDB,
			 bool refill = false)
		{
			if (refill == true)
			{
				this.ClearIturDictionary();
				this.FillIturDictionary(pathDB);
			}
			return this._iturDictionary;
		}

		public void ClearIturDictionary()
		{
			this._iturDictionary.Clear();
			GC.Collect();
		}

		public void AddIturInDictionary(string code, Itur itur)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._iturDictionary.ContainsKey(code) == false)
			{
				this._iturDictionary.Add(code, itur);
			}
		}

		public void RemoveIturFromDictionary(string code)
		{
			try
			{
				this._iturDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistIturInDictionary(string code)
		{
			if (this._iturDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public Itur GetIturByCodeFromDictionary(string code)
		{
			if (this._iturDictionary.ContainsKey(code) == true)
			{
				return this._iturDictionary[code];
			}
			return null;
		}

		public void FillIturDictionary(string pathDB)
		{
			this.ClearIturDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Iturs iturs = this.GetIturs(pathDB);

					this._iturDictionary = iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);
					//foreach (var i in this._iturDictionary)
					//{
					//    var f = i.Key;
					//    var f1 = i.Value;
					//}
				}
				catch (Exception exp)
				{
					_logger.ErrorException("FillIturDictionary", exp);
				}
			}
		}

		public Dictionary<string, Itur> GetERPIturDictionary(string pathDB)
		{
			Dictionary<string, Itur> iturDictionary = new Dictionary<string, Itur>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Iturs iturs = this.GetIturs(pathDB);
					//	iturDictionary = iturs.Select(e => e).Distinct().ToDictionary(k => k.ERPIturCode);
					foreach (Itur itur in iturs)
					{
						if (string.IsNullOrWhiteSpace(itur.ERPIturCode) == false)
						{
							iturDictionary[itur.ERPIturCode] = itur;
						}
					}
					//foreach (var i in this._iturDictionary)
					//{
					//    var f = i.Key;
					//    var f1 = i.Value;
					//}
				}
				catch (Exception exp)
				{
					_logger.ErrorException("FillIturDictionary", exp);
				}
			}
			return iturDictionary;
		}

		public List<string> GetIturCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Iturs.Select(e => e.IturCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetIturCodeList", exp);
				}
			}
			return ret;
		}

		public List<string> GetIturCodeListWithAnyDocument(string pathDB)
		{
			List<string> retCodeList = new List<string>();
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var documentHeaders = db.DocumentHeaders.ToList();
				if (documentHeaders != null)
				{
					foreach (var dh in documentHeaders)
					{
						if (dh != null)
						{
							retCodeList.Add(dh.IturCode);
						}
					}
				}
			}
			return retCodeList;
		}

		public List<string> GetLocationCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Iturs.Select(e => e.LocationCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetLocationCodeList", exp);
				}
			}
			return ret;
		}




		public void RepairCodeFromDB(string pathDB)
		{
			//IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
			//IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();

			//Itur from DocumentHeader
			List<string> iturCodeListFromDoc = _documentHeaderRepository.GetIturCodeList(pathDB);
			List<string> iturCodeListFromItur = this.GetIturCodeList(pathDB);
			Dictionary<string, string> difference = new Dictionary<string, string>();
			foreach (var iturCodeFromDoc in iturCodeListFromDoc)
			{
				if (iturCodeListFromItur.Contains(iturCodeFromDoc) == false)
				{
					difference[iturCodeFromDoc] = iturCodeFromDoc;
				}
			}

			foreach (KeyValuePair<string, string> keyValuePair in difference)
			{

				if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
				{
					Itur iturNew = new Itur();
					iturNew.IturCode = keyValuePair.Value;
					if (iturNew.IturCode.Length == 8)
					{
						iturNew.NumberPrefix = iturNew.IturCode.Substring(0, 4);
						iturNew.NumberSufix = iturNew.IturCode.Substring(4, 4);
						//int ret = newItur.Number.ValidateInt32TrimStart0(newItur.NumberSufix);
						int number = 0;
						bool ret = Int32.TryParse(iturNew.NumberSufix.TrimStart('0'), out number);
						if (ret == true)
						{
							iturNew.Number = number;
						}
					}
					iturNew.RestoreBit = true;
					iturNew.Restore = DateTime.Now.ToString();
					iturNew.ModifyDate = DateTime.Now;
					this.Insert(iturNew, pathDB);
				}
			}

			this.RefillApproveStatusBit(pathDB);
		}
		#endregion

		#region Private methods

		private App_Data.Itur GetEntityByCode(App_Data.Count4UDB db, string iturCode)
		{
			var entity = AsQueryable(db.Iturs).FirstOrDefault(e => e.IturCode.CompareTo(iturCode) == 0);
			return entity;
		}

		public Dictionary<int, int> GetIturTotalGroupByStatuses(string pathDB)
		{
			Dictionary<int, int> result = new Dictionary<int, int>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					int totalSum = 0;
					var query = from itur in db.Iturs
								group itur by itur.StatusIturGroupBit
									into grp
								select new
								{
									Status = grp.Key,
									Total = grp.Count()
								};


					foreach (var q in query)
					{
						result[q.Status] = q.Total;
						//totalSum = totalSum + q.Total;
					}

					//result[8] = totalSum; 
				}
				catch { }
			}

			return result;
		}


		public Dictionary<string, int> GetIturTotalGroupByLocationCode(string pathDB)
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var query = from itur in db.Iturs
								group itur by itur.LocationCode
									into grp
								select new
								{
									LocationCode = grp.Key,
									Total = grp.Count()
								};


					foreach (var q in query)
					{
						result[q.LocationCode] = q.Total;
						//totalSum = totalSum + q.Total;
					}

					//result[8] = totalSum; 
				}
				catch { }
			}

			return result;
		}
		#endregion

		#region IturCreateNativ

		private Itur GetOrCreateNativItur(string erpIturCode, string dbPath)
		{
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			dtfi.ShortDatePattern = @"dd/MM/yyyy";
			dtfi.ShortTimePattern = @"hh:mm:ss";

		//	IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = this.GetERPIturDictionary(dbPath);
			}
			catch { return null; }


			//ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
			List<string> locations = _locationRepository.GetLocationCodeList(dbPath);
			Dictionary<string, int> dictionaryPrffixIndex = new Dictionary<string, int>();
			foreach (string code in locations)
			{
				dictionaryPrffixIndex[code] = 1;
				int maxNumber = this.GetMaxNumber(code, dbPath);
				if (maxNumber > 1) dictionaryPrffixIndex[code] = maxNumber;
			}

			erpIturCode = erpIturCode.TrimEnd('-');
			erpIturCode = erpIturCode.TrimEnd('-');
			erpIturCode = erpIturCode.TrimEnd('-');
			erpIturCode = erpIturCode.TrimEnd('-');

			if (string.IsNullOrWhiteSpace(erpIturCode) == true) return null;

			if (dictionaryIturCodeERP.ContainsKey(erpIturCode) == true)            //есть в текущей Count4UDB
			{
				Itur itur = dictionaryIturCodeERP[erpIturCode];
				return itur;
			}
			else    //нет в текущей Count4UDB	- itur in  Count4UDB
			{
				Itur newItur = null;
				//Insert
				string iturCodeERP = this.GetIturCodeERP(erpIturCode);
				string[] locationCodes = iturCodeERP.Split('-');
				for (int i = 0; i < locationCodes.Length; i++)
				{
					if (string.IsNullOrWhiteSpace(locationCodes[i]) == true)
					{
						locationCodes[i] = "unknown";
					}
				}
				if (locationCodes.Length > 0)
				{
					int deepNode = locationCodes.Length;
					string locationCode = this.GetLocationCode(locationCodes);
					if (string.IsNullOrWhiteSpace(locationCode) == false)
					{
						if (string.IsNullOrWhiteSpace(iturCodeERP) == false)
						{
							//Insert
							//add new Itur in Count4UDB
							Itur tempItur = GetNewIturCode(dbPath, dictionaryPrffixIndex,
							iturCodeERP, locationCode, deepNode);                                                                 //base

							newItur = new Itur();
							IturString newIturString = new IturString();
							newIturString.IturCode = tempItur.IturCode;
							newIturString.ERPIturCode = iturCodeERP;
							newIturString.LocationCode = locationCode;
							newIturString.StatusIturBit = "0";
							if (deepNode == 2) newIturString.Name = locationCodes[1];
							if (deepNode == 3) newIturString.Name = locationCodes[2];
							int retBit = newItur.ValidateError(newIturString, dtfi);
							if (retBit != 0)  //Error
							{
								return null;
							}
							else //	Error  retBit == 0 
							{
								retBit = newItur.ValidateWarning(newIturString, dtfi); //Warning
								newItur.LevelNum = deepNode;
								newItur.Disabled = false;
								if (deepNode == 1)      //root 
								{
								}
								if (deepNode == 2)          //root +1
								{
									newItur.ParentIturCode = locationCodes[0];// newItur.Level1;
								}
								if (deepNode == 3)        //root +2
								{
									newItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1];//newItur.Level1 + "-" + newItur.Level2;
								}
								if (deepNode == 4)        //root +3
								{
									newItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1] + "-" + locationCodes[2];//newItur.Level1 + "-" + newItur.Level2;
								}

								newItur.InvStatus = 0;
								newItur.NodeType = 0;

							}
						}
					}
				}
				return newItur;
			}

		}

		private string GetLocationCode(string[] locationCodes)
		{
			string locationCode = "";
			locationCode = locationCodes[0].Trim();
			locationCode = locationCode.CutLength(249);
			if (locationCode.Trim().ToLower() == "locationcode") locationCode = "";
			return locationCode;
		}

		private string GetIturCodeERP(string locationCodeFrom)
		{
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');

			string iturCodeERP = locationCodeFrom.CutLength(249);
			return iturCodeERP;
		}

		private Itur GetNewIturCode(string toDBPath, Dictionary<string, int> dictionaryPrffixIndex, string iturCodeERP, string locationCode, int deepNode)
		{
			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Itur tempItur = new Itur();
			if (dictionaryPrffixIndex.ContainsKey(locationCode) == false)
			{
				dictionaryPrffixIndex[locationCode] = 1;       //добавляем новый счетчик специально для локейшена (считаем намбер для суффикса)
				int maxNumber = this.GetMaxNumber(locationCode, toDBPath);
				if (maxNumber > 1) dictionaryPrffixIndex[locationCode] = maxNumber;
			}
			//================================
			string prefix = locationCode;
			string suffix = "";
			if (deepNode == 1)
			{
				suffix = "1";
			}
			else
			{
				int lastIndex = dictionaryPrffixIndex[locationCode];
				lastIndex++;
				suffix = lastIndex.ToString();
				dictionaryPrffixIndex[locationCode] = lastIndex;
			}


			int num = 0;
			bool ret = Int32.TryParse(suffix.TrimStart('0'), out num);
			tempItur.Number = num;
			tempItur.NumberPrefix = prefix.PadLeft(4, '0');
			tempItur.NumberSufix = suffix.ToString().PadLeft(4, '0');
			string newIturCode = tempItur.NumberPrefix + tempItur.NumberSufix;
			tempItur.IturCode = newIturCode;
			return tempItur;
		}

		#endregion IturCreateNativ

	}

}