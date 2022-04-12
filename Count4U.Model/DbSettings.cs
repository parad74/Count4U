using System;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Count4U.GenerationReport.Settings;
using Count4U.Model.App_Data;
using Count4U.Model.Interface;
using NLog;
using System.Linq;
using Count4U.Localization;	  

namespace Count4U.Model
{
    public class DBSettings : IDBSettings
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly ISettingsRepository _settingsRepository;


        public DBSettings(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

		public ISettingsRepository SettingsRepository
		{
			get { return _settingsRepository; }
		} 

		public string ConnectionEFMaxDatabaseSize
		{
			get { return PropertiesSettings.ConnectionEFMaxDatabaseSize; }
		}

		public string ConnectionEFMaxBufferSize
		{
			get { return PropertiesSettings.ConnectionEFMaxBufferSize; }
		}


		//	   <add name="Count4U.Model.Properties.Settings.AuditDBConnectionString"
		//  connectionString="metadata=res://*/App_Data.AuditDB.csdl|res://*/App_Data.AuditDB.ssdl|res://*/App_Data.AuditDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=&quot;Data Source={0}&quot;" />
        private string AuditDBConnectionString
        {
			get { return PropertiesSettings.AuditDBConnectionString; }
        }

		//<add name="Count4U.Model.Properties.Settings.AnalyticDBConnectionString"
		//  connectionString="metadata=res://*/App_Data.AnalyticDB.csdl|res://*/App_Data.AnalyticDB.ssdl|res://*/App_Data.AnalyticDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=&quot;Data Source={0}&quot;" />
		private string AnalyticDBConnectionString
        {
			get { return PropertiesSettings.AnalyticDBConnectionString; }
        }

	//<add name="Count4U.Model.Properties.Settings.ProcessDBConnectionString"
	//  connectionString="metadata=res://*/App_Data.ProcessDB.csdl|res://*/App_Data.ProcessDB.ssdl|res://*/App_Data.ProcessDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=&quot;Data Source={0}&quot;" />
		private string ProcessDBConnectionString
		{
			get { return PropertiesSettings.ProcessDBConnectionString; }
		}

		//<add name="Count4U.Model.Properties.Settings.Count4UDBConnectionString"
		//  connectionString="metadata=res://*/App_Data.Count4UDB.csdl|res://*/App_Data.Count4UDB.ssdl|res://*/App_Data.Count4UDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=&quot;Data Source={0};Default Lock Timeout=60000;  Max Database Size = {1}; Max Buffer Size = {2}&quot;" />
        private string Count4UDBConnectionString
        {
  			get { return PropertiesSettings.Count4UDBConnectionString; }
        }

		//<add name="Count4U.Model.Properties.Settings.MainDBConnectionString"
		//  connectionString="metadata=res://*/App_Data.MainDB.csdl|res://*/App_Data.MainDB.ssdl|res://*/App_Data.MainDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=&quot;Data Source={0}&quot;" />
        private string MainDbConnectionString
        {
			get { return PropertiesSettings.MainDBConnectionString; }
        }

        public string FolderApp_Data
        {
            get { return PropertiesSettings.FolderApp_Data.Trim('\\'); }
        }


		public string FolderSetupDb
        {
			get { return PropertiesSettings.SetupDbFolder.Trim('\\'); }
        }

        public string Count4UDBFile
        {
            get { return PropertiesSettings.Count4UDBFile.Trim('\\'); }
        }

		public string AuditDBFile
        {
            get { return PropertiesSettings.AuditDBFile.Trim('\\'); }
        }

        public string MainDBFile
        {
            get { return PropertiesSettings.MainDBFile.Trim('\\'); }
        }

		public string EmptyAuditDBFile
		{
			get { return PropertiesSettings.EmptyAuditDBFile.Trim('\\'); }
		}

		public string EmptyMainDBFile
		{
			get { return PropertiesSettings.EmptyMainDBFile.Trim('\\'); }
		}

		public string ProcessDBFile
		{
			get { return PropertiesSettings.ProcessDBFile.Trim('\\'); }
		}

        public string EmptyCount4UDBFile
        {
            get { return PropertiesSettings.EmptyCount4UDBFile.Trim('\\'); }
        }



		public string EmptyAnalyticDBFile
        {
            get { return PropertiesSettings.EmptyAnalyticDBFile.Trim('\\'); }
        }

		public string EmptyCount4MobileDBFile
		{
			get { return PropertiesSettings.EmptyCount4MobileDBFile.Trim('\\'); }
		}

		

		public string AnalyticDBFile
		{
			get { return PropertiesSettings.AnalyticDBFile.Trim('\\'); }
		}


        public string FolderLogoFile
        {
            get { return PropertiesSettings.FolderLogoFile.Trim('\\'); }
        }

        public string FolderCustomer
        {
            get { return PropertiesSettings.FolderCustomer.Trim('\\'); }
        }

        public string FolderBranch
        {
            get { return PropertiesSettings.FolderBranch.Trim('\\'); }
        }


        public string FolderInventor
        {
            get { return PropertiesSettings.FolderInventor.Trim('\\'); }
        }

        public string FolderImport
        {
            get { return PropertiesSettings.FolderImport.Trim('\\'); }
        }

        private string FolderErpExport
        {
            get { return PropertiesSettings.FolderErpExport.Trim('\\'); }
        }

        private string DebugDataPath
        {
            get { return PropertiesSettings.DebugDataPath.Trim('\\'); }
        }

        private string DebugImportPath
        {
            get { return PropertiesSettings.DebugImportPath.Trim('\\'); }
        }

        private string DebugDbPath
        {
            get
            {
                string debugDbPath = PropertiesSettings.DebugDbPath.Trim('\\');
                // debugDbPath = @"\" + debugDbPath + @"\";
                return debugDbPath;
            }
        }

        private string ReportModulePath
        {
            get
            {
                string reportModulePath = PropertiesSettings.ReportModulePath.Trim('\\');
                return reportModulePath;
            }
        }

        private string ReportTemplateFolder
        {
            get
            {
                string reportTemplateFolder = PropertiesSettings.ReportTemplateFolder.Trim('\\');
                return reportTemplateFolder;
            }
        }

        private string ExportToPDAFolder
        {
            get
            {
                string exportToPDAFolder = PropertiesSettings.ExportToPDAFolder.Trim('\\');
                return exportToPDAFolder;
            }
        }

        private string DebugExportToPDAPath
        {
            get { return PropertiesSettings.DebugExportToPDAPath.Trim('\\'); }
        }


        //public string FolderInventorPath (string subFolder){
        //}

        //public string FolderCustomerPath (string subFolder){
        //}

        //public string FolderBranchPath(string subFolder)
        //{
        //}


        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
		// ========== Count4UDB EF ConnectionString===========
		public string BuildCount4UConnectionString(string subFolder)
		{
			string file = this.Count4UDBFile;
			return BuildConnectionString(subFolder, file) ;
		}


		public string BuildConnectionString(string subFolder, string fileName = "Count4UDB.sdf")
        {
            string result = String.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(subFolder) == true)
                {
                    subFolder = @"\";
                }
                else
                {
                    subFolder = subFolder.Trim('\\');
                    subFolder = @"\" + subFolder + @"\";
                }

                string connectionString = this.Count4UDBConnectionString;
                //;Default Lock Timeout=6000
                string path;
				int maxDatabaseSize = 512;
				bool ret = int.TryParse(this.ConnectionEFMaxDatabaseSize, out maxDatabaseSize);
				int maxBufferSize = 1024;
				ret = int.TryParse(this.ConnectionEFMaxBufferSize, out maxBufferSize);

#if DEBUG

                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
                       DebugDbPath + @"\" +
                       String.Format("{0}{1}{2}",
                                     this.FolderApp_Data, //+ @"\",
                    // this.FolderInventor,
                                     subFolder,
									 fileName);
                                     //this.Count4UDBFile);
				result = String.Format(connectionString, path, maxDatabaseSize, maxBufferSize);

#else

                path = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" +
                       String.Format("{0}{1}{2}",
                                     this.FolderApp_Data,// + @"\" ,
                                     //this.FolderInventor,
                                     subFolder,
									 fileName);
                                    // this.Count4UDBFile);
				result = String.Format(connectionString, path, maxDatabaseSize, maxBufferSize);
                //result = String.Format(connectionString, path);

#endif

			}
            catch (Exception e)
            {
				_logger.ErrorException("BuildConnectionString :" + fileName, e);
            }

