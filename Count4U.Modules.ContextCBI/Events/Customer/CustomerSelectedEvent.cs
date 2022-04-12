//using Microsoft.Practices.Composite.Events;

using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    class CustomerSelectedEvent : CompositePresentationEvent<CustomerSelectedEventPayload>
    {
    }

    public class CustomerSelectedEventPayload
    {
        public Customer Customer { get; set; }
        public CBIContext Context { get; set; }
    }
}
