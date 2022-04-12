using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class ReportPrintItemViewModel : NotificationObject
    {
        public string ReportCode { get; set; }
        public string FileFormat { get; set; }        
        public bool Include { get; set; }
        public bool Print { get; set; }
		public bool Print2 { get; set; }
        public string ReportName { get; set; }
		public bool Landscape { get; set; }
		public string SelectReportBy { get; set; }
		public bool RefillAlways { get; set; }

		public ReportPrintItemViewModel()
		{
			RefillAlways = false;
		}
    }

		
}