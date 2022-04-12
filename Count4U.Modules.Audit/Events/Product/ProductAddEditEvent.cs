using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class ProductAddEditEvent : CompositePresentationEvent<ProductAddEditEventPayload>
    {
         
    }

    public class ProductAddEditEventPayload
    {        
        public Product Product { get; set; }        
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }

    }
}