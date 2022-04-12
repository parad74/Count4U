using System;
namespace Count4U.Model.Interface
{
    /// <summary>
    /// Интерфейс для подключения к БД
	/// Interface connection to DB
    /// </summary>
    public interface IConnectionDB
    {

		//string ProductMakatBarcodesDictionaryCapacity { get; }
		//string MainDBConnectionString { get;  }
		//string AuditConnectionString { get;  }
		//string ProcessDBConnectionString { get; }

		//string BuildCount4UConnectionString(string subFolder);
		//string BuildAnalyticDBConnectionString(string subFolder);

		//string BuildAnalyticDBFilePath(string subFolder);
		//string BuildCount4UDBFilePath(string subFolder);

		//string BuildCount4UDBFolderPath(string subFolder);
		//string BuildAnalyticDBFolderPath(string subFolder);

		//string ImportFolderPath();
		//string ExportToPDAFolderPath();
		//string ExportToPDAFolder();
		////string RootFromFolderFtp(string subFolder = "");
		//string RootFolderFtp(string subFolder = "");
		//string RootComplexDataFolderFtp(string subFolder = "");
		//string FolderLogoPath();
		//string ExecutablePath();

		string ProductMakatBarcodesDictionaryCapacity { get; }
		string MainDBConnectionString { get; }
		string AuditConnectionString { get; }
		string ProcessDBConnectionString { get; }
		IDBSettings DBSettings { get; }
		string BuildCount4UConnectionString(string subFolder);
		string BuildAnalyticDBConnectionString(string subFolder);    //уже с	AnalyticDB.sdf
		string BuildCount4UDBFilePath(string subFolder);
		string BuildAnalyticDBFilePath(string subFolder);
		string BuildCount4UDBFolderPath(string subFolder);
		string BuildAnalyticDBFolderPath(string subFolder);
		string EmptyCount4UDBFilePath();
		string EmptyAnalyticDBFilePath();
		string EmptyAuditDBFilePath();
		string AuditDBFilePath(string processCode);
		string MainDBFilePath(string processCode);
		string EmptyMainDBFilePath();
		string SetupEmptyProcessDBFilePath();
		string ProcessDBFilePath();
		string EmptyCount4MobileDBFilePath();
		string ImportFolderPath();
		string ExportToPDAFolderPath();
		string ExportToPDAFolder();
		string RootFolderFtp(string subFolder = "");
		string RootCount4UFolderFtp(string subFolder = "");
		string RootComplexDataFolderFtp(string subFolder = "");
		string ExecutablePath();
		string FolderLogoPath();
		string RemoveDB(string dbPath, string folder, bool full = false);
		string CopyEmptyCount4UAndAnaliticDB(string dbPath, string folder);
		string CopyCount4UDB(string dbPath, string folder, string sourceDbPath);
		string CopyAnaliticDB(string dbPath, string folder, string sourceDbPath);
		string CopyEmptyCount4UAndAnaliticDB(string relativePath);
		string CopyEmptyAnaliticDB(string relativePath);
		string CopyEmptyAuditDBToProcess(string processCode);
		string CopyEmptyMainDBToProcess(string processCode);
		string CopyMainDBToProcess(string processCode);
		string ReplaceEmptyAnaliticDB(string relativePath);
		string CopyEmptyCount4UDB(string toFolderPath);
		string CopyFromSetupEmptyProcessDB();
		string CopyEmptyMobileDB(string toFolderPath);


	}
}
