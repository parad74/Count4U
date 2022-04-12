using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class BranchAddEvent : CompositePresentationEvent<BranchAddEventPayload>
    {
        
    }

    public class BranchAddEventPayload
    {
        public Branch Branch { get; set; }
        public CBIContext Context { get; set; }
        public bool IsCustomerComboVisible { get; set; }
    }
}