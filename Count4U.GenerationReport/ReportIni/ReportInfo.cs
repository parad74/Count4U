using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Transfer;
using Microsoft.ReportingServices.Interfaces;
using NLog;
using Count4U.Model.SelectionParams;
using System.Threading.Tasks;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.GenerationReport
{
	public class ReportInfo
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public string ReportCode { get; set; }
		public string Format { get; set; }
		public bool IncludeInZip { get; set; }
		public bool Print { get; set; }
		public bool Print2 { get; set; }
		public string SelectReportBy { get; set; }
		public bool ShowInContextMenu { get; set; }
		public string ReportNumInContextMenu { get; set; }
		public object param1 { get; set; }
		public object param2 { get; set; }
		public object param3 { get; set; }
		public bool RefillAlways { get; set; }
		
		public GenerateReportArgs GenerateArgs { get; set; }
		public IReportRepository _reportRepository;

		public ReportInfo(IReportRepository reportRepository)
		{
			this.GenerateArgs = null;
			this.param1 = null;
			this.param2 = null;
			this.param3 = null;
			this.IncludeInZip = false;
			this.Print = false;
			this.Print2 = false;
			this.SelectReportBy="";
			this.ShowInContextMenu = false;
			this.ReportNumInContextMenu = "";
			this._reportRepository = reportRepository;
			this.RefillAlways = false;
			
		}

	
	}

	public static class ReportInfoAction
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public static GenerateReportArgs BuildReportArgs(this ReportInfo reportInfo, CBIState cbiState, ViewDomainContextEnum viewDomainContext = ViewDomainContextEnum.Iturs)//, IReportRepository reportRepository)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(reportInfo.ReportCode) == true) return null;

				reportInfo.GenerateArgs = null; 

				GenerateReportArgs generateArgs = new GenerateReportArgs();
				generateArgs.Customer = cbiState.CurrentCustomer;
				generateArgs.Branch = cbiState.CurrentBranch;
				generateArgs.Inventor = cbiState.CurrentInventor;
				generateArgs.DbPath = cbiState.GetDbPath;
				generateArgs.SelectParams = new SelectParams();
				generateArgs.ViewDomainContextType = viewDomainContext;//ViewDomainContextEnum.Iturs;

				Count4U.GenerationReport.Report report = null;
				
				Reports reports = reportInfo._reportRepository.GetReportByCodeReport(reportInfo.ReportCode);
				if (reports != null)
				{
					report = reports.FirstOrDefault();
				}

				if (report == null)
				{
					_logger.Warn("Report {0} missing", reportInfo.ReportCode);
					return null;
				}

				generateArgs.Report = report;
				reportInfo.GenerateArgs = generateArgs;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("BuildReportArgs :" + reportInfo.ReportCode, exc);
			}

			return reportInfo.GenerateArgs;
		}


		public static GenerateReportArgs BuildReportArgs(this ReportInfo reportInfo, string dbPath, ViewDomainContextEnum viewDomainContext = ViewDomainContextEnum.Device)//, IReportRepository reportRepository)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(reportInfo.ReportCode) == true) return null;

				reportInfo.GenerateArgs = null;

				GenerateReportArgs generateArgs = new GenerateReportArgs();
				generateArgs.DbPath = dbPath;
				generateArgs.SelectParams = new SelectParams();
				generateArgs.ViewDomainContextType = viewDomainContext;//ViewDomainContextEnum.Iturs;

				Count4U.GenerationReport.Report report = null;

				Reports reports = reportInfo._reportRepository.GetReportByCodeReport(reportInfo.ReportCode);
				if (reports != null)
				{
					report = reports.FirstOrDefault();
				}

				if (report == null)
				{
					_logger.Warn("Report {0} missing", reportInfo.ReportCode);
					return null;
				}

				generateArgs.Report = report;
				reportInfo.GenerateArgs = generateArgs;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("BuildReportArgs :" + reportInfo.ReportCode, exc);
			}

			return reportInfo.GenerateArgs;
		}
	}
}