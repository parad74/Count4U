using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using NLog;

namespace Count4U.GenerationReport
{
	public class ReportIniRepository : IReportIniRepository
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IDBSettings _dbSettings;
		//private readonly IIniFileParser _iniFileParser;

		private readonly IServiceLocator _serviceLocator;
		public ReportIniRepository(
			 IDBSettings dbSettings,
			 IServiceLocator serviceLocator)
		{
			this._dbSettings = dbSettings;
			this._serviceLocator = serviceLocator;
		}

		public string CopyReportTemplateIniFile(string code, string objectType = "Inventor")
		{
			try
			{
				if (String.IsNullOrWhiteSpace(code)) return string.Empty;

				string reportIniFile = string.Empty;
				// у Inventor есть ini файл 
				reportIniFile = Common.Helpers.UtilsPath.ExportReportIniFile(this._dbSettings, code, objectType);	  //путь до  ini заданного для Inventor
				if (File.Exists(reportIniFile) == true) return reportIniFile;

				// у Inventor нет ini файл - надо скопировать
				string sourceFileIniPath = UtilsPath.ExportReportTemplateIniFile(this._dbSettings);	//путь до  ini по умолчанию
				if (File.Exists(sourceFileIniPath) == false) return string.Empty;

				File.Copy(sourceFileIniPath, reportIniFile);
				return reportIniFile;
			}
			catch (Exception exc)
			{
				_logger.Info("CopyReportTemplateIniFileFromInventor", exc);
				return string.Empty;
			}
		}

		public string CopyReportTemplateIniFileToCustomer(string configPath)
		{
			try
			{
				string reportIniFile = string.Empty;
				// у Customer есть ini файл 
				reportIniFile = Path.Combine(configPath, UtilsPath.ExportReportIniFileName);
				//reportIniFile = Common.Helpers.UtilsPath.ExportReportIniFile(this._dbSettings, code, objectType);	  //путь до  ini заданного для Inventor
				if (File.Exists(reportIniFile) == true) return reportIniFile;

				// у Inventor нет ini файл - надо скопировать
				string sourceFileIniPath = UtilsPath.ExportReportTemplateIniFile(this._dbSettings);	//путь до  ini по умолчанию
				if (File.Exists(sourceFileIniPath) == false) return string.Empty;

				File.Copy(sourceFileIniPath, reportIniFile);
				return reportIniFile;
			}
			catch (Exception exc)
			{
				_logger.Info("CopyReportTemplateIniFileFromInventor", exc);
				return string.Empty;
			}
		}


	
		public string CopyContextMenuReportTemplateIniFile(string inventorCode)
		{
			try
			{
				if (String.IsNullOrWhiteSpace(inventorCode)) return string.Empty;

				string reportIniFile = string.Empty;
				// у Inventor есть ini файл 
				reportIniFile = Common.Helpers.UtilsPath.ContextMenuReportIniFile(this._dbSettings, inventorCode);	  //путь до  ini заданного для Inventor
				if (File.Exists(reportIniFile) == true) return reportIniFile;

				// у Inventor нет ini файл - надо скопировать
				string sourceFileIniPath = UtilsPath.ContextMenuReportTemplateIniFile(this._dbSettings);	//путь до  ini по умолчанию
				if (File.Exists(sourceFileIniPath) == false) return string.Empty;

				File.Copy(sourceFileIniPath, reportIniFile);
				return reportIniFile;
			}
			catch (Exception exc)
			{
				_logger.Info("CopyContextMenuReportTemplateIniFile", exc);
				return string.Empty;
			}
		}

		public string CopyPrintReportTemplateIniFile(string inventorCode)
		{
			try
			{
				if (String.IsNullOrWhiteSpace(inventorCode)) return string.Empty;

				string reportIniFile = string.Empty;
				// у Inventor есть ini файл 
				reportIniFile = Common.Helpers.UtilsPath.PrintReportIniFile(this._dbSettings, inventorCode);	  //путь до  ini заданного для Inventor
				if (File.Exists(reportIniFile) == true) return reportIniFile;

				// у Inventor нет ini файл - надо скопировать
				string sourceFileIniPath = UtilsPath.PrintReportTemplateIniFile(this._dbSettings);	//путь до  ini по умолчанию
				if (File.Exists(sourceFileIniPath) == false) return string.Empty;

				File.Copy(sourceFileIniPath, reportIniFile);
				return reportIniFile;
			}
			catch (Exception exc)
			{
				_logger.Info("CopyPrintReportTemplateIniFile", exc);
				return string.Empty;
			}
		}
		
	}

	public enum ReportIniEnum
    {
		ContextPrintReportForLocation,
		ContextPrintReportForIturs,
		ContextSendToOffice,
		ContextMenuIturList,
		FileType,
		IncludeInZip,
		Print,
		SecondPrint,
		SelectReportBy,
		ShowInContextMenu,
		ReportNumInContextMenu
	}

	public class ReportIniProperty
	{
		public const string ContextPrintReportForLocation = "ContextPrintReportForLocation";
		public const string ContextPrintReportForIturs = "ContextPrintReportForIturs";
		public const string ContextPrintReportByTagForLocation = "ContextPrintReportByTagForLocation";
		public const string ContextPrintReportByTagForItur = "ContextPrintReportByTagForItur";
		public const string ContextPrintReportByTagForSection = "ContextPrintReportByTagForSection";
		
		public const string ContextSendToOffice = "ContextSendToOffice";
		public const string ForCustomer = "ForCustomer";
		public const string RefillAlways = "RefillAlways";
		public const string ContextMenuIturList = "ContextMenuIturList";
		public const string FileType = "FileType";
		public const string IncludeInZip = "IncludeInZip";
		public const string Print = "Print";
		public const string SecondPrint = "SecondPrint";
		public const string SelectReportBy = "SelectReportBy";
		public const string ShowInContextMenu = "ShowInContextMenu";
		public const string ReportNumInContextMenu = "ReportNumInContextMenu";
	}


	public enum ReportIniContextMenuIndex
	{
		Report0 = 0,
		Report1 = 1,
		Report2 = 2,
		Report3 = 3,
		Report4 = 4,
	}

	public class ReportIniContextMenuStringIndex
	{
		public const string Report0 = "Report0";
		public const string Report1 = "Report1";
		public const string Report2 = "Report2";
		public const string Report3 = "Report3";
		public const string Report4 = "Report4";
		public const string Report5 = "Report5";
	}

	public class SelectReportByType
	{
		public const string IturCodes = "IturCodes";
		public const string LocationCode = "LocationCode";
		public const string LocationTag = "LocationTag";
		public const string IturTag = "IturTag";
		public const string SectionTag = "SectionTag";
		

	}
}