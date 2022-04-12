using Count4U.Model;
using Count4U.Model.Audit;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class InventorStatusChangeEvent : CompositePresentationEvent<InventorStatusChangeEventPayload>
    {
         
    }

    public class InventorStatusChangeEventPayload
    {
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}