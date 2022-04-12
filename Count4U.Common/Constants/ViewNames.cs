using System;

namespace Count4U.Common
{

	
    public static class ViewNames
    {
        public const string IturListDetailsView = "IturListDetailsView";

        public const string StripView = "StripView";

        //choose
        public const string CustomerChooseView = "CustomerChooseView";
        public const string BranchChooseView = "BranchChooseView";
        public const string InventorChooseView = "InventorChooseView";        

        //dashboards                
        public const string HomeDashboardView = "HomeDashboardView";
        public const string CustomerDashboardFullView = "CustomerDashboardFullView";
        public const string BranchDashboardFullView = "BranchDashboardFullView";
        public const string InventorDashboardFullView = "InventorDashboardFullView";

        //dialogs
        public const string CustomerAddView = "CustomerAddView";
        public const string BranchAddView = "BranchAddView";
        public const string InventorAddView = "InventorAddView";
        public const string CustomerGroundView = "CustomerGroundView";
        public const string CustomerPostView = "CustomerPostView";
        public const string BranchPostView = "BranchPostView";
        public const string InventorPostView = "InventorPostView";

        public const string CustomerEditView = "CustomerEditView";
        public const string BranchEditView = "BranchEditView";
        public const string InventorEditView = "InventorEditView";
		public const string InventorEditOptionsView = "InventorEditOptionsView";
		public const string CustomerEditOptionsView = "CustomerEditOptionsView";  
		
		
      

        //mdi
        public const string MenuDashboardPartView = "MenuDashboardPartView";
        public const string CatalogInfoForCBIDashboardPartView = "CatalogInfoForCBIDashboardPartView";
        public const string ReportsDashboardPartView = "ReportsDashboardPartView";
        public const string LastBranchesDashboardPartView = "LastBranchesDashboardPartView";
        public const string BranchesDashboardPartView = "BranchesDashboardPartView";
        public const string LastInventorsDashboardPartView = "LastInventorsDashboardPartView";
        public const string LocationDashboardPartView = "LocationDashboardPartView";
        public const string PdaDashboardPartView = "PdaDashboardPartView";
        public const string FromPdaDashboardPartView = "FromPdaDashboardPartView";
        public const string IturimDashboardPartView = "IturimDashboardPartView";
        public const string InventorStatusDashboardPartView = "InventorStatusDashboardPartView";
        public const string LastCustomersDashboardPartView = "LastCustomersDashboardPartView";        
        public const string SectionDashboardPartView = "SectionDashboardPartView";
        public const string SupplierDashboardPartView = "SupplierDashboardPartView";
		public const string FamilyDashboardPartView = "FamilyDashboardPartView";
        public const string InventProductPartView = "InventProductPartView";
        public const string CustomerDashboardPartView = "CustomerDashboardPartView";
        public const string BranchDashboardPartView = "BranchDashboardPartView";        
        public const string InventorDashboardPartView = "InventorDashboardPartView";
        public const string HomeDashboardPartView = "HomeDashboardPartView";
        public const string InventProductSimplePartView = "InventProductSimplePartView";
        public const string InventProductSumPartView = "InventProductSumPartView";
        public const string PlanogramPartView = "PlanogramPartView";
        public const string StatisticDashboardPartView = "StatisticDashboardPartView";
       
        public const string InventorChangeStatusView = "InventorChangeStatusView";

        //location
        public const string LocationAddEditView = "LocationAddEditView";
		public const string LocationMultiAddView = "LocationMultiAddView";

		public const string ProcessAddEditView = "ProcessAddEditView";
		public const string AddUnknownProcess = "AddUnknownProcess";
		
		
        //status
        public const string IturStatusChangeView = "IturStatusChangeView";

        //invent product
        public const string InventProductListDetailsView = "InventProductListDetailsView";
        public const string InventProductAddEditView = "InventProductAddEditView";
        public const string InventProductCloneView = "InventProductCloneView";
        public const string InventProductView = "InventProductView";
        public const string InventProductListSimpleView = "InventProductListSimpleView";
        public const string InventProductListSumView = "InventProductListSumView";

		//document header
		public const string DocumentHeaderCloneView = "DocumentHeaderCloneView";

        //iturim

        public const string IturimAddEditDeleteView = "IturimAddEditDeleteView";
        public const string IturLocationChangeView = "IturLocationChangeView";
		public const string IturPrefixChangeView = "IturPrefixChangeView";
		public const string ShowShelfView = "ShowShelfView";
		public const string IturNameChangeView = "IturNameChangeView";
        public const string IturAddView = "IturAddView";
        public const string IturEditView = "IturEditView";
		public const string IturDeleteView = "IturDeleteView";
        public const string IturStateChangeView = "IturStateChangeView";
        public const string IturSelectView = "IturSelectView";
		public const string IturSelectDissableView = "IturSelectDissableView";
		
