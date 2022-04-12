using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class ShelfMapper
    {

		public static Shelf ToDomainObject(this App_Data.Shelf entity)
		{
			if (entity == null) return null;
			return new Shelf()
			{
				ID = entity.ID,
				ShelfPartCode = entity.ShelfPartCode,
				IturCode = entity.IturCode,
				SupplierCode = entity.SupplierCode,
				ShelfCode = entity.ShelfCode,
				SupplierName = entity.SupplierName,
				CreateDataTime = entity.CreateDataTime != null ? Convert.ToDateTime(entity.CreateDataTime) : DateTime.Now,
				ShelfNum = entity.ShelfNum != null ? Convert.ToInt32(entity.ShelfNum) : 0,
				Width = entity.Width != null ? Convert.ToDouble(entity.Width) : 0.0,
				Height = entity.Height != null ? Convert.ToDouble(entity.Height) : 0.0,
				Area = entity.Area != null ? Convert.ToDouble(entity.Area) : 0.0
			};
		}

		public static Shelf ToSimpleDomainObject(this App_Data.Shelf entity)
        {
			return new Shelf()
            {
                ID = entity.ID,
				ShelfPartCode = entity.ShelfPartCode,
				IturCode = entity.IturCode,
				SupplierCode = entity.SupplierCode,
				ShelfCode = entity.ShelfCode,
				SupplierName = entity.SupplierName,
				CreateDataTime = entity.CreateDataTime != null ? Convert.ToDateTime(entity.CreateDataTime) : DateTime.Now,
				ShelfNum = entity.ShelfNum != null ? Convert.ToInt32(entity.ShelfNum) : 0,
				Width = entity.Width != null ? Convert.ToDouble(entity.Width) : 0.0,
				Height = entity.Height != null ? Convert.ToDouble(entity.Height) : 0.0,
				Area = entity.Area != null ? Convert.ToDouble(entity.Area) : 0.0
            };
        }


		public static App_Data.Shelf ToEntity(this Shelf domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.Shelf()
            {
                ID = domainObject.ID,
				ShelfPartCode = domainObject.ShelfPartCode,
				IturCode = domainObject.IturCode,
				SupplierCode = domainObject.SupplierCode,
				ShelfCode = domainObject.ShelfCode,
				SupplierName = domainObject.SupplierName,
				CreateDataTime = domainObject.CreateDataTime,
				ShelfNum = domainObject.ShelfNum != null ? Convert.ToInt32(domainObject.ShelfNum) : 0,
				Width = domainObject.Width != null ? Convert.ToDouble(domainObject.Width) : 0.0,
				Height = domainObject.Height != null ? Convert.ToDouble(domainObject.Height) : 0.0,
				Area = domainObject.Area != null ? Convert.ToDouble(domainObject.Area) : 0.0
            };
        }


		public static void ApplyChanges(this App_Data.Shelf entity, Shelf domainObject)
        {
			if (domainObject == null) return;
			entity.ShelfPartCode = domainObject.ShelfPartCode;
			entity.IturCode = domainObject.IturCode;
			entity.SupplierCode = domainObject.SupplierCode;
			entity.ShelfCode = domainObject.ShelfCode;
			entity.SupplierName = domainObject.SupplierName;
			entity.CreateDataTime = domainObject.CreateDataTime;
			entity.ShelfNum = domainObject.ShelfNum != null ? Convert.ToInt32(domainObject.ShelfNum) : 0;
			entity.Width = domainObject.Width != null ? Convert.ToDouble(domainObject.Width) : 0.0;
			entity.Height = domainObject.Height != null ? Convert.ToDouble(domainObject.Height) : 0.0;
			entity.Area = domainObject.Area != null ? Convert.ToDouble(domainObject.Area) : 0.0;
        }
    }
}
