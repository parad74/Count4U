using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using Count4U.GenerationReport;

namespace Count4U.Model.Interface
{
	public interface IReportPrintProvider 
	{
		void Print(GenerateReportArgs args);
		Dictionary<ImportProviderParmEnum, object> Parms { get; set; }
	}
}
