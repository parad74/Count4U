using System;
using System.Linq;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.Mapping;

namespace Count4U.Model.Count4U
{
    public class ProductRepository : IProductRepository
	{
        private Products _productList;

        #region IProductRepository Members

		public Products GetProducts(string pathDB)
        {
            if (this._productList == null)
            {
                this._productList = new Products {
                    new Product() { ID = 1, Barcode = "Barcode1", Name = "Product1" },
                    new Product() { ID = 2, Barcode = "Barcode2", Name = "Product2" },
                    new Product() { ID = 3, Barcode = "Barcode3", Name = "Product3" },
                    new Product() { ID = 4, Barcode = "Barcode4", Name = "Product4" },
                    new Product() { ID = 5, Barcode = "Barcode5", Name = "Product5" },
                    new Product() { ID = 6, Barcode = "Barcode6", Name = "Product6" },
                };
            }
            return this._productList;
        }

		public Products GetProducts(SelectParams selectParams, string pathDB)
        {
            throw new NotImplementedException();
        }

		public Products GetProductsByInputTypeCode(string inputTypeCode, string pathDB)
        {
			var domainObjects = this.GetProducts(pathDB).Where(e => e.InputTypeCode.CompareTo(inputTypeCode) == 0)
                                             .ToList().Select(e => e.ToDomainObject());
            return Products.FromEnumerable(domainObjects);
        }

		public Product GetProductByBarcode(string barcode, string pathDB)
        {
			var entity = this.GetEntityByBarcode(barcode, pathDB);
			if (entity == null) return null;
			return entity.ToDomainObject();
        }

		//public Products GetProductsByBarcode(Barcode barcode, string pathDB)
		//{
		//    return this.GetProductsByBarcodeValue(barcode.Value, pathDB);
		//}

		public Product GetProductByProductMakat(string makat, string pathDB)
        {
			var entity = this.GetEntityByMakat(makat, pathDB);
			if (entity == null) return null;
            return entity.ToDomainObject();
        }

		public Products GetProductsBySupplierCode(string supplierCode, string pathDB)
        {
			var domainObjects = this.GetProducts(pathDB).Where(e => e.SupplierCode.CompareTo(supplierCode) == 0)
                                             .ToList().Select(e => e.ToDomainObject());
            return Products.FromEnumerable(domainObjects);
        }

		public Products GetProductsBySectionCode(string sectionCode, string pathDB)
        {
			var domainObjects = this.GetProducts(pathDB).Where(e => e.SectionCode.CompareTo(sectionCode) == 0)
                                             .ToList().Select(e => e.ToDomainObject());
            return Products.FromEnumerable(domainObjects);
        }

		public Products GetProductsByMarkat(string markat, string pathDB)
        {
            var domainObjects = this.GetProducts(pathDB).Where(e => e.Makat.CompareTo(markat) == 0)
                                             .ToList().Select(e => e.ToDomainObject());
            return Products.FromEnumerable(domainObjects);
        }

		public Products GetProductsByTypeCode(string typeCode, string pathDB)
        {
			var domainObjects = this.GetProducts(pathDB).Where(e => e.TypeCode.CompareTo(typeCode) == 0)
                                             .ToList().Select(e => e.ToDomainObject());
            return Products.FromEnumerable(domainObjects);
        }

 

		public void Insert(Product product, string pathDB)
        {
			if (product == null) return;
            var entity = product.ToEntity();
            this.GetProducts(pathDB).Add(entity);
        }

		public void Delete(string makat, string pathDB)
		{
			var entity = this.GetEntityByMakat(makat, pathDB);
			if (entity == null) return;
			this.GetProducts(pathDB).Remove(entity);
        }

		public void Insert(Products products, string pathDB)
        {
			foreach (var p in products)
			{
				this.Insert(p, pathDB);
			}
        }

		public void Update(Product product, string pathDB)
        {
			if (product == null) return;
			var entity = this.GetEntityByMakat(product.Makat, pathDB);
			if (entity == null) return;
            entity.ApplyChanges(product);
        }

	
		public Products GetProductsByName(string name, string pathDB)
        {
            var domainObjects = this.GetProducts(pathDB).Where(e => e.Name.CompareTo(name) == 0)
                                             .ToList().Select(e => e.ToDomainObject());
            return Products.FromEnumerable(domainObjects);
        }

		public long CountProduct(string pathDB)
        {
            return this.GetProducts(pathDB).LongCount();
        }

		#endregion

        #region private

		private Product GetEntityByCode(string code, string pathDB)
        {
			var entity = this.GetProducts(pathDB).First(e => e.Code == code);
            return entity;
        }

		private Product GetEntityByMakat(string makat, string pathDB)
		{
			var entity = this.GetProducts(pathDB).First(e => e.Makat == makat);
			return entity;
		}

		private Product GetEntityByBarcode(string barcode, string pathDB)
		{
			var entity = this.GetProducts(pathDB).First(e => e.Barcode == barcode);
			return entity;
		}
        #endregion

		#region IProductRepository Members


		public Product GetProductByMakat(string makat, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void DeleteAll(Products products, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.IEnumerable<ProductSimple> GetProductSimples(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public Product Clone(Product product)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.Dictionary<string, ProductSimple> GetProductSimpleDictionary(string pathDB, bool refill = false, string typeMakat = "M")
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.List<App_Data.Product> GetApp_DataProducts(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Copy(System.Collections.Generic.List<App_Data.Product> products, string toPathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public DateTime GetMaxModifyDate(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public long CountMakat(string pathDB)
		{
			throw new NotImplementedException();
		}

		public long CountBarcode(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public void DeleteAll(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public bool IsAnyProductInDb(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.List<string> GetSectionCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.List<string> GetSupplierCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion


		public System.Collections.Generic.List<string> GetFamilyCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}


		public System.Collections.Generic.Dictionary<string, Product> GetProduct_IturCodeErpDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}


		public void DeleteAllIsUpdated(string pathDB, bool isUpdate = true)
		{
			throw new NotImplementedException();
		}


		public Products GetProductsMakatOnly(string pathDB)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.Dictionary<string, ProductSimple> GetProductQuantityERPDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}


		public void DeleteAllMakat(string pathDB)
		{
			throw new NotImplementedException();
		}

		#region IProductRepository Members


		public void FillProductSimpleDictionary(string pathDB, string typeMakat, out System.Collections.Generic.Dictionary<string, ProductSimple> productSimpleDictionary)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.Dictionary<string, Product> GetProductDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public Products GetProductsBarcodeOnly(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.Dictionary<string, ProductSimple> GetSumProductQuantityERPByMakatDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public System.Collections.Generic.Dictionary<string, ProductTagSimple> GetProductTagDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProductRepository Members


		public void SetLastUpdatedCatalog(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
