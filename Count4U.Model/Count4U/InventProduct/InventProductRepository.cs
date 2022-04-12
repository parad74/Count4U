using System;
using System.Linq;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.Mapping;
using Codeplex.Reactive;
using System.Collections.Generic;

namespace Count4U.Model.Count4U
{
    public class InventProductRepository : IInventProductRepository
	{
        private InventProducts _inventProductList;

        #region IInventProductRepository Members

		public InventProducts GetInventProducts(string pathDB)
        {
            if (this._inventProductList == null)
            {
                this._inventProductList = new InventProducts {
                    new InventProduct() { Barcode = "Barcode1", InputTypeCode = "InputType1", StatusInventProductCode = "Status1" },
                    new InventProduct() { Barcode = "Barcode2",InputTypeCode = "InputType2", StatusInventProductCode = "Status2" },
                    new InventProduct() { Barcode = "Barcode3",  InputTypeCode = "InputType1", StatusInventProductCode = "Status1" },
                    new InventProduct() { Barcode = "Barcode4",  InputTypeCode = "InputType2", StatusInventProductCode = "Status2" },
                    new InventProduct() { Barcode = "Barcode5", InputTypeCode = "InputType1", StatusInventProductCode = "Status1" },
                    new InventProduct() { Barcode = "Barcode6",  InputTypeCode = "InputType2", StatusInventProductCode = "Status2" },
                };
            }
            return this._inventProductList;
        }

		public InventProducts GetInventProducts(SelectParams selectParams, string pathDB)
        {
            throw new NotImplementedException();
        }

		public InventProduct GetInventProductByCode(string inventProductCode, string pathDB)
        {
			var entity = this.GetEntityByCode(inventProductCode, pathDB);
			if (entity == null) return null;
            return entity.ToDomainObject();
        }

		public InventProduct GetInventProductByID(long ID, string pathDB)
		{
			throw new NotImplementedException();
		}

		//public InventProducts GetInventProductsByDocumentHeaderName(string documentHeaderName, string pathDB)
		//{
		//    var domainObjects = GetInventProducts(pathDB).Where(e => e.DocumentHeader.CompareTo(documentHeaderName) == 0)
		//                                            .ToList().Select(e => e.ToDomainObject());
		//    return InventProducts.FromEnumerable(domainObjects);
		//}

		//public InventProducts GetInventProductsByStatus(string status, string pathDB)
		//{
		//    var domainObjects = GetInventProducts(pathDB).Where(e => e.StatusInventProduct.CompareTo(status) == 0)
		//                                           .ToList().Select(e => e.ToDomainObject());
		//    return InventProducts.FromEnumerable(domainObjects);
		//}

		public InventProducts GetInventProductsByStatusCode(string statusCode, string pathDB)
        {
			var domainObjects = GetInventProducts(pathDB).Where(e => e.StatusInventProductCode == statusCode)
                                                   .ToList().Select(e => e.ToDomainObject());
            return InventProducts.FromEnumerable(domainObjects);
        }

		public InventProducts GetInventProductsByDocumentHeader(DocumentHeader documentHeader, string pathDB)
        {
			return this.GetInventProductsByDocumentCode(documentHeader.DocumentCode, pathDB);
        }

		public InventProducts GetInventProductsByDocumentCode(string documentCode, string pathDB)
        {
               throw new NotImplementedException();
        }

		//public InventProducts GetInventProductsByBarcodeValue(string barcodeValue, string pathDB)
		//{
		//    var domainObjects = GetInventProducts(pathDB).Where(e => e.Barcode.CompareTo(barcodeValue) == 0)
		//                                           .ToList().Select(e => e.ToDomainObject());
		//    return InventProducts.FromEnumerable(domainObjects);
		//}

		//public InventProducts GetInventProductsByProductID(long productID, string pathDB)
		//{
		//              throw new NotImplementedException();
		//}

		//public InventProducts GetInventProductsByDocumentCode(string documentCode, string pathDB)
		//{
		//    var domainObjects = GetInventProducts(pathDB).Where(e => e.DocumentCode == documentCode)
		//                                           .ToList().Select(e => e.ToDomainObject());
		//    return InventProducts.FromEnumerable(domainObjects);
		//}

		public InventProducts GetInventProductsByInputTypeCode(string inputTypeName, string pathDB)
        {
			var domainObjects = GetInventProducts(pathDB).Where(e => e.InputTypeCode.CompareTo(inputTypeName) == 0)
                                                    .ToList().Select(e => e.ToDomainObject());
            return InventProducts.FromEnumerable(domainObjects);


        }

		//public InventProducts GetInventProductsByItur(long iturID, string pathDB)
		//{
		//              throw new NotImplementedException();
		//}

		//public InventProduct Clone(InventProduct inventProduct)
		//{
		//    var domainObject = inventProduct.Clone();
		//    domainObject.ID = 0;
		//    domainObject.CreateDate = DateTime.Now;
		//    return domainObject;
		//}

		public void Delete(InventProduct inventProduct, string pathDB)
        {
			var entity = this.GetEntityByCode(inventProduct.Code, pathDB);
			if (entity == null) return;
			this.GetInventProducts(pathDB).Remove(entity);
        }

