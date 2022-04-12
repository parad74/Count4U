using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4Mobile.MappingEF
{
	public static class TemporaryInventoryMapper
	{

		public static TemporaryInventory ToDomainObject(this App_Data.TemporaryInventory entity)
		{
			if (entity == null) return null;
			return new TemporaryInventory()
			{
				Id = entity.Id,
				OldUid = entity.OldUid != null ? entity.OldUid : "",
				NewUid = entity.NewUid != null ? entity.NewUid : "",
				Domain = entity.Domain != null ? entity.Domain : "",

				OldSerialNumber = entity.OldSerialNumber != null ? entity.OldSerialNumber : "",
				OldItemCode = entity.OldItemCode != null ? entity.OldItemCode : "",
				OldLocationCode = entity.OldLocationCode != null ? entity.OldLocationCode : "",
				OldProductCode = entity.OldProductCode != null ? entity.OldProductCode : "",
				OldKey = entity.OldKey != null ? entity.OldKey : "",

				NewSerialNumber = entity.NewSerialNumber != null ? entity.NewSerialNumber : "",
				NewItemCode = entity.NewItemCode != null ? entity.NewItemCode : "",
				NewLocationCode = entity.NewLocationCode != null ? entity.NewLocationCode : "",
				NewProductCode = entity.NewProductCode != null ? entity.NewProductCode : "",
				NewKey = entity.NewKey != null ? entity.NewKey : "",

				DateModified = entity.DateModified != null ? entity.DateModified : "",
				Operation = entity.Operation != null ? entity.Operation : "",
				Device = entity.Device != null ? entity.Device : "",
				DbFileName = entity.DbFileName != null ? entity.DbFileName : "",
				Tag = entity.Tag != null ? entity.Tag : "",
				Description = entity.Description != null ? entity.Description : "", 

			};
		}

		public static TemporaryInventory ToSimpleDomainObject(this App_Data.TemporaryInventory entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.TemporaryInventory ToEntity(this TemporaryInventory domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.TemporaryInventory()
			{
				Id = domainObject.Id,
				OldUid = domainObject.OldUid != null ? domainObject.OldUid : "",
				NewUid = domainObject.NewUid != null ? domainObject.NewUid : "",
				Domain = domainObject.Domain != null ? domainObject.Domain : "",

				OldSerialNumber = domainObject.OldSerialNumber != null ? domainObject.OldSerialNumber : "",
				OldItemCode = domainObject.OldItemCode != null ? domainObject.OldItemCode : "",
				OldLocationCode = domainObject.OldLocationCode != null ? domainObject.OldLocationCode : "",
				OldProductCode = domainObject.OldProductCode != null ? domainObject.OldProductCode : "",
				OldKey = domainObject.OldKey != null ? domainObject.OldKey : "",

				NewSerialNumber = domainObject.NewSerialNumber != null ? domainObject.NewSerialNumber : "",
				NewItemCode = domainObject.NewItemCode != null ? domainObject.NewItemCode : "",
				NewLocationCode = domainObject.NewLocationCode != null ? domainObject.NewLocationCode : "",
				NewProductCode = domainObject.NewProductCode != null ? domainObject.NewProductCode : "",
				NewKey = domainObject.NewKey != null ? domainObject.NewKey : "",

				DateModified = domainObject.DateModified != null ? domainObject.DateModified : "",
				Operation = domainObject.Operation != null ? domainObject.Operation : "",
				Device = domainObject.Device != null ? domainObject.Device : "",
				DbFileName = domainObject.DbFileName != null ? domainObject.DbFileName : "",
				Tag = domainObject.Tag != null ? domainObject.Tag : "",
				Description = domainObject.Description != null ? domainObject.Description : "", 
			};
		}


		public static void ApplyChanges(this App_Data.TemporaryInventory entity, TemporaryInventory domainObject)
		{
			if (domainObject == null) return;
			entity.Id = domainObject.Id;
				entity.OldUid = domainObject.OldUid != null ? domainObject.OldUid : "";
				entity.NewUid = domainObject.NewUid != null ? domainObject.NewUid : "";
				entity.Domain = domainObject.Domain != null ? domainObject.Domain : "";

				entity.OldSerialNumber = domainObject.OldSerialNumber != null ? domainObject.OldSerialNumber : "";
				entity.OldItemCode = domainObject.OldItemCode != null ? domainObject.OldItemCode : "";
				entity.OldLocationCode = domainObject.OldLocationCode != null ? domainObject.OldLocationCode : "";
				entity.OldProductCode = domainObject.OldProductCode != null ? domainObject.OldProductCode : "";
				entity.OldKey = domainObject.OldKey != null ? domainObject.OldKey : "";

				entity.NewSerialNumber = domainObject.NewSerialNumber != null ? domainObject.NewSerialNumber : "";
				entity.NewItemCode = domainObject.NewItemCode != null ? domainObject.NewItemCode : "";
				entity.NewLocationCode = domainObject.NewLocationCode != null ? domainObject.NewLocationCode : "";
				entity.NewProductCode = domainObject.NewProductCode != null ? domainObject.NewProductCode : "";
				entity.NewKey = domainObject.NewKey != null ? domainObject.NewKey : "";

				entity.DateModified = domainObject.DateModified != null ? domainObject.DateModified : "";
				entity.Operation = domainObject.Operation != null ? domainObject.Operation : "";
				entity.Device = domainObject.Device != null ? domainObject.Device : "";
				entity.DbFileName = domainObject.DbFileName != null ? domainObject.DbFileName : "";
				entity.Tag = domainObject.Tag != null ? domainObject.Tag : "";
				entity.Description = domainObject.Description != null ? domainObject.Description : "";
		}
	}
}
