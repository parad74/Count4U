using System;
using System.IO;
using Count4U.Model;
using Count4U.Model.Interface;
using NLog;

namespace Count4U.Common.Helpers
{
    public static class UtilsPath
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public const string ExportReportIniFileName = @"ExportReport.ini";
		public const string PrintReportIniFileName = @"PrintReport.ini";
		public const string ContextMenuReportIniFileName = @"ContextMenuReport.ini";

        public static string ExportErpFolder(IDBSettings dbSettings, string objectType, string objectCode)
        {
            try
            {
                string folder = dbSettings.ExportErpFolderPath();

                if (!Directory.Exists(folder) || String.IsNullOrWhiteSpace(objectCode)) return String.Empty;

				string folderPlusInventor = Path.Combine(folder, objectType);
                if (!Directory.Exists(folderPlusInventor))
                    Directory.CreateDirectory(folderPlusInventor);

                string folderPlusInventorPlusCode = Path.Combine(folderPlusInventor, objectCode);
                if (!Directory.Exists(folderPlusInventorPlusCode))
                    Directory.CreateDirectory(folderPlusInventorPlusCode);

                return folderPlusInventorPlusCode;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("ExportErpFolder", exc);
            }

            return String.Empty;
        }

        public static string ZipOfficeFolder(IDBSettings dbSettings)
        {
            string exportErpFolderPath = dbSettings.ExportErpFolderPath();
            if (!Directory.Exists(exportErpFolderPath))
            {
                _logger.Error("Export ERP folder missing");
                return String.Empty;
            }

            string sendDataFolderPath = Path.Combine(exportErpFolderPath, "FilesToOffice");
            if (!Directory.Exists(sendDataFolderPath))
            {
                Directory.CreateDirectory(sendDataFolderPath);
            }

            return sendDataFolderPath;
        }

		public static string ExportReportIniFolder(IDBSettings dbSettings, string code, string objectType = "Inventor")
        {
            string folder = dbSettings.ExportErpFolderPath();

            if (!Directory.Exists(folder) || String.IsNullOrEmpty(code))
                return String.Empty;

            string exportIniFolder = Path.Combine(folder, "ExportIni");

            if (!Directory.Exists(exportIniFolder))
            {
                Directory.CreateDirectory(exportIniFolder);
            }

			string exportIniInventorFolder = Path.Combine(exportIniFolder, objectType);

            if (!Directory.Exists(exportIniInventorFolder))
            {
                Directory.CreateDirectory(exportIniInventorFolder);
            }

            string resultPath = Path.Combine(exportIniInventorFolder, code);

            if (!Directory.Exists(resultPath))
            {
                Directory.CreateDirectory(resultPath);
            }

            string resultFile = Path.Combine(resultPath, ExportReportIniFileName);
            if (!File.Exists(resultFile))
            {
                string template = UtilsPath.ExportReportTemplateIniFile(dbSettings);
                if (!File.Exists(template))
                {
					_logger.Error("ExportReport.ini template missing!");
                    return null;
                }
                File.Copy(template, resultFile);
            }

            return resultPath;
        }

		public static string PrintReportIniFolder(IDBSettings dbSettings, string inventorCode)
		{
			string folder = dbSettings.ExportErpFolderPath();

			if (!Directory.Exists(folder) || String.IsNullOrEmpty(inventorCode))
				return String.Empty;

			string exportIniFolder = Path.Combine(folder, "ExportIni");

			if (!Directory.Exists(exportIniFolder))
			{
				Directory.CreateDirectory(exportIniFolder);
			}

			string exportIniInventorFolder = Path.Combine(exportIniFolder, "Inventor");

			if (!Directory.Exists(exportIniInventorFolder))
			{
				Directory.CreateDirectory(exportIniInventorFolder);
			}

			string resultPath = Path.Combine(exportIniInventorFolder, inventorCode);

			if (!Directory.Exists(resultPath))
			{
				Directory.CreateDirectory(resultPath);
			}

			string resultFile = Path.Combine(resultPath, PrintReportIniFileName);
			if (!File.Exists(resultFile))
			{
				string template = UtilsPath.PrintReportTemplateIniFile(dbSettings);
				if (!File.Exists(template))
				{
					_logger.Error("PrintReport.ini template missing!");
					return null;
				}
				File.Copy(template, resultFile);
			}

			return resultPath;
		}

		public static string ContextMenuReportIniFolder(IDBSettings dbSettings, string inventorCode)
		{
			string folder = dbSettings.ExportErpFolderPath();

			if (!Directory.Exists(folder) || String.IsNullOrEmpty(inventorCode))
				return String.Empty;

			string exportIniFolder = Path.Combine(folder, "ExportIni");

			if (!Directory.Exists(exportIniFolder))
			{
				Directory.CreateDirectory(exportIniFolder);
			}

			string exportIniInventorFolder = Path.Combine(exportIniFolder, "Inventor");

			if (!Directory.Exists(exportIniInventorFolder))
			{
				Directory.CreateDirectory(exportIniInventorFolder);
			}

			string resultPath = Path.Combine(exportIniInventorFolder, inventorCode);

			if (!Directory.Exists(resultPath))
			{
				Directory.CreateDirectory(resultPath);
			}

			string resultFile = Path.Combine(resultPath, ContextMenuReportIniFileName);
			if (!File.Exists(resultFile))
			{
				string template = UtilsPath.ContextMenuReportTemplateIniFile(dbSettings);
				if (!File.Exists(template))
				{
					_logger.Error("ContextMenuReport.ini template missing!");
					return null;
				}
				File.Copy(template, resultFile);
			}

			return resultPath;
		}

        public static string ExportReportIniFile(IDBSettings dbSettings, string code, string objectType = "Inventor")
        {
			return Path.Combine(UtilsPath.ExportReportIniFolder(dbSettings, code, objectType), UtilsPath.ExportReportIniFileName);
        }

		public static string ContextMenuReportIniFile(IDBSettings dbSettings, string inventorCode)
		{
			return Path.Combine(UtilsPath.ContextMenuReportIniFolder(dbSettings, inventorCode), UtilsPath.ContextMenuReportIniFileName);
		}

		public static string PrintReportIniFile(IDBSettings dbSettings, string inventorCode)
		{
			return Path.Combine(UtilsPath.PrintReportIniFolder(dbSettings, inventorCode), UtilsPath.PrintReportIniFileName);
		}

        public static string ExportReportTemplateIniFile(IDBSettings dbSettings)
        {
#if DEBUG
            return Path.Combine(dbSettings.ExecutablePath(), ExportReportIniFileName);
#else
            return Path.Combine(FileSystem.FileWithAppPath(), ExportReportIniFileName);

#endif
		}

		public static string PrintReportTemplateIniFile(IDBSettings dbSettings)
		{
#if DEBUG
			return Path.Combine(dbSettings.ExecutablePath(), PrintReportIniFileName);
#else
            return Path.Combine(FileSystem.FileWithAppPath(), PrintReportIniFileName);

#endif
		}

		public static string ContextMenuReportTemplateIniFile(IDBSettings dbSettings)
		{
#if DEBUG
			return Path.Combine(dbSettings.ExecutablePath(), ContextMenuReportIniFileName);
#else
            return Path.Combine(FileSystem.FileWithAppPath(), ContextMenuReportIniFileName);

#endif
		}


    }
}