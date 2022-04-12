using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Common
{
    public static class RegionNames
    {
        public static string ApplicationWindow = "ApplicationWindow";
        public static string ApplicationStrip = "ApplicationStrip";
        public static string ApplicationBottom = "ApplicationBottom";

        //iturs dashboard
        
        public static string MainRegionCenter = "MainRegionCenter";
        public static string MainRegionLeft1 = "MainRegionLeft1";
        public static string MainRegionLeft2 = "MainRegionLeft2";
        public static string MainRegionLeft3 = "MainRegionLeft3";
        public static string MainRegionLeft4 = "MainRegionLeft4";
        public static string MainRegionRight = "MainRegionRight";        
        //

        public static string ModalWindowRegion = "ModalWindowRegion";

        public static string DashboardPartMenu = "DashboardPartMenu";
        public static string DashboardPartCatalogInfoForCBI = "DashboardPartCatalogInfoForCBI";
        public static string DashboardPartReports = "DashboardPartReports";
        public static string DashboardPartBranches = "DashboardPartBranches";
        public static string DashboardPartAllBranches = "DashboardPartAllBranches";
        public static string DashboardPartLastInventors = "DashboardPartLastInventors";

        public static string DashboardPartLocation = "DashboardPartLocation";
        public static string DashboardPartPda = "DashboardPartPda";

        public static string DashboardPartInventProduct = "DashboardPartInventProduct";

        public static string DashboardPartFromPda = "DashboardPartFromPda";
        public static string DashboardPartIturim = "DashboardPartIturim";
        public static string DashboardPartInventorStatus = "DashboardPartInventorStatus";
        public static string DashboardPartLastCustomers = "DashboardPartLastCustomers";
        public static string DashboardPartLastCustomersBuild = "DashboardPartLastCustomersBuild";
        public static string DashboardPartSection = "DashboardPartSection";
        public static string DashboardPartSupplier = "DashboardPartSupplier";
		public static string DashboardPartFamily = "DashboardPartFamily";
        public static string DashboardPartPlanogram = "DashboardPartPlanogram";
        public static string DashboardPartStatistic = "DashboardPartStatistic";

        public static string DashboardPartCustomer = "DashboardPartCustomer";
        public static string DashboardPartBranch = "DashboardPartBranch";
        public static string DashboardPartInventor = "DashboardPartInventor";
        public static string DashboardPartHome = "DashboardPartHome";

        public static string DashboardParthInventProductSimple = "DashboardParthInventProductSimple";
        public static string DashboardParthInventProductSum = "DashboardParthInventProductSum";

        public static string UserSettings = "UserSettings";
        public static string PathSettings = "PathSettings";
        public static string SqlScriptSettings = "SqlScriptSettings";

        public static string ZipExport = "ZipExport";
        public static string ZipImport = "ZipImport";
		public static string Process = "Process";    
		

        //back-forward
        public static string InventProductListDetailsBackForward = "InventProductListDetailsBackForward";
        public static string MainRegionBackForward = "MainRegionBackForward";
        public static string MaskListBackForward = "MaskListBackForward";
        public static string ReportTemplateBackForward = "ReportTemplateIturBackForward";
        public static string ReportFavoritesBackForward = "ReportFavoritesBackForward";
        public static string ImportBackForward = "ImportBackForward";
        public static string ImportPdaBackForward = "ImportPdaBackForward";
		public static string ImportComplexForward = "ImportComplexForward";
		public static string ImportBranchComplexForward = "ImportBranchComplexForward";
		public static string ImportCustomerComplexForward = "ImportCustomerComplexForward";
        public static string ExportBackForward = "ExportBackForward";
        public static string ExportErpBackForward = "ExportErpBackForward";
        public static string MainDashboardBackForward = "MainDashboardBackForward";
        public static string CustomerDashboardBackForward = "CustomerDashboardBackForward";
        public static string BranchDashboardBackForward = "BranchDashboardBackForward";
        public static string InventorDashboardBackForward = "InventorDashboardBackForward";
        public static string IturAddEditDeleteBackForward = "IturAddEditDeleteBackForward";
        public static string LocationAddEditDeleteBackForward = "LocationAddEditDeleteBackForward";
        public static string CatalogFormBackForward = "CatalogFormBackForward";
        public static string InventProductBackForward = "InventProductBackForward";
        public static string AdapterLinkBackForward = "AdapterLinkBackForward";
        public static string SectionAddEditDeleteBackForward = "SectionAddEditDeleteBackForward";
        public static string SupplierAddEditDeleteBackForward = "SupplierAddEditDeleteBackForward";
		public static string FamilyAddEditDeleteBackForward = "FamilyAddEditDeleteBackForward";
        public static string InventProductListSimpleBackForward = "InventProductListSimpleBackForward";
        public static string InventProductListSumBackForward = "InventProductListSumBackForward";
        public static string PackBackForward = "PackBackForward";
        public static string UnpackBackForward = "UnpackBackForward";
        public static string PlanBasementBackForward = "PlanBasementBackForward";
        public static string PlanogramAddEditDeleteBackForward = "PlanogramAddEditDeleteBackForward";

        public static string CustomerChooseBackForward = "CustomerChooseBackForward";
        public static string BranchChooseBackForward = "BranchChooseBackForward";
        public static string InventorChooseBackForward = "InventorChooseBackForward";

        public static string ImportByModule = "ImportByModule";
		public static string ExportAdapterView = "ExportPdaAdapter";
        public static string ExportByModule = "ExportByModule";
        public static string ExportErpByModule = "ExportErpByModule";

		public static string ConfigAdapterSettingView = "ConfigAdapterSettingView";
		public static string AutoGenerateResultSettingsView = "AutoGenerateResultSettingsView";
	
        public static string ImportFolderCustomerAdd = "ImportFolderCustomerAdd";
        public static string ImportFolderCustomerEdit = "ImportFolderCustomerEdit";

        public static string ImportFolderBranchAdd = "ImportFolderBranchAdd";
        public static string ImportFolderBranchEdit = "ImportFolderBranchEdit";

        public static string ImportFolderInventorAdd = "ImportFolderInventorAdd";
        public static string ImportFolderInventorEdit = "ImportFolderInventorEdit";

        public static string ExportPdaSettingsCustomerAdd = "ExportPdaSettingsCustomerAdd";
        public static string ExportPdaSettingsCustomerEdit = "ExportPdaSettingsCustomerEdit";

        public static string ExportErpSettingsCustomerAdd = "ExportErpSettingsCustomerAdd";
        public static string ExportErpSettingsCustomerEdit = "ExportErpSettingsCustomerEdit";

        public static string ExportErpSettingsBranchAdd = "ExportErpSettingsBranchAdd";
        public static string ExportErpSettingsBranchEdit = "ExportErpSettingsBranchEdit";

        public static string ExportErpSettingsInventorAdd = "ExportErpSettingsInventorAdd";
        public static string ExportErpSettingsInventorEdit = "ExportErpSettingsInventorEdit";

		public static string ExportPdaSettingsInventorAdd = "ExportPdaSettingsInventorAdd";
		public static string ExportPdaSettingsInventorEdit = "ExportPdaSettingsInventorEdit";

		public static string AdditionalSettingsInventorEdit = "AdditionalSettingsInventorEdit";
		public static string AdditionalSettingsCustomerEdit = "AdditionalSettingsCustomerEdit";

		
		
		

        public static string DynamicColumnSettingsCustomerAdd = "DynamicColumnSettingsCustomerAdd";
        public static string DynamicColumnSettingsCustomerEdit = "DynamicColumnSettingsCustomerEdit";

        public static string DynamicColumnSettingsBranchAdd = "DynamicColumnSettingsBranchAdd";
        public static string DynamicColumnSettingsBranchEdit = "DynamicColumnSettingsBranchEdit";

        public static string DynamicColumnSettingsInventorAdd = "DynamicColumnSettingsInventorAdd";
        public static string DynamicColumnSettingsInventorEdit = "DynamicColumnSettingsInventorEdit";

        public static string ExportPdaSettings = "ExportPdaSettings";        
        public static string ExportPdaSettingsInner = "ExportPdaSettingsInner";
        public static string ExportPdaExtraSettings = "ExportPdaExtraSettings";
        public static string ExportPdaProgramType = "ExportPdaProgramType";
		public static string ExportPdaAdapter = "ExportPdaAdapter";
		

        public static string UpdateFolderCustomerAdd = "UpdateFolderCustomerAdd";
        public static string UpdateFolderCustomerEdit = "UpdateFolderCustomerEdit";

        public static string UpdateFolderBranchAdd = "UpdateFolderBranchAdd";
        public static string UpdateFolderBranchEdit = "UpdateFolderBranchEdit";

        public static string UpdateFolderInventorAdd = "UpdateFolderInventorAdd";
        public static string UpdateFolderInventorEdit = "UpdateFolderInventorEdit";

        public static string PopupFilterItur = "PopupFilterItur";
        public static string PopupFilterIturAddEditDelete = "PopupFilterIturAddEditDelete";
        public static string PopupFilterCustomer = "PopupFilterCustomer";
        public static string PopupFilterBranch = "PopupFilterBranch";
        public static string PopupFilterInventor = "PopupFilterInventor";    
        public static string PopupFilterInventProductListSimple = "PopupFilterInventProductListSimple";
        public static string PopupFilterInventProductListSum = "PopupFilterInventProductListSum";
        public static string PopupSearchItur = "PopupSearchItur";
        public static string PopupSearchIturAddEditDelete = "PopupSearchIturAddEditDelete";
        public static string PopupSearchListDetailsInventProduct = "PopupSearchListDetailsInventProduct";
        public static string PopupSearchInventProduct = "PopupSearchInventProduct";
        public static string PopupSearchCustomerDashboard = "PopupSearchCustomerDashboard";
        public static string PopupSearchBranchDashboard = "PopupSearchBranchDashboard";
        public static string PopupSearchInventorDashboard = "PopupSearchInventorDashboard";
        public static string PopupSearchHomeDashboard = "PopupSearchHomeDashboard";
        public static string PopupSearchCustomerChoose = "PopupSearchCustomerChoose";
        public static string PopupSearchBranchChoose = "PopupSearchBranchChoose";
        public static string PopupSearchInventorChoose = "PopupSearchInventorChoose";
        public static string PopupSearchInventProductListSimple = "PopupSearchInventProductListSimple";
        public static string PopupSearchInventProductListSum = "PopupSearchInventProductListSum";
        public static string PopupSearchLocationAddEditDelete = "PopupSearchLocationAddEditDelete";
        public static string PopupFilterLocationAddEditDelete = "PopupFilterLocationAddEditDelete";
        public static string PopupSearchSectionAddEditDelete = "PopupSearchSectionAddEditDelete";
        public static string PopupFilterSectionAddEditDelete = "PopupFilterSectionAddEditDelete";
        public static string PopupSearchSupplierAddEditDelete = "PopupSearchSectionAddEditDelete";
		public static string PopupSearchFamilyAddEditDelete = "PopupSearchFamilyAddEditDelete";      
        public static string PopupFilterSupplierAddEditDelete = "PopupFilterSectionAddEditDelete";
		public static string PopupFilterFamilyAddEditDelete = "PopupFilterFamilyAddEditDelete";
        public static string PopupFilterPack = "PopupFilterPack";
        public static string PopupFilterInventProduct = "PopupFilterInventProduct";
        public static string PopupSearchCatalogForm = "PopupSearchCatalogForm";
        public static string PopupFilterCatalogForm = "PopupFilterCatalogForm";

        public static string PopupFilterHomeDashboard = "PopupFilterHomeDashboard";
        public static string PopupFilterCustomerDashboard = "PopupFilterCustomerDashboard";
        public static string PopupFilterBranchDashboard = "PopupFilterBranchDashboard";
        public static string PopupFilterInventorDashboard = "PopupFilterInventorDashboard";

        public static string PopupSpeedLink = "PopupSpeedLink";

        public static string CustomerGround = "CustomerGround";

        public static string SearchFieldGround = "SearchFieldGround";
        public static string SearchGridGround = "SearchGridGround";
        public static string SearchFieldTemplate = "SearchFieldTemplate";
        public static string FilterFieldGround = "FilterFieldGround";

        public static string SearchFieldInventProductAdvancedGround = "SearchFieldInventProductAdvancedGround";
        public static string SearchGridInventProductAdvancedGround = "SearchGridInventProductAdvancedGround";

        public static string Sort = "Sort";

        //search filter
        public static string LocationAddEditDeleteSearchFilter = "LocationAddEditDeleteSearchFilter";        
        public static string SectionAddEditDeleteSearchFilter = "SectionAddEditDeleteSearchFilter";
        public static string SupplierAddEditDeleteSearchFilter = "SupplierAddEditDeleteSearchFilter";
		public static string FamilyAddEditDeleteSearchFilter = "FamilyAddEditDeleteSearchFilter";
        public static string InventProductSearchFilter = "InventProductSearchFilter";
        public static string ProductSearchFilter = "ProductSearchFilter";
        public static string PlanogramAddEditDeleteSearchFilter = "PlanogramAddEditDeleteSearchFilter";

        public static string PlanogramCanvas = "PlanogramCanvas";
        public static string PlanogramInfo = "PlanogramInfo";
        public static string PlanogramTree = "PlanogramTree";
        public static string PlanogramProperties = "PlanogramProperties";
    }
}
