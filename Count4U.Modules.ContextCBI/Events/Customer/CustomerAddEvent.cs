using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class CustomerAddEvent : CompositePresentationEvent<CustomerAddEventPayload>
    {
        
    }

    public class CustomerAddEventPayload
    {
        public Customer Customer { get; set; }
        public CBIContext Context { get; set; }
    }
}