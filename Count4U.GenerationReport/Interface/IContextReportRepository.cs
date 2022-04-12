using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Main;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using System.Globalization;
using Count4U.Common.Constants;

namespace Count4U.GenerationReport
{
	public interface IContextReportRepository
    {
		ContextReport GetNewContextReport();
		void InitContextReport();
		void InitContextReport(Dictionary<FilterAndSortEnum, string> dictionaryFilterAndSort);
		void InitContextReport(Customer customer);
		void InitContextReport(Branch baranch);
		void InitContextReport(Inventor inventor);
		void InitContextReport(Itur itur);
		void InitContextReport(Location location);
		void InitContextReport(DocumentHeader doc);
		void InitContextReport(string pathDB, Device device);
		void InitContextReport(string pathDB, string param1, string param2, string param3, string reportCode = "");
		ContextReports GetContextReports();
		ContextReports GetIturListContextReports(string pathDB, ContextReport contextReport = null);
		void Clear();
		DateTimeFormatInfo Dtfi { get; set; }

	}
}
