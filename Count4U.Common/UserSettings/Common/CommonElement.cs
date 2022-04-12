using System;
using System.Configuration;
using Count4U.Common.Constants;
using Count4U.Model;
using Count4U.Model.Count4U;

namespace Count4U.Common.UserSettings
{
    public class CommonElement : ConfigurationElement
    {
        public const int DefaultItursPortion = 500;
        public const int DefaultItursPortionList = 500;
        public const int DefaultInventProductsPortion = 200;
        public const int DefaultCBIPortion = 20;
        public const int DefaultSectionsPortion = 50;
        public const int DefaultSuppliersPortion = 50;
		public const int DefaultFamilysPortion = 50;
        public const int DefaultProductsPortion = 50;
        public const int DefaultImportEncoding = 1255;
        public const int DefaultGlobalEncoding = 1200;
        public const string DefaultLanguage = "en";
        public const string DefaultConfSet = "default";
        public const int DefaultDelay = 900;
        public const string DefaultIturSort = ComboValues.SortItur.NumberValue;
        public const string DefaultIturGroup = ComboValues.GroupItur.LocationValue;
        public const string DefaultIturMode = ComboValues.IturListDetailsMode.ModePaged;
        public const bool DefaultIsExpandedBottom = false;
        public const string DefaultBarcodePrefix = "%L%";
		public const string DefaultCustomerFilterCode = "";
		public const string DefaultCustomerFilterName = "";
		public const bool DefaultUseCustomerFilter = false;
		public const bool DefaultSearchDialogIsModal = false;
        public const string DefaultBarcodeType = "Code128";
		public const string DefaultPrinter = "default";
        public const string DefaultIturNamePrefix = "ITUR";
        public const char DefaultCurrency = Common.Constants.CurrencySymbol.SHEQEL;
        public const bool DefaultNavigateBackImportPdaForm = true;
        public const double DefaultDashboardBackgroundOpacity = 0.2;
		public const string DefaultReportRepository = "IturAnalyzesBulkRepository";//"IturAnalyzesReaderADORepository";
        public const bool DefaultShowIturERP = false;
		public const bool DefaultPackDataFileCatalog = true;
        public const string DefaultIturFilterSelected = ComboValues.FindItur.FilterIturNumber;
		public const string DefaultIturFilterSortAZSelected = ComboValues.FindIturSortAZ.SortASC;
		public const string DefaultIturFilterSortSelected = "";
        public const string DefaultInventProductFilterFocus = FocusValues.InventProductSearch.CodeInputFromPDA;
        public const string DefaultReportAppName = "COUNT4U";
        public const string DefaultPlanEmptyColor = "120,120,120";
        public const string DefaultPlanZeroColor = "160,160,160";
        public const string DefaultPlanHundredColor = "38,127,0";
		public const string DefaultInventProductMarkColor = "173,216,230";
        public const int DefaultUploadWakeupTime = 7;
		public const bool DefaultUploadOptionsHT630_BaudratePDA = true;
		public const string DefaultUploadOptionsHT630_BaudratePDAItem = "57600";
		public const string DefaultUploadOptionsRunMemoryItem = "RAM";
		public const string DefaultInventProductPropertyMarkSelectedItem = "IPValueStr10";
		public const string DefaultInventProductPropertyFilterSelectedItem = "IPValueStr10";
		public const string DefaultInventProductPropertyFilterSelectedNumberItem = "IPValueInt3";
		public const string DefaultInventProductPropertyPhotoSelectedItem = "IPValueStr10";
		public const string DefaultInventProductPropertySelectedItem = "IPValueStr10";
		public const string DefaultEditorTemplateSelectedItem = "Nativ";

		public const string DefaultDomainObjectSelectedItem = "Location";
		
