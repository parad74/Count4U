using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class BranchEditEvent : CompositePresentationEvent<BranchEditEventPayload>
    {

    }

    public class BranchEditEventPayload
    {
        public Branch Branch { get; set; }
        public CBIContext Context { get; set; }
    }
}