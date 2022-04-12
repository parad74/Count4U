namespace Count4U.Common
{
    public static class NavigationSettings
    {        
        public const string MenuDashboardMain = "MenuDashboardMain"; //menu region on main dashboard
        public const string MenuDashboardCustomer = "MenuDashboardCustomer";
        public const string MenuDashboardBranch = "MenuDashboardBranch";
        public const string MenuDashboardInventor = "MenuDashboardInventor";

        //Context
        public const string CBIContext = "CBIContext";
        public const string CBIContextInventor = "CBIContextInventor";
        public const string CBIContextMain = "CBIContextMain";
        public const string CBIContextHistory = "CBIContextHistory";
		public const string FromContext = "FromContext";			//бывает важно откуда открывают форму и форма должна сама что-то сделать/не сделать на открытии надо знать откуда открыли

        public const string ViewOnly = "ViewOnly"; //open form as read-only
        public const string IsCustomerComboVisible = "IsCustomerComboVisible"; //hide choose customer/branch combos
        public const string IsBranchComboVisible = "IsBranchComboVisible";
		public const string WithoutNavigate = "WithoutNavigate";
		

        public const string ControllerName = "ControllerName";        

        //settings for LastInventorsDashboardPartViewModel
        public const string LastInventors = "LastInventors";
        public const string LastInventorsForMain = "LastInventorsForMain";
        public const string LastInventorsForCustomer = "LastInventorsForCustomer";
        public const string LastInventorsForBranch = "LastInventorsForBranch";

		

        //setings for LastCustomersDashboardPartViewModel
        public const string LastCustomers = "LastCustomers";
        public const string LastCustomersInInventory = "LastCustomersLastCustomersInInventory";
        public const string LastCustomersBuild = "LastCustomersBuild";

		public const string CustomerCode = "CustomerCode";
		public const string CustomerCodes = "CustomerCodes";

        public const string IturCode = "IturCode";
        public const string IturCodes = "IturCodes";

		public const string ProcessCode = "ProcessCode";
		public const string AddUnknownProcess = "AddUnknownProcess";
		  
       public const string InventProductId = "InventProductId";

        public const string DocumentCode = "DocumentCode";
		public const string DeviceCode = "DeviceCode";

        public const string ImportMode = "ImportMode";
        public const string ImportModeCatalog = "ImportModeCatalog";
        public const string ImportModeLocation = "ImportModeLocation";
        public const string ImportModeItur = "ImportModeItur";
        public const string ImportModeSection = "ImportModeSection";
        public const string ImportModeSupplier = "ImportModeSupplier";
		public const string ImportModeFamily = "ImportModeFamily";
        public const string ImportModeUpdateCatalog = "ImportModeUpdateCatalog";
		public const string ComplexModeObject = "ComplexModeObject";
        public const string ImportModeBranch = "ImportModeBranch";
        public const string ImportModeUnitPlan = "ImportModeUnitPlan";

        //settings for LocationDashboardPartViewModel
        public const string LocationsPart = "LocationsPart";
        public const string LocationsPartCustomer = "LocationsPartCustomer";
        public const string LocationsPartBranch = "LocationsPartBranch";
        public const string LocationsPartInventor = "LocationsPartInventor";

        public const string MdiPartOwnerDashboard = "MdiPartOwnerDashboard";
        public const string MdiPartOwnerDashboardMain = "MdiPartOwnerDashboardMain";
        public const string MdiPartOwnerDashboardCustomer = "MdiPartOwnerDashboardCustomer";
        public const string MdiPartOwnerDashboardBranch = "MdiPartOwnerDashboardBranch";
        public const string MdiPartOwnerDashboardInventor = "MdiPartOwnerDashboardInventor";

        public const string CBIDbContext = "CBIDbContext";
        public const string CBIDbContextCustomer = "CBIDbContextCustomer";
        public const string CBIDbContextBranch = "CBIDbContextBranch";
        public const string CBIDbContextInventor = "CBIDbContextInventor";

        public const string ProductMakat = "ProductMakat";
        public const string LocationCode = "LocationCode";
		
		public const string LocationCodes = "LocationCodes";
		public const string SectionCodes = "SectionCodes";
        public const string AddUnknownLocation = "AddUnknownLocation";

        public const string StripMode = "StripMode";
        public const string StripModeEmpty = "StripModeEmpty";
        public const string StripModeCustomer = "StripModeCustomer";
        public const string StripModeCustomerBranch = "StripModeCustomerBranch";
        public const string StripModeCustomerBranchInventor = "StripModeCustomerBranchInventor";

        public const string MaskId = "MaskId";
        public const string MaskTemplateCode = "MaskTemplateCode";
        public const string MaskFileName = "MaskFileName";

        public const string AdapterName = "AdapterName";

        public const string AuditConfigCode = "AuditConfigCode";
        public const string AuditConfigCustomer = "AuditConfigCustomer";
        public const string AuditConfigBranch = "AuditConfigBranch";
        public const string AuditConfigInventor = "AuditConfigInventor";

        public const string ReportUniqueCode = "ReportUniqueCode";
        public const string ReportFileNames = "ReportFileNames";

        public const string ImportExportLog = "ImportExportLog";

        public const string SelectParams = "SelectParams";

        public const string ExportPdaSettingsConrolInExportFormMode = "ExportPdaSettingsConrolInExportFormMode";
        public const string IsReadOnly = "IsReadOnly";

        public const string ApplicationStart = "ApplicationStart";

        public const string SectionCode = "SectionCode";
        public const string SupplierCode = "SupplierCode";
		public const string FamilyCode = "FamilyCode";

        public const string CBIViewName = "CBIViewName";

        public const string CustomerMode = "CustomerMode";

        public const string SearchWithOnlyCBI = "SearchWithOnlyCBI";

        public const string NoNeedToBuildAnalyzeTable = "NoNeedToBuildAnalyzeTable";

        public const string NavigateBackCheckboxIsChecked = "NavigateBackCheckboxIsChecked";

        public const string AutoStartImportPda = "AutoStartImportPda";

//        public const string FilterTemplateContext = "FilterTemplateContext";
//        public const string FilterTemplateName = "FilterTemplateName";

        public const string PlanogramWidth = "PlanogramWidth";
        public const string PlanogramHeight = "PlanogramHeight";

        public const string PlanogramUnitCode = "PlanogramUnitCode";

        public const string IturPrefix = "IturPrefix";

        public const string OpenAsModalWindow = "OpenAsModalWindow";

        public const string AutoStartExportErp = "AutoStartExportErp";
        public const string AutoCloseExportErpWindow = "AutoCloseExportErpWindow";

		public const string CheckBoundrate = "CheckBoundrate";


		public const string PeriodFromInventorDate = "PeriodFromInventorDate";
		public const string PeriodFromStartDate = "PeriodFromStartDate";
		public const string QuentetyEdit = "QuentetyEdit";

		}
}