using System.Collections.Generic;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items;
using Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events.Misc
{
    public class MdiFilterChangedEvent : CompositePresentationEvent<MdiFilterState>
    {

    }
}