using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class DocumentHeaderAddedEditedEvent : CompositePresentationEvent<DocumentHeaderAddedEditedEventPayload>
    {
         
    }

    public class DocumentHeaderAddedEditedEventPayload
    {
        public DocumentHeader DocumentHeader { get; set; }
        public bool IsNew { get; set; }
    }

}