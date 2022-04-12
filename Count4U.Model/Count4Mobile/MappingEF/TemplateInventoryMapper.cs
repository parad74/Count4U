using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4Mobile.MappingEF
{
	public static class TemplateInventoryMapper
	{

		public static TemplateInventory ToDomainObject(this App_Data.TemplateInventory entity)
		{
			if (entity == null) return null;
			return new TemplateInventory()
			{
				Id = entity.Id,
				Uid = entity.Uid != null ? entity.Uid : "",
				Level1Code = entity.Level1Code != null ? entity.Level1Code : "",
				Level2Code = entity.Level2Code != null ? entity.Level2Code : "",
				Level3Code = entity.Level3Code != null ? entity.Level3Code : "",
				Level4Code = entity.Level4Code != null ? entity.Level4Code : "",
				ItemCode = entity.ItemCode != null ? entity.ItemCode : "",
				QuantityExpected = entity.QuantityExpected != null ? entity.QuantityExpected : "",
				Tag = entity.Tag != null ? entity.Tag : "",
				Domain = entity.Domain != null ? entity.Domain : "",
			};
		}

		public static TemplateInventory ToSimpleDomainObject(this App_Data.TemplateInventory entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.TemplateInventory ToEntity(this TemplateInventory domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.TemplateInventory()
			{
				Id = domainObject.Id,
				Uid = domainObject.Uid != null ? domainObject.Uid : "",
				Level1Code = domainObject.Level1Code != null ? domainObject.Level1Code : "",
				Level2Code = domainObject.Level2Code != null ? domainObject.Level2Code : "",
				Level3Code = domainObject.Level3Code != null ? domainObject.Level3Code : "",
				Level4Code = domainObject.Level4Code != null ? domainObject.Level4Code : "",
				ItemCode = domainObject.ItemCode != null ? domainObject.ItemCode : "",
				QuantityExpected = domainObject.QuantityExpected != null ? domainObject.QuantityExpected : "",
				Tag = domainObject.Tag != null ? domainObject.Tag : "",
				Domain = domainObject.Domain != null ? domainObject.Domain : "",
			};
		}


		public static void ApplyChanges(this App_Data.TemplateInventory entity, TemplateInventory domainObject)
		{
			if (domainObject == null) return;
			entity.Id = domainObject.Id;
			entity.Uid = domainObject.Uid != null ? domainObject.Uid : "";
			entity.Level1Code = domainObject.Level1Code != null ? domainObject.Level1Code : "";
			entity.Level2Code = domainObject.Level2Code != null ? domainObject.Level2Code : "";
			entity.Level3Code = domainObject.Level3Code != null ? domainObject.Level3Code : "";
			entity.Level4Code = domainObject.Level4Code != null ? domainObject.Level4Code : "";
			entity.ItemCode = domainObject.ItemCode != null ? domainObject.ItemCode : "";
			entity.QuantityExpected = domainObject.QuantityExpected != null ? domainObject.QuantityExpected : "";
			entity.Tag = domainObject.Tag != null ? domainObject.Tag : "";
			entity.Domain = domainObject.Domain != null ? domainObject.Domain : "";
		}
	}
}
