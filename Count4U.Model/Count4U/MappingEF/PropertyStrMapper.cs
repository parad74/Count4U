using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class PropertyStrMapper
    {

		public static PropertyStr ToDomainObject(this App_Data.PropertyStr entity)
        {
			if (entity == null) return null;
			return new PropertyStr()
            {
                ID = entity.ID,
				PropertyStrCode = entity.PropertyStrCode,
                Code = entity.Code,
                Name = entity.Name,
				TypeCode = entity.TypeCode,
				DomainObject = entity.DomainObject, 

            };
        }

		public static PropertyStr ToSimpleDomainObject(this App_Data.PropertyStr entity)
        {
			return new PropertyStr()
            {
				ID = entity.ID,
				PropertyStrCode = entity.PropertyStrCode,
				Code = entity.Code,
				Name = entity.Name,
				TypeCode = entity.TypeCode,
				DomainObject = entity.DomainObject, 

            };
        }


		public static App_Data.PropertyStr ToEntity(this PropertyStr domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.PropertyStr()
            {
                ID = domainObject.ID,
				PropertyStrCode = domainObject.PropertyStrCode,
				Code = domainObject.Code,
				Name = domainObject.Name,
				TypeCode = domainObject.TypeCode,
				DomainObject = domainObject.DomainObject, 
            };
        }


		public static void ApplyChanges(this App_Data.PropertyStr entity, PropertyStr domainObject)
		{
			if (domainObject == null) return;
			entity.PropertyStrCode = domainObject.PropertyStrCode;
			entity.Name = domainObject.Name;
			entity.Code = domainObject.Code;
			entity.TypeCode = domainObject.TypeCode;
			entity.DomainObject = domainObject.DomainObject;
  		}


		
    }
}
