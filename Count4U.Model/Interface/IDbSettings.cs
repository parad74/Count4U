using System.Configuration;
using Count4U.GenerationReport.Settings;

namespace Count4U.Model.Interface
{
    public interface IDBSettings
    {
        string Count4UDBFile { get; }
		ISettingsRepository SettingsRepository { get; }
        string MainDBFile { get; }
		string ProcessDBFile { get; }
		string AuditDBFile { get; }
		string AnalyticDBFile { get; }
		string EmptyCount4UDBFile { get; }
		string EmptyAnalyticDBFile { get; }
		string EmptyCount4MobileDBFile { get; }
		string EmptyAuditDBFile	 { get; }
		string EmptyMainDBFile { get; }
        string FolderInventor { get; }
        string FolderCustomer { get; }
        string FolderBranch { get; }
        string FolderLogoFile { get; }
        string FolderApp_Data { get; }
        string FolderImport { get; }
		string FolderSetupDb { get; }

		//string FolderInventorPath (string subFolder);
		//string FolderCustomerPath (string subFolder);
		//string FolderBranchPath(string subFolder);

		string ConnectionEFMaxDatabaseSize { get; }
		string ConnectionEFMaxBufferSize { get; }

        string BuildAppDataFolderPath();
		string MainDbSdfPath(string subProcess = "");

		string BuildMainDBConnectionString(string subProcess = "");
		string BuildAuditDbConnectionString(string subProcess = "");	// string subProcess = ""
		string BuildProcessDbConnectionString();
		//string ProcessCode_InProcess { get; set; }
		

        string BuildCount4UConnectionString(string subFolder);
		string BuildAnalyticDBConnectionString(string subFolder);	  //уже с	AnalyticDB.sdf
	
        string BuildCount4UDBFilePath(string subFolder);
		string BuildAnalyticDBFilePath(string subFolder);
		string AuditDBFilePath(string processCode);
		string MainDBFilePath(string processCode);

        string BuildCount4UDBFolderPath(string subFolder = null);
		string BuildAnalyticDBFolderPath(string subFolder = null);

		string SetupEmptyProcessDBFilePath();
		string ProcessDBFilePath();
		
        string EmptyCount4UDBFilePath();
		string EmptyCount4MobileFilePath();
		string EmptyAnalyticDBFilePath();
		string EmptyAuditDBFilePath();
		string EmptyMainDBFilePath();
	
        string BuildADOConnectionString(string subFolder);
        string BuildPathFileADO(string subFolder, string fileDB);
        string CheckDb();
        string ImportFolderPath();
        string FolderLogoPath();
        string ReportTemplatePath();
		string ReportTemplateRootPath();
        string ExportToPdaFolderPath();
        string ExecutablePath();
        string ExportErpFolderPath();

        string AdapterLinkTxtPath();

        string TerminalIDPath();
	

        //string OriginalConfigFolderPath();
        //string TargetConfigFolderPath();        
        string UIConfigSetFolderPath();
        string UIPropertySetFolderPath();
        string UIFilterTemplateSetFolderPath();
        string PlanogramPictureFolderPath();
		// Not use now
		//string AdapterDefaultConfigFolderPath();
		// Not use now
		//string GetOriginalAdapterDefaultParamFolderPath();

		string HomeBackgroundFilePath { get; }
		string CustomerBackgroundFilePath { get; }
		string BranchBackgroundFilePath { get; }
		string InventorBackgroundFilePath { get; }
		string MainBackgroundFilePath  { get; }
		double HomeOpacityBackground { get; }
		double CustomerOpacityBackground { get; }
		double BranchOpacityBackground { get; }
		double InventorOpacityBackground  { get; }
		double MainOpacityBackground { get; }
		string MISiDnextDataPath { get; }
		string MISCommunicatorPath { get; }
		
    }
}