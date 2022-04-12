using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Count4U.Model;
using System.Linq;
using Count4U.Model.Interface;
using Count4U.Model.Transfer;
using Ionic.Zip;
using NLog;

namespace Count4U.Modules.Prepare.Update
{
    class UpdaterPathInfo
    {
        public string ProgramFilesPath { get; set; }
        public string ProgramDataPath { get; set; }
        public string DbFolderName { get; set; }
        public string MainDbFullpath { get; set; }
		public string AuditDbFullpath { get; set; }
		public string Count4MobileDbTempleteFullpath { get; set; }
		public string EmptyCount4UdbFullpath { get; set; }
		public string EmptyAnalyticDbFullpath { get; set; }
		public string EmptyAuditDbFullpath { get; set; }
		public string EmptyMainDbFullpath { get; set; }
    }

    public class Updater
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        private readonly IZip _zip;

        public Updater(IDBSettings dbSettings, IZip zip)
        {
            this._zip = zip;
            this._dbSettings = dbSettings;
        }

        private UpdaterPathInfo FillUpdaterPathInfo()
        {
            string programFilesPath = FileSystem.FileWithAppPath(); //C:\Program Files (x86)\Count4U\

            string programDataPath = FileSystem.FileWithProgramDataPath(); //C:\ProgramData\Count4U\

            string dbFolderName = FileSystem.GetNameOfDbFolder(); //App_Data            
            string mainDbFileName = _dbSettings.MainDBFile; //MainDB.sdf
			string auditDbFileName = _dbSettings.AuditDBFile; //AuditDB.sdf
			string count4MobileDbTempleteName = _dbSettings.EmptyCount4MobileDBFile; //count4MobileDbTemplete.sdf
			string emptyCount4UdbName = _dbSettings.EmptyCount4UDBFile; //emptyCount4Udb.sdf
			string emptyAnalyticDbName = _dbSettings.EmptyAnalyticDBFile; //EmptyAnalyticDB.sdf

			string emptyAuditDbName = _dbSettings.EmptyAuditDBFile; //"EmptyAuditDB.sdf";
			string emptyMainDbName = _dbSettings.EmptyMainDBFile; //"EmptyMainDB.sdf";

            string mainDbFullPath = Path.Combine(programDataPath, dbFolderName, mainDbFileName); //C:\ProgramData\Count4U\App_Data\MainDB.sdf 
			string auditDbFullPath = Path.Combine(programDataPath, dbFolderName, auditDbFileName); //C:\ProgramData\Count4U\App_Data\AuditDB.sdf 
			string count4MobileDbTempleteFullPath = Path.Combine(programDataPath, dbFolderName, count4MobileDbTempleteName); //C:\ProgramData\Count4U\App_Data\count4MobileDbTemplete.sdf 
			string emptyCount4UdbDbFullPath = Path.Combine(programDataPath, dbFolderName, emptyCount4UdbName); //C:\ProgramData\Count4U\App_Data\emptyCount4Udb.sdf 
			string emptyAnalyticDbFullPath = Path.Combine(programDataPath, dbFolderName, emptyAnalyticDbName); //C:\ProgramData\Count4U\App_Data\emptyAnalyticDb.sdf 

			string emptyAuditDbFullPath = Path.Combine(programDataPath, dbFolderName, emptyAuditDbName); //C:\ProgramData\Count4U\App_Data\emptyAuditDb.sdf 
			string emptyMainDbFullPath = Path.Combine(programDataPath, dbFolderName, emptyMainDbName); //C:\ProgramData\Count4U\App_Data\emptyMainDb.sdf 
											

            UpdaterPathInfo result = new UpdaterPathInfo();
            result.ProgramFilesPath = programFilesPath;
            result.DbFolderName = dbFolderName;
            result.MainDbFullpath = mainDbFullPath;
			result.AuditDbFullpath = auditDbFullPath;
			result.Count4MobileDbTempleteFullpath = count4MobileDbTempleteFullPath;
			result.EmptyCount4UdbFullpath = emptyCount4UdbDbFullPath;
            result.ProgramDataPath = programDataPath;
			result.EmptyAnalyticDbFullpath = emptyAnalyticDbFullPath;
			result.EmptyAuditDbFullpath = emptyAuditDbFullPath;
			result.EmptyMainDbFullpath = emptyMainDbFullPath;
			
            return result;
        }

