using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Reports
{
    public class ReportTreeItemViewModel : ReportTreeItemRootViewModel
    {
        private string _description;
        private readonly GenerationReport.Report _report;

        public ReportTreeItemViewModel(GenerationReport.Report report)
            :base()
        {
            _report = report;
			if (string.IsNullOrWhiteSpace(Report.MenuCaption) == false)
			{
				_header = Report.MenuCaption;
			}
			else if (string.IsNullOrWhiteSpace(Report.Description) == false)
			{
				_header = Report.Description;
			}
			else
			{
				_header = Report.FileName;
			}
            //_header = Report.FileName;
            _description = Report.Description;
        }        

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DelegateCommand<ReportTreeItemViewModel> GenerateCommand { get; set; }

        public GenerationReport.Report Report
        {
            get { return _report; }
        }
    }
}