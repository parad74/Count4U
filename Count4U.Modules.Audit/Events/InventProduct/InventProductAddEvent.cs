using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class InventProductAddEvent : CompositePresentationEvent<InventProductAddEventPayload>
    {
         
    }

    public class InventProductAddEventPayload
    {       
        public string DocumentCode { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}