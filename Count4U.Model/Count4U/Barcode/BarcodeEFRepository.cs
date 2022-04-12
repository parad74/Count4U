using System;
using System.Data.Linq;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
    /*public class BarcodeEFRepository : BaseEFRepository, IBarcodeRepository
    {
		public BarcodeEFRepository(IConnectionDB connection)
			: base(connection)
        {
            
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion

        #region IBarcodeRepository Members

		public Barcodes GetBarcodes(string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var domainObjects = db.Barcodes.ToList().Select(e => e.ToDomainObject());
                return Barcodes.FromEnumerable(domainObjects);
            }
        }

        public Barcode Clone(Barcode barcode)
        {
            var domainObject = barcode.Clone();
            domainObject.ID = 0;
            return domainObject;
        }

		public void Delete(Barcode barcode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = GetEntityByID(db, barcode.ID);
				 if (entity == null) return;
                db.Barcodes.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void DeleteByProductID(long productID, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.Barcodes.Where(e => e.ProductID == productID).ToList();
				if (entities == null) return;
                entities.ForEach(e => db.Barcodes.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public void DeleteAllByBarcodeValue(string barcodeValue, string pathDB)
        {
            // TODO: удалять Barcode и связанные товары.
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entities = db.Barcodes.Where(e => e.Value.CompareTo(barcodeValue) == 0).ToList();
				if (entities == null) return;
                entities.ForEach(e => db.Barcodes.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public void DeleteAllByBarcodeID(long barcodeID, string pathDB)
        {
            // TODO: удалять Barcode и связанные товары.
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = GetEntityByID(db, barcodeID);
				 if (entity == null) return;
                db.Barcodes.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void Insert(Barcode barcode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = barcode.ToEntity();
                db.Barcodes.AddObject(entity);
                db.SaveChanges();
            }
        }

        public void Insert(Barcode barcode, Product product , string pathDB)
        {
            barcode.ProductID = product.ID;
			barcode.ProductName = product.Name;
			this.Insert(barcode, pathDB);
        }

		public void Update(Barcode barcode, string pathDB)
        {
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = GetEntityByID(db, barcode.ID);
				if (entity == null) return;
                entity.ApplyChanges(barcode);
                db.SaveChanges();
            }
        }

		public bool IsExistsBarcode(string barcodeValue, string pathDB)
        {
            // TODO: по хеш таблице.
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                long value = db.Barcodes.LongCount(e => e.Value.CompareTo(barcodeValue) == 0);
                return value > 0;
            }
        }

		public bool IsExistsBarcode(string barcodeValue, Product product, string pathDB)
        {
            // TODO: по хеш таблице.
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                long value = db.Barcodes.LongCount(e => e.ProductID == product.ID && e.Value.CompareTo(barcodeValue) == 0);
                return value > 0;
            }
        }

        #endregion

        #region private

        private App_Data.Barcode GetEntityByID(App_Data.Count4UDB db, long barcodeID)
        {
            var entity = db.Barcodes.FirstOrDefault(e => e.ID == barcodeID);
            return entity;
        }

        #endregion

		
	}			   */
}
