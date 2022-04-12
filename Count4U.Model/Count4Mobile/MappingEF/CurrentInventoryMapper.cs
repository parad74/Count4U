using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4Mobile.MappingEF
{
	public static class CurrentInventoryMapper
	{

		public static CurrentInventory ToDomainObject(this App_Data.CurrentInventory entity)
		{
			if (entity == null) return null;
			return new CurrentInventory()
			{
				Id = entity.Id,
				SerialNumberLocal = entity.SerialNumberLocal != null ? entity.SerialNumberLocal : "",
				ItemCode = entity.ItemCode != null ? entity.ItemCode : "",
				SerialNumberSupplier = entity.SerialNumberSupplier != null ? entity.SerialNumberSupplier : "",
				Quantity = entity.Quantity != null ? entity.Quantity : "",
				PropertyStr1 = entity.PropertyStr1 != null ? entity.PropertyStr1 : "",
				PropertyStr2 = entity.PropertyStr2 != null ? entity.PropertyStr2 : "",
				PropertyStr3 = entity.PropertyStr3 != null ? entity.PropertyStr3 : "",
				PropertyStr4 = entity.PropertyStr4 != null ? entity.PropertyStr4 : "",
				PropertyStr5 = entity.PropertyStr5 != null ? entity.PropertyStr5 : "",
				PropertyStr6 = entity.PropertyStr6 != null ? entity.PropertyStr6 : "",
				PropertyStr7 = entity.PropertyStr7 != null ? entity.PropertyStr7 : "",
				PropertyStr8 = entity.PropertyStr8 != null ? entity.PropertyStr8 : "",
				PropertyStr9 = entity.PropertyStr9 != null ? entity.PropertyStr9 : "",
				PropertyStr10 = entity.PropertyStr10 != null ? entity.PropertyStr10 : "",
				PropertyStr11 = entity.PropertyStr11 != null ? entity.PropertyStr11 : "",
				PropertyStr12 = entity.PropertyStr12 != null ? entity.PropertyStr12 : "",
				PropertyStr13 = entity.PropertyStr13 != null ? entity.PropertyStr13 : "",
				PropertyStr14 = entity.PropertyStr14 != null ? entity.PropertyStr14 : "",
				PropertyStr15 = entity.PropertyStr15 != null ? entity.PropertyStr15 : "",
				PropertyStr16 = entity.PropertyStr16 != null ? entity.PropertyStr16 : "",
				PropertyStr17 = entity.PropertyStr17 != null ? entity.PropertyStr17 : "",
				PropertyStr18 = entity.PropertyStr18 != null ? entity.PropertyStr18 : "",
				PropertyStr19 = entity.PropertyStr19 != null ? entity.PropertyStr19 : "",
				PropertyStr20 = entity.PropertyStr20 != null ? entity.PropertyStr20 : "",
				LocationCode = entity.LocationCode != null ? entity.LocationCode : "",
				DateModified = entity.DateModified != null ? entity.DateModified : "",
				DateCreated = entity.DateCreated != null ? entity.DateCreated : "",
				ItemStatus = entity.ItemStatus != null ? entity.ItemStatus : "",
				ItemType = entity.ItemType != null ? entity.ItemType : "",
				UnitTypeCode = entity.UnitTypeCode != null ? entity.UnitTypeCode : "",
			};
		}

		public static CurrentInventory ToSimpleDomainObject(this App_Data.CurrentInventory entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.CurrentInventory ToEntity(this CurrentInventory domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.CurrentInventory()
			{
				Id = domainObject.Id,
				SerialNumberLocal = domainObject.SerialNumberLocal != null ? domainObject.SerialNumberLocal : "",
				ItemCode = domainObject.ItemCode != null ? domainObject.ItemCode : "",
				SerialNumberSupplier = domainObject.SerialNumberSupplier != null ? domainObject.SerialNumberSupplier : "",
				Quantity = domainObject.Quantity != null ? domainObject.Quantity : "",
				PropertyStr1 = domainObject.PropertyStr1 != null ? domainObject.PropertyStr1 : "",
				PropertyStr2 = domainObject.PropertyStr2 != null ? domainObject.PropertyStr2 : "",
				PropertyStr3 = domainObject.PropertyStr3 != null ? domainObject.PropertyStr3 : "",
				PropertyStr4 = domainObject.PropertyStr4 != null ? domainObject.PropertyStr4 : "",
				PropertyStr5 = domainObject.PropertyStr5 != null ? domainObject.PropertyStr5 : "",
				PropertyStr6 = domainObject.PropertyStr6 != null ? domainObject.PropertyStr6 : "",
				PropertyStr7 = domainObject.PropertyStr7 != null ? domainObject.PropertyStr7 : "",
				PropertyStr8 = domainObject.PropertyStr8 != null ? domainObject.PropertyStr8 : "",
				PropertyStr9 = domainObject.PropertyStr9 != null ? domainObject.PropertyStr9 : "",
				PropertyStr10 = domainObject.PropertyStr10 != null ? domainObject.PropertyStr10 : "",
				PropertyStr11 = domainObject.PropertyStr11 != null ? domainObject.PropertyStr11 : "",
				PropertyStr12 = domainObject.PropertyStr12 != null ? domainObject.PropertyStr12 : "",
				PropertyStr13 = domainObject.PropertyStr13 != null ? domainObject.PropertyStr13 : "",
				PropertyStr14 = domainObject.PropertyStr14 != null ? domainObject.PropertyStr14 : "",
				PropertyStr15 = domainObject.PropertyStr15 != null ? domainObject.PropertyStr15 : "",
				PropertyStr16 = domainObject.PropertyStr16 != null ? domainObject.PropertyStr16 : "",
				PropertyStr17 = domainObject.PropertyStr17 != null ? domainObject.PropertyStr17 : "",
				PropertyStr18 = domainObject.PropertyStr18 != null ? domainObject.PropertyStr18 : "",
				PropertyStr19 = domainObject.PropertyStr19 != null ? domainObject.PropertyStr19 : "",
				PropertyStr20 = domainObject.PropertyStr20 != null ? domainObject.PropertyStr20 : "",
				LocationCode = domainObject.LocationCode != null ? domainObject.LocationCode : "",
				DateModified = domainObject.DateModified != null ? domainObject.DateModified : "",
				DateCreated = domainObject.DateCreated != null ? domainObject.DateCreated : "",
				ItemStatus = domainObject.ItemStatus != null ? domainObject.ItemStatus : "",
				ItemType = domainObject.ItemType != null ? domainObject.ItemType : "",
				UnitTypeCode = domainObject.UnitTypeCode != null ? domainObject.UnitTypeCode : "",
			};
		}


		public static void ApplyChanges(this App_Data.CurrentInventory entity, CurrentInventory domainObject)
		{
			if (domainObject == null) return;
			entity.Id = domainObject.Id;
			entity.Id = domainObject.Id;
			entity.SerialNumberLocal = domainObject.SerialNumberLocal != null ? domainObject.SerialNumberLocal : "";
			entity.ItemCode = domainObject.ItemCode != null ? domainObject.ItemCode : "";
			entity.SerialNumberSupplier = domainObject.SerialNumberSupplier != null ? domainObject.SerialNumberSupplier : "";
			entity.Quantity = domainObject.Quantity != null ? domainObject.Quantity : "";
			entity.PropertyStr1 = domainObject.PropertyStr1 != null ? domainObject.PropertyStr1 : "";
			entity.PropertyStr2 = domainObject.PropertyStr2 != null ? domainObject.PropertyStr2 : "";
			entity.PropertyStr3 = domainObject.PropertyStr3 != null ? domainObject.PropertyStr3 : "";
			entity.PropertyStr4 = domainObject.PropertyStr4 != null ? domainObject.PropertyStr4 : "";
			entity.PropertyStr5 = domainObject.PropertyStr5 != null ? domainObject.PropertyStr5 : "";
			entity.PropertyStr6 = domainObject.PropertyStr6 != null ? domainObject.PropertyStr6 : "";
			entity.PropertyStr7 = domainObject.PropertyStr7 != null ? domainObject.PropertyStr7 : "";
			entity.PropertyStr8 = domainObject.PropertyStr8 != null ? domainObject.PropertyStr8 : "";
			entity.PropertyStr9 = domainObject.PropertyStr9 != null ? domainObject.PropertyStr9 : "";
			entity.PropertyStr10 = domainObject.PropertyStr10 != null ? domainObject.PropertyStr10 : "";
			entity.PropertyStr11 = domainObject.PropertyStr11 != null ? domainObject.PropertyStr11 : "";
			entity.PropertyStr12 = domainObject.PropertyStr12 != null ? domainObject.PropertyStr12 : "";
			entity.PropertyStr13 = domainObject.PropertyStr13 != null ? domainObject.PropertyStr13 : "";
			entity.PropertyStr14 = domainObject.PropertyStr14 != null ? domainObject.PropertyStr14 : "";
			entity.PropertyStr15 = domainObject.PropertyStr15 != null ? domainObject.PropertyStr15 : "";
			entity.PropertyStr16 = domainObject.PropertyStr16 != null ? domainObject.PropertyStr16 : "";
			entity.PropertyStr17 = domainObject.PropertyStr17 != null ? domainObject.PropertyStr17 : "";
			entity.PropertyStr18 = domainObject.PropertyStr18 != null ? domainObject.PropertyStr18 : "";
			entity.PropertyStr19 = domainObject.PropertyStr19 != null ? domainObject.PropertyStr19 : "";
			entity.PropertyStr20 = domainObject.PropertyStr20 != null ? domainObject.PropertyStr20 : "";
			entity.LocationCode = domainObject.LocationCode != null ? domainObject.LocationCode : "";
			entity.DateModified = domainObject.DateModified != null ? domainObject.DateModified : "";
			entity.DateCreated = domainObject.DateCreated != null ? domainObject.DateCreated : "";
			entity.ItemStatus = domainObject.ItemStatus != null ? domainObject.ItemStatus : "";
			entity.ItemType = domainObject.ItemType != null ? domainObject.ItemType : "";
			entity.UnitTypeCode = domainObject.UnitTypeCode != null ? domainObject.UnitTypeCode : "";
		}
	}
}
