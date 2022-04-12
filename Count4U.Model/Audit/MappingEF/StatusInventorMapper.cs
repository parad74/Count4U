using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Audit.MappingEF
{
	public static class StatusInventorMapper
    {
      
		public static StatusInventor ToDomainObject(this App_Data.StatusInventor entity)
		{
			if (entity == null) return null;
			return new StatusInventor()
			{
				ID = entity.ID,
				Code = entity.Code,
				Description = entity.Description,
				Name = entity.Name
			};
		}

      
		public static StatusInventor ToSimpleDomainObject(this App_Data.StatusInventor entity)
        {
            throw new NotImplementedException();
        }

    
		public static App_Data.StatusInventor ToEntity(this StatusInventor domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.StatusInventor()
            {
				ID = domainObject.ID,
				Code = domainObject.Code,
				Description = domainObject.Description,
				Name = domainObject.Name
			};
        }

       
		public static void ApplyChanges(this App_Data.StatusInventor entity, StatusInventor domainObject)
        {
			if (domainObject == null) return;
 			entity.ID = domainObject.ID;
			entity.Code = domainObject.Code;
			entity.Description = domainObject.Description;
			entity.Name = domainObject.Name;
        }
    }
}
