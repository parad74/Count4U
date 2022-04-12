using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Count4U.Localization;

namespace Count4U.Model.Count4U
{
    public class ProductEFRepository : BaseEFRepository, IProductRepository
    {

		private Dictionary<string, ProductSimple> _productSimpleDictionary;
	//	private IContextCBIRepository _contextCBIRepository;
		private IInventorConfigRepository _inventorConfigRepository;
		//private List<string> this._productMakatList; 

		public ProductEFRepository(/*IContextCBIRepository contextCBIRepository,*/ IInventorConfigRepository inventorConfigRepository, IConnectionDB connection)
			: base(connection)
        {
			this._productSimpleDictionary = new Dictionary<string, ProductSimple>();
		//	this._contextCBIRepository = contextCBIRepository;
			this._inventorConfigRepository = inventorConfigRepository;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IProductRepository Members

		public Products GetProducts(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.Products.ToList().Select(e => e.ToDomainObject());
                return Products.FromEnumerable(domainObjects);
            }
        }


		public Products GetProductsMakatOnly(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string makatType = TypeMakatEnum.M.ToString();
				var domainObjects = db.Products.Where(e => e.TypeCode == makatType).ToList().Select(e => e.ToDomainObject());
				return Products.FromEnumerable(domainObjects);
			}
		}

