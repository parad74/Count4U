﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Count4U.Model.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("metadata=res://*/App_Data.AuditDB.csdl|res://*/App_Data.AuditDB.ssdl|res://*/App_" +
            "Data.AuditDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string" +
            "=\"Data Source={0}\"")]
        public string AuditDBConnectionString {
            get {
                return ((string)(this["AuditDBConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute(@"metadata=res://*/App_Data.Count4UDB.csdl|res://*/App_Data.Count4UDB.ssdl|res://*/App_Data.Count4UDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0};Default Lock Timeout=60000;  Max Database Size = {1}; Max Buffer Size = {2}""")]
        public string Count4UDBConnectionString {
            get {
                return ((string)(this["Count4UDBConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("metadata=res://*/App_Data.MainDB.csdl|res://*/App_Data.MainDB.ssdl|res://*/App_Da" +
            "ta.MainDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=\"D" +
            "ata Source={0}\"")]
        public string MainDBConnectionString {
            get {
                return ((string)(this["MainDBConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("App_Data")]
        public string FolderApp_Data {
            get {
                return ((string)(this["FolderApp_Data"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Count4UDB.sdf")]
        public string Count4UDBFile {
            get {
                return ((string)(this["Count4UDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EmptyCount4UDB.sdf")]
        public string EmptyCount4UDBFile {
            get {
                return ((string)(this["EmptyCount4UDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Temp\\testDB\\App_Data\\EmptyCount4UDB.sdf")]
        public string EmptyCount4UDBPath {
            get {
                return ((string)(this["EmptyCount4UDBPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\..\\..\\..\\Count4U.Model\\")]
        public string DebugDbPath {
            get {
                return ((string)(this["DebugDbPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Inventor")]
        public string FolderInventor {
            get {
                return ((string)(this["FolderInventor"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ImportData")]
        public string FolderImport {
            get {
                return ((string)(this["FolderImport"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\..\\..\\..\\Count4U.Model\\")]
        public string DebugImportPath {
            get {
                return ((string)(this["DebugImportPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Customer")]
        public string FolderCustomer {
            get {
                return ((string)(this["FolderCustomer"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Branch")]
        public string FolderBranch {
            get {
                return ((string)(this["FolderBranch"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LogoFile")]
        public string FolderLogoFile {
            get {
                return ((string)(this["FolderLogoFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AuditDB.sdf")]
        public string AuditDBFile {
            get {
                return ((string)(this["AuditDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MainDB.sdf")]
        public string MainDBFile {
            get {
                return ((string)(this["MainDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SetupDb")]
        public string SetupDbFolder {
            get {
                return ((string)(this["SetupDbFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ReportTemplate")]
        public string ReportTemplateFolder {
            get {
                return ((string)(this["ReportTemplateFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\..\\..\\..\\Count4U.GenerationReport\\")]
        public string ReportModulePath {
            get {
                return ((string)(this["ReportModulePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ExportToPDA")]
        public string ExportToPDAFolder {
            get {
                return ((string)(this["ExportToPDAFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\..\\..\\..\\Count4U.Model\\")]
        public string DebugExportToPDAPath {
            get {
                return ((string)(this["DebugExportToPDAPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("UploadPDA")]
        public string UploadFolder {
            get {
                return ((string)(this["UploadFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string CustomerConfigHash {
            get {
                return ((string)(this["CustomerConfigHash"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public string CustomerConfigFileType {
            get {
                return ((string)(this["CustomerConfigFileType"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string CustomerConfigQType {
            get {
                return ((string)(this["CustomerConfigQType"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public string CustomerConfigUseAlphaKey {
            get {
                return ((string)(this["CustomerConfigUseAlphaKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string CustomerConfigClientID {
            get {
                return ((string)(this["CustomerConfigClientID"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string CustomerConfigNewItem {
            get {
                return ((string)(this["CustomerConfigNewItem"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ExportData")]
        public string FolderErpExport {
            get {
                return ((string)(this["FolderErpExport"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\..\\..\\..\\Count4U.Configuration\\DebugData")]
        public string DebugDataPath {
            get {
                return ((string)(this["DebugDataPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.2")]
        public double HomeOpacityBackground {
            get {
                return ((double)(this["HomeOpacityBackground"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.2")]
        public double CustomerOpacityBackground {
            get {
                return ((double)(this["CustomerOpacityBackground"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.2")]
        public double BranchOpacityBackground {
            get {
                return ((double)(this["BranchOpacityBackground"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.2")]
        public double InventorOpacityBackground {
            get {
                return ((double)(this["InventorOpacityBackground"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.2")]
        public double MainOpacityBackground {
            get {
                return ((double)(this["MainOpacityBackground"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Count4U\\trunk\\Count4U\\Count4U.Media\\Background\\Home.jpg")]
        public string HomeBackgroundFilePath {
            get {
                return ((string)(this["HomeBackgroundFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Count4U\\trunk\\Count4U\\Count4U.Media\\Background\\Branch.jpg")]
        public string BranchBackgroundFilePath {
            get {
                return ((string)(this["BranchBackgroundFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Count4U\\trunk\\Count4U\\Count4U.Media\\Background\\Inventor.jpg")]
        public string InventorBackgroundFilePath {
            get {
                return ((string)(this["InventorBackgroundFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Count4U\\trunk\\Count4U\\Count4U.Media\\Background\\Main.jpg")]
        public string MainBackgroundFilePath {
            get {
                return ((string)(this["MainBackgroundFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ITUR")]
        public string CustomerConfigIturNameType {
            get {
                return ((string)(this["CustomerConfigIturNameType"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ITUR")]
        public string CustomerConfigIturNamePrefix {
            get {
                return ((string)(this["CustomerConfigIturNamePrefix"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Count4U\\trunk\\Count4U\\Count4U.Media\\Background\\Customer.jpg")]
        public string CustomerBackgroundFilePath {
            get {
                return ((string)(this["CustomerBackgroundFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("84")]
        public int DBVer {
            get {
                return ((int)(this["DBVer"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000000")]
        public string ProductMakatBarcodesDictionaryCapacity {
            get {
                return ((string)(this["ProductMakatBarcodesDictionaryCapacity"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2048")]
        public string ConnectionEFMaxDatabaseSize {
            get {
                return ((string)(this["ConnectionEFMaxDatabaseSize"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4096")]
        public string ConnectionEFMaxBufferSize {
            get {
                return ((string)(this["ConnectionEFMaxBufferSize"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("650")]
        public string CustomerConfigPassword {
            get {
                return ((string)(this["CustomerConfigPassword"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\MIS\\IDnextData")]
        public string MISiDnextDataPath {
            get {
                return ((string)(this["MISiDnextDataPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\MIS\\MISCommunicator")]
        public string MISCommunicatorPath {
            get {
                return ((string)(this["MISCommunicatorPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("No")]
        public string CustomerConfigHTcalculateLookUp {
            get {
                return ((string)(this["CustomerConfigHTcalculateLookUp"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("true")]
        public string AddNewLocation {
            get {
                return ((string)(this["AddNewLocation"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("999")]
        public string MaxQuantity {
            get {
                return ((string)(this["MaxQuantity"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DD-MM-YY HH:MM")]
        public string LastSync {
            get {
                return ((string)(this["LastSync"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string AllowZeroQuantity {
            get {
                return ((string)(this["AllowZeroQuantity"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Count4MobileDbTemplete.db3")]
        public string EmptyCount4MobileDBFile {
            get {
                return ((string)(this["EmptyCount4MobileDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"normal\"")]
        public string SearchDef {
            get {
                return ((string)(this["SearchDef"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string CustomerConfigNewItemBool {
            get {
                return ((string)(this["CustomerConfigNewItemBool"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AnalyticDB.sdf")]
        public string AnalyticDBFile {
            get {
                return ((string)(this["AnalyticDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public string MaxLen {
            get {
                return ((string)(this["MaxLen"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"0\"")]
        public string CustomerConfigInvertPrefix {
            get {
                return ((string)(this["CustomerConfigInvertPrefix"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"true\"")]
        public string ConfirmNewLocation {
            get {
                return ((string)(this["ConfirmNewLocation"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"false\"")]
        public string ConfirmNewItem {
            get {
                return ((string)(this["ConfirmNewItem"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"0\"")]
        public string AutoSendData {
            get {
                return ((string)(this["AutoSendData"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"true\"")]
        public string AllowQuantityFraction {
            get {
                return ((string)(this["AllowQuantityFraction"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"false\"")]
        public string AddExtraInputValue {
            get {
                return ((string)(this["AddExtraInputValue"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"\"")]
        public string AddExtraInputValueHeaderName {
            get {
                return ((string)(this["AddExtraInputValueHeaderName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string AddExtraInputValueSelectFromBatchListForm {
            get {
                return ((string)(this["AddExtraInputValueSelectFromBatchListForm"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string AllowNewValueFromBatchListForm {
            get {
                return ((string)(this["AllowNewValueFromBatchListForm"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string SearchIfExistInBatchList {
            get {
                return ((string)(this["SearchIfExistInBatchList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string AllowMinusQuantity {
            get {
                return ((string)(this["AllowMinusQuantity"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string FractionCalculate {
            get {
                return ((string)(this["FractionCalculate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("mINV")]
        public string RootFolderFtp {
            get {
                return ((string)(this["RootFolderFtp"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("mINV\\ComplexData")]
        public string RootComplexDataFolderFtp {
            get {
                return ((string)(this["RootComplexDataFolderFtp"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        public string PartialQuantity {
            get {
                return ((string)(this["PartialQuantity"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.100.1")]
        public string Host1 {
            get {
                return ((string)(this["Host1"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.100.20")]
        public string Host2 {
            get {
                return ((string)(this["Host2"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public string Timeout {
            get {
                return ((string)(this["Timeout"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public string Retry {
            get {
                return ((string)(this["Retry"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("USB")]
        public string DefaultHost {
            get {
                return ((string)(this["DefaultHost"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string SameBarcodeInLocation {
            get {
                return ((string)(this["SameBarcodeInLocation"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("metadata=res://*/App_Data.ProcessDB.csdl|res://*/App_Data.ProcessDB.ssdl|res://*/" +
            "App_Data.ProcessDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection " +
            "string=\"Data Source={0}\"")]
        public string ProcessDBConnectionString {
            get {
                return ((string)(this["ProcessDBConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ProcessDB.sdf")]
        public string ProcessDBFile {
            get {
                return ((string)(this["ProcessDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("metadata=res://*/App_Data.AnalyticDB.csdl|res://*/App_Data.AnalyticDB.ssdl|res://" +
            "*/App_Data.AnalyticDB.msl;provider=System.Data.SqlServerCe.4.0;provider connecti" +
            "on string=\"Data Source={0}\"")]
        public string AnalyticDBConnectionString {
            get {
                return ((string)(this["AnalyticDBConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EmptyAnalyticDB.sdf")]
        public string EmptyAnalyticDBFile {
            get {
                return ((string)(this["EmptyAnalyticDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Temp\\testDB\\App_Data\\AnalyticDB.sdf")]
        public string EmptyAnalyticDBPath {
            get {
                return ((string)(this["EmptyAnalyticDBPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EmptyAuditDB.sdf")]
        public string EmptyAuditDBFile {
            get {
                return ((string)(this["EmptyAuditDBFile"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EmptyMainDB.sdf")]
        public string EmptyMainDBFile {
            get {
                return ((string)(this["EmptyMainDBFile"]));
            }
        }
    }
}
