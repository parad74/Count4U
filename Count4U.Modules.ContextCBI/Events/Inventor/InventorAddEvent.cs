using Count4U.Model;
using Count4U.Model.Audit;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class InventorAddEvent : CompositePresentationEvent<InventorAddEventPayload>
    {
        
    }

    public class InventorAddEventPayload
    {
        public Inventor Inventor { get; set; }
        public bool IsCustomerComboVisible { get; set; }
        public bool IsBranchComboVisible { get; set; }
        public CBIContext Context { get; set; }
		public bool WithoutNavigate { get; set; }
    }
}