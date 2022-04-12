using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using Count4U.GenerationReport;

namespace Count4U.Model.Interface
{
	public interface IReportSaveProvider 
	{
		string Save(GenerateReportArgs args, string outputPath, string outputFormat, ReportInfo info = null);
		void Delete();
		Dictionary<ImportProviderParmEnum, object> Parms { get; set; }
	}
}
