using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.MappingEF
{
	public static class BranchMapper
	{
		public static Branch ToDomainObject(this App_Data.Branch entity)
		{
			if (entity == null) return null;
			return new Branch()
			{
				ID = entity.ID,
				Address = entity.Address,
				Code = entity.Code,
				ContactPerson = entity.ContactPerson,
				CustomerCode = entity.CustomerCode,
				Description = entity.Description,
				Fax = entity.Fax,
				//LogoFile = entity.LogoFile, 
				Mail = entity.Mail,
				Name = entity.Name,
				Phone = entity.Phone,
				DBPath = entity.DBPath,
				ImportCatalogProviderCode = entity.ImportCatalogProviderCode,
				ImportIturProviderCode = entity.ImportIturProviderCode,
				ImportLocationProviderCode = entity.ImportLocationProviderCode,
				ImportPDAProviderCode = entity.ImportPDAProviderCode,
				ImportCatalogAdapterParms = entity.ImportCatalogAdapterParms,
				ImportIturAdapterParms = entity.ImportIturAdapterParms,
				ImportLocationAdapterParms = entity.ImportLocationAdapterParms,
				ImportPDAAdapterParms = entity.ImportPDAAdapterParms,
				BranchCodeLocal = entity.BranchCodeLocal,
				BranchCodeERP = entity.BranchCodeERP,
				ExportCatalogAdapterCode = entity.ExportCatalogAdapterCode,
				ExportIturAdapterCode = entity.ExportIturAdapterCode,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				ModifyDate = entity.ModifyDate != null ? Convert.ToDateTime(entity.ModifyDate) : DateTime.Now,
				ReportContext = entity.ReportContext,
				ReportDS = entity.ReportDS,
				ReportName = entity.ReportName,
				ReportPath = entity.ReportPath,
				ImportSectionAdapterCode = entity.ImportSectionAdapterCode,
				ExportSectionAdapterCode = entity.ExportSectionAdapterCode,
				UpdateCatalogAdapterCode = entity.UpdateCatalogAdapterCode,
				ExportERPAdapterCode = entity.ExportERPAdapterCode,
				ImportSupplierAdapterCode = entity.ImportSupplierAdapterCode,
				Restore = entity.Restore,
				RestoreBit = Convert.ToBoolean(entity.RestoreBit),
				PriceCode = entity.PriceCode != null ? entity.PriceCode : "",
				Print = Convert.ToBoolean(entity.Print),
				PDAType = entity.PDAType,
				MaintenanceType = entity.MaintenanceType,
				ProgramType = entity.ProgramType,
				ComplexStaticPath1 = entity.ComplexStaticPath1 != null ? entity.ComplexStaticPath1 : "" 	,
				ComplexStaticPath2 = entity.ComplexStaticPath2 != null ? entity.ComplexStaticPath2 : "" 	,
				ComplexStaticPath3 = entity.ComplexStaticPath3 != null ? entity.ComplexStaticPath3 : "" 	,
				Host = entity.Host != null ? entity.Host : "" 	,
				Port = entity.Port != null ? entity.Port : "" 	,
				ImportCatalogPath = entity.ImportCatalogPath != null ? entity.ImportCatalogPath : "" 	,
				ImportFromPdaPath = entity.ImportFromPdaPath != null ? entity.ImportFromPdaPath : "" 	,
				ExportErpPath = entity.ExportErpPath != null ? entity.ExportErpPath : "" 	,
				ExportPdaPath = entity.ExportPdaPath != null ? entity.ExportPdaPath : "" 	,
				SendToOfficePath = entity.SendToOfficePath != null ? entity.SendToOfficePath : "" 	,
				LastUpdatedCatalog = entity.LastUpdatedCatalog != null ? Convert.ToDateTime(entity.LastUpdatedCatalog) : DateTime.Now,
				Tag = entity.Tag != null ? entity.Tag : "" ,
				Tag1 = entity.Tag1 != null ? entity.Tag1 : "" 	,
				Tag2 = entity.Tag2 != null ? entity.Tag2 : "" 	,
				Tag3 = entity.Tag3 != null ? entity.Tag3 : "" 	,
				ComplexAdapterCode = entity.ComplexAdapterCode != null ? entity.ComplexAdapterCode : "" 	,
				ComplexAdapterParametr = entity.ComplexAdapterParametr != null ? entity.ComplexAdapterParametr : "" 	,
				ComplexAdapterParametr1 = entity.ComplexAdapterParametr1 != null ? entity.ComplexAdapterParametr1 : "" 	,
				ComplexAdapterParametr2 = entity.ComplexAdapterParametr2 != null ? entity.ComplexAdapterParametr2 : "" 	,
				ComplexAdapterParametr3 = entity.ComplexAdapterParametr3 != null ? entity.ComplexAdapterParametr3 : "" 	,
				ComplexAdapterParametrERPCode = entity.ComplexAdapterParametrERPCode != null ? entity.ComplexAdapterParametrERPCode : "" 	,
				ComplexAdapterParametrInventorDateTime = entity.ComplexAdapterParametrInventorDateTime != null ? entity.ComplexAdapterParametrInventorDateTime : "" 	,
				StatusAuditConfig = entity.StatusAuditConfig != null ? entity.StatusAuditConfig : "" 	,
				ImportFamilyAdapterCode = entity.ImportFamilyAdapterCode != null ? entity.ImportFamilyAdapterCode : "" 	,
				MaxCharacters = entity.MaxCharacters != null ? entity.MaxCharacters : "" 	,
				MakatOrMakatOriginal = entity.MakatOrMakatOriginal != null ? entity.MakatOrMakatOriginal : ""
			};
		}

		
		public static Branch ToSimpleDomainObject(this App_Data.Branch entity)
		{
			throw new NotImplementedException();
		}

	
		public static App_Data.Branch ToEntity(this Branch domainObject)
		{
			if (domainObject == null) return null;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 249) description = description.Substring(0, 249);
			string restore = domainObject.Restore != null ? domainObject.Restore : "";
			if (restore.Length > 99) restore = restore.Substring(0, 99);

			return new App_Data.Branch()
			{
				ID = domainObject.ID,
				Address = domainObject.Address,
				Code = domainObject.Code,
				ContactPerson = domainObject.ContactPerson,
				CustomerCode = domainObject.CustomerCode,
				Description = description,
				Fax = domainObject.Fax,
				//LogoFile = domainObject.LogoFile, 
				Mail = domainObject.Mail,
				Name = domainObject.Name,
				Phone = domainObject.Phone,
				DBPath = domainObject.DBPath,
				ImportCatalogProviderCode = domainObject.ImportCatalogProviderCode,
				ImportIturProviderCode = domainObject.ImportIturProviderCode,
				ImportLocationProviderCode = domainObject.ImportLocationProviderCode,
				ImportPDAProviderCode = domainObject.ImportPDAProviderCode,
				ImportCatalogAdapterParms = domainObject.ImportCatalogAdapterParms,
				ImportIturAdapterParms = domainObject.ImportIturAdapterParms,
				ImportLocationAdapterParms = domainObject.ImportLocationAdapterParms,
				ImportPDAAdapterParms = domainObject.ImportPDAAdapterParms,
				BranchCodeLocal = domainObject.BranchCodeLocal,
				BranchCodeERP = domainObject.BranchCodeERP,
				ExportCatalogAdapterCode = domainObject.ExportCatalogAdapterCode,
				ExportIturAdapterCode = domainObject.ExportIturAdapterCode,
				CreateDate = domainObject.CreateDate,
				ModifyDate = domainObject.ModifyDate,
				ReportPath = domainObject.ReportPath,
				ReportContext = domainObject.ReportContext,
				ReportDS = domainObject.ReportDS,
				ReportName = domainObject.ReportName,
				ImportSectionAdapterCode = domainObject.ImportSectionAdapterCode,
				ExportSectionAdapterCode = domainObject.ExportSectionAdapterCode,
				UpdateCatalogAdapterCode = domainObject.UpdateCatalogAdapterCode,
				ExportERPAdapterCode = domainObject.ExportERPAdapterCode,
				ImportSupplierAdapterCode = domainObject.ImportSupplierAdapterCode,
				Restore = domainObject.Restore,
				RestoreBit = Convert.ToBoolean(domainObject.RestoreBit),
				PriceCode = domainObject.PriceCode != null ? domainObject.PriceCode : "",
				Print = Convert.ToBoolean(domainObject.Print)  ,
				PDAType = domainObject.PDAType,
				MaintenanceType = domainObject.MaintenanceType,
				ProgramType = domainObject.ProgramType		  ,
				ComplexStaticPath1 = domainObject.ComplexStaticPath1 != null ? domainObject.ComplexStaticPath1 : "" 	,
				ComplexStaticPath2 = domainObject.ComplexStaticPath2 != null ? domainObject.ComplexStaticPath2 : "" 	,
				ComplexStaticPath3 = domainObject.ComplexStaticPath3 != null ? domainObject.ComplexStaticPath3 : "" 	,
				Host = domainObject.Host != null ? domainObject.Host : "" 	,
				Port = domainObject.Port != null ? domainObject.Port : "" 	,
				ImportCatalogPath = domainObject.ImportCatalogPath != null ? domainObject.ImportCatalogPath : "" 	,
				ImportFromPdaPath = domainObject.ImportFromPdaPath != null ? domainObject.ImportFromPdaPath : "" 	,
				ExportErpPath = domainObject.ExportErpPath != null ? domainObject.ExportErpPath : "" 	,
				ExportPdaPath = domainObject.ExportPdaPath != null ? domainObject.ExportPdaPath : "" 	,
				SendToOfficePath = domainObject.SendToOfficePath != null ? domainObject.SendToOfficePath : "" 	,
				LastUpdatedCatalog = domainObject.LastUpdatedCatalog != null ? domainObject.LastUpdatedCatalog : DateTime.Now,
				Tag = domainObject.Tag != null ? domainObject.Tag : ""	 ,
				Tag1 = domainObject.Tag1 != null ? domainObject.Tag1 : "" 	,
				Tag2 = domainObject.Tag2 != null ? domainObject.Tag2 : "" 	,
				Tag3 = domainObject.Tag3 != null ? domainObject.Tag3 : "" 	,
				ComplexAdapterCode = domainObject.ComplexAdapterCode != null ? domainObject.ComplexAdapterCode : "" 	,
				ComplexAdapterParametr = domainObject.ComplexAdapterParametr != null ? domainObject.ComplexAdapterParametr : "" 	,
				ComplexAdapterParametr1 = domainObject.ComplexAdapterParametr1 != null ? domainObject.ComplexAdapterParametr1 : "" 	,
				ComplexAdapterParametr2 = domainObject.ComplexAdapterParametr2 != null ? domainObject.ComplexAdapterParametr2 : "" 	,
				ComplexAdapterParametr3 = domainObject.ComplexAdapterParametr3 != null ? domainObject.ComplexAdapterParametr3 : "" 	,
				ComplexAdapterParametrERPCode = domainObject.ComplexAdapterParametrERPCode != null ? domainObject.ComplexAdapterParametrERPCode : "" 	,
				ComplexAdapterParametrInventorDateTime = domainObject.ComplexAdapterParametrInventorDateTime != null ? domainObject.ComplexAdapterParametrInventorDateTime : "" 	,
				StatusAuditConfig = domainObject.StatusAuditConfig != null ? domainObject.StatusAuditConfig : "" 	,
				ImportFamilyAdapterCode = domainObject.ImportFamilyAdapterCode != null ? domainObject.ImportFamilyAdapterCode : "" 	,
				MaxCharacters = domainObject.MaxCharacters != null ? domainObject.MaxCharacters : "" 	,
				MakatOrMakatOriginal = domainObject.MakatOrMakatOriginal != null ? domainObject.MakatOrMakatOriginal : ""

  			};
		}

		public static App_Data.InventorConfig ToInventorConfigEntity(this Branch domainObject)
		{
			if (domainObject == null) return null;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 200) description = description.Substring(0, 200);
			return new App_Data.InventorConfig()
			{
				TypeObject = DomainTypeEnum.InventorConfig.ToString(),
				Code = domainObject.Code,
				CustomerCode = domainObject.CustomerCode,
				BranchCode = domainObject.Code,
				DBPath = domainObject.DBPath,
				Description = DomainTypeEnum.Branch.ToString() + ": " + domainObject.Name + "[" + description + "]",
				BranchName = domainObject.Name,
				CustomerName = "",
				IsDirty = false,
				ModifyDate = DateTime.Now,
				InventorDate = DateTime.Now,
				CreateDate = DateTime.Now
			};
		}


		public static void ApplyChanges(this App_Data.Branch entity, Branch domainObject)
		{
			if (domainObject == null) return;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 249) description = description.Substring(0, 249);
			string restore = domainObject.Restore != null ? domainObject.Restore : "";
			if (restore.Length > 99) restore = restore.Substring(0, 99);

			//entity.ID = domainObject.ID;
			entity.Address = domainObject.Address;
			entity.Code = domainObject.Code;
			entity.ContactPerson = domainObject.ContactPerson;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.Description = description;
			entity.Fax = domainObject.Fax;
			//entity.LogoFile = domainObject.LogoFile;
			entity.Mail = domainObject.Mail;
			entity.Name = domainObject.Name;
			entity.Phone = domainObject.Phone;
			entity.DBPath = domainObject.DBPath;
			entity.ImportCatalogProviderCode = domainObject.ImportCatalogProviderCode;
			entity.ImportIturProviderCode = domainObject.ImportIturProviderCode;
			entity.ImportLocationProviderCode = domainObject.ImportLocationProviderCode;
			entity.ImportPDAProviderCode = domainObject.ImportPDAProviderCode;
			entity.ImportCatalogAdapterParms = domainObject.ImportCatalogAdapterParms;
			entity.ImportIturAdapterParms = domainObject.ImportIturAdapterParms;
			entity.ImportLocationAdapterParms = domainObject.ImportLocationAdapterParms;
			entity.ImportPDAAdapterParms = domainObject.ImportPDAAdapterParms;
			entity.BranchCodeLocal = domainObject.BranchCodeLocal;
			entity.BranchCodeERP = domainObject.BranchCodeERP;
			entity.ExportCatalogAdapterCode = domainObject.ExportCatalogAdapterCode;
			entity.ExportIturAdapterCode = domainObject.ExportIturAdapterCode;
			entity.CreateDate = domainObject.CreateDate;
			entity.ModifyDate = domainObject.ModifyDate;
			entity.ReportPath = domainObject.ReportPath;
			entity.ReportContext = domainObject.ReportContext;
			entity.ReportDS = domainObject.ReportDS;
			entity.ReportName = domainObject.ReportName;
			entity.ImportSectionAdapterCode = domainObject.ImportSectionAdapterCode;
			entity.ExportSectionAdapterCode = domainObject.ExportSectionAdapterCode;
			entity.UpdateCatalogAdapterCode = domainObject.UpdateCatalogAdapterCode;
			entity.ExportERPAdapterCode = domainObject.ExportERPAdapterCode;
			entity.ImportSupplierAdapterCode = domainObject.ImportSupplierAdapterCode;
			entity.Restore = restore;
			entity.RestoreBit = Convert.ToBoolean(domainObject.RestoreBit);
			entity.PriceCode = domainObject.PriceCode != null ? domainObject.PriceCode : "";
			entity.Print = Convert.ToBoolean(domainObject.Print);
			entity.PDAType = domainObject.PDAType;
			entity.MaintenanceType = domainObject.MaintenanceType;
			entity.ProgramType = domainObject.ProgramType;
			entity.ComplexStaticPath1 = domainObject.ComplexStaticPath1 != null ? domainObject.ComplexStaticPath1 : "";
			entity.ComplexStaticPath2 = domainObject.ComplexStaticPath2 != null ? domainObject.ComplexStaticPath2 : "";
			entity.ComplexStaticPath3 = domainObject.ComplexStaticPath3 != null ? domainObject.ComplexStaticPath3 : "";
			entity.Host = domainObject.Host != null ? domainObject.Host : "";
			entity.Port = domainObject.Port != null ? domainObject.Port : "";
			entity.ImportCatalogPath = domainObject.ImportCatalogPath != null ? domainObject.ImportCatalogPath : "";
			entity.ImportFromPdaPath = domainObject.ImportFromPdaPath != null ? domainObject.ImportFromPdaPath : "";
			entity.ExportErpPath = domainObject.ExportErpPath != null ? domainObject.ExportErpPath : "";
			entity.ExportPdaPath = domainObject.ExportPdaPath != null ? domainObject.ExportPdaPath : "";
			entity.SendToOfficePath = domainObject.SendToOfficePath != null ? domainObject.SendToOfficePath : "";
			entity.LastUpdatedCatalog = domainObject.LastUpdatedCatalog != null ? domainObject.LastUpdatedCatalog : DateTime.Now;
			entity.Tag = domainObject.Tag != null ? domainObject.Tag : "";
			entity.Tag1 = domainObject.Tag1 != null ? domainObject.Tag1 : "";
			entity.Tag2 = domainObject.Tag2 != null ? domainObject.Tag2 : "";
			entity.Tag3 = domainObject.Tag3 != null ? domainObject.Tag3 : "";
			entity.ComplexAdapterCode = domainObject.ComplexAdapterCode != null ? domainObject.ComplexAdapterCode : "";
			entity.ComplexAdapterParametr = domainObject.ComplexAdapterParametr != null ? domainObject.ComplexAdapterParametr : "";
			entity.ComplexAdapterParametr1 = domainObject.ComplexAdapterParametr1 != null ? domainObject.ComplexAdapterParametr1 : "";
			entity.ComplexAdapterParametr2 = domainObject.ComplexAdapterParametr2 != null ? domainObject.ComplexAdapterParametr2 : "";
			entity.ComplexAdapterParametr3 = domainObject.ComplexAdapterParametr3 != null ? domainObject.ComplexAdapterParametr3 : "";
			entity.ComplexAdapterParametrERPCode = domainObject.ComplexAdapterParametrERPCode != null ? domainObject.ComplexAdapterParametrERPCode : "";
			entity.ComplexAdapterParametrInventorDateTime = domainObject.ComplexAdapterParametrInventorDateTime != null ? domainObject.ComplexAdapterParametrInventorDateTime : "";
			entity.StatusAuditConfig = domainObject.StatusAuditConfig != null ? domainObject.StatusAuditConfig : "";
			entity.ImportFamilyAdapterCode = domainObject.ImportFamilyAdapterCode != null ? domainObject.ImportFamilyAdapterCode : "";
			entity.MaxCharacters = domainObject.MaxCharacters != null ? domainObject.MaxCharacters : "";
			entity.MakatOrMakatOriginal = domainObject.MakatOrMakatOriginal != null ? domainObject.MakatOrMakatOriginal : "";
		}
	}
}
