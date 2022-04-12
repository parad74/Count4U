//using Count4U.Model.Services;

using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
    public class BranchSelectedEvent : CompositePresentationEvent<BranchSelectedEventPayload>
    {
    }

    public class BranchSelectedEventPayload
    {
        public Branch Branch { get; set; }
        public CBIContext Context { get; set; }
    }
}
