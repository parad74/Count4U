using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class DocumentHeaderAddEditEvent : CompositePresentationEvent<DocumentHeaderAddEditEventPayload>
    {
         
    }

    public class DocumentHeaderAddEditEventPayload
    {
        public DocumentHeader DocumentHeader { get; set; }
        public string IturCode { get; set; }
		public string DeviceCode { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}