		public const bool DefaultUploadOptionsHT630_CurrentDataPDA = true;
		public const bool DefaultUploadOptionsHT630_DeleteAllFilePDA = true;
		public const string DefaultUploadOptionsHT630_ExeptionFileNotDelete = "JENG.EXE;UPDATEDU.TXT;PDANUM.TXT";
		public const bool DefaultUploadOptionsHT630_AfterUploadPerformWarmStart = true;
		public const bool DefaultUploadOptionsHT630_AfterUploadRunExeFileNeedDo = true;
		public const string DefaultUploadOptionsHT630_AfterUploadRunExeFile = "OFFLINE.EXE";  //old
		public const string DefaultUploadOptionsHT630_AfterUploadRunExeFileList = "OFFLINE.EXE";
		public const bool DefaultCopyFromSource = true;
		public const bool DefaultForwardResendData = false;
		public const bool DefaultTagSubstring = true;
		public const bool DefaultCopyFromHost = false;
		public const bool DefaultCopyByCodeInventor = true;
		public const bool DefaultCountingFromSource = true;
		public const bool DefaultSendToFtpOffice = false;
		public const string DefaultImportPDAPath = @"C:\MIS";
		public const string DefaultExportPDAPath = @"C:\MIS";
        public const string DefaultImportTCPPath = @"C:\mInv";
        public const string DefaultExportTCPPath = @"C:\mInv";
        public const string DefaultTcpServerPort = @"3000";
        public const bool DefaultTcpServerOn = false;

        public const string DefaultHost = @"ftp://ftp.boscom.com";//@"ftp://ftp.idnext.co.il";
		public const string DefaultUser = "idnext";
		public const string DefaultPassword = "ab1111!";
		public const bool DefaultShowMark = false;
		public const bool DefaultPropertyIsEmpty = false;

        public const bool DefaultUseToo = false;

        public const string DefaultWebServiceLink = @"http://api.prod.minv.dimex.co.il/v1/c4u";   //http://api.dev.minv.dimex.co.il/v1/c4u
        public const string DefaultWebServiceDeveloperLink = @"http://api.dev.minv.dimex.co.il/v1/c4u";



        [ConfigurationProperty("CurrentConfigurationSet", DefaultValue = DefaultConfSet, IsRequired = false)]
        public string CurrentConfigurationSet
        {
            get { return (string)this["CurrentConfigurationSet"]; }
            set { this["CurrentConfigurationSet"] = value; }
        }

        [ConfigurationProperty("ItursPortion", DefaultValue = DefaultItursPortion, IsRequired = true)]
        public int ItursPortion
        {
            get { return (int)this["ItursPortion"]; }
            set { this["ItursPortion"] = value; }
        }

        [ConfigurationProperty("ItursPortionList", DefaultValue = DefaultItursPortionList)]
        public int ItursPortionList
        {
            get { return (int)this["ItursPortionList"]; }
            set { this["ItursPortionList"] = value; }
        }

        [ConfigurationProperty("InventProductsPortion", DefaultValue = DefaultInventProductsPortion)]
        public int InventProductsPortion
        {
            get { return (int)this["InventProductsPortion"]; }
            set { this["InventProductsPortion"] = value; }
        }

        [ConfigurationProperty("ProductsPortion", DefaultValue = DefaultProductsPortion)]
        public int ProductsPortion
        {
            get { return (int)this["ProductsPortion"]; }
            set { this["ProductsPortion"] = value; }
        }

        [ConfigurationProperty("CBIPortion", DefaultValue = DefaultCBIPortion, IsRequired = true)]
        public int CBIPortion
        {
            get { return (int)this["CBIPortion"]; }
            set { this["CBIPortion"] = value; }
        }

        [ConfigurationProperty("SectionsPortion", DefaultValue = DefaultSectionsPortion, IsRequired = false)]
        public int SectionsPortion
        {
            get { return (int)this["SectionsPortion"]; }
            set { this["SectionsPortion"] = value; }
        }

        [ConfigurationProperty("MainWindowTop", IsRequired = true)]
        public double MainWindowTop
        {
            get { return (double)this["MainWindowTop"]; }
            set { this["MainWindowTop"] = value; }
        }

        [ConfigurationProperty("MainWindowLeft", IsRequired = true)]
        public double MainWindowLeft
        {
            get { return (double)this["MainWindowLeft"]; }
            set { this["MainWindowLeft"] = value; }
        }

        [ConfigurationProperty("MainWindowWidth", IsRequired = true, DefaultValue = 1000.0)]
        public double MainWindowWidth
        {
            get { return (double)this["MainWindowWidth"]; }
            set { this["MainWindowWidth"] = value; }
        }