		public Products GetProductsBarcodeOnly(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string makatType = TypeMakatEnum.B.ToString();
				var domainObjects = db.Products.Where(e => e.TypeCode == makatType).ToList().Select(e => e.ToDomainObject());
				return Products.FromEnumerable(domainObjects);
			}
		}


		public List<App_Data.Product> GetApp_DataProducts(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				List<App_Data.Product> domainObjects = db.Products.ToList().Select(e => e.CopyEntity()).ToList();
				return domainObjects;
            }
        }

		public IEnumerable<ProductSimple> GetProductSimples(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.Products.ToList().OrderBy(x => x.Makat).Select(e => e.ToSimpleDomainObject());
				return domainObjects;
			}
		} 

		public Products GetProducts(SelectParams selectParams, string pathDB)
        {
            if (selectParams == null)
                return GetProducts(pathDB);

            long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
//			    var x = from p1 in db.Products
//                        from p2 in db.Products
//			            //join p2 in db.Products on p1.ParentMakat equals p2.Makat
//			            where
//                            p1.ParentMakat == p2.Makat &&
//			                p1.Makat.Contains("11") &&
//			                p1.TypeCode == "B" &&
//			                p2.TypeCode == "M"
//			            select p2;
			    
                
                // Получение сущностей и общего количества из БД.
                // Getting entities and total count from database.
                var entities = GetEntities(db, AsQueryable(db.Products), db.Products.AsQueryable(), 
					selectParams, out totalCount);

                // Преобразование сущностей в объекты предметной области.
                // Converting entites to domain objects.
                var domainObjects = entities.Select(e => e.ToDomainObject());

                // Возврат результата.
                // Returning result.
                var result = Products.FromEnumerable(domainObjects);
                result.TotalCount = totalCount;
                return result;
            }
        }

	
		public Products GetProductsByInputTypeCode(string inputTypeCode, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.Products.Where(e => e.InputTypeCode.CompareTo(inputTypeCode) == 0)
                                              .ToList().Select(e => e.ToDomainObject());
                return Products.FromEnumerable(domainObjects);
            }
        }

		public Products GetProductsByBarcode(string barcode, string pathDB)
        {
			//using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			//{
			//    var domainObjects = (from b in db.Barcodes
			//                         join p in db.Products on b.ProductID equals p.ID
			//                         where b.Value.CompareTo(barcodeValue) == 0
			//                         select p).ToList().Select(e => e.ToDomainObject());
			//    return Products.FromEnumerable(domainObjects);
			//}
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.Products.Where(e => e.Barcode.CompareTo(barcode) == 0)
											  .ToList().Select(e => e.ToDomainObject());
				return Products.FromEnumerable(domainObjects);
			}
        }

		//public Products GetProductsByBarcode(Barcode barcode, string pathDB)
		//{
		//    return GetProductsByBarcodeValue(barcode.Value, pathDB);
		//}

	
		public Products GetProductsBySupplierCode(string supplierCode, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.Products.Where(e => e.SupplierCode.CompareTo(supplierCode) == 0)
                                              .ToList().Select(e => e.ToDomainObject());
                return Products.FromEnumerable(domainObjects);
            }
        }

		public Products GetProductsBySectionCode(string sectionCode, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.Products.Where(e => e.SectionCode.CompareTo(sectionCode) == 0)
                                              .ToList().Select(e => e.ToDomainObject());
                return Products.FromEnumerable(domainObjects);
            }
        }

		public Product GetProductByMakat(string makat, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByMakat(db, makat);
				if (entity == null) return null;
				return entity.ToDomainObject();
            }
        }

		public Dictionary<string, ProductSimple> GetProductQuantityERPDictionary(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string barcodeType = TypeMakatEnum.B.ToString();
				var domainObjects = db.Products.Where(e => e.IsUpdateERP == true && e.TypeCode == barcodeType).ToList().Select(e => e.ToDomainObject());
				var domainObjectsSum = from e in domainObjects
														orderby e.ParentMakat
														group e by new
														{
															e.ParentMakat,
															//e.Makat
														} into g
														select new ProductSimple
														{
															Makat = g.Key.ParentMakat,
															BalanceQuantityERP = g.Sum(x => x.BalanceQuantityERP)
														};
				Dictionary<string, ProductSimple> productBalanceQuantityERPDictionary = domainObjectsSum.Select(e => e).Distinct().ToDictionary(k => k.Makat);
				return productBalanceQuantityERPDictionary;
			}
		}

		public Dictionary<string, ProductSimple> GetSumProductQuantityERPByMakatDictionary(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string barcodeType = TypeMakatEnum.B.ToString();
				var domainObjects = db.Products.Where(e=> e.TypeCode == barcodeType).ToList().Select(e => e.ToDomainObject());
				var domainObjectsSum = from e in domainObjects
									   orderby e.ParentMakat
									   group e by new
									   {
										   e.ParentMakat,
										   //e.Makat
									   } into g
									   select new ProductSimple
									   {
										   Makat = g.Key.ParentMakat,
										   BalanceQuantityERP = g.Sum(x => x.BalanceQuantityERP)
									   };
				Dictionary<string, ProductSimple> productBalanceQuantityERPDictionary = domainObjectsSum.Select(e => e).Distinct().ToDictionary(k => k.Makat);
				return productBalanceQuantityERPDictionary;
			}
		}

		public Product GetProductByBarcode(string barcode, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByBarcode(db, barcode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public Products GetProductsByTypeCode(string type, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.Products.Where(e => e.TypeCode.CompareTo(type) == 0)
                                              .ToList().Select(e => e.ToDomainObject());
                return Products.FromEnumerable(domainObjects);
            }
        }

		public DateTime GetMaxModifyDate( string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var MaxModifyDate = db.Products.Max(e => e.ModifyDate);
				return Convert.ToDateTime(MaxModifyDate);
			}
		}

        

		public void Delete(string makat, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByMakat(db, makat);
				if (entity == null) return;
                db.Products.DeleteObject(entity);
				this.SetLastUpdatedCatalog(pathDB);
                db.SaveChanges();

				//this.RemoveProductMakat(makat);
            }
        }

		public void Insert(Product product, string pathDB)
        {
			if (product == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = product.ToEntity();
                db.Products.AddObject(entity);
				this.SetLastUpdatedCatalog(pathDB);
                db.SaveChanges();

				//this.AddProductMakat(product.Makat);
            }
        }


		public void Copy(List<App_Data.Product> products, string toPathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(toPathDB)))
			{
				int k = 0;
				foreach (App_Data.Product product in products)
				{
					k++;
					//products.ForEach(e => db.Products.AddObject(e));
					db.Products.AddObject(product);
					if (k % 200 == 0)
					{
						db.SaveChanges();
					}
				}
				this.SetLastUpdatedCatalog(toPathDB);
				db.SaveChanges();
			}
		}

		public void DeleteAll(Products products, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var selectedMakats = products.Select(e => e.Makat).ToList();
				var entities = db.Products.Where(e => selectedMakats.Contains(e.Makat)).ToList();
				if (entities == null) return;
                entities.ForEach(e => db.Products.DeleteObject(e));
				this.SetLastUpdatedCatalog(pathDB);
                db.SaveChanges();

				//this.ClearProductMakats();
            }
        }

		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.Products.ToList().ForEach(e => db.Products.DeleteObject(e));
				this.SetLastUpdatedCatalog(pathDB);
				db.SaveChanges();
			}
		}

		public void DeleteAllIsUpdated(string pathDB, bool isUpdate = true)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{			
				string barcodeType = TypeMakatEnum.B.ToString();
			//	var domainObjects = db.Products.Where(e => e.IsUpdateERP == true && e.TypeCode == barcodeType).ToList().Select(e => e.ToDomainObject());
				try
				{
					var entities = db.Products.Where(e => e.IsUpdateERP == true && e.TypeCode == barcodeType).ToList();
					if (entities == null) return;
					entities.ForEach(e => db.Products.DeleteObject(e));
					//foreach (var entity in entities)
					//{
					//	db.Products.DeleteObject(entity);
					//}
				}
				catch (Exception ex)
				{
				}
				//var entities = entities1.ToList();
				//entities.ForEach(e => db.Products.DeleteObject(e));
				this.SetLastUpdatedCatalog(pathDB);
				db.SaveChanges();
			}
		}

		public void DeleteAllMakat(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string makatType = TypeMakatEnum.M.ToString();
				db.Products.Where(e => e.TypeCode.CompareTo(makatType) == 0).ToList().ForEach(e => db.Products.DeleteObject(e));
				this.SetLastUpdatedCatalog(pathDB);
				db.SaveChanges();
			}
		}
		

		public void Insert(Products products, string pathDB)
        {
			foreach (var e in products)
			{
				this.Insert(e, pathDB);
			}
        }

		public void Update(Product product, string pathDB)
        {
			if (product == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByMakat(db, product.Makat);
				if (entity == null) return;
                entity.ApplyChanges(product);
				this.SetLastUpdatedCatalog(pathDB);
                db.SaveChanges();
            }
        }

     	public Products GetProductsByName(string name, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.Products.Where(e => e.Name.CompareTo(name) == 0)
                                              .ToList().Select(e => e.ToDomainObject());
                return Products.FromEnumerable(domainObjects);
            }
        }

	
		public long CountProduct(string pathDB)
        {
			long count = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					count = db.Products.LongCount();
				}
				catch { }
				return count;
            }
        }

		public long CountMakat(string pathDB)
		{
			long count = 0;
			string typeMakat = TypeMakatEnum.M.ToString();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					count = db.Products.Where(e => e.TypeCode.CompareTo(typeMakat) == 0).LongCount();
				}
				catch { }
				return count;
			}
		}

		public long CountBarcode(string pathDB)
		{
			long count = 0;
			string typeBarcode = TypeMakatEnum.B.ToString();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					count = db.Products.Where(e => e.TypeCode.CompareTo(typeBarcode) == 0).LongCount();
				}
				catch { }
				return count;
			}
		}

	

		public Dictionary<string, ProductSimple> GetProductSimpleDictionary(string pathDB,
		bool refill = false, string typeMakat = "M")
		{
			if (refill == true)
			{
				this.ClearProductSimpleDictionary();
				this.FillProductSimpleDictionary(pathDB, typeMakat, out this._productSimpleDictionary);
			}
			return this._productSimpleDictionary;
		}


		public void FillProductSimpleDictionary(string pathDB, string typeMakat, out Dictionary<string, ProductSimple> productSimpleDictionary)
		{
			productSimpleDictionary = new Dictionary<string, ProductSimple>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//this._productMakatDictionary = 
			//string typeMakat = TypeMakatEnum.M.ToString();
				try
				{
					List<ProductSimple> productSimples = db.Products.
						Where(e => e.TypeCode == typeMakat).ToList().Select(e =>
						new ProductSimple
						{
							ParentMakat = e.ParentMakat,
							Name = e.Name,
							Makat = e.Makat,
							Code = e.Code,
							TypeCode = e.TypeCode,
							MakatOriginal = e.MakatOriginal,
							PriceBuy = e.PriceBuy,
							PriceSale = e.PriceSale,
							PriceExtra = Convert.ToDouble(e.PriceExtra),
							PriceString = e.PriceString,
							BalanceQuantityERP = Convert.ToDouble(e.BalanceQuantityERP),
							IsUpdateERP = Convert.ToBoolean(e.IsUpdateERP),
							SectionCode = e.SectionCode,
							SupplierCode = e.SupplierCode,
							UnitTypeCode = e.UnitTypeCode,
							FromCatalogType = (int)e.FromCatalogType,
							FromCatalogTypeWithoutInventProduct = (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct,
							BalanceQuantityPartialERP = (int)e.BalanceQuantityPartialERP,
							CountInParentPack = (int)e.CountInParentPack, 
							Family = e.Family,
							FamilyCode = e.FamilyCode,
							IturCodeExpected = e.IturCodeExpected,
							Description = e.Description
						}).ToList();

					productSimpleDictionary = productSimples.Select(e => e).Distinct().ToDictionary(k => k.Makat);
				}
				catch { 	}
			}
		}

		public Dictionary<string, Product> GetProductDictionary(string pathDB)
		{
			Dictionary<string, Product>  productSimpleDictionary = new Dictionary<string, Product>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//this._productMakatDictionary = 
				//string typeMakat = TypeMakatEnum.M.ToString();
				try
				{
					var products = db.Products.ToList().Select(e => e.ToDomainObject());
					productSimpleDictionary = products.Distinct().ToDictionary(k => k.Makat);
				}
				catch { }
			}
			return productSimpleDictionary;
		}

		public Dictionary<string, ProductTagSimple> GetProductTagDictionary(string pathDB)
		{
			Dictionary<string, ProductTagSimple> productSimpleDictionary = new Dictionary<string, ProductTagSimple>();
				string typeMakat = TypeMakatEnum.M.ToString();
				using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var productSimples = db.Products.
						Where(e => e.TypeCode == typeMakat).ToList().Select(e =>
						new ProductTagSimple
						{
							Makat = e.Makat,
							Tag = e.Tag, 
							Name = e.Name, 
							TypeCode = e.TypeCode
						}).ToList();
					productSimpleDictionary = productSimples.Distinct().ToDictionary(k => k.Makat);
				}
				catch { }
			}
			return productSimpleDictionary;
		}

		//public Dictionary<string, ProductSimple> GetProductSimpleUpdateOnlyDictionary(string pathDB,
		//bool refill = false)
		//{
		//    if (refill == true)
		//    {
		//        this.ClearProductSimpleDictionary();
		//        this.FillProductSimpleUpdateOnlyDictionary(pathDB);
		//    }
		//    return this._productSimpleDictionary;
		//}

		public void ClearProductSimpleDictionary()
		{
			this._productSimpleDictionary.Clear();
			GC.Collect();
		}


	

		public void FillProductSimpleUpdateOnlyDictionary(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//this._productMakatDictionary = 
				string typeMakat = TypeMakatEnum.M.ToString();
				try
				{
					List<ProductSimple> productSimples = db.Products.
						Where(e => e.TypeCode == typeMakat && e.IsUpdateERP == true).ToList().Select(e =>
						new ProductSimple
						{
							ParentMakat = e.ParentMakat,
							Name = e.Name,
							Makat = e.Makat,
							TypeCode = e.TypeCode,
							MakatOriginal = e.MakatOriginal,
							PriceBuy = e.PriceBuy,
							PriceSale = e.PriceSale,
							PriceExtra = Convert.ToDouble(e.PriceExtra),
							PriceString = e.PriceString,
							BalanceQuantityERP = Convert.ToDouble(e.BalanceQuantityERP),
							IsUpdateERP = Convert.ToBoolean(e.IsUpdateERP),
							SectionCode = e.SectionCode,
							SupplierCode = e.SupplierCode,
							UnitTypeCode = e.UnitTypeCode,
							FromCatalogType = (int)e.FromCatalogType,
							FromCatalogTypeWithoutInventProduct = (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct
						}).ToList();

					this._productSimpleDictionary = productSimples.Select(e => e).Distinct().ToDictionary(k => k.Makat);
				}
				catch { }
			}
		}


		public Dictionary<string, Product> GetProduct_IturCodeErpDictionary(string pathDB)
		{
			Dictionary<string, Product> productDictionary = new Dictionary<string, Product>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//this._productMakatDictionary = 
				string typeMakat = TypeMakatEnum.B.ToString();
				try
				{
					List<Product> productSimples = db.Products.
						Where(e => e.TypeCode == typeMakat && e.IsUpdateERP == true).ToList().Select(e =>
						new Product
						{
							ParentMakat = e.ParentMakat,  //Makat
							Makat = e.Makat,       //Makat##IturCodeERP
							TypeCode = e.TypeCode, 
							MakatOriginal = e.MakatOriginal,
							BalanceQuantityERP = Convert.ToDouble(e.BalanceQuantityERP),
							IsUpdateERP = Convert.ToBoolean(e.IsUpdateERP),
							FromCatalogType = (int)e.FromCatalogType, 
							Tag = e.Tag, //IturCode
							MakatERP = e.MakatERP //IturCodeERP
						}).ToList();

					productDictionary = productSimples.Select(e => e).Distinct().ToDictionary(k => k.Makat);
				}
				catch { }
			}
			return productDictionary;
		}


		public void SetLastUpdatedCatalog(string pathDB)
		{
			if (string.IsNullOrWhiteSpace(pathDB) == true) return;
			string objectCode = _inventorConfigRepository.GetObjectCodeFromRelativePathDB(pathDB);
			if (pathDB.ToLower().Contains("inventor"))
			{
				using (App_Data.AuditDB auditdc = new App_Data.AuditDB(this.AuditConnectionString()))
				{
					var entity = auditdc.Inventor.FirstOrDefault(e => e.Code.CompareTo(objectCode) == 0);
					if (entity == null) return;
					//inventor.InventorDate = DateTime.Now; //test - убрать
					entity.LastUpdatedCatalog = DateTime.Now;
					auditdc.SaveChanges();
				}
			}

			else if (pathDB.ToLower().Contains("customer"))
			{
				using (App_Data.MainDB maindc = new App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entity = maindc.Customer.FirstOrDefault(e => e.Code.CompareTo(objectCode) == 0);
					if (entity == null) return;
					entity.LastUpdatedCatalog = DateTime.Now;
					maindc.SaveChanges();
				}
			}
			else if (pathDB.ToLower().Contains("branch"))
			{
				using (App_Data.MainDB maindc = new App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entity = maindc.Branch.FirstOrDefault(e => e.Code.CompareTo(objectCode) == 0);
					if (entity == null) return;
					entity.LastUpdatedCatalog = DateTime.Now;
					maindc.SaveChanges();
				}
			}

		}
        #endregion

        #region private


		

        private App_Data.Product GetEntityByCode(App_Data.Count4UDB db, string code)
        {
			var entity = db.Products.FirstOrDefault(e => e.Code == code);
            return entity;
        }

		private App_Data.Product GetEntityByMakat(App_Data.Count4UDB db, string makat)
		{
			var entity = db.Products.FirstOrDefault(e => e.Makat == makat);
			return entity;
		}

		private App_Data.Product GetEntityByBarcode(App_Data.Count4UDB db, string barcode)
		{
			var entity = db.Products.FirstOrDefault(e => e.Barcode == barcode);
			return entity;
		}

		public bool IsAnyProductInDb(string pathDB)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				long n = db.Products.LongCount();
				if (n > 0) return true;
				else return false;
			}
		}

		public List<string> GetSectionCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Products.Select(e => e.SectionCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetSectionCodeList", exp);
				}
			}
			return ret;
		}

		public List<string> GetSupplierCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Products.Select(e => e.SupplierCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetSupplierCodeList", exp);
				}
			}
			return ret;
		}

		public List<string> GetFamilyCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Family.Select(e => e.FamilyCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetFamilyCodeList", exp);
				}
			}
			return ret;
		}
        #endregion

		#region IProductRepository Members


		public Product Clone(Product product)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
