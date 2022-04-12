using Count4U.Model;
using Count4U.Model.Audit;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class InventorViewEvent : CompositePresentationEvent<InventorViewEventPayload>
    {
        
    }

    public class InventorViewEventPayload
    {
        public Inventor Inventor { get; set; }
        public CBIContext Context { get; set; }
    }
}