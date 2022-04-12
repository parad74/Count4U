using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
    public class SpecialKeyEvent : CompositePresentationEvent<SpecialKey>
    {
    }

    public enum SpecialKey
    {
        Back,
        Forward
    }
}