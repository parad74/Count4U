using System;
using System.Collections.Generic;
using System.Windows.Media;
using Count4U.Common.UserSettings.LogType;
using Count4U.Common.UserSettings.Menu;
using Count4U.Model;

namespace Count4U.Common.UserSettings
{
    public interface IUserSettingsManager
    {
        void AdminSave();
        void AdminCommitSave(string section);
        List<string> AdminListConfiguration();
        void AdminSetCurrentConfiguration(string fileName);
        string AdminGetCurrentConfiguration();
        void AdminAddConfiguration(string fileName);

        RegionElement RegionElementGet(string sectionName);
        void RegionElementSet(string sectionName, RegionElement regionElement);

        MainWindowSettings MainWindowSettingsGet();
        void MainWindowSettingsSet(MainWindowSettings settings);

		DateTime StartInventorDateTimeGet();
		void StartInventorDateTimeSet(DateTime startInventorDateTime);

		DateTime EndInventorDateTimeGet();
		void EndInventorDateTimeSet(DateTime endInventorDateTime);

        int PortionItursGet();
        void PortionItursSet(int porition);

        int PortionCBIGet();
        void PortionCBISet(int porition);

        int PortionItursListGet();
        void PortionItursListSet(int porition);

        int PortionInventProductsGet();
        void PortionInventProdutsSet(int porition);

        int PortionProductsGet();
        void PortionProdutsSet(int porition);

        int PortionSectionsGet();
        void PortionSectionsSet(int porition);

        string StatusColorGet(string statusName);
        void StatusColorSet(string statusName, string color);

        string StatusGroupColorGet(string statusGroupName);
        void StatusGroupColorSet(string statusName, string color);

        int ImportEncodingGet();
        void ImportEncodingSet(int encoding);

        int GlobalEncodingGet();
        void GlobalEncodingSet(int encoding);

        LogTypeElementCollection LogTypeGet();
        void LogTypeSet(string logTypeName, bool isEnabled);

        enLanguage LanguageGet();
        void LanguageSet(enLanguage language);

        int DelayGet();
        void DelaySet(int delay);

        string IturSortGet();
        void IturSortSet(string sort);
        string IturGroupGet();
        void IturGroupSet(string group);

        bool IsExpandedBottomGet();
        void IsExpandedBottomSet(bool isExpanded);

        int PortionSuppliersGet();
        void PortionSuppliersSet(int portion);

		int PortionFamilysGet();
		void PortionFamilysSet(int portion);

        string IturModeGet();
        void IturModeSet(string mode);

        char CurrencyGet();
        void CurrencySet(char currency);

        string BarcodeTypeGet();
        void BarcodeTypeSet(string barcodeType);

		string PrinterGet();
		void PrinterSet(string printer);

        string BarcodePrefixGet();
        void BarcodePrefixSet(string barcodePrefix);

		string CustomerFilterCodeGet();
		void CustomerFilterCodeSet(string customerFilterCode);

		string CustomerFilterNameGet();
		void CustomerFilterNameSet(string customerFilterName);

		bool UseCustomerFilterGet();
		void UseCustomerFilterSet(bool useCustomerFilter);


		bool SearchDialogIsModalGet();
		void SearchDialogIsModalSet(bool searchDialogIsModal);

	    string IturNamePrefixGet();
		void IturNamePrefixSet(string barcodePrefix );

        bool NavigateBackImportPdaFormGet();
        void NavigateBackImportPdaFormSet(bool navigateBack);

        MenuElement MenuGet(string name, string partName, string dashboardName);
        void MenuInsert(MenuElement menu);
        void MenuUpdate(MenuElement menu);

		string ImportPDAPathGet();
        void ImportPDAPathSet(string path);
	    string ExportPDAPathGet();
		void ExportPDAPathSet(string path);

		string ImportTCPPathGet();
		void ImportTCPPathSet(string path);
		string ExportTCPPathGet();
		void ExportTCPPathSet(string path);
  		string TcpServerPortGet();
		void TcpServerPortSet(string port);
		void TcpServerOnSet(bool tcpServerOn);
		bool TcpServerOnGet();

		bool RefreshedReport { get; set; }

		void WebServiceLinkSet(string link);
		string WebServiceLinkGet();

		void UseTooSet(bool useToo);
		bool UseTooGet();



		void WebServiceDeveloperLinkSet(string link);
		string WebServiceDeveloperLinkGet();

	
		string HostGet();
		string HostFtpGet(out bool enableSsl);
		void HostSet(string host);
		string UserGet();
		void UserSet(string user);
		string PasswordGet();
		void PasswordSet(string password);

        string DashboardHomeBackgroundGet();
        void DashboardHomeBackgroundSet(string path);
        double DashboardHomeBackgroundOpacityGet();
        void DashboardHomeBackgroundOpacitySet(double opacity);
        string DashboardCustomerBackgroundGet();
        void DashboardCustomerBackgroundSet(string path);
        double DashboardCustomerBackgroundOpacityGet();
        void DashboardCustomerackgroundOpacitySet(double opacity);
        string DashboardBranchBackgroundGet();
        void DashboardBranchBackgroundSet(string path);
        double DashboardBranchBackgroundOpacityGet();
        void DashboardBranchBackgroundOpacitySet(double opacity);
        string DashboardInventorBackgroundGet();
        void DashboardInventorBackgroundSet(string path);
        double DashboardInventorBackgroundOpacityGet();
        void DashboardInventorBackgroundOpacitySet(double opacity);