        [ConfigurationProperty("MainWindowHeight", IsRequired = true, DefaultValue = 800.0)]
        public double MainWindowHeight
        {
            get { return (double)this["MainWindowHeight"]; }
            set { this["MainWindowHeight"] = value; }
        }

        [ConfigurationProperty("MainWindowIsMaximized", DefaultValue = true, IsRequired = true)]
        public bool IsMaximized
        {
            get { return (bool)this["MainWindowIsMaximized"]; }
            set { this["MainWindowIsMaximized"] = value; }
        }

        [ConfigurationProperty("ImportEncoding", DefaultValue = DefaultImportEncoding, IsRequired = false)]
        public int ImportEncoding
        {
            get { return (int)this["ImportEncoding"]; }
            set { this["ImportEncoding"] = value; }
        }

        [ConfigurationProperty("GlobalEncoding", DefaultValue = DefaultGlobalEncoding, IsRequired = false)]
        public int GlobalEncoding
        {
            get { return (int)this["GlobalEncoding"]; }
            set { this["GlobalEncoding"] = value; }
        }

        [ConfigurationProperty("Language", DefaultValue = DefaultLanguage, IsRequired = false)]
        public string Language
        {
            get { return (string)this["Language"]; }
            set { this["Language"] = value; }
        }

        [ConfigurationProperty("Delay", DefaultValue = DefaultDelay, IsRequired = false)]
        public int Delay
        {
            get { return (int)this["Delay"]; }
            set { this["Delay"] = value; }
        }

        [ConfigurationProperty("IturSort", DefaultValue = DefaultIturSort, IsRequired = false)]
        public string IturSort
        {
            get { return (string)this["IturSort"]; }
            set { this["IturSort"] = value; }
        }

        [ConfigurationProperty("IturGroup", DefaultValue = DefaultIturGroup, IsRequired = false)]
        public string IturGroup
        {
            get { return (string)this["IturGroup"]; }
            set { this["IturGroup"] = value; }
        }

        [ConfigurationProperty("IturMode", DefaultValue = DefaultIturMode, IsRequired = false)]
        public string IturMode
        {
            get { return (string)this["IturMode"]; }
            set { this["IturMode"] = value; }
        }

        [ConfigurationProperty("IsExpandedBottom", DefaultValue = DefaultIsExpandedBottom, IsRequired = false)]
        public bool IsExpandedBottom
        {
            get { return (bool)this["IsExpandedBottom"]; }
            set { this["IsExpandedBottom"] = value; }
        }

        [ConfigurationProperty("SuppliersPortion", DefaultValue = DefaultSuppliersPortion, IsRequired = false)]
        public int SuppliersPortion
        {
            get { return (int)this["SuppliersPortion"]; }
            set { this["SuppliersPortion"] = value; }
        }


		[ConfigurationProperty("FamilysPortion", DefaultValue = DefaultFamilysPortion, IsRequired = false)]
		public int FamilysPortion
        {
			get { return (int)this["FamilysPortion"]; }
			set { this["FamilysPortion"] = value; }
        }

        [ConfigurationProperty("BarcodeType", DefaultValue = DefaultBarcodeType, IsRequired = false)]
        public string BarcodeType
        {
            get { return (string)this["BarcodeType"]; }
            set { this["BarcodeType"] = value; }
        }

		[ConfigurationProperty("Printer", DefaultValue = DefaultPrinter, IsRequired = false)]
		public string Printer
		{
			get { return (string)this["Printer"]; }
			set { this["Printer"] = value; }
		}

        [ConfigurationProperty("BarcodePrefix", DefaultValue = DefaultBarcodePrefix, IsRequired = false)]
        public string BarcodePrefix
        {
            get { return (string)this["BarcodePrefix"]; }
            set { this["BarcodePrefix"] = value; }
        }



	  [ConfigurationProperty("CustomerFilterCode", DefaultValue = DefaultCustomerFilterCode, IsRequired = false)]
        public string CustomerFilterCode
        {
            get { return (string)this["CustomerFilterCode"]; }
            set { this["CustomerFilterCode"] = value; }
        }

