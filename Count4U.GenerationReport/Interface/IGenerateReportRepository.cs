using System.Collections.Generic;
namespace Count4U.GenerationReport
{
    public interface IGenerateReportRepository
    {
        void GenerateReport(GenerateReportArgs args);
		void RunPrintReport(GenerateReportArgs args, bool clearIturAnalysisAfterPrint = true);
		string RunSaveReport(GenerateReportArgs args, string outpuFilePath, string reportFileFormat, ReportInfo info = null);
        string BuildReportFullPath(string path, string reportFileName);
        string GetLocalizedReportName(Count4U.GenerationReport.Report report);
		List<Microsoft.Reporting.WinForms.ReportDataSource> FillReportDSList(GenerateReportArgs args);
    }
}