		public bool IsCleanRun()
        {
            _logger.Info("Start of IsCleanRun");
            UpdaterPathInfo info = FillUpdaterPathInfo();

            _logger.Info("Binary files path: " + info.ProgramFilesPath);

            if (string.IsNullOrEmpty(info.ProgramFilesPath) || !Directory.Exists(info.ProgramFilesPath))
                throw new InvalidOperationException("Binary files path missing");

            _logger.Info("Application data files path: " + info.ProgramDataPath);


            if (string.IsNullOrEmpty(info.ProgramDataPath) || !Directory.Exists(info.ProgramDataPath))
                throw new InvalidOperationException("Program data path missing");

            _logger.Info("MainDB.sdf full path: " + info.MainDbFullpath);

			//new 	((не существует mainDB) или (не существует EmptyCount4UdbFullpath) или (не существует Count4MobileDbTempleteFullpath)) return true
			//bool mainDbFileExists = File.Exists(info.MainDbFullpath);
			//bool auditDbFileExists = File.Exists(info.AuditDbFullpath);
			//bool emptyCount4UdbFullpath =  File.Exists(info.EmptyCount4UdbFullpath);
			//bool count4MobileDbTempleteFileExists = File.Exists(info.Count4MobileDbTempleteFullpath);

			//if (mainDbFileExists == false 
			//	|| emptyCount4UdbFullpath == false 
			//	|| count4MobileDbTempleteFileExists == false
			//	|| auditDbFileExists == false 
			//	)
			//{
			//	return true;
			//}
			//return false;

			return true; // всегда при запуске приложения проверять наличие файлов

			//  old не существует mainDB return true
        //    return !File.Exists(info.MainDbFullpath);			 
        }

		// Копи DBs files		  , только в Release mode должно вызываться
		//  from programFilesAppData
		//  to  programDataAppData
        public void RunAppDataCopy(/*Action<string> updateStatus*/)
        {
#if DEBUG
#else
            _logger.Info("Start of RunAppDataCopy");
			UpdaterPathInfo info = FillUpdaterPathInfo();

            string programFilesAppData = Path.Combine(info.ProgramFilesPath, info.DbFolderName); //C:\Program Files (x86)\Count4U\App_Data
            if (!Directory.Exists(programFilesAppData))
                return;

            string programDataAppData = Path.Combine(info.ProgramDataPath, info.DbFolderName); ////C:\ProgramData\Count4U\App_Data\

            DirectoryInfo diSource = new DirectoryInfo(programFilesAppData);
            DirectoryInfo diTarget = new DirectoryInfo(programDataAppData);

            _logger.Info("Preparing files for first run  {0} {1}", programFilesAppData, programDataAppData);

			// Копи DBs files		  , только в Release mode должно вызываться
			// diSource : from programFilesAppData
			// diTarget : to  programDataAppData
			CopyDirectoryAppData(diSource, diTarget/*, updateStatus*/);

#endif
        }

		// Копи DBs files		  , только в Release mode должно вызываться
		// source : from programFilesAppData
		// destination: to  programDataAppData
		void CopyDirectoryAppData(DirectoryInfo source, DirectoryInfo destination/*, Action<string> updateStatus*/)
		{
			//updateStatus(destination.FullName); //test
			if (!destination.Exists)
			{
				destination.Create();
			}

			// пустые шаблоны db - копируются всегда
			string count4MobileDbTempleteName = _dbSettings.EmptyCount4MobileDBFile; //count4MobileDbTemplete.sdf
			string emptyCount4UdbName = _dbSettings.EmptyCount4UDBFile; //emptyCount4Udb.sdf
			string emptyAnalyticDbName = _dbSettings.EmptyAnalyticDBFile;

			// Copy all files.
			FileInfo[] files = source.GetFiles();
			foreach (FileInfo file in files)
			{
				if (file.Name == emptyCount4UdbName
					|| file.Name == count4MobileDbTempleteName
					|| file.Name == emptyAnalyticDbName)
				{
					file.CopyTo(Path.Combine(destination.FullName, file.Name), true);			 //empty files db  - переписывать всегда
				}
				else
				{
					string destinationFile = Path.Combine(destination.FullName, file.Name);
					if (File.Exists(destinationFile) == false)
					{
						//updateStatus(destinationFile);
						file.CopyTo(destinationFile);					  // переписывать только если нет этих db
						//updateStatus("");
					}
				}
			}

			// Process subdirectories.
			// вроде бы не используется никогда, по этому не использовать эту функцию 
			DirectoryInfo[] dirs = source.GetDirectories();
			foreach (DirectoryInfo dir in dirs)
			{
				// Get destination directory.
				string destinationDir = Path.Combine(destination.FullName, dir.Name);

				// Call CopyDirectory() recursively.
				CopyDirectoryAppData(dir, new DirectoryInfo(destinationDir)/*, updateStatus*/);
			}
		}