	  [ConfigurationProperty("CustomerFilterName", DefaultValue = DefaultCustomerFilterName, IsRequired = false)]
        public string CustomerFilterName
        {
            get { return (string)this["CustomerFilterName"]; }
            set { this["CustomerFilterName"] = value; }
        }

	  [ConfigurationProperty("UseCustomerFilter", DefaultValue = DefaultUseCustomerFilter, IsRequired = false)]
	  public bool UseCustomerFilter
	  {
		  get { return (bool)this["UseCustomerFilter"]; }
		  set { this["UseCustomerFilter"] = value; }
	  }

	  [ConfigurationProperty("SearchDialogIsModal", DefaultValue = DefaultSearchDialogIsModal, IsRequired = false)]
	  public bool SearchDialogIsModal
	  {
		  get { return (bool)this["SearchDialogIsModal"]; }
		  set { this["SearchDialogIsModal"] = value; }
	  }
		

        [ConfigurationProperty("IturNamePrefix", DefaultValue = DefaultIturNamePrefix, IsRequired = false)]
        public string IturNamePrefix
        {
            get { return (string)this["IturNamePrefix"]; }
            set { this["IturNamePrefix"] = value; }
        }

        [ConfigurationProperty("Currency", DefaultValue = DefaultCurrency, IsRequired = false)]
        public char Currency
        {
            get { return (char)this["Currency"]; }
            set { this["Currency"] = value; }
        }

        [ConfigurationProperty("NavigateBackImportPdaForm", DefaultValue = DefaultNavigateBackImportPdaForm, IsRequired = false)]
        public bool NavigateBackImportPdaForm
        {
            get { return (bool)this["NavigateBackImportPdaForm"]; }
            set { this["NavigateBackImportPdaForm"] = value; }
        }


	
        //DefaultDashboardBackgroundOpacity

        [ConfigurationProperty("DashboardHomeBackground", DefaultValue = "", IsRequired = false)]
        public string DashboardHomeBackground
        {
            get { return (string)this["DashboardHomeBackground"]; }
            set { this["DashboardHomeBackground"] = value; }
        }

        [ConfigurationProperty("DashboardHomeBackgroundOpacity", DefaultValue = DefaultDashboardBackgroundOpacity, IsRequired = false)]
        public double DashboardHomeBackgroundOpacity
        {
            get { return (double)this["DashboardHomeBackgroundOpacity"]; }
            set { this["DashboardHomeBackgroundOpacity"] = value; }
        }

        [ConfigurationProperty("DashboardCustomerBackground", DefaultValue = "", IsRequired = false)]
        public string DashboardCustomerBackground
        {
            get { return (string)this["DashboardCustomerBackground"]; }
            set { this["DashboardCustomerBackground"] = value; }
        }

        [ConfigurationProperty("DashboardCustomerBackgroundOpacity", DefaultValue = DefaultDashboardBackgroundOpacity, IsRequired = false)]
        public double DashboardCustomerBackgroundOpacity
        {
            get { return (double)this["DashboardCustomerBackgroundOpacity"]; }
            set { this["DashboardCustomerBackgroundOpacity"] = value; }
        }

        [ConfigurationProperty("DashboardBranchBackground", DefaultValue = "", IsRequired = false)]
        public string DashboardBranchBackground
        {
            get { return (string)this["DashboardBranchBackground"]; }
            set { this["DashboardBranchBackground"] = value; }
        }

        [ConfigurationProperty("DashboardBranchBackgroundOpacity", DefaultValue = DefaultDashboardBackgroundOpacity, IsRequired = false)]
        public double DashboardBranchBackgroundOpacity
        {
            get { return (double)this["DashboardBranchBackgroundOpacity"]; }
            set { this["DashboardBranchBackgroundOpacity"] = value; }
        }

        [ConfigurationProperty("DashboardInventorBackground", DefaultValue = "", IsRequired = false)]
        public string DashboardInventorBackground
        {
            get { return (string)this["DashboardInventorBackground"]; }
            set { this["DashboardInventorBackground"] = value; }
        }

        [ConfigurationProperty("DashboardInventorBackgroundOpacity", DefaultValue = DefaultDashboardBackgroundOpacity, IsRequired = false)]
        public double DashboardInventorBackgroundOpacity
        {
            get { return (double)this["DashboardInventorBackgroundOpacity"]; }
            set { this["DashboardInventorBackgroundOpacity"] = value; }
        }

