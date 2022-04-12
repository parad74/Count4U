using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class ProductAddedEditedEvent : CompositePresentationEvent<ProductAddedEditedEventPayload>
    {

      
    }

    public class ProductAddedEditedEventPayload
    {
        public bool IsNew { get; set; }
        public Product Product { get; set; }
    } 
}