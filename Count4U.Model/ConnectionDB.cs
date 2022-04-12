using System;
using System.Configuration;
using Count4U.Model.App_Data;
using Count4U.Model.Interface;
using System.Reflection;
using System.IO;
using NLog;
using Count4U.Model.Interface.ProcessC4U;
using Count4U.Localization;

namespace Count4U.Model
{
    public class ConnectionDB : IConnectionDB
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public string _configurationMainDBConnectionString;				//Я исправила для процессов ?????? может и не надо проверить	 protected readonly
        public  string _configurationAuditDBConnectionString;
		public string _configurationProcessDBConnectionString;
		

        private readonly IDBSettings _dbSettings;

		public ConnectionDB(IDBSettings dbSettings)
        {
            this._dbSettings = dbSettings;
			this._configurationProcessDBConnectionString = dbSettings.BuildProcessDbConnectionString();
			//string subProcess = @"Process\" + processRepository.GetProcessCode_InProcess();
			  //получить текущий процесс и конектить БД из него
			//string subProcess = @"Process\ProcessCode1";
			string subProcess = @"";
			if (string.IsNullOrWhiteSpace(dbSettings.SettingsRepository.ProcessCode) == false)
				subProcess = @"Process\" + dbSettings.SettingsRepository.ProcessCode;
			//string subProcess = @"";
			this._configurationMainDBConnectionString = dbSettings.BuildMainDBConnectionString(subProcess);
			this._configurationAuditDBConnectionString = dbSettings.BuildAuditDbConnectionString(subProcess);

        }

		 public string ProductMakatBarcodesDictionaryCapacity
		{
            get
            {
                return PropertiesSettings.ProductMakatBarcodesDictionaryCapacity;
            }
        }

        #region IConnection Members

        public string MainDBConnectionString
        {
            get
            {
                return this._configurationMainDBConnectionString;
            }
         //   set { this._configurationMainDBConnectionString = value; }
        }

        public string AuditConnectionString
        {
            get
            {
                return this._configurationAuditDBConnectionString;
            }
        }

		public string ProcessDBConnectionString
		{
			get
			{
				return this._configurationProcessDBConnectionString;
			}
		}

        public IDBSettings DBSettings
        {
            get { return this._dbSettings; }
        }

        public string BuildCount4UConnectionString(string subFolder)
        {
            return this._dbSettings.BuildCount4UConnectionString(subFolder);
        }


		public string BuildAnalyticDBConnectionString(string subFolder)	 //уже с	AnalyticDB.sdf
        {
			return this._dbSettings.BuildAnalyticDBConnectionString(subFolder);
        }

        //metadata=res://*/App_Data.Count4UDB.csdl|res://*/App_Data.Count4UDB.ssdl
        //|res://*/App_Data.Count4UDB.msl;provider=System.Data.SqlServerCe.4.0;
        //provider connection string="Data Source=|
        //DataDirectory|\..\..\..\Count4U.Model\App_Data\2011\7\24\d8c547d3-af9c-4e82-ba35-3d242dd31e0e\Count4UDB.sdf"
        public string BuildCount4UDBFilePath(string subFolder)
        {
            return this._dbSettings.BuildCount4UDBFilePath(subFolder);
        }

		public string BuildAnalyticDBFilePath(string subFolder)
		{
			return this._dbSettings.BuildAnalyticDBFilePath(subFolder);
		}

        public string BuildCount4UDBFolderPath(string subFolder)
        {
            return this._dbSettings.BuildCount4UDBFolderPath(subFolder);
        }

		public string BuildAnalyticDBFolderPath(string subFolder)
		{
			return this._dbSettings.BuildAnalyticDBFolderPath(subFolder);
		}

        public string EmptyCount4UDBFilePath()
        {
            return this._dbSettings.EmptyCount4UDBFilePath();
        }

		public string EmptyAnalyticDBFilePath()
		{
			return this._dbSettings.EmptyAnalyticDBFilePath();
		}

		public string EmptyAuditDBFilePath()
		{
			return this._dbSettings.EmptyAuditDBFilePath();
		}

		public string AuditDBFilePath(string processCode)
		{
			return this._dbSettings.AuditDBFilePath(processCode);
		}

		public string MainDBFilePath(string processCode)
		{
			return this._dbSettings.MainDBFilePath(processCode);
		}
		public string EmptyMainDBFilePath()
		{
			return this._dbSettings.EmptyMainDBFilePath();
		}

		public string SetupEmptyProcessDBFilePath()
		{
			return this._dbSettings.SetupEmptyProcessDBFilePath();
		}