        [ConfigurationProperty("ReportRepository", DefaultValue = DefaultReportRepository, IsRequired = false)]
        public string ReportRepository
        {
            get { return (string)this["ReportRepository"]; }
            set { this["ReportRepository"] = value; }
        }

        [ConfigurationProperty("ShowIturERP", DefaultValue = DefaultShowIturERP, IsRequired = false)]
        public bool ShowIturERP
        {
            get { return (bool)this["ShowIturERP"]; }
            set { this["ShowIturERP"] = value; }
        }

		[ConfigurationProperty("PackDataFileCatalog", DefaultValue = DefaultPackDataFileCatalog, IsRequired = false)]
		public bool PackDataFileCatalog
        {
			get { return (bool)this["PackDataFileCatalog"]; }
			set { this["PackDataFileCatalog"] = value; }
        }
		

        [ConfigurationProperty("IturFilterSelected", DefaultValue = DefaultIturFilterSelected, IsRequired = false)]
        public string IturFilterSelected
        {
            get { return (string)this["IturFilterSelected"]; }
            set { this["IturFilterSelected"] = value; }
        }

		[ConfigurationProperty("IturFilterSortSelected", DefaultValue = DefaultIturFilterSortSelected, IsRequired = false)]
		public string IturFilterSortSelected
		{
			get { return (string)this["IturFilterSortSelected"]; }
			set { this["IturFilterSortSelected"] = value; }
		}

		[ConfigurationProperty("IturFilterSortAZSelected", DefaultValue = DefaultIturFilterSortAZSelected, IsRequired = false)]
		public string IturFilterSortAZSelected
		{
			get { return (string)this["IturFilterSortAZSelected"]; }
			set { this["IturFilterSortAZSelected"] = value; }
		}

        [ConfigurationProperty("InventProductFilterFocus", DefaultValue = DefaultInventProductFilterFocus, IsRequired = false)]
        public string InventProductFilterFocus
        {
            get { return (string)this["InventProductFilterFocus"]; }
            set { this["InventProductFilterFocus"] = value; }
        }

        [ConfigurationProperty("ReportAppName", DefaultValue = DefaultReportAppName, IsRequired = false)]
        public string ReportAppName
        {
            get { return (string)this["ReportAppName"]; }
            set { this["ReportAppName"] = value; }
        }

        [ConfigurationProperty("PlanEmptyColor", DefaultValue = DefaultPlanEmptyColor, IsRequired = false)]
        public string PlanEmptyColor
        {
            get { return (string)this["PlanEmptyColor"]; }
            set { this["PlanEmptyColor"] = value; }
        }

        [ConfigurationProperty("PlanZeroColor", DefaultValue = DefaultPlanZeroColor, IsRequired = false)]
        public string PlanZeroColor
        {
            get { return (string)this["PlanZeroColor"]; }
            set { this["PlanZeroColor"] = value; }
        }

        [ConfigurationProperty("PlanHundredColor", DefaultValue = DefaultPlanHundredColor, IsRequired = false)]
        public string PlanHundredColor
        {
            get { return (string)this["PlanHundredColor"]; }
            set { this["PlanHundredColor"] = value; }
        }

		 [ConfigurationProperty("InventProductMarkColor", DefaultValue = DefaultInventProductMarkColor, IsRequired = false)]
		public string InventProductMarkColor
        {
			get { return (string)this["InventProductMarkColor"]; }
			set { this["InventProductMarkColor"] = value; }
        }

		

        [ConfigurationProperty("UploadWakeupTime", DefaultValue = DefaultUploadWakeupTime, IsRequired = false)]
        public int UploadWakeupTime
        {
            get { return (int)this["UploadWakeupTime"]; }
            set { this["UploadWakeupTime"] = value; }
        }

		[ConfigurationProperty("UploadOptionsHT630_BaudratePDAItem", DefaultValue = DefaultUploadOptionsHT630_BaudratePDAItem, IsRequired = false)]
		public string UploadOptionsHT630_BaudratePDAItem
		{
			get { return (string)this["UploadOptionsHT630_BaudratePDAItem"]; }
			set { this["UploadOptionsHT630_BaudratePDAItem"] = value; }
		}

