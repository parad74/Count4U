using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4Mobile.MappingEF
{
	public static class PreviousInventoryMapper
	{

		public static PreviousInventory ToDomainObject(this App_Data.PreviousInventory entity)
		{
			if (entity == null) return null;
			return new PreviousInventory()
			{
				Id = entity.Id,
				Uid = entity.Uid != null ? entity.Uid : "",
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
				PropExtenstion1 = entity.PropExtenstion1 != null ? entity.PropExtenstion1 : "",
				PropExtenstion2 = entity.PropExtenstion2 != null ? entity.PropExtenstion2 : "",
				PropExtenstion3 = entity.PropExtenstion3 != null ? entity.PropExtenstion3 : "",
				PropExtenstion4 = entity.PropExtenstion4 != null ? entity.PropExtenstion4 : "",
				PropExtenstion5 = entity.PropExtenstion5 != null ? entity.PropExtenstion5 : "",
				PropExtenstion6 = entity.PropExtenstion6 != null ? entity.PropExtenstion6 : "",
				PropExtenstion7 = entity.PropExtenstion7 != null ? entity.PropExtenstion7 : "",
				PropExtenstion8 = entity.PropExtenstion8 != null ? entity.PropExtenstion8 : "",
				PropExtenstion9 = entity.PropExtenstion9 != null ? entity.PropExtenstion9 : "",
				PropExtenstion10 = entity.PropExtenstion10 != null ? entity.PropExtenstion10 : "",
				PropExtenstion11 = entity.PropExtenstion11 != null ? entity.PropExtenstion11 : "",
				PropExtenstion12 = entity.PropExtenstion12 != null ? entity.PropExtenstion12 : "",
				PropExtenstion13 = entity.PropExtenstion13 != null ? entity.PropExtenstion13 : "",
				PropExtenstion14 = entity.PropExtenstion14 != null ? entity.PropExtenstion14 : "",
				PropExtenstion15 = entity.PropExtenstion15 != null ? entity.PropExtenstion15 : "",
				PropExtenstion16 = entity.PropExtenstion16 != null ? entity.PropExtenstion16 : "",
				PropExtenstion17 = entity.PropExtenstion17 != null ? entity.PropExtenstion17 : "",
				PropExtenstion18 = entity.PropExtenstion18 != null ? entity.PropExtenstion18 : "",
				PropExtenstion19 = entity.PropExtenstion19 != null ? entity.PropExtenstion19 : "",
				PropExtenstion20 = entity.PropExtenstion20 != null ? entity.PropExtenstion20 : "",
				PropExtenstion21 = entity.PropExtenstion21 != null ? entity.PropExtenstion21 : "",
				PropExtenstion22 = entity.PropExtenstion22 != null ? entity.PropExtenstion22 : ""
			};
		}

		public static PreviousInventory ToSimpleDomainObject(this App_Data.PreviousInventory entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.PreviousInventory ToEntity(this PreviousInventory domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.PreviousInventory()
			{
				Id = domainObject.Id,
				Uid = domainObject.Uid != null ? domainObject.Uid : "",
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
				PropExtenstion1 = domainObject.PropExtenstion1 != null ? domainObject.PropExtenstion1 : "",
				PropExtenstion2 = domainObject.PropExtenstion2 != null ? domainObject.PropExtenstion2 : "",
				PropExtenstion3 = domainObject.PropExtenstion3 != null ? domainObject.PropExtenstion3 : "",
				PropExtenstion4 = domainObject.PropExtenstion4 != null ? domainObject.PropExtenstion4 : "",
				PropExtenstion5 = domainObject.PropExtenstion5 != null ? domainObject.PropExtenstion5 : "",
				PropExtenstion6 = domainObject.PropExtenstion6 != null ? domainObject.PropExtenstion6 : "",
				PropExtenstion7 = domainObject.PropExtenstion7 != null ? domainObject.PropExtenstion7 : "",
				PropExtenstion8 = domainObject.PropExtenstion8 != null ? domainObject.PropExtenstion8 : "",
				PropExtenstion9 = domainObject.PropExtenstion9 != null ? domainObject.PropExtenstion9 : "",
				PropExtenstion10 = domainObject.PropExtenstion10 != null ? domainObject.PropExtenstion10 : "",
				PropExtenstion11 = domainObject.PropExtenstion11 != null ? domainObject.PropExtenstion11 : "",
				PropExtenstion12 = domainObject.PropExtenstion12 != null ? domainObject.PropExtenstion12 : "",
				PropExtenstion13 = domainObject.PropExtenstion13 != null ? domainObject.PropExtenstion13 : "",
				PropExtenstion14 = domainObject.PropExtenstion14 != null ? domainObject.PropExtenstion14 : "",
				PropExtenstion15 = domainObject.PropExtenstion15 != null ? domainObject.PropExtenstion15 : "",
				PropExtenstion16 = domainObject.PropExtenstion16 != null ? domainObject.PropExtenstion16 : "",
				PropExtenstion17 = domainObject.PropExtenstion17 != null ? domainObject.PropExtenstion17 : "",
				PropExtenstion18 = domainObject.PropExtenstion18 != null ? domainObject.PropExtenstion18 : "",
				PropExtenstion19 = domainObject.PropExtenstion19 != null ? domainObject.PropExtenstion19 : "",
				PropExtenstion20 = domainObject.PropExtenstion20 != null ? domainObject.PropExtenstion20 : "",
				PropExtenstion21 = domainObject.PropExtenstion21 != null ? domainObject.PropExtenstion21 : "",
				PropExtenstion22 = domainObject.PropExtenstion22 != null ? domainObject.PropExtenstion22 : ""
			};
		}


		public static void ApplyChanges(this App_Data.PreviousInventory entity, PreviousInventory domainObject)
		{
			if (domainObject == null) return;
			entity.Id = domainObject.Id;
			entity.Uid = domainObject.Uid != null ? domainObject.Uid : "";
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
			entity.PropExtenstion1 = domainObject.PropExtenstion1 != null ? domainObject.PropExtenstion1 : "";
			entity.PropExtenstion2 = domainObject.PropExtenstion2 != null ? domainObject.PropExtenstion2 : "";
			entity.PropExtenstion3 = domainObject.PropExtenstion3 != null ? domainObject.PropExtenstion3 : "";
			entity.PropExtenstion4 = domainObject.PropExtenstion4 != null ? domainObject.PropExtenstion4 : "";
			entity.PropExtenstion5 = domainObject.PropExtenstion5 != null ? domainObject.PropExtenstion5 : "";
			entity.PropExtenstion6 = domainObject.PropExtenstion6 != null ? domainObject.PropExtenstion6 : "";
			entity.PropExtenstion7 = domainObject.PropExtenstion7 != null ? domainObject.PropExtenstion7 : "";
			entity.PropExtenstion8 = domainObject.PropExtenstion8 != null ? domainObject.PropExtenstion8 : "";
			entity.PropExtenstion9 = domainObject.PropExtenstion9 != null ? domainObject.PropExtenstion9 : "";
			entity.PropExtenstion10 = domainObject.PropExtenstion10 != null ? domainObject.PropExtenstion10 : "";
			entity.PropExtenstion11 = domainObject.PropExtenstion11 != null ? domainObject.PropExtenstion11 : "";
			entity.PropExtenstion12 = domainObject.PropExtenstion12 != null ? domainObject.PropExtenstion12 : "";
			entity.PropExtenstion13 = domainObject.PropExtenstion13 != null ? domainObject.PropExtenstion13 : "";
			entity.PropExtenstion14 = domainObject.PropExtenstion14 != null ? domainObject.PropExtenstion14 : "";
			entity.PropExtenstion15 = domainObject.PropExtenstion15 != null ? domainObject.PropExtenstion15 : "";
			entity.PropExtenstion16 = domainObject.PropExtenstion16 != null ? domainObject.PropExtenstion16 : "";
			entity.PropExtenstion17 = domainObject.PropExtenstion17 != null ? domainObject.PropExtenstion17 : "";
			entity.PropExtenstion18 = domainObject.PropExtenstion18 != null ? domainObject.PropExtenstion18 : "";
			entity.PropExtenstion19 = domainObject.PropExtenstion19 != null ? domainObject.PropExtenstion19 : "";
			entity.PropExtenstion20 = domainObject.PropExtenstion20 != null ? domainObject.PropExtenstion20 : "";
			entity.PropExtenstion21 = domainObject.PropExtenstion21 != null ? domainObject.PropExtenstion21 : "";
			entity.PropExtenstion22 = domainObject.PropExtenstion22 != null ? domainObject.PropExtenstion22 : "";
		}
	}
}
