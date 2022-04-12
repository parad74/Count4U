using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
    public class SupplierEFRepository : BaseEFRepository, ISupplierRepository
    {
		private Dictionary<string, Supplier> _supplierDictionary;
		private readonly IServiceLocator _serviceLocator;

		public SupplierEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
			: base(connection)												   
        {
			this._supplierDictionary = new Dictionary<string, Supplier>();
			this._serviceLocator = serviceLocator;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region ISupplierRepository Members

		public Suppliers GetSuppliers(string pathDB)
        {
			if (String.IsNullOrWhiteSpace(pathDB) == true) return new Suppliers();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.Suppliers.ToList().Select(e => e.ToDomainObject());
                return Suppliers.FromEnumerable(domainObjects);
            }
        }

		public Suppliers GetSuppliers(SelectParams selectParams, string pathDB)
		{
			if (String.IsNullOrWhiteSpace(pathDB) == true) return new Suppliers();
			if (selectParams == null)
				return GetSuppliers(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Suppliers), db.Suppliers.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = Suppliers.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void ReCountShilfSum(SelectParams selectParams, string pathDB)
		{
			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Iturs iturs = iturRepository.GetIturs(selectParams, pathDB);
			//Dictionary<string, Itur> iturDictionary = iturRepository.GetIturDictionary(pathDB, true);
			long totalCount = 0;
			double total = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Shelf), db.Shelf.AsQueryable(), selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject()).ToList(); //  ToSimpleMakatDomainObject

				// ============ сводим - подгоняем
				// ============ это что насчитали 
				var domainIturs = from e in domainObjects
								  orderby e.IturCode, e.ShelfNum
								  group e by new
								  {
									  e.IturCode,
									  e.ShelfNum
								  } into g
								  select new
								  {
									  IturCode = g.Key.IturCode,
									  ShelfNum = g.Key.ShelfNum,
									  Width = g.Sum(x => x.Width),
								  };


				//Shelfs newShelfList = new Shelfs();

				foreach (Itur itur in iturs) //перемеряем по всем итурам 
				{
					double addWidth = (double)itur.Width;
					var domainIturShelves = domainIturs.Where(x => x.IturCode == itur.IturCode); // снятые измерения (суммы ширины)
					if (domainIturShelves == null) // вообще полки в итуре не мерили
					{
						for (int i = 1; i <= itur.ShelfInItur; i++) //проверяем по всем полкам в итурe
						{
							//Добавляем все полки что болжны быть
							Shelf entity = new Shelf();
							entity.ShelfCode = Guid.NewGuid().ToString();
							entity.IturCode = itur.IturCode;
							entity.SupplierCode = "1"; //??
							entity.Width = itur.Width;
							entity.Height = (double)itur.Height / (double)itur.ShelfInItur;
							entity.Area = (int)((entity.Height * entity.Width));
							entity.ShelfNum = i;
							entity.CreateDataTime = DateTime.Now;
							entity.ShelfPartCode = "counted";
							domainObjects.Add(entity);
						}
					}
					else  //(domainIturShelves != null) // что-то  намерили
					{ //проверямем и добавляем разницу
						for (int i = 1; i <= itur.ShelfInItur; i++) //проверяем по всем полкам в итурe
						{
							var fildShelf = domainIturShelves.FirstOrDefault(e => e.ShelfNum == i); // измерения для полки
							if (fildShelf != null) // нашли измерение для полки
							{
								addWidth = (double)itur.Width - fildShelf.Width; // если не 0 надо будет добавить
							}
							else
							{
								// не нашли измерений для полки береб всю ширину полки
								addWidth = itur.Width;
							}

							if (addWidth != 0)
							{

								Shelf entity = new Shelf();
								entity.ShelfCode = Guid.NewGuid().ToString();
								entity.IturCode = itur.IturCode;
								entity.SupplierCode = "1"; //??
								entity.Width = addWidth;
								entity.Height = (double)itur.Height / (double)itur.ShelfInItur;
								entity.Area = (int)((entity.Height * entity.Width));
								entity.ShelfNum = i;
								entity.CreateDataTime = DateTime.Now;
								entity.ShelfPartCode = "counted";
								domainObjects.Add(entity);
							}
						}
					}
				}
				// ======= 

				var domainObjectsSum = from e in domainObjects
									   orderby e.IturCode, e.SupplierCode
									   group e by new
									   {
										   e.IturCode,
										   e.SupplierCode
									   } into g
									   select new
									   {
										   IturCode = g.Key.IturCode,
										   SupplierCode = g.Key.SupplierCode,
										   Area = g.Sum(x => x.Area)
									   };

				var suppliers = from e in domainObjectsSum
								orderby e.SupplierCode
								group e by e.SupplierCode into g
								select new
								{
									SupplierCode = g.Key,
									Area = g.Sum(x => x.Area),
									Count = g.Count()
								};

				var sumtotal = suppliers.Sum(x => x.Area);

				foreach (var supplier in suppliers)
				{
					var entity = db.Suppliers.FirstOrDefault(e => e.SupplierCode.CompareTo(supplier.SupplierCode) == 0);
					if (entity == null) return;
					entity.Area = supplier.Area;
					entity.IturCount = supplier.Count;
					entity.PercentArea = (supplier.Area / sumtotal) * 100;
				}

				db.SaveChanges();
			}
		}		
	

		public void Delete(Supplier supplier, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, supplier.SupplierCode);
				if (entity == null) return;
				db.Suppliers.DeleteObject(entity);
				db.SaveChanges();
			}
		}

		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.Suppliers.ToList().ForEach(e => db.Suppliers.DeleteObject(e));
				db.SaveChanges();
			}
		}

		public void Insert(Supplier supplier, string pathDB)
		{
			if (supplier == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = supplier.ToEntity();
				db.Suppliers.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(Dictionary<string, Supplier> dictionarySupplier, string pathDB)
		{
			this.GetSupplierDictionary(pathDB, true);

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (KeyValuePair<string, Supplier> keyValuePair in dictionarySupplier)
				{
					if (this._supplierDictionary.ContainsKey(keyValuePair.Key) == false)
					{
						if (keyValuePair.Value != null)
						{
							var entity = keyValuePair.Value.ToEntity();
							db.Suppliers.AddObject(entity);
						}
					}
				}
				db.SaveChanges();
			}
		}

		public void Update(Supplier supplier, string pathDB)
		{
			if (supplier == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, supplier.SupplierCode);
				if (entity == null) return;
				entity.ApplyChanges(supplier);
				db.SaveChanges();
			}
		}

		public Supplier GetSupplierByName(string name, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByName(db, name);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public Supplier GetSupplierByCode(string code, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

        public int GetSuppliersTotal(string pathDB)
        {
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                return db.Suppliers.Count();
            }
        }

		public List<string> GetSupplierCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Suppliers.Select(e => e.SupplierCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetSupplierCodeList", exp);
				}
			}
			return ret;
		}

		public void RepairCodeFromDB(string pathDB)
		{
			IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
			List<string> supplierCodeListFromProduct = productRepository.GetSupplierCodeList(pathDB);			//из
			List<string> supplierCodeListFromSupplier = this.GetSupplierCodeList(pathDB); //в
			Dictionary<string, string> difference = new Dictionary<string, string>();

			foreach (var supplierCodeFromProduct in supplierCodeListFromProduct)			   //из
			{
				if (supplierCodeListFromSupplier.Contains(supplierCodeFromProduct) == false)		 //в
				{
					difference[supplierCodeFromProduct] = supplierCodeFromProduct;
				}
			}

			foreach (KeyValuePair<string, string> keyValuePair in difference)
			{
				if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
				{
					Model.Count4U.Supplier newSupplier = new Model.Count4U.Supplier();
					newSupplier.SupplierCode = keyValuePair.Value;
					newSupplier.Name = keyValuePair.Value;
					newSupplier.Description = "Repair from Product";
					this.Insert(newSupplier, pathDB);
				}
			}
		}

        #endregion

		#region Dictionary

		public Dictionary<string, Supplier> GetSupplierDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearSupplierDictionary();
				this.FillSupplierDictionary(pathDB);
			}
			return this._supplierDictionary;
		}

		public void ClearSupplierDictionary()
		{
			this._supplierDictionary.Clear();
			GC.Collect();
		}

		public void AddSupplierInDictionary(string code, Supplier location)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._supplierDictionary.ContainsKey(code) == false)
			{
				this._supplierDictionary.Add(code, location);
			}
		}

		public void RemoveSupplierFromDictionary(string code)
		{
			try
			{
				this._supplierDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistSupplierInDictionary(string code)
		{
			if (this._supplierDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public Supplier GetSupplierByCodeFromDictionary(string code)
		{
			if (this._supplierDictionary.ContainsKey(code) == true)
			{
				return this._supplierDictionary[code];
			}
			return null;
		}

		public void FillSupplierDictionary(string pathDB)
		{
			this.ClearSupplierDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Suppliers suppliers = this.GetSuppliers(pathDB);
					this._supplierDictionary = suppliers.Select(e => e).Distinct().ToDictionary(k => k.SupplierCode); 
					
				}
				catch { }
			}
		}
         #endregion

		#region private

		private App_Data.Supplier GetEntityByCode(App_Data.Count4UDB db, string supplierCode)
		{
			var entity = db.Suppliers.FirstOrDefault(e => e.SupplierCode.CompareTo(supplierCode) == 0);
			return entity;
		}


		private App_Data.Supplier GetEntityByName(App_Data.Count4UDB db, string name)
		{
			var entity = db.Suppliers.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
			return entity;
		}

		#endregion

        public bool IsAnyInDb(string pathDB)
        {
            using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                try
                {
					long n = db.Suppliers.LongCount();
					if (n > 0) return true;
					else return false;
                }
                catch { }
            }

            return false;
        }
	}
}
