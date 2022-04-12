using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.ProcessC4U.MappingEF
{
	public static class ProcessMapper
	{

		public static Process ToDomainObject(this App_Data.Process entity)
		{
			if (entity == null) return null;
			return new Process()
			{
				ID = entity.ID,
				ProcessCode = entity.ProcessCode != null ? entity.ProcessCode : "",
				Code = entity.Code != null ? entity.Code : "",
				SyncCode = entity.SyncCode != null ? entity.SyncCode : "",
				DBPath = entity.DBPath != null ? entity.DBPath : "",
				CreateDate = entity.CreateDate,
				Manager = entity.Manager != null ? entity.Manager : "",
				Name = entity.Name != null ? entity.Name : "",
				Title = entity.Title != null ? entity.Title : "",
				Description = entity.Description != null ? entity.Description : "",
				StatusCode = entity.StatusCode != null ? entity.StatusCode : "",
				Tag = entity.Tag != null ? entity.Tag : "",
				Tag1 = entity.Tag1 != null ? entity.Tag1 : "",
				Tag2 = entity.Tag2 != null ? entity.Tag2 : "",
				Tag3 = entity.Tag3 != null ? entity.Tag3 : "",
			};
		}

		public static Process ToSimpleDomainObject(this App_Data.Process entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.Process ToEntity(this Process domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Process()
			{
				ID = domainObject.ID,
				ProcessCode = domainObject.ProcessCode != null ? domainObject.ProcessCode : "",
				Code = domainObject.Code != null ? domainObject.Code : "",
				SyncCode = domainObject.SyncCode != null ? domainObject.SyncCode : "",
				DBPath = domainObject.DBPath != null ? domainObject.DBPath : "",
				CreateDate = domainObject.CreateDate,
				Manager = domainObject.Manager != null ? domainObject.Manager : "",
				Name = domainObject.Name != null ? domainObject.Name : "",
				Title = domainObject.Title != null ? domainObject.Title : "",
				Description = domainObject.Description != null ? domainObject.Description : "",
				StatusCode = domainObject.StatusCode != null ? domainObject.StatusCode : "",
				Tag = domainObject.Tag != null ? domainObject.Tag : "",
				Tag1 = domainObject.Tag1 != null ? domainObject.Tag1 : "",
				Tag2 = domainObject.Tag2 != null ? domainObject.Tag2 : "",
				Tag3 = domainObject.Tag3 != null ? domainObject.Tag3 : "",
			};
		}


		public static void ApplyChanges(this App_Data.Process entity, Process domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.ProcessCode = domainObject.ProcessCode != null ? domainObject.ProcessCode : "";
			entity.Code = domainObject.Code != null ? domainObject.Code : "";
			entity.SyncCode = domainObject.SyncCode != null ? domainObject.SyncCode : "";
			entity.DBPath = domainObject.DBPath != null ? domainObject.DBPath : "";
			entity.CreateDate = domainObject.CreateDate;
			entity.Manager = domainObject.Manager != null ? domainObject.Manager : "";
			entity.Name = domainObject.Name != null ? domainObject.Name : "";
			entity.Title = domainObject.Title != null ? domainObject.Title : "";
			entity.Description = domainObject.Description != null ? domainObject.Description : "";
			entity.StatusCode = domainObject.StatusCode != null ? domainObject.StatusCode : "";
			entity.Tag = domainObject.Tag != null ? domainObject.Tag : "";
			entity.Tag1 = domainObject.Tag1 != null ? domainObject.Tag1 : "";
			entity.Tag2 = domainObject.Tag2 != null ? domainObject.Tag2 : "";
			entity.Tag3 = domainObject.Tag3 != null ? domainObject.Tag3 : "";
		}
	}
}
