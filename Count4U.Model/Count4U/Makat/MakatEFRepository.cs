using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Collections;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Model.Count4U
{
	public class MakatEFRepository : BaseEFRepository, IMakatRepository
	{
		//private List<string> this._productMakatList;
		private IServiceLocator _serviceLocator;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private Dictionary<string, string> _productMakatBarcodesDictionary;
		private Dictionary<string, ProductMakat> _productBarcodeDictionary;
		Dictionary<string, ProductMakat> _productUnitTypeDictionary;
		private Hashtable _productBarcodeHashtable;
		private List<ProductMakat> _productMakatList;
		private readonly ILog _log;
		private bool _productMakatDictionaryFill;
		//private Dictionary<string, ProductMakat> _parentMakatProductDictionary;

		public MakatEFRepository(ILog log, IServiceLocator serviceLocator, IConnectionDB connection)
			: base(connection)
		{
			//this._productMakatList = new List<string>();
			this._serviceLocator = serviceLocator;
			this._productMakatBarcodesDictionary = new Dictionary<string, string>();
			int productMakatBarcodesDictionaryCapacity = 100001;
			bool ret = int.TryParse(base.ProductMakatBarcodesDictionaryCapacity, out productMakatBarcodesDictionaryCapacity);
			this._productBarcodeDictionary = new Dictionary<string, ProductMakat>(productMakatBarcodesDictionaryCapacity);
			this._productMakatDictionaryFill = false;
			this._productUnitTypeDictionary = new Dictionary<string, ProductMakat>();
			//this._productBarcodeHashtable = new Hashtable(5000000, (float).4); //capacity , loadfactor
			this._productMakatList = new List<ProductMakat>();
			if (log == null) throw new ArgumentNullException("log");
			this._log = log;
		}

		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}
		#region  ProductBarcodeDictionary

		public bool ProductMakatDictionaryFill
		{
			get { return this._productMakatDictionaryFill; }
			set { this._productMakatDictionaryFill = value; }
		}
 
		public Dictionary<string, ProductMakat> ProductMakatDictionaryRefill(string pathDB , bool refill = false)
		{
			//if (this.ProductMakatDictionaryFill == false)
			if (refill == true)
			{
				//ILog log = _serviceLocator.GetInstance<ILog>();
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				_logger.Error(String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "ProductMakatDictionary"));
				this.ClearProductBarcodeDictionary();
				this.FillProductBarcodeDictionary(pathDB);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill [{0}]"
				_logger.Error(String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "ProductMakatDictionary"));
				//this.ProductMakatDictionaryFill = true;
			}
			return this._productBarcodeDictionary;
		}


		public Dictionary<string, ProductMakat> GetProductBarcodeDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				this.ClearProductBarcodeDictionary();
				this.FillProductBarcodeDictionary(pathDB);
			}
			return this._productBarcodeDictionary;
		}

		public void ClearProductBarcodeDictionary()
		{
			this._productBarcodeDictionary.Clear();
			GC.Collect();
		}



		//public Dictionary<string, ProductMakat> GetParentMakatProductDictionary(string pathDB,
		//bool refill = false)
		//{
		//    if (refill == true)
		//    {
		//        this.ClearParentMakatProductDictionary();
		//        this.FillParentMakatProductDictionary(pathDB);
		//    }
		//    return this._parentMakatProductDictionary;
		//}

		//public void ClearParentMakatProductDictionary()
		//{
		//    this._parentMakatProductDictionary.Clear();
		//    GC.Collect();
		//}

		public List<ProductMakat> GetBarcodeProducts(string parentMakat, List<ProductMakat> products)
		{
			//try
			//{
				return products.Where(e => e.ParentMakat.CompareTo(parentMakat) == 0).Select(e => e).ToList();
			//}
			//catch { return null; }
		}

		public void AddProductBarcode(string makat, ProductMakat productMakat)
		{
			if (string.IsNullOrWhiteSpace(makat)) return;
			if (this._productBarcodeDictionary.ContainsKey(makat) == false)
			{
				this._productBarcodeDictionary.Add(makat, productMakat);
			}
		}

		public bool IsExistProductBarcode(string makat, bool top = false)
		{
			if (top == false)
			{
				if (this._productBarcodeDictionary.ContainsKey(makat) == true)
				{
					while (string.IsNullOrWhiteSpace(this._productBarcodeDictionary[makat].ParentMakat) == false)
					{
						makat = this._productBarcodeDictionary[makat].ParentMakat;
						if (this._productBarcodeDictionary.ContainsKey(makat) == false)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
			else
			{
				if (this._productBarcodeDictionary.ContainsKey(makat) == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}


		public ProductMakat GetProductByBarcode(string makat, bool top = false)
		{
			if (top == false)
			{
				if (this._productBarcodeDictionary.ContainsKey(makat) == true)
				{
					while (string.IsNullOrWhiteSpace(this._productBarcodeDictionary[makat].ParentMakat) == false)
					{
						makat = this._productBarcodeDictionary[makat].ParentMakat;
						if (this._productBarcodeDictionary.ContainsKey(makat) == false)
						{
							return null;
						}
					}
					return this._productBarcodeDictionary[makat];
				}
				return null;
			}
			else
			{
				if (this._productBarcodeDictionary.ContainsKey(makat) == true)
				{
					return this._productBarcodeDictionary[makat];
				}
				return null;
			}

		}

		public string GetProductNameByBarcode(string makat)
		{
			if (this._productBarcodeDictionary.ContainsKey(makat) == true)
			{
				while (string.IsNullOrWhiteSpace(this._productBarcodeDictionary[makat].ParentMakat) == false)
				{
					makat = this._productBarcodeDictionary[makat].ParentMakat;
					if (this._productBarcodeDictionary.ContainsKey(makat) == false)
					{
						return "";
					}
				}
				return this._productBarcodeDictionary[makat].Name;
			}
			return "";
		}

		public List<ProductMakat> GetProductMakatList(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					//this._productMakatDictionary = 
					try
					{
						this._productMakatList.Clear();
						var makatList = db.Products.Select(e =>
							new ProductMakat
							{
								ParentMakat = e.ParentMakat,
								Name = e.Name,
								Makat = e.Makat,
								TypeCode = e.TypeCode,
								MakatOriginal = e.MakatOriginal
							}).ToList();
						this._productMakatList = makatList.Distinct().ToList();

					}
					catch { }
				}
			}
			return this._productMakatList;
		}

		// Relation 1:n	   [Makat:Barcodes] 
		// key - Makat
		// value - Barcode1, BakatBarcode2
		public Dictionary<string, string> GetProductMakatBarcodesDictionary(string pathDB,
			 bool refill = false)
		{
			if (refill == true)
			{
				using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
				{
					string typeMakat = TypeMakatEnum.M.ToString();
					string typeBarcode = TypeMakatEnum.B.ToString();

					// List with type'B' - Distinct  by ParentMakat
					//Список всех родительских макатов (Distinct), для баркодов
					List<string> barcodeList = db.Products.Where(e => e.TypeCode.CompareTo(typeBarcode) == 0 && e.IsUpdateERP == false).
						Select(e => e.ParentMakat).ToList();

					//Dictionary  key -  makat		 ; string										  //1
					//					value - barcodes ; string = barcode1, barcode2, barcode3	//n
					// Relation 1:n
					this._productMakatBarcodesDictionary.Clear();
					this._productMakatBarcodesDictionary = barcodeList.Select(e => e).Distinct().ToDictionary(k => k);

					//barcodeList.Clear();

					//Clear value
					foreach (var barcode in barcodeList)
					{
						this._productMakatBarcodesDictionary[barcode] = "";
					}


					//if (openWith.TryGetValue("tif", out value))
					//{
					//    Console.WriteLine("For key = \"tif\", value = {0}.", value);
					//}
					//else
					//{
					//    Console.WriteLine("Key = \"tif\" is not found.");
					//}


					//foreach (var keyValuePair in this._productMakatBarcodesDictionary)
					//{
					//    this._productMakatBarcodesDictionary[keyValuePair.Key] = "";
					//}

					// List ProductMakat with type'B' - Distinct  by all field
					var productMakatList = db.Products.Where(e => e.TypeCode.CompareTo(typeBarcode) == 0).
						Select(e => new ProductMakat
					{
						ParentMakat = e.ParentMakat,
						Name = e.Name,
						Makat = e.Makat,
						TypeCode = e.TypeCode,
						MakatOriginal = e.MakatOriginal
					}).ToList();

					List<ProductMakat> productMakats = productMakatList.Distinct().ToList();


					//productMakat.ParentMakat    - that is  Relation on Makat	
					// => key in _productMakatBarcodesDictionary
					//productMakat.Makat    - that is Barcode  - add in ListBarcode() 
					// => in value in _productMakatBarcodesDictionary
					foreach (ProductMakat productMakat in productMakats)
					{
						//key - B
						if (this._productMakatBarcodesDictionary.ContainsKey(productMakat.ParentMakat) == true)
						{
							string val = this._productMakatBarcodesDictionary[productMakat.ParentMakat];
							if (string.IsNullOrWhiteSpace(val) == false)
							{
								val = val + "," + productMakat.Makat;
							}
							else
							{
								val = productMakat.Makat;
							}
							this._productMakatBarcodesDictionary[productMakat.ParentMakat] = val;
						}
					}
				}
			}
			return this._productMakatBarcodesDictionary;
		}

		public void FillProductBarcodeDictionary(string pathDB)
		{
			_log.Add(MessageTypeEnum.Trace, "Start FillProductBarcodeDictionary before connection at DB");

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//this._productMakatDictionary = 
				try
				{
					_log.Add(MessageTypeEnum.Trace, "Start FillProductBarcodeDictionary arter connection at DB");

					List<ProductMakat> productMakats = db.Products.Select(e =>
						new ProductMakat
						{
							ParentMakat = e.ParentMakat,
							Name = e.Name,
							Makat = e.Makat,
							TypeCode = e.TypeCode,
							MakatOriginal = e.MakatOriginal, 
							UnitTypeCode = e.UnitTypeCode,
							BalanceQuantityERP = e.BalanceQuantityERP,
							SectionCode = e.SectionCode,
							FamilyCode = e.FamilyCode, 
							SupplierCode = e.SupplierCode,
							SubSectionCode = e.SubSectionCode,
						//	IsUpdateERP = e.IsUpdateERP
						}).ToList();

					_log.Add(MessageTypeEnum.Trace, "FillProductBarcodeDictionary - Get productMakat DistinctList from DB");

					this._productBarcodeDictionary = productMakats.Select(e => e).Distinct().ToDictionary(k => k.Makat);
				
					_log.Add(MessageTypeEnum.Trace, "FillProductBarcodeDictionary - Create ProductBarcodeDictionary from ProductMakatDistinctList ");

 				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}
			}
		}


		public Dictionary<string, ProductSimple> FillProductBarcodeExcludeItursDictionary(List<string> iturCodes, string pathDB)
		{
			Dictionary<string, ProductSimple> otherdictionry = new Dictionary<string, ProductSimple>();

			_log.Add(MessageTypeEnum.Trace, "Start FillProductBarcodeDictionary before connection at DB");

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//this._productMakatDictionary = 
				try
				{
					_log.Add(MessageTypeEnum.Trace, "Start FillProductBarcodeDictionary arter connection at DB");

					List<App_Data.InventProduct> entities = new List<App_Data.InventProduct>();
					entities = AsQueryable(db.InventProducts).Select(e => e).ToList();
		
					var inventProducts = from e in entities
										   orderby e.Makat, e.IturCode
										   group e by new
										   {
											   e.Makat,
											   e.IturCode				//barcode from PDA
										   } into g
										 select new ProductSimple
										   {
											   Makat = g.Key.Makat,
											   ParentMakat = g.Key.IturCode,							//IturCode
											   BalanceQuantityERP = g.Sum(x => x.QuantityEdit),	   //QuantityEdit
										   };

					var inventProductsNot0 = inventProducts.Where(e => e.BalanceQuantityERP > 0).ToList();

					List<ProductSimple> resultInventProduct = new List<ProductSimple>();
					foreach (var inventProductnot0 in inventProductsNot0)
					{
						if (iturCodes.Contains(inventProductnot0.ParentMakat) == false)
						{
							resultInventProduct.Add(inventProductnot0);
						}
					}

					try
					{
						otherdictionry = resultInventProduct.Select(e => e).Distinct().ToDictionary(k => k.Makat);
					}
					catch { }


					//foreach (var productMakat in inventProducts)
					//{
					//	string iturCode =  productMakat.ParentMakat;
					//	double 	quantityEdit = 0;
					//	string makat = 	productMakat.Makat;
					//	try{
					//		quantityEdit = Convert.ToDouble (productMakat.BalanceQuantityERP);
					//	}catch{}

					//	if (iturCodes.Contains(iturCode) == false 
					//		&& quantityEdit > 0)
					//	{
					//		otherdictionry[makat] = quantityEdit;
					//	}
					//}

					_log.Add(MessageTypeEnum.Trace, "FillProductBarcodeExcludeItursDictionary - Get productMakat DistinctList from DB");
				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}
			}
			return otherdictionry;
		}


		public Dictionary<string, ProductSimple> GetMakatDictionaryFromInventProduct(string pathDB)
		{
			Dictionary<string, ProductSimple> otherdictionry = new Dictionary<string, ProductSimple>();

			_log.Add(MessageTypeEnum.Trace, "Start FillProductBarcodeDictionary before connection at DB");

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				//this._productMakatDictionary = 
				try
				{
					_log.Add(MessageTypeEnum.Trace, "Start FillProductBarcodeDictionary arter connection at DB");

					List<App_Data.InventProduct> entities = new List<App_Data.InventProduct>();
					entities = AsQueryable(db.InventProducts).Select(e => e).ToList();

					var inventProducts = from e in entities
										 orderby e.Makat
										 group e by new
										 {
											 e.Makat,
										 } into g
										 select new ProductSimple
										 {
											 Makat = g.Key.Makat,
											 BalanceQuantityERP = g.Sum(x => x.QuantityEdit),	   //QuantityEdit
										 };

					var resultInventProduct = inventProducts.Where(e => e.BalanceQuantityERP > 0).ToList();

					try
					{
						otherdictionry = resultInventProduct.Select(e => e).Distinct().ToDictionary(k => k.Makat);
					}
					catch { }


					_log.Add(MessageTypeEnum.Trace, "GetMakatDictionaryFromInventProduct - Get productMakat Distinct from [InventProduct]");
				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}
			}
			return otherdictionry;
		}

		public Dictionary<string, ProductMakat> GetProductUnitTypeDictionary(string pathDB, bool refill = false)
		{
			//Dictionary<string, ProductMakat> _productUnitTypeDictionary = new Dictionary<string,ProductMakat>();
			if (refill == false) return this._productUnitTypeDictionary;
			else this._productUnitTypeDictionary.Clear();
			string typeMakat = TypeMakatEnum.M.ToString();
					
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					List<App_Data.Product> entities = new List<App_Data.Product>();
					entities = AsQueryable(db.Products).Where(x => x.TypeCode == typeMakat).Select(e => e).ToList();

		
					List<ProductMakat> productMakats = entities.Select(e =>
						new ProductMakat
						{
							ParentMakat = e.ParentMakat,
							Name = e.Name,
							Makat = e.Makat,
							TypeCode = e.TypeCode,
							MakatOriginal = e.MakatOriginal,
							UnitTypeCode = e.UnitTypeCode,
							SectionCode = e.SectionCode,
                            FamilyCode = e.FamilyCode,
							SupplierCode = e.SupplierCode,
							SubSectionCode = e.SubSectionCode
						}).ToList();

					this._productUnitTypeDictionary = productMakats.Select(e => e).Distinct().ToDictionary(k => k.Makat);
				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}

				return _productUnitTypeDictionary;
			}
		}

		#endregion

		
	}


}
