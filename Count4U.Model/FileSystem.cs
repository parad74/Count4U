using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Count4U.Localization;
using NLog;

namespace Count4U.Model
{
    public class FileSystem
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public const string ImportModulesFolderName = @"ImportModules";
        public const string ExportModulesFolderName = @"ExportModules";

        private const string AppDataLocalFolder = @"Count4U";

        private const string DbPathFile = @"dbPath.txt";
        private const string NameOfDbFolder = @"App_Data";

        public const string inData = @"inData";

        public const string ConfigFolderName = @"Config";
		public const string ProfileFolderName = @"Profile";
        public const string UIConfigSetFolderName = @"UIConfigSet";
        public const string UIPropertySetFolderName = @"UIPropertySet";
        public const string UIFilterTemplateFolderName = @"UIFilterTemplateSet";
		public const string PlanogramPictureFolderName = @"PlanogramPicture";
		public const string AdapterDefaultConfigFolderName = @"AdapterDefaultConfig";
		
        private const string UserSettingsFileName = @"userSettings.config";
        public const string ConfigSetFileExtension = @".config";

        //cache
        private static string _dbPathFileContent = String.Empty;
        private static string _importPathFileContent = String.Empty;
        private static string _appPathFileContent = String.Empty;
        private static string _exportPdaFileContent = String.Empty;
        private static string _appRedactionContent = String.Empty;
        private static List<string> _setupSettingsFile;

        //App_Data
        public static string GetNameOfDbFolder()
        {
            return NameOfDbFolder;
        }

		private static string DebugDbPath
		{
			get
			{
				string debugDbPath = PropertiesSettings.DebugDbPath.Trim('\\');
				// debugDbPath = @"\" + debugDbPath + @"\";
				return debugDbPath;
			}
		}

        //c:\Users\[UserName]\AppData\Local\Count4U\
        public static string UserCount4UFolder()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDataCount4U = Path.Combine(appData, AppDataLocalFolder);

            return appDataCount4U;
        }

        //c:\ProgramData\Count4U\
        public static string CommonCount4UFolder()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string appDataCount4U = Path.Combine(appData, AppDataLocalFolder);

            return appDataCount4U;
        }

        //c:\Users\[UserName]\AppData\Local\Count4U\userSettings.config
        public static string UserSettingsFileOld()
        {
            string folder = UserCount4UFolder();

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, UserSettingsFileName);
            return file;
        }


		public static string UserSettingsFile(string processCode = "")
		{
			string folder;
			string subfolder = processCode.Trim().Trim('\\');
			if (string.IsNullOrWhiteSpace(subfolder) == false) subfolder = @"\" + subfolder;
			else subfolder = "";

#if DEBUG
            folder = String.Format("{0}{1}",
                                   Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Trim('\\') + @"\",
								   DebugDbPath + @"\App_Data\Process" + subfolder);
#else
			folder = String.Format("{0}",
								   FileSystem.FileWithImportPath().Trim('\\') + @"\App_Data\Process" + subfolder);
#endif

			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			string file = Path.Combine(folder, UserSettingsFileName);
			return file;
		}

	
        //reads configuration file which is created by setup program
        private static List<string> ConfigFileContent()
        {
            if (_setupSettingsFile == null)
            {
                try
                {
                    string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    string dbPathFile = Path.Combine(appData, AppDataLocalFolder, DbPathFile);
                    if (File.Exists(dbPathFile))
                    {
                        _setupSettingsFile = File.ReadAllLines(dbPathFile).ToList();
                    }
                    else
                    {
                        _logger.Info(String.Format("FileSystem.ConfigFileContent() - {0} missing", DbPathFile));
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorException("ConfigFileContent", e);
                }
            }
            return _setupSettingsFile;
        }

        public static string FileWithProgramDataPath()
        {
            if (String.IsNullOrEmpty(_dbPathFileContent) == true)
            {
                List<string> lines = ConfigFileContent();
                if (lines != null && lines.Count > 0)
                {
                    _dbPathFileContent = lines[0];
                }
            }

            return _dbPathFileContent;
        }

        public static string FileWithImportPath()
        {
            if (String.IsNullOrEmpty(_importPathFileContent) == true)
            {
                List<string> lines = ConfigFileContent();
                if (lines != null && lines.Count > 1)
                {
                    _importPathFileContent = lines[1];
                }
            }

            return _importPathFileContent;
        }

        public static string FileWithAppPath()
        {
            if (String.IsNullOrEmpty(_appPathFileContent) == true)
            {
                List<string> lines = ConfigFileContent();
                if (lines != null && lines.Count > 2)
                {
                    _appPathFileContent = lines[2];
                }
            }

            return _appPathFileContent;
        }

        public static string FileWithExportPdaPath()
        {
            if (String.IsNullOrEmpty(_exportPdaFileContent))
            {
                List<string> lines = ConfigFileContent();
                if (lines != null && lines.Count > 3)
                {
                    _exportPdaFileContent = lines[3];
                }
            }

            return _exportPdaFileContent;
        }

        public static bool IsAppRedactionOffice()
        {
#if DEBUG
            if (ConfigFileContent() != null)
            {
                List<string> lines = ConfigFileContent();
                if (lines != null && lines.Count > 4)
                {
                    _appRedactionContent = lines[4].Trim();
                }

				//return true; //WIXUI_OFFICE
				//return false; //WIXUI_LAPTOP
				return _appRedactionContent == "WIXUI_OFFICE";
				
            }
            return true;
#else
#endif
            if (String.IsNullOrEmpty(_appRedactionContent))
            {
                List<string> lines = ConfigFileContent();
                if (lines != null && lines.Count > 4)
                {
                    _appRedactionContent = lines[4].Trim();
                }
            }

			return _appRedactionContent == "WIXUI_OFFICE";
			//return _appRedactionContent == "LAPTOP";		
        }

        public static string ImportModulesFolderPath()
        {
#if DEBUG
            string baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(baseFolder, ImportModulesFolderName);
#else
            string baseFolder = FileWithProgramDataPath();
            return Path.Combine(baseFolder, ImportModulesFolderName);
#endif

        }

        public static string ExportModulesFolderPath()
        {
#if DEBUG
            string baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(baseFolder, ExportModulesFolderName);
#else
            string baseFolder = FileWithProgramDataPath();
            return Path.Combine(baseFolder, ExportModulesFolderName);
#endif

        }



    }
}