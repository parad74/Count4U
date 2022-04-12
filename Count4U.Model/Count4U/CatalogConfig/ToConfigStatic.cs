using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Count4U.Model.Audit;

namespace Count4U.Model.Count4U
{
	public static class ToConfigStatic
	{
		public static InventorConfig ToInventorConfig(this AuditConfig auditConfig)
		{
			if (auditConfig == null) return null;
			InventorConfig inventorConfig = new InventorConfig()
			{
				//ID = item.ID,
				//Code = auditConfig.Code,
				InventorDate = auditConfig.InventorDate,
				Code = auditConfig.InventorCode,
				CreateDate = auditConfig.CreateDate,
				Description = auditConfig.Description,
				CustomerCode = auditConfig.CustomerCode,
				CustomerName = auditConfig.CustomerName,
				BranchCode = auditConfig.BranchCode,
				BranchName = auditConfig.BranchName,
				StatusInventorConfigCode = auditConfig.StatusInventorCode,
				DBPath = auditConfig.DBPath
			};
			return inventorConfig;
		}


		public static CatalogConfig ToCatalogConfig(this AuditConfig auditConfig)
		{
			if (auditConfig == null) return null;
			CatalogConfig catalogConfig = new CatalogConfig()
			{
				InventorCode = auditConfig.InventorCode,
				CreateDate = auditConfig.CreateDate,
				Description = auditConfig.Description,
				CustomerCode = auditConfig.CustomerCode,
				BranchCode = auditConfig.BranchCode
			};
			return catalogConfig;
		}
	}
}
