using Count4U.Model;
using Microsoft.Practices.Prism.Events;
using Count4U.Model.Audit;

namespace Count4U.Modules.ContextCBI.Events
{
    public class InventorSelectedEvent : CompositePresentationEvent<InventorSelectedEventPayload>
    {
    }

    public class InventorSelectedEventPayload
    {
        public Inventor Inventor { get; set; }
        public CBIContext Context { get; set; }
    }
}
