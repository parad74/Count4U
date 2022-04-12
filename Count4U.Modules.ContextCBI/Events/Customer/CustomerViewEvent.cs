using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class CustomerViewEvent : CompositePresentationEvent<CustomerViewEventPayload>
    {
        
    }

    public class CustomerViewEventPayload
    {
        public Customer Customer { get; set; }
        public CBIContext Context { get; set; }
    }
}