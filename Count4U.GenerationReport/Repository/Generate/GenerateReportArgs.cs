using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Common.Constants;

namespace Count4U.GenerationReport
{
	public class GenerateReportArgs
	{
		public Count4U.GenerationReport.Report Report { get; set; }
		//public string Path { get; set; }
		//public string ReportTemplateFileName { get; set; }
		public ViewDomainContextEnum ViewDomainContextType { get; set; }
		public Location Location { get; set; }
		public Itur Itur { get; set; }
		public DocumentHeader Doc { get; set; }
		public Device Device { get; set; }
		public SelectParams SelectParams { get; set; }

		public string DbPath { get; set; }

		public Customer Customer { get; set; }
		public Branch Branch { get; set; }
		public Inventor Inventor { get; set; }

		//public bool Landscape { get; set; }
		public IFilterData FilterData { get; set; }

		public string PrinterName { get; set; }
		
		public bool RefillReportDS { get; set; }
		public bool SaveReportToSendOffice { get; set; }

		public string Param1 { get; set; }
		public string Param2 { get; set; }
		public string Param3 { get; set; }

		public GenerateReportArgs()
		{
			RefillReportDS = false;
			SaveReportToSendOffice = false;
			PrinterName = "";
			Param1 = "";
			Param2 = "";
			Param3 = "";
		}
	}
}