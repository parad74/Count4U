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
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using NLog;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;

namespace Count4U.GenerationReport
{
	public class ReportInfoRepository : IReportInfoRepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IReportRepository _reportRepository;
		private readonly IReportIniRepository _reportIniRepository;
		private readonly IIniFileParser _iniFileParser;
		private readonly IServiceLocator _serviceLocator;
		private readonly IDBSettings _dbSettings;

		public ReportInfoRepository(
			IReportRepository reportRepository,
			IReportIniRepository reportIniRepository,
			IIniFileParser iniFileParser,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator)
		{
			this._dbSettings = dbSettings;
			this._serviceLocator = serviceLocator;
			this._reportRepository = reportRepository;
			this._reportIniRepository = reportIniRepository;
			this._iniFileParser = iniFileParser;
		}

		public List<ReportInfo> BuildContextMenuReportInfoList(string inventorCode, string reportIniFile, string reportN)
		{
			reportN = reportN.ToLower();
			List<ReportInfo> reportInfoList = this.BuildContextMenuReportInfoList(inventorCode, reportIniFile);
			List<ReportInfo> repInfoList = reportInfoList.Where(x => x != null && x.ReportNumInContextMenu.ToLower() == reportN).ToList();
			if (repInfoList == null) return new List<ReportInfo>();
			return repInfoList;
		}


		public List<ReportInfo> BuildContextMenuReportInfoList(string inventorCode, string reportIniFile)
		{
			//this._reports.Clear();
			List<ReportInfo> reportInfoList = new List<ReportInfo>();

			if (File.Exists(reportIniFile) == false)
			{
				reportIniFile = this._reportIniRepository.CopyContextMenuReportTemplateIniFile(inventorCode);
			}
			if (File.Exists(reportIniFile) == false) return reportInfoList;

			const string Context = ReportIniProperty.ContextMenuIturList;
			const string PrintKey = ReportIniProperty.Print;
			const string SecondPrintKey = ReportIniProperty.SecondPrint;
			const string SelectReportByKey = ReportIniProperty.SelectReportBy;
			const string ShowInContextMenu = ReportIniProperty.ShowInContextMenu;
			const string ReportNumInContextMenu = ReportIniProperty.ReportNumInContextMenu;

			foreach (IniFileData iniFileData in this._iniFileParser.Get(reportIniFile))
			{
				string reportCode = iniFileData.SectionName;

				if (String.IsNullOrWhiteSpace(reportCode) == true) continue;

				bool isContextMenuIturList = false;
				if (iniFileData.Data.ContainsKey(Context) == true)
				{
					isContextMenuIturList = iniFileData.Data[Context] == "1";
				}
				if (isContextMenuIturList == false) continue;

				string reportNumInContextMenu = "";
				if (iniFileData.Data.ContainsKey(ReportNumInContextMenu) == true)
				{
					reportNumInContextMenu = iniFileData.Data[ReportNumInContextMenu];
				}
				if (reportNumInContextMenu == "") continue;

				string selectReportBy = "";
				if (iniFileData.Data.ContainsKey(SelectReportByKey) == true)
				{
					selectReportBy = iniFileData.Data[SelectReportByKey];
				}

				bool isPrint = false;
				if (iniFileData.Data.ContainsKey(PrintKey))
				{
					isPrint = iniFileData.Data[PrintKey] == "1";
				}

				bool isSecondPrint = false;
				if (iniFileData.Data.ContainsKey(SecondPrintKey) == true)
				{
					isSecondPrint = iniFileData.Data[SecondPrintKey] == "1";
				}

				bool isShowInContextMenu = false;
				if (iniFileData.Data.ContainsKey(ShowInContextMenu) == true)
				{
					isShowInContextMenu = iniFileData.Data[ShowInContextMenu] == "1";
				}


				string reportCodeBracket = String.Format("[{0}]", reportCode);
				Count4U.GenerationReport.Reports reports = this._reportRepository.GetReportByCodeReport(reportCodeBracket);
				Count4U.GenerationReport.Report report = null;
				if (reports != null)
				{
					report = reports.FirstOrDefault();
				}

				if (report == null)
				{
					_logger.Warn("BuildReports: Report is missing{0}", reportCode);
					continue;
				}

				ReportInfo item = new ReportInfo(this._reportRepository);
				item.ReportCode = reportCodeBracket;
				item.Print = isPrint;
				item.Print2 = isSecondPrint;
				item.param2 = selectReportBy;
				item.SelectReportBy = selectReportBy;
				item.ShowInContextMenu = isShowInContextMenu;
				item.ReportNumInContextMenu = reportNumInContextMenu;
				reportInfoList.Add(item);
			}
			return reportInfoList;
		}


	}


}