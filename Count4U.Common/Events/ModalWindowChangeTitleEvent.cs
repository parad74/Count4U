using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
    public class ModalWindowChangeTitleEvent : CompositePresentationEvent<ModalWindowChangeTitleEventPayload>
    {
         
    }

    public class ModalWindowChangeTitleEventPayload
    {
        public object ViewModel { get; set; }
        public string Title { get; set; }
    }
}