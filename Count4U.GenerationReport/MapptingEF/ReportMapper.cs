using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.GenerationReport.MappingEF
{
	public static class ReportMapper
    {
        /// <summary>
        /// Конвертация в объект предметной области.
        /// 
        /// Converting to domain object.
        /// </summary>
        /// <param name="entity">
        /// Сущность базы данных.
        /// 
        /// Database entity.
        /// </param>
        /// <returns>
        /// Объект предметной области.
        /// 
        /// Domain object.
        /// </returns>
		public static Report ToReportMainDomainObject(this Count4U.Model.App_Data.Report entity)
        {
			if (entity == null) return null;
			return new Report()
			{
				ID = entity.ID,
				Description = entity.Description,
				Code = entity.Code,
				DomainContext = entity.DomainContext,
				TypeDS = entity.TypeDS,
				Path = entity.Path,
				FileName = entity.FileName,
				DomainType = entity.DomainType,
				Menu = Convert.ToBoolean(entity.Menu),
				MenuCaption = entity.MenuCaption,
				Tag = entity.Tag,
				Print = Convert.ToBoolean(entity.Print),
				Landscape = Convert.ToBoolean(entity.Landscape),
				NN = Convert.ToInt32(entity.NN),
				MenuCaptionLocalizationCode = entity.MenuCaptionLocalizationCode,
				IturAdvancedSearchMenu = Convert.ToBoolean(entity.IturAdvancedSearchMenu),
				InventProductAdvancedSearchMenu = Convert.ToBoolean(entity.InventProductAdvancedSearchMenu),
				InventProductSumAdvancedSearchMenu = Convert.ToBoolean(entity.InventProductSumAdvancedSearchMenu),
				CustomerSearchMenu = Convert.ToBoolean(entity.CustomerSearchMenu),
				BranchSearchMenu = Convert.ToBoolean(entity.BranchSearchMenu),
				InventorSearchMenu = Convert.ToBoolean(entity.InventorSearchMenu),
				AuditConfigSearchMenu = Convert.ToBoolean(entity.AuditConfigSearchMenu),
				IturSearchMenu = Convert.ToBoolean(entity.IturSearchMenu),
				InventProductSearchMenu = Convert.ToBoolean(entity.InventProductSearchMenu),
				LocationSearchMenu = Convert.ToBoolean(entity.LocationSearchMenu),
				ProductSearchMenu = Convert.ToBoolean(entity.ProductSearchMenu),
				CodeReport = entity.CodeReport,
				SupplierSearchMenu = Convert.ToBoolean(entity.SupplierSearchMenu),
				SectionSearchMenu = Convert.ToBoolean(entity.SectionSearchMenu),
				ItursPopupMenu = Convert.ToBoolean(entity.ItursPopupMenu),
				IturPopupMenu = Convert.ToBoolean(entity.IturPopupMenu),
				DocumentHeaderPopupMenu = Convert.ToBoolean(entity.DocumentHeaderPopupMenu),
				ItursListPopupMenu = Convert.ToBoolean(entity.ItursListPopupMenu)
			};
        }

		public static Report ToReportAuditDomainObject(this Count4U.Model.App_Data.AuditReport entity)
		{
			if (entity == null) return null;
			return new Report()
			{
				ID = entity.ID,
				Description = entity.Description,
				Code = entity.Code,
				DomainContext = entity.DomainContext,
				TypeDS = entity.TypeDS,
				Path = entity.Path,
				FileName = entity.FileName,
				DomainType = entity.DomainType ,
				Menu = Convert.ToBoolean(entity.Menu),
				MenuCaption = entity.MenuCaption,
				Tag = entity.Tag,
				Print =  Convert.ToBoolean(entity.Print),
				Landscape = Convert.ToBoolean(entity.Landscape),
				NN = Convert.ToInt32(entity.NN)	,
				MenuCaptionLocalizationCode = entity.MenuCaptionLocalizationCode,
				IturAdvancedSearchMenu = Convert.ToBoolean(entity.IturAdvancedSearchMenu),
				InventProductAdvancedSearchMenu = Convert.ToBoolean(entity.InventProductAdvancedSearchMenu),
				InventProductSumAdvancedSearchMenu = Convert.ToBoolean(entity.InventProductSumAdvancedSearchMenu),
				CustomerSearchMenu = Convert.ToBoolean(entity.CustomerSearchMenu),
				BranchSearchMenu = Convert.ToBoolean(entity.BranchSearchMenu),
				InventorSearchMenu = Convert.ToBoolean(entity.InventorSearchMenu),
				AuditConfigSearchMenu = Convert.ToBoolean(entity.AuditConfigSearchMenu),
				IturSearchMenu = Convert.ToBoolean(entity.IturSearchMenu),
				InventProductSearchMenu = Convert.ToBoolean(entity.InventProductSearchMenu),
				LocationSearchMenu = Convert.ToBoolean(entity.LocationSearchMenu),
				ProductSearchMenu = Convert.ToBoolean(entity.ProductSearchMenu)	  ,
				CodeReport = entity.CodeReport,
				SupplierSearchMenu = Convert.ToBoolean(entity.SupplierSearchMenu),
				SectionSearchMenu = Convert.ToBoolean(entity.SectionSearchMenu),
				ItursPopupMenu = Convert.ToBoolean(entity.ItursPopupMenu),
				IturPopupMenu = Convert.ToBoolean(entity.IturPopupMenu),
				DocumentHeaderPopupMenu = Convert.ToBoolean(entity.DocumentHeaderPopupMenu),
				ItursListPopupMenu = Convert.ToBoolean(entity.ItursListPopupMenu)

			};
		}

        /// <summary>
        /// Конвертация в упрощенный объект предметной области.
        /// 
        /// Converting to simple domain object.
        /// </summary>
        /// <param name="entity">
        /// Сущность базы данных.
        /// 
        /// Database entity.
        /// </param>
        /// <returns>
        /// Упрощенный объект предметной области.
        /// 
        /// Simple domain object.
        /// </returns>
		public static Report ToSimpleDomainObject(this Count4U.Model.App_Data.Report entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Конвертация в сущность базы данных.
        /// 
        /// Converting to database entity.
        /// </summary>
        /// <param name="domainObject">
        /// Объект предметной области.
        /// 
        /// Domain object.
        /// </param>
        /// <returns>Database entity.</returns>
		public static Count4U.Model.App_Data.Report ToMainReportEntity(this Report domainObject)
        {
			if (domainObject == null) return null;
			return new Count4U.Model.App_Data.Report()
            {
				ID = domainObject.ID,
				Description = domainObject.Description,
				Code = domainObject.Code,
				DomainContext = domainObject.DomainContext,
				TypeDS = domainObject.TypeDS,
				Path = domainObject.Path,
				FileName = domainObject.FileName,
				DomainType = domainObject.DomainType,
				Menu = domainObject.Menu,
				MenuCaption = domainObject.MenuCaption,
				Tag = domainObject.Tag,
				Print = domainObject.Print,
				Landscape = Convert.ToBoolean(domainObject.Landscape),
				NN = Convert.ToInt32(domainObject.NN),
				MenuCaptionLocalizationCode = domainObject.MenuCaptionLocalizationCode,
				IturAdvancedSearchMenu = Convert.ToBoolean(domainObject.IturAdvancedSearchMenu),
				InventProductAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductAdvancedSearchMenu),
				InventProductSumAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductSumAdvancedSearchMenu),
				CustomerSearchMenu = Convert.ToBoolean(domainObject.CustomerSearchMenu),
				BranchSearchMenu = Convert.ToBoolean(domainObject.BranchSearchMenu),
				InventorSearchMenu = Convert.ToBoolean(domainObject.InventorSearchMenu),
				AuditConfigSearchMenu = Convert.ToBoolean(domainObject.AuditConfigSearchMenu),
				IturSearchMenu = Convert.ToBoolean(domainObject.IturSearchMenu),
				InventProductSearchMenu = Convert.ToBoolean(domainObject.InventProductSearchMenu),
				LocationSearchMenu = Convert.ToBoolean(domainObject.LocationSearchMenu),
				ProductSearchMenu = Convert.ToBoolean(domainObject.ProductSearchMenu),
				CodeReport = domainObject.CodeReport  ,
				SupplierSearchMenu = Convert.ToBoolean(domainObject.SupplierSearchMenu),
				SectionSearchMenu = Convert.ToBoolean(domainObject.SectionSearchMenu),
				ItursPopupMenu = Convert.ToBoolean(domainObject.ItursPopupMenu),
				IturPopupMenu = Convert.ToBoolean(domainObject.IturPopupMenu),
				DocumentHeaderPopupMenu = Convert.ToBoolean(domainObject.DocumentHeaderPopupMenu),
				ItursListPopupMenu = Convert.ToBoolean(domainObject.ItursListPopupMenu)
			};
        }

		public static Count4U.Model.App_Data.AuditReport ToAuditReportEntity(this Report domainObject)
		{
			if (domainObject == null) return null;
			return new Count4U.Model.App_Data.AuditReport()
			{
				ID = domainObject.ID,
				Description = domainObject.Description,
				Code = domainObject.Code,
				DomainContext = domainObject.DomainContext,
				TypeDS = domainObject.TypeDS,
				Path = domainObject.Path,
				FileName = domainObject.FileName,
				DomainType = domainObject.DomainType,
				Menu = domainObject.Menu,
				MenuCaption = domainObject.MenuCaption,
				Tag = domainObject.Tag,
				Print = domainObject.Print,
				Landscape = Convert.ToBoolean(domainObject.Landscape),
				NN = Convert.ToInt32(domainObject.NN),
				MenuCaptionLocalizationCode = domainObject.MenuCaptionLocalizationCode,
				IturAdvancedSearchMenu = Convert.ToBoolean(domainObject.IturAdvancedSearchMenu),
				InventProductAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductAdvancedSearchMenu),
				InventProductSumAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductSumAdvancedSearchMenu),
				CustomerSearchMenu = Convert.ToBoolean(domainObject.CustomerSearchMenu),
				BranchSearchMenu = Convert.ToBoolean(domainObject.BranchSearchMenu),
				InventorSearchMenu = Convert.ToBoolean(domainObject.InventorSearchMenu),
				AuditConfigSearchMenu = Convert.ToBoolean(domainObject.AuditConfigSearchMenu),
				IturSearchMenu = Convert.ToBoolean(domainObject.IturSearchMenu),
				InventProductSearchMenu = Convert.ToBoolean(domainObject.InventProductSearchMenu),
				LocationSearchMenu = Convert.ToBoolean(domainObject.LocationSearchMenu),
				ProductSearchMenu = Convert.ToBoolean(domainObject.ProductSearchMenu),
				CodeReport = domainObject.CodeReport ,
				SupplierSearchMenu = Convert.ToBoolean(domainObject.SupplierSearchMenu),
				SectionSearchMenu = Convert.ToBoolean(domainObject.SectionSearchMenu),
				ItursPopupMenu = Convert.ToBoolean(domainObject.ItursPopupMenu),
				IturPopupMenu = Convert.ToBoolean(domainObject.IturPopupMenu),
				DocumentHeaderPopupMenu = Convert.ToBoolean(domainObject.DocumentHeaderPopupMenu),
				ItursListPopupMenu = Convert.ToBoolean(domainObject.ItursListPopupMenu)

			};
		}

        /// <summary>
        /// Применение изменений к сущности базы данных.
        /// 
        /// Apply changes to database entity.
        /// </summary>
        /// <param name="entity">
        /// Сущность базы данных.
        /// 
        /// Database entity.
        /// </param>
        /// <param name="domainObject">
        /// Объект предметной области.
        /// 
        /// Domain object.
        /// </param>
		public static void ApplyMainReportChanges(this Count4U.Model.App_Data.Report entity, Report domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Description = domainObject.Description;
			entity.Code = domainObject.Code;
			entity.DomainContext = domainObject.DomainContext;
			entity.TypeDS = domainObject.TypeDS;
			entity.Path = domainObject.Path;
			entity.FileName = domainObject.FileName;
			entity.DomainType = domainObject.DomainType;
			entity.Menu = domainObject.Menu;
			entity.MenuCaption = domainObject.MenuCaption;
			entity.Tag = domainObject.Tag;
			entity.Print = domainObject.Print;
			entity.Landscape = Convert.ToBoolean(domainObject.Landscape);
			entity.NN = Convert.ToInt32(domainObject.NN);
			entity.MenuCaptionLocalizationCode = domainObject.MenuCaptionLocalizationCode;
			entity.IturAdvancedSearchMenu = Convert.ToBoolean(domainObject.IturAdvancedSearchMenu);
			entity.InventProductAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductAdvancedSearchMenu);
			entity.InventProductSumAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductSumAdvancedSearchMenu);
			entity.CustomerSearchMenu = Convert.ToBoolean(domainObject.CustomerSearchMenu);
			entity.BranchSearchMenu = Convert.ToBoolean(domainObject.BranchSearchMenu);
			entity.InventorSearchMenu = Convert.ToBoolean(domainObject.InventorSearchMenu);
			entity.AuditConfigSearchMenu = Convert.ToBoolean(domainObject.AuditConfigSearchMenu);
			entity.IturSearchMenu = Convert.ToBoolean(domainObject.IturSearchMenu);
			entity.InventProductSearchMenu = Convert.ToBoolean(domainObject.InventProductSearchMenu);
			entity.LocationSearchMenu = Convert.ToBoolean(domainObject.LocationSearchMenu);
			entity.ProductSearchMenu = Convert.ToBoolean(domainObject.ProductSearchMenu);
			entity.CodeReport = domainObject.CodeReport;
			entity.SupplierSearchMenu = Convert.ToBoolean(domainObject.SupplierSearchMenu);
			entity.SectionSearchMenu = Convert.ToBoolean(domainObject.SectionSearchMenu);
			entity.ItursPopupMenu = Convert.ToBoolean(domainObject.ItursPopupMenu);
			entity.IturPopupMenu = Convert.ToBoolean(domainObject.IturPopupMenu);
			entity.DocumentHeaderPopupMenu = Convert.ToBoolean(domainObject.DocumentHeaderPopupMenu);
			entity.ItursListPopupMenu = Convert.ToBoolean(domainObject.ItursListPopupMenu);
		}

		public static void ApplyAuditReportChanges(this Count4U.Model.App_Data.AuditReport entity, Report domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Description = domainObject.Description;
			entity.Code = domainObject.Code;
			entity.DomainContext = domainObject.DomainContext;
			entity.TypeDS = domainObject.TypeDS;
			entity.Path = domainObject.Path;
			entity.FileName = domainObject.FileName;
			entity.DomainType = domainObject.DomainType;
			entity.Menu = domainObject.Menu;
			entity.MenuCaption = domainObject.MenuCaption;
			entity.Tag = domainObject.Tag;
			entity.Print = domainObject.Print;
			entity.Landscape = Convert.ToBoolean(domainObject.Landscape);
			entity.NN = Convert.ToInt32(domainObject.NN);
			entity.MenuCaptionLocalizationCode = domainObject.MenuCaptionLocalizationCode;
			entity.IturAdvancedSearchMenu = Convert.ToBoolean(domainObject.IturAdvancedSearchMenu);
			entity.InventProductAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductAdvancedSearchMenu);
			entity.InventProductSumAdvancedSearchMenu = Convert.ToBoolean(domainObject.InventProductSumAdvancedSearchMenu);
			entity.CustomerSearchMenu = Convert.ToBoolean(domainObject.CustomerSearchMenu);
			entity.BranchSearchMenu = Convert.ToBoolean(domainObject.BranchSearchMenu);
			entity.InventorSearchMenu = Convert.ToBoolean(domainObject.InventorSearchMenu);
			entity.AuditConfigSearchMenu = Convert.ToBoolean(domainObject.AuditConfigSearchMenu);
			entity.IturSearchMenu = Convert.ToBoolean(domainObject.IturSearchMenu);
			entity.InventProductSearchMenu = Convert.ToBoolean(domainObject.InventProductSearchMenu);
			entity.LocationSearchMenu = Convert.ToBoolean(domainObject.LocationSearchMenu);
			entity.ProductSearchMenu = Convert.ToBoolean(domainObject.ProductSearchMenu);
			entity.CodeReport = domainObject.CodeReport;
			entity.SupplierSearchMenu = Convert.ToBoolean(domainObject.SupplierSearchMenu);
			entity.SectionSearchMenu = Convert.ToBoolean(domainObject.SectionSearchMenu);
			entity.ItursPopupMenu = Convert.ToBoolean(domainObject.ItursPopupMenu);
			entity.IturPopupMenu = Convert.ToBoolean(domainObject.IturPopupMenu);
			entity.DocumentHeaderPopupMenu = Convert.ToBoolean(domainObject.DocumentHeaderPopupMenu);
			entity.ItursListPopupMenu = Convert.ToBoolean(domainObject.ItursListPopupMenu);

		}
    }
}
