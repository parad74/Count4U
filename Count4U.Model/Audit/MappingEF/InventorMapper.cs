using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Audit.MappingEF
{
    public static class InventorMapper
    {
		public static Inventor ToDomainObject(this App_Data.Inventor entity)
		{
			if (entity == null) return null;
			return new Inventor()
			{
				ID = entity.ID,
				BranchCode = entity.BranchCode,
				Code = entity.Code,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				CustomerCode = entity.CustomerCode, 
				Description = entity.Description,
				ImportCatalogAdapterCode = entity.ImportCatalogAdapterCode,
				ImportIturAdapterCode = entity.ImportIturAdapterCode,
				ImportLocationAdapterCode = entity.ImportLocationAdapterCode,
				ImportCatalogParms = entity.ImportCatalogParms,
				ImportIturParms = entity.ImportIturParms,
				ImportLocationParms = entity.ImportLocationParms,
				InventorDate = entity.InventorDate != null ? Convert.ToDateTime(entity.InventorDate) : DateTime.Now,
				Name = entity.InventorDate.ToString(),
				StatusCode = entity.StatusInventorCode,
				DBPath = entity.DBPath,
				ImportSectionAdapterCode = entity.ImportSectionAdapterCode,
				UpdateCatalogAdapterCode = entity.UpdateCatalogAdapterCode,
				ImportPDAProviderCode = entity.ImportPDAProviderCode,
				ExportERPAdapterCode = entity.ExportERPAdapterCode,
				ImportSupplierAdapterCode = entity.ImportSupplierAdapterCode,
				Restore = entity.Restore,
				RestoreBit = Convert.ToBoolean(entity.RestoreBit),
				CompleteDate = entity.CompleteDate != null ? Convert.ToDateTime(entity.CompleteDate) : DateTime.Now,
				Manager = entity.Manager != null ? entity.Manager : "",
				PriceCode = entity.PriceCode != null ? entity.PriceCode : "",
				ReportName = entity.ReportName,
				ReportContext = entity.ReportContext,
				ReportDS = entity.ReportDS,
				ReportPath = entity.ReportPath,
				Print = Convert.ToBoolean(entity.Print)	 ,
				PDAType = entity.PDAType,
				MaintenanceType = entity.MaintenanceType,
				ProgramType = entity.ProgramType,
				ComplexStaticPath1 = entity.ComplexStaticPath1 != null ? entity.ComplexStaticPath1 : "",
				ComplexStaticPath2 = entity.ComplexStaticPath2 != null ? entity.ComplexStaticPath2 : "",
				ComplexStaticPath3 = entity.ComplexStaticPath3 != null ? entity.ComplexStaticPath3 : "",
				Host = entity.Host != null ? entity.Host : "",
				Port = entity.Port != null ? entity.Port : "",
				ImportCatalogPath = entity.ImportCatalogPath != null ? entity.ImportCatalogPath : "",
				ImportFromPdaPath = entity.ImportFromPdaPath != null ? entity.ImportFromPdaPath : "",
				ExportErpPath = entity.ExportErpPath != null ? entity.ExportErpPath : "",
				ExportPdaPath = entity.ExportPdaPath != null ? entity.ExportPdaPath : "",
				SendToOfficePath = entity.SendToOfficePath != null ? entity.SendToOfficePath : "",
				LastUpdatedCatalog = entity.LastUpdatedCatalog != null ? Convert.ToDateTime(entity.LastUpdatedCatalog) : DateTime.Now,
				Tag = entity.Tag != null ? entity.Tag : "",
				Tag1 = entity.Tag1 != null ? entity.Tag1 : "",
				Tag2 = entity.Tag2 != null ? entity.Tag2 : "",
				Tag3 = entity.Tag3 != null ? entity.Tag3 : "",
				ComplexAdapterCode = entity.ComplexAdapterCode != null ? entity.ComplexAdapterCode : "",
				ComplexAdapterParametr = entity.ComplexAdapterParametr != null ? entity.ComplexAdapterParametr : "",
				ComplexAdapterParametr1 = entity.ComplexAdapterParametr1 != null ? entity.ComplexAdapterParametr1 : "",
				ComplexAdapterParametr2 = entity.ComplexAdapterParametr2 != null ? entity.ComplexAdapterParametr2 : "",
				ComplexAdapterParametr3 = entity.ComplexAdapterParametr3 != null ? entity.ComplexAdapterParametr3 : "",
				ComplexAdapterParametrERPCode = entity.ComplexAdapterParametrERPCode != null ? entity.ComplexAdapterParametrERPCode : "",
				ComplexAdapterParametrInventorDateTime = entity.ComplexAdapterParametrInventorDateTime != null ? entity.ComplexAdapterParametrInventorDateTime : "",
				StatusAuditConfig = entity.StatusAuditConfig != null ? entity.StatusAuditConfig : "",
				ImportFamilyAdapterCode = entity.ImportFamilyAdapterCode != null ? entity.ImportFamilyAdapterCode : "",
				MaxCharacters = entity.MaxCharacters != null ? entity.MaxCharacters : "",
				MakatOrMakatOriginal = entity.MakatOrMakatOriginal != null ? entity.MakatOrMakatOriginal : "" ,
				ExportSectionAdapterCode = entity.ExportSectionAdapterCode != null ? entity.ExportSectionAdapterCode : "",
				ExportCatalogAdapterCode = entity.ExportCatalogAdapterCode != null ? entity.ExportCatalogAdapterCode : "",
				ParentCode = entity.ParentCode != null ? entity.ParentCode : "",
				SourceCode = entity.SourceCode != null ? entity.SourceCode : "",
				RootCode = entity.RootCode != null ? entity.RootCode : "",
				ProcessCode = entity.ProcessCode != null ? entity.ProcessCode : ""
			};
		}

		public static Inventor ToSimpleDomainObject(this App_Data.Inventor entity)
        {
            throw new NotImplementedException();
        }

    
		public static App_Data.Inventor ToEntity(this Inventor domainObject)
		{
			if (domainObject == null) return null;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 249) description = description.Substring(0, 249);
			string restore = domainObject.Restore != null ? domainObject.Restore : "";
			if (restore.Length > 99) restore = restore.Substring(0, 99);

			return new App_Data.Inventor()
			{
				ID = domainObject.ID,
				BranchCode = domainObject.BranchCode,
				Code = domainObject.Code,
				CreateDate = domainObject.CreateDate,
				CustomerCode = domainObject.CustomerCode,
				Description = description,
				ImportCatalogAdapterCode = domainObject.ImportCatalogAdapterCode,
				ImportIturAdapterCode = domainObject.ImportIturAdapterCode,
				ImportLocationAdapterCode = domainObject.ImportLocationAdapterCode,
				ImportCatalogParms = domainObject.ImportCatalogParms,
				ImportIturParms = domainObject.ImportIturParms,
				ImportLocationParms = domainObject.ImportLocationParms,
				InventorDate = domainObject.InventorDate,
				Name = domainObject.InventorDate.ToString(),
				StatusInventorCode = domainObject.StatusCode,
				DBPath = domainObject.DBPath,
				ImportSectionAdapterCode = domainObject.ImportSectionAdapterCode,
				UpdateCatalogAdapterCode = domainObject.UpdateCatalogAdapterCode,
				ImportPDAProviderCode = domainObject.ImportPDAProviderCode,
				ExportERPAdapterCode = domainObject.ExportERPAdapterCode,
				ImportSupplierAdapterCode = domainObject.ImportSupplierAdapterCode,
				Restore = restore,
				RestoreBit = Convert.ToBoolean(domainObject.RestoreBit),
				CompleteDate = domainObject.CompleteDate,
				Manager = domainObject.Manager != null ? domainObject.Manager : "",
				PriceCode = domainObject.PriceCode != null ? domainObject.PriceCode : ""  ,
				ReportName = domainObject.ReportName,
				ReportContext = domainObject.ReportContext,
				ReportDS = domainObject.ReportDS,
				ReportPath = domainObject.ReportPath ,
				Print = Convert.ToBoolean(domainObject.Print)  ,
				PDAType = domainObject.PDAType,
				MaintenanceType = domainObject.MaintenanceType,
				ProgramType = domainObject.ProgramType	  ,
				ComplexStaticPath1 = domainObject.ComplexStaticPath1 != null ? domainObject.ComplexStaticPath1 : "",
				ComplexStaticPath2 = domainObject.ComplexStaticPath2 != null ? domainObject.ComplexStaticPath2 : "",
				ComplexStaticPath3 = domainObject.ComplexStaticPath3 != null ? domainObject.ComplexStaticPath3 : "",
				Host = domainObject.Host != null ? domainObject.Host : "",
				Port = domainObject.Port != null ? domainObject.Port : "",
				ImportCatalogPath = domainObject.ImportCatalogPath != null ? domainObject.ImportCatalogPath : "",
				ImportFromPdaPath = domainObject.ImportFromPdaPath != null ? domainObject.ImportFromPdaPath : "",
				ExportErpPath = domainObject.ExportErpPath != null ? domainObject.ExportErpPath : "",
				ExportPdaPath = domainObject.ExportPdaPath != null ? domainObject.ExportPdaPath : "",
				SendToOfficePath = domainObject.SendToOfficePath != null ? domainObject.SendToOfficePath : "",
				LastUpdatedCatalog = domainObject.LastUpdatedCatalog != null ? Convert.ToDateTime(domainObject.LastUpdatedCatalog) : DateTime.Now,
				Tag = domainObject.Tag != null ? domainObject.Tag : "",
				Tag1 = domainObject.Tag1 != null ? domainObject.Tag1 : "",
				Tag2 = domainObject.Tag2 != null ? domainObject.Tag2 : "",
				Tag3 = domainObject.Tag3 != null ? domainObject.Tag3 : "",
				ComplexAdapterCode = domainObject.ComplexAdapterCode != null ? domainObject.ComplexAdapterCode : "",
				ComplexAdapterParametr = domainObject.ComplexAdapterParametr != null ? domainObject.ComplexAdapterParametr : "",
				ComplexAdapterParametr1 = domainObject.ComplexAdapterParametr1 != null ? domainObject.ComplexAdapterParametr1 : "",
				ComplexAdapterParametr2 = domainObject.ComplexAdapterParametr2 != null ? domainObject.ComplexAdapterParametr2 : "",
				ComplexAdapterParametr3 = domainObject.ComplexAdapterParametr3 != null ? domainObject.ComplexAdapterParametr3 : "",
				ComplexAdapterParametrERPCode = domainObject.ComplexAdapterParametrERPCode != null ? domainObject.ComplexAdapterParametrERPCode : "",
				ComplexAdapterParametrInventorDateTime = domainObject.ComplexAdapterParametrInventorDateTime != null ? domainObject.ComplexAdapterParametrInventorDateTime : "",
				StatusAuditConfig = domainObject.StatusAuditConfig != null ? domainObject.StatusAuditConfig : "",
				ImportFamilyAdapterCode = domainObject.ImportFamilyAdapterCode != null ? domainObject.ImportFamilyAdapterCode : "",
				MaxCharacters = domainObject.MaxCharacters != null ? domainObject.MaxCharacters : "",
				MakatOrMakatOriginal = domainObject.MakatOrMakatOriginal != null ? domainObject.MakatOrMakatOriginal : ""	,
				ExportSectionAdapterCode = domainObject.ExportSectionAdapterCode != null ? domainObject.ExportSectionAdapterCode : "",
				ExportCatalogAdapterCode = domainObject.ExportCatalogAdapterCode != null ? domainObject.ExportCatalogAdapterCode : "",
				ParentCode = domainObject.ParentCode != null ? domainObject.ParentCode : "",
				SourceCode = domainObject.SourceCode != null ? domainObject.SourceCode : "",
				RootCode = domainObject.RootCode != null ? domainObject.RootCode : "",
				ProcessCode = domainObject.ProcessCode != null ? domainObject.ProcessCode : ""

			};

		}

		public static App_Data.InventorConfig ToInventorConfigEntity(this Inventor domainObject)
		{
			if (domainObject == null) return null;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 200) description = description.Substring(0, 200);
			return new App_Data.InventorConfig()
			{
				TypeObject = DomainTypeEnum.InventorConfig.ToString(),
				Code = domainObject.Code, 
				InventorCode = domainObject.Code, 
				CreateDate = domainObject.CreateDate,
				CustomerCode = domainObject.CustomerCode,
				BranchCode = domainObject.BranchCode,
				Description = "Inventor:" + domainObject.Name + "[" + description + "]",
				InventorDate = domainObject.InventorDate,
				StatusInventorCode = domainObject.StatusCode,
				DBPath = domainObject.DBPath,
				BranchName = "",
				CustomerName = "",
				IsDirty = false,
				ModifyDate = DateTime.Now,
				//CompleteDate = DateTime.Now,
				//Manager = domainObject.Manager
			};
		}

		public static void ApplyChanges(this App_Data.Inventor entity, Inventor domainObject)
		{
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 249) description = description.Substring(0, 249);
			string restore = domainObject.Restore != null ? domainObject.Restore : "";
			if (restore.Length > 99) restore = restore.Substring(0, 99);

			if (domainObject == null) return;
			//entity.ID = domainObject.ID;
			entity.BranchCode = domainObject.BranchCode;
			entity.Code = domainObject.Code;
			entity.CreateDate = domainObject.CreateDate;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.Description = description;
			entity.ImportCatalogAdapterCode = domainObject.ImportCatalogAdapterCode;
			entity.ImportLocationAdapterCode = domainObject.ImportLocationAdapterCode;
			entity.ImportIturAdapterCode = domainObject.ImportIturAdapterCode;
			entity.ImportCatalogParms = domainObject.ImportCatalogParms;
			entity.ImportIturParms = domainObject.ImportIturParms;
			entity.ImportLocationParms = domainObject.ImportLocationParms;
			entity.InventorDate = domainObject.InventorDate;
			entity.Name = domainObject.InventorDate.ToString();
			entity.StatusInventorCode = domainObject.StatusCode;
			entity.DBPath = domainObject.DBPath;
			entity.ImportSectionAdapterCode = domainObject.ImportSectionAdapterCode;
			entity.UpdateCatalogAdapterCode = domainObject.UpdateCatalogAdapterCode;
			entity.ImportPDAProviderCode = domainObject.ImportPDAProviderCode;
			entity.ExportERPAdapterCode = domainObject.ExportERPAdapterCode;
			entity.ImportSupplierAdapterCode = domainObject.ImportSupplierAdapterCode;
			entity.Restore = restore;
			entity.RestoreBit = Convert.ToBoolean(domainObject.RestoreBit);
			entity.CompleteDate = domainObject.CompleteDate;
			entity.Manager = domainObject.Manager != null ? domainObject.Manager : "";
			entity.PriceCode = domainObject.PriceCode != null ? domainObject.PriceCode : "";
			entity.ReportName = domainObject.ReportName;
			entity.ReportContext = domainObject.ReportContext;
			entity.ReportDS = domainObject.ReportDS;
			entity.ReportPath = domainObject.ReportPath;
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
			entity.LastUpdatedCatalog = domainObject.LastUpdatedCatalog != null ? Convert.ToDateTime(domainObject.LastUpdatedCatalog) : DateTime.Now;
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
			entity.ExportSectionAdapterCode = domainObject.ExportSectionAdapterCode != null ? domainObject.ExportSectionAdapterCode : "";
			entity.ExportCatalogAdapterCode = domainObject.ExportCatalogAdapterCode != null ? domainObject.ExportCatalogAdapterCode : "";
			entity.ParentCode = domainObject.ParentCode != null ? domainObject.ParentCode : "";
			entity.SourceCode = domainObject.SourceCode != null ? domainObject.SourceCode : "";
			entity.RootCode = domainObject.RootCode != null ? domainObject.RootCode : "";
			entity.ProcessCode = domainObject.ProcessCode != null ? domainObject.ProcessCode : "";
		}

	
    }
}
