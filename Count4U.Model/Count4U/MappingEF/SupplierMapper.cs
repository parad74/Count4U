using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class SupplierMapper
    {
    
        public static Supplier ToDomainObject(this App_Data.Supplier entity)
        {
			if (entity == null) return null;
            return new Supplier()
            {
                ID = entity.ID,
                Name = entity.Name,
				Description = entity.Description,
				SupplierCode = entity.SupplierCode,
				PlaceCount =Convert.ToInt32(entity.PlaceCount),
				Value = Convert.ToDouble(entity.Value),
				IturCount = Convert.ToInt32(entity.IturCount),
				Area = Convert.ToDouble(entity.Area),
				PercentArea = Convert.ToDouble(entity.PercentArea),
				SupplierLevel = Convert.ToInt32(entity.SupplierLevel)


            };
        }

     
        public static Supplier ToSimpleDomainObject(this App_Data.Supplier entity)
        {
            return new Supplier()
            {
                ID = entity.ID,
                Name = entity.Name,
                Description = entity.Description,
				SupplierCode = entity.SupplierCode,
				PlaceCount = Convert.ToInt32(entity.PlaceCount),
				Value = Convert.ToDouble(entity.Value),
				IturCount = Convert.ToInt32(entity.IturCount),
				Area = Convert.ToDouble(entity.Area),
				PercentArea = Convert.ToDouble(entity.PercentArea),
				SupplierLevel = Convert.ToInt32(entity.SupplierLevel)
            };
        }

       
        public static App_Data.Supplier ToEntity(this Supplier domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.Supplier()
			{
				ID = domainObject.ID,
				Name = domainObject.Name,
				Description = domainObject.Description,
				SupplierCode = domainObject.SupplierCode,
				PlaceCount = domainObject.PlaceCount,
				Value = domainObject.Value,
				IturCount =domainObject.IturCount,
				Area = domainObject.Area,
				PercentArea = domainObject.PercentArea,
				SupplierLevel = domainObject.SupplierLevel
			};
        }


		public static void ApplyChanges(this App_Data.Supplier entity, Supplier domainObject)
		{
			if (domainObject == null) return;
			entity.Name = domainObject.Name;
			entity.Description = domainObject.Description;
			entity.SupplierCode = domainObject.SupplierCode;
			entity.PlaceCount = domainObject.PlaceCount;
			entity.Value = domainObject.Value;
			entity.IturCount = domainObject.IturCount;
			entity.Area = domainObject.Area;
			entity.PercentArea = domainObject.PercentArea;
			entity.SupplierLevel = domainObject.SupplierLevel;
		}
    }
}