		[ConfigurationProperty("UploadOptionsRunMemoryItem", DefaultValue = DefaultUploadOptionsRunMemoryItem, IsRequired = false)]
		public string UploadOptionsRunMemoryItem
		{
			get { return (string)this["UploadOptionsRunMemoryItem"]; }
			set { this["UploadOptionsRunMemoryItem"] = value; }
		}

		[ConfigurationProperty("InventProductPropertyMarkSelectedItem", DefaultValue = DefaultInventProductPropertyMarkSelectedItem, IsRequired = false)]
		public string InventProductPropertyMarkSelectedItem
		{
			get { return (string)this["InventProductPropertyMarkSelectedItem"]; }
			set { this["InventProductPropertyMarkSelectedItem"] = value; }
		}

		[ConfigurationProperty("DomainObjectSelectedItem", DefaultValue = DefaultDomainObjectSelectedItem, IsRequired = false)]
		public string DomainObjectSelectedItem
		{
			get { return (string)this["DomainObjectSelectedItem"]; }
			set { this["DomainObjectSelectedItem"] = value; }
		}


		[ConfigurationProperty("InventProductPropertyFilterSelectedItem", DefaultValue = DefaultInventProductPropertyFilterSelectedItem, IsRequired = false)]
		public string InventProductPropertyFilterSelectedItem
		{
			get { return (string)this["InventProductPropertyFilterSelectedItem"]; }
			set { this["InventProductPropertyFilterSelectedItem"] = value; }
		}

		[ConfigurationProperty("InventProductPropertyFilterSelectedNumberItem", DefaultValue = DefaultInventProductPropertyFilterSelectedNumberItem, IsRequired = false)]
		public string InventProductPropertyFilterSelectedNumberItem
		{
			get { return (string)this["InventProductPropertyFilterSelectedNumberItem"]; }
			set { this["InventProductPropertyFilterSelectedNumberItem"] = value; }
		}


		[ConfigurationProperty("InventProductPropertyPhotoSelectedItem", DefaultValue = DefaultInventProductPropertyPhotoSelectedItem, IsRequired = false)]
		public string InventProductPropertyPhotoSelectedItem
		{
			get { return (string)this["InventProductPropertyPhotoSelectedItem"]; }
			set { this["InventProductPropertyPhotoSelectedItem"] = value; }
		}

		[ConfigurationProperty("InventProductPropertySelectedItem", DefaultValue = DefaultInventProductPropertySelectedItem, IsRequired = false)]
		public string InventProductPropertySelectedItem
		{
			get { return (string)this["InventProductPropertySelectedItem"]; }
			set { this["InventProductPropertySelectedItem"] = value; }
		}

		[ConfigurationProperty("EditorTemplateSelectedItem", DefaultValue = DefaultEditorTemplateSelectedItem, IsRequired = false)]
		public string EditorTemplateSelectedItem
		{
			get { return (string)this["EditorTemplateSelectedItem"]; }
			set { this["EditorTemplateSelectedItem"] = value; }
		}

		[ConfigurationProperty("UploadOptionsHT630_AfterUploadRunExeFileList", DefaultValue = DefaultUploadOptionsHT630_AfterUploadRunExeFileList, IsRequired = false)]
		public string UploadOptionsHT630_AfterUploadRunExeFileList
		{
			get { return (string)this["UploadOptionsHT630_AfterUploadRunExeFileList"]; }
			set { this["UploadOptionsHT630_AfterUploadRunExeFileList"] = value; }
		}

		[ConfigurationProperty("UploadOptionsHT630_CurrentDataPDA", DefaultValue = DefaultUploadOptionsHT630_CurrentDataPDA, IsRequired = false)]
		public bool UploadOptionsHT630_CurrentDataPDA
		{
			get { return (bool)this["UploadOptionsHT630_CurrentDataPDA"]; }
			set { this["UploadOptionsHT630_CurrentDataPDA"] = value; }
		}

