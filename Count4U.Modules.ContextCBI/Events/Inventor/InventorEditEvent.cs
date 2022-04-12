using Count4U.Model;
using Count4U.Model.Audit;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class InventorEditEvent : CompositePresentationEvent<InventorEditEventPayload>
    {

    }

    public class InventorEditEventPayload
    {
        public Inventor Inventor { get; set; }
        public CBIContext Context { get; set; }
    }
}