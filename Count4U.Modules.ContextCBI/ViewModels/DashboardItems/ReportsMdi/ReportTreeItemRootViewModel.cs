using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Reports
{
    public class ReportTreeItemRootViewModel : NotificationObject
    {
        private readonly ObservableCollection<ReportTreeItemViewModel> _children;
        protected string _header;

        public ReportTreeItemRootViewModel()
        {
            _children = new ObservableCollection<ReportTreeItemViewModel>();
        }

        public ObservableCollection<ReportTreeItemViewModel> Children
        {
            get { return _children; }
        }

        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

    }
}