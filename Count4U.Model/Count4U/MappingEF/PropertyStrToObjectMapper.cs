using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class PropertyStrToObjectMapper
    {

		public static PropertyStrToObject ToDomainObject(this App_Data.PropertyStrToObject entity)
        {
			if (entity == null) return null;
			return new PropertyStrToObject()
            {
                ID = entity.ID,
				PropertyStrCode = entity.PropertyStrCode,
				DomainObject = entity.DomainObject,
				ObjectCode = entity.ObjectCode,
            };
        }

		public static PropertyStrToObject ToSimpleDomainObject(this App_Data.PropertyStrToObject entity)
        {
			return new PropertyStrToObject()
            {
                ID = entity.ID,
				PropertyStrCode = entity.PropertyStrCode,
				DomainObject = entity.DomainObject,
				ObjectCode = entity.ObjectCode,
            };
        }


		public static App_Data.PropertyStrToObject ToEntity(this PropertyStrToObject domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.PropertyStrToObject()
            {
                ID = domainObject.ID,
				PropertyStrCode = domainObject.PropertyStrCode,
				DomainObject = domainObject.DomainObject,
				ObjectCode = domainObject.ObjectCode,
            };
        }


		public static void ApplyChanges(this App_Data.PropertyStrToObject entity, PropertyStrToObject domainObject)
		{
			if (domainObject == null) return;
			entity.PropertyStrCode = domainObject.PropertyStrCode;
			entity.DomainObject = domainObject.DomainObject;
			entity.ObjectCode = domainObject.ObjectCode;
  		}


		
    }
}