        public string BackupAppDataBeforeSchemeMigration(Action<double, string, string> progress/*, DbVersion dbVersion*/)
        {
#if DEBUG
#else
            _logger.Info("BackupAppDataBeforeSchemeMigration");

            try
            {
                string dbPath = this._zip.GetRootFolder(ZipRootFolder.Db).PathOnDisk;

                if (String.IsNullOrEmpty(dbPath) || !Directory.Exists(dbPath)) return String.Empty;
                DirectoryInfo di = new DirectoryInfo(dbPath);

                ZipPackInfo pack = new ZipPackInfo();
                pack.RootFolder = ZipRootFolder.Db;

                BackupDir(di, pack);

                if (pack.EmptyFolders.Any() || pack.Files.Any())
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    string archive = this._zip.Pack(new List<ZipPackInfo> { pack }, cts.Token, progress, null);

                    if (!String.IsNullOrEmpty(archive) && File.Exists(archive))
                    {
                        FileInfo fi = new FileInfo(archive);
                        if (fi.Directory == null)
                        {
                            _logger.Error("BackupAppDataBeforeSchemeMigration - archive parent directory is null");
                            return String.Empty;
                        }

                        const string fileName = "MigrateBackup.zip";
                        string finalPath = Path.Combine(fi.Directory.FullName, fileName);

                        if (File.Exists(finalPath))
                            File.Delete(finalPath);
                        File.Move(fi.FullName, finalPath);

                        _logger.Info("BackupAppDataBeforeSchemeMigration Done");

                        return finalPath;
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BackupAppDataBeforeSchemeMigration", exc);
                return String.Empty;
            }

#endif

            return String.Empty;
        }

        private void BackupDir(DirectoryInfo di, ZipPackInfo pack)
        {
            List<DirectoryInfo> dirs = di.EnumerateDirectories().ToList();
            List<FileInfo> files = di.EnumerateFiles().Where(r => this._zip.ValidExtensionsForDbFolder.Contains(r.Extension)).ToList();

            if (dirs.Count == 0 && files.Count == 0)
            {
                pack.EmptyFolders.Add(di);
                return;
            }
            else
            {
                foreach (FileInfo fileInfo in files)
                {
                    pack.Files.Add(fileInfo);
                }

                foreach (DirectoryInfo directoryInfo in dirs)
                {
                    BackupDir(directoryInfo, pack);
                }
            }

        }

        public void RestoreAppDataOnFailedDbMigration(Action<double, string, string> progress, string archivePath)
        {
#if DEBUG
#else
            if (String.IsNullOrEmpty(archivePath) || !File.Exists(archivePath))
                return;

            try
            {
                ZipUnpackInfo unpackInfo = new ZipUnpackInfo();
                unpackInfo.IsClearImportData = false;
                unpackInfo.IsOverwrite = true;
                unpackInfo.ZipFilePath = archivePath;

                using (ZipFile zip = new ZipFile(archivePath))
                {
                    ZipUnpackInfoItem unpackItem = new ZipUnpackInfoItem();
                    unpackItem.RootFolder = ZipRootFolder.Db;
                    unpackItem.Entries = zip.Entries.ToList();
                    unpackInfo.Items.Add(unpackItem);

                    CancellationTokenSource cts = new CancellationTokenSource();
                    this._zip.Unpack(unpackInfo, cts.Token, progress);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("RestoreAppDataOnFailedDbMigration", exc);
            }
#endif
        }
    }
}