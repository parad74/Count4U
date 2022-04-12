using Count4U.Model;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events.Misc
{
    public class ObjectPropertiesViewEvent : CompositePresentationEvent<ObjectPropertiesViewEventPayload>
    {
         
    }

    public class ObjectPropertiesViewEventPayload
    {
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}