		public const string LocationTagChangeView = "LocationTagChangeView";
		public const string SectionTagChangeView = "SectionTagChangeView";
		public const string IturTagChangeView = "IturTagChangeView";

		public const string LocationCodeSelectView = "LocationCodeSelectView";
		public const string TagSelectView = "TagSelectView";

        //catalog

        public const string CatalogFormView = "CatalogFormView";

		//Device PDA
		public const string DeviceFormView = "DeviceFormView";
        public const string DeviceWorkerFormView = "DeviceWorkerFormView";

        //import
        public const string ImportFromPdaView = "ImportFromPdaView";
		public const string ComplexOperationView = "ComplexOperationView";
        public const string ErpExpectedStep1View = "ErpExpectedStep1View";
        public const string ImportWithModulesView = "ImportWithModulesView";
        public const string ImportByModuleView = "ImportByModuleView";
        public const string ImportFoldersView = "ImportFoldersView";
		public const string ConfigAdapterSettingView = "ConfigAdapterSettingView";
        public const string DynamicColumnSettingsView = "DynamicColumnSettingsView";
		public const string AdditionalSettingsSettingsView = "AdditionalSettingsSettingsView" ;
		public const string AutoGenerateResultSettingsView = "AutoGenerateResultSettingsView";
		
		

        //export
        public const string ExportPdaWithModulesView = "ExportPdaWithModulesView";
        public const string ExportErpWithModulesView = "ExportErpWithModulesView";
        public const string ExportByModuleView = "ExportByModuleView";
        public const string ExportErpByModuleView = "ExportErpByModuleView";        
        public const string ExportLogView = "ExportLogView";
		public const string ConfigEditAndSaveView = "ConfigEditAndSaveView";
		

        //
        public const string LocationAddEditDeleteView = "LocationAddEditDeleteView";

        public const string ProductAddEditView = "ProductAddEditView";

        public const string DocumentHeaderAddEditView = "DocumentHeaderAddEditView";
		public const string DeviceAddEditView = "DeviceAddEditView";


        public const string UserSettingsView = "UserSettingsView";
        public const string PathSettingsView = "PathSettingsView";
        public const string SqlScriptSettingsView = "SqlScriptSettingsView";
        public const string ConfigurationSetAddView = "ConfigurationSetAddView";

        public const string BackForwardView = "BackForwardView";        

        //zip import/export

        public const string ZipExportView = "ZipExportView";
        public const string ZipImportView = "ZipImportView";
		public const string ProcessAddEditGridView = "ProcessAddEditGridView";
		public const string ProcessZipView = "ProcessZipView";
		

        public const string PackView = "PackView";
        public const string UnpackView = "UnpackView";

        public const string UpdateView = "UpdateView";

        //masks        
        public const string MaskTemplateAddEditView = "MaskTemplateAddEditView";
        public const string MaskListView = "MaskListView";
        public const string MaskAddEditView = "MaskAddEditView";
        public const string MaskSelectView = "MaskSelectView";
        public const string MaskScriptOpenView = "MaskScriptOpenView";
        public const string MaskScriptSaveView = "MaskScriptSaveView";

        public const string AdaptersMaskView = "AdaptersMaskView";

        //reports
        public const string ReportsView = "ReportsView";
        public const string ReportTemplateView = "ReportTemplateView";
        public const string ReportFavoritesView = "ReportFavoritesView";                
        public const string ReportAddEditView = "ReportAddEditView";
        public const string ReportScriptView = "ReportScriptView";
        public const string ReportScriptSaveView = "ReportScriptSaveView";

        //misc                
        public const string LogView = "LogView";        

        public const string ExportPdaSettingsView = "ExportPdaSettingsView";
        public const string ExportPdaSettingsControlView = "ExportPdaSettingsControlView";
        public const string ExportPdaExtraSettingsView = "ExportPdaExtraSettingsView";
        public const string ExportPdaProgramTypeView = "ExportPdaProgramTypeView";
        public const string ExportErpSettingsView = "ExportErpSettingsView";
		public const string ExportPdaMerkavaAdapterView = "ExportPdaMerkavaAdapterView";
		

        public const string CBIObjectPropertiesView = "CBIObjectPropertiesView";

        public const string AdapterLinkView = "AdapterLinkView";
        public const string AdapterLinkScriptSaveView = "AdapterLinkScriptSaveView";
        public const string AdapterLinkScriptOpenView = "AdapterLinkScriptOpenView";

        public const string CBIScriptSaveView = "CBIScriptSaveView";
        public const string CBIScriptOpenView = "CBIScriptOpenView";

