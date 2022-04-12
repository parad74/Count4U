using Count4U.Model.Audit;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class LastInventorsListItem : NotificationObject
    {
        private readonly AuditConfig _auditConfig;

        public LastInventorsListItem(AuditConfig audit)
        {            
            this._auditConfig = audit;

            this.Status = audit.StatusAuditConfig;
            this.Date = audit.InventorDate.ToShortDateString();
			//this.CompleteDate = audit.CompleteDate.ToShortDateString();
            this.Customer = audit.CustomerName;
            this.Branch = audit.BranchName;
            this.Inventor = audit.InventorName;
			this.Description = audit.Description;  
        }
        public string Status { get; set; }
        public string Date { get; set; }
		public string CompleteDate { get; set; }
		
		public string Description { get; set; }
        public string Customer { get; set; }
        public string Branch { get; set; }
        public string Inventor { get; set; }

        public AuditConfig AuditConfig
        {
            get { return this._auditConfig; }
        }
    }
}