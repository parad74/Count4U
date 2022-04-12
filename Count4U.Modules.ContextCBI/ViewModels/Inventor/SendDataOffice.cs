using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.GenerationReport.Interface;
using Count4U.GenerationReport.Repository.Generate;
using Count4U.Model.Interface;
using NLog;
using Count4U.Model.SelectionParams;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class SendDataOffice
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        private readonly IZip _zip;
        private readonly IReportSaveProvider _reportSaveProvider;
        private readonly IReportRepository _reportRepository;

        public SendDataOffice(
            IDBSettings dbSettings,
            IZip zip,
            IReportSaveProvider reportSaveProvider,
            IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
            _reportSaveProvider = reportSaveProvider;
            _zip = zip;
            _dbSettings = dbSettings;
        }

        public void BuildZip(CBIState cbiState,
            Action<string> updateStatus,
            bool includeInventorSdf,
            bool includeEndOfInventoryFiles,
            string resultPathZipPath)
        {
            try
            {
                List<string> filesToZip = new List<string>();

                if (includeEndOfInventoryFiles)
                {
                    updateStatus(Localization.Resources.View_SendDataOffice_BuildingEndInventor);

                    List<string> exportFiles = BuildExportResultsPath(cbiState);
                    if (exportFiles != null)
                    {
                        foreach (string exportFile in exportFiles)
                        {
                            filesToZip.Add(exportFile);
                        }
                    }
                }

                List<string> reports = BuildReportsPath(cbiState, updateStatus);

                if (reports != null)
                {
                    foreach (string reportPath in reports)
                    {
                        filesToZip.Add(reportPath);
                    }
                }

                if (includeInventorSdf)
                {
                    string inventorSdfFullPath = BuildInventorSdfPath(cbiState);
                    if (File.Exists(inventorSdfFullPath))
                    {
                        filesToZip.Add(inventorSdfFullPath);
                    }
                }

                updateStatus(Localization.Resources.View_SendDataOffice_BuildingZip);

                if (File.Exists(resultPathZipPath))
                {
                    _logger.Info(String.Format("Removed previous zip: {0}", resultPathZipPath));
                    File.Delete(resultPathZipPath);
                }

                _zip.CreateZip(filesToZip, resultPathZipPath);
            }

            catch (Exception exc)
            {
                _logger.ErrorException("BuildZip", exc);
            }
        }

        private List<string> BuildReportsPath(CBIState cbiState, Action<string> updateStatus)
        {
            List<string> result = new List<string>();

            List<ReportInfo> reportInfo = new List<ReportInfo>();
            reportInfo.Add(new ReportInfo() { ReportCode = "[Rep-IS1-03]", Format = ReportFileFormat.Pdf });
            reportInfo.Add(new ReportInfo() { ReportCode = "[Rep-IS1-02]", Format = ReportFileFormat.Pdf });
            reportInfo.Add(new ReportInfo() { ReportCode = "[Rep-IS1-02]", Format = ReportFileFormat.Excel });

            string tempPath = Path.GetTempPath();
            if (!Directory.Exists(tempPath))
            {
                _logger.Warn("Temp path missing");
                return null;
            }

            string tempDir = Path.Combine(tempPath, "Count4U");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            foreach (ReportInfo info in reportInfo)
            {
                updateStatus(String.Format(Localization.Resources.View_SendDataOffice_GeneratingReport, info.ReportCode, info.Format));

                GenerateReportArgs generateArgs = new GenerateReportArgs();
                generateArgs.Customer = cbiState.CurrentCustomer;
                generateArgs.Branch = cbiState.CurrentBranch;
                generateArgs.Inventor = cbiState.CurrentInventor;
                generateArgs.DbPath = cbiState.GetDbPath;
				generateArgs.SelectParams = new SelectParams();
				generateArgs.SelectParams.IsEnablePaging = false;
                generateArgs.ViewDomainContextType = ViewDomainContextEnum.Iturs;

                Count4U.GenerationReport.Report report = null;
                Reports reports = _reportRepository.GetReportByCodeReport(info.ReportCode);
                if (reports != null)
                {
                    report = reports.FirstOrDefault();
                }

                if (report == null) continue;

                generateArgs.ReportTemplateFileName = report.FileName;
                generateArgs.Path = report.Path;

                string resultReportPath = Path.Combine(tempDir, report.FileName);

                if (File.Exists(resultReportPath))
                {
                    File.Delete(resultReportPath);
                }

                string generatedReportPath = _reportSaveProvider.Save(generateArgs, resultReportPath, info.Format);

                if (result.Contains(generatedReportPath))
                {
                    _logger.Warn("Result already contains: {0}", generatedReportPath);
                }
                else
                {
                    result.Add(generatedReportPath);
                }
            }

            return result;
        }

        private string BuildInventorSdfPath(CBIState cbiState)
        {
            string inventorRelative = cbiState.ContextCBIRepository.GetDBPath(cbiState.CurrentInventor);
            string inventorSdfFullPath = this._dbSettings.BuildCount4UDBFilePath(inventorRelative);

            FileInfo fi = new FileInfo(inventorSdfFullPath);
            inventorSdfFullPath = fi.FullName;

            return inventorSdfFullPath;
        }

        private class ReportInfo
        {
            public string ReportCode { get; set; }
            public string Format { get; set; }
        }

        private List<string> BuildExportResultsPath(CBIState cbiState)
        {
            List<string> result = new List<string>();

            string exportErpFolder = UtilsMisc.BuildPathToExportErpFolder(_dbSettings, cbiState.CurrentInventor.Code);

            if (!Directory.Exists(exportErpFolder))
                return null;

            foreach (string file in Directory.GetFiles(exportErpFolder))
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension.ToLower() != ".log")
                {
                    result.Add(file);
                }
            }

            return result;
        }
    }
}