		[ConfigurationProperty("UploadOptionsHT630_BaudratePDA", DefaultValue = DefaultUploadOptionsHT630_BaudratePDA, IsRequired = false)]
		public bool UploadOptionsHT630_BaudratePDA
		{
			get { return (bool)this["UploadOptionsHT630_BaudratePDA"]; }
			set { this["UploadOptionsHT630_BaudratePDA"] = value; }
		}

		

		[ConfigurationProperty("UploadOptionsHT630_DeleteAllFilePDA", DefaultValue = DefaultUploadOptionsHT630_DeleteAllFilePDA, IsRequired = false)]
		public bool UploadOptionsHT630_DeleteAllFilePDA
		{
			get { return (bool)this["UploadOptionsHT630_DeleteAllFilePDA"]; }
			set { this["UploadOptionsHT630_DeleteAllFilePDA"] = value; }
		}

		[ConfigurationProperty("UploadOptionsHT630_ExeptionFileNotDelete", DefaultValue = DefaultUploadOptionsHT630_ExeptionFileNotDelete, IsRequired = false)]
		public string UploadOptionsHT630_ExeptionFileNotDelete
		{
			get { return (string)this["UploadOptionsHT630_ExeptionFileNotDelete"]; }
			set { this["UploadOptionsHT630_ExeptionFileNotDelete"] = value; }
		}

		[ConfigurationProperty("UploadOptionsHT630_AfterUploadPerformWarmStart", DefaultValue = DefaultUploadOptionsHT630_AfterUploadPerformWarmStart, IsRequired = false)]
		public bool UploadOptionsHT630_AfterUploadPerformWarmStart
		{
			get { return (bool)this["UploadOptionsHT630_AfterUploadPerformWarmStart"]; }
			set { this["UploadOptionsHT630_AfterUploadPerformWarmStart"] = value; }
		}

		[ConfigurationProperty("UploadOptionsHT630_AfterUploadRunExeFileNeedDo", DefaultValue = DefaultUploadOptionsHT630_AfterUploadRunExeFileNeedDo, IsRequired = false)]
		public bool UploadOptionsHT630_AfterUploadRunExeFileNeedDo
		{
			get { return (bool)this["UploadOptionsHT630_AfterUploadRunExeFileNeedDo"]; }
			set { this["UploadOptionsHT630_AfterUploadRunExeFileNeedDo"] = value; }
		}

		[ConfigurationProperty("CopyFromSource", DefaultValue = DefaultCopyFromSource, IsRequired = false)]
		public bool CopyFromSource
		{
			get { return (bool)this["CopyFromSource"]; }
			set { this["CopyFromSource"] = value; }
		}


		[ConfigurationProperty("SendToFtpOffice", DefaultValue = DefaultSendToFtpOffice, IsRequired = false)]
		public bool SendToFtpOffice
		{
			get { return (bool)this["SendToFtpOffice"]; }
			set { this["SendToFtpOffice"] = value; }
		}

		[ConfigurationProperty("ForwardResendData", DefaultValue = DefaultForwardResendData, IsRequired = false)]
		public bool ForwardResendData
		{
			get { return (bool)this["ForwardResendData"]; }
			set { this["ForwardResendData"] = value; }
		}


		[ConfigurationProperty("TagSubstring", DefaultValue = DefaultTagSubstring, IsRequired = false)]
		public bool TagSubstring
		{
			get { return (bool)this["TagSubstring"]; }
			set { this["TagSubstring"] = value; }
		}

		[ConfigurationProperty("CopyFromHost", DefaultValue = DefaultCopyFromHost, IsRequired = false)]
		public bool CopyFromHost
		{
			get { return (bool)this["CopyFromHost"]; }
			set { this["CopyFromHost"] = value; }
		}
		

		[ConfigurationProperty("CopyByCodeInventor", DefaultValue = DefaultCopyByCodeInventor, IsRequired = false)]
		public bool CopyByCodeInventor
		{
			get { return (bool)this["CopyByCodeInventor"]; }
			set { this["CopyByCodeInventor"] = value; }
		}

		[ConfigurationProperty("CountingFromSource", DefaultValue = DefaultCountingFromSource, IsRequired = false)]
		public bool CountingFromSource
		{
			get { return (bool)this["CountingFromSource"]; }
			set { this["CountingFromSource"] = value; }
		}


