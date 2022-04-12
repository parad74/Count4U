using System;
using System.Linq;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Mapping;

namespace Count4U.Model.Count4U
{
    public class BarcodeRepository : IBarcodeRepository
	{
        private Barcodes _barcodeList;

        #region IBarcodeRepository Members

		public Barcodes GetBarcodes(string pathDB)
        {
            if (this._barcodeList == null)
            {
                this._barcodeList = new Barcodes {
                    new Barcode() { ID = 1, ProductID = 1, ProductName = "Product1", Value = "Barcode1" },
                    new Barcode() { ID = 2, ProductID = 2, ProductName = "Product2", Value = "Barcode2" },
                    new Barcode() { ID = 3, ProductID = 3, ProductName = "Product3", Value = "Barcode3" },
                    new Barcode() { ID = 4, ProductID = 4, ProductName = "Product4", Value = "Barcode4" },
                    new Barcode() { ID = 5, ProductID = 5, ProductName = "Product5", Value = "Barcode5" },
                    new Barcode() { ID = 6, ProductID = 6, ProductName = "Product6", Value = "Barcode6" },
                };
            }
            return this._barcodeList;
        }

        public Barcode Clone(Barcode barcode)
        {
            var domainObject = barcode.Clone();
            domainObject.ID = 0;
            return domainObject;
        }

		public void Delete(Barcode barcode, string pathDB)
        {
			var entity = this.GetEntityByID(barcode.ID, pathDB);
			if (entity == null) return;
			this.GetBarcodes(pathDB).Remove(entity);
        }

		public void DeleteByProductID(long productID, string pathDB)
        {
			this.GetBarcodes(pathDB).RemoveAll(e => e.ProductID == productID);
        }

		public void DeleteAllByBarcodeValue(string barcodeValue, string pathDB)
        {
            // TODO: удалять Barcode и связанные товары.
			this.GetBarcodes(pathDB).RemoveAll(e => e.Value.CompareTo(barcodeValue) == 0);
        }

		public void DeleteAllByBarcodeID(long barcodeID, string pathDB)
        {
            // TODO: удалять Barcode и связанные товары.
			var entity = this.GetEntityByID(barcodeID, pathDB);
			if (entity == null) return;
			this.GetBarcodes(pathDB).Remove(entity);
        }

		public void Insert(Barcode barcode, string pathDB)
        {
			if (barcode == null) return;
            var entity = barcode.ToEntity();
			this.GetBarcodes(pathDB).Add(entity);
        }

        public void Insert(Barcode barcode, Product product,  string pathDB)
        {
            barcode.ProductID = product.ID;
			barcode.ProductName = product.Name;
            this.Insert(barcode, pathDB);
        }

		public void Update(Barcode barcode, string pathDB)
        {
			if (barcode == null) return;
			var entity = this.GetEntityByID(barcode.ID, pathDB);
			if (entity == null) return;
            entity.ApplyChanges(barcode);
        }

		public bool IsExistsBarcode(string barcodeValue, string pathDB)
        {
			return this.GetBarcodes(pathDB).LongCount(e => e.Value.CompareTo(barcodeValue) == 0) > 0;
        }

		public bool IsExistsBarcode(string barcodeValue, Product product, string pathDB)
        {
			return this.GetBarcodes(pathDB).LongCount(e => e.ProductID == product.ID && e.Value.CompareTo(barcodeValue) == 0) > 0;
        }

        #endregion

        #region private

		private Barcode GetEntityByID(long barcodeID, string pathDB)
        {
			var entity = this.GetBarcodes(pathDB).First(e => e.ID == barcodeID);
            return entity;
        }

        #endregion

	
	}
}