		public void DeleteAllByDocumentHeaderCode(string documentCode, string pathDB)
        {
			GetInventProducts(pathDB).RemoveAll(e => e.DocumentHeaderCode == documentCode);
        }

		public void Insert(InventProduct inventProduct, DocumentHeader documentHeader, string pathDB)
        {
			if (inventProduct == null) return;
            var entity = inventProduct.ToEntity();
			entity.DocumentCode = documentHeader.Code;
			entity.DocumentHeaderCode = documentHeader.Code;
            //entity.DocumentHeader = documentHeader.Name;
			GetInventProducts(pathDB).Add(entity);
        }

		public void Insert(InventProduct inventProduct, string pathDB)
        {
			if (inventProduct == null) return;
            var entity = inventProduct.ToEntity();
			GetInventProducts(pathDB).Add(entity);
        }

		public void Update(InventProduct inventProduct, string pathDB)
        {
			if (inventProduct == null) return;
			var entity = this.GetEntityByCode(inventProduct.Code, pathDB);
			if (entity == null) return;
            entity.ApplyChanges(inventProduct);
        }

        #endregion

        #region private

        private InventProduct GetEntityByCode(string inventProductCode, string pathDB)
        {
			var entity = GetInventProducts(pathDB).First(e => e.Code == inventProductCode);
            return entity;
        }

        #endregion



		#region IInventProductRepository Members


		public InventProduct GetInventProductByBarcode(string barcode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public InventProduct GetInventProductByMakat(string makat, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public long CountInventProduct(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public void ClearStatusBit(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void SetStatusBitByMakat(string makat, int bit, string pathDB)
		{
			throw new NotImplementedException();
		}

		public int GetStatusBitByMakat(string makat, string pathDB)
		{
			throw new NotImplementedException();
		}

		public int[] GetStatusBitArrayIntByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.List<System.Collections.BitArray> GetStatusBitArrayListByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public System.Collections.BitArray GetResultStatusBitOrByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

        public bool IsAnyInventProductInDb(string pathDB)
        {
            throw new NotImplementedException();
        }

        #endregion

		#region IInventProductRepository Members


		public void SetStatusBitByID(string code, int bit, string pathDB)
		{
			throw new NotImplementedException();
		}

		public int GetStatusBitByID(long ID, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public void SetStatusBitByID(long ID, int bit, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public System.Collections.Generic.Dictionary<string, InventProduct> GetInventProductDictionary(string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public void FillInventProductDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void ClearInventProductDictionary()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public void ClearStatusBit(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public double GetSumQuantityEditByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public double GetSumQuantityOriginalByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public double GetSumQuantityDifferenceByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public double GetSumQuantityEditByIturCode(string iturCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public double GetSumQuantityDifferenceByIturCode(string iturCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public double GetSumQuantityOriginalByIturCode(string iturCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public IQueryable<InventProduct> GetInventProductMakatAndSumQuantityEdit(string pathDB)
		{
			throw new NotImplementedException();
		}

		public Dictionary<Tuple<string, string, string>, InventProduct> GetQuantitySerialInventProducts(string pathDB)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, InventProduct> GetDictionaryInventProductsUID(string pathDB)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IInventProductRepository Members


		public int GetMaxNumForDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public string GetCountMakatTotal(AnalezeValueTypeEnum resulteCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public int GetCountItemByDocumentCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public System.Collections.Generic.List<string> GetDocumentCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion
	
#region IInventProductRepository Members


		public ReactiveProperty<string> ReturnStringSumQuantityEditByDocumentCode(string documentCode, string pathDB)
{
 	throw new NotImplementedException();
}

#endregion

		#region IInventProductRepository Members


		public ReactiveProperty<string> ReturnStringSumQuantityEditByIturCode(string iturCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public InventProducts GetInventProductTotal(string pathDB, SelectParams selectParams = null)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public double GetSumQuantityEditByMakat(string makat, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public System.Collections.Generic.IEnumerable<Itur> GetIturSumQuantityEdit(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion


		public void Insert(InventProducts inventProducts, string pathDB)
		{
			throw new NotImplementedException();
		}


		public System.Collections.Generic.Dictionary<string, ProductMakat> GetIPCountByMakatsAndIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#region IInventProductRepository Members


		public void InsertOrUpdate(InventProducts newInventProducts, string toDocumentHeaderCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyByMakatsAndIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyByMakatsAndDocAndIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyByMakatAndSNAndIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyBarcodeAndIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyByBarcodeAndSNAndIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyCodeAndIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public List<string> GetIturCodeList(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public InventProducts GetInventProductsExtended(SelectParams selectParams, string code, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyEditIturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public List<string> DeleteAllNotExistInCatalog(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public InventProducts GetInventProductNotExistInCatalog(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventProductRepository Members


		public Dictionary<string, InventProduct> GetIPQuntetyByMakatAndSNAndProp10IturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, InventProduct> GetIPQuntetyByBarcodeAndSNAndProp10IturCode(SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		public bool IsAnyInventProductInIturCode(string iturCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void InsertClone(InventProducts inventProducts, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