		[ConfigurationProperty("ShowMark", DefaultValue = DefaultShowMark, IsRequired = false)]
		public bool ShowMark
		{
			get { return (bool)this["ShowMark"]; }
			set { this["ShowMark"] = value; }
		}

		[ConfigurationProperty("PropertyIsEmpty", DefaultValue = DefaultPropertyIsEmpty, IsRequired = false)]
		public bool PropertyIsEmpty
		{
			get { return (bool)this["PropertyIsEmpty"]; }
			set { this["PropertyIsEmpty"] = value; }
		}
		

		[ConfigurationProperty("ImportPDAPath", DefaultValue = DefaultImportPDAPath, IsRequired = false)]
		public string ImportPDAPath
		{
			get { return (string)this["ImportPDAPath"]; }
			set { this["ImportPDAPath"] = value; }
		}

        [ConfigurationProperty("ImportTCPPath", DefaultValue = DefaultImportTCPPath, IsRequired = false)]
        public string ImportTCPPath
        {
            get { return (string)this["ImportTCPPath"]; }
            set { this["ImportTCPPath"] = value; }
        }
        [ConfigurationProperty("TcpServerPort", DefaultValue = DefaultTcpServerPort, IsRequired = false)]
        public string TcpServerPort
        {
            get { return (string)this["TcpServerPort"]; }
            set { this["TcpServerPort"] = value; }
        }

        

        [ConfigurationProperty("WebServiceLink", DefaultValue = DefaultWebServiceLink, IsRequired = false)]
        public string WebServiceLink
        {
            get { return (string)this["WebServiceLink"]; }
            set { this["WebServiceLink"] = value; }
        }


        
         [ConfigurationProperty("UseToo", DefaultValue = DefaultUseToo, IsRequired = false)]
        public bool UseToo
        {
            get { return (bool)this["UseToo"]; }
            set { this["UseToo"] = value; }
        }


        [ConfigurationProperty("TcpServerOn", DefaultValue = DefaultTcpServerOn, IsRequired = false)]
        public bool TcpServerOn
        {
            get { return (bool)this["TcpServerOn"]; }
            set { this["TcpServerOn"] = value; }
        }

        

        [ConfigurationProperty("WebServiceDeveloperLink", DefaultValue = DefaultWebServiceDeveloperLink, IsRequired = false)]
        public string WebServiceDeveloperLink
        {
            get { return (string)this["WebServiceDeveloperLink"]; }
            set { this["WebServiceDeveloperLink"] = value; }
        }


        [ConfigurationProperty("ExportPDAPath", DefaultValue = DefaultExportPDAPath, IsRequired = false)]
		public string ExportPDAPath
		{
			get { return (string)this["ExportPDAPath"]; }
			set { this["ExportPDAPath"] = value; }
		}

        [ConfigurationProperty("ExportTCPPath", DefaultValue = DefaultExportTCPPath, IsRequired = false)]
        public string ExportTCPPath
        {
            get { return (string)this["ExportTCPPath"]; }
            set { this["ExportTCPPath"] = value; }
        }

        [ConfigurationProperty("Host", DefaultValue = DefaultHost, IsRequired = false)]
		public string Host
		{
			get { return (string)this["Host"]; }
			set { this["Host"] = value; }
		}

		[ConfigurationProperty("User", DefaultValue = DefaultUser, IsRequired = false)]
		public string User
		{
			get { return (string)this["User"]; }
			set { this["User"] = value; }
		}

		[ConfigurationProperty("Password", DefaultValue = DefaultPassword, IsRequired = false)]
		public string Password
		{
			get { return (string)this["Password"]; }
			set { this["Password"] = value; }
		}

//old
		[ConfigurationProperty("UploadOptionsHT630_AfterUploadRunExeFile", DefaultValue = DefaultUploadOptionsHT630_AfterUploadRunExeFile, IsRequired = false)]
		public string UploadOptionsHT630_AfterUploadRunExeFile
		{
			get { return (string)this["UploadOptionsHT630_AfterUploadRunExeFile"]; }
			set { this["UploadOptionsHT630_AfterUploadRunExeFile"] = value; }
		}

		


	}
}