        IturAnalyzesRepositoryEnum ReportRepositoryGet();
        void ReportRepositorySet(IturAnalyzesRepositoryEnum en);

        bool ShowIturERPGet();
        void ShotIturERPSet(bool isShow);

		bool PackDataFileCatalogGet();
		void PackDataFileCatalogSet(bool isPackDataFileCatalog);
		

        string IturFilterSelectedGet();
        void IturFilterSelectedSet(string selectedValue);

		string IturFilterSortSelectedGet();
		void IturFilterSortSelectedSet(string selectedValue);

		string IturFilterSortAZSelectedGet();
		void IturFilterSortAZSelectedSet(string selectedValue);

        string InventProductFilterFocusGet();
        void InventProductFilterFocusSet(string focusValue);

        string ReportAppNameGet();
        void ReportAppNameSet(string value);

		

	    Color PlanEmptyColorGet();

        void PlanEmptyColorSet(Color color);
        Color PlanZeroColorGet();
        void PlanZeroColorSet(Color color);
        Color PlanHundredColorGet();
        void PlanHundredSet(Color color);

		Color InventProductMarkColorGet();
		 void InventProductMarkColorSet(Color color);
		

        int UploadWakeupTimeGet();
        void UploadWakeupTimeSet(int time);

		string UploadOptionsHT630_BaudratePDAItemGet();
		void UploadOptionsHT630_BaudratePDAItemSet(string baudratePDA);

		string UploadOptionsRunMemoryItemGet();
		void UploadOptionsRunMemoryItemSet(string memory);

		string DomainObjectSelectedItemGet();
		void DomainObjectSelectedItemSet(string domainObject);

		bool TagSubstringGet();
		void TagSubstringSet(bool substring);


		string InventProductPropertyMarkSelectedItemGet();
		void InventProductPropertyMarkSelectedItemSet(string propertyName);

	
		string InventProductPropertyFilterSelectedItemGet();
		void InventProductPropertyFilterSelectedItemSet(string propertyName);

		string InventProductPropertyFilterSelectedNumberItemGet();
		void InventProductPropertyFilterSelectedNumberItemSet(string propertyName);


		string InventProductPropertyPhotoSelectedItemGet();
		void InventProductPropertyPhotoSelectedItemSet(string propertyName);

		string InventProductPropertySelectedItemGet();
		void InventProductPropertySelectedItemSet(string propertyName);

		string EditorTemplateSelectedItemGet();
		void EditorTemplateSelectedItemSet(string templateName);
		

		bool CopyFromSourceGet();
		void CopyFromSourceSet(bool copyFromSource);

		bool SendToFtpOfficeGet();
		void SendToFtpOfficeSet(bool copyFromSource);
	   

		bool ForwardResendDataGet();
		void ForwardResendDataSet(bool forwardResendData);

		bool CopyFromHostGet();
		void CopyFromHostSet(bool copyFromHost);

		bool CopyByCodeInventorGet();
		void CopyByCodeInventorSet(bool copyByCodeInventor);

		

		bool CountingFromSourceGet();
		void CountingFromSourceSet(bool countingFromSource);


		bool ShowMarkGet();
		void ShowMarkSet(bool showMark);

		bool PropertyIsEmptyGet();
		void PropertyIsEmptySet(bool propertyIsEmpty);
		

		bool UploadOptionsHT630_CurrentDataPDAGet();
		void UploadOptionsHT630_CurrentDataPDASet(bool currentDataPDA);

		bool UploadOptionsHT630_BaudratePDAGet();
		void UploadOptionsHT630_BaudratePDASet(bool currentDataPDA);
		
		bool UploadOptionsHT630_DeleteAllFilePDAGet();
		void UploadOptionsHT630_DeleteAllFilePDASet(bool deleteAllFilePDA);

		string UploadOptionsHT630_ExeptionFileNotDeleteGet();
		void UploadOptionsHT630_ExeptionFileNotDeleteSet(string fileName);

		bool UploadOptionsHT630_AfterUploadPerformWarmStartGet();
		void UploadOptionsHT630_AfterUploadPerformWarmStartSet(bool performWarmStart);

		bool UploadOptionsHT630_AfterUploadRunExeFileNeedDoGet();
		void UploadOptionsHT630_AfterUploadRunExeFileNeedDoSet(bool value);
		
		//string UploadOptionsHT630_AfterUploadRunExeFileGet();
		//void UploadOptionsHT630_AfterUploadRunExeFileSet(string fileName);
			
		string UploadOptionsHT630_AfterUploadRunExeFileListGet();
		void UploadOptionsHT630_AfterUploadRunExeFileListSet(string value);



		
		
    }
}