using Count4U.Model;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events.ParsingMask
{
    public class MaskAddEditEvent : CompositePresentationEvent<MaskAddEditEventPayload>
    {
         
    }

    public class MaskAddEditEventPayload
    {
        public Mask Mask { get; set; }
        public string CBIDbContext { get; set; }
        public CBIContext CDBContext { get; set; }
    }
}