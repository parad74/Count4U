using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.MappingEF
{
	public static class CustomerReportMapper
    {
  
		public static CustomerReport ToDomainObject(this App_Data.CustomerReport entity)
        {
			if (entity == null) return null;
			return new CustomerReport()
            {
                ID = entity.ID,
				Description = entity.Description != null ? entity.Description : "",
				Name = entity.Name != null ? entity.Name : "",
				ReportCode = entity.ReportCode != null ? entity.ReportCode : "",
				CustomerCode = entity.CustomerCode != null ? entity.CustomerCode : "",
            };
        }

	     
		public static CustomerReport ToSimpleDomainObject(this App_Data.CustomerReport entity)
        {
            throw new NotImplementedException();
        }

    
		public static App_Data.CustomerReport ToEntity(this CustomerReport domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.CustomerReport()
            {
				ID = domainObject.ID,
				Description = domainObject.Description != null ? domainObject.Description : "",
				Name = domainObject.Name != null ? domainObject.Name : "",
				ReportCode = domainObject.ReportCode != null ? domainObject.ReportCode : "",
				CustomerCode = domainObject.CustomerCode != null ? domainObject.CustomerCode : "",

			};
        }


		public static void ApplyChanges(this App_Data.CustomerReport entity, CustomerReport domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Description = domainObject.Description != null ? domainObject.Description : "";
			entity.Name = domainObject.Name != null ? domainObject.Name : "";
			entity.ReportCode = domainObject.ReportCode != null ? domainObject.ReportCode : "";
			entity.CustomerCode = domainObject.CustomerCode != null ? domainObject.CustomerCode : "";

		}
    }
}
