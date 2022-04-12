using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Common.Constants;

namespace Count4U.GenerationReport
{
    public interface IReportRepository
    {
		Reports GetReports(string code, string fileName, string path, string allowedReportTemplate);
		Reports GetReportByCodeReport(string codeReport, AllowedReportTemplate allowedReportTemplate = AllowedReportTemplate.Main);
		void ClearTag(AllowedReportTemplate allowedReportTemplate = AllowedReportTemplate.Main);
		//Reports GetReports(SelectParams selectParams);
		Dictionary<string, string> GetDomainContextDataSetDictionary();
		Dictionary<string, ViewDomainContextEnum> GetViewDomainContextEnumDictionary();
		Dictionary<ViewDomainContextEnum, string> GetViewDomainContextStartPathDictionary();
		Reports GetAllowedReportTemplate(string customerCode, string branchCode,
			string inventorCode, ViewDomainContextEnum viewDomainContextEnum,
			List<AllowedReportTemplate> allowedReportTemplates);
		Reports GetAllowedReportTemplateTag(string customerCode, string branchCode,
			string inventorCode, ViewDomainContextEnum domainContextType,
			List<AllowedReportTemplate> allowedReportTemplates);
		bool IsAllowedSelectParmReport(ViewDomainContextEnum domainContextType,
			string reportTemplatePath);
		bool IsSearchView(ViewDomainContextEnum clickOnView);
		Count4U.GenerationReport.Report GetReportFastPrint(ViewDomainContextEnum viewDomainContextType, string customerCode, string branchCode, string inventorCode);
		Count4U.GenerationReport.Report GetReportFastPrintByReportCode(string reportCode);

		void Insert(Report report, AllowedReportTemplate allowedReportTemplate);
		void Update(Report report, AllowedReportTemplate allowedReportTemplate);
		void Delete(Report report, AllowedReportTemplate allowedReportTemplate);

    }
}