            return result;
        }

		// ========== AnaliticDb  EF ConnectionString===========
		public string BuildAnalyticDBConnectionString(string subFolder)	    //уже с	AnalyticDB.sdf
		{
			string file = this.AnalyticDBFile;
			return BuildAnalyticDBConnectionString(subFolder, file);
		}

		public string BuildAnalyticDBConnectionString(string subFolder, string fileName = "AnalyticDB.sdf")
		{
			string result = String.Empty;

			try
			{
				if (string.IsNullOrWhiteSpace(subFolder) == true)
				{
					subFolder = @"\";
				}
				else
				{
					subFolder = subFolder.Trim('\\');
					subFolder = @"\" + subFolder + @"\";
				}

				string connectionString = this.AnalyticDBConnectionString;
				//;Default Lock Timeout=6000
				string path;
				int maxDatabaseSize = 512;
				bool ret = int.TryParse(this.ConnectionEFMaxDatabaseSize, out maxDatabaseSize);
				int maxBufferSize = 1024;
				ret = int.TryParse(this.ConnectionEFMaxBufferSize, out maxBufferSize);

#if DEBUG

				path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
					   DebugDbPath + @"\" +
					   String.Format("{0}{1}{2}",
									 this.FolderApp_Data, //+ @"\",
					// this.FolderInventor,
									 subFolder,
									 fileName);
				//this.Count4UDBFile);
				result = String.Format(connectionString, path, maxDatabaseSize, maxBufferSize);

#else

                path = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" +
                       String.Format("{0}{1}{2}",
                                     this.FolderApp_Data,// + @"\" ,
                                     //this.FolderInventor,
                                     subFolder,
									 fileName);
                                    // this.Count4UDBFile);
				result = String.Format(connectionString, path, maxDatabaseSize, maxBufferSize);
                //result = String.Format(connectionString, path);

#endif

			}
			catch (Exception e)
			{
				_logger.ErrorException("BuildConnectionString :" + fileName, e);
			}

			return result;
		}

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
		public string BuildCount4UDBFilePath(string subFolder)
		{
			string file = this.Count4UDBFile;
			return BuildDBFilePath(subFolder, file);
		}

		public string BuildAnalyticDBFilePath(string subFolder)
		{
			string file = this.AnalyticDBFile;
			return BuildDBFilePath(subFolder, file);
		}


        public string BuildDBFilePath(string subFolder, string fileName = "Count4UDB.sdf")
        {
            string pathDBFile = "";

            try
            {
                if (string.IsNullOrWhiteSpace(subFolder) == true)
                {
                    subFolder = @"\";
                }
                else
                {
                    subFolder = subFolder.Trim('\\');

                    subFolder = @"\" + subFolder + @"\";
                }

#if DEBUG
                pathDBFile = String.Format("{0}{1}{2}{3}{4}",
                                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\",
                                            this.DebugDbPath + @"\",
                                            this.FolderApp_Data,// + @"\",
                    //this.FolderInventor, 
                                            subFolder,
											fileName);
                                            //this.Count4UDBFile);
#else
                pathDBFile = String.Format("{0}{1}{2}{3}",
                                           FileSystem.FileWithProgramDataPath().Trim('\\') + @"\",
                                           this.FolderApp_Data, //+ @"\",
                                           //this.FolderInventor,
                                           subFolder,
											fileName);
                                           //this.Count4UDBFile);
#endif
			}
            catch (Exception e)
            {
				_logger.ErrorException("BuildDBFilePath :" + fileName, e);
            }

            return pathDBFile;
        }

        public string BuildAppDataFolderPath()
        {
#if DEBUG
            string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
                                   this.DebugDbPath + @"\" +
                                   this.FolderApp_Data;

#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;

#endif
            DirectoryInfo di = new DirectoryInfo(appDataFolder);
            return di.FullName;
        }

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
		public string BuildAnalyticDBFolderPath(String subFolder = null)
		{
			return this.BuildCount4UDBFolderPath(subFolder);
		}

        public string BuildCount4UDBFolderPath(String subFolder = null)
        {
            string pathFolderDB = "";

            try
            {
                if (string.IsNullOrWhiteSpace(subFolder) == true)
                {
                    subFolder = @"\";
                }
                else
                {
                    subFolder = subFolder.Trim('\\');
                    subFolder = @"\" + subFolder + @"\";
                }

#if DEBUG
                string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
                                       this.DebugDbPath + @"\" +
                                       this.FolderApp_Data;

#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;

#endif


                if (Directory.Exists(appDataFolder) == false)
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                pathFolderDB = appDataFolder + subFolder;
            }
            catch (Exception e)
            {
                _logger.ErrorException("BuildCount4UDBFolderPath", e);
            }

            return pathFolderDB;
        }

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
        public string EmptyCount4UDBFilePath()
        {
            string retPath = "";
            try
            {

#if DEBUG
                string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
                                       this.DebugDbPath + @"\" + this.FolderApp_Data;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;
#endif


                if (Directory.Exists(appDataFolder) == false)
                {
                    Directory.CreateDirectory(appDataFolder);
                }
                retPath = appDataFolder + @"\" + this.EmptyCount4UDBFile;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("EmptyCount4UDBFilePath", exc);
            }
            return retPath;
        }


		public string EmptyAuditDBFilePath()
		{
			string retPath = "";
			try
			{

#if DEBUG
				string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
									   this.DebugDbPath + @"\" + this.FolderApp_Data;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;
#endif


				if (Directory.Exists(appDataFolder) == false)
				{
					Directory.CreateDirectory(appDataFolder);
				}
				retPath = appDataFolder + @"\" + this.EmptyAuditDBFile;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("EmptyAuditDBFilePath", exc);
			}
			return retPath;
		}

			public string EmptyMainDBFilePath()
		{
			string retPath = "";
			try
			{

#if DEBUG
				string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
									   this.DebugDbPath + @"\" + this.FolderApp_Data;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;
#endif


				if (Directory.Exists(appDataFolder) == false)
				{
					Directory.CreateDirectory(appDataFolder);
				}
				retPath = appDataFolder + @"\" + this.EmptyMainDBFile;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("EmptyMainDBFilePath", exc);
			}
			return retPath;
		}

		//==============
			public string AuditDBFilePath(string processCode)
			{
				string retPath = "";

				string folder = processCode.Trim().Trim('\\');
				if (string.IsNullOrWhiteSpace(folder) == false) { folder =@"\Process\" + folder; }
				else { folder = ""; }

				try
				{

#if DEBUG
					string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
										   this.DebugDbPath + @"\" + this.FolderApp_Data + folder;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data + folder;
#endif


					if (Directory.Exists(appDataFolder) == false)
					{
						Directory.CreateDirectory(appDataFolder);
					}
					retPath = appDataFolder + @"\" + this.AuditDBFile;
				}
				catch (Exception exc)
				{
					_logger.ErrorException("AuditDBFilePath", exc);
				}
				return retPath;
			}



			public string MainDBFilePath(string processCode)
			{
				string retPath = "";
				string folder = processCode.Trim().Trim('\\');
				if (string.IsNullOrWhiteSpace(folder) == false) { folder = @"\Process\" + folder; }
				else { folder = ""; }
				try
				{

#if DEBUG
					string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
										   this.DebugDbPath + @"\" + this.FolderApp_Data + folder;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data + folder;
#endif


					if (Directory.Exists(appDataFolder) == false)
					{
						Directory.CreateDirectory(appDataFolder);
					}
					retPath = appDataFolder + @"\" + this.MainDBFile;
				}
				catch (Exception exc)
				{
					_logger.ErrorException("MainDBFilePath", exc);
				}
				return retPath;
			}
		//==============

		public string EmptyAnalyticDBFilePath()	 
		{
			string retPath = "";
			try
			{

#if DEBUG
				string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
									   this.DebugDbPath + @"\" + this.FolderApp_Data;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;
#endif


				if (Directory.Exists(appDataFolder) == false)
				{
					Directory.CreateDirectory(appDataFolder);
				}
				retPath = appDataFolder + @"\" + this.EmptyAnalyticDBFile;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("EmptyAnalyticDBFilePath", exc);
			}
			return retPath;
		}



		/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
		public string SetupEmptyProcessDBFilePath()
        {
            string retPath = "";
            try
            {

#if DEBUG
                string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
									   this.DebugDbPath + @"\" + this.FolderApp_Data + @"\" + this.FolderSetupDb;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data+ @"\" + this.FolderSetupDb;
#endif


				if (Directory.Exists(appDataFolder) == false)
                {
                    Directory.CreateDirectory(appDataFolder);
                }
				retPath = appDataFolder + @"\" + this.ProcessDBFile;
            }
            catch (Exception exc)
            {
				_logger.ErrorException("SetupEmptyProcessDBFilePath", exc);
            }
            return retPath;
        }


		public string ProcessDBFilePath()
        {
            string retPath = "";
            try
            {

#if DEBUG
                string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
									   this.DebugDbPath + @"\" + this.FolderApp_Data;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;
#endif


				if (Directory.Exists(appDataFolder) == false)
                {
                    Directory.CreateDirectory(appDataFolder);
                }
				retPath = appDataFolder + @"\" + this.ProcessDBFile;
            }
            catch (Exception exc)
            {
				_logger.ErrorException("ProcessDBFilePath", exc);
            }
            return retPath;
        }

		/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
		public string EmptyCount4MobileFilePath()
		{
			string retPath = "";
			try
			{

#if DEBUG
				string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
									   this.DebugDbPath + @"\" + this.FolderApp_Data;
#else
                string appDataFolder = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.FolderApp_Data;
#endif


				if (Directory.Exists(appDataFolder) == false)
				{
					Directory.CreateDirectory(appDataFolder);
				}
				retPath = appDataFolder + @"\" + this.EmptyCount4MobileDBFile;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("EmptyCount4MobileFilePath", exc);
			}
			return retPath;
		}

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
        public string BuildADOConnectionString(string subFolder)
        {
            _logger.Info("BuildADOConnectionString");
            return BuildCount4UConnectionString(subFolder);
        }

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
        public string BuildPathFileADO(string subFolder, string fileDB)
        {
            string result = String.Empty;

            if (string.IsNullOrWhiteSpace(subFolder) == true)
            {
                subFolder = @"\";
            }
            else
            {
                subFolder = @"\" +
                                subFolder.Trim('\\') +
                                @"\";
            }
            try
            {
#if DEBUG
                result = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') +
                                        @"\" +
                                         this.DebugDbPath +
                                         @"\" +
                                         this.FolderApp_Data +
                    // @"\" +
                    //this.FolderInventor +
                    //@"\" +
                    //subFolder.Trim('\\') +
                    //@"\" +
                                        subFolder +
                    //this.Count4UDBFile;
                                         fileDB;
#else
                result = FileSystem.FileWithProgramDataPath().Trim('\\') +
										@"\" +
                                         this.FolderApp_Data +
                                         //@"\" +
                                         //this.FolderInventor +
										 //@"\" +
										 //subFolder +
										 //@"\" +
										subFolder +
                                         //this.Count4UDBFile;
										fileDB;
#endif

            }
            catch (Exception e)
            {
                _logger.ErrorException("BuildPathFileADO", e);
            }
            return result;
        }

		//TODO для pack	 - не сделанно
		public string MainDbSdfPath(string subProcess = "") //asis process должен работать как раньше при "" это корневая БД
        {
            string result = String.Empty;

			string folder = subProcess.Trim('\\');
			if (string.IsNullOrWhiteSpace(folder) == false) { folder = folder + @"\"; }
			else { folder = ""; }
#if DEBUG
			result = DebugDbPath + @"\App_Data\" + folder + @"MainDB.sdf";
#else
            result = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\App_Data\" + folder + @"MainDB.sdf";
#endif

			DirectoryInfo di = new DirectoryInfo(result);

            return di.FullName;
        }

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
		public string BuildMainDBConnectionString(string subProcess = "") //asis process должен работать как раньше при "" это корневая БД
        {
            string format = this.MainDbConnectionString;
			string folder = subProcess.Trim('\\');
			if (string.IsNullOrWhiteSpace(folder) == false) { folder = folder + @"\"; }
			else { folder = ""; }

#if DEBUG
			return String.Format(format, @"|DataDirectory|" + DebugDbPath + @"\App_Data\" + folder + @"MainDB.sdf");

#else
            return String.Format(format, FileSystem.FileWithProgramDataPath().Trim('\\') + @"\App_Data\" + folder + @"MainDB.sdf");
#endif
		}

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
        public string BuildAuditDbConnectionString(string subProcess = "") //asis process должен работать как раньше при "" это корневая БД
        {
            string format = this.AuditDBConnectionString;
			string folder = subProcess.Trim('\\');
			if (string.IsNullOrWhiteSpace(folder) == false)	   {   	folder = folder + @"\";	   	}
			else 	 {		   	folder = "";		}

#if DEBUG
            return String.Format(format, @"|DataDirectory|" + DebugDbPath + @"\App_Data\" + folder + @"AuditDB.sdf");
#else
            return String.Format(format, FileSystem.FileWithProgramDataPath().Trim('\\') + @"\App_Data\" + folder + @"AuditDB.sdf");
#endif
		}

		//ProcessDb одна 
		public string BuildProcessDbConnectionString()
        {
			string format = this.ProcessDBConnectionString;
   
#if DEBUG
			return String.Format(format, @"|DataDirectory|" + DebugDbPath + @"\App_Data\ProcessDB.sdf");
#else
            return String.Format(format, FileSystem.FileWithProgramDataPath().Trim('\\') + @"\App_Data\ProcessDB.sdf");
#endif
		}

		public string ProcessCode_InProcess{	get;	set;	}


		//Пока предположение, что достаточно проверить корневы БД, если будет усложнение  - будем проверять 
		// пока вроде достаточно
        public string CheckDb()
        {
            //empty db check
            string emptyFilePath = EmptyCount4UDBFilePath();
            if (!File.Exists(emptyFilePath))
                return "Empty database file is missed: " + emptyFilePath;

            _logger.Info("empty file path: " + emptyFilePath);

            //main, audit db check

            string connectionString = this.BuildMainDBConnectionString();
            try
            {
                using (MainDB mainDb = new MainDB(connectionString))
                {
                    var n = mainDb.Customer.Count();
                    _logger.Trace("customers count in main db: " + n);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("main db connection error: ", exc);
                _logger.ErrorException("main db connection error: ", exc.InnerException);
                //Localization.Resources.Err_ConnectionDBException %" Connection Error "
                return Localization.Resources.Err_ConnectionDBException + " [MainDB] :" + exc + Environment.NewLine + exc.InnerException;
            }

            _logger.Info("main db connection string: " + connectionString);


			connectionString = this.BuildProcessDbConnectionString();
			try
			{
				using (ProcessDB processDb = new ProcessDB(connectionString))
				{
					var n = processDb.ProcessDBIni.Count();
					_logger.Trace("ver count in Process db: " + n);
				}

				string inprocess = StatusAuditConfigEnum.InProcess.ToString();

				using (App_Data.ProcessDB dc = new App_Data.ProcessDB(connectionString))
				{
					var entity = dc.Process.Where(e => (e.StatusCode == inprocess)).FirstOrDefault();
					if (entity == null)
					{
						ProcessCode_InProcess = "";
					}
					else
					{
						ProcessCode_InProcess = entity.ProcessCode;
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("Process db connection error: ", exc);
				_logger.ErrorException("Process db connection error: ", exc.InnerException);
				//Localization.Resources.Err_ConnectionDBException %" Connection Error "
				return Localization.Resources.Err_ConnectionDBException + " [ProcessDB] :" + exc + Environment.NewLine + exc.InnerException;
			}

			_logger.Info("Process db connection string: " + connectionString);
			//count4u db check

            connectionString = this.BuildAuditDbConnectionString();
            try
            {
                using (AuditDB auditDb = new AuditDB(connectionString))
                {
                    var n = auditDb.Inventor.Count();
                    _logger.Trace("inventors count in audit db: " + n);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("audit db connection error: ", exc);
                _logger.ErrorException("audit db connection error: ", exc.InnerException);
                //Localization.Resources.Err_ConnectionDBException %" Connection Error "
                return Localization.Resources.Err_ConnectionDBException + " [AuditDB] :" + exc + Environment.NewLine + exc.InnerException;
            }

            _logger.Info("audit db connection string: " + connectionString);


		

            try
            {
                using (AuditDB auditDb = new AuditDB(connectionString))
                {
                    string inProcess = StatusAuditConfigEnum.InProcess.ToString();
                    AuditConfig auditConfig = auditDb.AuditConfig.FirstOrDefault(r => r.StatusAuditConfig == inProcess);
                    if (auditConfig != null)
                    {
                        string dbPath = auditConfig.DBPath;
                        string count4UConnectionString = BuildCount4UConnectionString(FolderInventor + @"\" + dbPath.Trim('\\'));
                        using (Count4UDB count4Udb = new Count4UDB(count4UConnectionString))
                        {
                            var n = count4Udb.Locations.Count();
                            _logger.Trace("locations count in count4u db: " + n);
                        }

                        _logger.Info("count4u db connection string: " + count4UConnectionString);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("count4u db connection error: ", exc);
                _logger.ErrorException("count4u db connection error: ", exc.InnerException);
                //Localization.Resources.Err_ConnectionDBException %" Connection Error "
                return Localization.Resources.Err_ConnectionDBException + " [Count4UDB] :" + exc + Environment.NewLine + exc.InnerException;
            }

            return String.Empty;
        }

        public string ImportFolderPath()
        {
            string result = String.Empty;
#if DEBUG
            string relativeFolder = String.Format("{0}{1}{2}",
                                   Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\",
                                   this.DebugImportPath + @"\",
                                   this.FolderImport);

            if (Directory.Exists(relativeFolder))
            {
                result = new DirectoryInfo(relativeFolder).FullName;
            }
#else
            result = String.Format("{0}{1}",
                                   FileSystem.FileWithImportPath().Trim('\\') + @"\",
                                   this.FolderImport);
#endif

            return result;
        }

        public string ExportErpFolderPath()
        {
            string result = String.Empty;
#if DEBUG
            result = String.Format("{0}{1}",
                                   Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\",
                                   this.FolderErpExport);

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
#else
            result = String.Format("{0}{1}",
                                   FileSystem.FileWithImportPath().Trim('\\') + @"\",
                                   this.FolderErpExport);
#endif

            return result;
        }

        public string FolderLogoPath()
        {
            string result;
#if DEBUG
            result = String.Format("{0}{1}{2}",
                                   Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\",
                                   this.DebugDbPath + @"\App_Data\",
                                   this.FolderLogoFile);
#else
            result = String.Format("{0}{1}",
                                   FileSystem.FileWithImportPath().Trim('\\') + @"\App_Data\",
                                   this.FolderLogoFile);
#endif

            DirectoryInfo di = new DirectoryInfo(result);

            return di.FullName;
        }

        public string ReportTemplatePath()
        {
            string result = String.Empty;
            try
            {
                string language = _settingsRepository.CurrentLanguage;
                if (language == "it")
                    language = "en";
#if DEBUG
                result = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
                                       this.ReportModulePath + @"\" + this.ReportTemplateFolder + @"\" + language;
#else
                result = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.ReportTemplateFolder + @"\" + language;
#endif


                if (Directory.Exists(result) == false)
                {
                    Directory.CreateDirectory(result);
                }

                DirectoryInfo di = new DirectoryInfo(result);
                result = di.FullName;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("ReportTemplatePath", exc);
            }
            return result;
        }

        public string ReportTemplateRootPath()
        {
            string result = String.Empty;
            try
            {
                //string language = _settingsRepository.CurrentLanguage;
                //if (language == "it")
                //    language = "en";
#if DEBUG
                result = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\" +
                                       this.ReportModulePath + @"\" + this.ReportTemplateFolder;// +@"\" + language;
#else
                result = FileSystem.FileWithProgramDataPath().Trim('\\') + @"\" + this.ReportTemplateFolder; //+ @"\" + language;
#endif


                if (Directory.Exists(result) == false)
                {
                    Directory.CreateDirectory(result);
                }

                DirectoryInfo di = new DirectoryInfo(result);
                result = di.FullName;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("ReportTemplateRootPath", exc);
            }
            return result;
        }

        public string ExportToPdaFolderPath()
        {
            string result = String.Empty;

            try
            {

#if DEBUG
                string relativeFolder = String.Format("{0}{1}{2}",
                                       Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\",
                                       this.DebugExportToPDAPath + @"\",
                                       this.ExportToPDAFolder);

                if (Directory.Exists(relativeFolder))
                {
                    result = new DirectoryInfo(relativeFolder).FullName;
                }
#else
            result = String.Format("{0}{1}",
                                   FileSystem.FileWithExportPdaPath().Trim('\\') + @"\",
                                   this.ExportToPDAFolder);
#endif
            }
            catch (Exception exc)
            {
                _logger.ErrorException("ExportToPdaFolderPath", exc);
            }

            return result;
        }

        public string ExecutablePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public string AdapterLinkTxtPath()
        {
            return Path.Combine(FileSystem.ImportModulesFolderPath(), "adapterlink.txt");
        }

        private string OriginalConfigFolderPath()
        {
            string result = String.Empty;

#if DEBUG

            result = Path.Combine(DebugDataPath, FileSystem.ConfigFolderName);

#else
            result = Path.Combine(FileSystem.FileWithAppPath(), FileSystem.ConfigFolderName);
#endif
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            FileInfo fi = new FileInfo(result);

            return fi.FullName;
        }

        private string TargetConfigFolderPath()
        {
            string result = String.Empty;

#if DEBUG

            string baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            result = Path.Combine(baseFolder, FileSystem.ConfigFolderName);

#else
            result = Path.Combine(FileSystem.FileWithProgramDataPath(), FileSystem.ConfigFolderName);
#endif
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            FileInfo fi = new FileInfo(result);

            return fi.FullName;
        }

        public string UIConfigSetFolderPath()
        {
            string result = String.Empty;

            string fromFolder = String.Empty;
            string toFolder = String.Empty;

#if DEBUG
            fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.UIConfigSetFolderName);
            toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.UIConfigSetFolderName);
#else
            fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.UIConfigSetFolderName);
            toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.UIConfigSetFolderName);
#endif

            if (!Directory.Exists(fromFolder))
            {
                Directory.CreateDirectory(fromFolder);
            }

            if (!Directory.Exists(toFolder))
            {
                Directory.CreateDirectory(toFolder);
            }

            DirectoryInfo di = new DirectoryInfo(fromFolder);

            foreach (FileInfo fi in di.GetFiles("*.config"))
            {
                string sourceFile = fi.FullName;
                string targetFile = Path.Combine(toFolder, fi.Name);

                if (!File.Exists(targetFile))
                {
                    File.Copy(sourceFile, targetFile);
                }
            }

            result = toFolder;

            return result;
        }

        public string UIPropertySetFolderPath()
        {
            string result = String.Empty;

            string fromFolder = String.Empty;
            string toFolder = String.Empty;

#if DEBUG
            fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.UIPropertySetFolderName);
            toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.UIPropertySetFolderName);
#else
            fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.UIPropertySetFolderName);
            toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.UIPropertySetFolderName);
#endif

            if (!Directory.Exists(fromFolder))
            {
                Directory.CreateDirectory(fromFolder);
            }

            if (!Directory.Exists(toFolder))
            {
                Directory.CreateDirectory(toFolder);
            }

            DirectoryInfo di = new DirectoryInfo(fromFolder);

            foreach (FileInfo fi in di.GetFiles("*.ini"))
            {
				if (fi.Name.StartsWith("~")) continue;
                string sourceFile = fi.FullName;
                string targetFile = Path.Combine(toFolder, fi.Name);

#if DEBUG
                if (File.Exists(targetFile))
                    File.Delete(targetFile);
#else
                if (File.Exists(targetFile))
                    File.Delete(targetFile);
#endif

                File.Copy(sourceFile, targetFile);
            }

            result = toFolder;

            return result;
        }


		private void CopyFilesWithSubfolders(string fromFolder, string toFolder)
		{
			//Берём нашу исходную папку
			DirectoryInfo fromFolderInfo = new DirectoryInfo(fromFolder);
			//Перебираем все внутренние папки
			foreach (DirectoryInfo dir in fromFolderInfo.GetDirectories())
			{
				//Проверяем - если директории не существует, то создаём;
				if (Directory.Exists(toFolder + "\\" + dir.Name) != true)
				{
					try
					{
						Directory.CreateDirectory(toFolder + "\\" + dir.Name);
					}
					catch { }
				}

				//Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
				CopyFilesWithSubfolders(dir.FullName, toFolder + "\\" + dir.Name);
			}

			//Перебираем файлики в папке источнике.
			DirectoryInfo di = new DirectoryInfo(fromFolder);
			foreach (FileInfo fi in di.GetFiles("*.xml"))
			{
				if (fi.Name.StartsWith("~")) continue;
				string sourceFile = fi.FullName;
				string targetFile = Path.Combine(toFolder, fi.Name);
				try
				{
					File.Copy(sourceFile, targetFile, false);
				}
				catch { }
			}

			//foreach (string file in Directory.GetFiles(fromFolder))
			//{
				//Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
				//string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
				//Копируем файлик с перезаписью из источника в приёмник.
				//File.Copy(file, toFolder + "\\" + filik, true);
			//}
		}

		// Not use now
		//public string GetOriginalAdapterDefaultParamFolderPath()
		//{
		//	string fromFolder = String.Empty;
		//	fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.AdapterDefaultConfigFolderName);
		//	return fromFolder;
		//}

		// Not use now
//		public string AdapterDefaultConfigFolderPath()
//		{
//			string result = String.Empty;

//			string fromFolder = String.Empty;
//			string toFolder = String.Empty;

//#if DEBUG
//			fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.AdapterDefaultConfigFolderName);
//			toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.AdapterDefaultConfigFolderName);
//#else
//			fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.AdapterDefaultConfigFolderName);
//			toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.AdapterDefaultConfigFolderName);
//#endif

//			CopyFilesWithSubfolders(fromFolder, toFolder);


//			result = toFolder;

//			return result;
//		}

        private string _cacheUIFilterTemplateSetFolderPath;

        public string UIFilterTemplateSetFolderPath()
        {
            if (String.IsNullOrEmpty(_cacheUIFilterTemplateSetFolderPath))
            {

                string fromFolder = String.Empty;
                string toFolder = String.Empty;

#if DEBUG
                fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.UIFilterTemplateFolderName);
                toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.UIFilterTemplateFolderName);
#else
            fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.UIFilterTemplateFolderName);
            toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.UIFilterTemplateFolderName);
#endif

                if (!Directory.Exists(fromFolder))
                {
                    Directory.CreateDirectory(fromFolder);
                }

                if (!Directory.Exists(toFolder))
                {
                    Directory.CreateDirectory(toFolder);
                }

                foreach (string directory in Directory.EnumerateDirectories(fromFolder))
                {
                    //sub dir
                    DirectoryInfo di = new DirectoryInfo(directory);

                    string targetDirectory = Path.Combine(toFolder, di.Name);
                    if (Directory.Exists(targetDirectory) == false)
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    foreach (FileInfo sourceFi in di.GetFiles("*.xml"))
                    {
                        string sourceFile = sourceFi.FullName;
                        string targetFile = Path.Combine(targetDirectory, sourceFi.Name);

#if DEBUG
                        if (File.Exists(targetFile))
                            File.Delete(targetFile);
#else
                if (File.Exists(targetFile))
                    File.Delete(targetFile);
#endif

                        File.Copy(sourceFile, targetFile);
                    }
                }

                _cacheUIFilterTemplateSetFolderPath = toFolder;
            }

            return _cacheUIFilterTemplateSetFolderPath;
        }

        private string planPictureFolderPath;
        public string PlanogramPictureFolderPath()
        {
            if (String.IsNullOrEmpty(planPictureFolderPath))
            {
                string fromFolder = String.Empty;
                string toFolder = String.Empty;

#if DEBUG
                fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.PlanogramPictureFolderName);
                toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.PlanogramPictureFolderName);
#else
            fromFolder = Path.Combine(OriginalConfigFolderPath(), FileSystem.PlanogramPictureFolderName);
            toFolder = Path.Combine(TargetConfigFolderPath(), FileSystem.PlanogramPictureFolderName);
#endif

                if (!Directory.Exists(fromFolder))
                {
                    Directory.CreateDirectory(fromFolder);
                }

                if (!Directory.Exists(toFolder))
                {
                    Directory.CreateDirectory(toFolder);
                }

                DirectoryInfo di = new DirectoryInfo(fromFolder);

                foreach (FileInfo fi in di.GetFiles())
                {
                    string sourceFile = fi.FullName;
                    string targetFile = Path.Combine(toFolder, fi.Name);

                    if (!File.Exists(targetFile))
                    {
                        File.Copy(sourceFile, targetFile);
                    }
                }

                planPictureFolderPath = toFolder;
            }
            return planPictureFolderPath;
        }

        public string TerminalIDPath()
        {
            string exportAdaptersPath = FileSystem.ExportModulesFolderPath();

            string terminalPath = System.IO.Path.Combine(exportAdaptersPath, "TerminalID");

            return terminalPath;
        }

        //=======
        public string HomeBackgroundFilePath
        {
            get { return PropertiesSettings.HomeBackgroundFilePath; }
        }

        public string CustomerBackgroundFilePath
        {
            get { return PropertiesSettings.CustomerBackgroundFilePath; }
        }

        public string BranchBackgroundFilePath
        {
            get { return PropertiesSettings.BranchBackgroundFilePath; }
        }

        public string InventorBackgroundFilePath
        {
            get { return PropertiesSettings.InventorBackgroundFilePath; }
        }

        public string MainBackgroundFilePath
        {
            get { return PropertiesSettings.MainBackgroundFilePath; }
        }

        //=

		public string MISiDnextDataPath
		{
			get 
			{
				if (Directory.Exists(PropertiesSettings.MISiDnextDataPath) == false)
				{
					try { Directory.CreateDirectory(PropertiesSettings.MISiDnextDataPath); }
					catch { }
				}
				if (Directory.Exists(PropertiesSettings.MISiDnextDataPath) == true)
				{
					return PropertiesSettings.MISiDnextDataPath;
				}
				else return "";
			}
		}

		public string MISCommunicatorPath
		{
			get { 
				if (Directory.Exists(PropertiesSettings.MISCommunicatorPath) == false)
				{
					try { Directory.CreateDirectory(PropertiesSettings.MISCommunicatorPath); }
					catch { }
				}
				if (Directory.Exists(PropertiesSettings.MISCommunicatorPath) == true)
				{
					return PropertiesSettings.MISCommunicatorPath;
				}
				else return "";
			}
		}
		//======
        public double HomeOpacityBackground
        {
            get { return PropertiesSettings.HomeOpacityBackground; }
        }

        public double CustomerOpacityBackground
        {
            get { return PropertiesSettings.CustomerOpacityBackground; }
        }

        public double BranchOpacityBackground
        {
            get { return PropertiesSettings.BranchOpacityBackground; }
        }

        public double InventorOpacityBackground
        {
            get { return PropertiesSettings.InventorOpacityBackground; }
        }

        public double MainOpacityBackground
        {
            get { return PropertiesSettings.MainOpacityBackground; }
        }

	
    }
}