		public string ProcessDBFilePath()
		{
			return this._dbSettings.ProcessDBFilePath();
		}


		public string EmptyCount4MobileDBFilePath()
        {
			return this._dbSettings.EmptyCount4MobileFilePath();
        }

        public string ImportFolderPath()
        {
            return this._dbSettings.ImportFolderPath();
        }

        public string ExportToPDAFolderPath()
        {
            return this._dbSettings.ExportToPdaFolderPath();
        }

		public string ExportToPDAFolder()
		{
			string exportToPDAFolder = PropertiesSettings.ExportToPDAFolder.Trim('\\');
			return exportToPDAFolder;
		}

		//now for Android 	 mINV
		public string RootFolderFtp(string subFolder = "")
		{
			string rootFolderFtp = PropertiesSettings.RootFolderFtp.Trim('\\');
			if (string.IsNullOrWhiteSpace(subFolder) == true) return rootFolderFtp;
			else
			{
				rootFolderFtp = rootFolderFtp + @"\" + subFolder.Trim('\\');
				return rootFolderFtp;
			}
		}

		//now for 	Count4U Send to Office
		public string RootCount4UFolderFtp(string subFolder = "")
		{
			string rootFolderFtp = PropertiesSettings.RootCount4UFolderFtp.Trim('\\');	//Count4U
			if (string.IsNullOrWhiteSpace(subFolder) == true) return rootFolderFtp;
			else
			{
				rootFolderFtp = rootFolderFtp + @"\" + subFolder.Trim('\\');
				return rootFolderFtp;
			}
		}

		//now for Android 	 mINV
		//public string RootFromFolderFtp(string subFolder = "")
		//{
		//	string rootFromFolderFtp = PropertiesSettings.RootFromFolderFtp.Trim('\\');

		//	if (string.IsNullOrWhiteSpace(subFolder) == true) return rootFromFolderFtp;
		//	else
		//	{
		//		rootFromFolderFtp = rootFromFolderFtp + @"\" + subFolder.Trim('\\');
		//		return rootFromFolderFtp;
		//	}
		//}

		public string RootComplexDataFolderFtp(string subFolder = "")
		{
			string rootComplexDataFolderFtp = PropertiesSettings.RootComplexDataFolderFtp.Trim('\\');
			if (string.IsNullOrWhiteSpace(subFolder) == true) return rootComplexDataFolderFtp;
			else
			{
				rootComplexDataFolderFtp = rootComplexDataFolderFtp + @"\" + subFolder.Trim('\\');
				return rootComplexDataFolderFtp;
			}
		}

        public string ExecutablePath()
        {
            return this._dbSettings.ExecutablePath();
        }

        public string FolderLogoPath()
        {
            return this._dbSettings.FolderLogoPath();
        }


        #endregion

        internal string BuildADOConnectionString(string subFolder)
        {
            return this._dbSettings.BuildADOConnectionString(subFolder);
        }


        public string RemoveDB(string dbPath, string folder, bool full = false)
		{
			string toFilePath = "";
            if (string.IsNullOrWhiteSpace(dbPath) == true) return "";
            //	string folderInventor = PropertiesSettings.FolderInventor.Trim('\\') + @"\";
            dbPath = folder.Trim('\\') + @"\" + dbPath.Trim('\\');
            if (String.IsNullOrWhiteSpace(dbPath) == true) return "";
            if (full == false)
            {
                string toFolderPath = this.BuildCount4UDBFolderPath(dbPath + @"\removed");
				string fromFolderPath = this.BuildCount4UDBFolderPath(dbPath);

				if (Directory.Exists(fromFolderPath) == true)	   // если есть  fromFolderPath	для CountDB
				{
					if (Directory.Exists(toFolderPath) == false)
					{
						try
						{
							Directory.CreateDirectory(toFolderPath);
						}
						catch (Exception exp)
						{
							_logger.ErrorException("RemoveDB\\CreateDirectory", exp);
						}
					}


					string fromFilePath = this.BuildCount4UDBFilePath(dbPath);
					toFilePath = this.BuildCount4UDBFilePath(dbPath + @"\removed");

					string fromAnaliticFilePath = this.BuildAnalyticDBFilePath(dbPath);
					string toAnaliticFilePath = this.BuildAnalyticDBFilePath(dbPath + @"\removed");

					try
					{
						if (File.Exists(fromFilePath) == true)	   // если есть  CountDB которую удаляют
						{
							if (File.Exists(toFilePath) == true)
							{
								File.Delete(toFilePath);
							}

							GC.Collect();

							File.Copy(fromFilePath, toFilePath);
							File.Delete(fromFilePath);
						}

						//--------- Analitic 
						if (File.Exists(fromAnaliticFilePath) == true)	   // если есть  CountDB которую удаляют
						{
							if (File.Exists(toAnaliticFilePath) == true)
							{
								File.Delete(toAnaliticFilePath);
							}

							GC.Collect();

							File.Copy(fromAnaliticFilePath, toAnaliticFilePath);
							File.Delete(fromAnaliticFilePath);
						}


					}
					catch (Exception exp)
					{
						_logger.ErrorException("RemoveDB", exp);
					}
				}			// end если есть  fromFolderPath	CountDB

                return toFilePath;
            }

            else  //full delete
            {
                string fromFilePath = this.BuildCount4UDBFilePath(dbPath);
				string fromAnaliticFilePath = this.BuildAnalyticDBFilePath(dbPath);
                if (File.Exists(fromFilePath) == true)
                {
                    File.Delete(fromFilePath);
                }

				if (File.Exists(fromAnaliticFilePath) == true)
                {
					File.Delete(fromAnaliticFilePath);
                }
                return "";
            }
        }

