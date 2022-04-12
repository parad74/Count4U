using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class FamilyMapper
    {

		public static Family ToDomainObject(this App_Data.Family entity)
		{
			if (entity == null) return null;
			return new Family()
			{
				ID = entity.ID,
				Name = entity.Name,
				Description = entity.Description,
				FamilyCode = entity.FamilyCode,
				Type = entity.Type,
				Size = entity.Size,
				Extra1 = entity.Extra1,
				Extra2 = entity.Extra2

			};
		}

        public static Family ToSimpleDomainObject(this App_Data.Family entity)
        {
            return new Family()
            {
                ID = entity.ID,
                Name = entity.Name,
                Description = entity.Description,
				FamilyCode = entity.FamilyCode,
				Type = entity.Type,
				Size = entity.Size
            };
        }

        
        public static App_Data.Family ToEntity(this Family domainObject)
        {
			if (domainObject == null) return null;
            return new App_Data.Family()
            {
                ID = domainObject.ID,
                Name = domainObject.Name,
                Description = domainObject.Description,
				FamilyCode = domainObject.FamilyCode,
				Type = domainObject.Type,
				Size = domainObject.Size,
				Extra1 = domainObject.Extra1,
				Extra2 = domainObject.Extra2

            };
        }

      
        public static void ApplyChanges(this App_Data.Family entity, Family domainObject)
        {
			if (domainObject == null) return;
            entity.Name = domainObject.Name;
            entity.Description = domainObject.Description;
			entity.FamilyCode = domainObject.FamilyCode;
			entity.Type = domainObject.Type;
			entity.Size = domainObject.Size;
			entity.Extra1 = domainObject.Extra1;
			entity.Extra2 = domainObject.Extra2;
        }
    }
}
