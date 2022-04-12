using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
	public class IturQuantityEditChangedEvent : CompositePresentationEvent<IturQuantityEditChangedEventPayload>
    {
         
    }

    public class IturQuantityEditChangedEventPayload
    {
  //      public Iturs Iturs { get; set; }
		//public string Name { get; set; }
		//public string ERPCode { get; set; }
       // public Location Location { get; set; }
    }
}