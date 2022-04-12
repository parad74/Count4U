using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.ProcessC4U.MappingEF
{
	public static class PortMapper
	{

		public static Port ToDomainObject(this App_Data.Port entity)
		{
			if (entity == null) return null;
			return new Port()
			{
				ID = entity.ID,
				PortCode = entity.PortCode != null ? entity.PortCode : "",
				Code = entity.Code != null ? entity.Code : "",
				IP = entity.IP != null ? entity.IP : "",
				Address = entity.Address != null ? entity.Address : "",
				Name = entity.Name != null ? entity.Name : "",
				Description = entity.Description != null ? entity.Description : "",
				StatusCode = entity.StatusCode != null ? entity.StatusCode : "",
				Tag = entity.Tag != null ? entity.Tag : "",
				Tag1 = entity.Tag1 != null ? entity.Tag1 : "",
				Tag2 = entity.Tag2 != null ? entity.Tag2 : "",
				Tag3 = entity.Tag3 != null ? entity.Tag3 : "",
			};
		}

		public static Port ToSimpleDomainObject(this App_Data.Port entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.Port ToEntity(this Port domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Port()
			{
				ID = domainObject.ID,
				PortCode = domainObject.PortCode != null ? domainObject.PortCode : "",
				Code = domainObject.Code != null ? domainObject.Code : "",
				IP = domainObject.IP != null ? domainObject.IP : "",
				Address = domainObject.Address != null ? domainObject.Address : "",
				Name = domainObject.Name != null ? domainObject.Name : "",
				Description = domainObject.Description != null ? domainObject.Description : "",
				StatusCode = domainObject.StatusCode != null ? domainObject.StatusCode : "",
				Tag = domainObject.Tag != null ? domainObject.Tag : "",
				Tag1 = domainObject.Tag1 != null ? domainObject.Tag1 : "",
				Tag2 = domainObject.Tag2 != null ? domainObject.Tag2 : "",
				Tag3 = domainObject.Tag3 != null ? domainObject.Tag3 : "",
			};
		}


		public static void ApplyChanges(this App_Data.Port entity, Port domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.PortCode = domainObject.PortCode != null ? domainObject.PortCode : "";
			entity.Code = domainObject.Code != null ? domainObject.Code : "";
			entity.IP = domainObject.IP != null ? domainObject.IP : "";
			entity.Address = domainObject.Address != null ? domainObject.Address : "";
			entity.Name = domainObject.Name != null ? domainObject.Name : "";
			entity.Description = domainObject.Description != null ? domainObject.Description : "";
			entity.StatusCode = domainObject.StatusCode != null ? domainObject.StatusCode : "";
			entity.Tag = domainObject.Tag != null ? domainObject.Tag : "";
			entity.Tag1 = domainObject.Tag1 != null ? domainObject.Tag1 : "";
			entity.Tag2 = domainObject.Tag2 != null ? domainObject.Tag2 : "";
			entity.Tag3 = domainObject.Tag3 != null ? domainObject.Tag3 : "";
		}
	}
}