        public string CopyEmptyCount4UAndAnaliticDB(string dbPath, string folder)
        {
            string fromFilePath = String.Empty;
            string toFilePath = String.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(dbPath) == true) return "";
                //string folderInventor = PropertiesSettings.FolderInventor.Trim('\\') + @"\";
                dbPath = folder.Trim('\\') + @"\" + dbPath.Trim('\\');
                string toFolderPath = this.BuildCount4UDBFolderPath(dbPath);
                if (Directory.Exists(toFolderPath) == false)
                {
					try
					{
						Directory.CreateDirectory(toFolderPath);
					}
					catch (Exception ex)
					{
						_logger.ErrorException("CopyEmptyCount4UAndAnaliticDB", ex);
						return "";
					}
                }

                fromFilePath = this.EmptyCount4UDBFilePath();
                toFilePath = this.BuildCount4UDBFilePath(dbPath);
                if (File.Exists(toFilePath) == false)
                {
					File.Copy(fromFilePath, toFilePath, false);
                }
               

				 //=======  Analiticdb
				fromFilePath = this.EmptyAnalyticDBFilePath();
				toFilePath = this.BuildAnalyticDBFilePath(dbPath);
				if (File.Exists(toFilePath) == false)
				{
					File.Copy(fromFilePath, toFilePath, false);
				}
			
            }
            catch (Exception exc)
            {
				_logger.ErrorException("CopyEmptyCount4UAndAnaliticDB", exc);
				_logger.Error("CopyEmptyCount4UAndAnaliticDB, dbPath: {0}, folder: {1}, fromFilePath: {2}, toFilePath: {3}", dbPath, folder, fromFilePath, toFilePath);
                throw;
            }

            return dbPath;
        }

        public string CopyCount4UDB(string dbPath, string folder, string sourceDbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath) == true) return "";
            if (string.IsNullOrWhiteSpace(sourceDbPath) == true) return "";

            string toFilePath = String.Empty;
            try
            {
                dbPath = folder.Trim('\\') + @"\" + dbPath.Trim('\\');
                string toFolderPath = this.BuildCount4UDBFolderPath(dbPath);
                if (Directory.Exists(toFolderPath) == false)
                {
                    try
                    {
                        Directory.CreateDirectory(toFolderPath);
                    }
					catch (Exception ex)
					{
						_logger.ErrorException("CopyDB", ex);
						return "";
					}
                }

                toFilePath = this.BuildCount4UDBFilePath(dbPath);
                if (File.Exists(toFilePath) == true)
                {
                    File.Delete(toFilePath);
                }
                File.Copy(sourceDbPath, toFilePath, true);
            }
            catch (Exception exc)
            {
				toFilePath = "";
                _logger.ErrorException("CopyDB", exc);
                _logger.Error("CopyDB, dbPath: {0}, folder: {1}, sourceDbPath: {2}, toFilePath: {3}", dbPath, folder, sourceDbPath, toFilePath);
                throw;
            }

			return toFilePath;
        }

		public string CopyAnaliticDB(string dbPath, string folder, string sourceDbPath)
		{
			if (string.IsNullOrWhiteSpace(dbPath) == true) return "";
			if (string.IsNullOrWhiteSpace(sourceDbPath) == true) return "";

			string toFilePath = String.Empty;
			try
			{
				dbPath = folder.Trim('\\') + @"\" + dbPath.Trim('\\');
				string toFolderPath = this.BuildAnalyticDBFolderPath(dbPath);
				if (Directory.Exists(toFolderPath) == false)
				{
					try
					{
						Directory.CreateDirectory(toFolderPath);
					}
					catch (Exception ex)
					{
						_logger.ErrorException("CopyDB", ex);
						return "";
					}
				}

				
				toFilePath = this.BuildAnalyticDBFilePath(dbPath);
				if (File.Exists(toFilePath) == false)
				{
					File.Copy(sourceDbPath, toFilePath, false);
				}
				
			}
			catch (Exception exc)
			{
				toFilePath = "";
				_logger.ErrorException("CopyDB", exc);
				_logger.Error("CopyDB, dbPath: {0}, folder: {1}, sourceDbPath: {2}, toFilePath: {3}", dbPath, folder, sourceDbPath, toFilePath);
				throw;
			}

			return toFilePath;
		}

		public string CopyEmptyCount4UAndAnaliticDB(string relativePath)			   
        {
			if (string.IsNullOrWhiteSpace(relativePath) == true) return "";
			string toFolderPath = BuildCount4UDBFilePath(relativePath);
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";
			string folder = Path.GetDirectoryName(toFolderPath);

			try
			{
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyCount4UAndAnaliticDB", exp);
						return "";
					}
				}

				// ======== Count4Udb
				string fromCount4UDFilePath = this.EmptyCount4UDBFilePath();
				string toCount4UDPath = this.BuildCount4UDBFilePath(relativePath);
				if (File.Exists(toCount4UDPath) == false)			//копируем, только если нет файла
				{
					File.Copy(fromCount4UDFilePath, toCount4UDPath, false);
				}

				//=======  Analiticdb
				string fromAnalyticDBFilePath = this.EmptyAnalyticDBFilePath();
				string toAnalyticDBFilePath = this.BuildAnalyticDBFilePath(relativePath);
				if (File.Exists(toAnalyticDBFilePath) == false)			//копируем, только если нет файла
				{
					File.Copy(fromAnalyticDBFilePath, toAnalyticDBFilePath, false);
				}
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyEmptyCount4UAndAnaliticDB", exp);
				_logger.Error("CopyEmptyCount4UAndAnaliticDB,  toFolderPath: {1}", toFolderPath);
				throw;
			}

			return folder;
        }


		public string CopyEmptyAnaliticDB(string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath) == true) return "";
			string toFolderPath = BuildAnalyticDBFilePath(relativePath);
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";
			string fromFilePath = "";
			string toAnalyticDBFilePath = "";
			try
			{
				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyAnaliticDB", exp);
						return "";
					}
				}

				fromFilePath = this.EmptyAnalyticDBFilePath();
				toAnalyticDBFilePath = this.BuildAnalyticDBFilePath(relativePath);
				if (File.Exists(toAnalyticDBFilePath) == false)			//копируем, только если нет файла
				{
					File.Copy(fromFilePath, toAnalyticDBFilePath, false);
				}
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyEmptyAnaliticDB", exp);
				_logger.Error("CopyEmptyAnaliticDB, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toAnalyticDBFilePath);
				throw;
			}

			return toFolderPath;
		}


		public string CopyEmptyAuditDBToProcess(string processCode)
		{
			if (string.IsNullOrWhiteSpace(processCode) == true) return "";
			string toFolderPath = AuditDBFilePath(processCode);
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";
			string fromFilePath = "";
			string toAuditDBFilePath = "";
			try
			{
				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyAuditDBToProcess", exp);
						return "";
					}
				}

				fromFilePath = this.EmptyAuditDBFilePath();
				toAuditDBFilePath = this.AuditDBFilePath(processCode);
				if (File.Exists(toAuditDBFilePath) == false)			//копируем, только если нет файла
				{
					File.Copy(fromFilePath, toAuditDBFilePath, false);
				}
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyEmptyAuditDBToProcess", exp);
				_logger.Error("CopyEmptyAuditDBToProcess, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toAuditDBFilePath);
				throw;
			}
			return toFolderPath;
		}


		public string CopyEmptyMainDBToProcess(string processCode)
		{
			if (string.IsNullOrWhiteSpace(processCode) == true) return "";
			string toFolderPath = MainDBFilePath(processCode);
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";
			string fromFilePath = "";
			string toMainDBFilePath = "";
			try
			{
				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyMainDBToProcess", exp);
						return "";
					}
				}

				fromFilePath = this.EmptyMainDBFilePath();
				toMainDBFilePath = this.MainDBFilePath(processCode);
				if (File.Exists(toMainDBFilePath) == false)			//копируем, только если нет файла
				{
					File.Copy(fromFilePath, toMainDBFilePath, false);
				}
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyEmptyMainDBToProcess", exp);
				_logger.Error("CopyEmptyMainDBToProcess, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toMainDBFilePath);
				throw;
			}
			return toFolderPath;
		}

		public string CopyMainDBToProcess(string processCode)
		{
			if (string.IsNullOrWhiteSpace(processCode) == true) return "";
			string toFolderPath = MainDBFilePath(processCode);
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";
			string fromFilePath = "";
			string toMainDBFilePath = "";
			try
			{
				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyMainDBToProcess", exp);
						return "";
					}
				}

				fromFilePath = this.MainDBFilePath(""); //из APP_DATA
				toMainDBFilePath = this.MainDBFilePath(processCode);
				if (File.Exists(toMainDBFilePath) == false)			//копируем, только если нет файла
				{
					File.Copy(fromFilePath, toMainDBFilePath, false);
				}
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyMainDBToProcess", exp);
				_logger.Error("CopyMainDBToProcess, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toMainDBFilePath);
				throw;
			}
			return toFolderPath;
		}

		public string ReplaceEmptyAnaliticDB(string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath) == true) return "";
			string toFolderPath = BuildAnalyticDBFilePath(relativePath);
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";
			string fromFilePath = "";
			string toAnalyticDBFilePath = "";
			try
			{
				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyAnaliticDB", exp);
						return "";
					}
				}

				fromFilePath = this.EmptyAnalyticDBFilePath();
				toAnalyticDBFilePath = this.BuildAnalyticDBFilePath(relativePath);
				if (File.Exists(toAnalyticDBFilePath) == true)			//зфменяем файл
				{
					File.Delete(toAnalyticDBFilePath);
				}
				File.Copy(fromFilePath, toAnalyticDBFilePath, true);
			}
			catch (Exception exp)
			{
				_logger.ErrorException("ReplaceEmptyAnaliticDB", exp);
				_logger.Error("ReplaceEmptyAnaliticDB, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toAnalyticDBFilePath);
				throw;
			}

			return toFolderPath;
		}

		public string CopyEmptyCount4UDB(string toFolderPath)
		{
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";

			string fromFilePath = String.Empty;
			try
			{
				fromFilePath = this.EmptyCount4UDBFilePath();
				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyCount4UAndAnaliticDB", exp);
						return "";
					}
				}

				File.Copy(fromFilePath, toFolderPath, true);

			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyEmptyCount4UDB", exp);
				_logger.Error("CopyEmptyCount4UDB, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toFolderPath);
				throw;
			}

			return toFolderPath;
		}

		public string CopyFromSetupEmptyProcessDB()
		{
			string toFolderPath = this.ProcessDBFilePath();
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";
			if (File.Exists(toFolderPath) == true) return toFolderPath;
			string fromEmptyProcessDBFilePath = String.Empty;
			try
			{
				fromEmptyProcessDBFilePath = this.SetupEmptyProcessDBFilePath();
				if (File.Exists(fromEmptyProcessDBFilePath) == false) return String.Empty;

				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyFromSetupEmptyProcessDB", exp);
						return "";
					}
				}

				File.Copy(fromEmptyProcessDBFilePath, toFolderPath, false);
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyFromSetupEmptyProcessDB", exp);
				_logger.Error("CopyFromSetupEmptyProcessDB, fromFilePath: {0}, toFolderPath: {1}", fromEmptyProcessDBFilePath, toFolderPath);
				throw;
			}

			return toFolderPath;
		}

		public string CopyEmptyMobileDB(string toFolderPath)
		{
			if (string.IsNullOrWhiteSpace(toFolderPath) == true) return "";

			string fromFilePath = String.Empty;
			try
			{
				fromFilePath = this.EmptyCount4MobileDBFilePath();		  //EmptyCount4MobileDBFilePath

				string folder = Path.GetDirectoryName(toFolderPath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyMobileDB", exp);
						return "";
					}
				}

				File.Copy(fromFilePath, toFolderPath, true);
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyEmptyMobileDB", exp);
				_logger.Error("CopyEmptyMobileDB, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toFolderPath);
				throw;
			}

			return toFolderPath;
		}

	
	}
}
