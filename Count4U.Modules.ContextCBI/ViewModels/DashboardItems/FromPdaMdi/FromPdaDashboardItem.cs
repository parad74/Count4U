using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.FromPda
{
    public class FromPdaDashboardItem : NotificationObject
    {
        public string CreateDate { get; set; }
        public string CountDocument { get; set; }
        public string CountItem { get; set; }
    }
}