        public const string MdiFilterView = "MdiFilterView";

        //section
        public const string SectionAddEditDeleteView = "SectionAddEditDeleteView";
        public const string SectionAddEditView = "SectionAddEditView";

        //supplier
        public const string SupplierAddEditDeleteView = "SupplierAddEditDeleteView";
        public const string SupplierAddEditView = "SupplierAddEditView";

		//family
		public const string FamilyAddEditDeleteView = "FamilyAddEditDeleteView";
		public const string FamilyAddEditView = "FamilyAddEditView";

        //search view
        public const string SearchView = "SearchView";
        public const string SearchBranchFieldView = "SearchBranchFieldView";
        public const string SearchCustomerFieldView = "SearchCustomerFieldView";
        public const string SearchInventorFieldView = "SearchInventorFieldView";        
        public const string SearchCustomerView = "SearchCustomerView";
        public const string SearchBranchView = "SearchBranchView";
        public const string SearchInventorView = "SearchInventorView";        
        public const string SearchIturView = "SearchIturView";
        public const string SearchIturFieldView = "SearchIturFieldView";
        public const string SearchIturAdvancedView = "SearchIturAdvancedView";
        public const string SearchIturAdvancedFieldView = "SearchIturAdvancedFieldView";
        public const string SearchLocationView = "SearchLocationView";
        public const string SearchLocationFieldView = "SearchLocationFieldView";
        public const string SearchSectionView = "SearchSectionView";
        public const string SearchSectionFieldView = "SearchSectionFieldView";
        public const string SearchSupplierView = "SearchSupplierView";
        public const string SearchSupplierFieldView = "SearchSupplierFieldView";
		public const string SearchFamilyView = "SearchFamilyView";
		public const string SearchFamilyFieldView = "SearchFamilyFieldView";

        public const string SearchInventProductFieldView = "SearchInventProductFieldView";
        public const string SearchInventProductView = "SearchInventProductView";
        public const string SearchInventProductAdvancedFieldView = "SearchInventProductAdvancedFieldView";
		public const string SearchInventProductAdvancedAggregateFieldView = "SearchInventProductAdvancedAggregateFieldView";
        public const string SearchInventProductAdvancedView = "SearchInventProductAdvancedView";
		public const string SearchInventProductAdvancedAggregateView = "SearchInventProductAdvancedAggregateView";
	    public const string SearchInventProductAdvancedFieldSumView = "SearchInventProductAdvancedFieldSumView";
        public const string SearchInventProductAdvancedFieldSimpleView = "SearchInventProductAdvancedFieldSimpleView";
        public const string SearchInventProductAdvancedGridSimpleView = "SearchInventProductAdvancedGridSimpleView";
        public const string SearchInventProductAdvancedGridSumView = "SearchInventProductAdvancedGridSumView";
        public const string SearchProductFieldView = "SearchProductFieldView";
        public const string SearchProductView = "SearchProductView";

        public const string SearchPackFieldView = "SearchPackFieldView";

        public const string SpeedLinkView = "SpeedLinkView";

        public const string UpdateAdaptersView = "UpdateAdaptersView";

        public const string BottomView = "BottomView";

        public const string FilterView = "FilterView";

        public const string SortView = "SortView";
        public const string FilterTemplateView = "FilterTemplateView";
        public const string FilterTemplateAddEditView = "FilterTemplateAddEditView";

        public const string SearchFilterView = "SearchFilterView";

        public const string PlanBasementView = "PlanBasementView";
        public const string PlanCanvasView = "PlanCanvasView";
        public const string PlanSizeChangeView = "PlanSizeChangeView";
        public const string PlanIturAddView = "PlanIturAddView";
        public const string PlanInfoView = "PlanInfoView";
        public const string PlanAddEditDeleteView = "PlanAddEditDeleteView";
        public const string PlanTreeView = "PlanTreeView";
        public const string PlanPropertiesView = "PlanPropertiesView";
        public const string PlanLocationAssignView = "PlanLocationAssignView";
        public const string PlanTextAssignView = "PlanTextAssignView";
        public const string PlanPictureAssignView = "PlanPictureAssignView";

        public const string UploadToPdaView = "UploadToPdaView";
		public const string DownloadFromPDAView = "DownloadFromPDAView";
		public const string FromFtpView = "FromFtpView";
		public const string ToFtpView = "ToFtpView";
		public const string ToFtpViewModel = "ToFtpViewModel";
		public const string FromFtpViewModel = "FromFtpViewModel";

        public const string LocationItursChangeView = "LocationItursChangeView";
        

    }

	public static class ViewModelNames
	{
		public const string InventorChangeStatusViewModel = "InventorChangeStatusViewModel";
	}


}